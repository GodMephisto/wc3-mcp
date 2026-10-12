// src/Wc3.MapDocument/TriggerDataSource.cs
using System.Reflection;
using System.Resources;
using War3Net.Build.Script;

namespace Wc3.Model;

/// <summary>
/// The stock World Editor function table every GUI trigger read goes through. Use this, never
/// War3Net's <see cref="TriggerData.Default"/> directly.
/// </summary>
/// <remarks>
/// Why it exists. War3Net 6.1.0 and 6.1.1 add the patch 3.0.0 map formats (w3i version 39 and doo
/// version 13, which the 3.0 World Editor writes and 6.0.3 refuses), and the same releases bundle
/// a TriggerData.txt whose [TriggerParams] section defines UnitOrderChallengingCall twice. Their
/// TriggerData.Default throws on that, so with the package bumped every war3map.wtg failed to
/// parse, 210 of 248 maps in the owner's folder, and 61 tests went red. Measured 2026-10-12.
///
/// What it does. It returns War3Net's own table when that loads. When it throws, it rebuilds the
/// table from the same bundled text with any repeated key in a section kept at its first
/// definition, through the table's non-public constructor. Once upstream fixes the text the first
/// branch wins and this costs nothing.
/// </remarks>
public static class TriggerDataSource
{
    private static readonly Lazy<TriggerData> Table = new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>The stock table, loaded once.</summary>
    public static TriggerData Default => Table.Value;

    /// <summary>True when the bundled table needed repairing, false when War3Net's loaded as is.</summary>
    public static bool Repaired { get; private set; }

    private static TriggerData Load()
    {
        try { return TriggerData.Default; }
        catch (ArgumentException) { }

        var asm = typeof(TriggerData).Assembly;
        var text = new ResourceManager("War3Net.Build.Resources.War3Resources", asm).GetString("TriggerData")
            ?? throw new InvalidOperationException("War3Net no longer bundles TriggerData, update TriggerDataSource.");
        var ctor = typeof(TriggerData).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, new[] { typeof(StringReader) })
            ?? throw new InvalidOperationException("War3Net's TriggerData has no StringReader constructor, update TriggerDataSource.");
        Repaired = true;
        return (TriggerData)ctor.Invoke(new object[] { new StringReader(FirstDefinitionWins(text)) });
    }

    /// <summary>
    /// Repairs the two defects measured in War3Net 6.1.1's bundled text. Eight keys are defined twice
    /// in a section, and every later line is dropped so the first definition wins. One line,
    /// _BlzSetHDWaterMinOpacityBJ_Defaults0, lost its equals sign, and a "_Name_Defaults" line with no
    /// equals sign gets one back before the value.
    /// </summary>
    internal static string FirstDefinitionWins(string text)
    {
        var lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var raw = lines[i];
            int defaults = raw.IndexOf("_Defaults", StringComparison.Ordinal);
            if (raw.StartsWith('_') && !raw.Contains('=') && defaults > 0)
                lines[i] = raw.Insert(defaults + "_Defaults".Length, "=");
        }

        var output = new System.Text.StringBuilder(text.Length);
        int start = 0;
        while (start < lines.Length)
        {
            // One section, its header line first, up to the next header.
            int end = start + 1;
            while (end < lines.Length && !lines[end].StartsWith('[')) end++;
            // [TriggerCalls] values start with a game version and a 0 or 1 flag for "usable in
            // events", which War3Net reads as a 32 bit bool. Three 3.0 calls carry 2 there
            // (UnitItemInEquipmentSlotBJ, GetItemEquippedByHeroOfTypeBJ and its EquipmentType
            // twin), and the bool read throws. Any other value is read as 1.
            if (lines[start].TrimEnd('\r') == "[TriggerCalls]")
                for (int i = start + 1; i < end; i++)
                    lines[i] = NormalizeCallFlag(lines[i]);
            var names = new HashSet<string>(StringComparer.Ordinal);
            for (int i = start; i < end; i++)
                if (Key(lines[i]) is { } k && !k.StartsWith('_')) names.Add(k);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = start; i < end; i++)
            {
                if (Key(lines[i]) is { } k)
                {
                    if (!seen.Add(k)) continue;
                    // A property of a function the section never defines. War3Net 6.1.1 has
                    // _SetSoundFacialAnimationSetFilepathv_Defaults, a typo for ...FilepathAction,
                    // and its reader throws InvalidDataException on it.
                    int last = k.LastIndexOf('_');
                    if (k.StartsWith('_') && last > 1 && !names.Contains(k[1..last])) continue;
                }
                output.Append(lines[i]).Append('\n');
            }
            start = end;
        }
        return output.ToString(0, Math.Max(0, output.Length - 1));
    }

    private static string NormalizeCallFlag(string raw)
    {
        if (Key(raw) is not { } k || k.StartsWith('_')) return raw;
        var parts = raw.Split(',');
        if (parts.Length < 2 || parts[1] is "0" or "1") return raw;
        parts[1] = "1";
        return string.Join(",", parts);
    }

    private static string? Key(string raw)
    {
        var line = raw.TrimEnd('\r');
        if (line.StartsWith('[') || line.StartsWith("//", StringComparison.Ordinal)) return null;
        int eq = line.IndexOf('=');
        return eq > 0 ? line[..eq] : null;
    }
}
