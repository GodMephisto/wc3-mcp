// src/Wc3.Commands/EditorCatalogCommand.cs
using Wc3.GameData;

namespace Wc3.Commands;

/// <summary>One catalog in the names listing, its name and how many entries it holds.</summary>
public sealed record EditorCatalogSummary(string Name, int Entries);

/// <summary>Result of the catalog names query, every catalog the installed game defines.</summary>
public sealed record EditorCatalogNamesResult(
    int Total,
    IReadOnlyList<EditorCatalogSummary> Catalogs);

/// <summary>Result of listing one catalog, its entries in the file's own order.</summary>
public sealed record EditorCatalogListResult(
    string Catalog,
    int Total,
    IReadOnlyList<EditorCatalogEntry> Entries);

/// <summary>
/// Read-only queries over the World Editor's catalogs (<c>UI\WorldEditData.txt</c>), the
/// data the editor drives its own pickers from. TileSets, SkyModels, LoadingScreens,
/// SoundChannels, the brush palettes, MapSizes and the rest. A front end asks here so it can
/// offer "Ashenvale" instead of making someone type the tileset letter A.
/// </summary>
public static class EditorCatalogCommand
{
    /// <summary>Every catalog name the installed game defines, with entry counts, in the
    /// file's own order. Throws <see cref="InvalidOperationException"/> with an actionable
    /// message when the install is unavailable.</summary>
    public static EditorCatalogNamesResult Names(string? gameDir)
    {
        var catalogs = Open(gameDir);
        var summaries = catalogs.Names
            .Select(n => new EditorCatalogSummary(
                n, catalogs.TryGet(n, out var entries) ? entries.Count : 0))
            .ToList();
        return new EditorCatalogNamesResult(summaries.Count, summaries);
    }

    /// <summary>The entries of one catalog, in the file's own order, display names resolved.
    /// The name is matched case insensitively. Throws <see cref="InvalidOperationException"/>
    /// for an unknown name or an unavailable install.</summary>
    public static EditorCatalogListResult List(string catalogName, string? gameDir)
    {
        if (string.IsNullOrWhiteSpace(catalogName))
            throw new InvalidOperationException("Pass a catalog name. List them with the catalog names query.");

        var catalogs = Open(gameDir);
        if (!catalogs.TryGet(catalogName, out var entries))
            throw new InvalidOperationException(
                $"Unknown catalog '{catalogName}'. Known catalogs: {string.Join(", ", catalogs.Names)}.");

        // Report the file's own casing for the name, not whatever the caller typed.
        var canonical = catalogs.Names.First(n =>
            string.Equals(n, catalogName, StringComparison.OrdinalIgnoreCase));
        return new EditorCatalogListResult(canonical, entries.Count, entries);
    }

    private static EditorCatalogData Open(string? gameDir)
    {
        if (!GameData.GameData.TryOpen(gameDir, out var ctx, out var diag) || ctx is null)
            throw new InvalidOperationException(
                "Could not open the installed game data"
                + (string.IsNullOrWhiteSpace(diag) ? "." : $": {diag}"));
        return ctx.EditorCatalogs;
    }
}
