// src/Wc3.Commands/ObjectBatchSetCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One edit in a batch, already resolved to a kind. The order of the list is the
/// order the edits are applied, so a later edit to the same field wins.</summary>
public sealed record ObjectBatchEdit(ObjectKind Kind, string Rawcode, string Field, string Value);

/// <summary>An edit that stopped the batch, with its zero-based position in the request.</summary>
public sealed record ObjectBatchFailure(int Index, string Rawcode, string Field, string Message);

/// <summary>
/// Outcome of a batch. <paramref name="Ok"/> is true only when every edit applied, and
/// <paramref name="Applied"/> is then the number of edits. On a failure, nothing is written,
/// <paramref name="Applied"/> is the number that had been applied in memory before the failure,
/// and <paramref name="Failed"/> names the edit that stopped the batch.
/// </summary>
public sealed record ObjectBatchSetResult(
    bool Ok, string Message, int Applied,
    IReadOnlyList<ObjectBatchFailure> Failed, IReadOnlyList<string> Warnings);

/// <summary>
/// Applies several object edits in one load. This exists because the file-per-write model
/// loses the first of two edits made from one base, and the usual workaround is a chain of
/// writes. A batch makes the whole set one write.
///
/// The batch is all or nothing. Edits are applied in order to the in-memory document, the
/// first failure stops the run, and the caller writes only when the result is Ok. A partly
/// edited map is therefore never produced.
///
/// It is not a transaction across processes. Two batches that both name the same base still
/// produce two output files, and the caller chooses which one to keep.
/// </summary>
public static class ObjectBatchSetCommand
{
    /// <summary>The most edits one call may carry. A larger list is a sign the caller wants a
    /// bulk operation, which has its own tool and its own where clauses.</summary>
    public const int MaxEdits = 500;

    public static ObjectBatchSetResult Execute(MapDocument doc, IReadOnlyList<ObjectBatchEdit> edits)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(edits);

        if (edits.Count == 0)
            return new(false, "no edits were supplied", 0, Array.Empty<ObjectBatchFailure>(), Array.Empty<string>());

        if (edits.Count > MaxEdits)
            return new(false,
                $"at most {MaxEdits} edits are allowed in one call, got {edits.Count}",
                0, Array.Empty<ObjectBatchFailure>(), Array.Empty<string>());

        var warnings = new List<string>(edits.Count);
        for (int i = 0; i < edits.Count; i++)
        {
            var edit = edits[i];
            var r = ObjectSetCommand.Execute(doc, edit.Kind, edit.Rawcode, edit.Field, edit.Value);
            if (!r.Ok)
            {
                return new(false,
                    $"edit {i + 1} of {edits.Count} failed on {edit.Kind.ToString().ToLowerInvariant()} "
                    + $"{edit.Rawcode} {edit.Field}, so nothing was written",
                    i,
                    new[] { new ObjectBatchFailure(i, edit.Rawcode, edit.Field, r.Message) },
                    warnings);
            }

            if (r.Warning is { Length: > 0 } warning)
                warnings.Add($"edit {i + 1}, {edit.Field}: {warning}");
        }

        return new(true, $"applied {edits.Count} edits in one load", edits.Count,
            Array.Empty<ObjectBatchFailure>(), warnings);
    }
}
