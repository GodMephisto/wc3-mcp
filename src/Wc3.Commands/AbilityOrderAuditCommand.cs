// src/Wc3.Commands/AbilityOrderAuditCommand.cs
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One ability that shares an order string with another.</summary>
public sealed record AbilityOrderMember(string Rawcode, string? Name);

/// <summary>An order string used by more than one ability. On one unit this means
/// casting either one can fire both, because the game dispatches by order id.</summary>
public sealed record AbilityOrderGroup(string Order, IReadOnlyList<AbilityOrderMember> Members);

/// <summary>A single ability whose order data is unusable or ambiguous.</summary>
public sealed record AbilityOrderProblem(string Kind, string Rawcode, string? Name, string Detail);

/// <summary>
/// Result of the order audit. ChannelAbilities counts the abilities derived from the
/// Channel base, DistinctOrders counts the order strings they use, and Collisions lists
/// every string used more than once.
/// </summary>
public sealed record AbilityOrderAuditResult(
    int ChannelAbilities, int DistinctOrders,
    IReadOnlyList<AbilityOrderGroup> Collisions,
    IReadOnlyList<AbilityOrderProblem> Problems,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Finds Channel-based abilities that share a Base Order ID. Channel is the custom-ability
/// base in most anime maps, and its order string is what the engine dispatches on, so two
/// abilities on one unit with the same string can both fire from one key press. This is a
/// read of the map's own object data only, no game install and no script execution.
///
/// With a hero rawcode, only the abilities that hero's uabi list names are checked, which is
/// the precise case. Without one, every Channel ability in the map is grouped, which finds
/// the collisions a hero-level check would find plus the ones across heroes.
/// </summary>
public static class AbilityOrderAuditCommand
{
    /// <summary>The Channel base rawcode. Every custom Channel ability derives from it.</summary>
    private const string ChannelBase = "ANcl";

    /// <summary>The order a Channel ability uses when it declares none.</summary>
    private const string ChannelDefault = "channel";

    public static AbilityOrderAuditResult Execute(MapDocument doc, string? heroRawcode = null)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var abilities = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability));
        var byId = abilities.ToDictionary(e => e.Id);
        var diagnostics = new List<string>();

        var scope = abilities;
        if (!string.IsNullOrWhiteSpace(heroRawcode))
        {
            var hero = heroRawcode.Trim();
            if (hero.Length != 4)
                return new(0, 0, Array.Empty<AbilityOrderGroup>(), Array.Empty<AbilityOrderProblem>(),
                    new[] { $"hero rawcode must be four characters, got '{hero}'" });

            var units = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit));
            var unit = units.FirstOrDefault(u => u.Id == hero.FromRawcode());
            if (unit is null)
                return new(0, 0, Array.Empty<AbilityOrderGroup>(), Array.Empty<AbilityOrderProblem>(),
                    new[] { $"'{hero}' is not in the map's own unit object data" });

            var mods = ObjectKinds.ModsToDict(unit.Mods);
            if (!mods.TryGetValue("uabi", out var list) || string.IsNullOrWhiteSpace(list))
                return new(0, 0, Array.Empty<AbilityOrderGroup>(), Array.Empty<AbilityOrderProblem>(),
                    new[] { $"'{hero}' has no uabi ability list in the map's data" });

            var wanted = list.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => x.Length == 4)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            scope = abilities.Where(a => wanted.Contains(a.Id.ToRawcode())).ToList();

            var missing = wanted.Where(w => !scope.Any(a => string.Equals(a.Id.ToRawcode(), w, StringComparison.OrdinalIgnoreCase))).ToList();
            if (missing.Count > 0)
                diagnostics.Add($"uabi names {missing.Count} ability rawcodes that are not in the map's object data: {string.Join(", ", missing)}");
        }

        var channel = scope.Where(a => IsChannel(a, byId)).ToList();
        if (channel.Count == 0)
            return new(0, 0, Array.Empty<AbilityOrderGroup>(), Array.Empty<AbilityOrderProblem>(), diagnostics);

        var problems = new List<AbilityOrderProblem>();
        var members = new List<(string Order, AbilityOrderMember Member)>();

        foreach (var entry in channel)
        {
            string rawcode = entry.Id.ToRawcode();
            var mods = ObjectKinds.ModsToDict(entry.Mods);
            string? name = mods.TryGetValue("anam", out var n) && !string.IsNullOrWhiteSpace(n) ? n.Trim() : null;

            var values = entry.Mods
                .Where(kv => kv.Key.StartsWith("Ncl6", StringComparison.OrdinalIgnoreCase))
                .Select(kv => (kv.Key, Value: kv.Value.Trim()))
                .ToList();

            if (values.Count == 0)
            {
                members.Add((ChannelDefault, new AbilityOrderMember(rawcode, name)));
                continue;
            }

            var distinct = values.Select(v => v.Value)
                .Where(v => v.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (distinct.Count == 0)
            {
                problems.Add(new("empty-order", rawcode, name, "every level sets an empty Base Order ID"));
                continue;
            }

            if (distinct.Count > 1)
            {
                problems.Add(new("levels-disagree", rawcode, name,
                    "levels use different orders: " + string.Join(", ", distinct)));
            }

            bool anyEmpty = values.Any(v => v.Value.Length == 0);
            if (anyEmpty)
                problems.Add(new("empty-level", rawcode, name, "at least one level sets an empty Base Order ID"));

            members.Add((distinct[0], new AbilityOrderMember(rawcode, name)));
        }

        var groups = members
            .GroupBy(m => m.Order, StringComparer.OrdinalIgnoreCase)
            .Select(g => new AbilityOrderGroup(g.Key, g.Select(x => x.Member).ToList()))
            .Where(g => g.Members.Count > 1)
            .OrderByDescending(g => g.Members.Count)
            .ThenBy(g => g.Order, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new(channel.Count, members.Select(m => m.Order).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            groups, problems, diagnostics);
    }

    /// <summary>True when the ability's derivation chain reaches the Channel base, directly
    /// or through another custom ability.</summary>
    private static bool IsChannel(MapObjectEntry entry, IReadOnlyDictionary<int, MapObjectEntry> byId)
    {
        var current = entry;
        for (int hop = 0; hop < 8; hop++)
        {
            string? baseCode = current.OldId == 0 ? null : current.OldId.ToRawcode();
            if (string.Equals(baseCode, ChannelBase, StringComparison.OrdinalIgnoreCase)) return true;
            if (baseCode is null || baseCode.Length != 4) return false;
            if (!byId.TryGetValue(baseCode.FromRawcode(), out var next)) return false;
            if (next.Id == current.Id) return false;
            current = next;
        }
        return false;
    }
}
