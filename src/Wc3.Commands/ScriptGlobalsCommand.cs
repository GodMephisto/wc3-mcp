// src/Wc3.Commands/ScriptGlobalsCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The variables a map's compiled script declares, as opposed to the GUI variables the
/// trigger tree declares.
/// </summary>
/// <remarks>
/// <para>These are two different populations and conflating them reads as a missing feature.
/// Anime_WOS2_0.29d carries ZERO GUI variables in war3map.wtg and 12,543 declarations in its
/// script's globals block. A panel that reads only the tree therefore shows nothing at all,
/// on a map that plainly has thousands of variables.</para>
///
/// <para>That is correct against the file and useless to a reader, which is the failure
/// already recorded for the Regions and Cameras panels. A front-end needs both numbers so it
/// can say "no GUI variables, and the script declares 12,543" rather than showing a blank.</para>
///
/// <para>This is a reader. Nothing here edits the script, because a global's declaration and
/// its uses are spread across the file and rewriting one safely is a different job.</para>
/// </remarks>
public static class ScriptGlobalsCommand
{
    /// <summary>One declaration inside the script's globals block.</summary>
    public sealed record ScriptGlobal(
        string Name,
        string Type,
        bool IsArray,
        bool IsConstant,
        string? InitialValue);

    public sealed record GlobalsResult(
        bool Ok,
        string Message,
        IReadOnlyList<ScriptGlobal> Globals);

    private static readonly GlobalsResult None =
        new(false, "this map has no readable script", Array.Empty<ScriptGlobal>());

    // A declaration is "[constant] <type> [array] <name> [= <value>]". Anchored at the line
    // start so a "globals" mentioned inside a string or comment cannot open a block.
    private static readonly Regex Declaration = new(
        @"^\s*(?<const>constant\s+)?(?<type>[A-Za-z_][A-Za-z0-9_]*)\s+(?<arr>array\s+)?"
        + @"(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*(=\s*(?<init>.+?))?\s*$",
        RegexOptions.Compiled);

    /// <summary>Reads the globals block from war3map.j (or the Lua equivalent's absence).</summary>
    public static GlobalsResult List(MapDocument doc)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (entry is null) return None;

        string text;
        try
        {
            // ScriptText, not a fresh Encoding choice. A map script is a byte stream with no
            // declared encoding, and every call site here agrees on Latin-1 so that a
            // read-modify-write cannot mangle bytes it did not touch.
            // CurrentBytes, not RawBytes. RawBytes is the archive's stored payload and ignores
            // an edit that has not been saved yet, so reading it would report the globals the
            // map had before the user changed anything.
            text = ScriptText.GetString(entry.CurrentBytes);
        }
        catch
        {
            return None;
        }

        var globals = new List<ScriptGlobal>();
        bool inBlock = false;

        foreach (var raw in text.Split('\n'))
        {
            string line = raw.TrimEnd('\r').Trim();

            if (!inBlock)
            {
                if (line.Equals("globals", StringComparison.Ordinal)) inBlock = true;
                continue;
            }
            if (line.Equals("endglobals", StringComparison.Ordinal)) break;

            if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal)) continue;

            var m = Declaration.Match(line);
            if (!m.Success) continue;

            string type = m.Groups["type"].Value;
            if (type is "constant") continue;   // malformed, the const group should have caught it

            globals.Add(new ScriptGlobal(
                m.Groups["name"].Value,
                type,
                m.Groups["arr"].Success,
                m.Groups["const"].Success,
                m.Groups["init"].Success ? m.Groups["init"].Value.Trim() : null));
        }

        if (!inBlock)
            return new GlobalsResult(true,
                "this map's script has no globals block", Array.Empty<ScriptGlobal>());

        int arrays = globals.Count(g => g.IsArray);
        return new GlobalsResult(true,
            $"{globals.Count} global(s) declared in the script, {arrays} array(s) and "
            + $"{globals.Count - arrays} scalar(s)",
            globals);
    }

    /// <summary>How many globals the script declares, or 0 when there is no readable script.</summary>
    public static int Count(MapDocument doc) => List(doc).Globals.Count;
}
