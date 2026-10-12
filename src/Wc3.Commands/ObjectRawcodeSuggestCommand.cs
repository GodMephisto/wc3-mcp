// src/Wc3.Commands/ObjectRawcodeSuggestCommand.cs
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record ObjectRawcodeSuggestResult(
    string Kind, int Count, IReadOnlyList<string> Rawcodes, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Proposes free four-character rawcodes that collide with neither the map's objects
/// nor the base game data for the kind. Read-only. Without a game directory the
/// suggestions are checked against the map's objects only, and a diagnostic says so,
/// because copying a base-game rawcode would silently shadow a standard object.
/// </summary>
public static class ObjectRawcodeSuggestCommand
{
    private const string NoBaseDataNote =
        "no game data, so suggested codes were checked against the map's own objects only, "
        + "not against base game data. Point --game-dir at a Warcraft III install for that.";

    public static ObjectRawcodeSuggestResult Execute(
        MapDocument doc, ObjectKind kind, int count, string? gameDirOverride)
    {
        bool opened = GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _);
        var diagnostics = new List<string>();
        if (!opened || ctx is null)
        {
            diagnostics.Add(NoBaseDataNote);
            ctx = null;
        }
        return Execute(doc, kind, count, ctx, diagnostics);
    }

    internal static ObjectRawcodeSuggestResult Execute(
        MapDocument doc, ObjectKind kind, int count, GameDataContext? ctx,
        IReadOnlyList<string> diagnostics)
    {
        string kindName = kind.ToString().ToLowerInvariant();
        if (count <= 0)
            return new ObjectRawcodeSuggestResult(
                kindName, 0, Array.Empty<string>(), diagnostics);

        var used = RawcodeAllocator.UsedRawcodes(doc);
        if (ctx is not null)
            foreach (var rc in BaseRawcodes(ctx, kind))
                if (rc.Length == 4) used.Add(rc.FromRawcode());

        string seed = SeedFor(kind);
        var codes = new List<string>(count);
        var taken = new HashSet<int>(used);
        for (int i = 0; i < count; i++)
        {
            int code = RawcodeAllocator.Allocate(seed, taken);
            taken.Add(code);
            codes.Add(code.ToRawcode());
        }
        return new ObjectRawcodeSuggestResult(kindName, codes.Count, codes, diagnostics);
    }

    internal static IEnumerable<string> BaseRawcodes(GameDataContext ctx, ObjectKind kind) => kind switch
    {
        ObjectKind.Unit => ctx.Units.Rawcodes,
        ObjectKind.Ability => ctx.Abilities.Rawcodes,
        ObjectKind.Item => ctx.Items.Rawcodes,
        ObjectKind.Destructable => ctx.Destructables.Rawcodes,
        ObjectKind.Doodad => ctx.Doodads.Rawcodes,
        ObjectKind.Buff => ctx.Buffs.Rawcodes,
        ObjectKind.Upgrade => ctx.Upgrades.Rawcodes,
        _ => Enumerable.Empty<string>(),
    };

    // Keeping the leading character the editor convention uses for each kind, so a
    // suggestion looks like a rawcode a person would have typed there themselves.
    private static string SeedFor(ObjectKind kind) => kind switch
    {
        ObjectKind.Ability => "A000",
        ObjectKind.Item => "I000",
        ObjectKind.Destructable => "B000",
        ObjectKind.Doodad => "D000",
        ObjectKind.Buff => "B000",
        ObjectKind.Upgrade => "R000",
        _ => "u000",
    };
}
