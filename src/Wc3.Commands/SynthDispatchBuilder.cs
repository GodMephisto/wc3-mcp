// src/Wc3.Commands/SynthDispatchBuilder.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Reads one hero's OWN branch out of a source map's shared spellcast dispatcher (the
/// "if GetUnitTypeId(c) == Hero_ID then if id == HeroQ_ID then call ... endif endif" shape a GUI
/// hero arena commonly compiles down to) and turns it into the calls a fresh, self-contained
/// dispatcher needs, verbatim.
///
/// Why this exists: the shared dispatcher a hero's own cast logic lives inside is written to
/// satisfy every gate the SOURCE map wired around it, a placed-hero registration array, a map rect
/// that stays null off the source map, a cooldown hashtable. Carrying that dispatcher whole
/// carries every one of those gates too, and a ported hero fails every one of them at once. The
/// hero's OWN branch names exactly which handler each of its abilities calls, so reading it (never
/// executing it) is enough to synthesize a minimal dispatcher that skips every gate entirely,
/// see <see cref="ScriptPorter"/> for where this is spliced in (opt-in, behind --synth-dispatch).
///
/// Pure text analysis, same idiom as <see cref="ScriptPorter"/> and <see cref="BundleCommand"/>,
/// deliberately conservative, never assumes the shape and reports null rather than guessing.
/// </summary>
internal static class SynthDispatchBuilder
{
    /// <summary>One ability the hero's own branch dispatches: its rawcode, and the exact
    /// "call Foo(...)" statement(s) that branch runs, copied byte-for-byte (arguments untouched)
    /// from the source. <see cref="Callees"/> are the function names those calls invoke, fed back
    /// into the porter's carry set so a combo ability granted only through a shared runtime helper
    /// (invisible to a plain call-graph walk) is still carried.</summary>
    public sealed record CastBranch(string AbilityRawcode, IReadOnlyList<string> Calls, IReadOnlyList<string> Callees);

    private static readonly Regex CalleeName = new(
        @"^call\s+([A-Za-z_][A-Za-z0-9_]*)\s*\(", RegexOptions.Compiled);

    private static readonly Regex AbilityLevelZeroGuard = new(
        @"GetUnitAbilityLevel\s*\(\s*(?<unit>[A-Za-z_][A-Za-z0-9_]*)\s*,\s*(?<ability>'[^']{4}'|[A-Za-z_][A-Za-z0-9_]*)\s*\)\s*==\s*0",
        RegexOptions.Compiled);

    /// <summary>
    /// Every ability branch found under <paramref name="heroRawcode"/>'s own
    /// "GetUnitTypeId(...) == HeroId" guard(s), anywhere in <paramref name="sourceJass"/>, merged
    /// and de-duplicated by ability rawcode (first occurrence wins). Null when the script has no
    /// such guard for this hero at all (an unsupported dispatch shape, or the hero casts through
    /// something other than a shared id-comparison dispatcher) — the caller must not synthesize
    /// anything in that case, not guess. An empty (non-null) result never happens; a guard with
    /// zero resolvable ability branches is treated the same as "no guard found".
    /// </summary>
    public static IReadOnlyList<CastBranch>? ExtractHeroCastBranches(string sourceJass, string heroRawcode)
    {
        if (string.IsNullOrEmpty(sourceJass)) return null;

        var aliases = JassRawcodeAliases.Parse(sourceJass);
        var lines = sourceJass.Replace("\r\n", "\n").Split('\n');
        var functions = JassFunctionIndex.Parse(sourceJass);

        var result = new List<CastBranch>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        ForEachHeroBranch(functions, lines, heroRawcode, aliases, branch =>
        {
            foreach (var cb in ExtractAbilityBranches(lines, branch.BodyStart, branch.BodyEnd, aliases))
                if (seen.Add(cb.AbilityRawcode)) result.Add(cb);
        });
        return result.Count > 0 ? result : null;
    }

    /// <summary>One ability a hero grants itself on level up, guarded the idempotent way the source
    /// arena already writes it (<c>if GetUnitAbilityLevel(unit, X) == 0 then ...</c>), so re-emitting
    /// this at spawn is safe even if the hero later actually does level up too. <see cref="Condition"/>
    /// and <see cref="Calls"/> are copied verbatim from the source, <see cref="UnitParam"/> names which
    /// identifier in them stands for the hero unit, so the caller can substitute its own local for it
    /// (a preplaced hero has no <c>GetTriggerUnit()</c> to read one from).</summary>
    public sealed record AbilityGrant(string AbilityRawcode, string UnitParam, string Condition, IReadOnlyList<string> Calls);

    /// <summary>
    /// Every ability <paramref name="heroRawcode"/> grants itself inside its own top level guard in a
    /// level up (or similar "run this once a condition becomes true") handler, anywhere in
    /// <paramref name="sourceJass"/>. Reuses the exact same hero guard recognised for a cast dispatcher
    /// (<see cref="ForEachHeroBranch"/>), since a level up handler is written the same way, one shared
    /// function dispatching every hero's own branch off a single "if HeroId == id then" guard.
    ///
    /// Only the idempotent-guarded shape qualifies, a depth-0 <c>if GetUnitAbilityLevel(unit, X) == 0
    /// then ... endif</c> nested directly inside the hero's own branch, unit a bare local (never a call
    /// expression, there would be nothing to rename it to for a freshly created unit). Anything else in
    /// the branch (a level threshold, a saved flag, a one-shot ability-tier upgrade) is left alone, not
    /// because it is unsafe in principle but because there is no guard here that makes replaying it at
    /// spawn idempotent the way GetUnitAbilityLevel(...) == 0 is, and guessing would risk carrying
    /// another hero's side effect the way an unscoped UnitAddAbility scan would. Null when the hero has
    /// no such branch at all, or the branch grants nothing in this shape, the caller must emit nothing
    /// in that case rather than guess.
    /// </summary>
    public static IReadOnlyList<AbilityGrant>? ExtractHeroLevelUpGrants(string sourceJass, string heroRawcode)
    {
        if (string.IsNullOrEmpty(sourceJass)) return null;

        var aliases = JassRawcodeAliases.Parse(sourceJass);
        var lines = sourceJass.Replace("\r\n", "\n").Split('\n');
        var functions = JassFunctionIndex.Parse(sourceJass);

        var result = new List<AbilityGrant>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        ForEachHeroBranch(functions, lines, heroRawcode, aliases, branch =>
        {
            foreach (var g in ExtractAbilityGrants(lines, branch.BodyStart, branch.BodyEnd, aliases))
                if (seen.Add(g.AbilityRawcode)) result.Add(g);
        });
        return result.Count > 0 ? result : null;
    }

    /// <summary>A safe textual rename of a bare JASS identifier, every whole-token occurrence of
    /// <paramref name="oldName"/> in <paramref name="text"/> becomes <paramref name="newName"/>, an
    /// occurrence embedded in a longer identifier (the "G" inside "AstaG_ID") is left alone. Used to
    /// retarget an extracted grant's unit local onto whatever local the caller's own generated code
    /// created the hero into.</summary>
    public static string ReplaceIdentifier(string text, string oldName, string newName) =>
        oldName == newName ? text : Regex.Replace(text, @"\b" + Regex.Escape(oldName) + @"\b", newName);

    /// <summary>
    /// Every ability branch inside <paramref name="functionName"/>'s OWN body directly, no enclosing
    /// hero guard, the shape <see cref="BuildRawText"/> itself generates for a synthesized dispatcher
    /// (an unconditional "if id == 'A0DL' then ... elseif id == 'A0DM' then ... endif" chain, already
    /// scoped to one hero since the whole function is). Reuses the exact same branch/call extraction
    /// <see cref="ExtractHeroCastBranches"/> uses once IT has found a hero's guard, entered here
    /// without that search since a --synth-dispatch dispatcher already represents exactly one hero.
    ///
    /// This is <see cref="AbilityAuditCommand"/>'s way to read a per-ability call closure back out of
    /// an already-ported map for a --synth-dispatch hero, the same way <see cref="ExtractHeroCastBranches"/>
    /// does it for a plain-ported one (which still carries the source's hero-guarded shared dispatcher).
    /// Null when the function is not declared, or declares no ability branch at all.
    /// </summary>
    internal static IReadOnlyList<CastBranch>? ExtractBranchesFromDispatchFunction(string jass, string functionName)
    {
        var f = JassFunctionIndex.Parse(jass).FirstOrDefault(x => x.Name == functionName);
        if (f is null) return null;

        var aliases = JassRawcodeAliases.Parse(jass);
        var lines = jass.Replace("\r\n", "\n").Split('\n');
        int bodyStart = f.StartLine;                                  // just past "function ... takes"
        int bodyEnd = Math.Min(f.EndLine - 1, lines.Length);           // just before "endfunction"
        var result = ExtractAbilityBranches(lines, bodyStart, bodyEnd, aliases);
        return result.Count > 0 ? result : null;
    }

    /// <summary>The bare "trigger gg_trg_..." declaration <paramref name="triggerGlobal"/> needs in
    /// the target's globals block (no initializer, JASS convention for every gg_trg_* the World
    /// Editor itself emits, and how HeroWiringAudit's static analysis recognizes an attached
    /// trigger, its own name is not enough on its own). The caller adds it alongside the other
    /// carried globals.</summary>
    public static string BuildGlobalDeclaration(string triggerGlobal) => "trigger " + triggerGlobal;

    /// <summary>Builds the raw (pre-rename, pre-rawcode-remap) text of the synthesized cast
    /// dispatcher: a fresh function testing <c>GetSpellAbilityId()</c> against each branch's
    /// literal rawcode and running its call(s) verbatim, plus a companion init function that
    /// assigns <paramref name="triggerGlobal"/> its own trigger and registers it for every
    /// player's spell-effect event (an any-unit registration, so it needs no per-player wiring and
    /// no placed-hero registration array). Named with the gg_trg_ prefix (not a plain local), the
    /// same convention every World-Editor-generated trigger uses, so this reads as a real,
    /// end-to-end wired trigger to anything that looks for one, this codebase's own
    /// HeroWiringAudit included, not just to a human reading the script.
    /// The caller is expected to run this through the SAME rename/rawcode-remap pass every other
    /// carried body gets, so a collision-driven rename of a callee or a remap of an ability's
    /// rawcode (from the object port) is picked up exactly like anywhere else.</summary>
    public static string BuildRawText(string dispatchFunctionName, string initFunctionName,
        string triggerGlobal, IReadOnlyList<CastBranch> branches)
    {
        var sb = new StringBuilder();
        sb.Append("function ").Append(dispatchFunctionName).Append(" takes nothing returns nothing\n");
        sb.Append("    local unit c = GetSpellAbilityUnit()\n");
        sb.Append("    local integer id = GetSpellAbilityId()\n");
        sb.Append("    local real x = GetSpellTargetX()\n");
        sb.Append("    local real y = GetSpellTargetY()\n");
        sb.Append("    local unit td = GetSpellTargetUnit()\n");
        for (int i = 0; i < branches.Count; i++)
        {
            sb.Append("    ").Append(i == 0 ? "if" : "elseif").Append(" id == '")
              .Append(branches[i].AbilityRawcode).Append("' then\n");
            foreach (var call in branches[i].Calls)
                sb.Append("        ").Append(call).Append('\n');
        }
        if (branches.Count > 0) sb.Append("    endif\n");
        sb.Append("    set c = null\n");
        sb.Append("    set td = null\n");
        sb.Append("endfunction\n");

        sb.Append("function ").Append(initFunctionName).Append(" takes nothing returns nothing\n");
        sb.Append("    set ").Append(triggerGlobal).Append(" = CreateTrigger()\n");
        sb.Append("    call TriggerAddAction(").Append(triggerGlobal).Append(", function ")
          .Append(dispatchFunctionName).Append(")\n");
        sb.Append("    call TriggerRegisterAnyUnitEventBJ(").Append(triggerGlobal)
          .Append(", EVENT_PLAYER_UNIT_SPELL_EFFECT)\n");
        sb.Append("endfunction\n");
        return sb.ToString();
    }

    /// <summary>A safe JASS identifier fragment out of a rawcode, a human-readable port label
    /// ("Asta (H028)"), or anything else: non alphanumeric bytes become underscores, then any run
    /// of underscores is trimmed off BOTH ends (an embedded one, "Asta__H028", is left alone, only
    /// a boundary one is a problem), and a leading digit (or an empty result) gets an "x" prefix,
    /// a letter, never an underscore.
    ///
    /// The real World Editor JASS parser (pjass) rejects an identifier that starts OR ends with an
    /// underscore outright ("Unrecognized character _"), embedded runs are fine. This codebase's
    /// own lenient JassScriptCheck does not catch that shape at all, so a name built by naively
    /// replacing "(" and ")" with "_" (a marker label always ends in one) passed this tool's own
    /// compile gate and then failed the real compiler, an uncompilable map is worse than any
    /// cosmetic ugliness in the generated name, so this is a correctness fix, not a style one.
    /// </summary>
    public static string SanitizeIdentifier(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var c in s) sb.Append(char.IsLetterOrDigit(c) ? c : '_');

        int start = 0;
        while (start < sb.Length && sb[start] == '_') start++;
        int end = sb.Length;
        while (end > start && sb[end - 1] == '_') end--;
        string trimmed = sb.ToString(start, end - start);

        if (trimmed.Length == 0) return "x";
        return char.IsDigit(trimmed[0]) ? "x" + trimmed : trimmed;
    }

    // ---- one hero guard --------------------------------------------------------------------

    private sealed record HeroBranch(int BodyStart, int BodyEnd);

    /// <summary>Walks every function in <paramref name="lines"/>, and invokes
    /// <paramref name="onBranchFound"/> once for each occurrence of <paramref name="heroRawcode"/>'s
    /// own top level if/elseif guard, wherever that shape recurs (a cast dispatcher, a level up
    /// handler, anything else an arena writes as one shared function branching on the hero's id).
    /// Shared so every caller of <see cref="TryFindHeroBranch"/> agrees on what counts as the hero's
    /// own branch, one open-ifs walk instead of a second copy of it per caller.</summary>
    private static void ForEachHeroBranch(
        IReadOnlyList<JassFunction> functions, IReadOnlyList<string> lines, string heroRawcode,
        IReadOnlyDictionary<string, string> aliases, Action<HeroBranch> onBranchFound)
    {
        foreach (var f in functions)
        {
            int start = f.StartLine - 1, end = Math.Min(f.EndLine, lines.Count);
            var openIfs = new Stack<int>(); // line index of each currently-open "if", for "elseif"

            for (int i = start; i < end; i++)
            {
                var head = lines[i].TrimStart();
                if (StartsWithWord(head, "if"))
                {
                    openIfs.Push(i);
                    if (TryFindHeroBranch(lines, i, i, start, end, heroRawcode, aliases) is { } branch)
                        onBranchFound(branch);
                }
                else if (StartsWithWord(head, "elseif"))
                {
                    if (openIfs.Count > 0
                        && TryFindHeroBranch(lines, openIfs.Peek(), i, start, end, heroRawcode, aliases) is { } branch)
                        onBranchFound(branch);
                }
                else if (StartsWithWord(head, "endif"))
                {
                    if (openIfs.Count > 0) openIfs.Pop();
                }
            }
        }
    }

    /// <summary>When the "if"/"elseif" at <paramref name="conditionLine"/> guards on
    /// <paramref name="heroRawcode"/>, splits the chain it opens (starting at
    /// <paramref name="chainStart"/>) and returns that one branch's body range. Null otherwise, or
    /// when the chain never closes (malformed input). <paramref name="funcStart"/>/<paramref name="funcEnd"/>
    /// bound the enclosing function, needed only for the relaxed guard shape below.</summary>
    private static HeroBranch? TryFindHeroBranch(
        IReadOnlyList<string> lines, int chainStart, int conditionLine, int funcStart, int funcEnd,
        string heroRawcode, IReadOnlyDictionary<string, string> aliases)
    {
        var condition = ConditionOf(lines[conditionLine]);
        if (!ConditionNamesHero(condition, heroRawcode, aliases, lines, funcStart, funcEnd)) return null;

        if (SplitIfChain(lines, chainStart) is not { } chain) return null;
        var mine = chain.Branches.FirstOrDefault(b => b.ConditionLine == conditionLine);
        return mine is null ? null : new HeroBranch(mine.BodyStart, mine.BodyEnd);
    }

    /// <summary>Whether <paramref name="condition"/> is <paramref name="heroRawcode"/>'s own hero
    /// check, either the shape a cast dispatcher's guard usually compiles to
    /// (<c>GetUnitTypeId(...) == HeroId</c>, the "..." a bare local the map cached the caster into,
    /// as Anime_WOS2 writes it, OR the accessor called inline with no caching local at all,
    /// <c>GetUnitTypeId(GetSpellAbilityUnit()) == HeroId</c>, as Anime Choice Arena writes it), or
    /// <c>HeroId == id</c> where <c>id</c> is a LOCAL the enclosing function assigned from
    /// <c>GetUnitTypeId</c> earlier (the shape a handler that already holds the unit type in a local
    /// compiles to, since it has no reason to call GetUnitTypeId a second time inline, a level up
    /// handler being the case this was written for).</summary>
    private static bool ConditionNamesHero(
        string condition, string heroRawcode, IReadOnlyDictionary<string, string> aliases,
        IReadOnlyList<string> lines, int funcStart, int funcEnd)
    {
        if (SplitTopLevelEquality(condition) is not { } eq) return false;

        // Either side may itself be a call (GetUnitTypeId(...), or the inner accessor it wraps),
        // never just a bare token, so this checks for the SUBSTRING rather than trying to resolve
        // the operand as a whole the way the ability-rawcode and local-alias checks below do.
        if (eq.Left.Contains("GetUnitTypeId", StringComparison.Ordinal)
            && string.Equals(Resolve(eq.Right, aliases), heroRawcode, StringComparison.Ordinal))
            return true;
        if (eq.Right.Contains("GetUnitTypeId", StringComparison.Ordinal)
            && string.Equals(Resolve(eq.Left, aliases), heroRawcode, StringComparison.Ordinal))
            return true;

        string? other = string.Equals(Resolve(eq.Left, aliases), heroRawcode, StringComparison.Ordinal) ? eq.Right
            : string.Equals(Resolve(eq.Right, aliases), heroRawcode, StringComparison.Ordinal) ? eq.Left : null;
        return other is not null && IsLocalFromGetUnitTypeId(other, lines, funcStart, funcEnd);
    }

    /// <summary>Whether <paramref name="name"/> is a local in [<paramref name="funcStart"/>,
    /// <paramref name="funcEnd"/>) assigned (anywhere, a declaration's own initializer or a later
    /// "set") from an expression naming <c>GetUnitTypeId</c>. Every assignment is checked, not just the
    /// first, the same caution <c>PreplacedUnitsScript.ResolveIndexToPlayer</c> uses for a comparable
    /// shape, a placeholder declaration followed by the real assignment is common in this codebase's
    /// source maps.</summary>
    private static bool IsLocalFromGetUnitTypeId(string name, IReadOnlyList<string> lines, int funcStart, int funcEnd)
    {
        if (!Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$")) return false;
        var assign = new Regex(@"^\s*(?:local\s+integer\s+|set\s+)" + Regex.Escape(name) + @"\s*=\s*(.+)$");
        for (int i = funcStart; i < funcEnd && i < lines.Count; i++)
        {
            var m = assign.Match(lines[i]);
            if (m.Success && m.Groups[1].Value.Contains("GetUnitTypeId", StringComparison.Ordinal)) return true;
        }
        return false;
    }

    /// <summary>Every ability sub-branch inside a hero's own guard body: every depth-0 if/elseif
    /// chain whose OWN condition resolves an ability rawcode, merged and de-duplicated. Depth-0
    /// here means relative to the branch body (an "if" nested one level deeper, inside an ability's
    /// own branch, is not split further — its calls are read as part of that ability's body by
    /// <see cref="ExtractCalls"/>, which scans the whole range flatly).</summary>
    private static List<CastBranch> ExtractAbilityBranches(
        IReadOnlyList<string> lines, int bodyStart, int bodyEnd, IReadOnlyDictionary<string, string> aliases)
    {
        var result = new List<CastBranch>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        int depth = 0;
        for (int i = bodyStart; i < bodyEnd; i++)
        {
            var head = lines[i].TrimStart();
            if (StartsWithWord(head, "if"))
            {
                if (depth == 0 && ResolveAbilityRawcode(ConditionOf(lines[i]), aliases) is not null
                    && SplitIfChain(lines, i) is { } chain)
                    foreach (var cb in BuildCastBranches(lines, chain.Branches, aliases))
                        if (seen.Add(cb.AbilityRawcode)) result.Add(cb);
                depth++;
            }
            else if (StartsWithWord(head, "endif"))
            {
                depth--;
            }
        }
        return result;
    }

    private static List<CastBranch> BuildCastBranches(
        IReadOnlyList<string> lines, IReadOnlyList<IfBranch> branches, IReadOnlyDictionary<string, string> aliases)
    {
        var result = new List<CastBranch>();
        foreach (var b in branches)
        {
            if (b.Condition.Length == 0) continue; // a trailing "else" names no ability
            var rawcode = ResolveAbilityRawcode(b.Condition, aliases);
            if (rawcode is null) continue;
            var calls = ExtractCalls(lines, b.BodyStart, b.BodyEnd);
            if (calls.Count == 0) continue; // nothing to synthesize (e.g. bookkeeping only)
            result.Add(new CastBranch(rawcode, calls, CalleesOf(calls)));
        }
        return result;
    }

    /// <summary>Every depth-0 <c>if GetUnitAbilityLevel(unit, X) == 0 then ... endif</c> block inside a
    /// hero's own level up branch, one <see cref="AbilityGrant"/> per distinct ability. A block nested
    /// one level deeper (an ability's own grant testing something else) is not split further, matching
    /// <see cref="ExtractAbilityBranches"/>'s same depth-0 convention.</summary>
    private static List<AbilityGrant> ExtractAbilityGrants(
        IReadOnlyList<string> lines, int bodyStart, int bodyEnd, IReadOnlyDictionary<string, string> aliases)
    {
        var result = new List<AbilityGrant>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        int depth = 0;
        for (int i = bodyStart; i < bodyEnd; i++)
        {
            var head = lines[i].TrimStart();
            if (StartsWithWord(head, "if"))
            {
                if (depth == 0)
                {
                    var condition = ConditionOf(lines[i]);
                    var m = AbilityLevelZeroGuard.Match(condition);
                    if (m.Success && SplitIfChain(lines, i) is { } chain
                        && chain.Branches.FirstOrDefault(br => br.ConditionLine == i) is { } mine)
                    {
                        var rawcode = Resolve(m.Groups["ability"].Value, aliases);
                        var calls = ExtractCalls(lines, mine.BodyStart, mine.BodyEnd);
                        if (rawcode is not null && calls.Count > 0 && seen.Add(rawcode))
                            result.Add(new AbilityGrant(rawcode, m.Groups["unit"].Value, condition, calls));
                    }
                }
                depth++;
            }
            else if (StartsWithWord(head, "endif"))
            {
                depth--;
            }
        }
        return result;
    }

    // ---- statement extraction ---------------------------------------------------------------

    /// <summary>Every "call Foo(...)" statement in [start, end), flat (nested if/loop structure
    /// inside the range is not split further, every call in it still counts). Revives a line this
    /// tool itself commented out on some EARLIER port (its own <see cref="JassScriptCheck.TrimMarker"/>
    /// prefix, which only ever wraps the exact original statement, never deletes it), but leaves a
    /// genuinely author-commented line alone — reviving the map author's own intentionally disabled
    /// code would not be ours to do.</summary>
    private static List<string> ExtractCalls(IReadOnlyList<string> lines, int start, int end)
    {
        var result = new List<string>();
        for (int i = start; i < end; i++)
        {
            var trimmed = lines[i].TrimStart();
            if (trimmed.StartsWith(JassScriptCheck.TrimMarker, StringComparison.Ordinal))
                trimmed = trimmed[JassScriptCheck.TrimMarker.Length..].TrimStart();
            else if (trimmed.StartsWith("//", StringComparison.Ordinal))
                continue; // genuinely disabled by the map author, not ours to revive
            if (!trimmed.StartsWith("call ", StringComparison.Ordinal)) continue;
            if (ExtractOneCall(trimmed) is { } call) result.Add(call);
        }
        return result;
    }

    /// <summary>"call Foo(a, b)  // trailing comment" to "call Foo(a, b)", parens balanced so a
    /// nested call in an argument (Player(0)) does not truncate early. Null when the call's parens
    /// never balance on this line (a call that spans multiple lines is not handled, matching this
    /// codebase's existing single-line assumption for JASS statements, e.g. BundleCommand.CallArgs).</summary>
    private static string? ExtractOneCall(string trimmedLine)
    {
        int open = trimmedLine.IndexOf('(');
        if (open < 0) return null;
        int depth = 0;
        for (int i = open; i < trimmedLine.Length; i++)
        {
            if (trimmedLine[i] == '(') depth++;
            else if (trimmedLine[i] == ')')
            {
                depth--;
                if (depth == 0) return trimmedLine[..(i + 1)];
            }
        }
        return null;
    }

    private static string? CalleeOf(string callStatement) =>
        CalleeName.Match(callStatement) is { Success: true } m ? m.Groups[1].Value : null;

    private static List<string> CalleesOf(IReadOnlyList<string> calls) =>
        calls.Select(CalleeOf).Where(c => c is not null).Select(c => c!).Distinct(StringComparer.Ordinal).ToList();

    private static string? ResolveAbilityRawcode(string condition, IReadOnlyDictionary<string, string> aliases)
    {
        if (SplitTopLevelEquality(condition) is not { } eq) return null;
        return Resolve(eq.Left, aliases) ?? Resolve(eq.Right, aliases);
    }

    private static string? Resolve(string token, IReadOnlyDictionary<string, string> aliases) =>
        token.Length == 6 && token[0] == '\'' ? token[1..^1] : aliases.GetValueOrDefault(token);

    /// <summary>Splits <paramref name="condition"/> at the first "==" that sits OUTSIDE every pair
    /// of parentheses (never inside one, however deeply nested), so a call-expression operand like
    /// <c>GetUnitTypeId(GetSpellAbilityUnit())</c> is returned whole rather than being cut apart at
    /// its own inner parens. Trimmed on both sides. Null when the condition has no top-level "=="
    /// at all, a compound guard joining more than one comparison with "and"/"or" is a different
    /// shape this deliberately does not resolve, same scope limit the codebase's callers already
    /// document (see <see cref="ConditionNamesHero"/>).</summary>
    private static (string Left, string Right)? SplitTopLevelEquality(string condition)
    {
        int depth = 0;
        for (int i = 0; i < condition.Length - 1; i++)
        {
            char c = condition[i];
            if (c == '(') depth++;
            else if (c == ')') depth--;
            else if (depth == 0 && c == '=' && condition[i + 1] == '=')
                return (condition[..i].Trim(), condition[(i + 2)..].Trim());
        }
        return null;
    }

    // ---- generic if/elseif/else/endif splitting ----------------------------------------------

    /// <summary>One sibling branch of an if/elseif/.../else/endif chain: the condition text of its
    /// own "if"/"elseif" line (empty for a trailing "else"), that line's index, and the [BodyStart,
    /// BodyEnd) range of lines up to the next sibling (or the chain's own "endif"), at the SAME
    /// nesting depth. A nested if/endif pair inside a branch is left whole, part of that branch's
    /// range, never split further.</summary>
    private sealed record IfBranch(string Condition, int ConditionLine, int BodyStart, int BodyEnd);

    /// <summary>Splits the if/elseif/.../else/endif chain opening at <paramref name="start"/>
    /// (lines[start]'s trimmed text must begin with the word "if") into its sibling branches, or
    /// null when the chain never closes within <paramref name="lines"/> (malformed/truncated input,
    /// never assumed).</summary>
    private static (List<IfBranch> Branches, int EndLine)? SplitIfChain(IReadOnlyList<string> lines, int start)
    {
        if (!StartsWithWord(lines[start].TrimStart(), "if")) return null;

        var branches = new List<IfBranch>();
        int depth = 0;
        int branchStart = start;
        string branchCondition = ConditionOf(lines[start]);

        for (int i = start + 1; i < lines.Count; i++)
        {
            var head = lines[i].TrimStart();
            if (StartsWithWord(head, "if")) { depth++; continue; }
            if (depth > 0)
            {
                if (StartsWithWord(head, "endif")) depth--;
                continue;
            }
            if (StartsWithWord(head, "elseif"))
            {
                branches.Add(new IfBranch(branchCondition, branchStart, branchStart + 1, i));
                branchStart = i;
                branchCondition = ConditionOf(lines[i]);
            }
            else if (StartsWithWord(head, "else"))
            {
                branches.Add(new IfBranch(branchCondition, branchStart, branchStart + 1, i));
                branchStart = i;
                branchCondition = "";
            }
            else if (StartsWithWord(head, "endif"))
            {
                branches.Add(new IfBranch(branchCondition, branchStart, branchStart + 1, i));
                return (branches, i + 1);
            }
        }
        return null; // never closed
    }

    /// <summary>The text of an "if"/"elseif" line with the leading keyword and trailing "then"
    /// stripped, and any line comment removed. "if id == AstaQ_ID then // note" -> "id == AstaQ_ID".</summary>
    private static string ConditionOf(string ifOrElseifLine)
    {
        var t = StripLineComment(ifOrElseifLine).Trim();
        int sp = t.IndexOf(' ');
        t = sp < 0 ? "" : t[(sp + 1)..].Trim();
        if (t.EndsWith("then", StringComparison.Ordinal)) t = t[..^4].TrimEnd();
        return t;
    }

    private static string StripLineComment(string line)
    {
        int i = line.IndexOf("//", StringComparison.Ordinal);
        return i >= 0 ? line[..i] : line;
    }

    private static bool StartsWithWord(string s, string word)
    {
        if (!s.StartsWith(word, StringComparison.Ordinal)) return false;
        if (s.Length == word.Length) return true;
        char c = s[word.Length];
        return !(char.IsLetterOrDigit(c) || c == '_');
    }
}
