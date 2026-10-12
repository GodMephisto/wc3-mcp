// src/Wc3.Commands/ScriptUnusedGlobalsCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One global that is declared and never read. Line is 1-based in war3map.j.</summary>
public sealed record UnusedGlobal(string Name, string Type, string Declaration, int Line);

public sealed record ScriptUnusedGlobalsResult(
    string ScriptFile,
    int Declared,
    int Framework,
    int Unused,
    IReadOnlyList<UnusedGlobal> Globals);

/// <summary>
/// Finds the globals a script declares and never reads, the variable half of dead code.
///
/// A global that is only ever assigned, <c>set udg_Counter = ...</c>, and never appears on the
/// right of anything is reported too, since its value is never used. A read is any occurrence
/// other than the declaration itself or a pure assignment target, so <c>set g = g + 1</c> counts
/// as a read and <c>set g = 0</c> does not.
///
/// The World Editor's generated globals are excluded, because they are framework rather than the
/// author's: every <c>gg_</c> handle (a generated trigger, region, camera, unit or destructable),
/// every <c>bj_</c> name (the Blizzard.j convention the script checker already treats as
/// external), and the literal <c>worldedit</c>. A <c>udg_</c> name is the author's GUI variable
/// and IS checked, that is the whole point of the command.
/// </summary>
public static class ScriptUnusedGlobalsCommand
{
    private static readonly Regex SetStatement = new(
        @"^\s*set\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    public static ScriptUnusedGlobalsResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out var name, out var script, out _))
            return new(name, 0, 0, 0, Array.Empty<UnusedGlobal>());
        return Analyze(name, script);
    }

    internal static ScriptUnusedGlobalsResult Analyze(string scriptFile, string script)
    {
        var code = JassComments.Strip(JassLines.Split(script));
        var decls = new List<UnusedGlobal>();
        var index = new Dictionary<string, int>(StringComparer.Ordinal);
        var declLine = new Dictionary<int, string>();   // 0-based line index to its global's name
        int framework = 0;

        bool inGlobals = false;
        for (int i = 0; i < code.Length; i++)
        {
            var t = code[i].Trim();
            if (!inGlobals)
            {
                if (t == "globals") inGlobals = true;
                continue;
            }
            if (t == "endglobals") break;
            if (t.Length == 0) continue;

            var name = JassGlobals.NameOf(t);
            if (name is null) continue;
            if (IsFramework(name)) { framework++; continue; }
            if (index.ContainsKey(name)) continue;   // a duplicate declaration, first one wins
            index[name] = decls.Count;
            declLine[i] = name;
            decls.Add(new UnusedGlobal(name, JassGlobals.TypeOf(t)?.Type ?? "", t, i + 1));
        }

        // A global is read unless every mention of it is a pure write. The declaration line names
        // the global it declares, which is not a read of itself, so that one line's own name is
        // skipped while any OTHER global named on the same line (an initializer) still counts.
        var read = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < code.Length; i++)
        {
            var t = code[i].Trim();
            if (t.Length == 0) continue;
            var masked = JassCalls.Mask(t);
            var set = SetStatement.Match(masked);
            string? setTarget = set.Success ? set.Groups[1].Value : null;
            string? tail = set.Success ? masked[(set.Index + set.Length)..] : null;
            string? ownDecl = declLine.TryGetValue(i, out var n) ? n : null;

            foreach (var id in JassCalls.Identifiers(masked))
            {
                if (!index.ContainsKey(id)) continue;
                if (id == ownDecl) continue;                                   // its declaration
                if (id == setTarget && tail is not null
                    && !JassCalls.Identifiers(tail).Any(x => x == id)) continue; // a pure write
                read.Add(id);
            }
        }

        var unused = decls.Where(d => !read.Contains(d.Name)).ToList();
        return new(scriptFile, decls.Count, framework, unused.Count, unused);
    }

    private static bool IsFramework(string name) =>
        name.Equals("worldedit", StringComparison.OrdinalIgnoreCase)
        || name.StartsWith("gg_", StringComparison.Ordinal)
        || name.StartsWith("bj_", StringComparison.Ordinal);
}
