// src/Wc3.Commands/ScriptPorter.cs
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Best-effort JASS script port: carries a unit's trigger-function closure (and the
/// globals it references) from the source map's war3map.j into the target's, renaming
/// any symbol that collides with the target, rewriting rawcode literals to follow the
/// object remap, and hooking carried InitTrig_* functions into the target's init.
///
/// This is text surgery over an untyped, Turing-complete language — it is deliberately
/// conservative and always reports what it did. Tightly-coupled maps (a shared spell
/// dispatcher) produce very large closures; the mechanism still applies but the notes
/// flag that a large fraction of the source script was carried.
/// </summary>
internal static class ScriptPorter
{
    private static readonly Regex Ident = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);
    private static readonly Regex Rawcode = new(@"'(\\?.|[^'\\]{1,4})'", RegexOptions.Compiled);

    /// <summary>
    /// Splices the function closure into <paramref name="target"/> (mutated via
    /// AddOrReplaceRawFile) — or, with <paramref name="apply"/> false, computes the exact
    /// same <see cref="ScriptPortInfo"/> without touching the target (dry-run preview;
    /// one code path so preview and port cannot drift). <paramref name="codeRemap"/> maps
    /// source rawcode → target rawcode (as 4-char strings) for the objects that were
    /// remapped on collision; <paramref name="markerLabel"/> names the port in the spliced
    /// block's BEGIN/END comments (e.g. "Raiden Ei (H000)"). Returns null when there is
    /// nothing to port (no functions, or no target script).
    /// </summary>
    public static ScriptPortInfo? PortScript(
        MapDocument source, MapDocument target,
        IReadOnlyList<BundleFunction> functions, string markerLabel,
        IReadOnlyDictionary<string, string> codeRemap, bool apply = true)
    {
        var notes = new List<string>();
        if (functions.Count == 0) return null;

        var srcEntry = ScriptEntry(source);
        var tgtEntry = ScriptEntry(target);
        if (srcEntry is null) { notes.Add("source has no war3map.j — script not ported."); return new ScriptPortInfo(0, 0, 0, false, notes); }
        if (tgtEntry is null) { notes.Add("target has no war3map.j — script not ported."); return new ScriptPortInfo(0, 0, 0, false, notes); }

        string srcJ = Encoding.UTF8.GetString(srcEntry.RawBytes);
        string tgtJ = Encoding.UTF8.GetString(tgtEntry.RawBytes);
        var srcLines = srcJ.Replace("\r\n", "\n").Split('\n');

        // Closure function bodies, in source order.
        var closureNames = functions.Select(f => f.Name).ToHashSet(StringComparer.Ordinal);
        var srcFns = JassFunctionIndex.Parse(srcJ)
            .Where(f => closureNames.Contains(f.Name))
            .OrderBy(f => f.StartLine).ToList();
        if (srcFns.Count == 0) return new ScriptPortInfo(0, 0, 0, false, notes);
        if (srcFns.Count > 400)
            notes.Add($"{srcFns.Count} functions carried — the source script is highly coupled (likely a shared " +
                      "spell dispatcher), so a large fraction of it came along. Verify the ported map in-game.");

        // Globals referenced by the carried functions.
        var (srcGlobals, srcGlobalOrder) = ParseGlobals(srcLines);
        var used = new HashSet<string>(StringComparer.Ordinal);
        foreach (var f in srcFns)
            foreach (Match m in Ident.Matches(BodyText(srcLines, f)))
                if (srcGlobals.ContainsKey(m.Value)) used.Add(m.Value);
        var carriedGlobals = srcGlobalOrder.Where(used.Contains).ToList();

        // Symbols the target already defines (functions + its own globals).
        var targetSymbols = JassFunctionIndex.Parse(tgtJ).Select(f => f.Name).ToHashSet(StringComparer.Ordinal);
        foreach (var g in ParseGlobals(tgtJ.Replace("\r\n", "\n").Split('\n')).Item1.Keys) targetSymbols.Add(g);

        // Rename any carried symbol that collides with the target (or with common.j-ish
        // reserved names we can't see); rewrite references within the ported block only.
        var rename = new Dictionary<string, string>(StringComparer.Ordinal);
        var carriedSymbols = srcFns.Select(f => f.Name).Concat(carriedGlobals).ToList();
        var taken = new HashSet<string>(targetSymbols, StringComparer.Ordinal);
        foreach (var name in carriedSymbols)
        {
            if (!taken.Contains(name)) { taken.Add(name); continue; }
            string fresh = name;
            int n = 1;
            while (taken.Contains(fresh)) fresh = $"{name}_p{n++}";
            rename[name] = fresh;
            taken.Add(fresh);
        }

        string Rewrite(string code) => RewriteRawcodes(ApplyRenames(code, rename, carriedSymbols), codeRemap);

        // Build the ported globals + functions text.
        var portedGlobals = new StringBuilder();
        foreach (var g in carriedGlobals)
            portedGlobals.Append("    ").Append(Rewrite(srcGlobals[g].Trim())).Append('\n');

        var portedFns = new StringBuilder();
        foreach (var f in srcFns)
            portedFns.Append(Rewrite(BodyText(srcLines, f))).Append('\n');

        // Splice into the target: globals into its globals block, functions after endglobals.
        string marker = $"wc3ctl ported: {markerLabel}";
        string merged = Splice(tgtJ, portedGlobals.ToString(), portedFns.ToString(), marker, notes);

        // Best-effort init hook: call carried InitTrig_* functions from InitCustomTriggers.
        var initFns = srcFns.Select(f => f.Name)
            .Where(nm => nm.StartsWith("InitTrig_", StringComparison.Ordinal))
            .Select(nm => rename.GetValueOrDefault(nm, nm)).ToList();
        bool hooked = false;
        if (initFns.Count > 0)
        {
            merged = HookInit(merged, initFns, marker, out hooked);
            notes.Add(hooked
                ? $"wired {initFns.Count} InitTrig_* function(s) into the target's InitCustomTriggers."
                : "carried InitTrig_* functions but could not find InitCustomTriggers in the target — " +
                  "trigger registration may need to be wired manually.");
        }
        else
        {
            notes.Add("no InitTrig_* init functions in the closure — if the spells register via a custom " +
                      "init path, verify it runs in the target.");
        }

        if (apply)
            target.AddOrReplaceRawFile(tgtEntry.FileName!, Encoding.UTF8.GetBytes(merged));
        return new ScriptPortInfo(srcFns.Count, carriedGlobals.Count, rename.Count, hooked, notes);
    }

    private static MapFileEntry? ScriptEntry(MapDocument doc) =>
        doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");

    private static string BodyText(string[] lines, JassFunction f)
    {
        var sb = new StringBuilder();
        for (int i = f.StartLine - 1; i < f.EndLine && i < lines.Length; i++)
            sb.Append(lines[i]).Append('\n');
        return sb.ToString();
    }

    /// <summary>Global name → its full declaration line, plus declaration order.</summary>
    private static (Dictionary<string, string>, List<string>) ParseGlobals(string[] lines)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        var order = new List<string>();
        bool inBlock = false;
        foreach (var raw in lines)
        {
            var t = raw.Trim();
            if (!inBlock) { if (t == "globals") inBlock = true; continue; }
            if (t == "endglobals") { inBlock = false; continue; }
            if (t.Length == 0 || t.StartsWith("//", StringComparison.Ordinal)) continue;
            var name = GlobalName(t);
            if (name is not null && !map.ContainsKey(name)) { map[name] = t; order.Add(name); }
        }
        return (map, order);
    }

    private static string? GlobalName(string decl)
    {
        var toks = decl.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int i = 0;
        if (i < toks.Length && toks[i] == "constant") i++;
        i++; // type
        if (i < toks.Length && toks[i] == "array") i++;
        if (i >= toks.Length) return null;
        return toks[i].Split('=')[0].Trim();
    }

    private static string ApplyRenames(string code, IReadOnlyDictionary<string, string> rename, IEnumerable<string> _)
    {
        if (rename.Count == 0) return code;
        return Ident.Replace(code, m => rename.TryGetValue(m.Value, out var r) ? r : m.Value);
    }

    private static string RewriteRawcodes(string code, IReadOnlyDictionary<string, string> remap)
    {
        if (remap.Count == 0) return code;
        return Rawcode.Replace(code, m =>
        {
            var inner = m.Groups[1].Value;
            return inner.Length == 4 && remap.TryGetValue(inner, out var to) ? $"'{to}'" : m.Value;
        });
    }

    private static string Splice(string tgtJ, string portedGlobals, string portedFns, string marker, List<string> notes)
    {
        string nl = tgtJ.Contains("\r\n") ? "\r\n" : "\n";
        var text = tgtJ.Replace("\r\n", "\n");
        string block = $"\n// ==== BEGIN {marker} ====\n{portedFns}// ==== END {marker} ====\n";

        int endGlobals = FindLine(text, "endglobals");
        if (endGlobals >= 0 && portedGlobals.Length > 0)
        {
            int insertAt = text.LastIndexOf('\n', endGlobals);
            string gblock = $"\n    // ==== {marker} (globals) ====\n{portedGlobals}";
            text = text.Insert(insertAt < 0 ? endGlobals : insertAt, gblock);
            endGlobals = FindLine(text, "endglobals"); // shifted
        }
        else if (portedGlobals.Length > 0)
        {
            // No globals block in target: prepend one.
            text = $"globals\n{portedGlobals}endglobals\n" + text;
            notes.Add("target had no globals block — one was created for the ported globals.");
            endGlobals = FindLine(text, "endglobals");
        }

        // Functions go right after endglobals (so target code can call them; JASS is top-down).
        int after = endGlobals >= 0 ? text.IndexOf('\n', endGlobals) + 1 : 0;
        text = text.Insert(after, block);
        return text.Replace("\n", nl);
    }

    private static string HookInit(string mergedJ, IReadOnlyList<string> initFns, string marker, out bool hooked)
    {
        hooked = false;
        string nl = mergedJ.Contains("\r\n") ? "\r\n" : "\n";
        var text = mergedJ.Replace("\r\n", "\n");
        var idx = JassFunctionIndex.Parse(text)
            .FirstOrDefault(f => f.Name == "InitCustomTriggers");
        if (idx is null) return mergedJ;

        var lines = text.Split('\n').ToList();
        var calls = initFns.Select(fn => $"    call {fn}() // {marker}").ToList();
        lines.InsertRange(idx.EndLine - 1, calls); // before the "endfunction" line
        hooked = true;
        return string.Join('\n', lines).Replace("\n", nl);
    }

    private static int FindLine(string text, string exactTrimmed)
    {
        int pos = 0;
        foreach (var line in text.Split('\n'))
        {
            if (line.Trim() == exactTrimmed) return pos;
            pos += line.Length + 1;
        }
        return -1;
    }
}
