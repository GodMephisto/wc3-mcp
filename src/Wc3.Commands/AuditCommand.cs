// src/Wc3.Commands/AuditCommand.cs
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One thing the audit found wrong, with the object and the hero it belongs to.</summary>
public sealed record AuditIssue(
    DiagnosticSeverity Severity,
    string Check,
    string Rawcode,
    string? Owner,
    string Message);

public sealed record AuditResult(
    bool Ok,
    int ObjectsChecked,
    IReadOnlyList<AuditIssue> Issues,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Behavioural audit of a map's object data, read-only.
///
/// <see cref="ValidateCommand"/> answers whether a map can load. This answers whether its
/// abilities do what they claim, which is a different and much larger question, and one that
/// cannot be settled by playing a map with a hundred heroes.
///
/// The leverage is that a hand-authored map documents itself. Tooltips state the cooldown, the
/// mana cost, the cast range and the area per level, in prose the author wrote, so the object
/// data can be checked against the author's own specification. On one real map that comparison
/// found twelve wrong cooldowns, and then found a thirteenth that a repair had introduced.
///
/// What it CANNOT see, and no map-only check can. Whether a trigger's damage formula is right,
/// whether a status effect lands, or anything about timing. A clean result means no claim in
/// the map contradicts its data. It does not mean the map is correct.
/// </summary>
public static class AuditCommand
{
    /// <summary>Check names, usable with the CLI's filter so a run can be narrowed.</summary>
    public static readonly string[] AllChecks =
    {
        "level-gap", "level-tooltip", "tooltip-claim", "orphan-ability",
        "requirement", "portrait-risk", "dangling-reference", "missing-model",
    };

    // Rawcode-list fields that name another object, and what that object has to be. A stale
    // id here fails silently, which is the whole reason this check exists. An ability whose
    // abuf names a buff the game dropped applies no buff and reports nothing, so the spell
    // casts, costs mana, and does not land its status effect.
    private static readonly (ObjectKind Kind, string Code, string Names)[] References =
    {
        (ObjectKind.Ability, "abuf", "buff"),
        (ObjectKind.Ability, "aeff", "buff"),
        (ObjectKind.Unit, "uhab", "ability"),
        (ObjectKind.Unit, "uabi", "ability"),
        (ObjectKind.Item, "iabi", "ability"),
    };

    // Tooltip claims. Each is a field code, a human label, and the phrase that states it.
    // Colour tags are stripped first so one pattern covers the coloured and plain spellings.
    private static readonly (string Code, string Label, Regex Pattern)[] Claims =
    {
        ("acdn", "cooldown", new Regex(@"Cooldown\s*:?\s*(\d+(?:\.\d+)?)", RegexOptions.IgnoreCase)),
        ("amcs", "mana cost", new Regex(@"Mana\s*cost\s*:?\s*(\d+(?:\.\d+)?)", RegexOptions.IgnoreCase)),
        ("aran", "cast range", new Regex(@"Cast\s*range\s*:?\s*(\d+(?:\.\d+)?)", RegexOptions.IgnoreCase)),
        // "Area of Effect" only. "Area of Damage" is NOT a radius, and reading it as one
        // produced ten false claims on one real map. Two independent proofs. On A0DK the
        // base-specific field carrying exactly the tooltip's 500,600,700,800,900 is named
        // "AOE Damage" while the real aare is a flat 340, and on A0M8 the prose beside the
        // line reads "damage to all enemies within 300 AoE" while the line itself states 200.
        // The author means damage dealt inside the area, so the number was never a radius.
        ("aare", "area of effect",
            new Regex(@"Area of Effect\s*:?\s*(\d+(?:\.\d+)?)", RegexOptions.IgnoreCase)),
    };

    private static readonly Regex ColourTag = new(@"\|c[0-9a-fA-F]{8}|\|r|\|n", RegexOptions.IgnoreCase);
    private static readonly Regex RawcodeLiteral = new(@"'([A-Za-z0-9]{4})'");
    private static readonly Regex DecimalLiteral = new(@"\b(\d{9,10})\b");

    // A per-level Data field, whose column is base specific (Htb1, Efk1, nca1 and friends).
    // Used only to decide whether an ability does anything at all on its own.
    private static readonly Regex DataField = new(@"^[A-Z][a-z]{2}\d$");

    // The engine fields a missing level actually breaks, which is the defect a player sees as
    // "the level 5 spell has range 0, damage 0 and no cooldown".
    //
    // Data fields are deliberately NOT here, and that is the whole point of the list. A missing
    // Data level is how this author writes "off". Measured on one real map, 26 of the 35
    // abilities setting Osh2 start their series at level 2 with a flat 99999 sentinel, and Osh2
    // is written as an explicit zero exactly once in the entire map, so absence is the idiom
    // rather than an omission. Reporting those turned 333 low value warnings into 33 mostly
    // false errors, which is a worse trade than the noise it was meant to fix.
    private static readonly HashSet<string> EngineLevelFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "aran",   // cast range
        "acdn",   // cooldown
        "amcs",   // mana cost
        "aare",   // area of effect
        "adur",   // duration
        "ahdu",   // hero duration
        "acas",   // casting time
        "adct",   // cast point
    };

    public static AuditResult Execute(
        MapDocument doc, string? gameDirOverride = null, IReadOnlyCollection<string>? only = null)
    {
        var issues = new List<AuditIssue>();
        var diagnostics = new List<string>();
        bool Want(string check) => only is null || only.Count == 0 || only.Contains(check);

        var strings = MapStrings.From(doc);
        var abilities = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability))
            .ToDictionary(e => e.Id);
        var units = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit)).ToList();
        var items = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Item)).ToList();

        // Which hero or item grants which ability, and in which slot, so an issue names
        // something a player can go and look at rather than a rawcode.
        //
        // Items are included deliberately. On one real map 327 custom items grant 156 of the
        // map's own abilities, and attributing only unit-granted abilities left every one of
        // those with no owner, which silently excluded them from the orphan check.
        var owner = new Dictionary<int, string>();
        // The rawcode of the unit or item that grants each ability, kept beside the display
        // label because the orphan check needs the ID rather than the name.
        var ownerCode = new Dictionary<int, string>();
        void Claim(string? code, string label, string ownerRawcode)
        {
            if (code is null || code.Length != 4) return;
            int key = code.FromRawcode();
            if (!owner.ContainsKey(key)) owner[key] = label;
            if (!ownerCode.ContainsKey(key)) ownerCode[key] = ownerRawcode;
        }
        foreach (var u in units)
        {
            string name = strings.Resolve(ModValue(u, "unam")) is { Length: > 0 } n
                && !n.StartsWith("TRIGSTR_", StringComparison.Ordinal) ? n : u.Id.ToRawcode();
            int slot = 0;
            foreach (var code in Split(ModValue(u, "uhab")))
                Claim(code, $"{name} [{Slot(slot++)}]", u.Id.ToRawcode());
            foreach (var code in Split(ModValue(u, "uabi")))
                Claim(code, name, u.Id.ToRawcode());
        }
        foreach (var i in items)
        {
            string name = strings.Resolve(ModValue(i, "unam") ?? ModValue(i, "inam")) is { Length: > 0 } n
                && !n.StartsWith("TRIGSTR_", StringComparison.Ordinal) ? n : i.Id.ToRawcode();
            foreach (var code in Split(ModValue(i, "iabi")))
                Claim(code, $"item {name}", i.Id.ToRawcode());
        }

        string script = ScriptText(doc);
        var scriptCodes = new HashSet<string>(
            RawcodeLiteral.Matches(script).Select(m => m.Groups[1].Value), StringComparer.Ordinal);
        var scriptInts = new HashSet<string>(
            DecimalLiteral.Matches(script).Select(m => m.Groups[0].Value), StringComparer.Ordinal);

        // Does the trigger script name this object at all, as a 'ABCD' literal or as either
        // byte order of its integer. Shared by orphan-ability and tooltip-claim, because both
        // ask the same question. A script that names an ability can change any of its values
        // at runtime, so no static read of the object data can be called authoritative.
        bool NamedInScript(string rawcode, int id) =>
            scriptCodes.Contains(rawcode)
            || scriptInts.Contains(BigEndian(rawcode).ToString(CultureInfo.InvariantCulture))
            || scriptInts.Contains(id.ToString(CultureInfo.InvariantCulture));

        // Tooltip claims the data contradicts on an ability the script drives, and tooltips
        // authored above the level a player can reach. Both are reported once at the end as
        // notes rather than per ability as warnings, for reasons recorded at each site.
        var triggerOwnedClaims = new List<string>();
        var deadTips = new List<string>();

        // Lazily merged base-and-map field views, keyed by rawcode. Only abilities that actually
        // show a gap pay for one, because the merge opens CASC and is far dearer than the scan.
        // A null dictionary means the game data is unavailable, which the gap check must report
        // rather than silently treat as "the base has nothing".
        var baseFields = new Dictionary<string, IReadOnlyDictionary<string, string>?>(
            StringComparer.OrdinalIgnoreCase);

        // Of the levels this field is missing, the ones the BASE ability does not supply either.
        // Returns null when base data could not be read at all, which is a different answer from
        // an empty list and must not be collapsed into one.
        List<int>? BaseGaps(string rawcode, string code, IEnumerable<int> missing)
        {
            if (!baseFields.TryGetValue(rawcode, out var fields))
            {
                try
                {
                    var merged = ObjectGetCommand.Execute(
                        doc, ObjectKind.Ability, rawcode, gameDirOverride);
                    fields = merged.Fields.Any(f => f.Source == "base")
                        ? merged.Fields.Where(f => f.Source == "base")
                            .GroupBy(f => f.Code, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(g => g.Key, g => g.First().Value,
                                StringComparer.OrdinalIgnoreCase)
                        : null;
                }
                catch (Exception) { fields = null; }
                baseFields[rawcode] = fields;
            }
            if (fields is null) return null;

            var gaps = new List<int>();
            foreach (int lv in missing)
            {
                // ONLY the per-level key counts. The merged view also emits a bare 'code' for a
                // levelled field, and that is a convenience duplicate of level 1 rather than a
                // fallback for every level. Accepting it made every level resolve, so the check
                // reported 0 on a map carrying 129 real engine field gaps, which is a clean
                // result from a check that could not fire.
                if (!fields.TryGetValue($"{code}:{lv}", out var v) || v.Length == 0)
                    gaps.Add(lv);
            }
            return gaps;
        }

        foreach (var (id, entry) in abilities.OrderBy(kv => kv.Key))
        {
            string rawcode = id.ToRawcode();
            owner.TryGetValue(id, out var who);
            var byCode = Levelled(entry);
            int declared = Declared(byCode);

            if (Want("level-gap"))
                foreach (var (code, levels) in byCode)
                {
                    if (levels.Keys.All(l => l == 0)) continue;
                    if (!EngineLevelFields.Contains(code)) continue;
                    var missing = Enumerable.Range(1, declared)
                        .Where(l => !levels.ContainsKey(l)).ToList();
                    if (missing.Count == 0 || missing.Count >= declared) continue;

                    // A gap is only a defect when the fallback lands on nothing. An ability that
                    // sets a field at levels 1 to 3 and lets 4 to 6 inherit is ORDINARY authoring
                    // when the base ability has values there, and reporting it flagged 333 fields
                    // on one real map where 26 were real. A check whose noise is 12 to 1 teaches
                    // the reader to skip it, which costs more than the check is worth.
                    //
                    // The real defect is the map declaring MORE levels than its base carries, so
                    // level 5 of a 4 level base resolves to nothing. That is the shape behind
                    // "the level 5 spell has range 0, damage 0 and no cooldown".
                    var unresolved = BaseGaps(rawcode, code, missing);
                    if (unresolved is null)                        // no game data, cannot judge
                        issues.Add(new AuditIssue(DiagnosticSeverity.Warning, "level-gap", rawcode, who,
                            $"field {code} has values at levels "
                            + $"[{string.Join(",", levels.Keys.Where(l => l >= 1).OrderBy(l => l))}] "
                            + $"but the ability declares {declared}, so levels "
                            + $"[{string.Join(",", missing)}] fall back to the base ability, "
                            + "which could not be read"));
                    // A hole is only a CLIFF when a level below it holds a non-zero value. An
                    // ability that sets mana cost 0 at level 1 and nothing after is free at
                    // every level, which is consistent and intended, and A001 on one real map
                    // is exactly that. Filling it would invent a cost the author never wanted.
                    // Requiring a non-zero level below the hole cut 112 candidate repairs to
                    // the ones a player would actually report.
                    else if (unresolved.Count > 0
                             && unresolved.Any(lv => levels.Any(kv => kv.Key >= 1 && kv.Key < lv
                                 && double.TryParse(kv.Value, NumberStyles.Float,
                                        CultureInfo.InvariantCulture, out var below)
                                 && below != 0)))
                        issues.Add(new AuditIssue(DiagnosticSeverity.Warning, "level-gap", rawcode, who,
                            $"field {code} has values at levels "
                            + $"[{string.Join(",", levels.Keys.Where(l => l >= 1).OrderBy(l => l))}] "
                            + $"and the ability declares {declared}, but level(s) "
                            + $"[{string.Join(",", unresolved)}] resolve to nothing, because the "
                            + "base ability has no value there either"));
                }

            if (Want("level-tooltip"))
            {
                var tipped = new HashSet<int>();
                foreach (var f in new[] { "atp1", "aub1" })
                    if (byCode.TryGetValue(f, out var lv))
                        foreach (var kv in lv)
                            if (kv.Key >= 1 && !string.IsNullOrWhiteSpace(kv.Value)) tipped.Add(kv.Key);

                if (tipped.Count > 0)
                {
                    // The defect a PLAYER can see. Some reachable level was given its own
                    // tooltip and another was not, so that level falls through to the base
                    // ability's text and the command card reads "Inner Fire" on a custom spell.
                    var blank = Enumerable.Range(1, declared).Where(l => !tipped.Contains(l)).ToList();
                    if (blank.Count > 0 && blank.Count < declared)
                        issues.Add(new AuditIssue(DiagnosticSeverity.Warning, "level-tooltip", rawcode, who,
                            $"declares {declared} level(s) and authors a tooltip for "
                            + $"[{string.Join(",", tipped.Where(l => l <= declared).OrderBy(l => l))}], "
                            + $"so level(s) [{string.Join(",", blank)}] fall back to the base "
                            + "ability's own text"));

                    // Text ABOVE the declared count is unreachable, so it is untidy rather than
                    // broken. This was a warning until it was measured. On one real map it fired
                    // 38 times, and every one of the 38 was an ability whose reachable levels all
                    // carried their own tooltip, so no player could observe any of it. The script
                    // was checked too, and its 230 level-setting calls never ask for a level above
                    // the declared count, only read the current one. A check that cries wolf 38
                    // times against 1 real case teaches its reader to skip the one that matters.
                    if (tipped.Any(l => l > declared))
                        deadTips.Add($"{rawcode} declares {declared} with text to {tipped.Max()}");
                }
            }

            if (Want("tooltip-claim"))
                for (int lv = 1; lv <= declared; lv++)
                {
                    string tip = Tooltip(byCode, lv, strings);
                    if (tip.Length == 0) continue;
                    foreach (var (code, label, pattern) in Claims)
                    {
                        var m = pattern.Match(tip);
                        if (!m.Success) continue;
                        if (!double.TryParse(m.Groups[1].Value, NumberStyles.Float,
                                CultureInfo.InvariantCulture, out var promised)) continue;
                        if (!byCode.TryGetValue(code, out var levels) ||
                            !levels.TryGetValue(lv, out var raw)) continue;   // inherits, not a defect
                        if (!double.TryParse(raw, NumberStyles.Float,
                                CultureInfo.InvariantCulture, out var actual)) continue;
                        if (Math.Abs(actual - promised) <= Math.Max(0.01, promised * 0.001)) continue;

                        // A script that names the ability can set any of these at runtime, so
                        // the stored field is not what the player experiences and the tooltip
                        // is not contradicted by it. Measured on one real map, all four
                        // abilities left here are named in the script, and one of them holds
                        // its area at 0.1, which is the author neutralising the base so a
                        // trigger can own the radius outright. Writing the tooltip's number
                        // into the field would then apply the area twice.
                        //
                        // Reported rather than dropped, because the alternative reading is a
                        // stale tooltip and only the author can tell the two apart.
                        string claim =
                            $"level {lv} promises {label} {promised.ToString(CultureInfo.InvariantCulture)} "
                            + $"but the data holds {actual.ToString(CultureInfo.InvariantCulture)}";
                        if (NamedInScript(rawcode, id)) triggerOwnedClaims.Add($"{rawcode} {claim}");
                        else
                            issues.Add(new AuditIssue(DiagnosticSeverity.Warning, "tooltip-claim",
                                rawcode, who, claim));
                    }
                }

            if (Want("orphan-ability") && who is not null)
            {
                // Two very different things look alike here, and only one is a defect.
                //
                // An ability that overrides NO effect value is simply a plain use of its base,
                // and it works. Flagging those put 167 warnings on one map where the real
                // number was one, which trains a reader to ignore the check.
                //
                // An ability that sets its effect values and sets them all to ZERO is the
                // author deliberately neutralising the base so a trigger can own the behaviour.
                // That one is broken if no trigger ever mentions it.
                bool hasEffectFields = byCode.Any(kv => DataField.IsMatch(kv.Key));
                bool anyEffect = byCode.Any(kv => DataField.IsMatch(kv.Key)
                    && kv.Value.Values.Any(v => v is not ("0" or "0.0" or "" or null)));
                if (!hasEffectFields) continue;
                // A script refers to an ability as a 'ABCD' literal or as its integer value,
                // and the byte order of that integer is NOT the one War3Net's FromRawcode
                // produces. On a real map 'A050' appears nine times as 1093678384, the
                // big-endian form, and zero times as the little-endian one, so testing only
                // the entry Id reported 140 abilities as implemented by nothing when the real
                // number was one. Both orders are checked.
                // The OWNER counts too, and missing that reported six false orphans on one
                // real map. A trigger commonly implements an item by watching for the ITEM id
                // rather than for the ability id, so the ability's own rawcode never appears
                // while the behaviour is fully implemented. Orb of Fire, Force Staff and
                // Sacrificial Wand are all that shape there.
                string? ownerRaw = ownerCode.GetValueOrDefault(id);
                bool ownerNamed = ownerRaw is not null
                    && NamedInScript(ownerRaw, ownerRaw.FromRawcode());
                if (!anyEffect && !ownerNamed && !NamedInScript(rawcode, id))
                    issues.Add(new AuditIssue(DiagnosticSeverity.Warning, "orphan-ability", rawcode, who,
                        "sets its effect values to zero, which neutralises the base ability so a "
                        + "trigger can own the behaviour, but its rawcode never appears in the "
                        + "trigger script in either byte order, so nothing implements it"));
            }
        }

        if (triggerOwnedClaims.Count > 0)
            diagnostics.Add($"tooltip-claim set aside {triggerOwnedClaims.Count} claim(s) on "
                + $"{triggerOwnedClaims.Select(c => c[..4]).Distinct().Count()} ability(ies) the "
                + "trigger script names, where the stored field is not what the player gets ("
                + string.Join("; ", triggerOwnedClaims.Take(4))
                + (triggerOwnedClaims.Count > 4 ? "; ..." : "") + ")");

        if (deadTips.Count > 0)
            diagnostics.Add($"level-tooltip found {deadTips.Count} ability(ies) carrying "
                + "tooltip text above the level a player can reach, which is unreachable rather "
                + $"than wrong ({string.Join("; ", deadTips.Take(3))}"
                + (deadTips.Count > 3 ? "; ..." : "") + ")");

        if (Want("requirement"))
            RequirementCheck(doc, abilities, owner, gameDirOverride, issues, diagnostics);

        if (Want("portrait-risk"))
            PortraitCheck(doc, units, issues);

        if (Want("dangling-reference"))
            ReferenceCheck(doc, owner, strings, gameDirOverride, issues, diagnostics);

        if (Want("missing-model"))
        {
            // A model that fails to load is logged and retried on every creation, never fatal,
            // so it is a warning. `repair model-paths` fixes the ones with a single answer.
            var scan = ModelPathCommand.Scan(doc, gameDirOverride);
            foreach (var m in scan.Missing)
                issues.Add(new AuditIssue(DiagnosticSeverity.Warning, "missing-model",
                    m.Rawcode ?? "script", m.Field is null ? null : $"{m.Source} {m.Field}",
                    (m.Placeholder
                        ? $"'{m.Path}' names no file, so the engine fails to load it every time it is created"
                        : $"'{m.Path}' resolves to no file in the map or the game")
                    + (m.Uses > 1 ? $", {m.Uses} uses" : "")
                    + (m.Suggestion is null ? "" : $", the archive holds '{m.Suggestion}'")));
            diagnostics.AddRange(scan.Diagnostics.Select(d => $"missing-model {d}"));
        }

        bool ok = issues.All(i => i.Severity != DiagnosticSeverity.Error);
        return new AuditResult(ok, abilities.Count, issues, diagnostics);
    }

    /// <summary>
    /// An ability whose requirement comes from its BASE and names something the map never
    /// defines. The map cannot satisfy it, so the ability is permanently locked.
    ///
    /// This is the shape that broke twenty abilities on one map after 3.0.0. Berserk carries
    /// Requires=Robk, the Berserker Upgrade, so every custom ability built on it inherits a
    /// requirement that a map with no orc tech tree can never meet.
    /// </summary>
    private static void RequirementCheck(
        MapDocument doc, IReadOnlyDictionary<int, MapObjectEntry> abilities,
        IReadOnlyDictionary<int, string> owner, string? gameDirOverride,
        List<AuditIssue> issues, List<string> diagnostics)
    {
        if (!GameData.GameData.TryOpen(gameDirOverride, out _, out var diagnostic))
        {
            diagnostics.Add($"requirement check skipped, game data unavailable ({diagnostic})");
            return;
        }
        foreach (var r in InheritedRequirements(doc, abilities.Keys, gameDirOverride))
        {
            owner.TryGetValue(r.Rawcode.FromRawcode(), out var who);
            issues.Add(new AuditIssue(DiagnosticSeverity.Error, "requirement", r.Rawcode, who,
                $"inherits Requirements '{r.Inherited}' from its base ability, and the map defines no "
                + $"{string.Join(", ", r.Unmet)}, so the ability can never be unlocked. "
                + "Set an explicit empty Requirements at level 0 to override the inheritance."));
        }
    }

    /// <summary>An ability whose base Requirements name something the map never defines.</summary>
    internal sealed record InheritedRequirement(string Rawcode, string Inherited, IReadOnlyList<string> Unmet);

    /// <summary>
    /// The abilities <see cref="RequirementCheck"/> reports, as data, so the audit and
    /// <see cref="AuditRepairCommand"/> can never disagree about which ones are locked.
    /// Needs game data to see the inherited value. Empty when it is unavailable.
    /// </summary>
    internal static List<InheritedRequirement> InheritedRequirements(
        MapDocument doc, IEnumerable<int> abilityIds, string? gameDirOverride)
    {
        var found = new List<InheritedRequirement>();
        // Everything the map defines, which is what a requirement may legitimately name. The
        // map's own gating redefines base unit ids as invisible markers, so a requirement
        // naming one of those is deliberate and must not be reported.
        var defined = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kind in ObjectKinds.All)
            foreach (var e in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                defined.Add(e.Id.ToRawcode());

        foreach (var id in abilityIds.OrderBy(i => i))
        {
            string rawcode = id.ToRawcode();
            var merged = ObjectGetCommand.Execute(doc, ObjectKind.Ability, rawcode, gameDirOverride);
            var field = merged.Fields.FirstOrDefault(f =>
                f.Name.Equals("Requirements", StringComparison.OrdinalIgnoreCase));
            if (field is null || field.Source != "base" || string.IsNullOrWhiteSpace(field.Value))
                continue;
            var unmet = field.Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0 && s != "_" && !defined.Contains(s))
                .ToList();
            if (unmet.Count > 0) found.Add(new InheritedRequirement(rawcode, field.Value, unmet));
        }
        return found;
    }

    /// <summary>
    /// A rawcode list that names an object neither the map nor the installed game defines.
    ///
    /// This is the shape behind "the spell casts but nothing happens". An ability's abuf names
    /// the buff it applies, and a buff id that resolves nowhere applies no buff, with no error
    /// anywhere. The same holds for a unit granting an ability id that does not exist.
    ///
    /// Both halves of the lookup are needed and neither alone is enough. Checking only the map
    /// reports every base game reference, 284 of them on one real map where Bfro is simply
    /// Frost Armor and entirely correct. Checking only the game reports every custom object the
    /// map itself defines. The union is the only set that means anything.
    /// </summary>
    private static void ReferenceCheck(
        MapDocument doc, IReadOnlyDictionary<int, string> owner, MapStrings strings,
        string? gameDirOverride, List<AuditIssue> issues, List<string> diagnostics)
    {
        if (!GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic))
        {
            diagnostics.Add($"dangling-reference check skipped, game data unavailable ({diagnostic})");
            return;
        }

        var scan = DanglingReferences(doc, ctx!);
        var byId = new Dictionary<string, MapObjectEntry>(StringComparer.Ordinal);
        foreach (var kind in ObjectKinds.All)
            foreach (var e in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                byId.TryAdd(e.Id.ToRawcode(), e);
        foreach (var d in scan.Dangling)
        {
            owner.TryGetValue(d.Rawcode.FromRawcode(), out var who);
            string name = who ?? (byId.TryGetValue(d.Rawcode, out var e) ? DisplayName(e, strings) : d.Rawcode);
            issues.Add(new AuditIssue(DiagnosticSeverity.Error, "dangling-reference", d.Rawcode, name,
                $"{d.Key} names {d.Names} '{d.Target}', which neither the map nor the "
                + $"installed game defines, so it resolves to nothing at runtime"));
        }

        // A check that has only ever been seen returning nothing has reported on the check and
        // not on the map, so the population it walked is stated whatever the result.
        diagnostics.Add($"dangling-reference examined {scan.Examined} rawcode reference(s), "
            + $"{scan.ResolvedByMap} resolved by the map and {scan.ResolvedByGame} by the installed game");
    }

    private static HashSet<string> BuffsNamedByBaseAbilities(GameData.BaseAbilityStore abilities)
    {
        var named = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var code in abilities.Rawcodes)
        {
            if (!abilities.TryGetAbility(code, out var fields)) continue;
            foreach (var (key, value) in fields)
                if (key.StartsWith("abuf", StringComparison.Ordinal) || key.StartsWith("aeff", StringComparison.Ordinal))
                    foreach (var id in Split(value)) named.Add(id);
        }
        return named;
    }

    /// <summary>One list entry that names an object neither the map nor the game defines.</summary>
    internal sealed record DanglingReference(ObjectKind Kind, string Rawcode, string Key, string Target, string Names);

    internal sealed record DanglingScan(
        IReadOnlyList<DanglingReference> Dangling, int Examined, int ResolvedByMap, int ResolvedByGame);

    /// <summary>
    /// The references <see cref="ReferenceCheck"/> reports, as data, shared with
    /// <see cref="AuditRepairCommand"/> so the audit and the repair can never disagree.
    /// </summary>
    internal static DanglingScan DanglingReferences(MapDocument doc, GameData.GameDataContext ctx)
    {
        var mapDefines = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kind in ObjectKinds.All)
            foreach (var e in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                mapDefines.Add(e.Id.ToRawcode());

        // A buff id is legitimately satisfied by either store, because the engine lets an
        // ability id stand where a buff is expected and several base abilities do exactly that.
        var buffs = ctx.Buffs;
        var baseAbilities = ctx.Abilities;
        // Some buffs have no row in the buff table and exist only in the skin data, BHtb
        // (skinType=buff, named by ACcb's BuffID1) being the case found. A buff the game's own
        // abilities name is one the game defines, so those count too.
        var namedByGame = BuffsNamedByBaseAbilities(baseAbilities);
        bool GameHas(string code, string names) => names == "buff"
            ? buffs.TryGet(code, out _) || baseAbilities.TryGetAbility(code, out _) || namedByGame.Contains(code)
            : baseAbilities.TryGetAbility(code, out _);

        var found = new List<DanglingReference>();
        int examined = 0, resolvedByMap = 0, resolvedByGame = 0;
        foreach (var (kind, code, names) in References)
            foreach (var e in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
            {
                foreach (var (key, value) in e.Mods)
                {
                    if (!key.StartsWith(code, StringComparison.OrdinalIgnoreCase)) continue;
                    if (key.Length != code.Length && key[code.Length] != ':') continue;
                    foreach (var target in Split(value?.ToString()))
                    {
                        examined++;
                        if (mapDefines.Contains(target)) { resolvedByMap++; continue; }
                        if (GameHas(target, names)) { resolvedByGame++; continue; }
                        found.Add(new DanglingReference(kind, e.Id.ToRawcode(), key, target, names));
                    }
                }
            }
        return new DanglingScan(found, examined, resolvedByMap, resolvedByGame);
    }

    private static string DisplayName(MapObjectEntry e, MapStrings strings) =>
        strings.Resolve(ModValue(e, "unam") ?? ModValue(e, "anam")) is { Length: > 0 } n
        && !n.StartsWith("TRIGSTR_", StringComparison.Ordinal) ? n : e.Id.ToRawcode();

    /// <summary>
    /// A unit whose model ships a camera at a model version below 900. Those render a black
    /// portrait pane under Reforged 3.0.0 while models with no camera fall back to the engine's
    /// own and render correctly.
    ///
    /// Only the two header facts are read, not the whole model, because a map can reference
    /// several hundred models and parsing each one to answer "does a CAMS chunk exist" would
    /// cost far more than walking the top-level chunk list.
    /// </summary>
    private static void PortraitCheck(
        MapDocument doc, IReadOnlyList<MapObjectEntry> units, List<AuditIssue> issues)
    {
        var seen = new Dictionary<string, (int? Version, bool Camera)>(StringComparer.OrdinalIgnoreCase);
        foreach (var u in units)
        {
            string? model = ModValue(u, "umdl");
            if (string.IsNullOrWhiteSpace(model)) continue;
            string stem = model.Replace('/', '\\').Split('\\').Last();
            int dot = stem.LastIndexOf('.');
            if (dot > 0) stem = stem[..dot];

            if (!seen.TryGetValue(stem, out var info))
            {
                info = (null, false);
                // Resolved by MPQ name hash, NOT through GetFile. This map is protected and 793
                // of its 851 entries carry no listfile name, so a GetFile lookup found 3 of the
                // 113 units that actually carry the defect and the other 110 read as clean.
                foreach (var candidate in new[] { model, stem + ".mdx", stem + ".mdl" })
                {
                    if (!doc.TryReadFileByName(candidate, out var raw) || raw.Length < 12) continue;
                    info = ReadMdxHeader(raw);
                    break;
                }
                seen[stem] = info;
            }
            if (info.Camera && info.Version is int v && v < 900)
                issues.Add(new AuditIssue(DiagnosticSeverity.Warning, "portrait-risk",
                    u.Id.ToRawcode(), ModValue(u, "unam"),
                    $"model '{stem}' is version {v} and ships a camera, which renders a black "
                    + "portrait pane under Reforged 3.0.0. Models with no camera are unaffected."));
        }
    }

    /// <summary>(VERS value, has a CAMS chunk) from a top-level MDX chunk walk.</summary>
    internal static (int? Version, bool Camera) ReadMdxHeader(byte[] data)
    {
        if (data.Length < 12 || data[0] != 'M' || data[1] != 'D' || data[2] != 'L' || data[3] != 'X')
            return (null, false);
        int? version = null;
        bool camera = false;
        int p = 4;
        while (p + 8 <= data.Length)
        {
            string tag = Encoding.ASCII.GetString(data, p, 4);
            uint size = BitConverter.ToUInt32(data, p + 4);
            if (size > int.MaxValue || p + 8 + (long)size > data.Length) break;
            if (tag == "VERS" && size >= 4) version = BitConverter.ToInt32(data, p + 8);
            if (tag == "CAMS") camera = true;
            p += 8 + (int)size;
        }
        return (version, camera);
    }

    // ---- helpers -----------------------------------------------------------------

    private static string Slot(int i) => i < 5 ? "QWERD"[i].ToString() : (i + 1).ToString();

    /// <summary>The rawcode as the big-endian integer a trigger script writes it as.</summary>
    internal static long BigEndian(string rawcode)
    {
        long v = 0;
        foreach (char c in rawcode) v = (v << 8) | (byte)c;
        return v;
    }

    private static IEnumerable<string> Split(string? list) =>
        string.IsNullOrWhiteSpace(list)
            ? Enumerable.Empty<string>()
            : list.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => s.Trim()).Where(s => s.Length > 0 && s != "_");

    private static string? ModValue(MapObjectEntry e, string code) =>
        e.Mods.FirstOrDefault(m => m.Key == code).Value;

    /// <summary>Mods regrouped as code to level to value, with a bare key meaning level 0.</summary>
    private static Dictionary<string, Dictionary<int, string>> Levelled(MapObjectEntry e)
    {
        var map = new Dictionary<string, Dictionary<int, string>>(StringComparer.Ordinal);
        foreach (var (key, value) in e.Mods)
        {
            int colon = key.IndexOf(':');
            string code = colon < 0 ? key : key[..colon];
            int level = colon < 0 ? 0
                : int.TryParse(key[(colon + 1)..], out var n) ? n : 0;
            if (!map.TryGetValue(code, out var levels))
                map[code] = levels = new Dictionary<int, string>();
            levels[level] = value;
        }
        return map;
    }

    private static int Declared(IReadOnlyDictionary<string, Dictionary<int, string>> byCode) =>
        byCode.TryGetValue("alev", out var lv) && lv.Count > 0
            && int.TryParse(lv.Values.First(), out var n) && n > 0 ? n : 1;

    /// <summary>
    /// The extended and short tooltip for one level, colour tags stripped, with any
    /// TRIGSTR_ reference resolved against war3map.wts first. A tooltip held in the string
    /// table rather than inline reads as the literal token TRIGSTR_042 otherwise, and every
    /// claim inside it is silently skipped, which looks exactly like an ability that makes
    /// no claims.
    /// </summary>
    private static string Tooltip(
        IReadOnlyDictionary<string, Dictionary<int, string>> byCode, int level, MapStrings strings)
    {
        var sb = new StringBuilder();
        foreach (var f in new[] { "aub1", "atp1" })
            if (byCode.TryGetValue(f, out var lv) && lv.TryGetValue(level, out var t) && t.Length > 0)
            {
                string text = t.Contains("TRIGSTR_", StringComparison.Ordinal)
                    ? strings.Resolve(t.Trim()) : t;
                if (text.Length == 0 || text.StartsWith("TRIGSTR_", StringComparison.Ordinal)) continue;
                sb.Append(ColourTag.Replace(text, " ")).Append(' ');
            }
        return sb.ToString();
    }

    private static string ScriptText(MapDocument doc)
    {
        // Both script paths and a HASH lookup, because a protected map names neither and
        // Reforged moved the script under a scripts folder. Reading only war3map.j through
        // GetFile returned EMPTY on BleachVsOnepiece13, whose 4.4 MB script lives at
        // scripts\war3map.j, and an empty script makes every ability look like an orphan.
        // 166 of them did, on a map where the real number is one.
        foreach (var name in new[] { "war3map.j", "war3map.lua",
                                     @"scripts\war3map.j", @"scripts\war3map.lua" })
            if (doc.TryReadFileByName(name, out var bytes) && bytes.Length > 0)
                return Encoding.Latin1.GetString(bytes);
        return string.Empty;
    }
}
