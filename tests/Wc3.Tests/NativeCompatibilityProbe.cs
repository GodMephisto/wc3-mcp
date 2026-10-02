// tests/Wc3.Tests/NativeCompatibilityProbe.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.GameData;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Answers the question a patch always raises for an existing map. Does this map call anything the
/// current build no longer provides.
///
/// Blizzard publishes no diff, so the only authority is the build's own common.j and blizzard.j.
/// This reads them out of CASC, collects every native and function they declare, then collects
/// every call the map's script makes and every function the map defines for itself. What is left
/// is a call that resolves to nothing, which is a trigger that cannot work.
///
/// Signature changes are reported separately from disappearances, because a native that still
/// exists with a different arity is the quieter and more dangerous of the two.
/// </summary>
public class NativeCompatibilityProbe
{
    private const string Install = @"C:\Warcraft III";
    private readonly ITestOutputHelper _out;
    public NativeCompatibilityProbe(ITestOutputHelper output) => _out = output;

    // Deliberately NOT anchored to the start of a line. A protected map's script is commonly
    // emitted with the newlines stripped, and the anchored form then reported that a 4.5 MB
    // script defined zero functions, which is impossible and would have turned every one of its
    // own helpers into a false "missing native".
    private static readonly Regex Declared = new(
        @"\b(?:native|function)\s+([A-Za-z_]\w*)\s+takes\b", RegexOptions.Compiled);

    // A call is an identifier followed by an open paren. Crude, and right for this purpose,
    // because anything it over-collects is then cancelled by the declared and local sets.
    private static readonly Regex Called = new(
        @"\b([A-Za-z_]\w*)\s*\(", RegexOptions.Compiled);

    /// <summary>
    /// Blanks string literals and line comments so neither is mistaken for code. Colour codes
    /// like "|c000080FF" and prose like "Population (16)" otherwise read as calls to natives that
    /// do not exist, which grows phantom findings on a perfectly healthy map.
    ///
    /// One pass, not two regexes. Stripping comments first ate the closing quote of any literal
    /// containing "//", such as a path, after which the scan lost track of where strings began
    /// and ended. On BleachVsOnepiece15.w3x that swallowed 67% of a 4.5 MB script, deleted 54
    /// real function definitions along with it, and reported those 54 as missing natives. The
    /// balanced 384 definitions against 384 calls in that file is the correct answer.
    /// </summary>
    private static string StripLiterals(string script)
    {
        var sb = new StringBuilder(script.Length);
        bool inString = false, inComment = false;

        for (int i = 0; i < script.Length; i++)
        {
            char c = script[i];

            if (inComment)
            {
                if (c == '\n') { inComment = false; sb.Append(c); }
                else sb.Append(' ');
                continue;
            }
            if (inString)
            {
                // JASS uses a backslash escape, so a quote right after one does not close.
                if (c == '\\' && i + 1 < script.Length) { sb.Append("  "); i++; continue; }
                if (c == '"') inString = false;
                sb.Append(c == '\n' ? c : ' ');
                continue;
            }
            if (c == '"') { inString = true; sb.Append(' '); continue; }
            if (c == '/' && i + 1 < script.Length && script[i + 1] == '/')
            {
                inComment = true; sb.Append("  "); i++; continue;
            }
            sb.Append(c);
        }
        return sb.ToString();
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void What_does_the_build_declare()
    {
        if (!CascGameDataSource.TryOpen(Install, out var casc, out _) || casc is null)
        { _out.WriteLine("no install"); return; }
        using var source = casc;

        foreach (var name in source.EnumerateFileNames()
                     .Where(n => n.EndsWith(".j", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
        {
            var b = source.ReadFile(name);
            if (b is null) { _out.WriteLine($"{name}  unreadable"); continue; }
            string text = Encoding.Latin1.GetString(b);
            int decls = Declared.Matches(text).Count;
            _out.WriteLine($"{name}  {b.Length:N0} bytes, {decls} declaration(s)");
        }
    }

    [Theory]
    [Trait("Category", "GameData")]
    [InlineData("BleachVsOnepiece15.w3x")]
    [InlineData("Naruto Autobattle ENGv3ch11.w3x")]
    public void Does_this_map_call_anything_the_build_no_longer_has(string mapName)
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} not on disk"); return; }
        if (!CascGameDataSource.TryOpen(Install, out var casc, out _) || casc is null)
        { _out.WriteLine("no install"); return; }
        using var source = casc;

        // Everything the build declares, across every base script it ships.
        var provided = new HashSet<string>(StringComparer.Ordinal);
        foreach (var n in source.EnumerateFileNames()
                     .Where(n => n.EndsWith(".j", StringComparison.OrdinalIgnoreCase)))
        {
            var b = source.ReadFile(n);
            if (b is null) continue;
            foreach (Match m in Declared.Matches(Encoding.Latin1.GetString(b)))
                provided.Add(m.Groups[1].Value);
        }
        _out.WriteLine($"build declares {provided.Count} callable name(s)");

        var doc = MapDocument.Load(path);
        var scriptEntry = doc.GetFile("war3map.j") ?? doc.GetFile("war3map.lua");
        if (scriptEntry is null) { _out.WriteLine("map has no script"); return; }
        string raw = Encoding.Latin1.GetString(scriptEntry.OverrideBytes ?? scriptEntry.RawBytes);
        string script = StripLiterals(raw);
        _out.WriteLine($"{mapName}, script is {raw.Length:N0} bytes, "
                     + $"{raw.Length - script.Length:N0} of it string/comment");

        var localFns = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in Declared.Matches(script)) localFns.Add(m.Groups[1].Value);

        var unresolved = new SortedDictionary<string, int>(StringComparer.Ordinal);
        foreach (Match m in Called.Matches(script))
        {
            string name = m.Groups[1].Value;
            if (provided.Contains(name) || localFns.Contains(name) || Keywords.Contains(name)) continue;
            unresolved.TryGetValue(name, out int c);
            unresolved[name] = c + 1;
        }

        _out.WriteLine($"  {localFns.Count} function(s) defined by the map");
        _out.WriteLine($"  {unresolved.Count} called name(s) resolve to nothing");
        foreach (var (name, count) in unresolved.OrderByDescending(kv => kv.Value).Take(25))
            _out.WriteLine($"     {name}  called {count}x");
    }

    /// <summary>Language words that take a paren and are not calls.</summary>
    private static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "if", "loop", "while", "return", "set", "call", "local", "constant", "and", "or", "not",
        "function", "takes", "returns", "then", "endif", "endloop", "endfunction", "exitwhen",
        "elseif", "else", "globals", "endglobals", "native", "type", "extends", "array", "nothing",
        "true", "false", "null", "integer", "real", "string", "boolean", "code", "handle",
    };
}
