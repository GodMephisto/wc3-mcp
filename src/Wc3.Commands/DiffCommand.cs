using Wc3.Model;

namespace Wc3.Commands;

public static class DiffCommand
{
    /// <summary>
    /// Compares two maps file by file. A name appearing more than once in an archive is compared
    /// as the set of its copies, so two archives agree on that name only when every copy matches.
    /// </summary>
    /// <remarks>
    /// An MPQ can hold two entries under one name, and one map in the measured library of 34 does,
    /// with 14 duplicated names including Doodads\Doodads.slk twice. Keying a dictionary by name
    /// threw "An item with the same key has already been added" and took the whole comparison down,
    /// so <c>roundtrip</c> reported that map as a failure even though it saves perfectly well, all
    /// 100,788,325 bytes of it. The save was never the problem, the verdict was.
    ///
    /// Grouping rather than deduplicating, because dropping a copy would let a rebuild that lost
    /// one of the two report clean, and a fidelity check that cannot see a lost file is not one.
    /// </remarks>
    public static DiffResult Execute(MapDocument a, MapDocument b)
    {
        var av = ByName(a);
        var bv = ByName(b);
        var entries = new List<DiffEntry>();
        foreach (var name in av.Keys.Union(bv.Keys))
        {
            bool inA = av.TryGetValue(name, out var copiesA);
            bool inB = bv.TryGetValue(name, out var copiesB);
            if (inA && !inB) entries.Add(new DiffEntry(name, "removed"));
            else if (!inA && inB) entries.Add(new DiffEntry(name, "added"));
            else if (!SameContent(copiesA!, copiesB!)) entries.Add(new DiffEntry(name, "modified"));
        }
        return new DiffResult(entries);
    }

    private static Dictionary<string, List<byte[]>> ByName(MapDocument doc)
    {
        var map = new Dictionary<string, List<byte[]>>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in doc.Files)
        {
            if (f.FileName is null) continue;
            if (!map.TryGetValue(f.FileName, out var list))
                map[f.FileName] = list = new List<byte[]>();
            list.Add(f.CurrentBytes);
        }
        return map;
    }

    /// <summary>
    /// Whether two sets of same-named copies hold the same content. Order-insensitive, because a
    /// rebuild may emit duplicates in either order and that is not a content change.
    /// </summary>
    private static bool SameContent(List<byte[]> a, List<byte[]> b)
    {
        if (a.Count != b.Count) return false;
        if (a.Count == 1) return a[0].SequenceEqual(b[0]);   // the overwhelmingly common case

        var remaining = new List<byte[]>(b);
        foreach (var candidate in a)
        {
            int at = remaining.FindIndex(x => x.SequenceEqual(candidate));
            if (at < 0) return false;
            remaining.RemoveAt(at);
        }
        return remaining.Count == 0;
    }
}
