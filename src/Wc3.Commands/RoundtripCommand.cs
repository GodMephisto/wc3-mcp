using Wc3.Model;

namespace Wc3.Commands;

public static class RoundtripCommand
{
    // MPQ bookkeeping files handled specially by MpqArchiveBuilder on save:
    // (listfile) and (attributes) are regenerated; (signature) is not carried
    // over (the rebuilt map is unsigned). They are archive metadata, not map
    // data, so they are excluded from the fidelity verdict — but any that were
    // present in the original and dropped/changed are surfaced as notes.
    // Shared with tests/Wc3.Tests/RoundtripTests.cs.
    public static readonly IReadOnlySet<string> MpqSpecialFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "(listfile)", "(attributes)", "(signature)",
    };

    public static RoundtripResult Execute(MapDocument doc)
    {
        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var mismatches = new List<string>();
        if (!doc.PreArchiveData.SequenceEqual(rebuilt.PreArchiveData))
            mismatches.Add("(pre-archive header)");
        var diff = DiffCommand.Execute(doc, rebuilt);
        mismatches.AddRange(diff.Entries
            .Where(e => !MpqSpecialFiles.Contains(e.Name))
            .Select(e => $"{e.Name}:{e.Change}"));

        // Special files present in the original but dropped or rewritten on
        // save. Reported for transparency; they never flip the verdict.
        var excludedNotes = diff.Entries
            .Where(e => MpqSpecialFiles.Contains(e.Name))
            .Where(e => e.Change is "removed" or "modified")
            .Select(e => e.Change == "removed"
                ? $"{e.Name} not carried over on save (MPQ archive metadata; map content unaffected)"
                : $"{e.Name} regenerated on save (MPQ archive metadata; map content unaffected)")
            .ToList();

        return new RoundtripResult(mismatches.Count == 0, mismatches, excludedNotes);
    }
}
