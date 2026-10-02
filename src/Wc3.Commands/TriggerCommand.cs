// src/Wc3.Commands/TriggerCommand.cs
using Wc3.Model;
using War3Net.Build.Extensions;
using War3Net.Build.Script;

namespace Wc3.Commands;

/// <summary>One GUI-trigger-tree item, flattened for read-back and listing. Categories,
/// GUI/comment/script triggers and deleted stubs all share Id/ParentId/Type/Name; the
/// trigger-only flags (<see cref="IsEnabled"/> etc.) are null for non-trigger rows.</summary>
public sealed record TriggerItemFields(
    int Id, int ParentId, string Type, string Name,
    bool? IsEnabled, bool? IsInitiallyOn, bool? RunOnMapInit, bool? IsComment,
    int? FunctionCount);

/// <summary>One map-variable definition (the .wtg <c>Variables</c> list, which is distinct
/// from the TriggerItems tree).</summary>
public sealed record TriggerVariableFields(
    int Id, int ParentId, string Name, string Type,
    bool IsArray, int ArraySize, bool IsInitialized, string InitialValue);

/// <summary>Result of a mutating trigger op. <see cref="Count"/> is the trigger-item count
/// after the op (-1 when the op failed).</summary>
public sealed record TriggerOpResult(bool Ok, string Message, int Count = -1);

/// <summary>
/// Read/write access to the GUI-trigger file (war3map.wtg). The write side mutates the
/// already-parsed War3Net <see cref="MapTriggers"/> model and re-serializes the WHOLE file
/// via War3Net's writer (the inverse of the ReadMapTriggers parser wired in Parsers), so
/// untouched trigger items survive byte-for-byte. MapDocument.SerializeEntry has no
/// MapTriggers case, so the bytes go back through <see cref="MapDocument.AddOrReplaceRawFile"/>
/// (raw payloads are written verbatim on Save) and the parsed model is restored afterwards
/// so in-memory readers keep seeing the mutation. Edits here never add or remove items, so
/// the model's <c>TriggerItemCounts</c> stays consistent with the writer.
/// </summary>
public static class TriggerCommand
{
    public const string FileName = "war3map.wtg";

    /// <summary>Lists every trigger-tree item in document order.</summary>
    public static IReadOnlyList<TriggerItemFields> List(MapDocument doc) =>
        GetTriggers(doc).TriggerItems.Select(ToFields).ToList();

    /// <summary>Lists every map-variable definition in document order.</summary>
    public static IReadOnlyList<TriggerVariableFields> ListVariables(MapDocument doc) =>
        GetTriggers(doc).Variables.Select(ToVarFields).ToList();

    /// <summary>Renames the trigger item with the given id (any kind: category, trigger or
    /// deleted stub) and writes the re-serialized wtg back into the in-memory document.
    /// Rejects a blank name or an id that no item carries.</summary>
    public static TriggerOpResult Rename(MapDocument doc, int id, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return new TriggerOpResult(false, "Trigger name must not be blank.");

        var triggers = GetTriggers(doc);
        var item = triggers.TriggerItems.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return new TriggerOpResult(false, $"No trigger item with id {id}.");

        item.Name = newName;
        return Persist(doc, triggers, $"Renamed trigger item {id} to '{newName}'.");
    }

    /// <summary>Enables/disables the GUI or custom-text trigger with the given id. Rejects
    /// an id that is not a <see cref="TriggerDefinition"/> (categories and deleted stubs
    /// have no enabled flag).</summary>
    public static TriggerOpResult SetEnabled(MapDocument doc, int id, bool enabled) =>
        SetTriggerFlag(doc, id, t => t.IsEnabled = enabled,
            enabled ? $"Enabled trigger {id}." : $"Disabled trigger {id}.");

    /// <summary>Sets whether the trigger with the given id starts turned on at map load.</summary>
    public static TriggerOpResult SetInitiallyOn(MapDocument doc, int id, bool on) =>
        SetTriggerFlag(doc, id, t => t.IsInitiallyOn = on,
            $"Set trigger {id} initially-on = {on}.");

    /// <summary>Sets whether the trigger with the given id runs on map initialization.</summary>
    public static TriggerOpResult SetRunOnMapInit(MapDocument doc, int id, bool on) =>
        SetTriggerFlag(doc, id, t => t.RunOnMapInit = on,
            $"Set trigger {id} run-on-map-init = {on}.");

    private static TriggerOpResult SetTriggerFlag(
        MapDocument doc, int id, Action<TriggerDefinition> mutate, string message)
    {
        var triggers = GetTriggers(doc);
        var item = triggers.TriggerItems.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return new TriggerOpResult(false, $"No trigger item with id {id}.");
        if (item is not TriggerDefinition td)
            return new TriggerOpResult(false,
                $"Trigger item {id} is a {item.Type}, not an editable trigger.");

        mutate(td);
        return Persist(doc, triggers, message);
    }

    /// <summary>Serializes a MapTriggers with War3Net's writer (the exact inverse of the
    /// ReadMapTriggers parser wired in Parsers).</summary>
    public static byte[] Serialize(MapTriggers triggers)
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            writer.Write(triggers);
        return ms.ToArray();
    }

    private static TriggerOpResult Persist(MapDocument doc, MapTriggers triggers, string message)
    {
        var entry = doc.AddOrReplaceRawFile(FileName, Serialize(triggers));
        // AddOrReplaceRawFile drops the parsed model (raw payload wins on Save); restore it
        // so in-memory readers keep seeing the mutation. The override bytes were serialized
        // from this very model, so the two stay consistent.
        entry.Model = triggers;
        return new TriggerOpResult(true, message, triggers.TriggerItems.Count);
    }

    private static MapTriggers GetTriggers(MapDocument doc) =>
        doc.GetFile(FileName)?.Model as MapTriggers
        ?? throw new InvalidOperationException(
            $"{FileName} is missing or could not be parsed; triggers are not editable.");

    private static TriggerItemFields ToFields(TriggerItem i) => i is TriggerDefinition td
        ? new(i.Id, i.ParentId, i.Type.ToString(), i.Name ?? string.Empty,
              td.IsEnabled, td.IsInitiallyOn, td.RunOnMapInit, td.IsComment, td.Functions.Count)
        : new(i.Id, i.ParentId, i.Type.ToString(), i.Name ?? string.Empty,
              null, null, null, null, null);

    private static TriggerVariableFields ToVarFields(VariableDefinition v) => new(
        v.Id, v.ParentId, v.Name ?? string.Empty, v.Type ?? string.Empty,
        v.IsArray, v.ArraySize, v.IsInitialized, v.InitialValue ?? string.Empty);
}
