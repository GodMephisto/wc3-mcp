// src/Wc3.Commands/MpqListfileCommand.cs
using War3Net.IO.Mpq;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A count of how each entry got its name, and a sample of the ones that did not.</summary>
public sealed record MpqListfileResult(
    string Path,
    int TotalEntries,
    int NamedByListfile,
    int RecoveredByStandardProbe,
    int WithoutName,
    IReadOnlyList<string> WithoutNameSample);

/// <summary>
/// Reports the archive's listfile coverage in three parts. An entry is named by the listfile
/// the archive ships, named by the standard-name probe the toolkit runs, or still nameless.
///
/// The three are separable only if the probe runs after the listfile is read, which is why
/// this opens the archive itself rather than using MapDocument, whose Load has already folded
/// both sources together. The split is what a deprotection decision turns on, a name from the
/// listfile is a fact the map admits to, a probe recovery is one this toolkit guessed right,
/// and a nameless entry is the work still to do.
/// </summary>
public static class MpqListfileCommand
{
    private const int SampleLimit = 20;

    public static MpqListfileResult Execute(string path)
    {
        var bytes = File.ReadAllBytes(path);
        using var stream = new MemoryStream(bytes);
        using var archive = MpqArchive.Open(stream, loadListFile: true);

        // Snapshot the entries before probing so the listfile names and the probe recoveries
        // can be told apart. AddFileNames sets FileName on the same entry objects.
        var entries = archive.ToList();
        int total = entries.Count;
        int namedByListfile = entries.Count(e => e.FileName is not null);

        archive.AddFileNames(StandardMapFileNames.All);

        int namedAfterProbe = 0;
        var unnamed = new List<int>();
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].FileName is not null) namedAfterProbe++;
            else unnamed.Add(i);
        }
        int recoveredByProbe = namedAfterProbe - namedByListfile;

        var sample = unnamed.Take(SampleLimit).Select(i => $"block {i}").ToList();
        if (unnamed.Count > SampleLimit)
            sample.Add($"... and {unnamed.Count - SampleLimit} more");

        return new MpqListfileResult(path, total, namedByListfile, recoveredByProbe,
            unnamed.Count, sample);
    }
}
