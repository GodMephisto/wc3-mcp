// src/Wc3.MapDocument/Parsers.cs
using System.Text;
using War3Net.Build.Extensions;

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
        Wire("war3map.wtg", r => r.ReadMapTriggers());
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
        MapFormatRegistry.Register("war3map.j", raw => Encoding.UTF8.GetString(raw));
        MapFormatRegistry.Register("war3map.lua", raw => Encoding.UTF8.GetString(raw));
    }

    private static void Wire<T>(string file, Func<BinaryReader, T> read) =>
        MapFormatRegistry.Register(file, raw => MapFormatRegistry.ReadWith(raw, read));
}
