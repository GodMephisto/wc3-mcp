using Wc3.Model;

namespace Wc3.Commands;

public static class DiffCommand
{
    public static DiffResult Execute(MapDocument a, MapDocument b)
    {
        var av = a.Files.Where(f => f.FileName != null).ToDictionary(f => f.FileName!, f => f.RawBytes);
        var bv = b.Files.Where(f => f.FileName != null).ToDictionary(f => f.FileName!, f => f.RawBytes);
        var entries = new List<DiffEntry>();
        foreach (var name in av.Keys.Union(bv.Keys))
        {
            bool inA = av.ContainsKey(name), inB = bv.ContainsKey(name);
            if (inA && !inB) entries.Add(new DiffEntry(name, "removed"));
            else if (!inA && inB) entries.Add(new DiffEntry(name, "added"));
            else if (!av[name].SequenceEqual(bv[name])) entries.Add(new DiffEntry(name, "modified"));
        }
        return new DiffResult(entries);
    }
}
