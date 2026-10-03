// src/Wc3.Commands/PreplacedUnitsScript.cs
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Build.Widget;      // MapUnits, UnitData
using War3Net.Common.Extensions; // ToRawcode
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Generates the JASS that actually spawns a map's preplaced units and items at runtime.
///
/// <para>Warcraft III does NOT read war3mapUnits.doo directly for a map that ships a custom
/// war3map.j. For such a map the World Editor bakes a <c>CreateAllUnits</c> function (and a
/// <c>CreateAllItems</c> one) into the script and calls them from <c>main</c>; that generated
/// code is what brings preplaced widgets to life. A map whose .doo holds units but whose script
/// has no such function shows nothing in game, even though the editor still draws the widgets
/// from the .doo. wc3ctl and the Studio only wrote the .doo, so this closes the gap: after any
/// placement the creation script is regenerated and wired into <c>main</c>.</para>
///
/// <para>The generated code lives between two marker comments so a later sync can replace exactly
/// its own block and never touch a map's hand-written script. If a map already carries its own
/// (foreign) <c>CreateAllUnits</c> that we did not generate, the sync leaves the script untouched,
/// so we never double-create or clobber a real World-Editor map.</para>
/// </summary>
public static class PreplacedUnitsScript
{
    public const string ScriptFile = "war3map.j";

    /// <summary>
    /// What this generator is capable of emitting. Bump it whenever the generated block gains
    /// behaviour a map would lose by being regenerated with an older build.
    ///
    /// Its purpose is to stop a silent downgrade. An older binary regenerating a newer block used to
    /// quietly strip capabilities (hero registration and spell-dispatch wiring), leaving a map that
    /// still compiles and hosts but whose heroes cannot cast, which is far harder to spot than a
    /// crash. Version 1 was the original create-only block, 2 added the per-instance unit properties
    /// and hero-array registration, 3 added the deferred spell-dispatch wiring, 4 registers EVERY per-player hero array
    /// (both 0-based and 1-based) rather than only the most-used one, 5 calls the map own
    /// per-hero setup routine when it has one, 6 also calls the map own per-player doer-dummy setup
    /// routine (the one that populates a per-player unit array from a CreateUnit, e.g. the stun and
    /// damage-source dummies a hero's spells route through) under a null guard so it runs once, 7
    /// resolves a doer-dummy slot computed through a local rather than only a direct index, 8 stops
    /// registering a placed hero's owner on the source's shared cast dispatcher when a --synth-dispatch
    /// dispatcher already covers that hero (an older build re-registers it and every one of that
    /// hero's spells fires twice, once through each path), 9 also stops registering a dispatcher
    /// trigger the target's OWN script already reachably registers for the same event (an arena's
    /// per-player pick-hero flow, a level-up gate), an older build re-registers it too and every
    /// ability that player casts through it, including a hero we never touched, fires twice, 10 also
    /// grants a placed hero its own level up abilities at spawn, the ones an arena's level up handler
    /// would otherwise only add the first time that hero actually levels up in game, an event a hero
    /// created and levelled in one shot never fires for, an older build leaves a placed hero missing
    /// whatever abilities its kit only grants that way.
    /// </summary>
    public const int GeneratorVersion = 10;

    // The begin marker carries the generator version. Detection keys off the PREFIX so blocks
    // written before versioning existed (no "[gen vN]") are still recognised, and read as version 0.
    private const string BeginMarkerPrefix = "//=== wc3ctl preplaced widgets (generated, do not edit)";
    private static string BeginMarker => $"{BeginMarkerPrefix} [gen v{GeneratorVersion}] ===";
    private const string EndMarker = "//=== end wc3ctl preplaced widgets ===";

    private static readonly Regex BlockVersionPattern = new(
        @"^//=== wc3ctl preplaced widgets \(generated, do not edit\)(?: \[gen v(\d+)\])? ===",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private const string UnitsFunc = "CreateAllUnits";
    private const string ItemsFunc = "CreateAllItems";
    // Internal, not private: DebugWiringCommand instruments this exact generated function on an
    // already-ported map, and reuses the fixed name rather than keeping its own copy of it.
    internal const string WireSpellsFunc = "wc3ctl_WirePlacedHeroSpells";

    public sealed record SyncResult(bool Ok, string Message, int Units, int Items);

    /// <summary>
    /// State of the generated block in a map. <paramref name="NeedsSpellWiring"/> is true when the
    /// map has placed player-owned units AND the script dispatches spells through per-player
    /// triggers (the arena pattern), which is exactly when the block must register those triggers or
    /// the placed heroes silently cannot cast.
    /// </summary>
    public sealed record BlockAudit(
        bool HasBlock,
        int BlockVersion,
        int CurrentVersion,
        bool NeedsSpellWiring,
        bool HasSpellWiring)
    {
        public bool IsStale => HasBlock && BlockVersion < CurrentVersion;
        /// <summary>The consequential failure: the wiring is required but absent, so casts go nowhere.</summary>
        public bool IsMissingSpellWiring => HasBlock && NeedsSpellWiring && !HasSpellWiring;
    }

    /// <summary>
    /// Inspects the generated block without changing anything, so a degraded map can be reported.
    /// This exists because a stale block still compiles and still hosts, so neither the loader nor
    /// the JASS checker can see the problem, yet every placed hero is mute.
    /// </summary>
    public static BlockAudit Audit(MapDocument doc)
    {
        var entry = doc.GetFile(ScriptFile);
        byte[]? bytes = entry?.CurrentBytes;
        if (bytes is null || bytes.Length == 0)
            return new(false, 0, GeneratorVersion, false, false);

        string jass = Encoding.Latin1.GetString(bytes);
        if (!jass.Contains(BeginMarkerPrefix, StringComparison.Ordinal))
            return new(false, 0, GeneratorVersion, false, false);

        // Mirror the generator's own condition for emitting the wiring, so the audit and the
        // generator can never disagree about whether a block ought to have it.
        var units = doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        bool anyOwnedUnit = units?.Units.Any(u =>
            IsSpawnableUnit(u) && u.OwnerId >= 0 && u.OwnerId < PlayerColors.NeutralHostileId) ?? false;
        bool dispatchesSpells = DetectPerPlayerSpellTriggers(jass).Count > 0;

        return new(
            HasBlock: true,
            BlockVersion: BlockVersionOf(jass),
            CurrentVersion: GeneratorVersion,
            NeedsSpellWiring: anyOwnedUnit && dispatchesSpells,
            HasSpellWiring: jass.Contains("function " + WireSpellsFunc, StringComparison.Ordinal));
    }

    /// <summary>The generator version recorded in the block's begin marker, 0 when it predates
    /// versioning, -1 when there is no block at all.</summary>
    private static int BlockVersionOf(string jass)
    {
        var m = BlockVersionPattern.Match(jass);
        if (!m.Success) return -1;
        return m.Groups[1].Success ? int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) : 0;
    }

    /// <summary>
    /// Regenerates the preplaced-widget creation script for <paramref name="doc"/> from its
    /// current war3mapUnits.doo and wires the calls into <c>main</c>. Idempotent, running it
    /// twice with the same placements yields the same script. Never throws for an ordinary map,
    /// a missing or foreign script is reported through <see cref="SyncResult"/> instead.
    /// </summary>
    public static SyncResult Sync(MapDocument doc)
    {
        var scriptEntry = doc.GetFile(ScriptFile);
        // The effective script is any pending raw override, else the original bytes. Reading
        // RawBytes alone would miss what an earlier sync in this same session already wrote,
        // which would leave a stale block behind when placements change (e.g. a phantom unit
        // after the last one is removed).
        byte[]? effective = scriptEntry?.CurrentBytes;
        if (scriptEntry is null || effective is null || effective.Length == 0)
            return new(false, "no war3map.j to wire (a scriptless melee map spawns preplaced widgets from the .doo directly)", 0, 0);

        var units = doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        var spawnable = units?.Units.Where(IsSpawnableUnit).ToList() ?? new List<UnitData>();
        var items = units?.Units.Where(IsItem).ToList() ?? new List<UnitData>();

        // Latin1 is one byte per char, so the script round-trips byte-for-byte.
        var enc = Encoding.Latin1;
        string jass = enc.GetString(effective);
        string nl = jass.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";

        bool hasOurBlock = jass.Contains(BeginMarkerPrefix, StringComparison.Ordinal);

        // Never downgrade. If the block was written by a NEWER generator, this build would silently
        // strip capabilities it does not know about (that is exactly how a map's heroes ended up
        // unable to cast: an older binary regenerated a newer block and dropped its spell wiring).
        // Leaving it alone keeps the better block, and the message names the fix.
        int existingVersion = BlockVersionOf(jass);
        if (hasOurBlock && existingVersion > GeneratorVersion)
            return new(false,
                $"generated block is from a newer wc3ctl (gen v{existingVersion} > v{GeneratorVersion}), "
                + "left untouched so its newer behaviour is not stripped, update wc3ctl and re-run", 0, 0);

        // A real World-Editor map owns its own unit creation. Do not touch it.
        if (!hasOurBlock && MentionsForeignFunction(jass, UnitsFunc))
            return new(false, "map already creates its preplaced units through its own script, left untouched", 0, 0);

        // Nothing to create and nothing of ours to clean up: leave the file byte-identical.
        if (spawnable.Count == 0 && items.Count == 0 && !hasOurBlock)
            return new(true, "no preplaced widgets to wire", 0, 0);

        // Rebuild from a clean slate: drop our old block, then splice a fresh one (if any) in
        // just before main so every function it defines exists before main calls it.
        // An arena registers each player's hero in a global unit array (Hero[GetPlayerId(p)]=u), and
        // its spell handlers only act on a unit that is in that array. A placed hero never runs the
        // arena's selection flow, so it is absent and its spells do nothing. Detecting that array lets
        // CreateAllUnits register the placed hero into it, a best-effort so a ported arena hero can cast.
        //
        // Our own earlier block, if any, is stripped BEFORE every detector below runs, or a re-sync
        // would see its own previously emitted registration line and mistake it for something the map
        // itself provides. None of the other detectors ever match text that lives only inside our own
        // block (it declares no hero array, no per-hero setup, no doer-dummy assigner, no
        // wc3ctl_SynthCast_ function, all of those come from the map's own script or a different
        // generator entirely), so stripping first changes nothing else about their output.
        jass = RemoveBlock(jass, nl);

        var heroArrays = DetectHeroArrays(jass);
        var spellTriggers = DetectPerPlayerSpellTriggers(jass);
        // Placed hero types per owner (uppercase first char is the World Editor hero convention).
        var placedHeroes = spawnable
            .Where(u => u.OwnerId >= 0 && u.OwnerId < PlayerColors.NeutralHostileId)
            .Select(u => (Rawcode: u.TypeId.ToRawcode(), u.OwnerId))
            .Where(x => x.Rawcode.Length == 4 && char.IsUpper(x.Rawcode[0]))
            .Distinct().ToList();
        var heroSetup = DetectHeroSetup(jass, placedHeroes.Select(h => h.Rawcode).Distinct().ToList());
        // Only relevant once a per-hero setup is wired (an arena), the doer-dummy setup rides alongside it.
        var (doerDummy, doerCandidates) = heroSetup is not null && placedHeroes.Count > 0
            ? DetectDoerDummyAssigner(jass)
            : (null, 0);
        // Heroes already covered by their own --synth-dispatch dispatcher, see DetectSynthDispatchedHeroes,
        // must not also be registered on the shared dispatcher below or their spells fire twice.
        var synthDispatchedHeroes = DetectSynthDispatchedHeroes(jass, placedHeroes.Select(h => h.Rawcode).Distinct().ToList());
        // A trigger the map's OWN script already reachably registers for a spell-effect event (an
        // arena's per-player pick-hero flow, a level-up gate) must not ALSO be registered by us, see
        // DetectAlreadyLiveSpellTriggers, or WC3 fires it twice for whichever player the map's own
        // path eventually covers, including a hero we never touched.
        var alreadyLiveSpellTriggers = DetectAlreadyLiveSpellTriggers(jass, spellTriggers);
        // An ability an arena grants only once a hero actually levels up in game (see
        // SynthDispatchBuilder.ExtractHeroLevelUpGrants) never reaches a PREPLACED hero, CreateAllUnits
        // creates and levels it in one shot so that event never fires. Grant those at spawn instead.
        var levelUpGrants = DetectLevelUpGrants(jass, placedHeroes.Select(h => h.Rawcode).Distinct().ToList());

        if (spawnable.Count > 0 || items.Count > 0)
        {
            string block = BuildBlock(spawnable, items, nl, heroArrays, spellTriggers, heroSetup, placedHeroes,
                doerDummy, synthDispatchedHeroes, alreadyLiveSpellTriggers, levelUpGrants);
            jass = InsertBefore(jass, "function main takes nothing returns nothing", block + nl, nl);
        }

        // Wire (or unwire) the calls in main to match what the block now defines.
        jass = EnsureMainCall(jass, UnitsFunc, wanted: spawnable.Count > 0, nl);
        jass = EnsureMainCall(jass, ItemsFunc, wanted: items.Count > 0, nl);

        // Back to the entry it was READ from, which is not always the root name. 13 of 34
        // maps measured keep the script at scripts\\war3map.j, and writing to the hardcoded
        // root left those maps with two disagreeing scripts.
        ScriptCommand.Write(doc, enc.GetBytes(jass));

        string message = $"wired {spawnable.Count} unit(s) and {items.Count} item(s) into main()";
        if (heroSetup is not null && placedHeroes.Count > 0)
        {
            if (doerDummy is not null)
                message += $", plus the per-player doer-dummy setup {doerDummy.Function} (guarded)";
            else if (doerCandidates == 0)
                message += ", no per-player doer-dummy setup routine detected so none wired";
            else
                message += $", declined doer-dummy wiring ({doerCandidates} candidate routines, ambiguous)";
        }
        int grantedAbilities = levelUpGrants.Values.Sum(g => g.Count);
        if (grantedAbilities > 0)
            message += $", plus {grantedAbilities} level up ability grant(s) for {levelUpGrants.Count} hero(es) at spawn";
        return new(true, message, spawnable.Count, items.Count);
    }

    /// <summary>A spawnable unit is a real unit (not an item slot) and not a start location
    /// (those become <c>DefineStartLocation</c> in config, never a created unit).</summary>
    private static bool IsSpawnableUnit(UnitData u) =>
        u.OwnerId != PlacementCommand.ItemOwnerId && u.TypeId != StartLocationTypeId;

    private static bool IsItem(UnitData u) => u.OwnerId == PlacementCommand.ItemOwnerId;

    private static readonly int StartLocationTypeId = PlacementCommand.StartLocationRawcode.FromRawcode();

    /// <summary>
    /// Finds the arena's per-player hero array, a global <c>unit array</c> that the script indexes by
    /// <c>[GetPlayerId(...)]</c> (spell handlers test it to decide a cast belongs to that player's hero).
    /// Returns the array indexed that way most often, or null when the map has none.
    /// </summary>
    /// <summary>A per-player hero array and the index base the map uses for it.</summary>
    private sealed record HeroArray(string Name, bool OneBased)
    {
        /// <summary>The index expression for the unit in hand, matching the map's own convention.</summary>
        public string Index => OneBased
            ? "1 + GetPlayerId(GetOwningPlayer(u))"
            : "GetPlayerId(GetOwningPlayer(u))";
    }

    /// <summary>
    /// Every per-player hero array in the script, strongest first. A spell handler decides a cast is
    /// "this player's hero" by testing one of these, so a placed hero absent from them cannot cast.
    ///
    /// All of them are returned, not just the most-used one, because a map that received ports from
    /// several sources has several such arrays (one arena's <c>Hero[GetPlayerId(p)]</c> alongside
    /// another's <c>udg_Player[1 + GetPlayerId(p)]</c>), and registering only one leaves the other
    /// source's heroes mute. Both index conventions are recognised for the same reason, a 1-based
    /// array is invisible to a 0-based-only search.
    /// </summary>
    private static List<HeroArray> DetectHeroArrays(string jass)
    {
        var found = new List<(HeroArray Array, int Count)>();
        foreach (Match decl in Regex.Matches(jass, @"\bunit\s+array\s+([A-Za-z_][A-Za-z0-9_]*)"))
        {
            string name = decl.Groups[1].Value;
            string esc = Regex.Escape(name);
            int zeroBased = Regex.Matches(jass, esc + @"\s*\[\s*GetPlayerId\s*\(").Count;
            int oneBased = Regex.Matches(jass, esc + @"\s*\[\s*\(?\s*1\s*\+\s*GetPlayerId\s*\(").Count;
            if (zeroBased == 0 && oneBased == 0) continue;

            // Being indexed per player is not enough, plenty of arrays are. The test is whether the
            // array carries the player's IDENTITY, which is exactly what a handler asks when it
            // compares it against the casting unit, and exactly what a placed hero must satisfy to
            // cast. A hero-named array counts too, since an arena often passes it straight into its
            // spell functions rather than comparing it.
            //
            // Deliberately NOT enough: merely being assigned a freshly created unit. That admits
            // arrays holding spawned helpers (a revenge unit, a dummy caster), and writing a real
            // hero into one of those makes that system believe it owns our hero, so it may kill,
            // recycle, or refuse to spawn. Missing an obscure array only mutes one hero, whereas
            // impersonating a helper actively breaks the map, so this errs toward precision.
            bool comparedToCaster = Regex.IsMatch(jass,
                @"(GetTriggerUnit\(\)|GetSpellAbilityUnit\(\)|GetAttacker\(\))\s*==\s*" + esc + @"\s*\["
                + "|" + esc + @"\s*\[[^\]]*\]\s*==\s*(GetTriggerUnit\(\)|GetSpellAbilityUnit\(\)|GetAttacker\(\))");
            bool heroNamed = name.Contains("hero", StringComparison.OrdinalIgnoreCase);
            if (!comparedToCaster && !heroNamed) continue;

            // Belt and braces on the same risk: never impersonate a dummy unit slot.
            if (name.Contains("dummy", StringComparison.OrdinalIgnoreCase)) continue;

            found.Add((new HeroArray(name, oneBased > zeroBased), Math.Max(zeroBased, oneBased)));
        }
        // Capped, so a very large merged script cannot inflate the generated block without bound.
        return found.OrderByDescending(f => f.Count).ThenBy(f => f.Array.Name, StringComparer.Ordinal)
            .Take(4).Select(f => f.Array).ToList();
    }

    /// <summary>
    /// Global dispatch triggers (<c>gg_trg_*</c>) whose spell-effect event the arena registers per
    /// player (<c>TriggerRegisterPlayerUnitEvent(trg, somePlayer, EVENT_PLAYER_UNIT_SPELL_EFFECT, ...)</c>),
    /// normally during hero creation. A placed hero never triggers that registration, so its casts do
    /// not reach the dispatcher, CreateAllUnits registers the placed hero's player on each instead.
    /// Registering an unrelated dispatcher is harmless, its condition just filters the cast out.
    ///
    /// Internal, not private: DebugWiringCommand reuses this exact detection to find which trigger
    /// to instrument on an already-ported map, rather than keeping a second copy of the pattern.
    /// </summary>
    internal static List<string> DetectPerPlayerSpellTriggers(string jass)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in Regex.Matches(jass,
            @"TriggerRegisterPlayerUnitEvent\s*\(\s*(gg_trg_[A-Za-z0-9_]+)\s*,[^,]+,\s*EVENT_PLAYER_UNIT_SPELL_EFFECT"))
            set.Add(m.Groups[1].Value);
        return set.ToList();
    }

    /// <summary>
    /// Which of <paramref name="heroRawcodes"/> already have their OWN self-contained --synth-dispatch
    /// cast dispatcher in <paramref name="jass"/> (see <see cref="SynthDispatchBuilder"/>), an any-unit
    /// spell-effect trigger named <c>wc3ctl_SynthCast_&lt;rawcode&gt;</c> that fires for that hero
    /// unconditionally, on every player. Registering that hero's owner on the source's shared cast
    /// dispatcher too (the ordinary wiring below) would fire every one of its spells TWICE, once
    /// through each path, the exact double-damage and permanent-pause bug this got measured against.
    /// A plain port (no --synth-dispatch) never has one of these functions, so this always returns
    /// empty for it and its output is unaffected.
    /// </summary>
    private static HashSet<string> DetectSynthDispatchedHeroes(string jass, IReadOnlyCollection<string> heroRawcodes)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var rc in heroRawcodes)
        {
            string fn = "wc3ctl_SynthCast_" + SynthDispatchBuilder.SanitizeIdentifier(rc);
            if (Regex.IsMatch(jass, @"\bfunction\s+" + Regex.Escape(fn) + @"\b"))
                set.Add(rc);
        }
        return set;
    }

    /// <summary>
    /// Which of <paramref name="candidateTriggers"/> already have a LIVE spell-effect registration in
    /// <paramref name="jass"/>, reachable from the map's own entry points (<c>main</c>, <c>config</c>),
    /// independent of anything wc3ctl itself would generate. WC3 fires a trigger once per live
    /// registration on the same player and event, so a map whose own native script already registers
    /// a dispatcher this way, an arena's per-player pick-hero flow, a level-up gate like a real WOS2
    /// map's own <c>Trig_LvlUpCheck_Actions</c> (<c>TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck,
    /// GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)</c>, gated behind a
    /// once-per-player flag), does not need OUR registration too. An untouched map with a hero simply
    /// PLACED into it (no porting at all) hits exactly this, our own unconditional registration ran
    /// on top of the map's own lazy one, so every ability that player cast, including a hero we never
    /// touched, fired twice. <c>TriggerRegisterAnyUnitEventBJ</c> covers every player outright, a
    /// <c>TriggerRegisterPlayerUnitEvent</c> reachable through a per-unit-triggered path like a level
    /// up check practically does too (whichever player's unit trips that event), so either form found
    /// live for a trigger is enough to skip our own registration of it, for every owner, not only the
    /// specific player expression the map's own code happens to spell out.
    ///
    /// The reverse case, a hero ported into a BLANK map, must still get our registration. The source's
    /// own pick-hero flow is carried as TEXT (so it matches this same pattern) but nothing in a blank
    /// target ever calls it, so it is correctly absent from the reachable set and this returns empty
    /// for it, exactly the case this wiring exists for in the first place.
    ///
    /// A PLAIN port (no --synth-dispatch) complicates that, unlike synth-dispatch's own framework
    /// filter, an ordinary port hooks every carried InitTrig_* not already called by another carried
    /// aggregator straight into InitCustomTriggers (see ScriptPorter.HookInit), which includes a
    /// carried InitTrig_LvlUpCheck just as readily as any other, marking the call it inserts
    /// <c>// wc3ctl ported: ...</c>. Left alone, that makes a hero's OWN carried, merely-lazy,
    /// gated-behind-a-level-up per-player mechanism look "already reachable" in a freshly ported blank
    /// target too, exactly the case above that must still get our registration, since nothing else
    /// there promptly registers the dispatcher for an immediately castable placed hero. Every line
    /// carrying that marker is blanked out before computing reachability, so a wc3ctl-inserted call is
    /// never itself the reason something reads as already live, only a call the TARGET already had
    /// before wc3ctl touched it (the untouched-map case) counts.
    /// </summary>
    private static HashSet<string> DetectAlreadyLiveSpellTriggers(string jass, IReadOnlyCollection<string> candidateTriggers)
    {
        var live = new HashSet<string>(StringComparer.Ordinal);
        if (candidateTriggers.Count == 0) return live;

        var lines = jass.Replace("\r\n", "\n").Split('\n');
        var withoutWc3ctlWiring = lines
            .Select(l => l.Contains("// wc3ctl ported:", StringComparison.Ordinal) ? "" : l).ToArray();
        var code = JassComments.Strip(withoutWc3ctlWiring);
        var functions = JassFunctionIndex.Parse(jass);
        var allByName = new Dictionary<string, JassFunction>(StringComparer.Ordinal);
        foreach (var f in functions) allByName.TryAdd(f.Name, f);
        var bodies = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var f in allByName.Values)
        {
            int end = Math.Min(f.EndLine, code.Length);
            bodies[f.Name] = string.Join('\n', code[(f.StartLine - 1)..end]);
        }
        // Same forward call-graph closure RuntimeReadinessCommand and ScriptPorter's synth-dispatch
        // prune already use for "is this reachable from the map's real entry points", shared so all
        // three can never disagree about what "reachable" means.
        var reachable = ScriptPorter.ForwardClosure(new[] { "main", "config" }, bodies, allByName);

        foreach (var trg in candidateTriggers)
        {
            string esc = Regex.Escape(trg);
            var anyUnit = new Regex(@"TriggerRegisterAnyUnitEventBJ\s*\(\s*" + esc
                + @"\s*,\s*EVENT_PLAYER_UNIT_SPELL_EFFECT");
            var perPlayer = new Regex(@"TriggerRegisterPlayerUnitEvent\s*\(\s*" + esc
                + @"\s*,[^,]+,\s*EVENT_PLAYER_UNIT_SPELL_EFFECT");
            bool liveHere = reachable.Any(name =>
                bodies.TryGetValue(name, out var b) && (anyUnit.IsMatch(b) || perPlayer.IsMatch(b)));
            if (liveHere) live.Add(trg);
        }
        return live;
    }

    /// <summary>
    /// Every ability each of <paramref name="heroRawcodes"/> grants itself inside its own level up
    /// guard, see <see cref="SynthDispatchBuilder.ExtractHeroLevelUpGrants"/> for the shape recognised
    /// and why an unscoped scan for every UnitAddAbility in the script would wrongly hand a hero
    /// another hero's passive. Reads the TARGET's OWN script, whatever it currently is (an untouched
    /// map with nothing ported at all, a plain port, or a --synth-dispatch one), the same source every
    /// other detector in this file reads, so a placed hero gets its own kit's grants regardless of how
    /// it got onto the map. A hero with no such branch, or whose branch grants nothing in the
    /// recognised shape, is simply absent from the result, CreateAllUnits emits nothing extra for it.
    /// </summary>
    private static Dictionary<string, IReadOnlyList<SynthDispatchBuilder.AbilityGrant>> DetectLevelUpGrants(
        string jass, IReadOnlyCollection<string> heroRawcodes)
    {
        var map = new Dictionary<string, IReadOnlyList<SynthDispatchBuilder.AbilityGrant>>(StringComparer.Ordinal);
        foreach (var rc in heroRawcodes)
            if (SynthDispatchBuilder.ExtractHeroLevelUpGrants(jass, rc) is { } grants)
                map[rc] = grants;
        return map;
    }

    /// <summary>
    /// The map's own per-hero setup routine, if it has one. These scripts often carry a single
    /// function shaped <c>takes player p, unit u, integer heroCode</c> that dispatches on the code
    /// (<c>if heroCode == 'H000' then ... elseif ...</c>) and wires that hero up completely, binding
    /// its caster globals and registering its triggers with per-UNIT events
    /// (<c>TriggerRegisterUnitEvent(trg, udg_X_Caster, EVENT_UNIT_SPELL_EFFECT)</c>).
    ///
    /// This is what the arena runs when a player picks a hero, so calling it for a placed hero
    /// satisfies every prerequisite at once instead of us reverse-engineering them one at a time.
    /// The port carries the function but never its caller, so it sits dead until we call it.
    ///
    /// Only returned when the body actually dispatches on one of <paramref name="heroRawcodes"/>, so
    /// an unrelated three-parameter helper is never called by mistake.
    /// </summary>
    /// <summary>The map's hero setup routine and whether it returns a value, which decides whether
    /// it must be invoked with a set-assignment rather than call (JASS forbids call on a
    /// value-returning function, and the wrong form would fail the whole script).</summary>
    private sealed record HeroSetup(string Function, bool ReturnsValue);

    private static HeroSetup? DetectHeroSetup(string jass, IReadOnlyCollection<string> heroRawcodes)
    {
        if (heroRawcodes.Count == 0) return null;
        var lines = jass.Replace("\r\n", "\n").Split('\n');
        var signature = new Regex(
            @"^\s*function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes\s+player\s+[A-Za-z_][A-Za-z0-9_]*\s*,\s*"
            + @"unit\s+[A-Za-z_][A-Za-z0-9_]*\s*,\s*integer\s+([A-Za-z_][A-Za-z0-9_]*)\s*returns\s+([A-Za-z_][A-Za-z0-9_]*)");

        foreach (var f in JassFunctionIndex.Parse(jass))
        {
            var m = signature.Match(f.Signature);
            if (!m.Success) continue;

            var sb = new StringBuilder();
            for (int i = f.StartLine - 1; i < f.EndLine && i < lines.Length; i++)
                sb.Append(lines[i]).Append('\n');
            string body = sb.ToString();

            // It must register events (that is what makes a hero castable) and must recognise a hero
            // we actually placed, keyed off its own integer parameter.
            if (!body.Contains("TriggerRegisterUnitEvent", StringComparison.Ordinal)
                && !body.Contains("TriggerRegisterPlayerUnitEvent", StringComparison.Ordinal)) continue;

            string codeParam = Regex.Escape(m.Groups[2].Value);
            if (heroRawcodes.Any(rc => Regex.IsMatch(body, codeParam + @"\s*==\s*'" + Regex.Escape(rc) + "'")))
                return new HeroSetup(m.Groups[1].Value,
                    !string.Equals(m.Groups[3].Value, "nothing", StringComparison.Ordinal));
        }
        return null;
    }

    /// <summary>The map's per-player doer-dummy setup routine and what the generated call needs to know
    /// about it. <see cref="GuardArray"/> and <see cref="IndexExpr"/> (with <see cref="PlayerParam"/>
    /// substituted for the owner) form the null guard, <see cref="TakesCode"/> says whether the call
    /// passes the hero rawcode as a second argument.</summary>
    private sealed record DoerDummyAssigner(
        string Function, bool TakesCode, string GuardArray, string IndexExpr, string PlayerParam);

    /// <summary>
    /// The map's own routine that fills a per-player unit array from freshly created units, the general
    /// form of GGGA's WS_CreateWorkingSourceBagAndVendors which spawns the stun and damage-source dummies
    /// a hero's spells route through. A placed hero never runs the arena's pick flow, so this never runs
    /// and those arrays stay null, which is why a ported hero's stuns and routed damage silently do
    /// nothing even when every trigger is wired.
    ///
    /// It qualifies when it takes a player (optionally plus an integer) and returns nothing, spawns a
    /// unit (a CreateUnit-family call), and stores that unit into a `unit array` global at an index
    /// derived from the player parameter. Guarded, it must NOT branch on the integer parameter as a
    /// rawcode (that marks a per-hero dispatcher, not a generic per-player setup), and because the rule
    /// is inferred from one map we DECLINE (return null) when zero or more than one function qualifies,
    /// rather than guess. Detecting from the ported jass means an absent routine simply yields nothing to
    /// call, so the caller degrades safely.
    /// </summary>
    private static (DoerDummyAssigner? Assigner, int Candidates) DetectDoerDummyAssigner(string jass)
    {
        var lines = jass.Replace("\r\n", "\n").Split('\n');
        var unitArrays = UnitArrayGlobals(jass);
        if (unitArrays.Count == 0) return (null, 0);

        var signature = new Regex(
            @"^\s*function\s+([A-Za-z_][A-Za-z0-9_]*)\s+takes\s+player\s+([A-Za-z_][A-Za-z0-9_]*)\s*"
            + @"(?:,\s*integer\s+([A-Za-z_][A-Za-z0-9_]*)\s*)?returns\s+nothing\b");
        var createFamily = new Regex(
            @"\b(?:CreateUnit|CreateUnitAtLoc|CreateNUnitsAtLoc|CreateNUnitsAtLocFacingLocBJ)\s*\(");
        var storeUnit = new Regex(@"set\s+([A-Za-z_][A-Za-z0-9_]*)\s*\[([^\]]*)\]\s*=\s*bj_lastCreatedUnit");

        var found = new List<DoerDummyAssigner>();
        foreach (var f in JassFunctionIndex.Parse(jass))
        {
            var m = signature.Match(f.Signature);
            if (!m.Success) continue;
            string playerParam = m.Groups[2].Value;
            bool takesCode = m.Groups[3].Success;
            string codeParam = m.Groups[3].Value;

            var sb = new StringBuilder();
            for (int i = f.StartLine - 1; i < f.EndLine && i < lines.Length; i++)
                sb.Append(lines[i]).Append('\n');
            string body = sb.ToString();

            if (!createFamily.IsMatch(body)) continue;   // must spawn a unit
            // Guard, a routine that dispatches on the integer parameter as a rawcode is a per-hero
            // handler, not a generic per-player setup, so it is never treated as an assigner.
            if (takesCode && Regex.IsMatch(body,
                    @"(?:\b" + Regex.Escape(codeParam) + @"\s*==\s*')|(?:'[^']{4}'\s*==\s*"
                    + Regex.Escape(codeParam) + @"\b)"))
                continue;

            foreach (Match a in storeUnit.Matches(body))
            {
                string arr = a.Groups[1].Value;
                string idx = a.Groups[2].Value;
                if (!unitArrays.Contains(arr)) continue;

                // The index must derive from the player parameter, else this is not a per-player
                // routine. It usually does so INDIRECTLY, through a local computed once and reused,
                // "set slot = 1 + GetPlayerId(p)" then "set udg_X[slot] = ...". Requiring the index
                // to name the parameter itself matched only the direct form, so on every real map
                // this found nothing and the caller reported that no routine existed. Resolve a
                // single-identifier index back through its own assignment before giving up.
                string? resolved = ResolveIndexToPlayer(idx, body, playerParam);
                if (resolved is null) continue;
                found.Add(new DoerDummyAssigner(f.Name, takesCode, arr, resolved, playerParam));
                break;
            }
        }
        return found.Count == 1 ? (found[0], 1) : (null, found.Count);
    }

    /// <summary>
    /// The index expression rewritten so it derives visibly from <paramref name="playerParam"/>, or null
    /// when it does not derive from the player at all. An index that already names the parameter is
    /// returned as-is. A bare identifier is looked up as a local assigned once from the parameter
    /// ("set slot = 1 + GetPlayerId(p)"), and that initializer is returned in its place, so the caller
    /// can rebuild the same index for a different player by substituting the parameter.
    /// </summary>
    private static string? ResolveIndexToPlayer(string idx, string body, string playerParam)
    {
        string trimmed = idx.Trim();
        var namesPlayer = new Regex(@"\b" + Regex.Escape(playerParam) + @"\b");
        if (namesPlayer.IsMatch(trimmed)) return trimmed;

        // Only a bare local can be resolved. Anything more complex that still fails to name the
        // player is not a per-player index, and guessing at it would wire the wrong slot.
        if (!Regex.IsMatch(trimmed, @"^[A-Za-z_][A-Za-z0-9_]*$")) return null;

        // Every assignment to the local, not just the first. These routines declare the slot with a
        // placeholder and compute it later ("local integer slot= 0" then "set slot=1 + GetPlayerId(p)"),
        // so taking the first match reads the placeholder and concludes the index is not player-derived.
        foreach (Match assign in Regex.Matches(body,
            @"(?m)^\s*(?:local\s+integer\s+|set\s+)" + Regex.Escape(trimmed) + @"\s*=\s*([^\r\n]+)"))
        {
            string init = assign.Groups[1].Value.Trim();
            if (namesPlayer.IsMatch(init)) return init;
        }
        return null;
    }

    /// <summary>Names declared as a <c>unit array</c> global in the script.</summary>
    private static HashSet<string> UnitArrayGlobals(string jass)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in Regex.Matches(jass, @"^\s*unit\s+array\s+([A-Za-z_][A-Za-z0-9_]*)",
                     RegexOptions.Multiline))
            set.Add(m.Groups[1].Value);
        return set;
    }

    /// <summary>Builds the marker-wrapped block of creation functions. When <paramref name="heroArray"/>
    /// is the arena's per-player hero array, each placed hero registers itself into it (first hero per
    /// player wins) so the ported spell handlers recognise it.</summary>
    private static string BuildBlock(List<UnitData> units, List<UnitData> items, string nl,
        IReadOnlyList<HeroArray>? heroArraysOrNull = null, IReadOnlyList<string>? spellTriggersOrNull = null,
        HeroSetup? heroSetup = null, IReadOnlyList<(string Rawcode, int OwnerId)>? placedHeroes = null,
        DoerDummyAssigner? doerDummy = null, IReadOnlySet<string>? synthDispatchedHeroesOrNull = null,
        IReadOnlySet<string>? alreadyLiveSpellTriggersOrNull = null,
        IReadOnlyDictionary<string, IReadOnlyList<SynthDispatchBuilder.AbilityGrant>>? levelUpGrantsOrNull = null)
    {
        var heroArrays = heroArraysOrNull ?? Array.Empty<HeroArray>();
        var heroes = placedHeroes ?? Array.Empty<(string Rawcode, int OwnerId)>();
        var spellTriggers = spellTriggersOrNull ?? Array.Empty<string>();
        var alreadyLiveSpellTriggers = alreadyLiveSpellTriggersOrNull ?? new HashSet<string>(StringComparer.Ordinal);
        var synthDispatchedHeroes = synthDispatchedHeroesOrNull ?? new HashSet<string>(StringComparer.Ordinal);
        var levelUpGrants = levelUpGrantsOrNull
            ?? new Dictionary<string, IReadOnlyList<SynthDispatchBuilder.AbilityGrant>>(StringComparer.Ordinal);
        // Players that own a placed unit (a placed hero's owner is among them, neutral slots excluded).
        // The arena's spell-dispatch triggers get their spell-effect event registered for these players
        // so a placed hero's casts reach the handlers. This is deferred, not done in CreateAllUnits,
        // because CreateAllUnits runs BEFORE InitCustomTriggers in main(), so the gg_trg_* dispatch
        // triggers do not exist yet at unit-creation time (registering then would silently hit null).
        var heroOwners = units.Select(u => u.OwnerId)
            .Where(o => o >= 0 && o < PlayerColors.NeutralHostileId)
            .Distinct().OrderBy(o => o).ToList();
        // Owners whose EVERY placed hero already has its own --synth-dispatch dispatcher wired (see
        // DetectSynthDispatchedHeroes) are excluded from the shared-dispatcher registration below, or
        // that hero's spells would run twice, once through each path. An arena places one hero per
        // player, so this is the whole story in practice; a player who somehow owns a mix (one hero
        // ported plain, another with --synth-dispatch) still needs the shared dispatcher for the
        // plain one, so it stays registered there, the safer of the two imperfect choices.
        var fullySynthDispatchedOwners = heroes.Select(h => h.OwnerId).Distinct()
            .Where(o => heroes.Where(h => h.OwnerId == o).All(h => synthDispatchedHeroes.Contains(h.Rawcode)))
            .ToHashSet();
        bool wireSpells = (spellTriggers.Count > 0 || (heroSetup is not null && heroes.Count > 0))
            && heroOwners.Count > 0;

        var sb = new StringBuilder();
        sb.Append(BeginMarker).Append(nl);

        if (units.Count > 0)
        {
            if (wireSpells)
            {
                // Fired by a 0-second timer from CreateAllUnits, so it runs once map init has finished
                // and every gg_trg_* dispatch trigger has been created by InitCustomTriggers.
                sb.Append("function ").Append(WireSpellsFunc).Append(" takes nothing returns nothing").Append(nl);
                if (heroSetup is not null && heroes.Count > 0)
                {
                    sb.Append("    local group g = CreateGroup()").Append(nl);
                    sb.Append("    local unit hu").Append(nl);
                    if (heroSetup.ReturnsValue) sb.Append("    local boolean ok").Append(nl);
                }
                // A trigger the map's own script already reachably registers for this event (see
                // DetectAlreadyLiveSpellTriggers) is skipped outright, for every owner, not only the
                // specific player its own code names, registering it again doubles every ability that
                // player, including a hero we never touched, casts through it.
                foreach (var trg in spellTriggers.Where(t => !alreadyLiveSpellTriggers.Contains(t)))
                    foreach (var owner in heroOwners)
                    {
                        if (fullySynthDispatchedOwners.Contains(owner)) continue;
                        sb.Append("    call TriggerRegisterPlayerUnitEvent(").Append(trg)
                          .Append(", Player(").Append(owner).Append("), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)").Append(nl);
                    }
                // Run the map own per-hero setup for each placed hero. That routine binds the
                // hero caster globals and registers its per-unit spell events, which is how the
                // arena makes a picked hero castable, so this replaces guessing prerequisite by
                // prerequisite. Units are enumerated because a timer callback takes no arguments.
                if (heroSetup is not null && heroes.Count > 0)
                {
                    foreach (var owner in heroes.Select(h => h.OwnerId).Distinct().OrderBy(o => o))
                    {
                        sb.Append("    call GroupEnumUnitsOfPlayer(g, Player(").Append(owner).Append("), null)").Append(nl);
                        sb.Append("    loop").Append(nl);
                        sb.Append("        set hu = FirstOfGroup(g)").Append(nl);
                        sb.Append("        exitwhen hu == null").Append(nl);
                        sb.Append("        call GroupRemoveUnit(g, hu)").Append(nl);
                        foreach (var rc in heroes.Where(h => h.OwnerId == owner).Select(h => h.Rawcode).Distinct())
                        {
                            sb.Append("        if GetUnitTypeId(hu) == '").Append(rc).Append("' then").Append(nl);
                            sb.Append("            ")
                              .Append(heroSetup.ReturnsValue ? "set ok = " : "call ")
                              .Append(heroSetup.Function).Append("(Player(").Append(owner)
                              .Append("), hu, '").Append(rc).Append("')").Append(nl);
                            // The per-player doer-dummy setup, if the map has one. A placed hero never
                            // runs the arena's pick flow, so the per-player unit array this routine fills
                            // (the stun and damage-source dummies its spells route through) stays null and
                            // those spells do nothing. Call it once, guarded on the array still being null
                            // so a second placed hero for this player cannot spawn a duplicate set.
                            if (doerDummy is not null)
                            {
                                string idx = Regex.Replace(doerDummy.IndexExpr,
                                    @"\b" + Regex.Escape(doerDummy.PlayerParam) + @"\b", $"Player({owner})");
                                sb.Append("            if ").Append(doerDummy.GuardArray).Append('[').Append(idx)
                                  .Append("] == null then").Append(nl);
                                sb.Append("                call ").Append(doerDummy.Function)
                                  .Append("(Player(").Append(owner).Append(')');
                                if (doerDummy.TakesCode) sb.Append(", '").Append(rc).Append('\'');
                                sb.Append(')').Append(nl);
                                sb.Append("            endif").Append(nl);
                            }
                            sb.Append("        endif").Append(nl);
                        }
                        sb.Append("    endloop").Append(nl);
                    }
                    sb.Append("    call DestroyGroup(g)").Append(nl);
                    sb.Append("    set g = null").Append(nl);
                    sb.Append("    set hu = null").Append(nl);
                }
                sb.Append("    call DestroyTimer(GetExpiredTimer())").Append(nl);
                sb.Append("endfunction").Append(nl);
            }

            sb.Append("function ").Append(UnitsFunc).Append(" takes nothing returns nothing").Append(nl);
            sb.Append("    local unit u").Append(nl);
            foreach (var u in units)
            {
                float faceDeg = u.Rotation * 180f / MathF.PI;
                sb.Append("    set u = CreateUnit(Player(").Append(u.OwnerId).Append("), '")
                  .Append(Rawcode(u.TypeId)).Append("', ")
                  .Append(Real(u.Position.X)).Append(", ").Append(Real(u.Position.Y)).Append(", ")
                  .Append(Real(faceDeg)).Append(')').Append(nl);
                // Editor-set overrides, applied so a placed unit arrives exactly as the panel showed it
                // (a scripted map spawns from here, not from war3mapUnits.doo, so these must be re-applied).
                if (u.Scale.X is > 0f and not 1f)
                    sb.Append("    call SetUnitScale(u, ").Append(Real(u.Scale.X)).Append(", ")
                      .Append(Real(u.Scale.Y)).Append(", ").Append(Real(u.Scale.Z)).Append(')').Append(nl);
                if (u.HeroLevel > 1)
                    sb.Append("    call SetHeroLevel(u, ").Append(u.HeroLevel).Append(", false)").Append(nl);
                if (u.HeroStrength > 0)
                    sb.Append("    call SetHeroStr(u, ").Append(u.HeroStrength).Append(", true)").Append(nl);
                if (u.HeroAgility > 0)
                    sb.Append("    call SetHeroAgi(u, ").Append(u.HeroAgility).Append(", true)").Append(nl);
                if (u.HeroIntelligence > 0)
                    sb.Append("    call SetHeroInt(u, ").Append(u.HeroIntelligence).Append(", true)").Append(nl);
                if (u.GoldAmount > 0)
                    sb.Append("    call SetResourceAmount(u, ").Append(u.GoldAmount).Append(')').Append(nl);
                if (u.TargetAcquisition >= 0f)
                    sb.Append("    call SetUnitAcquireRange(u, ").Append(Real(u.TargetAcquisition)).Append(')').Append(nl);
                // Current life / mana as a percent of the now-correct (leveled) maximum.
                if (u.HP is >= 0 and <= 100)
                    sb.Append("    call SetUnitState(u, UNIT_STATE_LIFE, GetUnitState(u, UNIT_STATE_MAX_LIFE) * ")
                      .Append(Real(u.HP / 100f)).Append(')').Append(nl);
                if (u.MP is >= 0 and <= 100)
                    sb.Append("    call SetUnitState(u, UNIT_STATE_MANA, GetUnitState(u, UNIT_STATE_MAX_MANA) * ")
                      .Append(Real(u.MP / 100f)).Append(')').Append(nl);
                // Abilities this hero's kit only ever grants through a level up event (see Sync and
                // DetectLevelUpGrants). A preplaced hero is created and levelled in one shot, so that
                // event never fires for it, grant them here instead. The source's own idempotent guard
                // is kept verbatim (safe even if the hero also genuinely levels up later), only the
                // local naming the hero unit is retargeted from the source's onto ours.
                if (levelUpGrants.TryGetValue(Rawcode(u.TypeId), out var grants))
                    foreach (var g in grants)
                    {
                        sb.Append("    if ").Append(SynthDispatchBuilder.ReplaceIdentifier(g.Condition, g.UnitParam, "u"))
                          .Append(" then").Append(nl);
                        foreach (var call in g.Calls)
                            sb.Append("        ").Append(SynthDispatchBuilder.ReplaceIdentifier(call, g.UnitParam, "u")).Append(nl);
                        sb.Append("    endif").Append(nl);
                    }
                // Best-effort arena integration for a placed hero (see Sync): register it into the
                // per-player hero array so handlers that test Hero[pid] recognise it. Safe here because
                // it is a plain global-array write (the array exists from map load), unlike the spell
                // trigger registration, which must wait for the triggers and is done via the timer below.
                foreach (var arr in heroArrays)
                    sb.Append("    if IsUnitType(u, UNIT_TYPE_HERO) and ").Append(arr.Name)
                      .Append('[').Append(arr.Index).Append("] == null then").Append(nl)
                      .Append("        set ").Append(arr.Name)
                      .Append('[').Append(arr.Index).Append("] = u").Append(nl)
                      .Append("    endif").Append(nl);
            }
            if (wireSpells)
                sb.Append("    call TimerStart(CreateTimer(), 0., false, function ")
                  .Append(WireSpellsFunc).Append(')').Append(nl);
            sb.Append("    set u = null").Append(nl);
            sb.Append("endfunction").Append(nl);
        }

        if (items.Count > 0)
        {
            sb.Append("function ").Append(ItemsFunc).Append(" takes nothing returns nothing").Append(nl);
            foreach (var it in items)
                sb.Append("    call CreateItem('").Append(Rawcode(it.TypeId)).Append("', ")
                  .Append(Real(it.Position.X)).Append(", ").Append(Real(it.Position.Y)).Append(')').Append(nl);
            sb.Append("endfunction").Append(nl);
        }

        sb.Append(EndMarker);
        return sb.ToString();
    }

    /// <summary>Removes a previously generated block (markers included) plus the one trailing
    /// newline we inserted with it, leaving unrelated script untouched.</summary>
    private static string RemoveBlock(string jass, string nl)
    {
        // Prefix match, so a block written before the version stamp existed is still replaced.
        int start = jass.IndexOf(BeginMarkerPrefix, StringComparison.Ordinal);
        if (start < 0) return jass;
        int end = jass.IndexOf(EndMarker, start, StringComparison.Ordinal);
        if (end < 0) return jass;
        end += EndMarker.Length;
        if (jass.AsSpan(end).StartsWith(nl)) end += nl.Length;
        return jass.Remove(start, end - start);
    }

    /// <summary>Inserts <paramref name="text"/> immediately before the first line that begins
    /// with <paramref name="anchor"/>. Falls back to appending if the anchor is absent.</summary>
    private static string InsertBefore(string jass, string anchor, string text, string nl)
    {
        int at = jass.IndexOf(anchor, StringComparison.Ordinal);
        if (at < 0) return jass.EndsWith(nl, StringComparison.Ordinal) ? jass + text : jass + nl + text;
        // Back up to the start of the anchor's line so we do not split it.
        int lineStart = jass.LastIndexOf('\n', at) + 1;
        return jass.Insert(lineStart, text);
    }

    /// <summary>Adds or removes a <c>call Func(  )</c> statement inside main so the calls always
    /// match the functions the block defines. Inserts right before InitCustomTriggers (World
    /// Editor order, widgets before triggers), falling back to after InitBlizzard, then to the
    /// end of main.</summary>
    private static string EnsureMainCall(string jass, string func, bool wanted, string nl)
    {
        int mainAt = jass.IndexOf("function main takes nothing returns nothing", StringComparison.Ordinal);
        if (mainAt < 0) return jass;
        int mainEnd = jass.IndexOf(nl + "endfunction", mainAt, StringComparison.Ordinal);
        if (mainEnd < 0) mainEnd = jass.Length;

        string callToken = "call " + func + "(";
        int existing = jass.IndexOf(callToken, mainAt, StringComparison.Ordinal);
        bool present = existing >= 0 && existing < mainEnd;

        if (wanted && !present)
        {
            int anchor = jass.IndexOf("call InitCustomTriggers", mainAt, StringComparison.Ordinal);
            if (anchor < 0 || anchor > mainEnd)
            {
                anchor = jass.IndexOf("call InitBlizzard", mainAt, StringComparison.Ordinal);
                // After InitBlizzard's line rather than before it.
                if (anchor >= 0 && anchor < mainEnd)
                    anchor = jass.IndexOf('\n', anchor) + 1;
            }
            if (anchor < 0 || anchor > mainEnd) anchor = mainEnd; // last resort: end of main
            int lineStart = jass.LastIndexOf('\n', Math.Min(anchor, jass.Length - 1)) + 1;
            string indent = LeadingWhitespace(jass, lineStart);
            string stmt = indent + "call " + func + "(  )" + nl;
            return jass.Insert(lineStart, stmt);
        }
        if (!wanted && present)
        {
            int lineStart = jass.LastIndexOf('\n', existing) + 1;
            int lineEnd = jass.IndexOf('\n', existing);
            if (lineEnd < 0) lineEnd = jass.Length; else lineEnd += 1;
            return jass.Remove(lineStart, lineEnd - lineStart);
        }
        return jass;
    }

    /// <summary>True if the script defines <paramref name="func"/> outside any block of ours.</summary>
    private static bool MentionsForeignFunction(string jass, string func) =>
        jass.Contains("function " + func + " ", StringComparison.Ordinal) ||
        jass.Contains("function " + func + "\t", StringComparison.Ordinal);

    private static string LeadingWhitespace(string s, int lineStart)
    {
        int i = lineStart;
        while (i < s.Length && (s[i] == ' ' || s[i] == '\t')) i++;
        return s.Substring(lineStart, i - lineStart);
    }

    private static string Rawcode(int id) => id.ToRawcode();

    /// <summary>A JASS real literal, invariant, always with a decimal point (JASS does not
    /// promote an integer literal to a real argument).</summary>
    private static string Real(float v)
    {
        string s = v.ToString("0.0###", CultureInfo.InvariantCulture);
        return s;
    }
}
