using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One function's static engine cost.</summary>
public sealed record ScriptBenchRow(
    string Function, int Line, double? Interval, long PerSecond, int Cost,
    int CreateUnit, int SpecialEffect, int GroupEnum, int ConditionAndFilter, int Damage,
    string Leaks);

public sealed record ScriptBenchResult(
    bool Ok, string Message, string File, int Functions, int Periodic, IReadOnlyList<ScriptBenchRow> Rows);

/// <summary>
/// Static, headless cost ranking for every function in a map script. No game client. It finds the
/// periodic timer callbacks, counts the engine operations each one performs per tick, and orders
/// them by cost per second, so an ability can be triaged before a play test. The weights are
/// relative engine-call costs, not a frame-time measurement.
/// </summary>
public static class ScriptBenchCommand
{
    private static readonly Regex TimerLiteral = new(
        @"TimerStart\(\s*[^,]+,\s*([0-9.]+)\s*,\s*(true|false)\s*,\s*function\s+(\w+)", RegexOptions.Compiled);
    private static readonly Regex TimerEvent = new(
        @"TriggerRegisterTimerEventPeriodic\([^,]+,\s*([0-9.]+)\s*\)", RegexOptions.Compiled);

    private static readonly (string Key, Regex Pattern, int Weight)[] Ops =
    {
        ("create", new Regex(@"CreateUnit\(", RegexOptions.Compiled), 10),
        ("cond", new Regex(@"Condition\(", RegexOptions.Compiled), 8),
        ("filter", new Regex(@"Filter\(", RegexOptions.Compiled), 8),
        ("enum", new Regex(@"GroupEnum\w+\(", RegexOptions.Compiled), 6),
        ("sfx", new Regex(@"AddSpecialEffect(?:Target)?\(", RegexOptions.Compiled), 5),
        ("select", new Regex(@"SelectUnit|ClearSelection", RegexOptions.Compiled), 6),
        ("frame", new Regex(@"BlzFrame|BlzGetFrame|BlzSetFrame", RegexOptions.Compiled), 6),
        ("dmg", new Regex(@"UnitDamageTarget\(", RegexOptions.Compiled), 3),
        ("tag", new Regex(@"CreateTextTag", RegexOptions.Compiled), 4),
        ("sound", new Regex(@"MakeSound\(", RegexOptions.Compiled), 3),
        ("move", new Regex(@"SetUnitX\(|SetUnitY\(", RegexOptions.Compiled), 2),
        ("sleep", new Regex(@"PolledWait|TriggerSleepAction", RegexOptions.Compiled), 40),
        ("destroyfx", new Regex(@"DestroyEffect", RegexOptions.Compiled), 0),
        ("timedlife", new Regex(@"UnitApplyTimedLife", RegexOptions.Compiled), 0),
        ("removeunit", new Regex(@"RemoveUnit\(", RegexOptions.Compiled), 0),
        ("cleargroup", new Regex(@"GroupClear\(|DestroyGroup\(", RegexOptions.Compiled), 0),
    };

    public static ScriptBenchResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var (file, source, isLua) = ScriptAnalysis.Read(doc);
        if (isLua)
            return new(false, "the script is Lua; the static bench currently reads JASS only", file, 0, 0, Array.Empty<ScriptBenchRow>());

        var functions = ScriptAnalysis.Functions(source, false);
        var lineStarts = LineStarts(source);

        // interval by callback name, from every TimerStart(..., true, function X)
        var intervals = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var f in functions)
        {
            var body = ScriptCommand.SliceFunction(source, lineStarts, f.StartLine, f.EndLine);
            foreach (Match m in TimerLiteral.Matches(body))
            {
                if (m.Groups[2].Value != "true") continue;
                if (!double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out double iv) || iv <= 0) continue;
                var cb = m.Groups[3].Value;
                if (!intervals.TryGetValue(cb, out double cur) || iv < cur) intervals[cb] = iv;
            }
            foreach (Match m in TimerEvent.Matches(body))
                if (double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out double iv2) && iv2 > 0)
                    if (!intervals.TryGetValue(f.Name, out double cur) || iv2 < cur) intervals[f.Name] = iv2;
        }

        var rows = new List<ScriptBenchRow>(functions.Count);
        foreach (var f in functions)
        {
            var slice = ScriptCommand.SliceFunction(source, lineStarts, f.StartLine, f.EndLine);
            // Count live code only. A commented-out CreateUnit is not a cost, and counting it
            // ranked QShikiOneCast2 at 20,800/s when its live body is two effects.
            var body = string.Join("\n", ScriptAnalysis.CodeLines(slice, false));
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            int cost = 0;
            foreach (var (key, pattern, weight) in Ops)
            {
                int n = pattern.Matches(body).Count;
                counts[key] = n;
                cost += n * weight;
            }

            intervals.TryGetValue(f.Name, out double interval);
            double? iv = interval > 0 ? interval : null;
            long perSecond = iv is { } v ? (long)Math.Round(cost / v) : 0;

            var leaks = new List<string>();
            if (counts["create"] > counts["timedlife"] + counts["removeunit"]) leaks.Add("unit");
            if (counts["sfx"] > counts["destroyfx"]) leaks.Add("effect");
            if (counts["enum"] > counts["cleargroup"]) leaks.Add("group");
            if ((counts["cond"] + counts["filter"]) > 0 && iv is not null) leaks.Add("boolexpr/tick");
            if (counts["sleep"] > 0) leaks.Add("sleep");

            rows.Add(new ScriptBenchRow(
                f.Name, f.StartLine, iv, perSecond, cost,
                counts["create"], counts["sfx"], counts["enum"], counts["cond"] + counts["filter"],
                counts["dmg"], string.Join(',', leaks)));
        }

        rows.Sort((a, b) => b.PerSecond != a.PerSecond ? b.PerSecond.CompareTo(a.PerSecond) : b.Cost.CompareTo(a.Cost));
        int periodic = rows.Count(r => r.Interval is not null);
        return new(true, $"{rows.Count} function(s), {periodic} periodic", file, rows.Count, periodic, rows);
    }

    private static int[] LineStarts(string source)
    {
        var starts = new List<int> { 0 };
        for (int i = 0; i < source.Length; i++)
            if (source[i] == '\n') starts.Add(i + 1);
        return starts.ToArray();
    }
}
