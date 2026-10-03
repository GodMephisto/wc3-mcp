// src/Wc3.Commands/HeroStatConvention.cs
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One field of a target map's hero stat convention, with the evidence for it: how many of the
/// map's own heroes were sampled, and how many of them state this exact value.
/// </summary>
public sealed record StatConventionField(string Code, string Value, int Samples, int Agreement)
{
    /// <summary>Every sampled hero states the same value, so this is the map's rule rather than
    /// the most popular of several choices.</summary>
    public bool Unanimous => Agreement == Samples;

    public string Evidence => Unanimous
        ? $"{Code}={Value} (all {Samples} sampled hero(es) of the target)"
        : $"{Code}={Value} (the most common value, {Agreement} of {Samples} sampled hero(es))";
}

/// <summary>
/// What the target map's own heroes agree a hero's stats look like, measured from the heroes it
/// already ships so a ported hero can be brought onto the target's terms instead of keeping the
/// source map's.
/// </summary>
/// <remarks>
/// Two maps can both be hero arenas and disagree completely about where a hero's durability comes
/// from. Shadow Nanaya's home map drives it from attributes: she has strength 55, agility 72,
/// intelligence 50 and a 100 point hit-point pool that those attributes inflate. GGGA does the
/// opposite, every one of its 167 registered heroes has strength, agility and intelligence pinned
/// at exactly 0 and states a flat pool instead (its Stalkers cluster on 3600 hit points and 1000
/// mana). Installed unchanged, Nanaya arrives in GGGA with a 100 point pool and attributes the map
/// does not use, which is not a balance problem, it is a hero that dies instantly.
///
/// The numbers are the target's, not ours, so they are measured rather than written down here. A
/// hardcoded 3600 would be wrong on the next map and would silently stay wrong.
///
/// Only fields whose sampled values are all integers are offered. A brand new modification's data
/// type is inferred from the value's shape by <see cref="ObjectSetCommand"/>, and an Int written
/// where the game expects a Real stores a wrong number with no error, so a real-typed field such as
/// attribute-per-level growth is deliberately left out rather than risked.
/// </remarks>
public sealed record HeroStatConvention(string SampleDescription, IReadOnlyList<StatConventionField> Fields)
{
    /// <summary>
    /// The fields that define the stat MODEL, as opposed to a hero's individual tuning. These are
    /// the ones that are meaningless when carried across maps that model heroes differently.
    /// All five are integer-typed in the Object Editor, which is what makes them safe to write.
    /// </summary>
    private static readonly string[] ModelFields =
        { "ustr", "uagi", "uint", "uhpm", "umpm" };

    /// <summary>Fewer sampled heroes than this and a "convention" is just a couple of examples.</summary>
    private const int MinSamples = 8;

    /// <summary>
    /// A field most of the target's heroes never state is not part of its convention, it is
    /// whatever the base unit happened to come with. Only near-universal fields qualify.
    /// </summary>
    private const double MinCoverage = 0.8;

    /// <summary>
    /// Measures the convention from the heroes <paramref name="roster"/> already registers.
    ///
    /// When <paramref name="role"/> is given, only the target's heroes registered under that same
    /// role are sampled, which is both more accurate and more honest: GGGA's Tankers sit around
    /// 4500 hit points and its Stalkers around 3600, so sampling the whole roster would hand a
    /// Stalker a number no Stalker uses. Falls back to the whole roster when too few heroes share
    /// the role to measure anything.
    /// </summary>
    public static HeroStatConvention? Derive(MapDocument target, RosterRegistry roster, string? role)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(roster);

        var sample = SelectSample(roster, role, out var description);
        if (sample.Count < MinSamples) return null;

        // The target's own deltas only. A field the map does not state is inherited from the base
        // unit, and inherited values are precisely what a convention is not.
        var info = ObjectKinds.Info(ObjectKind.Unit);
        var deltas = new List<Dictionary<string, string>>();
        foreach (var entry in ObjectKinds.MergedEntries(target, info))
        {
            var code = entry.Id.ToRawcode();
            if (sample.Contains(code)) deltas.Add(ObjectKinds.ModsToDict(entry.Mods));
        }
        if (deltas.Count < MinSamples) return null;

        var fields = new List<StatConventionField>();
        foreach (var code in ModelFields)
        {
            var stated = deltas.Select(d => d.TryGetValue(code, out var v) ? v : null)
                .Where(v => v is not null).Select(v => v!).ToList();
            if (stated.Count < deltas.Count * MinCoverage) continue;
            // Integer-valued only, see the type-inference note on this class.
            if (stated.Any(v => !int.TryParse(v, System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture, out _))) continue;

            // Most common value, ties broken toward the smaller number so the choice is
            // deterministic rather than dependent on the order objects happen to sit in the file.
            var best = stated.GroupBy(v => v, StringComparer.Ordinal)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => int.Parse(g.Key, System.Globalization.CultureInfo.InvariantCulture))
                .First();
            fields.Add(new StatConventionField(code, best.Key, deltas.Count, best.Count()));
        }

        return fields.Count == 0 ? null
            : new HeroStatConvention($"{deltas.Count} {description}", fields);
    }

    /// <summary>
    /// The rawcodes to measure. A role narrows it when the target's registration call carries the
    /// role as one of its arguments, which is found by looking for the role as a quoted argument on
    /// the same line rather than by assuming an argument position.
    /// </summary>
    private static HashSet<string> SelectSample(RosterRegistry roster, string? role, out string description)
    {
        if (role is { Length: > 0 })
        {
            var quoted = "\"" + role + "\"";
            var matched = roster.Calls
                .Where(c => c.Line.Contains(quoted, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Rawcode)
                .ToHashSet(StringComparer.Ordinal);
            if (matched.Count >= MinSamples)
            {
                description = $"hero(es) the target registers as \"{role}\"";
                return matched;
            }
        }
        description = "hero(es) the target registers";
        return roster.RegisteredRawcodes.ToHashSet(StringComparer.Ordinal);
    }
}
