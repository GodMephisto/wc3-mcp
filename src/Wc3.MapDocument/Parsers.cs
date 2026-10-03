// src/Wc3.MapDocument/Parsers.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Script;

namespace Wc3.Model;

public static class DefaultParsers
{
    private static bool _registered;

    public static void RegisterDefaults()
    {
        if (_registered) return;
        _registered = true;
        RegisterAll();
    }

    // Test hook: the registry is process-global, so a test that swaps in a fake
    // parser must restore the real ones to keep the suite order-independent.
    internal static void Reregister() => RegisterAll();

    private static void RegisterAll()
    {
        // Signatures confirmed by reflection against War3Net.Build.Core 6.0.3
        // (all live in War3Net.Build.Extensions.BinaryReaderExtensions).
        Wire("war3map.w3i", r => r.ReadMapInfo());
        Wire("war3map.w3e", r => r.ReadMapEnvironment());
        Wire("war3map.wpm", r => r.ReadMapPathingMap());
        Wire("war3map.doo", r => r.ReadMapDoodads());
        Wire("war3mapUnits.doo", r => r.ReadMapUnits());
        Wire("war3map.w3r", r => r.ReadMapRegions());
        Wire("war3map.w3c", r => r.ReadMapCameras());
        Wire("war3map.w3s", r => r.ReadMapSounds());
        MapFormatRegistry.Register("war3map.wtg", ReadTriggers);
        Wire("war3map.wct", r => r.ReadMapCustomTextTriggers());
        Wire("war3map.imp", r => r.ReadImportedFiles());
        Wire("war3map.w3u", r => r.ReadUnitObjectData());
        Wire("war3map.w3a", r => r.ReadAbilityObjectData());
        Wire("war3map.w3t", r => r.ReadItemObjectData());
        Wire("war3map.w3b", r => r.ReadDestructableObjectData());
        Wire("war3map.w3d", r => r.ReadDoodadObjectData());
        Wire("war3map.w3h", r => r.ReadBuffObjectData());
        Wire("war3map.w3q", r => r.ReadUpgradeObjectData());

        // Reforged skin layer: identical binary formats under war3mapSkin.*.
        Wire("war3mapSkin.w3u", r => r.ReadUnitObjectData());
        Wire("war3mapSkin.w3a", r => r.ReadAbilityObjectData());
        Wire("war3mapSkin.w3t", r => r.ReadItemObjectData());
        Wire("war3mapSkin.w3b", r => r.ReadDestructableObjectData());
        Wire("war3mapSkin.w3d", r => r.ReadDoodadObjectData());
        Wire("war3mapSkin.w3h", r => r.ReadBuffObjectData());
        Wire("war3mapSkin.w3q", r => r.ReadUpgradeObjectData());

        // wts is a text format; War3Net parses it via StreamReaderExtensions.
        MapFormatRegistry.Register("war3map.wts", raw =>
        {
            using var ms = new MemoryStream(raw);
            using var sr = new StreamReader(ms);
            return sr.ReadTriggerStrings();
        });

        // Scripts stay as plain text.
        // Latin-1, not UTF-8. A map script is a byte stream with no declared encoding, and real
        // maps carry bytes that are not valid UTF-8 (Anime_WOS2_0.28a2 has about 52,000 of them,
        // author names and localised strings). Decoding those as UTF-8 yields U+FFFD replacement
        // characters, so any later text scan is looking at content the map does not contain.
        // Latin-1 maps every byte 0..255 to the same code point and back, which is lossless.
        MapFormatRegistry.Register("war3map.j", raw => ScriptText.GetString(raw));
        MapFormatRegistry.Register("war3map.lua", raw => ScriptText.GetString(raw));
    }

    /// <summary>
    /// Reads war3map.wtg, preferring the overload that knows the World-Editor function table.
    ///
    /// The wtg format stores a trigger function's parameters but NOT how many there are, so the
    /// count comes from the table on read. Without it, a GUI trigger carrying any function at all
    /// cannot be parsed: the reader runs off the end of the stream looking for the terminator of a
    /// string that was never there. Measured, writing one action and reading it back with the
    /// plain overload fails with exactly that.
    ///
    /// Every map in the library parsed fine with the plain overload only because they are
    /// optimizer-stripped and their GUI triggers genuinely hold no functions. The moment wc3ctl
    /// writes one, or someone opens a map the World Editor actually saved, the plain overload is
    /// not enough.
    ///
    /// War3Net ships the stock table as TriggerData.Default, so this needs no game install and no
    /// reference from this project to Wc3.GameData. A map built against a CUSTOM TriggerData can
    /// still defeat it, which is why the plain overload remains as a fallback: this is never worse
    /// than what it replaced, and is better wherever functions exist.
    /// </summary>
    private static object ReadTriggers(byte[] raw)
    {
        try
        {
            return MapFormatRegistry.ReadWith(raw, r => r.ReadMapTriggers(TriggerData.Default));
        }
        catch (Exception)
        {
            return MapFormatRegistry.ReadWith(raw, r => r.ReadMapTriggers());
        }
    }

    private static void Wire<T>(string file, Func<BinaryReader, T> read) =>
        MapFormatRegistry.Register(file, raw => MapFormatRegistry.ReadWith(raw, read));
}
