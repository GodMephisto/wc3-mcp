// src/Wc3.Commands/ContractCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One real registration the target already performs: the rawcode it registers and the
/// whole source line, kept so a caller can read the OTHER arguments of that call.</summary>
public sealed record RosterCall(string Rawcode, string Line);

/// <summary>A registration call a target map expects for each unit it hosts.</summary>
public sealed record RosterRegistry(
    string Function,
    int CallCount,
    string Signature,
    string ExampleCall,
    IReadOnlyList<string> RegisteredRawcodes,
    IReadOnlyList<RosterCall> Calls);

/// <summary>A per-player array a hero must appear in for the map's systems to see it.</summary>
public sealed record HeroArray(string Name, string Type, int AssignmentSites);

/// <summary>
/// A function that branches on a hero's rawcode, one hand-written branch per hero, to give that
/// hero its kit. A hero with no branch here reaches the end of the ladder and gets nothing.
/// </summary>
public sealed record HeroDispatchChain(string Function, int Branches, IReadOnlyList<string> Rawcodes);

/// <summary>
/// A roster expressed as a repeating STATEMENT BLOCK rather than a single call. Some maps register
/// a hero with one function call; others need several statements (a slot assignment, a counter, a
/// hidden preview dummy, one call per ability). <see cref="TemplateLines"/> is a real existing
/// entry, so an installer can copy it and substitute rather than invent the shape.
/// </summary>
public sealed record RosterTemplate(
    string ArrayName,
    int Entries,
    int StartLine,
    int EndLine,
    IReadOnlyList<string> TemplateLines);

/// <summary>One function that mentions a hero's rawcode, and how many times.</summary>
public sealed record IntegrationPoint(string Function, int References, bool HeroSpecificName);

/// <summary>
/// What a WORKING hero of this map is wired into that a candidate hero is not.
/// </summary>
public sealed record IntegrationGap(
    string ReferenceHero,
    string CandidateHero,
    int ReferenceFunctions,
    int CandidateFunctions,
    IReadOnlyList<IntegrationPoint> Missing);

public sealed record ContractResult(
    string ScriptFile,
    IReadOnlyList<RosterRegistry> Registries,
    IReadOnlyList<string> SpellDispatchers,
    IReadOnlyList<HeroArray> HeroArrays,
    IReadOnlyList<RosterTemplate> Templates,
    /// <summary>
    /// Per-hero kit ladders. Measured in game on GGGA: an installed hero passed every gate the
    /// pick screen has (registered, right role, RPB_IsHeroCommitValid true) and CreateUnit
    /// produced her unit, and she still had no kit, because the map wires each hero's spells in a
    /// hand-written branch of WS_FinalizeWorkingSourceHero and she has none. Reported rather than
    /// generated, for the same reason a stub is never bound on a name match alone: a branch copied
    /// from another hero would bind THAT hero's triggers and caster global to this unit, which is
    /// worse than doing nothing and is invisible until someone plays her.
    /// </summary>
    IReadOnlyList<HeroDispatchChain> HeroDispatchChains,
    /// <summary>
    /// What the map's own heroes agree a hero's stats look like, when they agree on anything.
    /// Part of the contract for the same reason the roster call is: a hero can be registered,
    /// selectable and completely correct and still be unplayable because she brought her home
    /// map's stat model with her. Measured over the whole roster here; install re-measures over
    /// the hero's own role, which is narrower and more accurate.
    /// </summary>
    HeroStatConvention? StatConvention)
{
    public bool Any => Registries.Count > 0 || SpellDispatchers.Count > 0
        || HeroArrays.Count > 0 || Templates.Count > 0;
}

/// <summary>
/// Discovers a map's HERO INTEGRATION CONTRACT: what a unit must be registered with before the
/// map's own systems will treat it as a playable hero.
///
/// This is the piece nothing else in the ecosystem models, and it is where ported heroes actually
/// die. A hero can have correct object data, correct assets and a compiling script and still not
/// exist to the player, because the map builds its roster from its own script. GGGA's is
/// <c>RPB_AddHero(rawcode, role, portrait)</c> called 165 times inside
/// <c>RPB_SetHeroCardList</c>; a hero absent from that list has no card no matter how complete
/// everything else is. Finding it took reading a 200,000-line script by hand.
///
/// The generalisable signature is a function called many times whose first argument is a rawcode
/// literal. That is what a roster registration looks like in any map, whatever it is named.
/// </summary>
public static class ContractCommand
{
    /// <summary>Below this, repeated rawcode calls are a coincidence rather than a roster.</summary>
    private const int MinRegistrationCalls = 8;

    private static readonly Regex RawcodeCall =
        new(@"\bcall\s+([A-Za-z_][A-Za-z0-9_]*)\s*\(\s*'([^']{4})'", RegexOptions.Compiled);
    /// <summary>
    /// Leading whitespace and "constant" are both allowed, because a real map indents some of its
    /// declarations and this pattern also ENDS a function for the dispatch scan. Anchored at column
    /// zero it never closed an indented function, so every rawcode in the functions that followed
    /// was charged to the last one that happened to start flush left. On GGGA that invented four
    /// hero-kit ladders of 145, 109, 104 and 67 branches out of functions whose real bodies contain
    /// no rawcode at all, and the tool told a human to hand-write branches into every one of them.
    /// </summary>
    private static readonly Regex Declaration =
        new(@"^\s*(?:constant\s+)?function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes\s+(.*?)\s+returns",
            RegexOptions.Compiled);
    private static readonly Regex GlobalArray =
        new(@"^\s*(unit|integer|player)\s+array\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    public static ContractResult Run(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (entry is null)
            return new ContractResult("(none)", Array.Empty<RosterRegistry>(),
                Array.Empty<string>(), Array.Empty<HeroArray>(), Array.Empty<RosterTemplate>(),
                Array.Empty<HeroDispatchChain>(), null);

        // Latin-1, not UTF-8. Unlike the other read-only scans in this codebase, what this one
        // decodes DOES get written back: install copies ExampleCall and a RosterTemplate's lines
        // verbatim into the target's war3map.j. A war3map.j is a byte stream with no declared
        // encoding and real maps carry bytes that are not valid UTF-8, so a UTF-8 decode turns each
        // into U+FFFD and the Latin-1 write-back then stores '?'. Latin-1 round-trips every byte.
        var text = System.Text.Encoding.Latin1.GetString(entry.CurrentBytes);
        var lines = text.Split('\n');

        var signatures = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var l in lines)
            if (Declaration.Match(l) is { Success: true } m)
                signatures[m.Groups[1].Value] = m.Groups[2].Value.Trim();

        // A roster registry is a function called repeatedly with a rawcode as its first argument.
        var byFunction = new Dictionary<string, List<(string code, string call)>>(StringComparer.Ordinal);
        foreach (var l in lines)
        {
            if (RawcodeCall.Match(l) is not { Success: true } m) continue;
            var fn = m.Groups[1].Value;
            if (!byFunction.TryGetValue(fn, out var list))
                byFunction[fn] = list = new List<(string, string)>();
            list.Add((m.Groups[2].Value, l.Trim()));
        }

        var registries = byFunction
            .Where(kv => kv.Value.Count >= MinRegistrationCalls)
            .Select(kv => new RosterRegistry(kv.Key, kv.Value.Count,
                signatures.TryGetValue(kv.Key, out var s) ? s : "(unknown)",
                Shorten(kv.Value[0].call),
                kv.Value.Select(v => v.code).Distinct().OrderBy(c => c, StringComparer.Ordinal).ToList(),
                kv.Value.Select(v => new RosterCall(v.code, v.call)).ToList()))
            .OrderByDescending(r => r.CallCount)
            .ToList();

        // A spell dispatcher decides which handler runs for a cast. A ported hero whose ability id
        // never reaches one of these can be selected but will do nothing.
        var dispatchers = new List<string>();
        string current = "(top level)";
        foreach (var l in lines)
        {
            if (Declaration.Match(l) is { Success: true } dm) current = dm.Groups[1].Value;
            if (l.Contains("GetSpellAbilityId", StringComparison.Ordinal) && !dispatchers.Contains(current))
                dispatchers.Add(current);
        }

        // Per-player hero arrays. A hero missing from these is invisible to the map's own systems.
        var arrays = new List<HeroArray>();
        foreach (var l in lines)
        {
            if (GlobalArray.Match(l) is not { Success: true } gm) continue;
            var name = gm.Groups[2].Value;
            if (!LooksLikeHeroArray(name)) continue;
            int assignments = CountAssignments(text, name);
            if (assignments > 0) arrays.Add(new HeroArray(name, gm.Groups[1].Value, assignments));
        }

        // Measured against the biggest registry, the one an installer would register into.
        var stats = registries.Count > 0
            ? HeroStatConvention.Derive(doc, registries[0], role: null) : null;

        return new ContractResult(entry.FileName ?? "war3map.j", registries,
            dispatchers.Take(20).ToList(),
            arrays.OrderByDescending(a => a.AssignmentSites).Take(15).ToList(),
            FindRosterTemplates(lines), FindHeroDispatchChains(lines), stats);
    }


    /// <summary>Assignment of an array slot from an id-shaped global, e.g. set Hero_ID0[n]=Raiden_ID.</summary>
    private static readonly Regex RosterSlotAssign =
        new(@"^\s*set\s+([A-Za-z_][A-Za-z0-9_]*)\s*\[[^\]]*\]\s*=\s*([A-Za-z_][A-Za-z0-9_]*_ID)\b",
            RegexOptions.Compiled);

    /// <summary>
    /// Finds rosters built as a REPEATING BLOCK instead of a single call.
    /// </summary>
    /// <remarks>
    /// Searching for a registration FUNCTION found nothing on a map that plainly has hero
    /// selection, because its roster is an array of ability-id globals, not rawcode literals:
    /// <c>set Hero_ID0[n]=Raiden_ID</c>, followed by a counter bump, a hidden preview dummy and one
    /// UnitAddAbility per spell. Five statements per hero, repeated 56 times, with the display name
    /// set in a separate if/elseif chain.
    ///
    /// So a contract cannot be reduced to a signature. The map demonstrates its own shape dozens of
    /// times, and the reliable move is to capture one real entry as a template that an installer
    /// copies and substitutes into. A single-call roster is simply a one-line template, so this
    /// generalises both forms rather than special-casing either.
    /// </remarks>
    private static IReadOnlyList<RosterTemplate> FindRosterTemplates(string[] lines)
    {
        var sites = new Dictionary<string, List<int>>(StringComparer.Ordinal);
        for (int i = 0; i < lines.Length; i++)
        {
            var m = RosterSlotAssign.Match(lines[i]);
            if (!m.Success) continue;
            var name = m.Groups[1].Value;
            if (!sites.TryGetValue(name, out var l)) sites[name] = l = new List<int>();
            l.Add(i);
        }

        var result = new List<RosterTemplate>();
        foreach (var (name, at) in sites.OrderByDescending(kv => kv.Value.Count))
        {
            if (at.Count < 3) continue;   // a handful is a coincidence, not a roster
            // The pattern alone is not enough: an item table matches it too
            // (ItemsCurrentItem_ID). The same discipline as identifying a registration function -
            // the NAME has to indicate a unit, or we report something the caller cannot use.
            if (!LooksLikeHeroArray(name)) continue;

            // The block for one entry runs from its slot assignment to just before the next one,
            // which is how the map itself delimits them.
            int start = at[0];
            int end = at.Count > 1 ? at[1] - 1 : Math.Min(at[0] + 8, lines.Length - 1);
            // Include the immediately preceding counter/bump lines, which belong to the entry.
            while (start > 0 && lines[start - 1].TrimStart().StartsWith("set ", StringComparison.Ordinal)
                   && !RosterSlotAssign.IsMatch(lines[start - 1]))
                start--;

            var block = new List<string>();
            for (int i = start; i <= end && block.Count < 24; i++)
            {
                var t = lines[i].TrimEnd('\r');
                if (t.Trim().Length > 0) block.Add(t);
            }
            result.Add(new RosterTemplate(name, at.Count, start + 1, end + 1, block));
            if (result.Count == 6) break;
        }
        return result;
    }

    /// <summary>A rawcode equality test, the shape one branch of a per-hero ladder is written in.</summary>
    private static readonly Regex RawcodeCompare =
        new(@"==\s*'([^']{4})'", RegexOptions.Compiled);

    /// <summary>Below this, a few rawcode comparisons in one function are ordinary logic.</summary>
    private const int MinDispatchBranches = 8;

    /// <summary>
    /// What separates a KIT ladder from a lookup table. Branching on a hero's rawcode many times
    /// is not enough on its own: GGGA has several big ladders that only return a number (how much
    /// SP an upgrade costs, which respawn portal a hero uses), and naming those as things a ported
    /// hero must be added to is noise that buries the one that matters. A kit branch WIRES the
    /// unit, so the function has to do at least one of these somewhere in its body.
    /// </summary>
    private static readonly string[] KitWiringCalls =
        { "TriggerRegisterUnitEvent", "UnitAddAbility", "SetPlayerAbilityAvailable" };

    /// <summary>
    /// Finds the functions that hand a hero its kit by branching on her rawcode. The shape is one
    /// function containing many <c>== 'XXXX'</c> tests, which is how a map writes "and this is what
    /// THIS hero gets" when the work differs per hero and cannot be table-driven.
    /// </summary>
    /// <remarks>
    /// This is the last thing a ported hero is missing after the roster, and it is invisible to
    /// every other check: the hero is registered, selectable, and her unit is created, so nothing
    /// reports a problem, and in game she simply has no spells. Detected so install can say which
    /// function needs a branch, by name, instead of leaving it to be found by playing her.
    /// </remarks>
    private static IReadOnlyList<HeroDispatchChain> FindHeroDispatchChains(string[] lines)
    {
        var found = new List<HeroDispatchChain>();
        string current = "(top level)";
        var codes = new List<string>();
        bool wires = false;

        void Flush()
        {
            var distinct = codes.Distinct(StringComparer.Ordinal).ToList();
            if (wires && distinct.Count >= MinDispatchBranches)
                found.Add(new HeroDispatchChain(current, distinct.Count, distinct));
            codes.Clear();
            wires = false;
        }

        foreach (var l in lines)
        {
            if (Declaration.Match(l) is { Success: true } dm)
            {
                Flush();
                current = dm.Groups[1].Value;
            }
            foreach (Match m in RawcodeCompare.Matches(l)) codes.Add(m.Groups[1].Value);
            if (!wires)
                wires = KitWiringCalls.Any(c => l.Contains(c, StringComparison.Ordinal));
        }
        Flush();

        // The biggest five, not all of them. GGGA has thirteen ladders over the branch floor and a
        // list that long is not read; the one that actually blocked a hero (93 branches) sits
        // fourth, so the cut has to be low enough to keep it and high enough to stay readable.
        return found.OrderByDescending(c => c.Branches).Take(5).ToList();
    }

    private static bool LooksLikeHeroArray(string name) =>
        name.Contains("Hero", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Player", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Champion", StringComparison.OrdinalIgnoreCase);

    private static int CountAssignments(string text, string name) =>
        Regex.Matches(text, @"\bset\s+" + Regex.Escape(name) + @"\s*\[").Count;

    private static string Shorten(string s) => s.Length <= 120 ? s : s[..120] + "...";

    /// <summary>
    /// Every function whose body mentions this rawcode, with a count.
    /// </summary>
    public static IReadOnlyList<IntegrationPoint> IntegrationPoints(MapDocument doc, string rawcode)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (entry is null) return Array.Empty<IntegrationPoint>();

        // Latin-1: a script is a byte stream with no declared encoding and real maps are not valid
        // UTF-8, so decoding as UTF-8 mangles content this scan then fails to match.
        var lines = System.Text.Encoding.Latin1
            .GetString(entry.CurrentBytes).Split('\n');

        var starts = new List<int>();
        var nameAt = new Dictionary<int, string>();
        for (int i = 0; i < lines.Length; i++)
            if (Declaration.Match(lines[i]) is { Success: true } m)
            { starts.Add(i); nameAt[i] = m.Groups[1].Value; }

        var token = "'" + rawcode + "'";
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 0; i < lines.Length; i++)
        {
            if (!lines[i].Contains(token, StringComparison.Ordinal)) continue;
            int at = starts.BinarySearch(i);
            if (at < 0) at = ~at - 1;
            var fn = at >= 0 ? nameAt[starts[at]] : "(top level)";
            counts[fn] = counts.GetValueOrDefault(fn) + 1;
        }

        return counts.OrderByDescending(kv => kv.Value)
            .Select(kv => new IntegrationPoint(kv.Key, kv.Value, false))
            .ToList();
    }

    /// <summary>
    /// Diffs a candidate hero's wiring against a hero the map already supports, which is the only
    /// reliable way to enumerate what a map requires.
    /// </summary>
    /// <remarks>
    /// Detecting registries by shape found five rawcode-branching functions on GGGA and missed the
    /// rest, so a hero was fixed one requirement at a time out of about a dozen and the symptom
    /// never moved. Diffing against a hero that demonstrably works finds ALL of them at once,
    /// because the map itself defines the contract by example: a native Stalker is referenced in 26
    /// functions, a freshly installed hero in 9.
    ///
    /// A function whose NAME contains the reference hero's own name is flagged, since it is that
    /// character's private code rather than shared infrastructure, and no tool should copy it.
    /// </remarks>
    public static IntegrationGap CompareIntegration(
        MapDocument doc, string referenceRawcode, string candidateRawcode, string? referenceName = null)
    {
        var reference = IntegrationPoints(doc, referenceRawcode);
        var candidate = IntegrationPoints(doc, candidateRawcode)
            .Select(p => p.Function).ToHashSet(StringComparer.Ordinal);

        var tokens = (referenceName ?? "")
            .Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length >= 4)
            .ToList();

        var missing = reference
            .Where(p => !candidate.Contains(p.Function))
            .Select(p => p with
            {
                HeroSpecificName = tokens.Any(t =>
                    p.Function.Contains(t, StringComparison.OrdinalIgnoreCase)),
            })
            .ToList();

        return new IntegrationGap(referenceRawcode, candidateRawcode,
            reference.Count, candidate.Count, missing);
    }

}
