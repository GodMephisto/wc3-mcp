// src/Wc3.Commands/StringReferences.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Which war3map.wts keys a map's own data references, answered once so the five strings
/// commands agree about what "a reference" is. Four sources carry a reference, the object-data
/// field values, the map-info text fields, the GUI trigger tree, and the map script. Each is
/// read the way the rest of the toolkit reads it (the object fields through
/// <see cref="AssetReferences.ObjectFieldValues"/>, so both Reforged layers are merged, and the
/// script through <see cref="AssetReferences.ScriptEntry"/>), so a reference one command
/// recognises is recognised by all of them.
///
/// The reference text is the ASCII token TRIGSTR_nnn. Object fields hold it whole, a script line
/// embeds it inside a string literal, and a w3i or wtg string is length prefixed around it, so
/// the scan is a token search over each source's decoded text rather than an anchored match.
/// </summary>
internal static partial class StringReferences
{
    /// <summary>A TRIGSTR_ reference anywhere in a text blob, leading zeros tolerated.</summary>
    [GeneratedRegex(@"TRIGSTR_(\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex TrigStrToken();

    /// <summary>The keys the map script names (war3map.j, else war3map.lua).</summary>
    public static IReadOnlySet<int> FromScript(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var ids = new HashSet<int>();
        if (AssetReferences.ScriptEntry(doc) is { } script)
            Collect(ScriptText.GetString(script.CurrentBytes), ids);
        return ids;
    }

    /// <summary>The keys the GUI trigger tree names (war3map.wtg) and its custom-text bodies
    /// (war3map.wct). Scanned as raw text because a trigger string is stored inline in the
    /// token form, not as an id column.</summary>
    public static IReadOnlySet<int> FromTriggers(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var ids = new HashSet<int>();
        foreach (var name in new[] { "war3map.wtg", "war3map.wct" })
            if (doc.GetFile(name) is { } file)
                Collect(Encoding.Latin1.GetString(file.CurrentBytes), ids);
        return ids;
    }

    /// <summary>The keys any object-data field value names, across every Object Editor kind
    /// and both Reforged layers.</summary>
    public static IReadOnlySet<int> FromObjectFields(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var ids = new HashSet<int>();
        foreach (var value in AssetReferences.ObjectFieldValues(doc))
            Collect(value, ids);
        return ids;
    }

    /// <summary>The keys the map-info text fields name (war3map.w3i), read from the raw bytes
    /// so every string field is covered without enumerating the model.</summary>
    public static IReadOnlySet<int> FromMapInfo(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var ids = new HashSet<int>();
        if (doc.GetFile("war3map.w3i") is { } file)
            Collect(Encoding.Latin1.GetString(file.CurrentBytes), ids);
        return ids;
    }

    /// <summary>Every key any of the four sources references.</summary>
    public static IReadOnlySet<int> All(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var ids = new HashSet<int>(FromScript(doc));
        ids.UnionWith(FromTriggers(doc));
        ids.UnionWith(FromObjectFields(doc));
        ids.UnionWith(FromMapInfo(doc));
        return ids;
    }

    private static void Collect(string? text, HashSet<int> ids)
    {
        if (string.IsNullOrEmpty(text)) return;
        foreach (Match match in TrigStrToken().Matches(text))
            if (int.TryParse(match.Groups[1].Value, out var id)) ids.Add(id);
    }
}
