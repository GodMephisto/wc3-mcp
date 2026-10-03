// src/Wc3.Commands/TriggerFunctionBuilder.cs
using War3Net.Build.Script;

namespace Wc3.Commands;

/// <summary>One entry from the World-Editor function table, flattened for callers.</summary>
public sealed record TriggerFunctionSignature(
    string Name,
    string Kind,
    IReadOnlyList<string> ArgumentTypes,
    IReadOnlyList<string> Defaults,
    string? DisplayName,
    string? Category);

/// <summary>
/// Builds a <see cref="TriggerFunction"/> that the wtg reader can actually read back.
///
/// The constraint that governs everything here: war3map.wtg stores a function's parameters but
/// NOT how many there are. The count comes from the World-Editor function table at read time. So
/// a function written with the wrong number of parameters does not produce a wrong trigger, it
/// produces a FILE NOBODY CAN PARSE, and the reader fails looking for the terminator of a string
/// that was never written. Measured: writing DisplayTextToForce with one parameter when the table
/// declares two ("force", "StringExt") makes every subsequent read of that map throw.
///
/// That is why arity is not a suggestion to validate loosely. Every function built here carries
/// exactly the declared number of parameters, filling any the caller did not supply from the
/// table's own defaults.
///
/// The table is War3Net's <see cref="TriggerData.Default"/>, the stock Reforged one, which needs
/// no game install. A map built against a custom TriggerData is outside what this can author, and
/// says so rather than writing something unreadable.
/// </summary>
public static class TriggerFunctionBuilder
{
    /// <summary>Every function of the given kind the table declares, in name order.</summary>
    public static IReadOnlyList<TriggerFunctionSignature> Catalog(TriggerFunctionType kind)
    {
        var table = TableFor(kind);
        return table.Keys.Cast<string>().OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
            .Select(k => Signature(kind, k)!)
            .Where(s => s is not null)
            .ToList();
    }

    /// <summary>The declared signature, or null when the table has no such function.</summary>
    public static TriggerFunctionSignature? Signature(TriggerFunctionType kind, string name)
    {
        var table = TableFor(kind);
        if (!table.Contains(name)) return null;
        object? entry = table[name];
        if (entry is null) return null;

        return new TriggerFunctionSignature(
            name,
            kind.ToString(),
            Strings(entry, "ArgumentTypes"),
            Strings(entry, "Defaults"),
            Str(entry, "DisplayName"),
            Str(entry, "Category"));
    }

    /// <summary>
    /// Builds the function, or explains why it cannot be built. Never returns something the
    /// reader would choke on.
    /// </summary>
    public static (TriggerFunction? Function, string? Error) Build(
        TriggerFunctionType kind, string name, IReadOnlyList<string>? values)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (null, "A function name is required.");

        var sig = Signature(kind, name);
        if (sig is null)
        {
            var near = Catalog(kind)
                .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Take(5).Select(s => s.Name).ToList();
            return (null,
                $"The World-Editor table declares no {kind.ToString().ToLowerInvariant()} called "
                + $"'{name}'."
                + (near.Count == 0 ? "" : " Did you mean " + string.Join(", ", near) + "?"));
        }

        int arity = sig.ArgumentTypes.Count;
        values ??= Array.Empty<string>();
        if (values.Count > arity)
            return (null,
                $"'{name}' takes {arity} parameter(s) ({string.Join(", ", sig.ArgumentTypes)}), "
                + $"but {values.Count} were given.");

        var fn = new TriggerFunction { Type = kind, Name = name, IsEnabled = true };
        for (int i = 0; i < arity; i++)
        {
            string? given = i < values.Count ? values[i] : null;
            string value = given ?? DefaultAt(sig, i);
            fn.Parameters.Add(Parameter(value));
        }
        return (fn, null);
    }

    /// <summary>
    /// The table's own default for a parameter, or an empty string.
    ///
    /// The defaults list uses "_" as a placeholder for "no default", which must not be written
    /// through as a literal value.
    /// </summary>
    private static string DefaultAt(TriggerFunctionSignature sig, int i)
    {
        if (i >= sig.Defaults.Count) return string.Empty;
        string d = sig.Defaults[i];
        return d == "_" ? string.Empty : d;
    }

    /// <summary>
    /// Classifies one parameter value into the four shapes the format allows.
    ///
    /// A value naming a table CALL becomes a nested function call, built recursively so that it
    /// too carries its declared arity, since a nested call is read the same way as a top-level
    /// one and gets the same chance to derail the stream. A value naming a table PRESET becomes a
    /// preset. A "udg_" name becomes a variable reference. Anything else is a literal.
    /// </summary>
    private static TriggerFunctionParameter Parameter(string value)
    {
        if (value.Length == 0)
            return new TriggerFunctionParameter
            { Type = TriggerFunctionParameterType.String, Value = string.Empty };

        if (TriggerData.Default.TriggerCalls.ContainsKey(value))
        {
            var (nested, error) = Build(TriggerFunctionType.Call, value, null);
            if (error is null && nested is not null)
                return new TriggerFunctionParameter
                {
                    Type = TriggerFunctionParameterType.Function,
                    Value = value,
                    Function = nested,
                };
        }

        if (TriggerData.Default.TriggerParams.ContainsKey(value))
            return new TriggerFunctionParameter
            { Type = TriggerFunctionParameterType.Preset, Value = value };

        if (value.StartsWith("udg_", StringComparison.Ordinal))
            return new TriggerFunctionParameter
            { Type = TriggerFunctionParameterType.Variable, Value = value };

        return new TriggerFunctionParameter
        { Type = TriggerFunctionParameterType.String, Value = value };
    }

    private static System.Collections.IDictionary TableFor(TriggerFunctionType kind) => kind switch
    {
        TriggerFunctionType.Event => TriggerData.Default.TriggerEvents,
        TriggerFunctionType.Condition => TriggerData.Default.TriggerConditions,
        TriggerFunctionType.Action => TriggerData.Default.TriggerActions,
        TriggerFunctionType.Call => TriggerData.Default.TriggerCalls,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown function kind."),
    };

    // The table's entry types are internal to War3Net, so their fields are read by name. Kept in
    // one place so a War3Net upgrade that renames one fails here rather than in four call sites.
    private static IReadOnlyList<string> Strings(object entry, string property)
    {
        var value = entry.GetType().GetProperty(property)?.GetValue(entry);
        if (value is System.Collections.IEnumerable seq and not string)
            return seq.Cast<object?>().Select(x => x?.ToString() ?? string.Empty).ToList();
        return Array.Empty<string>();
    }

    private static string? Str(object entry, string property) =>
        entry.GetType().GetProperty(property)?.GetValue(entry)?.ToString();
}
