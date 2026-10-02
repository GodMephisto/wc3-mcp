// src/Wc3.Commands/TriggerCatalogCommand.cs
using System.Text;
using Wc3.GameData;

namespace Wc3.Commands;

/// <summary>A one-line summary of a GUI-trigger function for listing.</summary>
public sealed record TriggerCatalogEntry(
    string Name,
    string Kind,
    string? DisplayName,
    string? ReturnType,
    IReadOnlyList<string> ArgumentTypes,
    string? Category);

/// <summary>Result of a catalog <c>list</c> query: the source it came from and the matches.</summary>
public sealed record TriggerCatalogListResult(
    string Source,
    int Total,
    IReadOnlyList<TriggerCatalogEntry> Functions);

/// <summary>Full detail for a single GUI-trigger function (the <c>describe</c> result).</summary>
public sealed record TriggerCatalogDetail(
    string Name,
    string Kind,
    int GameVersion,
    bool UsableInEvents,
    string? ReturnType,
    IReadOnlyList<string> ArgumentTypes,
    string? DisplayName,
    string? ParametersLayout,
    string? Defaults,
    string? Category);

/// <summary>
/// Read-only queries over the World-Editor GUI-trigger catalog (<c>UI\TriggerData.txt</c>).
/// The catalog is loaded either from an explicit file or from the installed game's CASC
/// storage; both routes feed the shared <see cref="TriggerDataParser"/>.
/// </summary>
public static class TriggerCatalogCommand
{
    /// <summary>
    /// Loads the catalog from <paramref name="filePath"/> when given, otherwise from the
    /// installed game (optionally overridden by <paramref name="gameDir"/>). Throws
    /// <see cref="InvalidOperationException"/> with an actionable message when neither is available.
    /// </summary>
    public static TriggerDataCatalog Load(string? filePath, string? gameDir, out string source)
    {
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            if (!File.Exists(filePath))
                throw new InvalidOperationException($"TriggerData file not found: {filePath}");
            source = filePath;
            return TriggerDataParser.Parse(File.ReadAllText(filePath));
        }

        if (!GameData.GameData.TryOpen(gameDir, out var ctx, out var diag) || ctx is null)
            throw new InvalidOperationException(
                "Could not open the installed game data" +
                (string.IsNullOrWhiteSpace(diag) ? "." : $": {diag}") +
                " Pass --file to read a TriggerData.txt directly.");

        if (!ctx.TryReadFile(@"ui\triggerdata.txt", out var bytes))
            throw new InvalidOperationException(
                @"ui\triggerdata.txt was not found in the installed game data. " +
                "Pass --file to read a TriggerData.txt directly.");

        source = @"install:ui\triggerdata.txt";
        return TriggerDataParser.Parse(Encoding.UTF8.GetString(bytes));
    }

    /// <summary>
    /// Lists functions, optionally filtered by <paramref name="kind"/>
    /// (event|condition|action|call) and/or a case-insensitive <paramref name="search"/>
    /// over name and display name. Results are sorted by name.
    /// </summary>
    public static TriggerCatalogListResult List(
        TriggerDataCatalog catalog, string source, string? kind, string? search)
    {
        IEnumerable<TriggerFunction> q = catalog.Functions;

        if (!string.IsNullOrWhiteSpace(kind))
        {
            if (!Enum.TryParse<TriggerFunctionKind>(kind, ignoreCase: true, out var k))
                throw new InvalidOperationException(
                    $"Unknown kind '{kind}'. Expected one of: event, condition, action, call.");
            q = q.Where(f => f.Kind == k);
        }

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(f =>
                f.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (f.DisplayName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

        var entries = q
            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .Select(f => new TriggerCatalogEntry(
                f.Name, f.Kind.ToString(), f.DisplayName, f.ReturnType, f.ArgumentTypes, f.Category))
            .ToList();

        return new TriggerCatalogListResult(source, entries.Count, entries);
    }

    /// <summary>Full detail for a single function by name, or null when absent.</summary>
    public static TriggerCatalogDetail? Describe(TriggerDataCatalog catalog, string name)
    {
        var f = catalog.FindFunction(name);
        if (f is null) return null;
        return new TriggerCatalogDetail(
            f.Name, f.Kind.ToString(), f.GameVersion, f.UsableInEvents, f.ReturnType,
            f.ArgumentTypes, f.DisplayName, f.ParametersLayout, f.Defaults, f.Category);
    }
}
