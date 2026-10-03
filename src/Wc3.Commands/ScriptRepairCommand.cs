// src/Wc3.Commands/ScriptRepairCommand.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Outcome of repairing a map's script. <paramref name="Repairs"/> is how many broken
/// declarations were restored, <paramref name="Remaining"/> lists errors repair cannot fix.</summary>
public sealed record ScriptRepairResult(
    bool Ok,
    string Message,
    int Repairs,
    IReadOnlyList<string> Remaining);

/// <summary>
/// Repairs a map whose JASS was left un-compilable, the failure that empties a host lobby (an
/// undeclared variable fails the whole war3map.j, so config() never builds player slots). Fixes
/// what is mechanically fixable, then reports anything left so a still-broken map is never
/// mistaken for a healthy one. Mutates the document in place, the caller saves.
/// </summary>
public static class ScriptRepairCommand
{
    public static ScriptRepairResult Execute(MapDocument doc)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        byte[]? bytes = entry?.CurrentBytes;
        if (entry?.FileName is null || bytes is null || bytes.Length == 0)
            return new(false, "map has no war3map.j to repair", 0, Array.Empty<string>());

        // Latin1 round-trips every byte, so an untouched script re-serializes identically.
        var enc = Encoding.Latin1;
        string before = enc.GetString(bytes);
        var wasBroken = JassScriptCheck.Check(before)
            .Where(i => i.Severity == DiagnosticSeverity.Error).ToList();

        string after = JassScriptCheck.Repair(before, out int repairs);
        if (repairs > 0) doc.AddOrReplaceRawFile(entry.FileName, enc.GetBytes(after));

        var remaining = JassScriptCheck.Check(after)
            .Where(i => i.Severity == DiagnosticSeverity.Error)
            .Select(i => i.Line > 0 ? $"line {i.Line}: {i.Message}" : i.Message)
            .ToList();

        string message = (wasBroken.Count, repairs, remaining.Count) switch
        {
            (0, 0, 0) => "script already compiles, nothing to repair",
            (_, > 0, 0) => $"repaired {repairs} dropped declaration(s), the script now passes every check",
            (_, > 0, _) => $"repaired {repairs} dropped declaration(s), but {remaining.Count} error(s) remain",
            (_, 0, > 0) => $"{remaining.Count} error(s) found, none of them mechanically repairable",
            _ => "nothing to repair",
        };
        return new(remaining.Count == 0, message, repairs, remaining);
    }
}
