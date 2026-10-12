// src/Wc3.Commands/AssetDuplicatesCommand.cs
using System.Security.Cryptography;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Imported files whose bytes are identical, one group per distinct content.</summary>
public sealed record AssetDuplicateGroup(string Sha256, int SizeBytes, IReadOnlyList<string> Paths)
{
    /// <summary>Bytes recoverable by keeping one copy of the group.</summary>
    public long RecoverableBytes => (long)SizeBytes * (Paths.Count - 1);
}

/// <summary>
/// Identical imported files, grouped by the sha256 of their bytes. Read-only. Empty entries are
/// excluded, they are reported by <c>asset audit</c> and a map with hundreds of zero-byte
/// entries would otherwise read as one enormous duplicate group.
/// </summary>
public sealed record AssetDuplicatesResult(
    IReadOnlyList<AssetDuplicateGroup> Groups,
    int ImportedEntries,
    long RecoverableBytes);

/// <summary>
/// Finds imported files that are byte-for-byte the same, the wasted space a map carries when
/// the same texture or sound was imported twice under two names.
///
/// The digest comes from the entry's CURRENT bytes, so a replacement staged earlier in a
/// session is compared, not the original it shadows.
/// </summary>
public static class AssetDuplicatesCommand
{
    public static AssetDuplicatesResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entries = AssetReferences.ImportedEntries(doc);

        var byHash = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var sizes = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var entry in entries)
        {
            // A zero-byte entry holds nothing, so it deduplicates nothing. It is also far more
            // common than real duplicates on a protected map, where a naive grouping would drown
            // the answer in noise.
            if (entry.CurrentSize == 0) continue;
            var hash = Convert.ToHexString(SHA256.HashData(entry.CurrentBytes)).ToLowerInvariant();
            if (!byHash.TryGetValue(hash, out var paths))
            {
                byHash[hash] = paths = new List<string>();
                sizes[hash] = entry.CurrentSize;
            }
            paths.Add(entry.FileName!);
        }

        var groups = byHash
            .Where(kv => kv.Value.Count > 1)
            .Select(kv => new AssetDuplicateGroup(
                kv.Key,
                sizes[kv.Key],
                kv.Value.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList()))
            .OrderByDescending(g => g.RecoverableBytes)
            .ThenBy(g => g.Paths[0], StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new AssetDuplicatesResult(groups, entries.Count, groups.Sum(g => g.RecoverableBytes));
    }
}
