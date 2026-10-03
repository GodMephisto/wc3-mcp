using System.Text;

namespace Wc3.Commands;

/// <summary>
/// The one port report both front ends print. It existed twice, once in the CLI renderer and
/// once inline in Studio's port dialog, and the two drifted, so a fix to how the script closure
/// over-carry is presented landed in the CLI while Studio kept listing all 207 objects as if the
/// ported unit owned them. Same class of bug as the dependency panel and the bundle command had.
/// One definition, every caller.
///
/// This is text formatting over a result POCO, no CLI or GUI type is referenced, so the layering
/// rule holds. Front ends still own their own framing (a dry-run footer, a dialog, JSON).
/// </summary>
public static class PortReport
{
    /// <summary>The report body, header through diagnostics. Callers append their own footer.</summary>
    public static string Body(PortResult r)
    {
        var sb = new StringBuilder();
        Append(sb, r);
        return sb.ToString();
    }

    /// <summary>Appends the body to an existing builder, for a batch report that repeats it.</summary>
    public static void Append(StringBuilder sb, PortResult r)
    {
        string root = r.RootPortedTo == r.RootRawcode
            ? r.RootRawcode
            : $"{r.RootRawcode} → {r.RootPortedTo}";

        // The root's own objects, and the closure's counted rather than named. On a shared arena
        // script the closure carries other heroes' whole kits, which must be present for the
        // ported script to compile and run, but they are not this unit's and reading them here as
        // its own was the whole complaint. --json still carries every entry plus its flag.
        var own = r.Objects.Where(o => !o.CarriedByScriptClosure).ToList();
        int carried = r.Objects.Count - own.Count;

        sb.AppendLine($"Ported {root}{(r.RootName is null ? "" : $"  \"{r.RootName}\"")}");
        sb.AppendLine($"  {own.Count} object(s)"
                      + (carried > 0 ? $" (plus {carried} carried by the script closure)" : "")
                      + $", {r.CopiedFiles.Count} file(s) copied, "
                      + $"{r.InlinedStrings} string(s) inlined, {r.Remaps.Count} rawcode(s) remapped.");

        if (r.Remaps.Count > 0)
        {
            sb.AppendLine().AppendLine("Rawcode remaps (collisions with the target):");
            foreach (var m in r.Remaps)
                sb.AppendLine($"  {m.Kind.ToString().ToLowerInvariant()} {m.From} → {m.To}");
        }

        sb.AppendLine().AppendLine($"Objects ({own.Count}):");
        foreach (var o in own)
            sb.AppendLine($"  {o.Kind.ToString().ToLowerInvariant()} {o.Rawcode}"
                          + $"{(o.Name is null ? "" : $"  \"{o.Name}\"")}"
                          + $"{(o.ModifiesStandard ? "  (modifies standard object)" : "")}");
        if (carried > 0)
            sb.AppendLine($"  plus {carried} reached through the trigger script rather than this "
                + "unit's own fields (sub-abilities its handlers grant at runtime, dummies they "
                + "spawn), run with --json to list them");

        if (r.CopiedFiles.Count > 0)
        {
            // Same split. Every foreign icon was copied through a real art field, just on a foreign
            // object, so the field code can never tell them apart, only the object that asked can.
            var carriedFiles = (r.CarriedFiles ?? Array.Empty<string>()).ToHashSet(StringComparer.Ordinal);
            var ownFiles = r.CopiedFiles.Where(f => !carriedFiles.Contains(f)).ToList();
            sb.AppendLine().AppendLine($"Copied files ({ownFiles.Count}):");
            foreach (var f in ownFiles) sb.AppendLine($"  {f}");
            if (carriedFiles.Count > 0)
                sb.AppendLine($"  plus {carriedFiles.Count} carried by the script closure, "
                    + "run with --json to list them");
        }

        if (r.Script is { } s)
        {
            sb.AppendLine().AppendLine(
                $"Script (best-effort): {s.Functions} function(s), {s.Globals} global(s) carried, "
                + $"{s.Renamed} renamed, init {(s.InitHooked ? "wired" : "NOT wired")}.");
            foreach (var n in s.Notes) sb.AppendLine($"  - {n}");
        }

        if (r.Warnings.Count > 0)
        {
            sb.AppendLine().AppendLine("Warnings:");
            foreach (var w in r.Warnings) sb.AppendLine($"  ! {w}");
        }

        foreach (var d in r.Diagnostics) sb.AppendLine($"note: {d}");
    }
}
