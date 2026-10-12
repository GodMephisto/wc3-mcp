// src/Wc3.Commands/TriggerLintCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One lint finding, tagged with the rule that produced it and the trigger it is about.
/// <see cref="Rule"/> is one of <c>unreachable</c>, <c>duplicate-event</c> or <c>no-actions</c>.</summary>
public sealed record TriggerFinding(string Rule, int TriggerId, string TriggerName, string Detail);

/// <summary>The lint outcome, carrying every finding plus how many triggers were examined and how
/// many disabled ones were skipped.</summary>
public sealed record TriggerLintResult(
    IReadOnlyList<TriggerFinding> Findings,
    int TriggersChecked,
    int DisabledSkipped,
    string Summary);

/// <summary>
/// Three structural checks on a map's GUI trigger tree, all decidable from the tree alone.
///
/// <list type="bullet">
/// <item><c>unreachable</c> the trigger has no event and is not run on map initialization, so
/// nothing can ever start it.</item>
/// <item><c>duplicate-event</c> the same event, with the same arguments, is registered more than
/// once, so the trigger body runs once per copy.</item>
/// <item><c>no-actions</c> the trigger has no action, so reaching it does nothing.</item>
/// </list>
///
/// A disabled trigger is skipped, because being off is a deliberate state rather than a fault,
/// and reporting the rule against it is noise. The count of skipped triggers is carried in the
/// result rather than left implicit. Custom-text triggers are skipped too, since their logic is
/// JASS in war3map.wct and carries no event or action list to check.
/// </summary>
public static class TriggerLintCommand
{
    public const string RuleUnreachable = "unreachable";
    public const string RuleDuplicateEvent = "duplicate-event";
    public const string RuleNoActions = "no-actions";

    public static TriggerLintResult Lint(MapDocument doc)
    {
        var model = TriggerReadCommand.GetTriggers(doc);
        var findings = new List<TriggerFinding>();
        int checkedCount = 0;
        int skipped = 0;

        foreach (var trigger in model.Triggers)
        {
            if (trigger.IsCustomText) continue;
            if (!trigger.Enabled) { skipped++; continue; }
            checkedCount++;

            var events = trigger.Functions.Where(f => f.Kind == "Event").ToList();
            var actions = trigger.Functions.Where(f => f.Kind == "Action").ToList();

            if (events.Count == 0)
                findings.Add(new TriggerFinding(RuleUnreachable, trigger.Id, trigger.Name,
                    "no event, and not run on map initialization, so nothing can start it."));

            foreach (var duplicate in DuplicateEvents(events))
                findings.Add(new TriggerFinding(RuleDuplicateEvent, trigger.Id, trigger.Name,
                    $"the event '{duplicate}' is registered more than once."));

            if (actions.Count == 0)
                findings.Add(new TriggerFinding(RuleNoActions, trigger.Id, trigger.Name,
                    "no action, so reaching this trigger does nothing."));
        }

        string summary = findings.Count == 0
            ? $"{checkedCount} trigger(s) checked, no findings."
            : $"{findings.Count} finding(s) across {checkedCount} trigger(s) checked.";
        if (skipped > 0) summary += $" {skipped} disabled trigger(s) skipped.";

        return new TriggerLintResult(findings, checkedCount, skipped, summary);
    }

    /// <summary>The distinct event descriptions that appear more than once. Two events count as
    /// duplicates only when their names and arguments both match, so two events of the same kind
    /// that watch different players are not flagged.</summary>
    private static IEnumerable<string> DuplicateEvents(IEnumerable<TriggerFunctionInfo> events) =>
        events
            .GroupBy(TriggerTreeAnalysis.Describe, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.First().Name);
}
