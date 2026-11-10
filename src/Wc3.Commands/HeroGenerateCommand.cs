// src/Wc3.Commands/HeroGenerateCommand.cs
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One unit object generated for a form.</summary>
/// <param name="FormId">The spec's form id, for example <c>bankai</c>.</param>
/// <param name="Rawcode">The fresh unit rawcode the form was allocated.</param>
/// <param name="BaseRawcode">The unit base the form was derived from.</param>
/// <param name="Name">The name written to the unit's name field.</param>
/// <param name="AbilityRawcodes">The form's own abilities, in spec order, the exact value written to uabi.</param>
public sealed record HeroGeneratedUnit(
    string FormId, string Rawcode, string BaseRawcode, string Name,
    IReadOnlyList<string> AbilityRawcodes);

/// <summary>One ability object generated for a slot.</summary>
/// <param name="OrderId">The Base Order ID written to every level of Ncl6, derived from the rawcode.</param>
/// <param name="TransformsToForm">The target form id, for a transform ability, else null.</param>
/// <param name="TransformsToUnit">The target form's unit rawcode, for a transform ability, else null.</param>
public sealed record HeroGeneratedAbility(
    string FormId, string Slot, string Rawcode, string BaseRawcode, string Name,
    string OrderId, int Levels, bool IsTransform,
    string? TransformsToForm, string? TransformsToUnit);

/// <summary>One object-data change, either the creation of an object or one field written to it.</summary>
/// <param name="Kind">The object kind in lower case, for example <c>unit</c> or <c>ability</c>.</param>
/// <param name="Field">The field token as written, for example <c>acdn:1</c>, or <c>(created)</c>.</param>
public sealed record HeroObjectChange(string Kind, string Rawcode, string Field, string Value);

/// <summary>
/// What a generation produced. <see cref="Jass"/> is a self-contained block, not spliced into the
/// map script, because appending to a real war3map.j is a separate and lossy-looking operation.
/// </summary>
public sealed record HeroGenerateResult(
    bool Ok,
    string Message,
    IReadOnlyList<HeroGeneratedUnit> Units,
    IReadOnlyList<HeroGeneratedAbility> Abilities,
    IReadOnlyList<string> OrderIds,
    string Jass,
    IReadOnlyList<HeroObjectChange> ObjectChanges,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Generates a multi-form anime hero from a small JSON brief.
///
/// <para>Who, a content author who has a hero brief rather than an object editor session. What,
/// one unit object per form derived from a single base unit, one ability object per ability
/// derived from its base (Channel unless the brief says otherwise), a unique Base Order ID per
/// Channel ability, cooldown, cast time and tooltip per level, each form's own ability list in
/// uabi, and a JASS block with one transform function per form and one timer driven damage stub
/// per damaging ability. Where, this command builds every object through
/// <see cref="ObjectNewCommand"/> and <see cref="ObjectSetCommand"/>, so the map's own object
/// serializer is the only one in play.</para>
///
/// <para>When. Every run validates the whole brief before the first object is created, so a brief
/// that names an unknown base is refused with the map untouched rather than half written. The
/// reason is that a partially generated hero is worse than none, since the missing half looks like
/// a deliberate design choice in the World Editor and nothing in the map says otherwise.</para>
///
/// <para>Which. A form with no abilities still becomes a unit object with an empty ability list.
/// An ability's <c>kind</c> is <c>normal</c> or <c>transform</c>, and a transform must name a form
/// in the same brief through <c>to</c>. The generated JASS is returned rather than written into
/// war3map.j, because this command has no way to prove a splice preserved the rest of a real map's
/// script.</para>
/// </summary>
public static class HeroGenerateCommand
{
    /// <summary>The Channel base every ability inherits from when the brief names none.</summary>
    public const string DefaultAbilityBase = "ANcl";

    /// <summary>The area a generated damage stub damages, in world units. One value for every stub
    /// so the JASS stays readable, and a brief that needs another radius should edit the stub.</summary>
    public const double DefaultDamageRadius = 300.0;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    // A form id becomes part of a JASS function name, so it has to be an identifier by itself.
    // Refusing a spaced or dashed id is clearer than silently rewriting it into a name the brief
    // never mentioned, and two ids that sanitise to the same name would silently share a function.
    private static readonly Regex FormIdPattern = new("^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.Compiled);

    /// <summary>
    /// Loads <paramref name="mapPath"/>, applies <paramref name="specJson"/>, and writes the result
    /// to <paramref name="outputPath"/>. The output is only written when generation succeeded, so a
    /// refused brief leaves no file behind.
    /// </summary>
    /// <param name="mapPath">The source map to derive from. Never modified on disk.</param>
    /// <param name="outputPath">Where the generated map is written. May equal the source path.</param>
    /// <param name="specJson">The hero brief, see the class summary for its shape.</param>
    /// <param name="gameDirOverride">A Warcraft III install, only consulted when the map itself
    /// does not define a base rawcode. Null means auto-detect.</param>
    public static HeroGenerateResult Execute(
        string mapPath, string outputPath, string specJson, string? gameDirOverride = null)
    {
        if (string.IsNullOrWhiteSpace(mapPath) || !File.Exists(mapPath))
            return Fail($"map '{mapPath}' does not exist");
        if (string.IsNullOrWhiteSpace(outputPath))
            return Fail("an output map path is required");

        MapDocument doc;
        try { doc = MapDocument.Load(mapPath); }
        catch (Exception ex) { return Fail($"'{mapPath}' could not be loaded: {ex.Message}"); }

        var result = Execute(doc, specJson, gameDirOverride);
        if (!result.Ok) return result;

        try
        {
            var folder = Path.GetDirectoryName(Path.GetFullPath(outputPath));
            if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);
            doc.Save(outputPath);
        }
        catch (Exception ex) { return Fail($"'{outputPath}' could not be written: {ex.Message}"); }

        return result with { Message = result.Message + $" Written to {outputPath}." };
    }

    /// <summary>
    /// Applies the brief to an already loaded map and returns the result without saving.
    ///
    /// The document is mutated in place, the same contract as every other object command. All
    /// name, base and shape validation happens before the first creation, so a refused brief leaves
    /// the document as it was. A failure during the write phase itself, which is only reachable on
    /// a map whose object file exists but cannot be parsed, leaves the document partially modified
    /// and the caller must discard it rather than save it.
    /// </summary>
    public static HeroGenerateResult Execute(
        MapDocument doc, string specJson, string? gameDirOverride = null)
    {
        ArgumentNullException.ThrowIfNull(doc);

        HeroSpec? spec;
        try { spec = JsonSerializer.Deserialize<HeroSpec>(specJson, JsonOptions); }
        catch (JsonException ex) { return Fail($"the brief is not valid JSON: {ex.Message}"); }
        if (spec is null) return Fail("the brief is empty");

        var errors = new List<string>();
        var plans = Plan(spec, doc, gameDirOverride, errors);
        if (plans is null || errors.Count > 0)
            return Fail(string.Join(". ", errors));

        // Every check above passed, so the write phase starts from a known-good brief.
        var changes = new List<HeroObjectChange>();
        var abilities = new List<(AbilityPlan Plan, string Rawcode, string OrderId)>();
        foreach (var form in plans)
            foreach (var ability in form.Abilities)
            {
                var created = ObjectNewCommand.Execute(doc, ObjectKind.Ability, ability.Base, gameDirOverride);
                if (!created.Ok || created.NewRawcode is null)
                    return Fail($"ability '{ability.Name}' could not be created: {created.Message}");
                string rawcode = created.NewRawcode;
                string order = OrderIdFor(rawcode);
                abilities.Add((ability, rawcode, order));
                changes.Add(new("ability", rawcode, "(created)", $"base {ability.Base}"));
            }

        string baseUnit = spec.BaseUnit!.Trim();
        var units = new List<(FormPlan Plan, string Rawcode)>();
        foreach (var form in plans)
        {
            var created = ObjectNewCommand.Execute(doc, ObjectKind.Unit, baseUnit, gameDirOverride);
            if (!created.Ok || created.NewRawcode is null)
                return Fail($"form '{form.Id}' could not be created: {created.Message}");
            units.Add((form, created.NewRawcode));
            changes.Add(new("unit", created.NewRawcode, "(created)", $"base {baseUnit}"));
        }

        // One session per kind, learned after every creation, so each field set is a dictionary
        // lookup and a group edit rather than a fresh walk of the map's object data.
        var abilitySession = new ObjectSetCommand.Session(doc, ObjectKind.Ability);
        var unitSession = new ObjectSetCommand.Session(doc, ObjectKind.Unit);
        var writeErrors = new List<string>();

        bool Set(ObjectKind kind, ObjectSetCommand.Session session, string rawcode, string field, string value)
        {
            var set = ObjectSetCommand.Execute(doc, kind, rawcode, field, value, session);
            if (!set.Ok)
            {
                writeErrors.Add($"could not set {field} on {rawcode}: {set.Message}");
                return false;
            }
            changes.Add(new(kind.ToString().ToLowerInvariant(), rawcode, field, value));
            return true;
        }

        var orderIds = new List<string>();
        foreach (var (plan, rawcode, order) in abilities)
        {
            orderIds.Add(order);
            Set(ObjectKind.Ability, abilitySession, rawcode, "anam:0", plan.Name);
            Set(ObjectKind.Ability, abilitySession, rawcode, "ahky:0", plan.Slot);
            Set(ObjectKind.Ability, abilitySession, rawcode, "alev:0",
                plan.Levels.ToString(CultureInfo.InvariantCulture));
            for (int level = 1; level <= plan.Levels; level++)
            {
                Set(ObjectKind.Ability, abilitySession, rawcode, $"acdn:{level}",
                    Real(plan.Cooldowns[level - 1]));
                Set(ObjectKind.Ability, abilitySession, rawcode, $"acas:{level}", Real(plan.Cast));
                Set(ObjectKind.Ability, abilitySession, rawcode, $"atp1:{level}", plan.Tooltip);
                Set(ObjectKind.Ability, abilitySession, rawcode, $"aub1:{level}",
                    plan.ExtendedTooltip(level - 1));
                // The same order string on every level, because a level that disagrees makes the
                // ability ambiguous to the engine and is reported as a problem by the order audit.
                Set(ObjectKind.Ability, abilitySession, rawcode, $"Ncl6:{level}", order);
            }
        }

        var generatedUnits = new List<HeroGeneratedUnit>();
        foreach (var (plan, rawcode) in units)
        {
            var own = abilities
                .Where(a => string.Equals(a.Plan.FormId, plan.Id, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Rawcode)
                .ToList();

            Set(ObjectKind.Unit, unitSession, rawcode, "unam", plan.UnitName);
            if (own.Count > 0)
                Set(ObjectKind.Unit, unitSession, rawcode, "uabi", string.Join(",", own));

            generatedUnits.Add(new HeroGeneratedUnit(plan.Id, rawcode, baseUnit,
                plan.UnitName, own));
        }

        if (writeErrors.Count > 0)
            return Fail(string.Join(". ", writeErrors));

        var generatedAbilities = abilities.Select(a =>
        {
            string? toUnit = a.Plan.To is null
                ? null
                : units.FirstOrDefault(u => string.Equals(u.Plan.Id, a.Plan.To, StringComparison.OrdinalIgnoreCase)).Rawcode;
            return new HeroGeneratedAbility(a.Plan.FormId, a.Plan.Slot, a.Rawcode, a.Plan.Base,
                a.Plan.Name, a.OrderId, a.Plan.Levels, a.Plan.IsTransform, a.Plan.To, toUnit);
        }).ToList();

        string jass = BuildJass(spec.Name!.Trim(), units, abilities);

        return new HeroGenerateResult(
            true,
            $"generated {units.Count} form(s), {abilities.Count} ability object(s), "
                + $"{changes.Count} object change(s).",
            generatedUnits, generatedAbilities, orderIds, jass, changes,
            new[]
            {
                "the JASS block was returned and not spliced into war3map.j, because appending to a "
                    + "real map script is a separate operation with its own fidelity risk",
            });
    }

    /// <summary>The Base Order ID for a generated ability, derived from its own rawcode so two
    /// generated abilities can never share one.</summary>
    internal static string OrderIdFor(string rawcode) => "wc3gen" + rawcode;

    private sealed record FormPlan(string Id, string UnitName, IReadOnlyList<AbilityPlan> Abilities);

    private sealed record AbilityPlan(
        string FormId, string Slot, string Name, string Base, bool IsTransform, string? To,
        int Levels, double[] Cooldowns, double Cast, double[] Damage, string Tooltip)
    {
        /// <summary>The extended tooltip, stating the highest level's damage and cooldown because
        /// one line serves every level of the ability.</summary>
        internal string ExtendedTooltip(int level)
        {
            double cooldown = Cooldowns[Math.Min(level, Cooldowns.Length - 1)];
            string cd = Number(cooldown);
            return Damage.Length == 0
                ? $"Cooldown {cd} seconds."
                : $"Deals {Number(Damage[Math.Min(level, Damage.Length - 1)])} damage. "
                    + $"Cooldown {cd} seconds.";
        }
    }

    /// <summary>
    /// Validates the brief and turns it into a write plan. Returns null when the brief cannot be
    /// read at all, and collects every other problem into <paramref name="errors"/> so an author
    /// fixes the whole brief in one pass rather than one error at a time.
    /// </summary>
    private static List<FormPlan>? Plan(
        HeroSpec spec, MapDocument doc, string? gameDirOverride, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(spec.Name)) errors.Add("name is required");
        string heroName = spec.Name?.Trim() ?? "";
        string baseUnit = spec.BaseUnit?.Trim() ?? "";
        if (baseUnit.Length != 4) errors.Add($"base_unit must be a four character rawcode, got '{baseUnit}'");
        if (spec.Forms is null || spec.Forms.Count == 0) errors.Add("at least one form is required");

        var plans = new List<FormPlan>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var form in spec.Forms ?? new List<FormSpec>())
        {
            string id = form.Id?.Trim() ?? "";
            if (id.Length == 0) { errors.Add("a form has no id"); continue; }
            if (!FormIdPattern.IsMatch(id))
            {
                errors.Add($"form id '{id}' must start with a letter and contain only letters, "
                    + "digits and underscores, because it names a JASS function");
                continue;
            }
            if (!ids.Add(id)) { errors.Add($"form id '{id}' is declared more than once"); continue; }

            string unitName = plans.Count == 0 ? heroName : $"{heroName} ({id})";
            var abilities = new List<AbilityPlan>();
            foreach (var ability in form.Abilities ?? new List<AbilitySpec>())
                if (ReadAbility(ability, id, errors) is { } plan) abilities.Add(plan);
            plans.Add(new FormPlan(id, unitName, abilities));
        }

        // A transform has to point at a form this brief declares, or the JASS would name a unit
        // that does not exist. Checked after the whole form list is known so order does not matter.
        foreach (var form in plans)
            foreach (var ability in form.Abilities)
            {
                if (!ability.IsTransform) continue;
                var target = plans.FirstOrDefault(p =>
                    string.Equals(p.Id, ability.To, StringComparison.OrdinalIgnoreCase));
                if (target is null)
                    errors.Add($"ability '{ability.Name}' transforms to '{ability.To}', which is not a form in this brief");
            }

        // Base existence is the one check that needs the map or the game install, so it runs last
        // and only once per distinct code.
        var checkedBases = new HashSet<(ObjectKind, string)>();
        bool Known(ObjectKind kind, string code)
        {
            if (!checkedBases.Add((kind, code))) return true;
            return ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind))
                       .Any(e => e.Id == code.FromRawcode())
                || GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _)
                    && ctx is not null
                    && ObjectKinds.TryGetBaseFields(ctx, kind, code, out _);
        }

        if (baseUnit.Length == 4 && !Known(ObjectKind.Unit, baseUnit))
            errors.Add($"base_unit '{baseUnit}' is not a unit this map defines and the game data does not confirm it");
        foreach (var form in plans)
            foreach (var ability in form.Abilities)
                if (!Known(ObjectKind.Ability, ability.Base))
                    errors.Add($"ability '{ability.Name}' is based on '{ability.Base}', which is not an "
                        + "ability this map defines and the game data does not confirm it");

        return plans;
    }

    private static AbilityPlan? ReadAbility(AbilitySpec spec, string formId, List<string> errors)
    {
        string slot = spec.Slot?.Trim() ?? "";
        if (slot.Length == 0) errors.Add($"a {formId} ability has no slot");
        string name = spec.Name?.Trim() ?? "";
        if (name.Length == 0) errors.Add($"the {formId} {slot} ability has no name");

        string baseCode = string.IsNullOrWhiteSpace(spec.Base) ? DefaultAbilityBase : spec.Base.Trim();
        if (baseCode.Length != 4)
            errors.Add($"ability '{name}' base must be a four character rawcode, got '{baseCode}'");

        string kind = spec.Kind?.Trim().ToLowerInvariant() ?? "";
        if (kind is not ("" or "normal" or "transform"))
            errors.Add($"ability '{name}' kind '{kind}' is not normal or transform");
        bool isTransform = kind == "transform";
        string? to = isTransform ? spec.To?.Trim() : null;
        if (isTransform && string.IsNullOrEmpty(to))
            errors.Add($"transform ability '{name}' must name its target form in 'to'");

        var (cooldowns, cooldownError) = ReadNumbers(spec.Cooldown, $"ability '{name}' cooldown");
        if (cooldownError is not null) errors.Add(cooldownError);
        if (cooldowns.Length == 0) cooldowns = new[] { 0.0 };
        if (cooldowns.Any(c => c < 0)) errors.Add($"ability '{name}' has a negative cooldown");

        double cast = spec.Cast ?? 0;
        if (cast < 0) errors.Add($"ability '{name}' has a negative cast time");

        var (damage, damageError) = ReadNumbers(spec.Damage, $"ability '{name}' damage");
        if (damageError is not null) errors.Add(damageError);
        if (damage.Any(d => d < 0)) errors.Add($"ability '{name}' has a negative damage");
        if (damage.Length > 0 && damage.Length != cooldowns.Length)
            damage = Enumerable.Range(0, cooldowns.Length)
                .Select(i => damage[Math.Min(i, damage.Length - 1)]).ToArray();

        string tooltip = string.IsNullOrWhiteSpace(spec.Tooltip) ? name : spec.Tooltip.Trim();

        // An ability with no identity is not planned at all, since nothing downstream could name
        // it. Every other problem is reported and the plan is kept, so the caller sees the full
        // list of faults in one pass.
        if (slot.Length == 0 || name.Length == 0 || baseCode.Length != 4)
            return null;

        return new AbilityPlan(formId, slot, name, baseCode, isTransform, to,
            cooldowns.Length, cooldowns, cast, damage, tooltip);
    }

    /// <summary>Reads a JSON value that is either one number or an array of numbers.</summary>
    private static (double[] Values, string? Error) ReadNumbers(JsonElement element, string label)
    {
        if (element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return (Array.Empty<double>(), null);
        if (element.ValueKind == JsonValueKind.Number)
            return (new[] { element.GetDouble() }, null);
        if (element.ValueKind != JsonValueKind.Array)
            return (Array.Empty<double>(), $"{label} must be a number or an array of numbers");

        var values = new List<double>();
        foreach (var item in element.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Number)
                return (Array.Empty<double>(), $"{label} must contain only numbers");
            values.Add(item.GetDouble());
        }
        return values.Count == 0
            ? (Array.Empty<double>(), $"{label} must not be empty")
            : (values.ToArray(), null);
    }

    /// <summary>
    /// The JASS block. One transform function per form, so a form is both a source and a target
    /// without another function being written for each edge, plus one timer driven damage stub per
    /// damaging ability and a cast entry point that starts it.
    /// </summary>
    private static string BuildJass(
        string heroName,
        IReadOnlyList<(FormPlan Plan, string Rawcode)> forms,
        IReadOnlyList<(AbilityPlan Plan, string Rawcode, string OrderId)> abilities)
    {
        var sb = new StringBuilder();
        sb.AppendLine("//=== wc3ctl hero generate (generated, do not edit) ===");
        sb.Append("// Hero ").AppendLine(heroName);
        foreach (var (plan, rawcode) in forms)
        {
            var own = abilities.Where(a => string.Equals(a.Plan.FormId, plan.Id, StringComparison.OrdinalIgnoreCase))
                               .Select(a => a.Rawcode).ToList();
            sb.Append("// form ").Append(plan.Id).Append(" becomes unit ").Append(rawcode);
            if (own.Count > 0) sb.Append(" with abilities ").Append(string.Join(", ", own));
            sb.AppendLine();
        }
        sb.AppendLine();

        var damaging = abilities.Where(a => a.Plan.Damage.Length > 0).ToList();
        if (damaging.Count > 0)
        {
            sb.AppendLine("globals");
            // A hashtable rather than parallel arrays, because one shared timer table is the only
            // way a per-ability callback can recover its caster, position and level without a
            // global that two casts can overwrite.
            sb.AppendLine("    hashtable HeroGenHash = null");
            sb.AppendLine("endglobals");
            sb.AppendLine();
            sb.AppendLine("function HeroGen_EnsureHash takes nothing returns nothing");
            sb.AppendLine("    if HeroGenHash == null then");
            sb.AppendLine("        set HeroGenHash = InitHashtable()");
            sb.AppendLine("    endif");
            sb.AppendLine("endfunction");
            sb.AppendLine();
        }

        foreach (var (plan, rawcode) in forms)
        {
            sb.Append("function HeroGen_Transform_").Append(plan.Id)
              .AppendLine(" takes unit caster returns unit");
            sb.Append("    if GetUnitTypeId(caster) == '").Append(rawcode).AppendLine("' then");
            sb.AppendLine("        return caster");
            sb.AppendLine("    endif");
            // ReplaceUnitBJ carries hero level, experience and items, which CreateUnit plus
            // RemoveUnit does not, and the new unit type's own uabi supplies its ability list.
            sb.Append("    return ReplaceUnitBJ(caster, '").Append(rawcode)
              .AppendLine("', bj_UNIT_STATE_METHOD_RELATIVE)");
            sb.AppendLine("endfunction");
            sb.AppendLine();
        }

        foreach (var (plan, rawcode, _) in abilities)
        {
            if (plan.IsTransform)
            {
                var target = forms.First(f => string.Equals(f.Plan.Id, plan.To, StringComparison.OrdinalIgnoreCase));
                sb.Append("function HeroGen_Cast_").Append(rawcode)
                  .AppendLine(" takes unit caster returns unit");
                sb.Append("    return HeroGen_Transform_").Append(target.Plan.Id).AppendLine("(caster)");
                sb.AppendLine("endfunction");
                sb.AppendLine();
                continue;
            }
            if (plan.Damage.Length == 0) continue;

            sb.Append("function HeroGen_Damage_").Append(rawcode).AppendLine(" takes nothing returns nothing");
            sb.AppendLine("    local timer t = GetExpiredTimer()");
            sb.AppendLine("    local integer id = GetHandleId(t)");
            sb.AppendLine("    local unit caster = LoadUnitHandle(HeroGenHash, id, 0)");
            sb.AppendLine("    local real x = LoadReal(HeroGenHash, id, 1)");
            sb.AppendLine("    local real y = LoadReal(HeroGenHash, id, 2)");
            sb.AppendLine("    local integer level = LoadInteger(HeroGenHash, id, 3)");
            sb.Append("    local real damage = ").Append(JassReal(plan.Damage[0])).AppendLine();
            sb.AppendLine("    local group g = CreateGroup()");
            sb.AppendLine("    local unit u = null");
            for (int level = 1; level < plan.Damage.Length; level++)
            {
                sb.Append("    if level >= ").Append(level + 1).AppendLine(" then");
                sb.Append("        set damage = ").Append(JassReal(plan.Damage[level])).AppendLine();
                sb.AppendLine("    endif");
            }
            sb.Append("    call GroupEnumUnitsInRange(g, x, y, ").Append(JassReal(DefaultDamageRadius))
              .AppendLine(", null)");
            sb.AppendLine("    loop");
            sb.AppendLine("        set u = FirstOfGroup(g)");
            sb.AppendLine("        exitwhen u == null");
            sb.AppendLine("        if IsUnitEnemy(u, GetOwningPlayer(caster)) and GetUnitState(u, UNIT_STATE_LIFE) > 0.00 then");
            sb.AppendLine("            call UnitDamageTarget(caster, u, damage, true, false, ATTACK_TYPE_NORMAL, DAMAGE_TYPE_MAGIC, WEAPON_TYPE_WHOKNOWS)");
            sb.AppendLine("        endif");
            sb.AppendLine("        call GroupRemoveUnit(g, u)");
            sb.AppendLine("    endloop");
            sb.AppendLine("    call FlushChildHashtable(HeroGenHash, id)");
            sb.AppendLine("    call DestroyTimer(t)");
            sb.AppendLine("    call DestroyGroup(g)");
            sb.AppendLine("    set t = null");
            sb.AppendLine("    set caster = null");
            sb.AppendLine("    set g = null");
            sb.AppendLine("    set u = null");
            sb.AppendLine("endfunction");
            sb.AppendLine();

            sb.Append("function HeroGen_Cast_").Append(rawcode)
              .AppendLine(" takes unit caster, real x, real y, integer level returns nothing");
            sb.AppendLine("    local timer t = null");
            sb.AppendLine("    call HeroGen_EnsureHash()");
            sb.AppendLine("    set t = CreateTimer()");
            sb.AppendLine("    call SaveUnitHandle(HeroGenHash, GetHandleId(t), 0, caster)");
            sb.AppendLine("    call SaveReal(HeroGenHash, GetHandleId(t), 1, x)");
            sb.AppendLine("    call SaveReal(HeroGenHash, GetHandleId(t), 2, y)");
            sb.AppendLine("    call SaveInteger(HeroGenHash, GetHandleId(t), 3, level)");
            sb.Append("    call TimerStart(t, ").Append(JassReal(Math.Max(plan.Cast, 0.01)))
              .Append(", false, function HeroGen_Damage_").Append(rawcode).AppendLine(")");
            sb.AppendLine("    set t = null");
            sb.AppendLine("endfunction");
            sb.AppendLine();
        }

        sb.AppendLine("//=== end wc3ctl hero generate ===");
        return sb.ToString();
    }

    /// <summary>A double that always carries a decimal point, so a field added by inference is
    /// stored as an Unreal rather than an Int.</summary>
    private static string Real(double value) =>
        value.ToString("0.0##", CultureInfo.InvariantCulture);

    /// <summary>A double formatted for JASS, which wants two decimal places on reals.</summary>
    private static string JassReal(double value) =>
        value.ToString("0.00", CultureInfo.InvariantCulture);

    /// <summary>Short human form for tooltip text, without trailing zeros.</summary>
    private static string Number(double value) =>
        value.ToString("0.##", CultureInfo.InvariantCulture);

    private static HeroGenerateResult Fail(string message) => new(
        false, message,
        Array.Empty<HeroGeneratedUnit>(), Array.Empty<HeroGeneratedAbility>(),
        Array.Empty<string>(), "", Array.Empty<HeroObjectChange>(), Array.Empty<string>());

    private sealed class HeroSpec
    {
        public string? Name { get; set; }
        public string? BaseUnit { get; set; }
        public List<FormSpec>? Forms { get; set; }
    }

    private sealed class FormSpec
    {
        public string? Id { get; set; }
        public List<AbilitySpec>? Abilities { get; set; }
    }

    private sealed class AbilitySpec
    {
        public string? Slot { get; set; }
        public string? Name { get; set; }
        public string? Base { get; set; }
        public string? Kind { get; set; }
        public string? To { get; set; }
        public JsonElement Cooldown { get; set; }
        public double? Cast { get; set; }
        public JsonElement Damage { get; set; }
        public string? Tooltip { get; set; }
    }
}
