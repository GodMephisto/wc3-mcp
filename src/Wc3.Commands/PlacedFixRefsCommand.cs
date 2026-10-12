// src/Wc3.Commands/PlacedFixRefsCommand.cs
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One repair, the placement touched, the rawcode it carried, the action taken
/// ("dropped" or "rewrote") and the replacement rawcode when one was written.</summary>
public sealed record FixRefChange(
    string File, string Kind, int CreationNumber, string Rawcode,
    string Action, string? ToRawcode, float X, float Y);

/// <summary>Outcome of placed fix-refs, with each change so a caller can report them.</summary>
public sealed record PlacedFixRefsResult(
    bool Ok, string Message, int Changed, IReadOnlyList<FixRefChange> Changes);

/// <summary>
/// Repairs placements that reference a missing rawcode. With a replacement rawcode every
/// such placement is rewritten to it, otherwise the placements are dropped. Refuses (and
/// writes nothing) when no placement references a missing rawcode, so an accidental run on
/// a healthy map is a no-op rather than a rewrite. The map is mutated in memory only, the
/// caller Saves the edited copy.
/// </summary>
public static class PlacedFixRefsCommand
{
    public static PlacedFixRefsResult Execute(
        MapDocument doc, string? gameDir = null, string? toRawcode = null)
    {
        ArgumentNullException.ThrowIfNull(doc);

        bool rewrite = toRawcode is not null;
        if (rewrite && toRawcode!.Length != 4)
            return Fail($"invalid replacement rawcode '{toRawcode}', expected 4 characters");

        var missing = PlacedMissingRefsCommand.FindMissing(doc, gameDir);
        if (missing.Count == 0)
            return Fail("no placement references a missing rawcode; nothing to fix");

        int replacementId = rewrite ? toRawcode!.FromRawcode() : 0;
        string action = rewrite ? "rewrote" : "dropped";
        var changes = new List<FixRefChange>();

        if (doc.GetFile(PlacementCommand.UnitsFile)?.Model is MapUnits units)
        {
            bool touched = false;
            foreach (var m in missing.Where(m => m.File == PlacementCommand.UnitsFile))
            {
                var unit = units.Units.FirstOrDefault(u => u.CreationNumber == m.CreationNumber);
                if (unit is null) continue;
                if (rewrite) unit.TypeId = replacementId;
                else units.Units.Remove(unit);
                touched = true;
                changes.Add(new(m.File, m.Kind, m.CreationNumber, m.Rawcode, action, toRawcode, m.X, m.Y));
            }
            if (touched) PlacementCommand.CommitUnits(doc, units);
        }

        if (doc.GetFile(PlacementCommand.DoodadsFile)?.Model is MapDoodads doodads)
        {
            bool touched = false;
            foreach (var m in missing.Where(m => m.File == PlacementCommand.DoodadsFile))
            {
                var doodad = doodads.Doodads.FirstOrDefault(d => d.CreationNumber == m.CreationNumber);
                if (doodad is null) continue;
                if (rewrite) doodad.TypeId = replacementId;
                else doodads.Doodads.Remove(doodad);
                touched = true;
                changes.Add(new(m.File, m.Kind, m.CreationNumber, m.Rawcode, action, toRawcode, m.X, m.Y));
            }
            if (touched) doc.AddOrReplaceModelFile(PlacementCommand.DoodadsFile, doodads);
        }

        string what = rewrite
            ? $"{changes.Count} placement(s) rewritten to {toRawcode}"
            : $"{changes.Count} placement(s) dropped";
        return new(true, what, changes.Count, changes);
    }

    private static PlacedFixRefsResult Fail(string message)
        => new(false, message, 0, Array.Empty<FixRefChange>());
}
