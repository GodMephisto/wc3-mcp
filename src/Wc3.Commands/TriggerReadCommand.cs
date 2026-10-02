// src/Wc3.Commands/TriggerReadCommand.cs
using Wc3.Model;
using War3Net.Build.Info;
using War3Net.Build.Script;

namespace Wc3.Commands;

/// <summary>Full read-only snapshot of a map's GUI-trigger tree (war3map.wtg) plus the
/// custom-text bodies (war3map.wct) and the map's script language (war3map.w3i). A map
/// without triggers yields empty lists, never a throw.</summary>
public sealed record TriggerModel(
    IReadOnlyList<TriggerCategoryInfo> Categories,
    IReadOnlyList<TriggerInfo> Triggers,
    IReadOnlyList<TriggerVariableInfo> Variables,
    string ScriptLanguage);

/// <summary>One folder node of the trigger tree. <see cref="Kind"/> is the raw item type
/// ("RootCategory" or "Category" — the formats that distinguish more kinds surface them
/// verbatim).</summary>
public sealed record TriggerCategoryInfo(int Id, string Name, string Kind);

/// <summary>One trigger (GUI, comment or custom-text). <see cref="Functions"/> is the
/// event/condition/action tree for GUI triggers; <see cref="CustomText"/> carries the
/// war3map.wct script body when <see cref="IsCustomText"/>.</summary>
public sealed record TriggerInfo(
    int Id,
    int ParentCategoryId,
    string Name,
    string Description,
    bool Enabled,
    bool InitiallyOn,
    bool IsCustomText,
    IReadOnlyList<TriggerFunctionInfo> Functions,
    string? CustomText);

/// <summary>One ECA node. <see cref="Kind"/> = Event/Condition/Action (nested calls show
/// as Call). <see cref="Parameters"/> are best-effort readable renderings of the raw wtg
/// parameters (no TriggerData.txt localization — presets/functions keep their raw names).
/// <see cref="Children"/> holds nested blocks (if/then/else, loops, and/or).</summary>
public sealed record TriggerFunctionInfo(
    string Kind,
    string Name,
    bool Enabled,
    IReadOnlyList<string> Parameters,
    IReadOnlyList<TriggerFunctionInfo> Children);

/// <summary>One global variable from the wtg. <see cref="InitialValue"/> is null when the
/// variable is not explicitly initialized.</summary>
public sealed record TriggerVariableInfo(string Name, string Type, bool IsArray, string? InitialValue);

/// <summary>
/// Read-only projection of the map's triggers into plain POCOs — the model a trigger
/// editor UI consumes. Sources: war3map.wtg (tree + variables, War3Net
/// <see cref="MapTriggers"/>), war3map.wct (custom-text bodies,
/// <see cref="MapCustomTextTriggers"/>) and war3map.w3i (script language). Unlike
/// <see cref="TriggerCommand"/> (flat item listing + flag edits) this walks the full ECA
/// function tree. No write-back — mutation stays in <see cref="TriggerCommand"/>.
/// </summary>
public static class TriggerReadCommand
{
    public const string TriggersFileName = "war3map.wtg";
    public const string CustomTextFileName = "war3map.wct";

    /// <summary>Guards the ECA recursion (parameters can nest sub-functions which carry
    /// parameters again). Real wtg trees are a handful of levels deep; anything past this
    /// is rendered as a truncated stub instead of recursing further.</summary>
    private const int MaxDepth = 24;

    /// <summary>Reads the full trigger snapshot. A map without a (parseable) war3map.wtg
    /// returns an empty-but-valid model carrying just the script language.</summary>
    public static TriggerModel GetTriggers(MapDocument doc)
    {
        string language = (doc.GetFile("war3map.w3i")?.Model as MapInfo)?.ScriptLanguage.ToString()
            ?? ScriptLanguage.Jass.ToString();

        if (doc.GetFile(TriggersFileName)?.Model is not MapTriggers wtg)
            return new TriggerModel(
                Array.Empty<TriggerCategoryInfo>(),
                Array.Empty<TriggerInfo>(),
                Array.Empty<TriggerVariableInfo>(),
                language);

        var wct = doc.GetFile(CustomTextFileName)?.Model as MapCustomTextTriggers;

        var categories = new List<TriggerCategoryInfo>();
        var triggers = new List<TriggerInfo>();

        // war3map.wct carries one code body per TriggerDefinition, in wtg document order
        // (GUI triggers get an empty slot). Track the ordinal to pair them up.
        int definitionOrdinal = 0;
        foreach (var item in wtg.TriggerItems)
        {
            switch (item)
            {
                case TriggerCategoryDefinition cat:
                    categories.Add(new TriggerCategoryInfo(
                        cat.Id, cat.Name ?? string.Empty, cat.Type.ToString()));
                    break;
                case TriggerDefinition td:
                    triggers.Add(ToTriggerInfo(td, wct, definitionOrdinal));
                    definitionOrdinal++;
                    break;
                // DeletedTriggerItem stubs and TriggerVariableDefinition tree entries
                // (the sub-version format mirrors variables into the tree) carry no
                // trigger content — variables come from wtg.Variables below.
            }
        }

        var variables = wtg.Variables
            .Select(v => new TriggerVariableInfo(
                v.Name ?? string.Empty,
                v.Type ?? string.Empty,
                v.IsArray,
                v.IsInitialized ? v.InitialValue ?? string.Empty : null))
            .ToList();

        return new TriggerModel(categories, triggers, variables, language);
    }

    private static TriggerInfo ToTriggerInfo(TriggerDefinition td, MapCustomTextTriggers? wct, int ordinal)
    {
        // The sub-version format marks custom-text triggers by item type (Script); the
        // classic format by the IsCustomTextTrigger flag. Honour either.
        bool isCustomText = td.IsCustomTextTrigger || td.Type == TriggerItemType.Script;

        string? customText = null;
        if (isCustomText && wct is not null && ordinal < wct.CustomTextTriggers.Count)
            // Bodies are stored null-terminated; trim so consumers see clean script text.
            customText = wct.CustomTextTriggers[ordinal].Code?.TrimEnd('\0');

        return new TriggerInfo(
            td.Id,
            td.ParentId,
            td.Name ?? string.Empty,
            td.Description ?? string.Empty,
            td.IsEnabled,
            td.IsInitiallyOn,
            isCustomText,
            td.Functions.Select(f => ToFunctionInfo(f, depth: 0)).ToList(),
            customText);
    }

    private static TriggerFunctionInfo ToFunctionInfo(TriggerFunction f, int depth)
    {
        if (depth >= MaxDepth)
            return new TriggerFunctionInfo(
                f.Type.ToString(), f.Name ?? string.Empty, f.IsEnabled,
                new[] { "..." }, Array.Empty<TriggerFunctionInfo>());

        return new TriggerFunctionInfo(
            f.Type.ToString(),
            f.Name ?? string.Empty,
            f.IsEnabled,
            f.Parameters.Select(p => RenderParameter(p, depth + 1)).ToList(),
            f.ChildFunctions.Select(c => ToFunctionInfo(c, depth + 1)).ToList());
    }

    /// <summary>Best-effort compact rendering of one wtg parameter: presets and variables
    /// by raw name (with <c>[index]</c> for array reads), nested function calls as
    /// <c>Name(args)</c>, string literals quoted unless they read as a number (the wtg
    /// stores numeric literals with the String parameter type too).</summary>
    private static string RenderParameter(TriggerFunctionParameter p, int depth)
    {
        if (depth >= MaxDepth) return "...";

        switch (p.Type)
        {
            case TriggerFunctionParameterType.Preset:
                return p.Value ?? string.Empty;

            case TriggerFunctionParameterType.Variable:
                string name = p.Value ?? string.Empty;
                return p.ArrayIndexer is null
                    ? name
                    : $"{name}[{RenderParameter(p.ArrayIndexer, depth + 1)}]";

            case TriggerFunctionParameterType.Function:
                return p.Function is null
                    ? $"{p.Value}()"
                    : RenderCall(p.Function, depth + 1);

            case TriggerFunctionParameterType.String:
                string value = p.Value ?? string.Empty;
                return LooksNumeric(value) ? value : $"\"{value}\"";

            default:
                return p.Value ?? string.Empty;
        }
    }

    private static string RenderCall(TriggerFunction f, int depth)
    {
        if (depth >= MaxDepth) return $"{f.Name}(...)";
        return $"{f.Name}({string.Join(", ", f.Parameters.Select(p => RenderParameter(p, depth + 1)))})";
    }

    /// <summary>True for integer/real literals as the wtg stores them (invariant, optional
    /// sign/decimal point) so they render bare instead of quoted.</summary>
    private static bool LooksNumeric(string s) =>
        s.Length > 0 && double.TryParse(
            s, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out _);
}
