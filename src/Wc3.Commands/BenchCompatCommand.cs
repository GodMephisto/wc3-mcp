// src/Wc3.Commands/BenchCompatCommand.cs
namespace Wc3.Commands;

/// <summary>One map checked against one target build, with the finding counts behind the verdict.</summary>
public sealed record BenchCompatRow(
    string Map, string Target, bool Pass, int Errors, int Warnings, int Findings);

/// <summary>Aggregate over every map-by-target cell.</summary>
public sealed record BenchCompatAggregate(
    int Cells, int Passed, int Failed, int Errors, int Warnings, int Findings);

/// <summary>The pass or fail matrix plus its aggregate, maps and targets being the row and column labels.</summary>
public sealed record BenchCompatResult(
    IReadOnlyList<string> Maps,
    IReadOnlyList<string> Targets,
    IReadOnlyList<BenchCompatRow> Cells,
    BenchCompatAggregate Aggregate);

/// <summary>
/// Run the compatibility check for every map against every target build and report a pass or fail
/// matrix with the finding counts, plus the aggregate over the whole grid. Each cell is the same
/// check <see cref="TargetCompatCommand"/> makes for one map against one target, and
/// <see cref="CompatMatrixCommand"/> is the shared pass that produces it, so this command adds no
/// second compatibility rule.
/// </summary>
public static class BenchCompatCommand
{
    public static BenchCompatResult Execute(
        IReadOnlyList<string> maps, IReadOnlyList<string> targets, string? schemasDir = null) =>
        Analyze(CompatMatrixCommand.Execute(maps, targets, schemasDir));

    /// <summary>Fold a compatibility matrix into the bench rows and aggregate. Pure.</summary>
    public static BenchCompatResult Analyze(CompatMatrixResult matrix)
    {
        var cells = matrix.Cells
            .Select(c => new BenchCompatRow(c.Map, c.Target, c.Pass, c.Errors, c.Warnings, c.Findings))
            .ToList();

        var aggregate = new BenchCompatAggregate(
            cells.Count,
            cells.Count(c => c.Pass),
            cells.Count(c => !c.Pass),
            cells.Sum(c => c.Errors),
            cells.Sum(c => c.Warnings),
            cells.Sum(c => c.Findings));

        return new BenchCompatResult(matrix.Maps, matrix.Targets, cells, aggregate);
    }
}
