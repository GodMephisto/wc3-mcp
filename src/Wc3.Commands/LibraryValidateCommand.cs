// src/Wc3.Commands/LibraryValidateCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One map's load validation, with the issues that decided the verdict.</summary>
public sealed record LibraryValidateMap(
    string Map,
    bool Valid,
    int Errors,
    int Warnings,
    IReadOnlyList<ValidationIssue> Issues,
    string? Error);

/// <summary>A folder validation: every map's row plus the valid and invalid counts.</summary>
public sealed record LibraryValidateResult(
    IReadOnlyList<LibraryValidateMap> Maps,
    int Valid,
    int Invalid);

/// <summary>
/// Run the load validation over every map in a folder and list the ones that fail, with the reason.
/// Each map goes through <see cref="ValidateCommand.Execute"/>, so the verdict is the same one the
/// single-map validate produces. A map that cannot be opened at all counts as invalid, with the
/// loader's failure as its reason.
///
/// Maps are opened one at a time and released before the next, so a sweep never holds a whole
/// library in memory at once.
/// </summary>
public static class LibraryValidateCommand
{
    public static LibraryValidateResult Execute(string folder)
    {
        var maps = new List<LibraryValidateMap>();
        foreach (var path in BenchMapFolder.Enumerate(folder))
        {
            string name = Path.GetFileName(path);
            try
            {
                maps.Add(Probe(name, MapDocument.Load(path)));
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                maps.Add(Fail(name, e.Message));
            }
        }
        return new LibraryValidateResult(maps, maps.Count(m => m.Valid), maps.Count(m => !m.Valid));
    }

    /// <summary>One already-loaded map's validation row. Pure, so the report can reuse one load.</summary>
    public static LibraryValidateMap Probe(string name, MapDocument doc)
    {
        var r = ValidateCommand.Execute(doc);
        return new LibraryValidateMap(name, r.Valid, r.Errors, r.Warnings, r.Issues, null);
    }

    /// <summary>A map that could not be opened, treated as invalid with the loader's reason.</summary>
    public static LibraryValidateMap Fail(string name, string error) =>
        new(name, false, 1, 0, Array.Empty<ValidationIssue>(), error);
}
