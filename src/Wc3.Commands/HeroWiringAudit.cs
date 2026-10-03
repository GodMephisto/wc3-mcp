// src/Wc3.Commands/HeroWiringAudit.cs
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Which link of the wiring chain is missing for one ability. <see cref="Ok"/> means every
/// prerequisite this tool can check statically is satisfied.</summary>
public enum WiringStatus
{
    Ok,
    /// <summary>The ability object itself is not in the map, so the hero can never hold it.</summary>
    AbilityObjectMissing,
    /// <summary>Nothing in the script references this ability, yet its own object fields still look
    /// like a castable active (a real target or a cast cost), so it is a spell the map defines but
    /// never wires up. An unreferenced passive or aura is reported as <see cref="NotCastDispatched"/>
    /// instead, because those work from object data with no script presence at all.</summary>
    NoDispatch,
    /// <summary>The map uses this ability but never dispatches a cast on it. That is what a passive,
    /// an aura, an inventory or a spellbook looks like, so it is reported for information rather than
    /// counted as broken. Verified against real maps, abilities in this state have no cast dispatch in
    /// the SOURCE map either, so treating them as failures blamed the port for the map's own design.</summary>
    NotCastDispatched,
    /// <summary>The handler exists but is not attached to any trigger.</summary>
    NotAttachedToTrigger,
    /// <summary>The trigger has no spell event at all, so it never fires.</summary>
    NoEventRegistered,
    /// <summary>The trigger's event is per-player and this hero's player is not among them.</summary>
    PlayerNotRegistered,
    /// <summary>The handler body is empty, or every statement in it was commented out by porting.</summary>
    EmptyOrTrimmedHandler,
    /// <summary>The handler only acts on the unit held in a per-player array this hero is absent from.</summary>
    IdentityArrayNotRegistered,
    /// <summary>Nothing calls the init that creates the trigger, so it is never built.</summary>
    InitNeverCalled,
    /// <summary>Two calls to the init build two triggers, so the ability fires twice.</summary>
    InitCalledTwice,
    /// <summary>Two (or more) DIFFERENT triggers each independently complete the whole chain for this
    /// ability, so it fires once per trigger per cast. This is the double-registration shape
    /// (a synthesized --synth-dispatch dispatcher AND the source's own shared dispatcher both live
    /// for the same hero) that made every affected spell deal doubled damage and left a hero
    /// permanently paused, the earlier per-candidate check only caught ONE trigger built twice, not
    /// two DIFFERENT triggers each built once.</summary>
    MultipleLiveDispatchers,
}

public sealed record AbilityWiring(string Ability, string? Name, WiringStatus Status, string Detail);

public sealed record HeroWiringResult(
    string Hero, string? Name, int OwnerId, IReadOnlyList<AbilityWiring> Abilities)
{
    /// <summary>Abilities that are cast-wired end to end.</summary>
    public int Wired => Abilities.Count(a => a.Status == WiringStatus.Ok);
    /// <summary>Passives and the like, neither wired nor broken.</summary>
    public int NotCastable => Abilities.Count(a => a.Status == WiringStatus.NotCastDispatched);
    /// <summary>Only the genuinely actionable faults, so a passive never reads as a failure.</summary>
    public IReadOnlyList<AbilityWiring> Problems =>
        Abilities.Where(a => a.Status is not (WiringStatus.Ok or WiringStatus.NotCastDispatched)).ToList();
}

/// <summary>
/// Verifies, for every ability of a placed hero, that the map actually wires it up.
///
/// This exists because a ported hero can fail to cast for many unrelated reasons (a missing ability
/// object, a dispatch that was never carried, a trigger with no event, an event registered for the
/// wrong player, a handler whose body was trimmed away, a per-player identity array the hero is
/// absent from, an init nobody calls, or one two callers build twice). Each of those was previously
/// found by playing the map and noticing a dead spell. All of them are decidable from the script, so
/// they are decided here instead, and the specific missing link is named.
/// </summary>
public static class HeroWiringAudit
{
    /// <summary>The gg_trg_* a function is attached to, as a condition or an action, on a
    /// comment-stripped view of the whole script. Shared with <see cref="DebugWiringCommand"/>,
    /// which walks this the other way round (trigger to dispatch function) to find what to
    /// instrument on an already-ported map.</summary>
    public static string? FindAttachedTrigger(string codeText, string function)
    {
        var m = Regex.Match(codeText,
            @"TriggerAdd(?:Condition|Action)\s*\(\s*(gg_trg_[A-Za-z0-9_]+)\s*,[^)]*\b"
            + Regex.Escape(function) + @"\b");
        return m.Success ? m.Groups[1].Value : null;
    }

    /// <summary>Audits every distinct hero type placed on the map.</summary>
    public static IReadOnlyList<HeroWiringResult> AuditPlacedHeroes(MapDocument doc)
    {
        var units = doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        if (units is null) return Array.Empty<HeroWiringResult>();

        // A hero's rawcode starts uppercase by World Editor convention (Hpal/H000 vs hfoo/h000).
        var placed = units.Units
            .Where(u => u.OwnerId >= 0 && u.OwnerId < PlayerColors.NeutralHostileId)
            .Select(u => (Rawcode: u.TypeId.ToRawcode(), u.OwnerId))
            .Where(x => x.Rawcode.Length == 4 && char.IsUpper(x.Rawcode[0]))
            .GroupBy(x => x.Rawcode, StringComparer.Ordinal)
            .Select(g => (Rawcode: g.Key, OwnerId: g.First().OwnerId))
            .ToList();

        return placed.Select(p => Audit(doc, p.Rawcode, p.OwnerId)).ToList();
    }

    /// <summary>Audits one hero type as owned by <paramref name="ownerId"/>.</summary>
    public static HeroWiringResult Audit(MapDocument doc, string heroRawcode, int ownerId)
    {
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile);
        byte[]? bytes = entry?.CurrentBytes;
        string jass = bytes is { Length: > 0 } ? Encoding.Latin1.GetString(bytes) : "";

        var ctx = new ScriptContext(jass, ownerId, doc);
        var abilityNames = AbilityNames(doc);
        var results = new List<AbilityWiring>();
        var heroAbilities = HeroAbilities(doc, heroRawcode, ctx).ToHashSet(StringComparer.Ordinal);

        foreach (var ability in heroAbilities)
        {
            abilityNames.TryGetValue(ability, out var name);
            results.Add(CheckAbility(ability, name, ctx, heroAbilities));
        }

        string? heroName = UnitNames(doc).TryGetValue(heroRawcode, out var hn) ? hn : null;
        return new(heroRawcode, heroName, ownerId,
            results.OrderBy(r => r.Status == WiringStatus.Ok ? 1 : 0)
                   .ThenBy(r => r.Ability, StringComparer.Ordinal).ToList());
    }

    // ---- the chain -----------------------------------------------------------

    private static AbilityWiring CheckAbility(
        string ability, string? name, ScriptContext ctx, IReadOnlySet<string> heroAbilities)
    {
        AbilityWiring Fail(WiringStatus s, string detail) => new(ability, name, s, detail);

        // 1. The object must exist, else the hero cannot even hold the ability. This is the class
        //    that hid a dash-back ability the script adds at runtime.
        if (!ctx.CustomAbilities.Contains(ability) && !ctx.BaseLikeAbility(ability))
            return Fail(WiringStatus.AbilityObjectMissing,
                "the ability object is not in this map, port it from the source");

        // 2. Something must dispatch on it, by literal or through an id global.
        var dispatches = ctx.FindDispatches(ability);
        if (dispatches.Count == 0)
        {
            // A referenced-but-never-cast ability is a passive, aura, inventory or spellbook, exactly
            // how the source map designed it, so it is reported for information rather than as a fault.
            if (ctx.IsReferenced(ability))
                return Fail(WiringStatus.NotCastDispatched,
                    "used by the map but never cast-dispatched, so it is a passive, aura or spellbook");

            // No script presence at all. That alone is not a fault, because an aura or a base-game
            // passive is pure object data and works with zero script, and flagging those as inert was
            // the false positive being fixed. Only call it inert when the ability's own fields make it
            // look like a castable active (a real target or a cast cost, which a passive never carries).
            // With no game data here we see only the map's deltas, so the absence of those traits reads
            // as passive, the conservative side of the passive-versus-inert call.
            return ctx.LooksCastable(ability)
                ? Fail(WiringStatus.NoDispatch,
                    "a castable ability that nothing in the script ever dispatches, it is inert")
                : Fail(WiringStatus.NotCastDispatched,
                    "no script presence, an aura or passive the engine drives from object data alone");
        }

        // 3-7. A map often keys more than one trigger to the same ability, and one can be an inert
        //       duplicate stub (empty body, no event) beside the real handler. Judging by a single
        //       arbitrarily chosen dispatch then blamed the ability for the stub, so run the whole
        //       chain for every candidate and let the ability read as wired if ANY of them completes.
        var evaluated = dispatches.Select(d => (Dispatch: d, Result: EvaluateDispatch(ability, name, d, ctx))).ToList();

        // A candidate that completes everything but builds its OWN trigger twice is a genuine
        // double-fire, so it outranks even a clean Ok elsewhere, a second healthy trigger must not
        // hide it.
        if (evaluated.FirstOrDefault(e => e.Result.Status == WiringStatus.InitCalledTwice) is { Result: { } } doubled)
            return doubled.Result;

        // 8. Two (or more) candidates can each independently complete the whole chain through
        //    DIFFERENT triggers, every one built exactly once, so InitCalledTwice above never trips
        //    on either. That is not a single healthy path, the ability still fires once per trigger
        //    per cast. Scoped to distinct TRIGGERS, not distinct dispatch functions, because a
        //    condition and an action attached to the very same trigger complete as two candidates but
        //    describe one firing, not two (see the "stub" test this file already covers).
        //
        //    Also scoped to the SAME event VERB. A real map keys a "_Channel" trigger to
        //    EVENT_UNIT_SPELL_CHANNEL (fires when the cast starts, often to validate it) alongside a
        //    plain trigger on EVENT_..._SPELL_EFFECT (fires when the cast actually resolves, the real
        //    payload), and that pair is a completely normal GUI pattern, not a duplicate. Measured on
        //    a real GGGA port, Tohno's own "#Toono"/"#Public" abilities are wired exactly that way and
        //    grouping by trigger alone read them as firing 2-3 times, a false alarm. Only triggers that
        //    share the SAME verb (two independently live EFFECT triggers, deliverable 1's real bug)
        //    genuinely run the same payload twice for the same cast.
        //
        //    Also excludes a candidate whose condition tests a FOREIGN ability (one not in
        //    heroAbilities), a shared roster-wide condition (an ability-mimicry system, a class-change
        //    trigger) that merely happens to also test our ability, not our hero's own dedicated
        //    dispatcher. Measured on the same GGGA map, Trig_Battle_Mage_Spell_start (a different
        //    hero's ability-copy system, also tests bm81/bm03/bm18/bm12/bm74 alongside Tohno's A01F)
        //    independently completes the chain on EFFECT too, and without this the verb filter alone
        //    misread it as A01F firing twice.
        //
        //    Finally, scoped to groups where at least one live trigger is THIS PROJECT'S OWN porting
        //    artifact, a synthesized --synth-dispatch trigger (named wc3ctl_SynthCast_<rawcode>), not
        //    a native multi-trigger design the SOURCE map's own author already wrote and shipped
        //    working. Measured on the same GGGA map, Trig_ChangeWay2 and Trig_TONA_Alternate_Start are
        //    both genuinely Tohno's OWN complementary "Change Way" triggers for A04Z (one a plain
        //    ability swap, one an elaborate cinematic), present since before any porting touched this
        //    map, so a user playing the unported source sees the exact same two firings. Deliverable
        //    1's real bug always has a wc3ctl_SynthCast_* dispatcher on one side of the pair, since
        //    that mechanism is what stacked a second live registration onto the source's own existing
        //    one, so requiring it here catches the genuine port-introduced double-fire without
        //    flagging a map's own pre-existing, working design.
        var okCandidates = evaluated.Where(e => e.Result.Status == WiringStatus.Ok).ToList();
        var liveTriggers = okCandidates
            .Where(e => !ctx.TestsForeignAbility(e.Dispatch, heroAbilities))
            .Select(e => ctx.ResolveTrigger(e.Dispatch).Trigger)
            .Where(t => t is not null).Select(t => t!)
            .Distinct(StringComparer.Ordinal).ToList();
        var duplicateVerb = liveTriggers.GroupBy(t => ctx.EventVerb(t) ?? "")
            .Where(g => g.Key.Length > 0 && g.Count() > 1)
            .Where(g => g.Any(t => t.Contains("wc3ctl_SynthCast_", StringComparison.Ordinal)))
            .OrderByDescending(g => g.Count()).FirstOrDefault();
        if (duplicateVerb is not null)
            return Fail(WiringStatus.MultipleLiveDispatchers,
                $"reached by {duplicateVerb.Count()} independently wired triggers all on "
                + $"EVENT_..._SPELL_{duplicateVerb.Key} ({string.Join(", ", duplicateVerb)}), "
                + $"so this ability fires {duplicateVerb.Count()} times per cast");

        if (okCandidates.FirstOrDefault().Result is { } wired)
            return wired;

        // Nothing completed. Report the candidate that got furthest along the chain, the most
        // informative failure, rather than an arbitrary one.
        return evaluated.Select(e => e.Result).OrderByDescending(e => ChainProgress(e.Status)).First();
    }

    /// <summary>Runs the attach-through-init chain for one dispatch candidate. Broken out so an
    /// ability keyed to several triggers can be judged by whichever candidate fares best, rather than
    /// by a single arbitrary pick.</summary>
    private static AbilityWiring EvaluateDispatch(string ability, string? name, string dispatch, ScriptContext ctx)
    {
        AbilityWiring Fail(WiringStatus s, string detail) => new(ability, name, s, detail);

        // 3. The dispatch must hang off a trigger, either directly or through a short chain of callers.
        //    An inline condition helper carries the id check but is not itself attached, its trigger's
        //    action or condition function is the one that calls it, so credit the nearest attached
        //    caller and name the function that actually carries the trigger.
        var (trigger, carrier) = ctx.ResolveTrigger(dispatch);
        if (trigger is null)
            return Fail(WiringStatus.NotAttachedToTrigger,
                $"neither handler '{dispatch}' nor anything that calls it is attached to a trigger");

        // 4. That trigger needs a spell event that covers this hero's player.
        var (hasEvent, coversPlayer, eventDetail) = ctx.EventCoverage(trigger);
        if (!hasEvent)
            return Fail(WiringStatus.NoEventRegistered, $"trigger '{trigger}' has no spell event");
        if (!coversPlayer)
            return Fail(WiringStatus.PlayerNotRegistered,
                $"trigger '{trigger}' registers its spell event per player and player {ctx.OwnerId} is not one of them ({eventDetail})");

        // 5. The code that runs must actually do something.
        if (!ctx.HasLiveBody(trigger, dispatch))
            return Fail(WiringStatus.EmptyOrTrimmedHandler,
                $"every statement behind trigger '{trigger}' is empty or was commented out by porting");

        // 6. If the handler gates on "this player's hero", the hero must be in that array.
        if (ctx.IdentityGate(dispatch) is { } gate && !ctx.RegisteredArrays.Contains(gate))
            return Fail(WiringStatus.IdentityArrayNotRegistered,
                $"handler '{dispatch}' only acts on the unit in {gate}[...], which this hero is never put into");

        // 7. The trigger has to be built exactly once. Zero means it never exists, two means the
        //    ability fires twice (doubled damage and effects).
        int calls = ctx.InitCallCount(trigger);
        if (calls == 0)
            return Fail(WiringStatus.InitNeverCalled,
                $"nothing calls the init that creates trigger '{trigger}'");
        if (calls > 1)
            return Fail(WiringStatus.InitCalledTwice,
                $"the init for trigger '{trigger}' is called {calls} times, so this ability fires {calls} times");

        string via = string.Equals(carrier, dispatch, StringComparison.Ordinal) ? "" : $" via '{carrier}'";
        return new(ability, name, WiringStatus.Ok, $"dispatched by '{dispatch}'{via} on '{trigger}'");
    }

    /// <summary>How far along the wiring chain a status reached. A higher value means the candidate
    /// came closer to working, so among failing candidates the highest is the most informative to
    /// report. Ok and InitCalledTwice are handled before this is consulted.</summary>
    private static int ChainProgress(WiringStatus status) => status switch
    {
        WiringStatus.NotAttachedToTrigger => 1,
        WiringStatus.NoEventRegistered => 2,
        WiringStatus.PlayerNotRegistered => 3,
        WiringStatus.EmptyOrTrimmedHandler => 4,
        WiringStatus.IdentityArrayNotRegistered => 5,
        WiringStatus.InitNeverCalled => 6,
        _ => 0,
    };

    // ---- the hero's ability set ---------------------------------------------

    /// <summary>The hero's abilities from its object fields (normal + hero + spellbook contents),
    /// plus any the script grants at runtime with <c>UnitAddAbility</c>. That last source is the one
    /// object data alone cannot show, and it is where a combo or dash-back ability hides.</summary>
    private static IEnumerable<string> HeroAbilities(MapDocument doc, string heroRawcode, ScriptContext ctx)
    {
        var found = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        void AddList(string? csv)
        {
            foreach (var raw in (csv ?? "").Split(','))
            {
                var t = raw.Trim();
                if (t.Length == 4 && seen.Add(t)) found.Add(t);
            }
        }

        var unitFields = Fields(doc, ObjectKind.Unit, heroRawcode);
        AddList(unitFields.GetValueOrDefault("uabi"));
        AddList(unitFields.GetValueOrDefault("uhab"));

        // Spellbooks hold their real abilities in spb1, so follow one level (that is how these maps
        // hang a hero's kit off a single granted book).
        foreach (var ability in found.ToList())
            foreach (var kv in Fields(doc, ObjectKind.Ability, ability))
                if (kv.Key.StartsWith("spb1", StringComparison.OrdinalIgnoreCase))
                    AddList(kv.Value);

        // Runtime grants: UnitAddAbility(u, 'A1R6') or UnitAddAbility(u, SomeQ2_ID).
        foreach (Match m in Regex.Matches(ctx.CodeText,
            @"UnitAddAbility\s*\([^,]+,\s*(?:'([^']{4})'|([A-Za-z_][A-Za-z0-9_]*))\s*\)"))
        {
            string? id = m.Groups[1].Success ? m.Groups[1].Value
                : ctx.Aliases.GetValueOrDefault(m.Groups[2].Value);
            // Only the ones this hero's own kit references, so a map-wide scan does not pull in
            // every other hero's grants. An id global named after the hero counts, as does an
            // ability already in the set.
            if (id is null || !seen.Add(id)) continue;
            if (ctx.CustomAbilities.Contains(id) && ctx.SharesNamingGroup(heroRawcode, m.Groups[2].Value))
                found.Add(id);
            else seen.Remove(id);
        }
        return found;
    }

    private static Dictionary<string, string> Fields(MapDocument doc, ObjectKind kind, string rawcode)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var info = ObjectKinds.Info(kind);
        int id = rawcode.FromRawcode();
        foreach (var e in ObjectKinds.MergedEntries(doc, info))
        {
            if (e.Id != id) continue;
            foreach (var mod in e.Mods) result[mod.Key] = mod.Value;
        }
        return result;
    }

    private static Dictionary<string, string> AbilityNames(MapDocument doc) => Names(doc, ObjectKind.Ability);
    private static Dictionary<string, string> UnitNames(MapDocument doc) => Names(doc, ObjectKind.Unit);

    private static Dictionary<string, string> Names(MapDocument doc, ObjectKind kind)
    {
        var info = ObjectKinds.Info(kind);
        var strings = MapStrings.From(doc);
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var e in ObjectKinds.MergedEntries(doc, info))
            if (ObjectKinds.DeltaName(ObjectKinds.ModsToDict(e.Mods), info, strings) is { } n)
                result[e.Id.ToRawcode()] = n;
        return result;
    }

    // ---- script analysis ----------------------------------------------------

    /// <summary>Everything the checks need from the script, parsed once per hero.</summary>
    private sealed class ScriptContext
    {
        public int OwnerId { get; }
        public string CodeText { get; }
        public Dictionary<string, string> Aliases { get; }
        public HashSet<string> CustomAbilities { get; }
        public HashSet<string> RegisteredArrays { get; }

        private readonly string[] _code;                       // comment-stripped, same indices
        private readonly IReadOnlyList<JassFunction> _functions;
        private readonly Dictionary<string, string> _bodies;    // function -> comment-stripped body
        private readonly Dictionary<string, HashSet<string>> _abilityRefs; // ability -> dispatch fns
        private readonly MapDocument _doc;                      // for on-demand object-field lookups

        public ScriptContext(string jass, int ownerId, MapDocument doc)
        {
            OwnerId = ownerId;
            _doc = doc;
            _functions = JassFunctionIndex.Parse(jass);
            Aliases = JassRawcodeAliases.Parse(jass);

            var lines = jass.Replace("\r\n", "\n").Split('\n');
            _code = StripComments(lines);
            CodeText = string.Join("\n", _code);

            _bodies = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var f in _functions)
            {
                var sb = new StringBuilder();
                for (int i = f.StartLine - 1; i < f.EndLine && i < _code.Length; i++)
                    sb.Append(_code[i]).Append('\n');
                _bodies.TryAdd(f.Name, sb.ToString());
            }

            CustomAbilities = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability))
                .Select(e => e.Id.ToRawcode()).ToHashSet(StringComparer.Ordinal);

            // Which per-player arrays the generated block actually writes this hero into. The audit
            // compares a handler's identity gate against exactly this, rather than guessing.
            RegisteredArrays = Regex.Matches(CodeText, @"set\s+([A-Za-z_][A-Za-z0-9_]*)\s*\[[^\]]*\]\s*=\s*u\b")
                .Select(m => m.Groups[1].Value).ToHashSet(StringComparer.Ordinal);

            _abilityRefs = BuildAbilityRefs();
        }

        /// <summary>Base-game abilities are present in any map, so a 4-char code the map does not
        /// define is treated as base rather than missing (no game data needed for that call).</summary>
        public bool BaseLikeAbility(string ability) =>
            ability.Length == 4 && !ability.StartsWith("A0", StringComparison.Ordinal)
                               && !ability.StartsWith("A1", StringComparison.Ordinal)
                               && !ability.StartsWith("A2", StringComparison.Ordinal);

        /// <summary>Maps each ability to the functions that dispatch on it, resolving id globals.</summary>
        private Dictionary<string, HashSet<string>> BuildAbilityRefs()
        {
            var map = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

            void AddAbilityRef(string? ability, string functionName)
            {
                if (ability is null) return;
                if (!map.TryGetValue(ability, out var set))
                    map[ability] = set = new HashSet<string>(StringComparer.Ordinal);
                set.Add(functionName);
            }

            string? ResolveAbilityToken(string token, HashSet<string> localNames)
            {
                if (token.Length == 6 && token[0] == '\'' && token[^1] == '\'')
                    return token[1..^1];
                return localNames.Contains(token) ? null : Aliases.GetValueOrDefault(token);
            }

            var spellId = new Regex(
                @"GetSpellAbilityId\s*\(\s*\)\s*==\s*(?:'([^']{4})'|([A-Za-z_][A-Za-z0-9_]*))");
            var localDeclaration = new Regex(
                @"^\s*local\s+[A-Za-z_][A-Za-z0-9_]*\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)\b");
            var integerDeclaration = new Regex(
                @"^\s*local\s+integer\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)\b(?:\s*=\s*(.+?)\s*)?$");
            var setAssignment = new Regex(
                @"^\s*set\s+([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(.+?)\s*$");
            var spellIdValue = new Regex(@"^\s*GetSpellAbilityId\s*\(\s*\)\s*$");
            var equality = new Regex(
                @"('(?:[^']{4})'|[A-Za-z_][A-Za-z0-9_]*)\s*==\s*('(?:[^']{4})'|[A-Za-z_][A-Za-z0-9_]*)");
            foreach (var f in _functions)
            {
                if (!_bodies.TryGetValue(f.Name, out var body)) continue;
                foreach (Match m in spellId.Matches(body))
                {
                    string? id = m.Groups[1].Success ? m.Groups[1].Value
                        : Aliases.GetValueOrDefault(m.Groups[2].Value);
                    AddAbilityRef(id, f.Name);
                }

                var localNames = new HashSet<string>(StringComparer.Ordinal);
                var integerLocals = new HashSet<string>(StringComparer.Ordinal);
                var spellIdLocals = new HashSet<string>(StringComparer.Ordinal);

                foreach (var line in body.Split('\n'))
                {
                    if (localDeclaration.Match(line) is { Success: true } local)
                        localNames.Add(local.Groups[1].Value);

                    if (integerDeclaration.Match(line) is { Success: true } declaration)
                    {
                        string name = declaration.Groups[1].Value;
                        integerLocals.Add(name);
                        if (declaration.Groups[2].Success && spellIdValue.IsMatch(declaration.Groups[2].Value))
                            spellIdLocals.Add(name);
                        else
                            spellIdLocals.Remove(name);
                    }
                    else if (setAssignment.Match(line) is { Success: true } set)
                    {
                        string name = set.Groups[1].Value;
                        if (integerLocals.Contains(name))
                        {
                            if (spellIdValue.IsMatch(set.Groups[2].Value))
                                spellIdLocals.Add(name);
                            else
                                spellIdLocals.Remove(name);
                        }
                    }

                    foreach (Match m in equality.Matches(line))
                    {
                        string left = m.Groups[1].Value;
                        string right = m.Groups[2].Value;

                        if (spellIdLocals.Contains(left))
                            AddAbilityRef(ResolveAbilityToken(right, localNames), f.Name);
                        else if (spellIdLocals.Contains(right))
                            AddAbilityRef(ResolveAbilityToken(left, localNames), f.Name);
                    }
                }
            }
            return map;
        }

        /// <summary>The script mentions the ability somewhere other than an id-global declaration.</summary>
        public bool IsReferenced(string ability)
        {
            int refs = Regex.Matches(CodeText, "'" + Regex.Escape(ability) + "'").Count;
            bool aliased = Aliases.Values.Any(v => string.Equals(v, ability, StringComparison.Ordinal));
            return refs > (aliased ? 1 : 0);
        }

        /// <summary>Every function that dispatches on the ability, sorted for deterministic reporting.
        /// A map can key several triggers to one ability, so all of them are candidates, not just one.</summary>
        public IReadOnlyList<string> FindDispatches(string ability) =>
            _abilityRefs.TryGetValue(ability, out var fns)
                ? fns.OrderBy(x => x, StringComparer.Ordinal).ToList()
                : Array.Empty<string>();

        /// <summary>True when the ability's own map fields carry an active-cast trait no passive has,
        /// a real Targets Allowed or a positive cast cost/cooldown/range. Auras, inventories and
        /// base-game passives carry none of these. Only the map's deltas are visible here (no game
        /// data), so an ability with no such trait is read as passive, which is the conservative side
        /// of the passive-versus-inert call.</summary>
        public bool LooksCastable(string ability)
        {
            var fields = Fields(_doc, ObjectKind.Ability, ability);
            // A Targets Allowed list naming anything the caster aims at (beyond itself) is the surest
            // active tell. Then any positive cast cost, cooldown, range, or channel target-type (Ncl2
            // > 0 means the spell targets a unit or a point). Auras and self-buffs carry none of these.
            return HasCastTarget(fields, "atar")
                || Positive(fields, "acdn") || Positive(fields, "amcs")
                || Positive(fields, "aran") || Positive(fields, "Ncl2");
        }

        // Ability fields are per-level, keyed by the bare code or "code:N", so match either shape.
        private static IEnumerable<string> ValuesOf(IReadOnlyDictionary<string, string> fields, string code) =>
            fields.Where(kv => string.Equals(kv.Key, code, StringComparison.OrdinalIgnoreCase)
                            || kv.Key.StartsWith(code + ":", StringComparison.OrdinalIgnoreCase))
                  .Select(kv => kv.Value);

        /// <summary>True when a Targets Allowed list names a target the caster aims at, that is any
        /// token beyond "self" and the no-target markers. A pure "self" list is an aura or self-buff,
        /// which is exactly the passive that must not read as a castable active.</summary>
        private static bool HasCastTarget(IReadOnlyDictionary<string, string> fields, string code) =>
            ValuesOf(fields, code)
                .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Any(t => t.Length > 0
                    && !t.Equals("self", StringComparison.OrdinalIgnoreCase)
                    && !t.Equals("none", StringComparison.OrdinalIgnoreCase)
                    && t is not "_" and not "-");

        private static bool Positive(IReadOnlyDictionary<string, string> fields, string code) =>
            ValuesOf(fields, code).Any(v =>
                double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) && d > 0);

        /// <summary>The gg_trg_* the function is attached to, as a condition or an action.</summary>
        public string? TriggerFor(string function) => FindAttachedTrigger(CodeText, function);

        /// <summary>The trigger a dispatch is wired to, directly or through a bounded chain of callers,
        /// plus the function that actually carries it. A dispatch found inside an inline condition
        /// helper is not attached itself, some caller (the trigger's action or condition function) is,
        /// so walk callers outward a few levels and take the nearest attached one. Returns
        /// (null, dispatch) only when nothing in that chain is attached.</summary>
        public (string? Trigger, string Carrier) ResolveTrigger(string dispatch)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal) { dispatch };
            var frontier = new List<string> { dispatch };
            // Depth 3 is ample, these helper chains are one or two hops (helper, then actions, then trigger).
            for (int depth = 0; depth <= 3 && frontier.Count > 0; depth++)
            {
                // Deterministic within a level so the reported carrier is stable across runs.
                foreach (var fn in frontier.OrderBy(x => x, StringComparer.Ordinal))
                    if (TriggerFor(fn) is { } trg) return (trg, fn);

                var next = new List<string>();
                foreach (var fn in frontier)
                    foreach (var caller in CallersOf(fn))
                        if (seen.Add(caller)) next.Add(caller);
                frontier = next;
            }
            return (null, dispatch);
        }

        /// <summary>Functions whose body invokes <paramref name="callee"/>, whether as "call callee("
        /// or bare "callee(" (JASS condition helpers are called without the "call" keyword).</summary>
        private IEnumerable<string> CallersOf(string callee)
        {
            var invoked = new Regex(@"\b" + Regex.Escape(callee) + @"\s*\(");
            foreach (var f in _functions)
                if (!string.Equals(f.Name, callee, StringComparison.Ordinal)
                    && _bodies.TryGetValue(f.Name, out var body)
                    && invoked.IsMatch(body))
                    yield return f.Name;
        }

        /// <summary>Whether the trigger has a spell event, and whether it reaches this hero's player.
        /// An any-unit registration covers everyone, a per-player one must name our player.</summary>
        public (bool HasEvent, bool CoversPlayer, string Detail) EventCoverage(string trigger)
        {
            string esc = Regex.Escape(trigger);
            bool anyUnit = Regex.IsMatch(CodeText,
                @"TriggerRegisterAnyUnitEventBJ\s*\(\s*" + esc + @"\s*,\s*EVENT_PLAYER_UNIT_SPELL");
            if (anyUnit) return (true, true, "any-unit event");

            // Per-UNIT registration, TriggerRegisterUnitEvent(trg, udg_X_Caster, EVENT_UNIT_SPELL_EFFECT).
            // These arenas bind a hero's triggers to the single unit held in its caster global, set by
            // the map's own hero setup routine, which the generated block now calls for a placed hero.
            // The bound unit is a runtime value so coverage cannot be proven statically, but the
            // registration plainly exists, and reporting it as "no event" was simply wrong.
            var perUnit = Regex.Matches(CodeText,
                @"TriggerRegisterUnitEvent\s*\(\s*" + esc + @"\s*,\s*([^,]+),\s*EVENT_UNIT_SPELL");
            if (perUnit.Count > 0)
                return (true, true,
                    $"per-unit event bound to {perUnit[0].Groups[1].Value.Trim()} by the map's hero setup");

            var perPlayer = Regex.Matches(CodeText,
                @"TriggerRegisterPlayerUnitEvent\s*\(\s*" + esc + @"\s*,\s*([^,]+),\s*EVENT_PLAYER_UNIT_SPELL");
            if (perPlayer.Count == 0)
            {
                bool otherSpellEvent = Regex.IsMatch(CodeText,
                    @"TriggerRegister[A-Za-z]*Event[A-Za-z]*\s*\(\s*" + esc + @"\s*,[^)]*EVENT_PLAYER_UNIT_SPELL");
                return (otherSpellEvent, otherSpellEvent, otherSpellEvent ? "spell event" : "none");
            }

            // Our generated block registers with a literal Player(N); the arena's own registrations
            // use a runtime expression, which we cannot resolve, so treat those as not proven.
            bool covered = perPlayer.Cast<Match>().Any(m =>
                Regex.IsMatch(m.Groups[1].Value, @"\bPlayer\s*\(\s*" + OwnerId + @"\s*\)"));
            return (true, covered, $"{perPlayer.Count} per-player registration(s)");
        }

        /// <summary>The spell-lifecycle verb (EFFECT, CHANNEL, CAST, ENDCAST, FINISH, ...) the
        /// trigger's own spell event registers, the suffix of EVENT_(PLAYER_)UNIT_SPELL_&lt;verb&gt;.
        /// Null when no such event is registered at all. Two triggers on the SAME verb for the same
        /// ability genuinely run its payload twice (deliverable 1's real double registration, both on
        /// EFFECT); a CHANNEL trigger and an EFFECT trigger legitimately coexist for one ability, one
        /// validates the cast when it starts, the other applies it when it resolves, and the two must
        /// never be confused for a duplicate (see <see cref="WiringStatus.MultipleLiveDispatchers"/>'s
        /// only caller for the real map this was measured against).</summary>
        public string? EventVerb(string trigger)
        {
            string esc = Regex.Escape(trigger);
            var any = Regex.Match(CodeText,
                @"TriggerRegisterAnyUnitEventBJ\s*\(\s*" + esc + @"\s*,\s*EVENT_(?:PLAYER_)?UNIT_SPELL_([A-Z]+)");
            if (any.Success) return any.Groups[1].Value;

            var perUnit = Regex.Match(CodeText,
                @"TriggerRegisterUnitEvent\s*\(\s*" + esc + @"\s*,\s*[^,]+,\s*EVENT_UNIT_SPELL_([A-Z]+)");
            if (perUnit.Success) return perUnit.Groups[1].Value;

            var perPlayer = Regex.Match(CodeText,
                @"TriggerRegisterPlayerUnitEvent\s*\(\s*" + esc + @"\s*,\s*[^,]+,\s*EVENT_PLAYER_UNIT_SPELL_([A-Z]+)");
            return perPlayer.Success ? perPlayer.Groups[1].Value : null;
        }

        private static readonly Regex AbilityIdCheck = new(
            @"GetSpellAbilityId\s*\(\s*\)\s*==\s*(?:'([^']{4})'|([A-Za-z_][A-Za-z0-9_]*))", RegexOptions.Compiled);

        /// <summary>True when <paramref name="function"/>'s own id-check, or a one-hop inline
        /// condition helper it plainly calls (the "Trig_X_Func001C" shape GUI compiles for a
        /// multi-branch OR), tests an ability rawcode that is NOT one of
        /// <paramref name="heroAbilities"/>. That is the signature of a shared, roster-wide condition
        /// (an ability-mimicry system, a class-change/stance-swap trigger) that merely happens to
        /// also test our hero's ability, not our hero's own dedicated dispatcher, so its Ok verdict
        /// must never count as a genuinely separate live path for
        /// <see cref="WiringStatus.MultipleLiveDispatchers"/>.</summary>
        public bool TestsForeignAbility(string function, IReadOnlySet<string> heroAbilities)
        {
            var toScan = new List<string> { function };
            if (_bodies.TryGetValue(function, out var ownBody))
                foreach (Match m in Regex.Matches(ownBody, @"\b([A-Za-z_][A-Za-z0-9_]*)\s*\("))
                    if (m.Groups[1].Value != function && _bodies.ContainsKey(m.Groups[1].Value))
                        toScan.Add(m.Groups[1].Value);

            foreach (var fn in toScan)
            {
                if (!_bodies.TryGetValue(fn, out var body)) continue;
                foreach (Match m in AbilityIdCheck.Matches(body))
                {
                    string? id = m.Groups[1].Success ? m.Groups[1].Value : Aliases.GetValueOrDefault(m.Groups[2].Value);
                    if (id is not null && !heroAbilities.Contains(id)) return true;
                }
            }
            return false;
        }

        /// <summary>True when something behind the trigger still has executable statements. A ported
        /// handler whose every call was commented out compiles but does nothing.</summary>
        public bool HasLiveBody(string trigger, string dispatch)
        {
            var names = new HashSet<string>(StringComparer.Ordinal) { dispatch };
            foreach (Match m in Regex.Matches(CodeText,
                @"TriggerAdd(?:Condition|Action)\s*\(\s*" + Regex.Escape(trigger)
                + @"\s*,[^)]*?\bfunction\s+([A-Za-z_][A-Za-z0-9_]*)"))
                names.Add(m.Groups[1].Value);

            foreach (var n in names)
            {
                if (!_bodies.TryGetValue(n, out var body)) continue;
                foreach (var line in body.Split('\n'))
                {
                    var t = line.Trim();
                    if (t.Length == 0) continue;
                    if (t.StartsWith("function", StringComparison.Ordinal)
                        || t.StartsWith("endfunction", StringComparison.Ordinal)
                        || t.StartsWith("local ", StringComparison.Ordinal)) continue;
                    if (t.StartsWith("call ", StringComparison.Ordinal)
                        || t.StartsWith("set ", StringComparison.Ordinal)
                        || t.StartsWith("return ", StringComparison.Ordinal)
                        || t.StartsWith("if ", StringComparison.Ordinal)) return true;
                }
            }
            return false;
        }

        /// <summary>The per-player array a handler gates on ("... == Hero[GetPlayerId(p)]"), if any.</summary>
        public string? IdentityGate(string dispatch)
        {
            if (!_bodies.TryGetValue(dispatch, out var body)) return null;
            var m = Regex.Match(body,
                @"(?:GetTriggerUnit\(\)|GetSpellAbilityUnit\(\))\s*==\s*([A-Za-z_][A-Za-z0-9_]*)\s*\["
                + @"|([A-Za-z_][A-Za-z0-9_]*)\s*\[[^\]]*\]\s*==\s*(?:GetTriggerUnit\(\)|GetSpellAbilityUnit\(\))");
            if (!m.Success) return null;
            return m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
        }

        /// <summary>How many times the init that creates this trigger is called.</summary>
        public int InitCallCount(string trigger)
        {
            var creator = _functions.FirstOrDefault(f =>
                _bodies.TryGetValue(f.Name, out var b)
                && Regex.IsMatch(b, @"set\s+" + Regex.Escape(trigger) + @"\s*=\s*CreateTrigger"));
            if (creator is null)
                return Regex.IsMatch(CodeText, @"set\s+" + Regex.Escape(trigger) + @"\s*=\s*CreateTrigger") ? 1 : 0;
            return Regex.Matches(CodeText, @"\bcall\s+" + Regex.Escape(creator.Name) + @"\s*\(").Count;
        }

        /// <summary>True when an id global's name shares the hero's naming stem, the convention these
        /// maps use to group one hero's ids (DarkShiki_ID, DarkShikiQ_ID, DarkShikiQ2_ID).</summary>
        public bool SharesNamingGroup(string heroRawcode, string globalName)
        {
            if (string.IsNullOrEmpty(globalName)) return false;
            var heroGlobal = Aliases.FirstOrDefault(kv =>
                string.Equals(kv.Value, heroRawcode, StringComparison.Ordinal)).Key;
            if (heroGlobal is null) return false;
            string stem = heroGlobal.EndsWith("_ID", StringComparison.OrdinalIgnoreCase)
                ? heroGlobal[..^3] : heroGlobal;
            return stem.Length >= 4 && globalName.StartsWith(stem, StringComparison.Ordinal);
        }

        private static string[] StripComments(string[] lines)
        {
            var result = new string[lines.Length];
            bool inBlock = false;
            for (int i = 0; i < lines.Length; i++)
            {
                var sb = new StringBuilder(lines[i].Length);
                var line = lines[i];
                for (int c = 0; c < line.Length; c++)
                {
                    if (inBlock)
                    {
                        if (c + 1 < line.Length && line[c] == '*' && line[c + 1] == '/') { inBlock = false; c++; }
                        continue;
                    }
                    if (c + 1 < line.Length && line[c] == '/' && line[c + 1] == '/') break;
                    if (c + 1 < line.Length && line[c] == '/' && line[c + 1] == '*') { inBlock = true; c++; continue; }
                    sb.Append(line[c]);
                }
                result[i] = sb.ToString();
            }
            return result;
        }
    }
}
