// src/Wc3.Commands/MpqBytesDiffCommand.cs
using System.Security.Cryptography;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One name present in both archives whose payload bytes are not identical.</summary>
public sealed record MpqEntryDiff(string Name, long SizeA, long SizeB, string Sha256A, string Sha256B);

/// <summary>The per-name byte comparison of two archives.</summary>
public sealed record MpqBytesDiffResult(
    string PathA,
    string PathB,
    int InBoth,
    int IdenticalCount,
    int Differ,
    IReadOnlyList<MpqEntryDiff> Differences,
    IReadOnlyList<string> OnlyInA,
    IReadOnlyList<string> OnlyInB)
{
    public bool Identical => Differ == 0 && OnlyInA.Count == 0 && OnlyInB.Count == 0;
}

/// <summary>
/// Compares two archives entry by entry, by name, on the payload bytes each stores. For every
/// name the two archives share it reports whether a SHA256 of the payload matched, and it lists
/// the names only one side carries.
///
/// This is the byte-level counterpart of the structural <c>mpq-diff</c>. That command answers
/// "are these two archives stored the same way" from header fields and storage flags, and
/// <c>diff</c> answers "which files changed" through the parsed model layer. Neither says
/// whether a given entry's bytes are actually identical. This does, by hashing the raw payload,
/// so a rebuild that changed an entry's compression but not its content shows as identical here
/// and differently encoded there, which is exactly the distinction a fidelity review needs.
///
/// Only named entries are compared. An unnamed entry carries no name to match across two
/// archives and a block index means different things in each, so pairing them would be a guess.
/// </summary>
public static class MpqBytesDiffCommand
{
    public static MpqBytesDiffResult Execute(string pathA, string pathB)
    {
        var mapA = NamedPayloads(pathA);
        var mapB = NamedPayloads(pathB);

        int identical = 0;
        var differences = new List<MpqEntryDiff>();
        foreach (var (name, a) in mapA)
        {
            if (!mapB.TryGetValue(name, out var b)) continue;
            if (a.Hash == b.Hash)
            {
                identical++;
                continue;
            }
            differences.Add(new MpqEntryDiff(name, a.Size, b.Size, a.Hash, b.Hash));
        }

        var onlyInA = mapA.Keys.Where(k => !mapB.ContainsKey(k))
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        var onlyInB = mapB.Keys.Where(k => !mapA.ContainsKey(k))
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        differences = differences.OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase).ToList();

        return new MpqBytesDiffResult(pathA, pathB, mapA.Keys.Count(mapB.ContainsKey),
            identical, differences.Count, differences, onlyInA, onlyInB);
    }

    private static Dictionary<string, (long Size, string Hash)> NamedPayloads(string path)
    {
        var doc = MapDocument.Load(path);
        var result = new Dictionary<string, (long, string)>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in doc.Files)
        {
            if (entry.FileName is null) continue;
            // RawBytes is the payload as the archive stores it, which is what this compares.
            var bytes = entry.RawBytes;
            // TryAdd keeps the first of any duplicate name, the same way the archive resolves one.
            result.TryAdd(entry.FileName, (bytes.LongLength, Convert.ToHexString(SHA256.HashData(bytes))));
        }
        return result;
    }
}
