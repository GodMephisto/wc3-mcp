// src/Wc3.Commands/UnitAbilitiesCommand.cs
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Where a unit gets an ability from.</summary>
public enum AbilitySource
{
    /// <summary>The unit type's normal ability list (uabi).</summary>
    ObjectNormal,
    /// <summary>The unit type's hero ability list (uhab).</summary>
    ObjectHero,
    /// <summary>Listed inside a spellbook the unit has. Detail names the spellbook.</summary>
    Spellbook,
    /// <summary>Set on one placed unit in war3mapUnits.doo, with its level.</summary>
    PlacedUnit,
    /// <summary>Carried by a unit type this unit turns into. Detail names the form.</summary>
    MorphForm,
    /// <summary>Given by the map script. Inferred, Detail is "war3map.j:LINE".</summary>
    Script,
}

/// <summary>One ability a unit has, with where it comes from.</summary>
/// <param name="Detail">The spellbook or form rawcode, or "war3map.j:LINE" for a script ability.</param>
/// <param name="Level">The level set on a placed unit. Null for every other source.</param>
/// <param name="Autocast">Whether a placed unit has autocast on. Null for every other source.</param>
/// <param name="Inferred">True for script abilities, which are read from code rather than data.</param>
public sealed record UnitAbility(
    string Rawcode, string? Name, AbilitySource Source, string? Detail = null,
    int? Level = null, bool? Autocast = null, bool Inferred = false);

/// <summary>Every ability a unit has, grouped by where it comes from.</summary>
public sealed record UnitAbilitiesResult(
    string Unit, bool Found, string? Name, int? PlacedCreationNumber,
    IReadOnlyList<UnitAbility> Abilities, IReadOnlyList<string> Diagnostics);

/// <summary>
/// The abilities a unit has, from every place the game gives them. The unit type's own lists
/// (uabi, uhab) are only part of it. Passives often sit inside a spellbook, a placed unit can carry
/// its own abilities and levels, a morph ability brings the abilities of the form, and many maps
/// hand out abilities from the script at runtime. Each entry says which of these it came from, and
/// a script entry is marked inferred, because only the running game knows for sure.
/// </summary>
public static class UnitAbilitiesCommand
{
    private const string SpellbookBase = "Aspb";
    private const string SpellListField = "spb1";
    private const string LevelsField = "alev";

    // A morph ability names the form it turns into in a field of this metadata type.
    private const string UnitCodeType = "unitCode";

    /// <summary>The unit type's abilities, or a placed unit's when <paramref name="placedCreationNumber"/> is set.</summary>
    public static UnitAbilitiesResult Execute(
        MapDocument doc, string unitRawcode, string? gameDir, int? placedCreationNumber = null)
    {
        var diagnostics = new List<string>();
        PlacedUnit? placed = null;
        if (placedCreationNumber is { } cn)
        {
            placed = FindPlaced(doc, cn);
            if (placed is null)
                return new(unitRawcode, false, null, cn, Array.Empty<UnitAbility>(),
                    new[] { $"no placed unit with creation number {cn}" });
            unitRawcode = placed.TypeId.ToRawcode();
        }
        if (unitRawcode is null || unitRawcode.Length != 4)
            return new(unitRawcode ?? "", false, null, placedCreationNumber, Array.Empty<UnitAbility>(),
                new[] { "a unit rawcode has four characters" });

        var unit = ObjectGetCommand.Execute(doc, ObjectKind.Unit, unitRawcode, gameDir);
        if (!unit.Found)
            return new(unitRawcode, false, null, placedCreationNumber, Array.Empty<UnitAbility>(), unit.Diagnostics);

        var names = new Dictionary<string, MergedObjectResult>(StringComparer.Ordinal);
        MergedObjectResult Ability(string raw) =>
            names.TryGetValue(raw, out var r) ? r : names[raw] = ObjectGetCommand.Execute(doc, ObjectKind.Ability, raw, gameDir);
        string? Name(string raw) => Ability(raw) is { Found: true } a ? a.Name : null;

        var result = new List<UnitAbility>();
        var keys = new HashSet<(string, AbilitySource)>();
        var exact = new HashSet<string>(StringComparer.Ordinal);
        void Add(string raw, AbilitySource source, string? detail = null, int? level = null, bool? autocast = null,
            bool inferred = false)
        {
            if (!keys.Add((raw, source))) return;
            inferred |= source == AbilitySource.Script;
            if (!inferred) exact.Add(raw);
            result.Add(new UnitAbility(raw, Name(raw), source, detail, level, autocast, inferred));
        }

        var own = new List<string>();
        foreach (var raw in Rawcodes(FieldValue(unit, "uabi"))) { Add(raw, AbilitySource.ObjectNormal); own.Add(raw); }
        foreach (var raw in Rawcodes(FieldValue(unit, "uhab"))) { Add(raw, AbilitySource.ObjectHero); own.Add(raw); }

        if (placed?.AbilityData is { } mods)
            foreach (var m in mods)
            {
                var raw = m.AbilityId.ToRawcode();
                Add(raw, AbilitySource.PlacedUnit, level: m.HeroAbilityLevel, autocast: m.IsAutocastActive);
                own.Add(raw);
            }

        // Spellbook contents, then forms, for everything the unit itself carries.
        foreach (var raw in own.Distinct().ToList())
            ExpandSpellbook(raw, new HashSet<string>(StringComparer.Ordinal));
        foreach (var raw in own.Distinct().ToList())
            foreach (var form in FormsOf(Ability(raw), gameDir))
            {
                var formUnit = ObjectGetCommand.Execute(doc, ObjectKind.Unit, form, gameDir);
                if (!formUnit.Found) continue;
                foreach (var a in Rawcodes(FieldValue(formUnit, "uabi")).Concat(Rawcodes(FieldValue(formUnit, "uhab"))))
                    Add(a, AbilitySource.MorphForm, detail: form);
            }

        // The script last, listing only what no exact source already gave.
        if (UabiRuntimeRepairCommand.TryReadScript(doc, out _, out var script, out var why))
        {
            var index = ScriptIndex.For(doc, script, gameDir);
            if (index.ByUnit.TryGetValue(unitRawcode, out var hits))
            {
                var given = new List<string>();
                foreach (var (ability, line) in hits)
                    if (!exact.Contains(ability)) { Add(ability, AbilitySource.Script, detail: $"war3map.j:{line}"); given.Add(ability); }
                // A spellbook the script hands out holds abilities too. They are as inferred as the book.
                foreach (var raw in given)
                    ExpandSpellbook(raw, new HashSet<string>(StringComparer.Ordinal), inferred: true);
            }
        }
        else diagnostics.Add($"script not read, so script abilities are not listed ({why})");

        return new(unitRawcode, true, unit.Name, placedCreationNumber, result, diagnostics.Concat(unit.Diagnostics).ToList());

        void ExpandSpellbook(string raw, HashSet<string> path, bool inferred = false)
        {
            var book = Ability(raw);
            if (!book.Found || !string.Equals(book.BaseRawcode, SpellbookBase, StringComparison.Ordinal)) return;
            if (!path.Add(raw)) return; // a spellbook that lists itself, directly or through another
            foreach (var inner in LevelledValues(book, SpellListField).SelectMany(Rawcodes).Distinct())
            {
                Add(inner, AbilitySource.Spellbook, detail: raw, inferred: inferred);
                ExpandSpellbook(inner, path, inferred);
            }
            path.Remove(raw);
        }
    }

    /// <summary>A short label for where an ability came from, shared by every front-end.</summary>
    public static string SourceLabel(AbilitySource source) => source switch
    {
        AbilitySource.ObjectNormal => "unit data, normal",
        AbilitySource.ObjectHero => "unit data, hero",
        AbilitySource.Spellbook => "inside a spellbook",
        AbilitySource.PlacedUnit => "set on this placed unit",
        AbilitySource.MorphForm => "from a morph form",
        AbilitySource.Script => "given by the script, inferred",
        _ => source.ToString(),
    };

    /// <summary>One line for an ability, such as "A0GD  Raiden G  (war3map.j:39679)".</summary>
    public static string Describe(UnitAbility a)
    {
        var extras = new List<string>();
        if (a.Detail is not null) extras.Add(a.Detail);
        if (a.Level is { } level) extras.Add($"level {level}");
        if (a.Autocast == true) extras.Add("autocast on");
        if (a.Inferred && a.Source != AbilitySource.Script) extras.Add("inferred");
        return $"{a.Rawcode}  {a.Name ?? "(no name)"}" + (extras.Count > 0 ? $"  ({string.Join(", ", extras)})" : "");
    }

    /// <summary>
    /// The values a levelled field holds at the levels the ability actually has. The bare code is
    /// the base game's default and is used only when no level entry exists, because for a spellbook
    /// it is Blizzard's sample list rather than the map's.
    /// </summary>
    internal static IEnumerable<string> LevelledValues(MergedObjectResult ability, string code)
    {
        int levels = int.TryParse(FieldValue(ability, LevelsField), out var n) && n > 0 ? n : 1;
        var byLevel = new List<string>();
        for (int level = 1; level <= levels; level++)
            if (ability.Fields.FirstOrDefault(f => f.Code.Equals($"{code}:{level}", StringComparison.OrdinalIgnoreCase)) is { } f
                && IsValue(f.Value))
                byLevel.Add(f.Value);
        bool anyLevelEntry = ability.Fields.Any(f => f.Code.StartsWith(code + ":", StringComparison.OrdinalIgnoreCase));
        if (anyLevelEntry) return byLevel;
        return FieldValue(ability, code) is { } bare && IsValue(bare) ? new[] { bare } : Array.Empty<string>();
    }

    // The unit types an ability turns its owner into, found by field type rather than by name.
    private static IEnumerable<string> FormsOf(MergedObjectResult ability, string? gameDir)
    {
        if (!ability.Found || !GameData.GameData.TryOpen(gameDir, out var ctx, out _) || ctx is null) yield break;
        var codes = ability.Fields
            .Select(f => BareCode(f.Code)).Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(c => ctx.Abilities.FieldMetadata.TryGet(c, out var meta)
                        && string.Equals(meta.Type, UnitCodeType, StringComparison.OrdinalIgnoreCase));
        foreach (var code in codes)
            foreach (var value in LevelledValues(ability, code))
                foreach (var raw in Rawcodes(value))
                    yield return raw;
    }

    private static PlacedUnit? FindPlaced(MapDocument doc, int creationNumber) =>
        doc.GetFile(PlacementCommand.UnitsFile)?.Model is MapUnits units
            ? units.Units.FirstOrDefault(u => u.CreationNumber == creationNumber) is { } u ? new PlacedUnit(u) : null
            : null;

    private sealed record PlacedUnit(UnitData Data)
    {
        public int TypeId => Data.TypeId;
        public IReadOnlyList<ModifiedAbilityData>? AbilityData => Data.AbilityData;
    }

    private static string? FieldValue(MergedObjectResult obj, string code) =>
        obj.Fields.FirstOrDefault(f => f.Code.Equals(code, StringComparison.OrdinalIgnoreCase))?.Value;

    private static bool IsValue(string? v) => !string.IsNullOrWhiteSpace(v) && v != "-" && v != "_";

    /// <summary>A comma list of rawcodes, without the World Editor's empty-slot marker.</summary>
    internal static IEnumerable<string> Rawcodes(string? value) =>
        (value ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => s.Length == 4 && s != "_");

    private static string BareCode(string code)
    {
        int c = code.IndexOf(':');
        return c < 0 ? code : code[..c];
    }

    /// <summary>
    /// Which abilities the script ties to which unit types, built once per map and reused while
    /// the script text is unchanged. An ability is tied to a unit type when
    /// (a) one statement names both, as in HeroData_create(Raiden_ID, ..., RaidenF_ID), or
    /// (b) a statement names it inside an if or elseif whose condition names the unit type.
    /// </summary>
    internal sealed class ScriptIndex
    {
        private static readonly ConditionalWeakTable<MapDocument, ScriptIndex> Cache = new();

        private static readonly Regex If = new(@"^\s*(?:if|elseif)\b(.*)\bthen\b", RegexOptions.Compiled);
        private static readonly Regex Else = new(@"^\s*else\s*$", RegexOptions.Compiled);
        private static readonly Regex EndIf = new(@"^\s*endif\b", RegexOptions.Compiled);
        private static readonly Regex ElseIf = new(@"^\s*elseif\b", RegexOptions.Compiled);
        private static readonly Regex FunctionEdge = new(@"^\s*(?:constant\s+)?(?:function\b|endfunction\b)", RegexOptions.Compiled);
        private string _script = "";

        /// <summary>Per unit type, each tied ability with the first line that ties it.</summary>
        public Dictionary<string, List<(string Ability, int Line)>> ByUnit { get; } = new(StringComparer.Ordinal);

        public static ScriptIndex For(MapDocument doc, string script, string? gameDir)
        {
            if (Cache.TryGetValue(doc, out var cached) && string.Equals(cached._script, script, StringComparison.Ordinal))
                return cached;
            var built = Build(doc, script, gameDir);
            Cache.AddOrUpdate(doc, built);
            return built;
        }

        internal static ScriptIndex Build(MapDocument doc, string script, string? gameDir)
        {
            var units = JassRawcodes.MapDefined(doc, ObjectKind.Unit, @"Units\UnitData.slk");
            var abilities = JassRawcodes.MapDefined(doc, ObjectKind.Ability, @"Units\AbilityData.slk");
            if (GameData.GameData.TryOpen(gameDir, out var ctx, out _) && ctx is not null)
            {
                units.UnionWith(ctx.Units.Rawcodes);
                abilities.UnionWith(ctx.Abilities.Rawcodes);
            }
            return Build(script, units, abilities);
        }

        internal static ScriptIndex Build(string script, IReadOnlySet<string> units, IReadOnlySet<string> abilities)
        {
            var index = new ScriptIndex { _script = script };
            var lines = script.Split('\n');
            var globals = JassRawcodes.Globals(lines);
            var seen = new HashSet<(string, string)>();
            // One frame per open if. Each frame holds the unit types its live branch tests for.
            var frames = new List<HashSet<string>>();

            for (int i = 0; i < lines.Length; i++)
            {
                string code = JassRawcodes.StripComment(lines[i]).TrimEnd('\r');
                if (FunctionEdge.IsMatch(code)) { frames.Clear(); continue; }

                var named = JassRawcodes.In(code, globals).ToList();
                var lineUnits = named.Where(units.Contains).ToHashSet(StringComparer.Ordinal);
                var lineAbilities = named.Where(abilities.Contains).Where(a => !lineUnits.Contains(a)).ToList();

                if (If.Match(code) is { Success: true } m)
                {
                    var tested = JassRawcodes.In(m.Groups[1].Value, globals).Where(units.Contains)
                        .ToHashSet(StringComparer.Ordinal);
                    if (ElseIf.IsMatch(code) && frames.Count > 0) frames[^1] = tested;
                    else frames.Add(tested);
                }
                else if (Else.IsMatch(code) && frames.Count > 0) frames[^1] = new HashSet<string>(StringComparer.Ordinal);
                else if (EndIf.IsMatch(code) && frames.Count > 0) frames.RemoveAt(frames.Count - 1);

                // (a) one statement names the unit type and the ability.
                foreach (var u in lineUnits)
                    foreach (var a in lineAbilities)
                        index.Tie(u, a, i + 1, seen);

                // (b) a statement inside a branch that tests for the unit type. Any call counts, not
                // only UnitAddAbility, because maps hand out abilities through their own helpers,
                // such as GGG_ShadowRegisterSpellbookForHero(p, u, 'ky01') under heroCode == 'H01Z'.
                if (lineAbilities.Count > 0 && !If.IsMatch(code))
                    foreach (var u in frames.SelectMany(f => f).Distinct())
                        foreach (var a in lineAbilities)
                            index.Tie(u, a, i + 1, seen);
            }
            return index;
        }

        private void Tie(string unit, string ability, int line, HashSet<(string, string)> seen)
        {
            if (!seen.Add((unit, ability))) return;
            (ByUnit.TryGetValue(unit, out var list) ? list : ByUnit[unit] = new()).Add((ability, line));
        }
    }
}
