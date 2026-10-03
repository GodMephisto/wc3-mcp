// src/Wc3.Commands/HeroSystemAudit.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One hand-written per-hero table in the target's script, and how many of the target's
/// own roster heroes it names. <paramref name="Line"/> is 1-based, so a human can open it.</summary>
/// <summary>
/// One place a map hand-lists its heroes. <paramref name="HasFallback"/> is the difference
/// between "this hero is missing from a table she belongs in" and "this table has a default and
/// absence is the right answer for her", which are opposite instructions to give a person.
/// </summary>
public sealed record HeroSystem(string Function, int Line, int HeroCount,
    bool HasFallback = false, int RosterSize = 0)
{
    /// <summary>
    /// Share of the roster this table actually lists. A table covering half the roster leaves
    /// the other half absent, so absence is the state most heroes are already in and is poor
    /// evidence that an entry is required. A table covering nearly everyone is the opposite.
    /// </summary>
    public int CoveragePercent =>
        RosterSize <= 0 ? 0 : (int)Math.Round(100.0 * HeroCount / RosterSize);
}

/// <summary>
/// Finds every place a target map hand-lists its heroes, so an installed hero can be told which of
/// them she is missing from.
///
/// This exists because the kit ladder is not the only one, and reporting only it understated the
/// work sevenfold. Instrumenting GGGA in the running game settled where a ported hero does and does
/// not break: she passes every pick-screen gate, the commit is accepted, the seat finalizes,
/// CreateUnit produces her unit, and she arrives alive, visible, owned and selected, indistinguish-
/// able from a native hero at every measured point but one. Nothing in the SPAWN path is wrong.
/// What is wrong is that the map wires heroes into several separate hand-written tables (the kit,
/// damage-event routing, two death handlers, generated map state, the carried port's own roster
/// mirror) and an installed hero is in none of them.
///
/// The detection rule is roster MEMBERSHIP, not the <c>== 'XXXX'</c> comparison shape that
/// <see cref="ContractCommand"/> looks for, because half of these tables are not comparison
/// ladders: they are <c>SaveTriggerHandle(hash, 0, 'H006', trg)</c> rows, <c>set Arr[11]='H006'</c>
/// slots and <c>Register(... 'H006' ...)</c> calls. Counting how many of the target's OWN roster
/// rawcodes a function names catches every shape at once.
///
/// Membership alone is far too loose. On GGGA, 3371 functions name five or more roster heroes,
/// because a single hero's damage trigger legitimately tests seventeen others. The discriminator is
/// SCALE relative to the roster: a table that hand-lists a fifth of every hero in the map is a
/// per-hero system, and a function naming a handful is gameplay logic about those heroes. That one
/// threshold takes GGGA's 3371 candidates down to seven, all of them real, with no arbitrary
/// control hero to choose and nothing map-specific hardcoded.
/// </summary>
public static class HeroSystemAudit
{
    /// <summary>
    /// The share of the roster a function must name before it counts as a per-hero table. A fifth
    /// is measured, not guessed: on GGGA the real tables sit at 53%, 52%, 46%, 35%, 23%, 22% and
    /// 21% of the roster and the densest piece of ordinary gameplay logic sits at 15%, so the cut
    /// falls in a real gap rather than through the middle of anything.
    /// </summary>
    private const int RosterShareDivisor = 5;

    /// <summary>A floor for small rosters, where a fifth is one or two heroes and proves nothing.</summary>
    private const int MinRosterHeroes = 5;

    /// <summary>A list longer than this is not read. The real ones sort to the top by size.</summary>
    private const int MaxReported = 12;

    private static readonly Regex Rawcode = new(@"'([^'\r\n]{4})'", RegexOptions.Compiled);

    public static IReadOnlyList<HeroSystem> Run(string script, IReadOnlyCollection<string> roster,
        string installed)
    {
        var members = new HashSet<string>(roster, StringComparer.Ordinal);
        members.Remove(installed);
        // A "roster" smaller than one table's worth of heroes is not a roster.
        if (members.Count <= MinRosterHeroes) return Array.Empty<HeroSystem>();
        int floor = Math.Max(MinRosterHeroes, (members.Count + RosterShareDivisor - 1) / RosterShareDivisor);

        // Comments are blanked first. A porter writes provenance comments naming the rawcodes it
        // carried, and a generated map keeps retired lines commented out rather than deleting them,
        // so counting those charged a hero to a table whose live code never mentions her. On GGGA
        // that reported WS_Init_WorkingSourceGeneratedMapState on nothing but strings like
        // "// WS_REBORN_REMOVED_LEGACY_PICK_DUMMY: set u=CreateUnit(p, 'H006', ...)".
        var lines = JassComments.Strip(script.Split('\n'));
        var found = new List<HeroSystem>();
        foreach (var fn in JassFunctionIndex.Parse(script))
        {
            var heroes = new HashSet<string>(StringComparer.Ordinal);
            bool hasInstalled = false;
            for (int i = Math.Max(0, fn.StartLine - 1); i < Math.Min(fn.EndLine, lines.Length); i++)
                foreach (Match m in Rawcode.Matches(lines[i]))
                {
                    var code = m.Groups[1].Value;
                    if (members.Contains(code)) heroes.Add(code);
                    else if (string.Equals(code, installed, StringComparison.Ordinal)) hasInstalled = true;
                }
            // Present already, or too small to be a table every hero belongs in.
            if (hasInstalled || heroes.Count < floor) continue;
            found.Add(new HeroSystem(fn.Name, fn.StartLine, heroes.Count,
                HasFallback(lines, fn.StartLine - 1, fn.EndLine), members.Count));
        }

        return found
            .OrderByDescending(s => s.HeroCount).ThenBy(s => s.Function, StringComparer.Ordinal)
            .Take(MaxReported)
            .ToList();
    }

    /// <summary>
    /// Whether the ladder ends in a default, an <c>else</c> arm or a <c>return</c> reached after
    /// the last <c>endif</c>, so a hero with no branch still gets a defined answer.
    /// </summary>
    /// <remarks>
    /// This is the difference between a real gap and a false alarm. GGGA's
    /// WS_GetWorkingSourceSkinOriginCode maps a skinned hero back to the hero it is a skin OF, and
    /// ends with <c>return 0</c>. A newly installed hero is not a skin of anything, so having no
    /// branch there is correct, and telling someone to add one sends them to break something that
    /// already works. Reported as a fact rather than acted on, because whether the default suits a
    /// particular hero is a judgement about that map's semantics.
    /// </remarks>
    private static bool HasFallback(string[] lines, int start, int end)
    {
        int depth = 0;
        bool sawIf = false;
        for (int i = Math.Max(0, start); i < Math.Min(end, lines.Length); i++)
        {
            var t = lines[i].Trim();
            if (IfOpen.IsMatch(t)) { depth++; sawIf = true; continue; }
            if (t.StartsWith("endif", StringComparison.Ordinal)) { depth = Math.Max(0, depth - 1); continue; }
            // An else arm inside the ladder is itself the default.
            if (depth > 0 && Else.IsMatch(t)) return true;
            // A statement outside every branch, after at least one branch, is the fallback.
            if (depth == 0 && sawIf
                && (t.StartsWith("return", StringComparison.Ordinal)
                    || t.StartsWith("set ", StringComparison.Ordinal)
                    || t.StartsWith("call ", StringComparison.Ordinal)))
                return true;
        }
        return false;
    }

    private static readonly Regex IfOpen = new(@"^if\b.*\bthen$", RegexOptions.Compiled);
    private static readonly Regex Else = new(@"^else$", RegexOptions.Compiled);
}
