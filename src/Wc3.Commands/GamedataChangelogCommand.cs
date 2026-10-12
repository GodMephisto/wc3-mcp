// src/Wc3.Commands/GamedataChangelogCommand.cs
namespace Wc3.Commands;

/// <summary>One field change between two builds. Change is added, removed, retyped or bounds.</summary>
public sealed record GamedataFieldChange(string Code, string Change, string? Detail);

/// <summary>The field changes in one object metadata file.</summary>
public sealed record GamedataChangelogGroup(string Slk, IReadOnlyList<GamedataFieldChange> Changes);

/// <summary>
/// A field-level diff of two builds' object data. Groups holds only the files that changed, and
/// NotCompared names a file or a whole section that only one build carries, which is not the same as
/// every field being added or removed.
/// </summary>
public sealed record GamedataChangelogResult(
    string BuildA,
    string BuildB,
    IReadOnlyList<GamedataChangelogGroup> Groups,
    IReadOnlyList<string> NotCompared,
    int Added,
    int Removed,
    int Retyped,
    int BoundsChanged);

/// <summary>
/// Diffs the object fields of two builds, grouped by metadata file, and reports fields added,
/// removed, retyped, or whose minimum or maximum bound changed. Bounds covers a change to MinValue or
/// MaxValue.
/// </summary>
public static class GamedataChangelogCommand
{
    public static GamedataChangelogResult Execute(string buildA, string buildB, string? schemasDir = null)
    {
        var a = GamedataSupport.Find(schemasDir, buildA);
        var b = GamedataSupport.Find(schemasDir, buildB);

        var groups = new List<GamedataChangelogGroup>();
        var notCompared = new List<string>();
        int added = 0, removed = 0, retyped = 0, bounds = 0;

        if (a.Schema.Objects is null || b.Schema.Objects is null)
        {
            var missingIn = a.Schema.Objects is null && b.Schema.Objects is null
                ? a.Build + " or " + b.Build
                : a.Schema.Objects is null ? a.Build : b.Build;
            notCompared.Add("objects, not in the schema of " + missingIn);
            return new GamedataChangelogResult(a.Build, b.Build, groups, notCompared, 0, 0, 0, 0);
        }

        foreach (var slk in a.Schema.Objects.Keys.Union(b.Schema.Objects.Keys)
                     .OrderBy(k => k, StringComparer.Ordinal))
        {
            bool inA = a.Schema.Objects.TryGetValue(slk, out var left);
            bool inB = b.Schema.Objects.TryGetValue(slk, out var right);
            if (!inA || !inB)
            {
                notCompared.Add("objects " + slk + ", only in " + (inA ? a.Build : b.Build));
                continue;
            }

            var leftByCode = left!.GroupBy(f => f.Code, StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
            var rightByCode = right!.GroupBy(f => f.Code, StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

            var changes = new List<GamedataFieldChange>();
            foreach (var code in leftByCode.Keys.Union(rightByCode.Keys).OrderBy(k => k, StringComparer.Ordinal))
            {
                bool hasLeft = leftByCode.TryGetValue(code, out var x);
                bool hasRight = rightByCode.TryGetValue(code, out var y);
                if (!hasRight)
                {
                    changes.Add(new GamedataFieldChange(code, "removed", null));
                    removed++;
                    continue;
                }
                if (!hasLeft)
                {
                    changes.Add(new GamedataFieldChange(code, "added", null));
                    added++;
                    continue;
                }

                bool typeChanged = !string.Equals(x!.Type, y!.Type, StringComparison.Ordinal);
                bool minChanged = !string.Equals(x.MinValue, y.MinValue, StringComparison.Ordinal);
                bool maxChanged = !string.Equals(x.MaxValue, y.MaxValue, StringComparison.Ordinal);

                var parts = new List<string>();
                if (typeChanged) parts.Add("type " + Show(x.Type) + " -> " + Show(y.Type));
                if (minChanged) parts.Add("min " + Show(x.MinValue) + " -> " + Show(y.MinValue));
                if (maxChanged) parts.Add("max " + Show(x.MaxValue) + " -> " + Show(y.MaxValue));
                if (parts.Count == 0) continue;

                string change = typeChanged ? "retyped" : "bounds";
                if (typeChanged) retyped++; else bounds++;
                changes.Add(new GamedataFieldChange(code, change, string.Join(", ", parts)));
            }

            if (changes.Count > 0) groups.Add(new GamedataChangelogGroup(slk, changes));
        }

        return new GamedataChangelogResult(
            a.Build, b.Build, groups, notCompared, added, removed, retyped, bounds);
    }

    private static string Show(string? value) => string.IsNullOrEmpty(value) ? "(none)" : value;
}
