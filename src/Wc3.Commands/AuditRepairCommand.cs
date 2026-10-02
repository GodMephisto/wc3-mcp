// src/Wc3.Commands/AuditRepairCommand.cs
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One field the repair changed, with the value before and after.</summary>
public sealed record AuditRepairEdit(string Kind, string Rawcode, string Field, string Before, string After, string Why);

public sealed record AuditRepairResult(
    bool Ok,
    string Message,
    int DanglingFound,
    int RequirementsFound,
    IReadOnlyList<AuditRepairEdit> Edits,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Fixes the two error-level audit findings that have one safe mechanical answer, using the
/// SAME detection the audit uses (<see cref="AuditCommand.DanglingReferences"/> and
/// <see cref="AuditCommand.InheritedRequirements"/>), so a clean audit afterwards is the proof.
///
/// A dangling ability entry in a unit or item list (uabi, uhab, iabi) is removed and every
/// other entry is kept verbatim, because the engine skips an id that exists nowhere and the
/// removal changes nothing a player can see. A dangling buff (abuf, aeff) gets back the value
/// its base ability has at that level, because an ability whose buff is missing shows no
/// effect and cannot be dispelled or detected. When the base has none, the entry is dropped.
/// An inherited requirement the map never defines gets an explicit empty value at level 0,
/// the 3.0.0 override recorded in docs/reforged-3.0.0-blizzard-bug-report.md, because
/// otherwise the ability can never be unlocked.
///
/// Every edit goes through <see cref="ObjectSetCommand"/>, so it lands in whichever object
/// layer holds the field, and nothing is saved here. The caller saves once.
/// </summary>
public static class AuditRepairCommand
{
    public static AuditRepairResult Execute(MapDocument doc, bool apply, string? gameDirOverride = null)
    {
        var diagnostics = new List<string>();
        if (!GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var why))
            return new(false, $"game data unavailable ({why}), and both checks need it", 0, 0,
                Array.Empty<AuditRepairEdit>(), diagnostics);

        var scan = AuditCommand.DanglingReferences(doc, ctx!);
        var abilityIds = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability)).Select(e => e.Id).ToList();
        var locked = AuditCommand.InheritedRequirements(doc, abilityIds, gameDirOverride);
        diagnostics.Add($"examined {scan.Examined} reference(s), {scan.Dangling.Count} dangling, "
            + $"{locked.Count} abilit(ies) with an unmet inherited requirement");

        var edits = new List<AuditRepairEdit>();
        var entries = ObjectKinds.All.ToDictionary(k => k,
            k => ObjectKinds.MergedEntries(doc, ObjectKinds.Info(k)).GroupBy(e => e.Id.ToRawcode())
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal));

        foreach (var field in scan.Dangling.GroupBy(d => (d.Kind, d.Rawcode, d.Key)))
        {
            var (kind, rawcode, key) = field.Key;
            var entry = entries[kind][rawcode];
            string before = entry.Mods.First(m => m.Key == key).Value ?? "";
            var dead = field.Select(d => d.Target).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var kept = before.Split(',').Where(s => !dead.Contains(s.Trim())).ToList();
            string after = string.Join(",", kept);
            string reason = $"removed {string.Join(", ", dead.Order())}, defined by neither the map nor the game";

            if (field.First().Names == "buff" && kept.All(s => s.Trim().Length == 0 || s.Trim() == "_")
                && BaseValue(ctx!, entry.OldId.ToRawcode(), key) is { } restored)
            {
                after = restored;
                reason = $"{string.Join(", ", dead.Order())} is defined nowhere, restored the base "
                    + $"ability's value '{restored}'";
            }
            edits.Add(new(kind.ToString().ToLowerInvariant(), rawcode, key, before, after, reason));
        }

        foreach (var r in locked)
            edits.Add(new("ability", r.Rawcode, "areq:0", r.Inherited, "_",
                $"inherited Requirements name {string.Join(", ", r.Unmet)}, which the map never defines"));

        if (apply)
            foreach (var e in edits)
            {
                var kind = Enum.Parse<ObjectKind>(e.Kind, ignoreCase: true);
                var set = ObjectSetCommand.Execute(doc, kind, e.Rawcode, e.Field, e.After);
                if (!set.Ok)
                    return new(false, $"{e.Kind} {e.Rawcode} {e.Field} failed, {set.Message}", scan.Dangling.Count,
                        locked.Count, edits, diagnostics);
            }

        int buffs = edits.Count(e => e.Why.Contains("restored", StringComparison.Ordinal));
        int lists = edits.Count(e => e.Why.StartsWith("removed", StringComparison.Ordinal));
        string message = edits.Count == 0
            ? "nothing to repair, no dangling reference and no unmet inherited requirement"
            : $"{(apply ? "changed" : "would change")} {edits.Count} field(s), {lists} list(s) cleaned, "
              + $"{buffs} buff value(s) restored from the base ability, {locked.Count} requirement(s) cleared";
        return new(true, message, scan.Dangling.Count, locked.Count, edits, diagnostics);
    }

    /// <summary>
    /// The base ability's value for a field at the key's level. A base with fewer levels than
    /// the custom has no value past its last, so the highest level it defines is used, which is
    /// what the Object Editor fills new levels with.
    /// </summary>
    private static string? BaseValue(GameData.GameDataContext ctx, string baseRawcode, string key)
    {
        if (!ctx.Abilities.TryGetAbility(baseRawcode, out var fields)) return null;
        string code = key.Split(':')[0];
        int level = key.Contains(':') ? int.Parse(key[(code.Length + 1)..]) : 0;
        if (fields.TryGetValue(key, out var exact) && Usable(exact)) return exact;
        if (fields.TryGetValue(code, out var bare) && Usable(bare)) return bare;
        for (int l = Math.Max(level, 1); l >= 1; l--)
            if (fields.TryGetValue($"{code}:{l}", out var v) && Usable(v)) return v;
        for (int l = level + 1; l <= 200; l++)
            if (fields.TryGetValue($"{code}:{l}", out var v) && Usable(v)) return v;
        return null;
    }

    private static bool Usable(string v) => v.Trim().Length > 0 && v.Trim() != "_";
}
