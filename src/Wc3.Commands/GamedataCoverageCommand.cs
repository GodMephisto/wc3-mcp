// src/Wc3.Commands/GamedataCoverageCommand.cs
namespace Wc3.Commands;

/// <summary>One schema folder's coverage. Sections lists which of objects, triggers and script it holds.</summary>
public sealed record GamedataCoverageRow(
    string Build, string Era, IReadOnlyList<string> Sections, int Fields, int Natives);

/// <summary>Every schema folder's coverage, ordered oldest build first.</summary>
public sealed record GamedataCoverageResult(IReadOnlyList<GamedataCoverageRow> Rows);

/// <summary>
/// One row per schema folder, naming the build, its era and which sections it holds, so a reader can
/// tell a build that carries real object data from one that carries only its compiled script. Fields
/// counts every object field and Natives counts every native the script files declare.
/// </summary>
public static class GamedataCoverageCommand
{
    public static GamedataCoverageResult Execute(string? schemasDir = null)
    {
        var schemas = GamedataSupport.LoadAll(schemasDir, GameSchemaCommand.SchemaPart.Objects | GameSchemaCommand.SchemaPart.Script);
        var rows = schemas.Select(one => new GamedataCoverageRow(
            one.Build,
            GamedataSupport.Era(one.Build),
            GameSchemaCommand.Sections(one.Schema),
            one.Schema.Objects?.Values.Sum(f => f.Count) ?? 0,
            one.Schema.Script?.Values.Sum(a => a.Natives.Count) ?? 0))
            .ToList();
        return new GamedataCoverageResult(rows);
    }
}
