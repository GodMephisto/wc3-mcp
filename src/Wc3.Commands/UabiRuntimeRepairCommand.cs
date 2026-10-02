// src/Wc3.Commands/UabiRuntimeRepairCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A unit type whose uabi list stays in the object data, and why.</summary>
public sealed record UabiKept(string Rawcode, string Reason);

public sealed record UabiRuntimeResult(
    bool Ok,
    string Message,
    int UnitTypesWithUabi,
    int UnitTypesMoved,
    int ReferencesMoved,
    int DistinctBefore,
    int DistinctAfter,
    IReadOnlyList<UabiKept> Kept,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Moves each unit type's normal ability list (uabi) out of the object data and into the map
/// script, which adds the same abilities to every unit the moment it is created.
///
/// Why. Players report that a map whose units carry about 2,000 DISTINCT ability ids in uabi
/// disconnects almost every game, that 20 distinct ids over the same 2,000 references does not,
/// and that the same abilities added by trigger do not either. That is the report's claim, not
/// something measured here, and the three basemaps in the repository root are the experiment
/// built to test it. This is the fix that report names as the strongest, done mechanically.
///
/// How. Every unit type with a uabi set by the map gets that list saved into a hashtable keyed
/// by unit type, and uabi cleared. The script gains a trigger on a region covering the whole
/// map, registered at the top of main before any unit exists, so every unit created afterwards
/// runs it. The engine fires that event inside CreateUnit, before the call returns, which is
/// what unit indexers depend on, so a trigger that creates a unit and then reads its ability
/// on the next line still finds it. A sweep before the map's initialization triggers catches
/// anything the event missed, and a second add of the same ability is a no-op.
/// UnitMakeAbilityPermanent keeps each ability through a morph.
///
/// What stays. Hero abilities (uhab), which are learned and not granted. Any unit type named
/// by an ability's data, because a morph or transformation switches a unit into that type
/// without creating it, so no event would ever give it its list. Locust (Aloc), whose behaviour
/// differs when it is added after creation. Anything the map itself does not set.
///
/// What it cannot know. Whether the map's own code depends on an ability being present at an
/// earlier moment than creation. A clean pjass parse proves the script is valid, not that the
/// map plays the same, so the result has to be play-tested before it is distributed.
/// </summary>
public static class UabiRuntimeRepairCommand
{
    private const string Marker = "wc3ctl_uabi";
    private const int CallsPerChunk = 400;       // well under the per-thread operation limit
    private static readonly HashSet<string> KeepInData = new(StringComparer.Ordinal) { "Aloc" };
    private static readonly Regex Rawcode = new(@"\b[A-Za-z0-9]{4}\b", RegexOptions.Compiled);
    private static readonly Regex RunInit = new(@"^\s*call\s+RunInitializationTriggers\s*\(\s*\)\s*$",
        RegexOptions.Compiled);
    internal static readonly Regex MainHeader = new(@"^function\s+main\s+takes\s+nothing\s+returns\s+nothing\s*$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    /// <param name="only">When given, move only these ability ids and leave every other entry in
    /// uabi, so one suspect ability can be isolated in a bisection (AInv, the inventory 3.0.0
    /// reworked, is the case this was added for). Null moves everything.</param>
    public static UabiRuntimeResult Execute(MapDocument doc, bool apply, IReadOnlyCollection<string>? only = null)
    {
        // Only a real 4 character id can be added by script. Anything else, a typo such as
        // 'A0S4m' included, stays in the list exactly as the author wrote it.
        bool Moves(string id) => id.Length == 4 && !KeepInData.Contains(id)
            && (only is null || only.Count == 0 || only.Contains(id, StringComparer.Ordinal));
        ArgumentNullException.ThrowIfNull(doc);
        var diagnostics = new List<string>();
        var kept = new List<UabiKept>();
        var info = ObjectKinds.Info(ObjectKind.Unit);
        var units = ObjectKinds.MergedEntries(doc, info);

        // Every 4 character token any ability's data mentions. A unit type in here can be
        // entered by morphing, which creates nothing and so fires no event.
        var namedByAbilities = new HashSet<string>(StringComparer.Ordinal);
        foreach (var a in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability)))
            foreach (var (_, v) in a.Mods)
                if (v is not null)
                    foreach (Match m in Rawcode.Matches(v)) namedByAbilities.Add(m.Value);

        var plan = new List<(string Unit, List<string> Move, List<string> Stay)>();
        var before = new HashSet<string>(StringComparer.Ordinal);
        var after = new HashSet<string>(StringComparer.Ordinal);
        int withUabi = 0;
        foreach (var u in units)
        {
            var mods = ObjectKinds.ModsToDict(u.Mods);
            if (!mods.TryGetValue("uabi", out var list) || string.IsNullOrWhiteSpace(list)) continue;
            var ids = list.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
            if (ids.Count == 0) continue;
            withUabi++;
            before.UnionWith(ids);
            string raw = u.Id.ToRawcode();
            if (namedByAbilities.Contains(raw))
            {
                kept.Add(new(raw, "an ability names this unit type, so a morph can enter it without creating it"));
                after.UnionWith(ids);
                continue;
            }
            var stay = ids.Where(i => !Moves(i)).ToList();
            var move = ids.Where(Moves).ToList();
            after.UnionWith(stay);
            if (move.Count > 0) plan.Add((raw, move, stay));
        }

        int refs = plan.Sum(p => p.Move.Count);
        string summary = $"{plan.Count} of {withUabi} unit type(s) with uabi, {refs} reference(s), "
            + $"distinct ids in uabi {before.Count} before, {after.Count} after";
        if (!apply || plan.Count == 0)
            return new(true, summary, withUabi, plan.Count, refs, before.Count, after.Count, kept, diagnostics);

        if (!TryReadScript(doc, out var scriptName, out var script, out var why))
            return new(false, why, withUabi, 0, 0, before.Count, before.Count, kept, diagnostics);
        if (script.Contains(Marker, StringComparison.Ordinal))
            return new(false, "the script already carries this repair (found " + Marker + ")",
                withUabi, 0, 0, before.Count, before.Count, kept, diagnostics);

        string patched;
        try { patched = Inject(script, plan.Select(p => (p.Unit, (IReadOnlyList<string>)p.Move)).ToList()); }
        catch (InvalidDataException e)
        {
            return new(false, e.Message, withUabi, 0, 0, before.Count, before.Count, kept, diagnostics);
        }

        foreach (var (unit, _, stay) in plan)
            ClearEverywhere(doc, info, unit.FromRawcode(), string.Join(",", stay));
        doc.TryReplaceFileByName(scriptName, Encoding.Latin1.GetBytes(patched));
        diagnostics.Add($"{scriptName} gained the runtime adder, {refs} ability id(s) in "
            + $"{(refs + CallsPerChunk - 1) / CallsPerChunk} data function(s)");
        diagnostics.Add("war3map.wct and war3map.wtg are unchanged, so reopening this map in the "
            + "World Editor and saving would regenerate the script without the adder");
        return new(true, summary, withUabi, plan.Count, refs, before.Count, after.Count, kept, diagnostics);
    }

    /// <summary>
    /// The script with the adder inserted. One global before endglobals, the functions before
    /// main, a call to the init as main's first statement after its locals, and a sweep before
    /// RunInitializationTriggers, or at the end of main when the map has none.
    /// </summary>
    internal static string Inject(string script, IReadOnlyList<(string Unit, IReadOnlyList<string> Abilities)> plan)
    {
        string nl = LineEnding(script);
        var lines = script.Split(nl).ToList();

        int endGlobals = lines.FindIndex(l => l.Trim() == "endglobals");
        if (endGlobals < 0)
        {
            lines.InsertRange(0, new[] { "globals", "endglobals" });
            endGlobals = 1;
        }
        lines.Insert(endGlobals, $"    hashtable {Marker} = null");

        int main = lines.FindIndex(l => MainHeader.IsMatch(l.TrimEnd()));
        if (main < 0) throw new InvalidDataException("the script has no 'function main takes nothing returns nothing'");
        int mainEnd = lines.FindIndex(main + 1, l => l.Trim() == "endfunction");
        if (mainEnd < 0) throw new InvalidDataException("function main has no endfunction");

        int body = main + 1;
        while (body < mainEnd && (lines[body].TrimStart().StartsWith("local ", StringComparison.Ordinal)
                                  || lines[body].Trim().Length == 0 || lines[body].TrimStart().StartsWith("//")))
            body++;
        // The World Editor writes "call RunInitializationTriggers(  )", spaces inside the
        // parentheses, so an exact match missed every editor-saved map and the sweep fell
        // through to the end of main.
        int runInit = lines.FindIndex(body, mainEnd - body, l => RunInit.IsMatch(l));
        int sweepAt = runInit >= 0 ? runInit : mainEnd;
        lines.Insert(sweepAt, $"    call {Marker}_Sweep()");
        lines.Insert(body, $"    call {Marker}_Init()");
        lines.InsertRange(main, Functions(plan));
        return string.Join(nl, lines);
    }

    /// <summary>
    /// The separator the script mostly uses. Optimised scripts are often bare CR, measured on
    /// BleachVsOnepiece17 at 138,400 bare CR against 87 CRLF (the CRLF all inside one string
    /// literal), so assuming CRLF or LF from its mere presence split the file wrongly and main
    /// could not be found.
    /// </summary>
    internal static string LineEnding(string s)
    {
        int crlf = 0, cr = 0, lf = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '\r')
            {
                if (i + 1 < s.Length && s[i + 1] == '\n') { crlf++; i++; }
                else cr++;
            }
            else if (s[i] == '\n') lf++;
        }
        return crlf >= cr && crlf >= lf ? "\r\n" : cr >= lf ? "\r" : "\n";
    }

    private static List<string> Functions(IReadOnlyList<(string Unit, IReadOnlyList<string> Abilities)> plan)
    {
        var f = new List<string>
        {
            "// Added by wc3ctl repair uabi-runtime. Each unit type's normal abilities (uabi) were",
            "// moved out of the object data and are added here the moment a unit is created.",
        };
        var calls = new List<string>();
        foreach (var (unit, abilities) in plan)
        {
            long t = AuditCommand.BigEndian(unit);
            calls.Add($"    call SaveInteger({Marker}, {t}, 0, {abilities.Count})");
            for (int i = 0; i < abilities.Count; i++)
                calls.Add($"    call SaveInteger({Marker}, {t}, {i + 1}, {AuditCommand.BigEndian(abilities[i])})");
        }
        int chunks = 0;
        for (int i = 0; i < calls.Count; i += CallsPerChunk)
        {
            chunks++;
            f.Add($"function {Marker}_Data{chunks} takes nothing returns nothing");
            f.AddRange(calls.Skip(i).Take(CallsPerChunk));
            f.Add("endfunction");
        }
        f.AddRange(new[]
        {
            $"function {Marker}_Add takes unit u returns nothing",
            $"    local integer t = GetUnitTypeId(u)",
            $"    local integer n = LoadInteger({Marker}, t, 0)",
            $"    local integer i = 1",
            $"    local integer a",
            $"    loop",
            $"        exitwhen i > n",
            $"        set a = LoadInteger({Marker}, t, i)",
            $"        if UnitAddAbility(u, a) then",
            $"            call UnitMakeAbilityPermanent(u, true, a)",
            $"        endif",
            $"        set i = i + 1",
            $"    endloop",
            $"endfunction",
            $"function {Marker}_Enter takes nothing returns boolean",
            $"    call {Marker}_Add(GetTriggerUnit())",
            $"    return false",
            $"endfunction",
            $"function {Marker}_Each takes nothing returns nothing",
            $"    call {Marker}_Add(GetEnumUnit())",
            $"endfunction",
            $"function {Marker}_Sweep takes nothing returns nothing",
            $"    local group g = CreateGroup()",
            $"    call GroupEnumUnitsInRect(g, GetWorldBounds(), null)",
            $"    call ForGroup(g, function {Marker}_Each)",
            $"    call DestroyGroup(g)",
            $"    set g = null",
            $"endfunction",
            $"function {Marker}_Init takes nothing returns nothing",
            $"    local trigger t = CreateTrigger()",
            $"    local region r = CreateRegion()",
            $"    set {Marker} = InitHashtable()",
        });
        for (int c = 1; c <= chunks; c++)
            f.Add($"    call ExecuteFunc(\"{Marker}_Data{c}\")");
        f.AddRange(new[]
        {
            $"    call RegionAddRect(r, GetWorldBounds())",
            $"    call TriggerRegisterEnterRegion(t, r, null)",
            $"    call TriggerAddCondition(t, Condition(function {Marker}_Enter))",
            $"    set t = null",
            $"    set r = null",
            $"endfunction",
        });
        return f;
    }

    /// <summary>Sets uabi on this unit in every object layer that holds it.</summary>
    private static void ClearEverywhere(MapDocument doc, ObjectKindInfo info, int id, string value)
    {
        int field = "uabi".FromRawcode();
        foreach (var file in new[] { info.MapFile, info.SkinFile })
        {
            if (doc.GetFile(file)?.Model is not { } model) continue;
            var access = ObjectDataWriter.AccessFor(model);
            if (access?.FindGroup(id) is not { } group) continue;
            if (access.FindMod(group, field, 0) is not { } mod) continue;
            mod.Value = value;
            doc.AddOrReplaceModelFile(file, model);
        }
    }

    internal static bool TryReadScript(MapDocument doc, out string name, out string text, out string why)
    {
        foreach (var n in new[] { "war3map.j", @"scripts\war3map.j" })
            if (doc.TryReadFileByName(n, out var bytes) && bytes.Length > 0)
            {
                name = n; text = Encoding.Latin1.GetString(bytes); why = "";
                return true;
            }
        name = ""; text = "";
        why = doc.HasFileByName("war3map.lua") || doc.HasFileByName(@"scripts\war3map.lua")
            ? "this is a Lua map, and the adder is written in JASS"
            : "no war3map.j found";
        return false;
    }
}
