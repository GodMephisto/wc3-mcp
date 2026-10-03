using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The map's gameplay constants (war3mapMisc.txt). Ability learn requirements are gated
/// by MaxHeroLevel, which lives here, so a ported hero whose spells unlock past level 10
/// clamps to WC3's default cap of 10 unless the source map's cap comes across too.
/// war3mapMisc.txt is a standard map file (an <see cref="ImportsCommand"/> NonImportFile),
/// so the imports copy never carries it, this does so deliberately.
/// </summary>
public static class GameplayConstants
{
    public const string MiscFile = "war3mapMisc.txt";

    /// <summary>The gameplay constants that gate ported content and are safe to raise in place.</summary>
    private static readonly string[] LevelCapKeys = { "MaxHeroLevel", "MaxUnitLevel" };

    // war3mapMisc.txt is a Windows-1252 / Latin1 INI file; Latin1 round-trips every byte.
    private static readonly Encoding Enc = Encoding.Latin1;

    /// <summary>
    /// Carries the source map's hero and unit level caps into <paramref name="target"/>
    /// (mutated in place, the caller saves). If the target has no war3mapMisc.txt, the
    /// source's file is copied wholesale (nothing to conflict with). Otherwise only
    /// <c>MaxHeroLevel</c> and <c>MaxUnitLevel</c> are raised to the higher of the two
    /// values, never lowered, and the target's other tuning is left untouched. Idempotent,
    /// re-running makes no further change. Returns a one-line summary of what changed, or
    /// null when the source has no constants file or nothing needed raising.
    /// </summary>
    public static string? CarryLevelCaps(MapDocument source, MapDocument target)
    {
        var srcEntry = source.GetFile(MiscFile);
        if (srcEntry is null) return null;
        string srcText = Enc.GetString(srcEntry.CurrentBytes);

        var tgtEntry = target.GetFile(MiscFile);
        if (tgtEntry is null)
        {
            target.AddOrReplaceRawFile(MiscFile, Enc.GetBytes(srcText));
            int hero = ReadInt(srcText, "MaxHeroLevel") ?? 0;
            return hero > 0
                ? $"added {MiscFile} from source (MaxHeroLevel={hero})"
                : $"added {MiscFile} from source";
        }

        string tgtText = Enc.GetString(tgtEntry.CurrentBytes);
        var changes = new List<string>();
        foreach (var key in LevelCapKeys)
            tgtText = RaiseKey(tgtText, srcText, key, changes);
        if (changes.Count == 0) return null;

        target.AddOrReplaceRawFile(MiscFile, Enc.GetBytes(tgtText));
        return string.Join(", ", changes);
    }

    /// <summary>Raises <paramref name="key"/> in the target text to the source's value when
    /// the source is higher (or the target lacks the key). Appends a change note when it does.</summary>
    private static string RaiseKey(string tgtText, string srcText, string key, List<string> changes)
    {
        int? src = ReadInt(srcText, key);
        if (src is null) return tgtText;
        int? tgt = ReadInt(tgtText, key);
        if (tgt is not null && tgt >= src) return tgtText;
        changes.Add($"{key} {tgt?.ToString() ?? "default"} -> {src}");
        return WriteKey(tgtText, key, src.Value);
    }

    /// <summary>Reads an integer INI value (a <c>Key=N</c> line), or null if absent.</summary>
    private static int? ReadInt(string text, string key)
    {
        var m = Regex.Match(text, $@"(?m)^\s*{Regex.Escape(key)}\s*=\s*(-?\d+)");
        return m.Success ? int.Parse(m.Groups[1].Value) : null;
    }

    /// <summary>Overwrites a <c>Key=N</c> line, or appends the key when it is absent.</summary>
    private static string WriteKey(string text, string key, int val)
    {
        var re = new Regex($@"(?m)^(\s*{Regex.Escape(key)}\s*=\s*)-?\d+");
        if (re.IsMatch(text))
            return re.Replace(text, "${1}" + val, 1);
        string nl = text.Contains("\r\n") ? "\r\n" : "\n";
        return text.TrimEnd('\r', '\n') + nl + key + "=" + val + nl;
    }
}
