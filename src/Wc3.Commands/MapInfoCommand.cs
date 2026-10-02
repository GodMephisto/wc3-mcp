// src/Wc3.Commands/MapInfoCommand.cs
using System.Drawing;
using System.Globalization;
using Wc3.Model;
using War3Net.Build.Common;
using War3Net.Build.Extensions;
using War3Net.Build.Info;

namespace Wc3.Commands;

/// <summary>Editable snapshot of war3map.w3i. Text fields (name, author, description,
/// loading/prologue) are resolved against war3map.wts so callers show real text, not the raw
/// TRIGSTR_ key. Front-ends diff against this snapshot and only write changed fields, so an
/// untouched TRIGSTR reference is preserved on save. Colors are "R,G,B,A" byte strings.
/// Tileset, light environment, weather and fog style carry the War3Net enum name (an unknown
/// weather id degrades to its raw 4-letter code).
/// Fields the map's w3i format version predates read as defaults, and setting them
/// does not change the file bytes (the writer only emits what the version carries).
/// CameraBounds and the playable dimensions are informational and read-only.</summary>
public sealed record MapInfoFields(
    // identity
    string MapName,
    string Author,
    string Description,
    string RecommendedPlayers,
    // structure (read-only)
    int PlayableWidth,
    int PlayableHeight,
    int Players,
    string CameraBounds,
    // environment
    string Tileset,
    string LightEnvironment,
    string GlobalWeather,
    string SoundEnvironment,
    string WaterTintColor,
    // fog
    string FogStyle,
    float FogStartZ,
    float FogEndZ,
    float FogDensity,
    string FogColor,
    // loading screen
    int LoadingScreenIndex,
    string LoadingScreenPath,
    string LoadingScreenTitle,
    string LoadingScreenSubtitle,
    string LoadingScreenText,
    // prologue
    string PrologueTitle,
    string PrologueSubtitle,
    string PrologueText,
    // map options (individual MapFlags bits)
    bool MeleeMap,
    bool HideMinimapInPreview,
    bool ModifyAllyPriorities,
    bool MaskedAreasPartiallyVisible,
    bool FixedPlayerSettings,
    bool UseCustomForces,
    bool UseCustomTechtree,
    bool UseCustomAbilities,
    bool UseCustomUpgrades,
    bool WaterWavesOnCliffShores,
    bool WaterWavesOnRollingShores,
    bool HasTerrainFog,
    bool HasWaterTint,
    bool ItemClassification,
    bool AccurateProbabilities);

/// <summary>One picker choice for an enum-backed field: Name is the value
/// <see cref="MapInfoCommand.Set"/> accepts (and <see cref="MapInfoCommand.Read"/>
/// returns), Id is the short World-Editor style code shown beside it.</summary>
public sealed record FieldChoice(string Name, string Id);

/// <summary>
/// Read/write access to the map-info file (war3map.w3i). The write side mutates the
/// already-parsed War3Net <see cref="MapInfo"/> model and re-serializes the WHOLE file
/// via War3Net's writer, so every field the parser captured — including the Unk*
/// unknowns — is re-emitted; untouched fields survive byte-for-byte
/// (<c>MapInfoCommandTests.Unchanged_reserialize_is_byte_identical</c> pins this).
/// MapDocument.SerializeEntry has no MapInfo case, so the bytes go back through
/// <see cref="MapDocument.AddOrReplaceRawFile"/> (raw payloads are written verbatim on Save).
/// </summary>
public static class MapInfoCommand
{
    public const string FileName = "war3map.w3i";

    /// <summary>Map Options checkboxes: Set/Read field name to its MapFlags bit.
    /// <see cref="Set"/> accepts true/false (or 1/0) for these.</summary>
    private static readonly IReadOnlyDictionary<string, MapFlags> FlagFields =
        new Dictionary<string, MapFlags>(StringComparer.OrdinalIgnoreCase)
        {
            ["MeleeMap"] = MapFlags.MeleeMap,
            ["HideMinimapInPreview"] = MapFlags.HideMinimapInPreviewScreens,
            ["ModifyAllyPriorities"] = MapFlags.ModifyAllyPriorities,
            ["MaskedAreasPartiallyVisible"] = MapFlags.MaskedAreasArePartiallyVisible,
            ["FixedPlayerSettings"] = MapFlags.FixedPlayerSettingsForCustomForces,
            ["UseCustomForces"] = MapFlags.UseCustomForces,
            ["UseCustomTechtree"] = MapFlags.UseCustomTechtree,
            ["UseCustomAbilities"] = MapFlags.UseCustomAbilities,
            ["UseCustomUpgrades"] = MapFlags.UseCustomUpgrades,
            ["WaterWavesOnCliffShores"] = MapFlags.ShowWaterWavesOnCliffShores,
            ["WaterWavesOnRollingShores"] = MapFlags.ShowWaterWavesOnRollingShores,
            ["HasTerrainFog"] = MapFlags.HasTerrainFog,
            ["HasWaterTint"] = MapFlags.HasWaterTintingColor,
            ["ItemClassification"] = MapFlags.UseItemClassificationSystem,
            ["AccurateProbabilities"] = MapFlags.AccurateProbabilityForCalculations,
        };

    /// <summary>Field names <see cref="Set"/> accepts (matched case-insensitively).
    /// Structural values (dimensions, player list, camera bounds) are read-only.</summary>
    public static readonly IReadOnlyList<string> EditableFields = new[]
    {
        "MapName", "Author", "Description", "RecommendedPlayers",
        "Tileset", "LightEnvironment", "GlobalWeather", "SoundEnvironment", "WaterTintColor",
        "FogStyle", "FogStartZ", "FogEndZ", "FogDensity", "FogColor",
        "LoadingScreenIndex", "LoadingScreenPath", "LoadingScreenTitle",
        "LoadingScreenSubtitle", "LoadingScreenText",
        "PrologueTitle", "PrologueSubtitle", "PrologueText",
    }.Concat(FlagFields.Keys).ToList();

    /// <summary>Tileset (and light environment) picker choices. Id is the WE letter.</summary>
    public static IReadOnlyList<FieldChoice> TilesetChoices { get; } =
        Enum.GetValues<Tileset>()
            .Select(t => new FieldChoice(t.ToString(),
                t == Tileset.Unspecified ? t.ToString() : ((char)t).ToString()))
            .ToList();

    /// <summary>Global weather picker choices. Id is the raw 4-letter weather code.</summary>
    public static IReadOnlyList<FieldChoice> WeatherChoices { get; } =
        Enum.GetValues<WeatherType>()
            .Select(w => new FieldChoice(w.ToString(),
                w == WeatherType.None ? w.ToString() : FourCC((int)w)))
            .ToList();

    /// <summary>Fog style picker choices. Id is the numeric style the file stores.</summary>
    public static IReadOnlyList<FieldChoice> FogStyleChoices { get; } =
        Enum.GetValues<FogStyle>()
            .Select(s => new FieldChoice(s.ToString(),
                ((int)s).ToString(CultureInfo.InvariantCulture)))
            .ToList();

    public static MapInfoFields Read(MapDocument doc) => ToFields(GetInfo(doc), MapStrings.From(doc));

    /// <summary>Sets one editable field and writes the re-serialized w3i back into the
    /// in-memory document (persisted on the next Save). Returns the updated snapshot.</summary>
    public static MapInfoFields Set(MapDocument doc, string field, string value)
    {
        var info = GetInfo(doc);
        ApplyField(info, field, value);
        var entry = doc.AddOrReplaceRawFile(FileName, Serialize(info));
        // AddOrReplaceRawFile drops the parsed model (raw payload wins on Save); restore
        // it so in-memory readers keep seeing the mutated MapInfo. The two stay
        // consistent — the override bytes were serialized from this very model.
        entry.Model = info;
        return ToFields(info, MapStrings.From(doc));
    }

    /// <summary>Pure field mapping onto the War3Net model (hermetically testable).
    /// Values arrive as strings: floats/ints invariant, colors "R,G,B[,A]", flags
    /// true/false, enums by name (tilesets also by letter, weather also by 4cc).</summary>
    public static void ApplyField(MapInfo info, string field, string value)
    {
        if (FlagFields.TryGetValue(field, out var bit))
        {
            info.MapFlags = ParseBool(field, value)
                ? info.MapFlags | bit
                : info.MapFlags & ~bit;
            return;
        }

        switch (field.ToLowerInvariant())
        {
            case "mapname": info.MapName = value; break;
            case "author": info.MapAuthor = value; break;
            case "description": info.MapDescription = value; break;
            case "recommendedplayers": info.RecommendedPlayers = value; break;
            case "tileset": info.Tileset = ParseTileset(field, value); break;
            case "lightenvironment": info.LightEnvironment = ParseTileset(field, value); break;
            case "globalweather": info.GlobalWeather = ParseWeather(value); break;
            case "soundenvironment": info.SoundEnvironment = value; break;
            case "watertintcolor": info.WaterTintingColor = ParseColor(field, value); break;
            case "fogstyle": info.FogStyle = ParseEnum<FogStyle>(field, value); break;
            case "fogstartz": info.FogStartZ = ParseFloat(field, value); break;
            case "fogendz": info.FogEndZ = ParseFloat(field, value); break;
            case "fogdensity": info.FogDensity = ParseFloat(field, value); break;
            case "fogcolor": info.FogColor = ParseColor(field, value); break;
            case "loadingscreenindex": info.LoadingScreenBackgroundNumber = ParseInt(field, value); break;
            case "loadingscreenpath": info.LoadingScreenPath = value; break;
            case "loadingscreentitle": info.LoadingScreenTitle = value; break;
            case "loadingscreensubtitle": info.LoadingScreenSubtitle = value; break;
            case "loadingscreentext": info.LoadingScreenText = value; break;
            case "prologuetitle": info.PrologueScreenTitle = value; break;
            case "prologuesubtitle": info.PrologueScreenSubtitle = value; break;
            case "prologuetext": info.PrologueScreenText = value; break;
            default:
                throw new ArgumentException(
                    $"Unknown or read-only field '{field}'. Editable fields: {string.Join(", ", EditableFields)}.",
                    nameof(field));
        }
    }

    /// <summary>Serializes a MapInfo with War3Net's writer (the exact inverse of the
    /// ReadMapInfo parser wired in DefaultParsers).</summary>
    public static byte[] Serialize(MapInfo info)
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            writer.Write(info);
        return ms.ToArray();
    }

    private static MapInfo GetInfo(MapDocument doc) =>
        doc.GetFile(FileName)?.Model as MapInfo
        ?? throw new InvalidOperationException(
            $"{FileName} is missing or could not be parsed; map info is not editable.");

    // The identity/loading/prologue fields can be TRIGSTR_ references into war3map.wts (the
    // World Editor stores "Gutsy Geoid Game" as e.g. TRIGSTR_4084). Resolve them for display
    // so the panel shows real text, not the raw key - the reason Map Info looked "erased".
    // Non-TRIGSTR values pass through unchanged, so codes/hex fields stay as-is.
    private static MapInfoFields ToFields(MapInfo info, MapStrings strings)
    {
        string R(string? s) => strings.Resolve(s ?? string.Empty);
        return new(
        MapName: R(info.MapName),
        Author: R(info.MapAuthor),
        Description: R(info.MapDescription),
        RecommendedPlayers: R(info.RecommendedPlayers),
        PlayableWidth: info.PlayableMapAreaWidth,
        PlayableHeight: info.PlayableMapAreaHeight,
        Players: info.Players?.Count ?? 0,
        CameraBounds: CameraBoundsText(info),
        Tileset: TilesetName(info.Tileset),
        LightEnvironment: TilesetName(info.LightEnvironment),
        GlobalWeather: WeatherName(info.GlobalWeather),
        SoundEnvironment: info.SoundEnvironment ?? string.Empty,
        WaterTintColor: ColorText(info.WaterTintingColor),
        FogStyle: info.FogStyle.ToString(),
        FogStartZ: info.FogStartZ,
        FogEndZ: info.FogEndZ,
        FogDensity: info.FogDensity,
        FogColor: ColorText(info.FogColor),
        LoadingScreenIndex: info.LoadingScreenBackgroundNumber,
        LoadingScreenPath: info.LoadingScreenPath ?? string.Empty,
        LoadingScreenTitle: R(info.LoadingScreenTitle),
        LoadingScreenSubtitle: R(info.LoadingScreenSubtitle),
        LoadingScreenText: R(info.LoadingScreenText),
        PrologueTitle: R(info.PrologueScreenTitle),
        PrologueSubtitle: R(info.PrologueScreenSubtitle),
        PrologueText: R(info.PrologueScreenText),
        MeleeMap: Has(info, MapFlags.MeleeMap),
        HideMinimapInPreview: Has(info, MapFlags.HideMinimapInPreviewScreens),
        ModifyAllyPriorities: Has(info, MapFlags.ModifyAllyPriorities),
        MaskedAreasPartiallyVisible: Has(info, MapFlags.MaskedAreasArePartiallyVisible),
        FixedPlayerSettings: Has(info, MapFlags.FixedPlayerSettingsForCustomForces),
        UseCustomForces: Has(info, MapFlags.UseCustomForces),
        UseCustomTechtree: Has(info, MapFlags.UseCustomTechtree),
        UseCustomAbilities: Has(info, MapFlags.UseCustomAbilities),
        UseCustomUpgrades: Has(info, MapFlags.UseCustomUpgrades),
        WaterWavesOnCliffShores: Has(info, MapFlags.ShowWaterWavesOnCliffShores),
        WaterWavesOnRollingShores: Has(info, MapFlags.ShowWaterWavesOnRollingShores),
        HasTerrainFog: Has(info, MapFlags.HasTerrainFog),
        HasWaterTint: Has(info, MapFlags.HasWaterTintingColor),
        ItemClassification: Has(info, MapFlags.UseItemClassificationSystem),
        AccurateProbabilities: Has(info, MapFlags.AccurateProbabilityForCalculations));
    }

    private static bool Has(MapInfo info, MapFlags bit) => (info.MapFlags & bit) != 0;

    /// <summary>"BL (x, y) to TR (x, y), margins L R B T". Empty when the parser
    /// captured no bounds (very old formats).</summary>
    private static string CameraBoundsText(MapInfo info)
    {
        var q = info.CameraBounds;
        if (q is null)
            return string.Empty;
        var text = string.Format(CultureInfo.InvariantCulture,
            "BL ({0:0.#}, {1:0.#}) to TR ({2:0.#}, {3:0.#})",
            q.BottomLeft.X, q.BottomLeft.Y, q.TopRight.X, q.TopRight.Y);
        if (info.CameraBoundsComplements is { } m)
            text += string.Format(CultureInfo.InvariantCulture,
                ", margins L{0} R{1} B{2} T{3}", m.Left, m.Right, m.Bottom, m.Top);
        return text;
    }

    private static string TilesetName(Tileset tileset) =>
        Enum.IsDefined(tileset) ? tileset.ToString() : ((char)tileset).ToString();

    private static string WeatherName(WeatherType weather) =>
        Enum.IsDefined(weather) ? weather.ToString() : FourCC((int)weather);

    private static string ColorText(Color c) => $"{c.R},{c.G},{c.B},{c.A}";

    private static string FourCC(int value) => new(new[]
    {
        (char)(value & 0xFF),
        (char)((value >> 8) & 0xFF),
        (char)((value >> 16) & 0xFF),
        (char)((value >> 24) & 0xFF),
    });

    private static Tileset ParseTileset(string field, string value)
    {
        var v = value.Trim();
        if (Enum.TryParse<Tileset>(v, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
            return parsed;
        if (v.Length == 1 && Enum.IsDefined((Tileset)v[0]))
            return (Tileset)v[0];
        throw new ArgumentException(
            $"{field} must be a tileset name (e.g. LordaeronSummer) or its letter (e.g. L).",
            nameof(value));
    }

    private static WeatherType ParseWeather(string value)
    {
        var v = value.Trim();
        if (v.Length == 0)
            return WeatherType.None;
        if (Enum.TryParse<WeatherType>(v, ignoreCase: true, out var parsed))
            return parsed;
        // Unknown 4-letter codes pass through so custom weather ids survive.
        if (v.Length == 4)
            return (WeatherType)(v[0] | (v[1] << 8) | (v[2] << 16) | (v[3] << 24));
        throw new ArgumentException(
            "GlobalWeather must be a weather name (e.g. AshenvaleLightRain), a 4-letter code (e.g. RAlr), or None.",
            nameof(value));
    }

    private static TEnum ParseEnum<TEnum>(string field, string value) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed))
            return parsed;
        throw new ArgumentException(
            $"{field} must be one of: {string.Join(", ", Enum.GetNames<TEnum>())}.", nameof(value));
    }

    private static Color ParseColor(string field, string value)
    {
        var parts = value.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length is 3 or 4)
        {
            var bytes = new byte[parts.Length];
            var ok = true;
            for (var i = 0; i < parts.Length; i++)
                ok &= byte.TryParse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out bytes[i]);
            if (ok)
                return Color.FromArgb(parts.Length == 4 ? bytes[3] : 255,
                    bytes[0], bytes[1], bytes[2]);
        }
        throw new ArgumentException(
            $"{field} must be R,G,B or R,G,B,A with components 0..255.", nameof(value));
    }

    private static float ParseFloat(string field, string value) =>
        float.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var f)
            ? f
            : throw new ArgumentException($"{field} must be a number (dot decimal).", nameof(value));

    private static int ParseInt(string field, string value) =>
        int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)
            ? i
            : throw new ArgumentException($"{field} must be a whole number.", nameof(value));

    private static bool ParseBool(string field, string value)
    {
        var v = value.Trim();
        if (bool.TryParse(v, out var parsed))
            return parsed;
        if (v == "1")
            return true;
        if (v == "0")
            return false;
        throw new ArgumentException($"{field} must be true or false.", nameof(value));
    }
}
