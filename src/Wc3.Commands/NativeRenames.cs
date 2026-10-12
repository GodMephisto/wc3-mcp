// src/Wc3.Commands/NativeRenames.cs
namespace Wc3.Commands;

/// <summary>
/// The native renames the modernize pass applies, old spelling to the target's spelling.
///
/// This is a small curated table, not a complete history of the API. The pairs are the 1.29 PTR
/// natives that went live carrying a Blz prefix, Blizzard renamed SetUnitBaseDamage and its
/// siblings to BlzSetUnitBaseDamage and friends when the patch shipped. A name the table does not
/// hold is left exactly as the author wrote it, and a mapping is only applied when the target
/// build actually declares the new spelling, so the pass cannot invent a name a build lacks.
///
/// The table is deliberately short. A wrong pair would rewrite a working call to a name the build
/// does not have, which is worse than leaving a deprecated one alone, so a name is only added here
/// when it is known.
/// </summary>
internal static class NativeRenames
{
    /// <summary>Old native name to the target build's spelling, when the mapping is known.</summary>
    internal static readonly IReadOnlyList<(string From, string To)> All = new[]
    {
        ("SetUnitBaseDamage", "BlzSetUnitBaseDamage"),
        ("GetUnitBaseDamage", "BlzGetUnitBaseDamage"),
        ("SetUnitDiceNumber", "BlzSetUnitDiceNumber"),
        ("GetUnitDiceNumber", "BlzGetUnitDiceNumber"),
        ("SetUnitDiceSides", "BlzSetUnitDiceSides"),
        ("GetUnitDiceSides", "BlzGetUnitDiceSides"),
        ("SetUnitAttackCooldown", "BlzSetUnitAttackCooldown"),
        ("GetUnitAttackCooldown", "BlzGetUnitAttackCooldown"),
        ("SetUnitArmor", "BlzSetUnitArmor"),
        ("GetUnitArmor", "BlzGetUnitArmor"),
    };
}
