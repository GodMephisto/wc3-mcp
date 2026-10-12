// src/Wc3.Commands/TriggerDeadCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One reason a trigger is considered dead. <see cref="Kind"/> is one of
/// <c>no-actions</c>, <c>no-events</c>, <c>event-on-never-set-variable</c> or
/// <c>unit-event-without-unit</c>.</summary>
public sealed record DeadReason(string Kind, string Detail);

/// <summary>One trigger and every reason it is dead.</summary>
public sealed record DeadTrigger(int Id, string Name, IReadOnlyList<DeadReason> Reasons);

/// <summary>The dead-trigger outcome, plus whether the compiled script was available to check
/// variable assignments, since without it only the tree's own initializers can be consulted.</summary>
public sealed record TriggerDeadResult(
    IReadOnlyList<DeadTrigger> Dead,
    int TriggersChecked,
    bool ScriptScanned,
    string Summary);

/// <summary>
/// Finds triggers that cannot do anything. Two families, both read-only.
///
/// <list type="bullet">
/// <item>A trigger with no action, so even if it fires it runs nothing.</item>
/// <item>A trigger whose events can never fire. The decidable case is an event that depends on a
/// variable nothing ever sets, which covers the two shapes that bite in practice. A unit-specific
/// event holds a unit variable, and with nothing assigned to it there is no matching unit. A
/// generic variable event watches a variable, and a variable that is never set never changes, so
/// the event never triggers.</item>
/// </list>
///
/// A variable counts as set when the trigger tree initialises it OR, when the compiled script is
/// present, when the script assigns it. Checking the script matters because a variable set only at
/// runtime has no initializer in the tree, and calling that dead would be wrong. When no script is
/// available the check is limited to the tree and the result says so.
///
/// Disabled and custom-text triggers are skipped for the same reasons as in
/// <see cref="TriggerLintCommand"/>.
/// </summary>
public static class TriggerDeadCommand
{
    public const string ReasonNoActions = "no-actions";
    public const string ReasonNoEvents = "no-events";
    public const string ReasonNeverSetVariable = "event-on-never-set-variable";
    public const string ReasonUnitWithoutUnit = "unit-event-without-unit";

    /// <summary>Every assignment to a global variable, JASS (<c>set udg_x =</c>) or Lua
    /// (<c>udg_x =</c>). A comparison's <c>==</c> is excluded by the trailing lookahead.</summary>
    private static readonly Regex Assignment = new(
        @"(?:set\s+)?\budg_(?<name>[A-Za-z0-9_]+)\s*=(?!=)", RegexOptions.Compiled);

    public static TriggerDeadResult Dead(MapDocument doc)
    {
        var model = TriggerReadCommand.GetTriggers(doc);

        var variables = new Dictionary<string, TriggerVariableInfo>(StringComparer.Ordinal);
        foreach (var v in model.Variables)
            variables[TriggerTreeAnalysis.StripUdg(v.Name)] = v;

        string? script = ReadScript(doc);
        var scriptSet = new HashSet<string>(StringComparer.Ordinal);
        if (script is not null)
            foreach (Match m in Assignment.Matches(script))
                scriptSet.Add(m.Groups["name"].Value);

        var runOnInit = new Dictionary<int, bool>();
        foreach (var item in TriggerCommand.List(doc))
            if (item.RunOnMapInit is { } flag && !runOnInit.ContainsKey(item.Id))
                runOnInit[item.Id] = flag;

        var dead = new List<DeadTrigger>();
        int checkedCount = 0;

        foreach (var trigger in model.Triggers)
        {
            if (trigger.IsCustomText || !trigger.Enabled) continue;
            checkedCount++;

            var reasons = new List<DeadReason>();
            var events = trigger.Functions.Where(f => f.Kind == "Event").ToList();
            var actions = trigger.Functions.Where(f => f.Kind == "Action").ToList();

            if (actions.Count == 0)
                reasons.Add(new DeadReason(ReasonNoActions, "no action."));

            bool runOnInitFlag = runOnInit.TryGetValue(trigger.Id, out bool on) && on;
            if (events.Count == 0 && !runOnInitFlag)
            {
                reasons.Add(new DeadReason(ReasonNoEvents,
                    "no event, and not run on map initialization."));
            }
            else
            {
                foreach (var ev in events)
                    if (NeverFire(ev, variables, scriptSet, script is not null) is { } why)
                        reasons.Add(why);
            }

            if (reasons.Count > 0)
                dead.Add(new DeadTrigger(trigger.Id, trigger.Name, reasons));
        }

        string summary = dead.Count == 0
            ? $"{checkedCount} trigger(s) checked, none dead."
            : $"{dead.Count} dead trigger(s) among {checkedCount} checked.";
        if (script is null)
            summary += " The compiled script was not present, so only the tree's own variable "
                     + "initialisers could be checked, and a keyed-from-runtime map may read as "
                     + "dead wrongly through no fault of the tree.";
        if (variables.Count == 0)
            summary += " The tree defines no variables.";

        return new TriggerDeadResult(dead, checkedCount, script is not null, summary);
    }

    /// <summary>Why this event cannot fire, or null when nothing about it proves that. It is
    /// unfireable when it references a variable that nothing sets, which is exactly the shape
    /// that makes a unit event wait for a unit that is never assigned or a variable event watch a
    /// value that never changes.</summary>
    private static DeadReason? NeverFire(
        TriggerFunctionInfo ev,
        IReadOnlyDictionary<string, TriggerVariableInfo> variables,
        IReadOnlySet<string> scriptSet,
        bool scriptPresent)
    {
        var refs = TriggerTreeAnalysis.VariableRefs(new[] { TriggerTreeAnalysis.Describe(ev) });
        foreach (var reference in refs)
        {
            string name = TriggerTreeAnalysis.StripUdg(reference);
            bool known = variables.TryGetValue(name, out var variable);
            bool set = scriptSet.Contains(name) || (known && variable!.InitialValue is not null);
            if (set) continue;

            bool isUnit = known && string.Equals(variable!.Type, "unit", StringComparison.OrdinalIgnoreCase);
            if (!known && !scriptPresent)
                continue;   // an undeclared name with no script to search is not proof either way

            string where = known
                ? "declared but never initialized"
                : "not declared in the trigger tree";
            return isUnit
                ? new DeadReason(ReasonUnitWithoutUnit,
                    $"event '{ev.Name}' holds unit variable udg_{name}, which is {where} and set "
                    + "nowhere, so no matching unit exists and the event never fires.")
                : new DeadReason(ReasonNeverSetVariable,
                    $"event '{ev.Name}' watches udg_{name}, which is {where} and assigned nowhere, "
                    + "so it never changes and the event never fires.");
        }
        return null;
    }

    /// <summary>The compiled script text (JASS or Lua), or null when the map carries none.</summary>
    private static string? ReadScript(MapDocument doc)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile(@"scripts\war3map.j")
            ?? doc.GetFile("war3map.lua") ?? doc.GetFile(@"scripts\war3map.lua");
        return entry is null ? null : Wc3.Model.ScriptText.GetString(entry.CurrentBytes);
    }
}
