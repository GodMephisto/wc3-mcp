// src/Wc3.GameData/GameData.cs
using System.Collections.Concurrent;

namespace Wc3.GameData;

/// <summary>
/// One-call facade: locate the install, open CASC, build the base-game resolvers.
/// Never throws — callers degrade gracefully on false + diagnostic.
/// </summary>
public static class GameData
{
    private const string WorldEditStringsPath = @"war3.w3mod:_locales\enus.w3mod:ui\worldeditstrings.txt";
    // Object names for destructables/doodads (WESTRING_DEST_* / WESTRING_DOODAD_*) live in
    // this parallel file, not worldeditstrings.txt, so both are merged into one Strings table.
    private const string WorldEditGameStringsPath = @"war3.w3mod:_locales\enus.w3mod:ui\worldeditgamestrings.txt";

    // Base game data (SLK tables, editor strings) is assumed stable for the process
    // lifetime, so successful builds are cached per resolved install directory and
    // reused across object queries. Failures are never cached.
    private static readonly ConcurrentDictionary<string, GameDataContext> ContextCache = new();

    /// <summary>Build the resolvers for all seven Object Editor types plus editor strings
    /// and unit names from a single CASC open, caching the result per install for the
    /// process lifetime. The source is disposed before returning; the context holds only
    /// parsed data.</summary>
    public static bool TryOpen(string? gameDirOverride, out GameDataContext? ctx, out string diagnostic)
    {
        ctx = null; diagnostic = "";
        var dir = GameInstall.Locate(gameDirOverride);
        if (dir is null) { diagnostic = "Warcraft III install not found (pass --game-dir <path>)"; return false; }
        if (ContextCache.TryGetValue(dir, out var cached)) { ctx = cached; return true; }
        if (!CascGameDataSource.TryOpen(dir, out var src, out var err))
        { diagnostic = $"could not open game data at {dir}: {err}"; return false; }
        // The builders copy all bytes into parsed tables, so the source is safe to
        // dispose as soon as they return — success or failure.
        try
        {
            // The five newer types degrade individually: a missing/corrupt SLK leaves
            // that store Empty with a diagnostic rather than failing the whole open.
            var diags = new List<string>();
            ObjectDataStore BuildSafe(string type, Func<IGameDataSource, ObjectDataStore> build)
            {
                try { return build(src!); }
                catch (Exception ex) { diags.Add($"could not read base {type} data: {ex.Message}"); return ObjectDataStore.Empty; }
            }

            var strings = WorldEditStrings.FromByteSources(
                src!.ReadFile(WorldEditStringsPath),
                src!.ReadFile(WorldEditGameStringsPath));
            // Profile TXT stores fill the fields the metadata SLKs mark "Profile"
            // (names, tooltips, art refs). Buff sections live inside the ability
            // profile files, so those two kinds share one store.
            var itemProfile = ProfileTxtStore.BuildItems(src!, strings);
            var abilityProfile = ProfileTxtStore.BuildAbilities(src!, strings);
            var upgradeProfile = ProfileTxtStore.BuildUpgrades(src!, strings);
            ctx = new GameDataContext
            {
                Units = BaseUnitStore.Build(src!),
                Abilities = BaseAbilityStore.Build(src!, abilityProfile),
                Items = BuildSafe("item", s => ObjectDataStore.BuildItems(s, itemProfile)),
                Destructables = BuildSafe("destructable", ObjectDataStore.BuildDestructables),
                Doodads = BuildSafe("doodad", ObjectDataStore.BuildDoodads),
                Buffs = BuildSafe("buff", s => ObjectDataStore.BuildBuffs(s, abilityProfile)),
                Upgrades = BuildSafe("upgrade", s => ObjectDataStore.BuildUpgrades(s, upgradeProfile)),
                Strings = strings,
                UnitNames = UnitNameTable.FromSources(src!),
                Diagnostics = diags,
                InstallDir = dir, // lets TryReadFile lazily re-open CASC for raw assets
            };
            // Concurrent builders may race; whichever lands first wins and both
            // contexts are equivalent, so GetOrAdd keeps callers consistent.
            ctx = ContextCache.GetOrAdd(dir, ctx);
            return true;
        }
        catch (Exception ex) { diagnostic = $"could not read base game data: {ex.Message}"; return false; }
        finally { src!.Dispose(); }
    }

    public static bool TryOpenUnits(string? gameDirOverride, out BaseUnitStore? store, out string diagnostic)
    {
        store = null; diagnostic = "";
        var dir = GameInstall.Locate(gameDirOverride);
        if (dir is null) { diagnostic = "Warcraft III install not found (pass --game-dir <path>)"; return false; }
        if (!CascGameDataSource.TryOpen(dir, out var src, out var err))
        { diagnostic = $"could not open game data at {dir}: {err}"; return false; }
        // Build copies all SLK bytes into parsed tables, so the source is safe to
        // dispose as soon as it returns — success or failure.
        try { store = BaseUnitStore.Build(src!); return true; }
        catch (Exception ex) { diagnostic = $"could not read base unit data: {ex.Message}"; return false; }
        finally { src!.Dispose(); }
    }
}
