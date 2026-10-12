// src/Wc3.Commands/MpqAddCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The edited copy, whether the name was added or replaced, and whether the payload
/// round-tripped byte for byte.</summary>
public sealed record MpqAddResult(
    string Path,
    string SavedTo,
    string Name,
    bool Replaced,
    int InputEntries,
    int OutputEntries,
    int SizeBytes,
    bool ByteFaithful,
    long OutputBytes,
    bool Reopens,
    string? ReopenError);

/// <summary>
/// Adds or replaces one internal archive file from a file on disk, byte-faithfully.
///
/// The payload is staged as an override and written verbatim on save, so the bytes read back out
/// of the copy equal the bytes read from disk. <see cref="MpqAddResult.Replaced"/> says which case
/// this was, an existing name superseded or a new one added, and the result's
/// <see cref="MpqAddResult.ByteFaithful"/> is the measured round trip rather than an assumption.
///
/// A copy is written, and the input map is never modified in place.
/// </summary>
public static class MpqAddCommand
{
    public static MpqAddResult Execute(string path, string name, string fromFile, string? outPath)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("an internal file name is required", nameof(name));
        string dest = MpqStripBookkeepingCommand.ResolveOutput(path, outPath);
        if (!File.Exists(fromFile))
            throw new FileNotFoundException($"source file not found: {fromFile}", fromFile);

        byte[] payload = File.ReadAllBytes(fromFile);
        var doc = MapDocument.Load(path);
        int inputEntries = doc.Files.Count;
        bool replaced = doc.GetFile(name) is not null;
        doc.AddOrReplaceRawFile(name, payload);
        byte[] saved = doc.SaveToBytes();

        string? dir = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(dest, saved);

        bool reopens = true;
        string? reopenError = null;
        int outputEntries = 0;
        bool byteFaithful = false;
        try
        {
            var reopened = MapDocument.Load(saved);
            outputEntries = reopened.Files.Count;
            byteFaithful = reopened.TryReadFileByName(name, out var got) && got.AsSpan().SequenceEqual(payload);
        }
        catch (Exception ex)
        {
            reopens = false;
            reopenError = ex.Message;
        }

        return new MpqAddResult(path, dest, name, replaced, inputEntries, outputEntries,
            payload.Length, byteFaithful, saved.LongLength, reopens, reopenError);
    }
}
