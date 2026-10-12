// src/Wc3.Commands/TriggerExportCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One trigger in the structured export, its event, condition and action lists split
/// out so a consumer does not have to filter by kind. Each node carries the catalog function
/// name, its enabled flag, its rendered arguments and any nested blocks.</summary>
public sealed record TriggerExportTrigger(
    int Id,
    int ParentCategoryId,
    string Name,
    string Description,
    bool Enabled,
    bool InitiallyOn,
    bool RunOnMapInit,
    bool IsCustomText,
    string? CustomText,
    IReadOnlyList<TriggerFunctionInfo> Events,
    IReadOnlyList<TriggerFunctionInfo> Conditions,
    IReadOnlyList<TriggerFunctionInfo> Actions,
    IReadOnlyList<TriggerFunctionInfo> Other);

/// <summary>The whole trigger tree as structured data, carrying categories, triggers and GUI
/// variables.</summary>
public sealed record TriggerExportResult(
    IReadOnlyList<TriggerExportTrigger> Triggers,
    IReadOnlyList<TriggerCategoryInfo> Categories,
    IReadOnlyList<TriggerVariableInfo> Variables,
    string ScriptLanguage);

/// <summary>
/// The full GUI trigger tree as structured output. It reads through
/// <see cref="TriggerReadCommand.GetTriggers"/>, so the function names, arguments and nested
/// blocks are the reader's own projection rather than a second walk that could disagree with it.
/// Read-only. A map without a trigger tree yields empty lists rather than failing.
/// </summary>
public static class TriggerExportCommand
{
    public static TriggerExportResult Export(MapDocument doc)
    {
        var model = TriggerReadCommand.GetTriggers(doc);

        // TriggerInfo (the reader's projection) does not carry the run-on-map-init flag, so it
        // comes from the flat item list the same way the Studio panel reads it. Ids can repeat on
        // a sub-version map, so the first item for an id wins, which is the best a tree without a
        // unique key can do.
        var runOnInit = new Dictionary<int, bool>();
        foreach (var item in TriggerCommand.List(doc))
            if (item.RunOnMapInit is { } flag && !runOnInit.ContainsKey(item.Id))
                runOnInit[item.Id] = flag;

        var triggers = model.Triggers.Select(t => new TriggerExportTrigger(
            t.Id,
            t.ParentCategoryId,
            t.Name,
            t.Description,
            t.Enabled,
            t.InitiallyOn,
            runOnInit.TryGetValue(t.Id, out bool on) && on,
            t.IsCustomText,
            t.CustomText,
            t.Functions.Where(f => f.Kind == "Event").ToList(),
            t.Functions.Where(f => f.Kind == "Condition").ToList(),
            t.Functions.Where(f => f.Kind == "Action").ToList(),
            t.Functions.Where(f => f.Kind is not ("Event" or "Condition" or "Action")).ToList()))
            .ToList();

        return new TriggerExportResult(triggers, model.Categories, model.Variables, model.ScriptLanguage);
    }
}
