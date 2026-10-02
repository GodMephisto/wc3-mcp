// src/Wc3.Commands/UnitAbilitiesCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One ability a unit type carries, resolved to a readable name.</summary>
public sealed record UnitTypeAbility(string Rawcode, string? Name, bool IsHeroAbility);

/// <summary>
/// Resolves the abilities a unit TYPE has (its <c>uabi</c> normal-ability list and <c>uhab</c>
/// hero-ability list in the unit object data) from cryptic rawcodes to ability names, so a
/// front-end can show "War Stomp, Reincarnation" instead of "AOws,ANrc". This is what makes a
/// character's abilities actually visible when you inspect the unit.
/// </summary>
public static class UnitAbilitiesCommand
{
    private const string NormalAbilitiesCode = "uabi"; // Abilities - Normal
    private const string HeroAbilitiesCode = "uhab";   // Abilities - Hero

    /// <summary>The unit type's abilities (normal then hero), each with its resolved name
    /// (null when unresolvable). Empty when the unit is unknown or lists no abilities.</summary>
    public static IReadOnlyList<UnitTypeAbility> ForUnitType(MapDocument doc, string unitRawcode, string? gameDir)
    {
        if (unitRawcode is null || unitRawcode.Length != 4)
            return Array.Empty<UnitTypeAbility>();

        var unit = ObjectGetCommand.Execute(doc, ObjectKind.Unit, unitRawcode, gameDir);
        if (!unit.Found)
            return Array.Empty<UnitTypeAbility>();

        var result = new List<UnitTypeAbility>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Collect(string fieldCode, bool hero)
        {
            var field = unit.Fields.FirstOrDefault(
                f => BareCode(f.Code).Equals(fieldCode, StringComparison.OrdinalIgnoreCase));
            if (field is null) return;
            foreach (var raw in SplitRawcodes(field.Value))
            {
                if (!seen.Add(raw)) continue;
                result.Add(new UnitTypeAbility(raw, ResolveAbilityName(doc, raw, gameDir), hero));
            }
        }

        Collect(NormalAbilitiesCode, hero: false);
        Collect(HeroAbilitiesCode, hero: true);
        return result;
    }

    private static string? ResolveAbilityName(MapDocument doc, string abilityRawcode, string? gameDir)
    {
        var ab = ObjectGetCommand.Execute(doc, ObjectKind.Ability, abilityRawcode, gameDir);
        return ab.Found ? ab.Name : null;
    }

    /// <summary>Splits a comma-separated ability-list field into 4-char rawcodes, dropping the
    /// World Editor's empty-slot marker.</summary>
    private static IEnumerable<string> SplitRawcodes(string? value) =>
        (value ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => s.Length == 4 && !s.Equals("_", StringComparison.Ordinal));

    private static string BareCode(string code)
    {
        int c = code.IndexOf(':');
        return c < 0 ? code : code[..c];
    }
}
