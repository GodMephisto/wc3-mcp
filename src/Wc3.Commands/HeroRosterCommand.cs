// src/Wc3.Commands/HeroRosterCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The playable heroes a map registers, and which trigger holds each one's code.
/// </summary>
/// <remarks>
/// <para>The question this answers is "how many heroes does this map have, and where does each
/// one live", and until now nothing surfaced it. Measured on GGGA_V0.05, the map registers 170
/// heroes and its trigger tree carries 39 Character triggers, so counting triggers to count
/// heroes is off by a factor of four. The heroes are not missing, they are BUNDLED, four
/// grouped triggers hold 4.27 million characters of hero code between them while 35 heroes got
/// a trigger of their own.</para>
///
/// <para>That mismatch reads as data loss and is not. A hero is not a trigger, and no count of
/// one tells you the other. What a reader actually needs is the roster, with the trigger that
/// defines each hero named next to it.</para>
///
/// <para>The roster comes from <see cref="ContractCommand"/> rather than a pattern invented
/// here, because it already detects registration calls generically across maps that use
/// entirely different function names. Which trigger owns a hero is found by looking for the
/// hero's rawcode literal in each custom-text body, which is how the code refers to it.</para>
/// </remarks>
public static class HeroRosterCommand
{
    /// <summary>One registered hero.</summary>
    public sealed record RosterHero(
        string Rawcode,
        /// <summary>Display name from the map's object data, empty when the rawcode has no unit.</summary>
        string Name,
        /// <summary>String arguments from the registration call, typically a role and a portrait.</summary>
        IReadOnlyList<string> Tags,
        /// <summary>Trigger whose body mentions this rawcode, or null when none does.</summary>
        string? Trigger,
        /// <summary>How many triggers mention it, so an ambiguous owner is visible rather than guessed.</summary>
        int TriggerMatches);

    public sealed record RosterResult(
        bool Ok,
        string Message,
        /// <summary>The registration function the roster was read from.</summary>
        string RegistryFunction,
        IReadOnlyList<RosterHero> Heroes);

    private static readonly Regex Quoted = new("\"([^\"]*)\"", RegexOptions.Compiled);

    /// <summary>
    /// Reads the map's hero roster and attributes each hero to the trigger that defines it.
    /// </summary>
    public static RosterResult Run(MapDocument doc, string? gameDirOverride = null)
    {
        ContractResult contract;
        try
        {
            contract = ContractCommand.Run(doc);
        }
        catch (Exception ex)
        {
            return new(false, $"could not read the map's script ({ex.GetType().Name}: {ex.Message})",
                string.Empty, Array.Empty<RosterHero>());
        }

        // Names come from the map's own object data, the same source the object list uses. This
        // has to happen BEFORE the roster is chosen, because whether a call's rawcodes are units
        // at all is the thing that separates a roster from a native.
        var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var units = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var item in ObjectListCommand.Execute(doc, ObjectKind.Unit, gameDirOverride).Items)
            {
                units.Add(item.Rawcode);
                if (!string.IsNullOrEmpty(item.Name)) names[item.Rawcode] = item.Name!;
            }
        }
        catch { /* a roster without names is still a roster, selection falls back to count */ }

        // The roster is the call whose rawcodes are actually UNITS, not the one with the most
        // rawcodes. Counting alone picks the wrong function and does it narrowly enough to look
        // right: on GGGA_V0.05 the native SetUnitAbilityLevelSwapped carries 172 distinct
        // rawcodes against RPB_AddHero's 170 and wins by two, except that its rawcodes are
        // ABILITY codes and not one of them resolves to a unit. Scoring by unit resolution
        // separates them 170 to 0 rather than 172 to 170.
        var registry = contract.Registries
            .OrderByDescending(r => units.Count == 0 ? 0 : r.RegisteredRawcodes.Count(units.Contains))
            .ThenByDescending(r => r.RegisteredRawcodes.Count)
            .FirstOrDefault();

        if (registry is null || registry.RegisteredRawcodes.Count == 0)
            return new(false,
                "no roster registration call was found in this map's script, so there is no "
                + "list of playable heroes to read. Maps that create heroes inline rather than "
                + "registering them have no roster to find.",
                string.Empty, Array.Empty<RosterHero>());

        // Each custom-text body, so a rawcode can be attributed to the trigger that mentions it.
        var bodies = new List<(string Name, string Body)>();
        try
        {
            foreach (var t in TriggerReadCommand.GetTriggers(doc).Triggers)
                if (!string.IsNullOrEmpty(t.CustomText))
                    bodies.Add((t.Name, t.CustomText!));
        }
        catch { /* no tree, every hero simply reports no owning trigger */ }

        // One pass per body rather than one per hero. With 170 heroes and 79 bodies of 11.8
        // million characters, the naive nesting is 170 full scans of the whole script.
        var owners = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var wanted = new HashSet<string>(registry.RegisteredRawcodes, StringComparer.Ordinal);
        foreach (var (name, body) in bodies)
        {
            foreach (Match m in Regex.Matches(body, "'([^']{4})'"))
            {
                string code = m.Groups[1].Value;
                if (!wanted.Contains(code)) continue;
                if (!owners.TryGetValue(code, out var list))
                    owners[code] = list = new List<string>();
                if (!list.Contains(name)) list.Add(name);
            }
        }

        var byRawcode = registry.Calls
            .GroupBy(c => c.Rawcode, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First().Line, StringComparer.Ordinal);

        var heroes = new List<RosterHero>(registry.RegisteredRawcodes.Count);
        foreach (var code in registry.RegisteredRawcodes)
        {
            var tags = byRawcode.TryGetValue(code, out var line)
                ? Quoted.Matches(line).Select(m => m.Groups[1].Value)
                    .Where(s => s.Length > 0).ToList()
                : new List<string>();

            owners.TryGetValue(code, out var owning);
            heroes.Add(new RosterHero(
                code,
                names.TryGetValue(code, out var n) ? n : string.Empty,
                tags,
                owning is { Count: > 0 } ? owning[0] : null,
                owning?.Count ?? 0));
        }

        int placed = heroes.Count(h => h.Trigger is not null);
        return new(true,
            $"{heroes.Count} hero(es) registered via {registry.Function}, "
            + $"{placed} attributed to a trigger and {heroes.Count - placed} not found in any "
            + "trigger body (their code lives in the compiled script rather than the tree)",
            registry.Function,
            heroes);
    }
}
