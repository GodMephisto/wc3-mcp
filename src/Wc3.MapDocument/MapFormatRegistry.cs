// src/Wc3.MapDocument/MapFormatRegistry.cs
namespace Wc3.Model;

public delegate object ParseFn(byte[] raw);

public static class MapFormatRegistry
{
    // Known WC3 map files. Parse delegates are wired in Task 8; a null delegate
    // means "known but not yet parsed into a typed model" (degrades to raw).
    private static readonly Dictionary<string, ParseFn?> _parsers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["war3map.w3e"] = null,          // terrain
        ["war3map.wpm"] = null,          // pathing
        ["war3map.doo"] = null,          // doodads
        ["war3mapUnits.doo"] = null,     // units/items
        ["war3map.w3i"] = null,          // info
        ["war3map.w3r"] = null,          // regions
        ["war3map.w3c"] = null,          // cameras
        ["war3map.w3s"] = null,          // sounds
        ["war3map.wtg"] = null,          // triggers
        ["war3map.wct"] = null,          // custom text triggers
        ["war3map.j"] = null,            // JASS script
        ["war3map.lua"] = null,          // Lua script
        ["war3map.wts"] = null,          // trigger strings
        ["war3map.imp"] = null,          // imports
        ["war3map.w3u"] = null,          // unit object data
        ["war3map.w3a"] = null,          // ability object data
        ["war3map.w3t"] = null,          // item object data
        ["war3map.w3b"] = null,          // destructable object data
        ["war3map.w3d"] = null,          // doodad object data
        ["war3map.w3h"] = null,          // buff object data
        ["war3map.w3q"] = null,          // upgrade object data
        // Reforged skin object-data layer: same binary formats as war3map.*.
        ["war3mapSkin.w3u"] = null,      // unit object data (skin)
        ["war3mapSkin.w3a"] = null,      // ability object data (skin)
        ["war3mapSkin.w3t"] = null,      // item object data (skin)
        ["war3mapSkin.w3b"] = null,      // destructable object data (skin)
        ["war3mapSkin.w3d"] = null,      // doodad object data (skin)
        ["war3mapSkin.w3h"] = null,      // buff object data (skin)
        ["war3mapSkin.w3q"] = null,      // upgrade object data (skin)
        // Added by Reforged 3.0.0 (build 24268). war3map.w3l is the new lighting editor's
        // output, magic "W3L!" version 3, and war3mapPostProcessing.txt backs the map-level
        // post-processing tool. Neither appears in any of the 256 maps on this machine, and
        // war3map.w3l appears in exactly one of the 465 maps the build itself ships.
        ["war3map.w3l"] = null,                 // lights
        ["war3mapPostProcessing.txt"] = null,   // post-processing settings
        // Present in shipped maps and never registered, so they read as user imports.
        ["war3map.w3grp"] = null,
        ["war3map.soundasset"] = null,
        ["conversation.json"] = null,           // FaceFX conversation data
    };

    /// <summary>
    /// A Reforged map can carry a per-locale override layer, "_Locales\deDE.w3mod\war3map.wts"
    /// beside the neutral "war3map.wts". The inner name is an ordinary map file, so the layer is
    /// stripped before the lookup rather than registering one entry per locale per format.
    /// Blizzard's own maps ship thirteen of these, and without this they all read as imports.
    /// </summary>
    private static string StripLocaleLayer(string fileName)
    {
        const string prefix = "_locales";
        if (!fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return fileName;

        // Take everything after the last separator, which is the file inside the layer.
        int slash = fileName.LastIndexOfAny(new[] { '\\', '/' });
        return slash >= 0 && slash + 1 < fileName.Length ? fileName[(slash + 1)..] : fileName;
    }

    public static bool IsKnown(string fileName) =>
        _parsers.ContainsKey(fileName) || _parsers.ContainsKey(StripLocaleLayer(fileName));

    public static bool TryGetParser(string fileName, out ParseFn parser)
    {
        if (_parsers.TryGetValue(fileName, out var p) && p is not null)
        {
            parser = p;
            return true;
        }
        parser = _ => throw new InvalidOperationException();
        return false;
    }

    internal static void Register(string fileName, ParseFn parser) => _parsers[fileName] = parser;

    public static object ReadWith<T>(byte[] raw, Func<BinaryReader, T> read)
    {
        using var ms = new MemoryStream(raw);
        using var reader = new BinaryReader(ms);
        return read(reader) ?? throw new InvalidDataException($"Parser for {typeof(T).Name} returned null.");
    }
}
