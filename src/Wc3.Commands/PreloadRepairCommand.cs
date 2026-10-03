// src/Wc3.Commands/PreloadRepairCommand.cs
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A trigger that fires soon after the loading screen, and the function it runs.</summary>
public sealed record PreloadEntry(string Trigger, double Seconds, string Function);

public sealed record PreloadResult(
    bool Ok,
    string Message,
    IReadOnlyList<PreloadEntry> Entries,
    int FunctionsWalked,
    IReadOnlyList<string> UnitTypes,
    IReadOnlyList<string> Abilities,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Moves the asset loading a map does in its first seconds of play under the loading screen.
///
/// Why. The engine loads a model, its textures and an ability's data the first time something
/// uses them, and it does that on the game thread, so the game freezes while it reads. A map that
/// creates units or adds abilities from a timer shortly after start pays that whole cost the moment
/// play begins. Anime WOS2 0.32f is the case this was written for. At 1 second its BuildsForChars
/// trigger creates a hidden dummy of 32 heroes and adds 213 abilities to them, which reads every
/// hero model out of a 285 MB archive at once.
///
/// How. Every trigger registered on a single timer at or below the window is an entry. From each
/// entry the script's call graph is walked, and every integer the walked functions reference that
/// names a unit type or ability the MAP defines is collected, through a literal rawcode or an
/// integer global that holds one. The script gains, as the first thing main does before
/// InitBlizzard and so before any trigger or unit exists, code that creates one unit of each type
/// and adds each ability to a carrier unit, and removes them again straight away. The engine keeps
/// what it loaded, so the timer finds everything already in memory. The map's own code is not
/// changed, so the timer still runs at the moment the author chose and does exactly what it did.
///
/// What it cannot know. How long the freeze was, since only the running game can measure it, and
/// whether a map reacts to the preload units existing. No trigger exists yet when they are created
/// and removed, which is why they are placed before InitBlizzard. The loading screen grows by
/// about the time the freeze took.
/// </summary>
public static class PreloadRepairCommand
{
    private const string Marker = "wc3ctl_preload";
    private const int CallsPerChunk = 150;

    private static readonly Regex FunctionHeader = new(@"^\s*(?:constant\s+)?function\s+(\w+)\s+takes\b",
        RegexOptions.Compiled);
    private static readonly Regex TimerSingle = new(
        @"TriggerRegisterTimerEventSingle\s*\(\s*(\w+)\s*,\s*([0-9]*\.?[0-9]+)\s*\)", RegexOptions.Compiled);
    private static readonly Regex AddAction = new(@"TriggerAddAction\s*\(\s*(\w+)\s*,\s*function\s+(\w+)\s*\)",
        RegexOptions.Compiled);
    private static readonly Regex Reference = new(@"(?:\bfunction\s+(\w+))|(?:ExecuteFunc\s*\(\s*""(\w+)""\s*\))|(?:\b(\w+)\s*\()",
        RegexOptions.Compiled);
    private static readonly Regex InitBlizzard = new(@"^\s*call\s+InitBlizzard\s*\(\s*\)\s*$", RegexOptions.Compiled);

    /// <param name="withinSeconds">A single-shot timer trigger at or below this is an entry.</param>
    /// <param name="functions">Extra entry functions named by the caller, for a map that starts
    /// its heavy work some other way.</param>
    public static PreloadResult Execute(MapDocument doc, bool apply, double withinSeconds = 5,
        IReadOnlyCollection<string>? functions = null, bool allUnits = false)
    {
        var diagnostics = new List<string>();
        PreloadResult Fail(string why) => new(false, why, Array.Empty<PreloadEntry>(), 0,
            Array.Empty<string>(), Array.Empty<string>(), diagnostics);

        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out var scriptName, out var script, out var why))
            return Fail(why);
        if (script.Contains(Marker, StringComparison.Ordinal))
            return Fail("the script already carries the preload, run this on the original map");

        var plan = Plan(doc, script, withinSeconds, functions ?? Array.Empty<string>(), allUnits);
        diagnostics.Add($"walked {plan.Walked} function(s) from {plan.Entries.Count} entr(ies)");
        string summary = plan.Units.Count + plan.Abilities.Count == 0
            ? "nothing to preload, no early timer reaches a unit type or ability the map defines"
            : $"{(apply ? "preloads" : "would preload")} {plan.Units.Count} unit type(s) and "
              + $"{plan.Abilities.Count} abilit(ies) under the loading screen";
        if (!apply || plan.Units.Count + plan.Abilities.Count == 0)
            return new(true, summary, plan.Entries, plan.Walked, plan.Units, plan.Abilities, diagnostics);

        string patched = Inject(script, plan.Units, plan.Abilities);
        doc.TryReplaceFileByName(scriptName, Encoding.Latin1.GetBytes(patched));
        diagnostics.Add("war3map.wct and war3map.wtg are unchanged, so reopening this map in the "
            + "World Editor and saving would regenerate the script without the preload");
        return new(true, summary, plan.Entries, plan.Walked, plan.Units, plan.Abilities, diagnostics);
    }

    internal sealed record PreloadPlan(
        IReadOnlyList<PreloadEntry> Entries, int Walked, IReadOnlyList<string> Units, IReadOnlyList<string> Abilities);

    internal static PreloadPlan Plan(MapDocument doc, string script, double withinSeconds, IReadOnlyCollection<string> extra,
        bool allUnits = false)
    {
        string nl = UabiRuntimeRepairCommand.LineEnding(script);
        var lines = script.Split(nl);

        // Function bodies by name, and integer globals that hold a rawcode.
        var bodies = new Dictionary<string, (int Start, int End)>(StringComparer.Ordinal);
        var globals = JassRawcodes.Globals(lines);
        for (int i = 0; i < lines.Length; i++)
        {
            if (FunctionHeader.Match(lines[i]) is not { Success: true } f) continue;
            int end = i + 1;
            while (end < lines.Length && lines[end].Trim() != "endfunction") end++;
            bodies.TryAdd(f.Groups[1].Value, (i, end));
        }

        var entries = new List<PreloadEntry>();
        var actions = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (Match m in AddAction.Matches(script))
            (actions.TryGetValue(m.Groups[1].Value, out var l) ? l : actions[m.Groups[1].Value] = new()).Add(m.Groups[2].Value);
        foreach (Match m in TimerSingle.Matches(script))
        {
            double t = double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
            if (t > withinSeconds || !actions.TryGetValue(m.Groups[1].Value, out var fs)) continue;
            foreach (var f in fs) entries.Add(new(m.Groups[1].Value, t, f));
        }
        foreach (var f in extra) entries.Add(new("(named)", 0, f));

        // Walk the call graph from every entry.
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>(entries.Select(e => e.Function));
        var tokens = new HashSet<string>(StringComparer.Ordinal);
        while (queue.Count > 0)
        {
            string fn = queue.Dequeue();
            if (!seen.Add(fn) || !bodies.TryGetValue(fn, out var span)) continue;
            for (int i = span.Start + 1; i < span.End; i++)
            {
                string code = StripComment(lines[i]);
                foreach (Match r in Reference.Matches(code))
                {
                    string callee = r.Groups[1].Success ? r.Groups[1].Value
                        : r.Groups[2].Success ? r.Groups[2].Value : r.Groups[3].Value;
                    if (bodies.ContainsKey(callee) && !seen.Contains(callee)) queue.Enqueue(callee);
                }
                tokens.UnionWith(JassRawcodes.In(code, globals));
            }
        }

        // Only what the map defines. Base game assets ship in the game's own storage and are not
        // what a 285 MB map spends its first second reading.
        var unitIds = JassRawcodes.MapDefined(doc, ObjectKind.Unit, @"Units\UnitData.slk");
        var abilityIds = JassRawcodes.MapDefined(doc, ObjectKind.Ability, @"Units\AbilityData.slk");
        var abilities = tokens.Where(abilityIds.Contains).Order(StringComparer.Ordinal).ToList();

        // Every unit type the script names anywhere, for a map that spawns new types all game.
        // A tower defense loads each wave's model the moment that wave first spawns, mid-game.
        // Abilities stay limited to the early walk, since a map can name thousands.
        if (allUnits)
            foreach (var line in lines)
                tokens.UnionWith(JassRawcodes.In(StripComment(line), globals));
        var units = tokens.Where(unitIds.Contains).Order(StringComparer.Ordinal).ToList();
        return new(entries, seen.Count(bodies.ContainsKey), units, abilities);
    }

    /// <summary>
    /// The script with the preload functions before main and their calls as main's first
    /// statements, ahead of InitBlizzard, so no trigger and no preplaced unit exists yet.
    /// </summary>
    internal static string Inject(string script, IReadOnlyList<string> units, IReadOnlyList<string> abilities)
    {
        string nl = UabiRuntimeRepairCommand.LineEnding(script);
        var lines = script.Split(nl).ToList();
        int main = lines.FindIndex(l => UabiRuntimeRepairCommand.MainHeader.IsMatch(l.TrimEnd()));
        if (main < 0) throw new InvalidDataException("the script has no 'function main takes nothing returns nothing'");
        int mainEnd = lines.FindIndex(main + 1, l => l.Trim() == "endfunction");
        if (mainEnd < 0) throw new InvalidDataException("function main has no endfunction");

        int body = main + 1;
        while (body < mainEnd && (lines[body].TrimStart().StartsWith("local ", StringComparison.Ordinal)
                                  || lines[body].Trim().Length == 0 || lines[body].TrimStart().StartsWith("//")))
            body++;
        int blizz = lines.FindIndex(body, mainEnd - body, l => InitBlizzard.IsMatch(l));
        int callAt = blizz >= 0 ? blizz : body;

        // One unit per type, then every ability on a carrier, in chunks that each run on their
        // own thread through ExecuteFunc so no chunk nears the operation limit.
        var statements = new List<string>();
        string place = "GetRectCenterX(GetWorldBounds()), GetRectCenterY(GetWorldBounds()), 0.0";
        string owner = "Player(PLAYER_NEUTRAL_PASSIVE)";
        foreach (var u in units)
            statements.Add($"    set u = CreateUnit({owner}, '{u}', {place})\n    call RemoveUnit(u)");
        var chunks = statements.Chunk(CallsPerChunk).Select(c => c.ToList()).ToList();
        string carrier = units.Count > 0 ? units[0] : "hfoo";
        foreach (var group in abilities.Chunk(CallsPerChunk))
            chunks.Add(new[] { $"    set u = CreateUnit({owner}, '{carrier}', {place})" }
                .Concat(group.Select(a => $"    call UnitAddAbility(u, '{a}')"))
                .Append("    call RemoveUnit(u)").ToList());

        var fns = new List<string>
        {
            "//===========================================================================",
            $"// {Marker}. Written by wc3ctl repair preload. Loads the models and abilities the map",
            "// uses in its first seconds while the loading screen is still up, then removes them.",
            "//===========================================================================",
        };
        for (int i = 0; i < chunks.Count; i++)
        {
            fns.Add($"function {Marker}_{i + 1} takes nothing returns nothing");
            fns.Add("    local unit u");
            fns.AddRange(chunks[i].SelectMany(s => s.Split('\n')));
            fns.Add("    set u = null");
            fns.Add("endfunction");
            fns.Add("");
        }

        var calls = Enumerable.Range(1, chunks.Count).Select(i => $"    call ExecuteFunc(\"{Marker}_{i}\")").ToList();
        lines.InsertRange(callAt, calls);
        lines.InsertRange(main, fns);
        return string.Join(nl, lines);
    }

    /// <summary>$6E303135 as the rawcode 'n015', when all four bytes are printable.</summary>
    internal static string? FromHex(string hex) => JassRawcodes.FromHex(hex);

    private static string StripComment(string line) => JassRawcodes.StripComment(line);
}
