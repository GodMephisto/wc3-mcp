// src/Wc3.Commands/StringsSetCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One war3map.wts entry set or added, and where the edited copy landed.
/// <see cref="Added"/> is true when the id had no entry and one was appended.
/// </summary>
public sealed record StringsSetResult(
    string Path, string SavedTo, int Id, bool Added, string Text);

/// <summary>
/// Sets one war3map.wts entry's text, or adds the entry when the id is absent, and saves a copy.
/// The input map is never overwritten. <c>outPath</c> defaults to a sibling
/// <c>&lt;map&gt;.edited.&lt;ext&gt;</c> and a path equal to the input (or inside a folder map) is
/// refused.
///
/// The splice keeps the file's own encoding and line endings, it is
/// <see cref="StringsCommand.Upsert"/> over the existing bytes, so every other entry, header and
/// blank line survives. When the map carries no war3map.wts at all, one is created holding just
/// this entry.
/// </summary>
public static class StringsSetCommand
{
    public static StringsSetResult Execute(string path, int id, string text, string? outPath)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(text);

        // Same output contract as the other mutating commands, one shared resolver.
        string dest = MpqStripBookkeepingCommand.ResolveOutput(path, outPath);

        var doc = MapDocument.Load(path);
        bool added = !StringsCommand.List(doc).Entries.Any(e => e.Id == id);
        StringsCommand.Upsert(doc, id, text);

        string? dir = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        doc.Save(dest);

        return new StringsSetResult(path, dest, id, added, text);
    }
}
