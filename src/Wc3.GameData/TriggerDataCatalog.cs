using System.Text;

namespace Wc3.GameData;

/// <summary>
/// A GUI-trigger type declared in <c>UI\TriggerData.txt</c> (<c>[TriggerTypes]</c>), e.g.
/// <c>abilcode</c>, <c>unit</c>, <c>boolean</c>. The three leading flags are best-effort
/// semantic names for the three integer fields the World Editor stores; <see cref="BaseType"/>
/// is the underlying primitive a typed code lowers to (e.g. <c>abilcode</c> → <c>integer</c>).
/// </summary>
public sealed record TriggerType(
    string Name,
    bool CanBeGlobal,
    bool CanBeParameter,
    bool Displayed,
    string DisplayNameKey,
    string? BaseType);

/// <summary>A trigger category (<c>[TriggerCategories]</c>): folder + icon a function is filed under.</summary>
public sealed record TriggerCategory(string Name, string DisplayNameKey, string IconPath, bool Selectable);

/// <summary>
/// A preset parameter value (<c>[TriggerParams]</c>) — a named constant of a given
/// <see cref="TypeName"/> whose in-script form is <see cref="ScriptText"/>
/// (e.g. <c>OperatorAdd</c> → type <c>ArithmeticOperator</c>, script <c>+</c>).
/// </summary>
public sealed record TriggerParamPreset(
    string Name,
    int GameVersion,
    string TypeName,
    string ScriptText,
    string DisplayNameKey);

/// <summary>Which trigger-function section an entry came from.</summary>
public enum TriggerFunctionKind { Event, Condition, Action, Call }

/// <summary>
/// A GUI-trigger function: an event, condition, action, or call. <see cref="ArgumentTypes"/>
/// lists the parameter types (the sentinel <c>nothing</c> is dropped — an empty list means
/// no parameters). <see cref="ReturnType"/> is set for <see cref="TriggerFunctionKind.Call"/>
/// entries; conditions implicitly return <c>boolean</c>. Metadata (<see cref="DisplayName"/>,
/// <see cref="ParametersLayout"/>, <see cref="Defaults"/>, <see cref="Category"/>) comes from
/// the <c>_Name_Field=…</c> sub-lines that follow the header.
/// </summary>
public sealed record TriggerFunction(
    string Name,
    TriggerFunctionKind Kind,
    int GameVersion,
    bool UsableInEvents,
    string? ReturnType,
    IReadOnlyList<string> ArgumentTypes,
    string? DisplayName,
    string? ParametersLayout,
    string? Defaults,
    string? Category)
{
    /// <summary>True when the function takes no parameters.</summary>
    public bool HasParameters => ArgumentTypes.Count > 0;
}

/// <summary>
/// The parsed World-Editor GUI-trigger catalog (<c>UI\TriggerData.txt</c>): the set of
/// categories, types, type defaults, preset parameter values, and functions the GUI exposes.
/// Use <see cref="TriggerDataParser.Parse"/> to build one. Lookups are case-insensitive on
/// the function/type/preset key.
/// </summary>
public sealed class TriggerDataCatalog
{
    public IReadOnlyList<TriggerCategory> Categories { get; }
    public IReadOnlyList<TriggerType> Types { get; }
    /// <summary>Default in-script value for a type (<c>[TriggerTypeDefaults]</c>).</summary>
    public IReadOnlyDictionary<string, string> TypeDefaults { get; }
    public IReadOnlyList<TriggerParamPreset> Params { get; }
    public IReadOnlyList<TriggerFunction> Functions { get; }

    private readonly Dictionary<string, TriggerFunction> _functionsByName;
    private readonly Dictionary<string, TriggerType> _typesByName;
    private readonly Dictionary<string, TriggerParamPreset> _paramsByName;

    public TriggerDataCatalog(
        IReadOnlyList<TriggerCategory> categories,
        IReadOnlyList<TriggerType> types,
        IReadOnlyDictionary<string, string> typeDefaults,
        IReadOnlyList<TriggerParamPreset> @params,
        IReadOnlyList<TriggerFunction> functions)
    {
        Categories = categories;
        Types = types;
        TypeDefaults = typeDefaults;
        Params = @params;
        Functions = functions;

        _functionsByName = new(StringComparer.OrdinalIgnoreCase);
        foreach (var f in functions) _functionsByName[f.Name] = f;
        _typesByName = new(StringComparer.OrdinalIgnoreCase);
        foreach (var t in types) _typesByName[t.Name] = t;
        _paramsByName = new(StringComparer.OrdinalIgnoreCase);
        foreach (var p in @params) _paramsByName[p.Name] = p;
    }

    public TriggerFunction? FindFunction(string name) =>
        _functionsByName.TryGetValue(name, out var f) ? f : null;

    public TriggerType? FindType(string name) =>
        _typesByName.TryGetValue(name, out var t) ? t : null;

    public TriggerParamPreset? FindParam(string name) =>
        _paramsByName.TryGetValue(name, out var p) ? p : null;
}

/// <summary>
/// Parses the World-Editor <c>UI\TriggerData.txt</c> into a <see cref="TriggerDataCatalog"/>.
/// The parser is tolerant: malformed or unrecognised lines are skipped rather than throwing,
/// so a slightly-off community file still yields a usable catalog.
/// </summary>
public static class TriggerDataParser
{
    public static TriggerDataCatalog Parse(string text)
    {
        var categories = new List<TriggerCategory>();
        var types = new List<TriggerType>();
        var typeDefaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var @params = new List<TriggerParamPreset>();
        var functions = new List<TriggerFunction>();

        // Mutable accumulator for the function currently being built, so trailing
        // "_Name_Field=" meta lines can be folded into it before it is committed.
        FunctionBuilder? current = null;
        var section = Section.None;

        void Flush()
        {
            if (current is not null) { functions.Add(current.Build()); current = null; }
        }

        foreach (var raw in text.Replace("\r\n", "\n").Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//")) continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                Flush();
                section = line switch
                {
                    "[TriggerCategories]" => Section.Categories,
                    "[TriggerTypes]" => Section.Types,
                    "[TriggerTypeDefaults]" => Section.TypeDefaults,
                    "[TriggerParams]" => Section.Params,
                    "[TriggerEvents]" => Section.Events,
                    "[TriggerConditions]" => Section.Conditions,
                    "[TriggerActions]" => Section.Actions,
                    "[TriggerCalls]" => Section.Calls,
                    _ => Section.Other,
                };
                continue;
            }

            int eq = line.IndexOf('=');
            if (eq < 0) continue;
            var key = line[..eq].Trim();
            var val = line[(eq + 1)..].Trim();
            if (key.Length == 0) continue;

            switch (section)
            {
                case Section.Categories:
                {
                    var p = SplitCsv(val);
                    categories.Add(new TriggerCategory(
                        key,
                        At(p, 0),
                        At(p, 1),
                        At(p, 2) == "1"));
                    break;
                }
                case Section.Types:
                {
                    var p = SplitCsv(val);
                    string? baseType = p.Count > 4 ? Nz(p[4]) : null;
                    types.Add(new TriggerType(
                        key,
                        At(p, 0) == "1",
                        At(p, 1) == "1",
                        At(p, 2) == "1",
                        At(p, 3),
                        baseType));
                    break;
                }
                case Section.TypeDefaults:
                {
                    // "value[,WESTRING_key]" — the westring is the trailing field; the value
                    // may itself contain commas, so keep everything before a trailing WESTRING_.
                    var p = SplitCsv(val);
                    string value = val;
                    if (p.Count > 1 && p[^1].StartsWith("WESTRING_", StringComparison.Ordinal))
                        value = string.Join(",", p.Take(p.Count - 1));
                    typeDefaults[key] = value.Trim();
                    break;
                }
                case Section.Params:
                {
                    var p = SplitCsv(val);
                    @params.Add(new TriggerParamPreset(
                        key,
                        ParseInt(At(p, 0)),
                        At(p, 1),
                        Unquote(At(p, 2)),
                        At(p, 3)));
                    break;
                }
                case Section.Events:
                case Section.Conditions:
                case Section.Actions:
                case Section.Calls:
                {
                    if (key.StartsWith('_'))
                    {
                        // Metadata for the current function: _Name_Field=value.
                        if (current is not null) ApplyMeta(current, key, val);
                    }
                    else
                    {
                        Flush();
                        current = ParseHeader(section, key, val);
                    }
                    break;
                }
            }
        }
        Flush();

        return new TriggerDataCatalog(categories, types, typeDefaults, @params, functions);
    }

    private static FunctionBuilder ParseHeader(Section section, string name, string val)
    {
        var kind = section switch
        {
            Section.Events => TriggerFunctionKind.Event,
            Section.Conditions => TriggerFunctionKind.Condition,
            Section.Actions => TriggerFunctionKind.Action,
            _ => TriggerFunctionKind.Call,
        };

        var p = SplitCsv(val);
        int version = ParseInt(At(p, 0));
        bool usableInEvents = false;
        string? returnType = null;
        IEnumerable<string> argFields;

        if (kind == TriggerFunctionKind.Call)
        {
            // Name=version, usableInEvents(0/1), returnType, argTypes...
            usableInEvents = At(p, 1) == "1";
            returnType = Nz(At(p, 2));
            argFields = p.Skip(3);
        }
        else
        {
            // Name=version, argTypes...   (conditions implicitly return boolean)
            if (kind == TriggerFunctionKind.Condition) returnType = "boolean";
            argFields = p.Skip(1);
        }

        var args = argFields
            .Select(a => a.Trim())
            .Where(a => a.Length > 0 && !a.Equals("nothing", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return new FunctionBuilder
        {
            Name = name,
            Kind = kind,
            GameVersion = version,
            UsableInEvents = usableInEvents,
            ReturnType = returnType,
            ArgumentTypes = args,
        };
    }

    private static void ApplyMeta(FunctionBuilder fb, string key, string val)
    {
        // key looks like "_Name_Field"; associate only when it belongs to this function.
        string prefix = "_" + fb.Name + "_";
        if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return;
        string field = key[prefix.Length..];

        switch (field.ToLowerInvariant())
        {
            case "displayname": fb.DisplayName = Unquote(val); break;
            case "parameters": fb.ParametersLayout = val; break;
            case "defaults": fb.Defaults = val; break;
            case "category": fb.Category = val; break;
            // Other meta (UseWithAI, AIDefaults, ScriptName, Limit, …) intentionally ignored.
        }
    }

    private sealed class FunctionBuilder
    {
        public string Name = "";
        public TriggerFunctionKind Kind;
        public int GameVersion;
        public bool UsableInEvents;
        public string? ReturnType;
        public List<string> ArgumentTypes = new();
        public string? DisplayName;
        public string? ParametersLayout;
        public string? Defaults;
        public string? Category;

        public TriggerFunction Build() => new(
            Name, Kind, GameVersion, UsableInEvents, ReturnType,
            ArgumentTypes, DisplayName, ParametersLayout, Defaults, Category);
    }

    private enum Section
    {
        None, Other, Categories, Types, TypeDefaults, Params,
        Events, Conditions, Actions, Calls,
    }

    /// <summary>Splits a comma-separated value, respecting double-quoted spans.</summary>
    internal static List<string> SplitCsv(string s)
    {
        var parts = new List<string>();
        var sb = new StringBuilder();
        bool inQuote = false;
        foreach (var ch in s)
        {
            if (ch == '"') { inQuote = !inQuote; sb.Append(ch); }
            else if (ch == ',' && !inQuote) { parts.Add(sb.ToString().Trim()); sb.Clear(); }
            else sb.Append(ch);
        }
        parts.Add(sb.ToString().Trim());
        return parts;
    }

    private static string Unquote(string s) =>
        s.Length >= 2 && s[0] == '"' && s[^1] == '"' ? s[1..^1] : s;

    private static string At(List<string> p, int i) => i < p.Count ? p[i] : "";
    private static string? Nz(string s) => string.IsNullOrEmpty(s) ? null : s;

    private static int ParseInt(string s) =>
        int.TryParse(s.Trim(), out var v) ? v : 0;
}
