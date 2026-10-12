// src/Wc3.Commands/MpqStripBookkeepingCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The stripped copy, its entry counts, whether it reopened, and which bookkeeping survived.</summary>
public sealed record MpqStripResult(
    string Path,
    string SavedTo,
    int InputEntries,
    int OutputEntries,
    bool ListfilePresent,
    bool AttributesPresent,
    bool Reopens,
    string? ReopenError,
    long OutputBytes);

/// <summary>
/// Saves a copy with the optional MPQ bookkeeping files, <c>(listfile)</c> and
/// <c>(attributes)</c>, dropped rather than regenerated.
///
/// Warcraft III resolves a map's files through the hash table by name and treats attributes as
/// advisory, so neither file is needed to load a map. Dropping them removes a class of doubt
/// from a rebuilt map, since a regenerated <c>(attributes)</c> carries a CRC32 per file and a
/// rebuild that recompresses an entry can leave a stale CRC behind. The drop is driven by the
/// same environment switch MapDocument already reads, so this command and the switch can never
/// disagree about what dropping means.
///
/// The environment switch is process wide, so access is serialized on a lock and the previous
/// value is restored immediately after the save, whether the save succeeds or throws.
/// </summary>
public static class MpqStripBookkeepingCommand
{
    private const string DropVariable = "WC3CTL_DROP_MPQ_BOOKKEEPING";
    private static readonly object Gate = new();

    /// <summary>
    /// The output path, defaulting to a sibling <c>.edited</c> copy. Refuses the input map itself
    /// and any path inside a folder map, so the original is never overwritten.
    /// </summary>
    public static string ResolveOutput(string input, string? outPath)
    {
        string inputFull = Path.TrimEndingDirectorySeparator(Path.GetFullPath(input));
        string dest = string.IsNullOrWhiteSpace(outPath)
            ? Path.Combine(Path.GetDirectoryName(inputFull) ?? "",
                Path.GetFileNameWithoutExtension(inputFull) + ".edited" + Path.GetExtension(inputFull))
            : Path.GetFullPath(outPath);
        dest = Path.TrimEndingDirectorySeparator(dest);

        if (string.Equals(dest, inputFull, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "the output path is the input map, which is never overwritten. Pass a different -o.");
        if (Directory.Exists(inputFull)
            && dest.StartsWith(inputFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "the output path is inside the input map folder. Pass a path outside it.");
        return dest;
    }

    public static MpqStripResult Execute(string path, string? outPath)
    {
        string dest = ResolveOutput(path, outPath);
        var doc = MapDocument.Load(path);
        int inputEntries = doc.Files.Count;

        byte[] stripped;
        lock (Gate)
        {
            var previous = Environment.GetEnvironmentVariable(DropVariable);
            try
            {
                Environment.SetEnvironmentVariable(DropVariable, "both");
                stripped = doc.SaveToBytes();
            }
            finally
            {
                Environment.SetEnvironmentVariable(DropVariable, previous);
            }
        }

        string? dir = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(dest, stripped);

        // Confirm the result opens again, and that the two bookkeeping entries are gone. A map
        // that could only be saved by the in-place salvage patch keeps them, so the booleans are
        // the honest answer rather than an assumption that the drop always took.
        bool reopens = true;
        string? reopenError = null;
        int outputEntries = 0;
        bool listfile = false, attributes = false;
        try
        {
            var reopened = MapDocument.Load(stripped);
            outputEntries = reopened.Files.Count;
            listfile = reopened.HasFileByName("(listfile)");
            attributes = reopened.HasFileByName("(attributes)");
        }
        catch (Exception ex)
        {
            reopens = false;
            reopenError = ex.Message;
        }

        return new MpqStripResult(path, dest, inputEntries, outputEntries,
            listfile, attributes, reopens, reopenError, stripped.LongLength);
    }
}
