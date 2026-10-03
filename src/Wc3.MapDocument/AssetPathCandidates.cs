// src/Wc3.MapDocument/AssetPathCandidates.cs
using System.Text.RegularExpressions;

namespace Wc3.Model;

/// <summary>
/// Turns one candidate asset path, harvested from a script string literal or an object data
/// field by <see cref="MapDocument.HarvestAssetNames"/>, into the spelling variants Warcraft
/// actually accepts, so a stripped-listfile map's imported models, textures, and sounds can be
/// probed by name the same way <see cref="StandardMapFileNames"/> probes for the map's own
/// structural files. Warcraft loads several of these extension-agnostically, a reference to
/// "Foo.mdl" plays whatever the archive actually stores under "Foo.mdx", and a texture or sound
/// reference commonly carries no extension at all, the game appends one at load. A name pulled
/// out of the script therefore rarely matches the archive byte for byte, every sibling extension
/// in the same media family has to be tried too. An icon field also names only ONE of the up to
/// three files an ability icon actually ships as, the normal "BTN" art plus its "DISBTN"
/// (disabled) and "PASBTN" (passive) siblings, since the engine derives those two by substituting
/// the prefix at load rather than the map naming them anywhere readable, so those siblings are
/// probed too whenever a candidate's own file name carries one of the three prefixes.
/// </summary>
public static class AssetPathCandidates
{
    private static readonly string[] ModelExt = { ".mdx", ".mdl" };
    private static readonly string[] TextureExt = { ".blp", ".tga", ".dds" };
    private static readonly string[] SoundExt = { ".mp3", ".wav", ".flac" };
    private static readonly string[] AllExt = ModelExt.Concat(TextureExt).Concat(SoundExt).ToArray();
    private static readonly string[] IconPrefixes = { "BTN", "DISBTN", "PASBTN" };

    /// <summary>A token is worth probing as an asset iff it carries a known media extension
    /// anywhere, or a backslash, an extensionless model, texture, or sound reference is still
    /// written as a path ("war3mapImported\Foo"), a bare identifier with neither is never an
    /// asset reference.</summary>
    /// <remarks>
    /// A token must also be short enough, and free of control characters, to BE a path. That is
    /// not pedantry. A real map's object data carries 659 character multi-line tooltips full of
    /// colour codes, they contain backslashes, and handing one to War3Net's name hashing throws
    /// IndexOutOfRangeException. That took the entire name-recovery pass down on
    /// U9_PumpkinZ_v4.7d, so the map recovered nothing, and any caller not wrapping the call in
    /// a try/catch crashed outright. The script literal regex already applied this ceiling while
    /// this test did not, so anything arriving from another source (object data field values,
    /// notably) escaped it. The bound belongs here, in the one test every source goes through.
    /// </remarks>
    public static bool LooksLikeAssetPath(string token) =>
        token.Length > 0 &&
        token.Length <= MaxPathLength &&
        !token.Any(char.IsControl) &&
        (token.Contains('\\') || AllExt.Any(ext => token.Contains(ext, StringComparison.OrdinalIgnoreCase)));

    /// <summary>The longest a candidate may be. An MPQ path is short, and a token this long is
    /// prose. Matches the ceiling the script literal regex has always used.</summary>
    private const int MaxPathLength = 260;

    /// <summary>Un-escapes a JASS/Lua string literal captured raw out of a script (the inner text
    /// between the quotes, doubled backslash and all). A JASS source file writes one literal path
    /// separator as two backslash characters, EffectSpawn("war3mapImported\\Foo.mdx" ...) means the
    /// single-backslash path war3mapImported\Foo.mdx at runtime, so any caller that pulls a path out
    /// of script text with a plain "([^\"]*)" style regex must run it through here before treating
    /// it as a map file name, or every lookup silently misses a file that is genuinely present.</summary>
    public static string Unescape(string rawLiteral) => rawLiteral.Replace(@"\\", @"\");

    // A quoted run with no line break. The floor skips "" and one or two character literals,
    // which cannot name a file, and the ceiling skips a tooltip or a generated multi-kilobyte
    // string, which is prose rather than a path.
    private static readonly Regex ScriptStringLiteral =
        new("\"([^\"\r\n]{3,260})\"", RegexOptions.Compiled);

    /// <summary>
    /// Candidate asset paths named by string literals in a body of script text, unescaped and
    /// deduplicated in first-seen order. Filtered by <see cref="LooksLikeAssetPath"/>, so a
    /// result is worth probing and is not yet known to exist. Callers still run each through
    /// <see cref="Expand"/> against a real archive.
    /// </summary>
    /// <remarks>
    /// A script names most of its effect art as plain literals, so any caller scanning for them
    /// must apply the same literal shape and the same un-escaping every other caller does, or the
    /// two disagree about what a script depends on. One doubled backslash missed is a file
    /// reported absent while it sits in the archive.
    /// </remarks>
    public static IEnumerable<string> NamedInScript(string scriptText)
    {
        ArgumentNullException.ThrowIfNull(scriptText);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in ScriptStringLiteral.Matches(scriptText))
        {
            var path = Unescape(m.Groups[1].Value);
            if (LooksLikeAssetPath(path) && seen.Add(path))
                yield return path;
        }
    }

    /// <summary>
    /// Every spelling <paramref name="path"/> might actually be stored under in the archive,
    /// both slash conventions times {<paramref name="path"/> and its icon-prefix siblings, if
    /// any} times {as given, every sibling extension within its media family if it already has
    /// one of the three families' extensions, or every extension in every family appended if it
    /// has none}. Over-generating is the point, probing a candidate that does not exist costs
    /// one failed hash lookup, missing the one that does exist costs the asset.
    /// </summary>
    public static IEnumerable<string> Expand(string path)
    {
        foreach (var p in Slashes(path))
            foreach (var q in WithIconSiblings(p))
            {
                yield return q;
                var ext = Path.GetExtension(q);
                var family = ext.Length > 0 ? FamilyOf(ext) : null;
                if (family is not null)
                {
                    var stem = q[..^ext.Length];
                    foreach (var sibling in family)
                        if (!sibling.Equals(ext, StringComparison.OrdinalIgnoreCase))
                            yield return stem + sibling;
                }
                else
                {
                    foreach (var e in AllExt) yield return q + e;
                }
            }
    }

    private static string[]? FamilyOf(string ext) =>
        ModelExt.Contains(ext, StringComparer.OrdinalIgnoreCase) ? ModelExt
        : TextureExt.Contains(ext, StringComparer.OrdinalIgnoreCase) ? TextureExt
        : SoundExt.Contains(ext, StringComparer.OrdinalIgnoreCase) ? SoundExt
        : null;

    private static IEnumerable<string> Slashes(string path)
    {
        yield return path;
        if (path.Contains('/')) yield return path.Replace('/', '\\');
        if (path.Contains('\\')) yield return path.Replace('\\', '/');
    }

    /// <summary><paramref name="p"/> itself, plus the OTHER two icon-prefix spellings when its
    /// file name starts with "BTN", "DISBTN", or "PASBTN" (a file name matches at most one of
    /// the three, they are mutually prefixing).</summary>
    private static IEnumerable<string> WithIconSiblings(string p)
    {
        yield return p;
        var name = Path.GetFileName(p);
        var dirPrefix = p[..^name.Length]; // everything up to and including the last slash, or ""
        foreach (var prefix in IconPrefixes)
        {
            if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
            var stem = name[prefix.Length..];
            foreach (var sibling in IconPrefixes)
                if (!sibling.Equals(prefix, StringComparison.OrdinalIgnoreCase))
                    yield return dirPrefix + sibling + stem;
            yield break;
        }
    }
}
