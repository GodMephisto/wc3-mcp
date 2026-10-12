// src/Wc3.Commands/ScriptNullLocalsCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One handle local that is destroyed but left non-null. Lines are 1-based in war3map.j.</summary>
public sealed record NullLocal(
    string Function, string Name, string Type, int Line, int DestroyedAtLine, string Destroyer);

public sealed record ScriptNullLocalsResult(
    string ScriptFile,
    int Checked,
    int Findings,
    IReadOnlyList<NullLocal> Locals);

/// <summary>
/// Finds the classic JASS handle leak: a local of a handle type that the function destroys with
/// the matching Remove or Destroy but never sets to <c>null</c>.
///
/// JASS frees the handle's resources when it is destroyed, but the local still holds the value
/// until the function returns, so the handle id stays reserved for the rest of the function. On a
/// loop body or a callback that runs every tick, every iteration holds one id, and the reference
/// count for that handle type grows without bound. The fix is the two-line idiom the command looks
/// for the absence of, destroy then <c>set x = null</c>.
///
/// Only the unambiguous shape is reported. A local that is never destroyed is a different leak and
/// belongs to <see cref="ScriptLeaksCommand"/>; this command stays silent on it. Array locals are
/// skipped, since an array slot cannot be nulled as a whole.
/// </summary>
public static class ScriptNullLocalsCommand
{
    /// <summary>The handle type to the Remove or Destroy call that frees it.</summary>
    private static readonly Dictionary<string, string[]> Destroyers = new(StringComparer.Ordinal)
    {
        ["location"] = new[] { "RemoveLocation" },
        ["group"] = new[] { "DestroyGroup" },
        ["force"] = new[] { "DestroyForce" },
        ["effect"] = new[] { "DestroyEffect", "DestroyEffectBJ" },
        ["timer"] = new[] { "DestroyTimer" },
        ["trigger"] = new[] { "DestroyTrigger" },
        ["texttag"] = new[] { "DestroyTextTag" },
        ["lightning"] = new[] { "DestroyLightning", "DestroyLightningBJ" },
    };

    public static ScriptNullLocalsResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out var name, out var script, out _))
            return new(name, 0, 0, Array.Empty<NullLocal>());
        return Analyze(name, script);
    }

    internal static ScriptNullLocalsResult Analyze(string scriptFile, string script)
    {
        var code = JassComments.Strip(JassLines.Split(script));
        var functions = JassFunctionIndex.Parse(script);

        var findings = new List<NullLocal>();
        int checkedLocals = 0;
        foreach (var f in functions)
        {
            for (int i = f.StartLine; i < f.EndLine - 1 && i < code.Length; i++)
            {
                var t = code[i].TrimStart();
                if (!t.StartsWith("local ", StringComparison.Ordinal)) continue;
                var decl = t["local ".Length..];
                var type = JassGlobals.TypeOf(decl);
                if (type is null || type.Value.IsArray) continue;
                if (!Destroyers.TryGetValue(type.Value.Type, out var destroyCalls)) continue;
                var name = JassGlobals.NameOf(decl);
                if (name is null) continue;
                checkedLocals++;

                int destroyedAt = 0;
                string? destroyer = null;
                bool nulled = false;
                for (int j = f.StartLine; j < f.EndLine - 1 && j < code.Length; j++)
                {
                    if (destroyedAt == 0)
                        foreach (var d in destroyCalls)
                            if (Regex.IsMatch(code[j], $@"\b{d}\s*\(\s*{Regex.Escape(name)}\b"))
                            { destroyedAt = j + 1; destroyer = d; break; }
                    if (!nulled && Regex.IsMatch(code[j], $@"\bset\s+{Regex.Escape(name)}\s*=\s*null\b"))
                        nulled = true;
                }

                if (destroyedAt > 0 && !nulled)
                    findings.Add(new NullLocal(f.Name, name, type.Value.Type, i + 1, destroyedAt, destroyer!));
            }
        }

        findings = findings.OrderBy(x => x.Line).ToList();
        return new(scriptFile, checkedLocals, findings.Count, findings);
    }
}
