// src/Wc3.Commands/CarriedCastDispatch.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A carried cast dispatcher that nothing drives, and the wiring generated for it.</summary>
public sealed record CastDispatchWiring(string ConditionFunction, string GeneratedScript, string InitCall);

/// <summary>
/// Wires up a cast dispatcher the port carried but left dead.
///
/// Export walks a hero's script closure and brings her spell code with her, including the source
/// map's cast dispatcher, the <c>takes nothing returns boolean</c> condition that reads
/// <c>GetSpellAbilityId()</c> and calls the right spell-start function. What it cannot bring is the
/// TRIGGER, because a trigger is a runtime handle created by the source map's own init, not a
/// function in the closure. So the dispatcher arrives complete, compiles, and is never called by
/// anything. Measured on GGGA: the installed hero's dispatcher
/// (<c>hH0DA__GearHero_Conditions</c>) is declared and referenced nowhere, every one of her
/// spell-start functions is unreachable, and her abilities do nothing when cast. That is the
/// difference between a hero who spawns and a hero who plays.
///
/// The generated wiring is deliberately narrow. A carried dispatcher branches on unit type for
/// EVERY hero of the source map, and those source rawcodes can collide with real heroes of the
/// target, so registering it as-is would make the target's own Kirito run the source map's Kirito
/// spells. The gate here returns false unless the casting unit is the installed hero, so only her
/// branch can ever be reached and no other unit's cast can enter carried code.
///
/// Wired through <c>ExecuteFunc</c> from the target's <c>main</c> rather than by a direct call,
/// because the carried body is appended AFTER <c>main</c> and vanilla JASS resolves direct calls by
/// declaration order. <c>ExecuteFunc</c> resolves by name at runtime, which is the only way to call
/// forward.
/// </summary>
public static class CarriedCastDispatch
{
    private static readonly Regex BoolFunction =
        new(@"^\s*function\s+([A-Za-z_]\w*)\s+takes\s+nothing\s+returns\s+boolean\b",
            RegexOptions.Compiled);

    /// <summary>What makes a boolean function a cast dispatcher rather than an ordinary filter.</summary>
    private const string DispatchMarker = "GetSpellAbilityId()";

    /// <summary>
    /// Finds carried dispatchers nothing drives and returns the wiring for them. <paramref
    /// name="head"/> is the target's existing script, needed because a dispatcher the TARGET
    /// already references must be left alone.
    /// </summary>
    public static IReadOnlyList<CastDispatchWiring> Wire(string head, string body, string rawcode)
    {
        var result = new List<CastDispatchWiring>();
        var bodyLines = body.Split('\n');
        var stripped = JassComments.Strip(bodyLines);

        foreach (var fn in JassFunctionIndex.Parse(body))
        {
            int declaration = Math.Max(0, fn.StartLine - 1);
            if (declaration >= stripped.Length) continue;
            if (BoolFunction.Match(stripped[declaration]) is not { Success: true } m) continue;
            if (!string.Equals(m.Groups[1].Value, fn.Name, StringComparison.Ordinal)) continue;

            bool dispatches = false;
            for (int i = declaration; i < Math.Min(fn.EndLine, stripped.Length); i++)
                if (stripped[i].Contains(DispatchMarker, StringComparison.Ordinal)) { dispatches = true; break; }
            if (!dispatches) continue;

            // A dispatcher the hero cannot reach is not hers to wire.
            bool mentionsHero = false;
            for (int i = declaration; i < Math.Min(fn.EndLine, stripped.Length); i++)
                if (stripped[i].Contains("'" + rawcode + "'", StringComparison.Ordinal)) { mentionsHero = true; break; }

            // Named anywhere other than its own declaration means something already drives it.
            if (References(stripped, fn.Name, declaration) || Mentions(head, fn.Name)) continue;
            // Aliases are searched over the target's script as well as the carried body, because a
            // definition declares its rawcode globals in hero.json and install writes them into the
            // TARGET's globals block. Searching only the body finds nothing and the real dispatcher
            // is skipped, which is exactly what happened the first time this ran on GGGA.
            if (!mentionsHero
                && !MentionsThroughAlias(stripped, declaration, fn.EndLine, head + "\n" + body, rawcode))
                continue;

            result.Add(new CastDispatchWiring(fn.Name, Generate(fn.Name, rawcode), InitCallFor(fn.Name, rawcode)));
        }
        return result;
    }

    /// <summary>
    /// A carried dispatcher usually compares against a global (<c>hH0DA__DarkShiki_ID</c>) rather
    /// than a literal, so a literal-only search would miss the hero it actually serves. This
    /// resolves those globals through the carried script's own <c>integer NAME= 'XXXX'</c>
    /// declarations.
    /// </summary>
    private static bool MentionsThroughAlias(string[] stripped, int from, int to, string body, string rawcode)
    {
        var aliases = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in Regex.Matches(body,
                     @"\b(?:constant\s+)?integer\s+([A-Za-z_]\w*)\s*=\s*'([^'\r\n]{4})'"))
            if (string.Equals(m.Groups[2].Value, rawcode, StringComparison.Ordinal))
                aliases.Add(m.Groups[1].Value);
        if (aliases.Count == 0) return false;

        for (int i = from; i < Math.Min(to, stripped.Length); i++)
            foreach (Match m in Regex.Matches(stripped[i], @"\b[A-Za-z_]\w*\b"))
                if (aliases.Contains(m.Value)) return true;
        return false;
    }

    private static bool References(string[] stripped, string name, int declaration)
    {
        var pattern = new Regex(@"\b" + Regex.Escape(name) + @"\b", RegexOptions.Compiled);
        for (int i = 0; i < stripped.Length; i++)
            if (i != declaration && pattern.IsMatch(stripped[i])) return true;
        return false;
    }

    private static bool Mentions(string head, string name) =>
        Regex.IsMatch(head, @"\b" + Regex.Escape(name) + @"\b");

    private static string GateFor(string fn, string rawcode) => "wc3ctl_CastGate_" + rawcode + "_" + fn;
    private static string WireFor(string fn, string rawcode) => "wc3ctl_WireCast_" + rawcode + "_" + fn;

    public static string InitCallFor(string fn, string rawcode) =>
        "    call ExecuteFunc(\"" + WireFor(fn, rawcode) + "\")";

    private static string Generate(string fn, string rawcode)
    {
        var gate = GateFor(fn, rawcode);
        var wire = WireFor(fn, rawcode);
        var sb = new StringBuilder();
        sb.Append("\n// wc3ctl: the port carried '").Append(fn).Append("' and nothing drove it, so the\n");
        sb.Append("// hero's spells were unreachable. Gated to '").Append(rawcode).Append("' because a\n");
        sb.Append("// carried dispatcher also branches on the SOURCE map's other heroes, whose rawcodes\n");
        sb.Append("// can be real units here.\n");
        sb.Append("function ").Append(gate).Append(" takes nothing returns boolean\n");
        sb.Append("    if GetUnitTypeId(GetSpellAbilityUnit()) != '").Append(rawcode).Append("' then\n");
        sb.Append("        return false\n");
        sb.Append("    endif\n");
        sb.Append("    return ").Append(fn).Append("()\n");
        sb.Append("endfunction\n\n");
        sb.Append("function ").Append(wire).Append(" takes nothing returns nothing\n");
        sb.Append("    local trigger t= CreateTrigger()\n");
        sb.Append("    call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)\n");
        sb.Append("    call TriggerAddCondition(t, Condition(function ").Append(gate).Append("))\n");
        sb.Append("    set t=null\n");
        sb.Append("endfunction\n");
        return sb.ToString();
    }

    /// <summary>
    /// Puts the init calls at the end of <c>main</c>. Appending after <c>endfunction</c> would put
    /// them at file scope, which does not compile, and any earlier point risks running before the
    /// map has built the state a carried dispatcher may read.
    /// </summary>
    public static string InsertIntoMain(string head, IEnumerable<string> calls)
    {
        var list = calls.ToList();
        if (list.Count == 0) return head;
        var lines = head.Split('\n');
        var main = JassFunctionIndex.Parse(head)
            .FirstOrDefault(f => string.Equals(f.Name, "main", StringComparison.Ordinal));
        if (main is null) return head;

        int at = main.EndLine - 1;               // the endfunction line
        if (at < 0 || at > lines.Length) return head;
        string eol = lines.Length > 0 && lines[0].EndsWith('\r') ? "\r" : "";
        var output = new List<string>(lines);
        output.InsertRange(at, list.Select(c => c + eol));
        return string.Join("\n", output);
    }
}
