// src/Wc3.Commands/BundleDiffCommand.cs
namespace Wc3.Commands;

/// <summary>One member of a bundle, either an object (its kind and rawcode) or an asset (its path).</summary>
public sealed record BundleMember(string Kind, string Id, string? Name);

/// <summary>
/// What one bundle has that the other does not. Objects are matched by kind and rawcode,
/// assets by path (case-insensitively, a Windows path), so a rename of the display name or a
/// re-categorization of an asset is reported as changed rather than as a spurious add and remove.
/// </summary>
public sealed record BundleDiffResult(
    string RootA,
    string RootB,
    IReadOnlyList<BundleMember> Added,
    IReadOnlyList<BundleMember> Removed,
    IReadOnlyList<BundleMember> Changed,
    int Shared);

/// <summary>
/// Compares two dependency bundles by their object and asset members. Pure, neither bundle is
/// touched, so this sits beside the merge and the port without either knowing about it.
/// </summary>
public static class BundleDiffCommand
{
    public static BundleDiffResult Execute(UnitBundle a, UnitBundle b)
    {
        // Objects keyed by (kind, rawcode). Two bundles can each hold the same rawcode as
        // different kinds, which are different members, not a collision.
        var objA = a.Objects.ToDictionary(
            o => (o.Kind, o.Rawcode), o => o, ObjectMemberKey.Instance);
        var objB = b.Objects.ToDictionary(
            o => (o.Kind, o.Rawcode), o => o, ObjectMemberKey.Instance);

        var added = new List<BundleMember>();
        var removed = new List<BundleMember>();
        var changed = new List<BundleMember>();
        int shared = 0;

        foreach (var kv in objB)
        {
            if (!objA.ContainsKey(kv.Key)) added.Add(Member(kv.Value));
            else
            {
                shared++;
                if (!string.Equals(objA[kv.Key].Name, kv.Value.Name, StringComparison.Ordinal))
                    changed.Add(Member(kv.Value));
            }
        }
        foreach (var kv in objA)
            if (!objB.ContainsKey(kv.Key)) removed.Add(Member(kv.Value));

        // Assets keyed by path, case-insensitively.
        var fileA = a.Files.ToDictionary(f => f.Path, f => f, StringComparer.OrdinalIgnoreCase);
        var fileB = b.Files.ToDictionary(f => f.Path, f => f, StringComparer.OrdinalIgnoreCase);
        foreach (var kv in fileB)
        {
            if (!fileA.ContainsKey(kv.Key)) added.Add(Member(kv.Value));
            else
            {
                shared++;
                if (!string.Equals(fileA[kv.Key].Category, kv.Value.Category, StringComparison.Ordinal))
                    changed.Add(Member(kv.Value));
            }
        }
        foreach (var kv in fileA)
            if (!fileB.ContainsKey(kv.Key)) removed.Add(Member(kv.Value));

        return new BundleDiffResult(
            a.RootRawcode, b.RootRawcode,
            added.OrderBy(m => m.Kind, StringComparer.Ordinal).ThenBy(m => m.Id, StringComparer.Ordinal).ToList(),
            removed.OrderBy(m => m.Kind, StringComparer.Ordinal).ThenBy(m => m.Id, StringComparer.Ordinal).ToList(),
            changed.OrderBy(m => m.Kind, StringComparer.Ordinal).ThenBy(m => m.Id, StringComparer.Ordinal).ToList(),
            shared);
    }

    private static BundleMember Member(BundleNode o) => new(o.Kind.ToString().ToLowerInvariant(), o.Rawcode, o.Name);

    private static BundleMember Member(BundleFile f) => new(f.Category, f.Path, null);

    /// <summary>Value equality for the (kind, rawcode) object key.</summary>
    private sealed class ObjectMemberKey : IEqualityComparer<(ObjectKind Kind, string Rawcode)>
    {
        public static readonly ObjectMemberKey Instance = new();
        public bool Equals((ObjectKind Kind, string Rawcode) x, (ObjectKind Kind, string Rawcode) y) =>
            x.Kind == y.Kind && string.Equals(x.Rawcode, y.Rawcode, StringComparison.Ordinal);
        public int GetHashCode((ObjectKind Kind, string Rawcode) o) =>
            HashCode.Combine(o.Kind, StringComparer.Ordinal.GetHashCode(o.Rawcode));
    }
}
