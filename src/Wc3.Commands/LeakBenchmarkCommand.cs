// src/Wc3.Commands/LeakBenchmarkCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One synthetic map with a known injected leak. Expected is how many the detector should
/// find, Detected how many it did, and FalsePositives are the findings that are not the injection.</summary>
public sealed record LeakBenchmarkCase(
    string Kind, string Creator, int Expected, int Detected, int FalsePositives, bool TruePositive);

/// <summary>The detector's measured accuracy over the synthetic corpus. Precision and recall are
/// counted, not claimed, from the cases below.</summary>
public sealed record LeakBenchmarkResult(
    bool Ok,
    string Message,
    int Maps,
    int TruePositives,
    int FalsePositives,
    int FalseNegatives,
    double Precision,
    double Recall,
    IReadOnlyList<LeakBenchmarkCase> Cases,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Runs the leak detector over a set of synthetic maps, each carrying one injected leak of a known
/// creator kind, plus a clean control with a leak that is cleaned up. It reports the true positive
/// and false positive counts, so the detector's accuracy is a measured number rather than a claim.
///
/// The maps are built in memory with BlankMap and the injected script is written into war3map.j, so
/// the same code path a real map takes is exercised. No game is started and nothing is written to
/// disk.
/// </summary>
public static class LeakBenchmarkCommand
{
    private static readonly (string Kind, string Creator, string Call)[] Injections =
    {
        ("location", "Location", "call Location(0, 0)"),
        ("group", "CreateGroup", "call CreateGroup()"),
        ("force", "CreateForce", "call CreateForce()"),
        ("effect", "AddSpecialEffect", "call AddSpecialEffect(\"x.mdl\", 0, 0)"),
        ("timer", "CreateTimer", "call CreateTimer()"),
        ("texttag", "CreateTextTag", "call CreateTextTag()"),
        ("lightning", "AddLightning", "call AddLightning(\"CLPB\", true, 0, 0, 0, 0)"),
        ("trigger", "CreateTrigger", "call CreateTrigger()"),
    };

    public static LeakBenchmarkResult Execute()
    {
        var diagnostics = new List<string>();
        var cases = new List<LeakBenchmarkCase>();

        foreach (var (kind, creator, call) in Injections)
        {
            var leaks = Detect(MapWith(
                "function LeakTick takes nothing returns nothing\n"
                + $"    {call}\n"
                + "endfunction\n"
                + Periodic()));

            int detected = leaks.Leaks.Count(l => l.Rule == "discarded" && l.Handle == kind && l.Heat == "hot");
            int extra = leaks.Leaks.Count - detected;
            if (leaks.Leaks.Count == 0)
                diagnostics.Add($"{kind}, the detector found nothing at all for an injected {creator}");
            cases.Add(new LeakBenchmarkCase(kind, creator, 1, detected, extra, detected >= 1));
        }

        int controlFalsePositive = Detect(MapWith(
            "function LeakTick takes nothing returns nothing\n"
            + "    local group g = CreateGroup()\n"
            + "    call DestroyGroup(g)\n"
            + "endfunction\n"
            + Periodic())).Leaks.Count;
        if (controlFalsePositive > 0)
            diagnostics.Add($"control, the detector reported {controlFalsePositive} leak(s) in a map that cleans up");
        cases.Add(new LeakBenchmarkCase("control", "DestroyGroup", 0, 0, controlFalsePositive, controlFalsePositive == 0));

        int truePositives = cases.Count(c => c.Kind != "control" && c.TruePositive);
        int falsePositives = cases.Sum(c => c.FalsePositives);
        int falseNegatives = Injections.Length - truePositives;
        double precision = truePositives + falsePositives == 0
            ? 1.0 : (double)truePositives / (truePositives + falsePositives);
        double recall = truePositives + falseNegatives == 0
            ? 1.0 : (double)truePositives / (truePositives + falseNegatives);

        string message = $"{Injections.Length + 1} synthetic map(s), {truePositives} true positive(s), "
            + $"{falsePositives} false positive(s), {falseNegatives} false negative(s), "
            + $"precision {precision:0.###}, recall {recall:0.###}";
        return new(true, message, Injections.Length + 1, truePositives, falsePositives,
            falseNegatives, precision, recall, cases, diagnostics);
    }

    private static ScriptLeaksResult Detect(MapDocument doc) => ScriptLeaksCommand.Execute(doc);

    private static MapDocument MapWith(string script)
    {
        var doc = BlankMap.Create();
        FileEditCommand.WriteText(doc, "war3map.j", script);
        return doc;
    }

    private static string Periodic() =>
        "function main takes nothing returns nothing\n"
        + "    call TimerStart(CreateTimer(), 0.03, true, function LeakTick)\n"
        + "endfunction\n";
}
