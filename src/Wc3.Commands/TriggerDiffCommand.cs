// src/Wc3.Commands/TriggerDiffCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A trigger that carries the same unique id in both maps but a different name, which is
/// a rename rather than an add plus a remove.</summary>
public sealed record TriggerRename(int Id, string From, string To);

/// <summary>One event, condition or action that a shared trigger gained or lost.
/// <see cref="Change"/> is <c>added</c> or <c>removed</c>.</summary>
public sealed record TriggerEcaDelta(string Kind, string Change, string Description);

/// <summary>A trigger present in both maps whose event, condition or action list differs.</summary>
public sealed record TriggerChanged(int Id, string Name, IReadOnlyList<TriggerEcaDelta> Changes);

/// <summary>The whole trigger-tree difference between two maps.</summary>
public sealed record TriggerDiffResult(
    IReadOnlyList<string> Added,
    IReadOnlyList<string> Removed,
    IReadOnlyList<TriggerRename> Renamed,
    IReadOnlyList<TriggerChanged> Changed,
    int TriggersInA,
    int TriggersInB);

/// <summary>
/// Compares two maps' GUI trigger trees. Triggers are matched first by a unique id (so a rename
/// reads as a rename rather than an add and a remove), then by name for everything left over, so
/// a pair that merely renumbered still compares as shared and gets an ECA comparison. Whatever is
/// still unmatched is an addition or a removal.
/// </summary>
/// <remarks>
/// The unique-id step is what makes rename detection work at all, and it is deliberately
/// conservative. On a sub-version map ids are namespaced by item type and repeats are normal, so
/// an id shared by two triggers in either map is not treated as an identity and falls through to
/// the name match.
/// </remarks>
public static class TriggerDiffCommand
{
    public static TriggerDiffResult Compare(MapDocument mapA, MapDocument mapB)
    {
        var a = TriggerReadCommand.GetTriggers(mapA);
        var b = TriggerReadCommand.GetTriggers(mapB);

        var matchedA = new HashSet<TriggerInfo>();
        var matchedB = new HashSet<TriggerInfo>();
        var renamed = new List<TriggerRename>();
        var changed = new List<TriggerChanged>();

        // Pass 1. Ids that each map carries exactly once are identities, so a name change there is
        // a rename and the trigger is compared for ECA changes under its own id.
        var uniqueA = UniqueById(a.Triggers);
        var uniqueB = UniqueById(b.Triggers);
        foreach (var (id, ta) in uniqueA)
        {
            if (!uniqueB.TryGetValue(id, out var tb)) continue;
            matchedA.Add(ta);
            matchedB.Add(tb);
            if (!string.Equals(ta.Name, tb.Name, StringComparison.Ordinal))
                renamed.Add(new TriggerRename(id, ta.Name, tb.Name));
            AddChanges(changed, ta, tb);
        }

        // Pass 2. Everything left is paired by name, in order, so the same trigger under two
        // different ids still compares as shared.
        var leftA = a.Triggers.Where(t => !matchedA.Contains(t)).ToList();
        var leftB = b.Triggers.Where(t => !matchedB.Contains(t)).ToList();
        var byNameA = Group(leftA);
        var byNameB = Group(leftB);

        foreach (var (name, queueA) in byNameA)
        {
            if (!byNameB.TryGetValue(name, out var queueB)) continue;
            while (queueA.Count > 0 && queueB.Count > 0)
            {
                var ta = queueA.Dequeue();
                var tb = queueB.Dequeue();
                matchedA.Add(ta);
                matchedB.Add(tb);
                AddChanges(changed, ta, tb);
            }
        }

        var added = b.Triggers.Where(t => !matchedB.Contains(t)).Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
        var removed = a.Triggers.Where(t => !matchedA.Contains(t)).Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();

        return new TriggerDiffResult(
            added, removed,
            renamed.OrderBy(r => r.Id).ToList(),
            changed.OrderBy(c => c.Id).ToList(),
            a.Triggers.Count, b.Triggers.Count);
    }

    private static Dictionary<int, TriggerInfo> UniqueById(IReadOnlyList<TriggerInfo> triggers)
    {
        var result = new Dictionary<int, TriggerInfo>();
        foreach (var group in triggers.GroupBy(t => t.Id))
            if (group.Count() == 1)
                result[group.Key] = group.Single();
        return result;
    }

    private static Dictionary<string, Queue<TriggerInfo>> Group(IEnumerable<TriggerInfo> triggers)
    {
        var result = new Dictionary<string, Queue<TriggerInfo>>(StringComparer.Ordinal);
        foreach (var t in triggers)
        {
            if (!result.TryGetValue(t.Name, out var queue))
                result[t.Name] = queue = new Queue<TriggerInfo>();
            queue.Enqueue(t);
        }
        return result;
    }

    private static void AddChanges(List<TriggerChanged> changed, TriggerInfo a, TriggerInfo b)
    {
        var deltas = new List<TriggerEcaDelta>();
        foreach (var kind in new[] { "Event", "Condition", "Action" })
        {
            var before = Multiset(a.Functions.Where(f => f.Kind == kind).Select(TriggerTreeAnalysis.Describe));
            var after = Multiset(b.Functions.Where(f => f.Kind == kind).Select(TriggerTreeAnalysis.Describe));
            foreach (var item in Subtract(after, before))
                deltas.Add(new TriggerEcaDelta(kind, "added", item));
            foreach (var item in Subtract(before, after))
                deltas.Add(new TriggerEcaDelta(kind, "removed", item));
        }
        if (deltas.Count > 0)
            changed.Add(new TriggerChanged(b.Id, b.Name, deltas));
    }

    private static Dictionary<string, int> Multiset(IEnumerable<string> items)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var item in items)
            counts[item] = counts.TryGetValue(item, out int n) ? n + 1 : 1;
        return counts;
    }

    /// <summary>Items in <paramref name="left"/> whose count exceeds the count in
    /// <paramref name="right"/>, each repeated for the excess.</summary>
    private static IEnumerable<string> Subtract(
        IReadOnlyDictionary<string, int> left, IReadOnlyDictionary<string, int> right)
    {
        foreach (var (item, count) in left)
        {
            right.TryGetValue(item, out int other);
            for (int i = other; i < count; i++)
                yield return item;
        }
    }
}
