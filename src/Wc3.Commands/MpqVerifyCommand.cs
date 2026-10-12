// src/Wc3.Commands/MpqVerifyCommand.cs
using War3Net.IO.Mpq;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One archive entry whose payload could not be opened or fully read.</summary>
public sealed record MpqUnreadableEntry(int BlockIndex, string? Name, long SizeBytes, string Error);

/// <summary>How many entries an archive holds and which of them would not read.</summary>
public sealed record MpqVerifyResult(
    string Path,
    int TotalEntries,
    int ReadableEntries,
    int UnreadableEntries,
    IReadOnlyList<MpqUnreadableEntry> Unreadable);

/// <summary>
/// Opens every entry of an archive and reports the ones that will not read.
///
/// A map can load and still carry an entry whose payload is corrupt or whose key is
/// unrecoverable. Load only opens and closes each entry, so a failure that appears during
/// decompression stays invisible until something reads the bytes. This reads every entry to
/// its end, which is the only way to prove the whole archive is usable rather than merely
/// addressable, and it names each failure with the exception the read raised.
/// </summary>
public static class MpqVerifyCommand
{
    public static MpqVerifyResult Execute(string path)
    {
        var bytes = File.ReadAllBytes(path);
        using var stream = new MemoryStream(bytes);
        using var archive = MpqArchive.Open(stream, loadListFile: true);
        // Same probe Load runs, so an entry the archive stores without a listfile name is
        // reported by its real name rather than by a block index.
        archive.AddFileNames(StandardMapFileNames.All);

        var unreadable = new List<MpqUnreadableEntry>();
        int total = 0, readable = 0;
        var buffer = new byte[81920];
        foreach (var entry in archive)
        {
            try
            {
                using var fs = archive.OpenFile(entry);
                while (fs.Read(buffer, 0, buffer.Length) > 0) { }
                readable++;
            }
            catch (Exception ex)
            {
                unreadable.Add(new MpqUnreadableEntry(total, entry.FileName, (long)entry.FileSize, ex.Message));
            }
            total++;
        }

        return new MpqVerifyResult(path, total, readable, unreadable.Count, unreadable);
    }
}
