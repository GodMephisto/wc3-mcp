// src/Wc3.Commands/UabiProfileCommand.cs
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>uabi totals for one race, as the map's effective unit data states it.</summary>
public sealed record UabiRaceSlice(string Race, int UnitTypes, int References, int Distinct);

/// <summary>
/// Everything about a map's unit ability lists (uabi) that could separate a map that disconnects
/// from one that does not. One row per map, so a bisection's variants can be compared side by side.
/// </summary>
public sealed record UabiProfile(
    string Map,
    int UnitTypes,
    int WithUabi,
    int References,
    int Distinct,
    int AbilityObjects,
    int HeroAbilityRefs,
    int HeroAbilityDistinct,
    int HeroAbilityOnNonHeroUnits,
    int ItemAbilityRefs,
    int DanglingRefs,
    int DuplicateWithinList,
    int AlsoInSomeUhab,
    IReadOnlyList<UabiRaceSlice> Races,
    string? Problem);

public sealed record UabiProfileReport(IReadOnlyList<UabiProfile> Maps, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Measures, for any number of maps at once and without starting the game, the properties of
/// their uabi lists. Read-only.
///
/// Why. A disconnect can only be observed with two running clients, but the maps a bisection
/// produces differ in their data, and those differences can be measured offline. Once each
/// variant's row is known, one round of multiplayer results says which property matters.
/// The GGGA bisection in uabi-disconnect\maps marks the custom Human units' lists as the part
/// whose removal made the map host cleanly, so a property present there and absent in the
/// other races' lists is the candidate.
///
/// Properties. Plain counts. A HERO ability (aher 1) placed in a normal list, and in particular
/// on a unit that is not a hero, which the editor does not produce through its own UI. An ITEM
/// ability (aite 1) in a unit list. A reference to an ability neither the map nor the game
/// defines. The same id twice in one list. An id that is also some unit's learnable hero
/// ability. Each is effective data, the map layer overlaid by the skin layer, with a field the
/// map does not set taken from the installed game.
/// </summary>
public static class UabiProfileCommand
{
    public static UabiProfileReport Execute(IEnumerable<string> paths, string? gameDirOverride = null)
    {
        var diagnostics = new List<string>();
        GameData.GameDataContext? game = null;
        if (GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diag)) game = ctx;
        else diagnostics.Add($"game data unavailable ({diag}), so hero, item, race and dangling "
            + "properties are counted from the map's own fields only");

        var files = paths.SelectMany(p => Directory.Exists(p)
                ? Directory.EnumerateFiles(p, "*.w3?", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".w3x", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".w3m", StringComparison.OrdinalIgnoreCase))
                : new[] { p })
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

        var maps = new List<UabiProfile>();
        foreach (var f in files)
        {
            try { maps.Add(Profile(Path.GetFileName(f), MapDocument.Load(f), game)); }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                maps.Add(new UabiProfile(Path.GetFileName(f), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    Array.Empty<UabiRaceSlice>(), e.Message));
            }
        }
        return new UabiProfileReport(maps, diagnostics);
    }

    internal static UabiProfile Profile(string name, MapDocument doc, GameData.GameDataContext? game)
    {
        var abilities = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability))
            .ToDictionary(e => e.Id.ToRawcode(), e => (Base: e.OldId.ToRawcode(), Mods: ObjectKinds.ModsToDict(e.Mods)),
                StringComparer.Ordinal);
        var units = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit));

        // A flag an ability carries, the map's value first, then its base's in the installed game.
        string? AbilityField(string id, string code)
        {
            if (abilities.TryGetValue(id, out var a))
            {
                if (a.Mods.TryGetValue(code, out var v)) return v;
                id = a.Base;
            }
            // The base table names these columns hero and item, the map's object data aher and aite.
            string column = code switch { "aher" => "hero", "aite" => "item", _ => code };
            return game is not null && game.Abilities.TryGetAbility(id, out var f)
                && (f.TryGetValue(code, out var bv) || f.TryGetValue(column, out bv))
                ? bv : null;
        }
        bool Defined(string id) => abilities.ContainsKey(id)
            || (game is not null && game.Abilities.TryGetAbility(id, out _));
        bool Flag(string id, string code) => AbilityField(id, code) is "1";

        var uhabAll = new HashSet<string>(StringComparer.Ordinal);
        foreach (var u in units)
            if (ObjectKinds.ModsToDict(u.Mods).TryGetValue("uhab", out var h))
                uhabAll.UnionWith(Ids(h));

        int withUabi = 0, refs = 0, heroRefs = 0, heroOnNonHero = 0, itemRefs = 0, dangling = 0, dupes = 0, inUhab = 0;
        var distinct = new HashSet<string>(StringComparer.Ordinal);
        var heroDistinct = new HashSet<string>(StringComparer.Ordinal);
        var byRace = new Dictionary<string, (int Units, int Refs, HashSet<string> Ids)>(StringComparer.OrdinalIgnoreCase);

        foreach (var u in units)
        {
            var mods = ObjectKinds.ModsToDict(u.Mods);
            if (!mods.TryGetValue("uabi", out var list)) continue;
            var ids = Ids(list).ToList();
            if (ids.Count == 0) continue;
            withUabi++;
            refs += ids.Count;
            distinct.UnionWith(ids);
            if (ids.Count != ids.Distinct(StringComparer.Ordinal).Count()) dupes++;

            string raw = u.Id.ToRawcode();
            bool isHero = char.IsUpper(raw[0]);     // the engine's own rule, a hero id starts uppercase
            foreach (var id in ids)
            {
                if (!Defined(id)) { dangling++; continue; }
                if (Flag(id, "aher"))
                {
                    heroRefs++; heroDistinct.Add(id);
                    if (!isHero) heroOnNonHero++;
                }
                if (Flag(id, "aite")) itemRefs++;
                if (uhabAll.Contains(id)) inUhab++;
            }

            string race = mods.TryGetValue("urac", out var r) ? r
                : game is not null && game.Units.TryGetUnit(u.OldId.ToRawcode(), out var bu)
                    && (bu.TryGetValue("urac", out var br) || bu.TryGetValue("race", out br)) ? br
                : "unknown";
            if (!byRace.TryGetValue(race, out var slot)) slot = (0, 0, new HashSet<string>(StringComparer.Ordinal));
            slot.Ids.UnionWith(ids);
            byRace[race] = (slot.Units + 1, slot.Refs + ids.Count, slot.Ids);
        }

        var races = byRace.OrderByDescending(kv => kv.Value.Refs)
            .Select(kv => new UabiRaceSlice(kv.Key, kv.Value.Units, kv.Value.Refs, kv.Value.Ids.Count)).ToList();
        return new UabiProfile(name, units.Count, withUabi, refs, distinct.Count, abilities.Count,
            heroRefs, heroDistinct.Count, heroOnNonHero, itemRefs, dangling, dupes, inUhab, races, null);
    }

    // Every entry, whatever its length. Keeping only 4 character ids hid 'A0S4m' on H08N in the
    // GGGA maps, a typo the engine can never resolve, and undercounted dangling ids as 42 of 43.
    private static IEnumerable<string> Ids(string list) =>
        list.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0 && s != "_");
}
