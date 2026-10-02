// src/Wc3.Commands/SoundCommand.cs
using System.Globalization;
using War3Net.Build.Audio;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Result of a mutating sound-catalog operation.</summary>
public sealed record SoundOpResult(bool Ok, string Message, int Count = -1);

/// <summary>One sound definition's headline fields, flattened for read-back and listing.</summary>
public sealed record SoundFields(
    string Name, string FilePath, string Channel, string Flags,
    int Volume, float Pitch, int Priority, float MinDistance, float MaxDistance);

/// <summary>
/// Edits the map's sound catalog (war3map.w3s → <see cref="MapSounds"/>). Sounds are keyed
/// by <see cref="Sound.Name"/> — the label triggers/UI reference. Mutations persist through
/// <see cref="MapDocument.AddOrReplaceModelFile"/>; the byte-faithful War3Net writer is
/// pinned by MapWriteTests.Sounds_writer_is_byte_faithful_on_real_map.
/// </summary>
public static class SoundCommand
{
    private const string FileName = "war3map.w3s";

    /// <summary>Field names accepted by <see cref="Set"/> / <see cref="ApplyField"/>.</summary>
    public static readonly string[] EditableFields =
    {
        "Name", "File", "Eax", "Volume", "Pitch", "PitchVariance", "FadeIn", "FadeOut",
        "Priority", "Channel", "Flags", "MinDistance", "MaxDistance", "DistanceCutoff",
        "ConeInside", "ConeOutside", "ConeOutsideVolume",
    };

    // A map with no war3map.w3s simply has no sounds - return an empty catalog rather than
    // throwing, so listing works everywhere (add creates the file on first use).
    public static IReadOnlyList<SoundFields> List(MapDocument doc) =>
        doc.GetFile(FileName)?.Model is MapSounds s
            ? s.Sounds.Select(ToFields).ToList()
            : Array.Empty<SoundFields>();

    /// <summary>Audio file extensions the World Editor imports as sounds.</summary>
    private static readonly string[] AudioExtensions =
        { ".mp3", ".wav", ".flac", ".ogg", ".aif", ".aiff", ".mid" };

    /// <summary>
    /// The map's imported audio FILES (raw assets in the archive, the World Editor's Import
    /// Manager), sorted by path. These are distinct from the sound DEFINITIONS <see cref="List"/>
    /// returns: a definition (war3map.w3s) names a file and gives it volume/channel/3D settings
    /// so triggers and units can play it. A map can be packed with audio yet define no sounds -
    /// so this is what makes the imported mp3/wav visible next to the (often empty) definitions.
    /// </summary>
    public static IReadOnlyList<string> ImportedAudioFiles(MapDocument doc) =>
        doc.Files
            .Select(f => f.FileName)
            .Where(n => n is not null
                && AudioExtensions.Any(e => n.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
            .Select(n => n!)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>A World-Editor-style sound label derived from a file path: the bare file name
    /// (no folders, no extension) prefixed with <c>gg_snd_</c>, non-identifier chars replaced
    /// with underscores. Callers ensure uniqueness (Add rejects a duplicate name).</summary>
    public static string SoundNameForFile(string filePath)
    {
        var stem = Path.GetFileNameWithoutExtension(filePath ?? "");
        var cleaned = new string((stem ?? "").Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
        if (cleaned.Length == 0) cleaned = "sound";
        return "gg_snd_" + cleaned;
    }

    public static SoundOpResult Add(MapDocument doc, string name, string? file)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new SoundOpResult(false, "Sound name must not be blank.");

        var sounds = GetOrCreateSounds(doc);
        if (sounds.Sounds.Any(s => NameEq(s.Name, name)))
            return new SoundOpResult(false, $"A sound named '{name}' already exists.");

        sounds.Sounds.Add(NewSound(name, file));
        return Persist(doc, sounds, $"Added sound '{name}'.");
    }

    public static SoundOpResult Set(MapDocument doc, string name, string field, string value)
    {
        var sounds = GetSounds(doc);
        var snd = sounds.Sounds.FirstOrDefault(s => NameEq(s.Name, name));
        if (snd is null)
            return new SoundOpResult(false, $"No sound named '{name}'.");

        // Renaming onto an existing key would collide on the label triggers reference.
        if (Eq(field, "Name") && !NameEq(snd.Name, value)
            && sounds.Sounds.Any(s => !ReferenceEquals(s, snd) && NameEq(s.Name, value)))
            return new SoundOpResult(false, $"A sound named '{value}' already exists.");

        if (!ApplyField(snd, field, value, out var error))
            return new SoundOpResult(false, error);

        return Persist(doc, sounds, $"Set {field} on sound '{name}'.");
    }

    public static SoundOpResult Remove(MapDocument doc, string name)
    {
        var sounds = GetSounds(doc);
        var snd = sounds.Sounds.FirstOrDefault(s => NameEq(s.Name, name));
        if (snd is null)
            return new SoundOpResult(false, $"No sound named '{name}'.");

        sounds.Sounds.Remove(snd);
        return Persist(doc, sounds, $"Removed sound '{name}'.");
    }

    /// <summary>Applies one field edit to a sound in place. Returns false with a message on error.</summary>
    public static bool ApplyField(Sound snd, string field, string value, out string error)
    {
        error = string.Empty;
        if (Eq(field, "Name"))
        {
            if (string.IsNullOrWhiteSpace(value)) { error = "Sound name must not be blank."; return false; }
            snd.Name = value;
            return true;
        }
        if (Eq(field, "File")) { snd.FilePath = value; return true; }
        if (Eq(field, "Eax")) { snd.EaxSetting = value; return true; }

        if (Eq(field, "Volume")) return SetInt(value, v => snd.Volume = v, out error);
        if (Eq(field, "Pitch")) return SetFloat(value, v => snd.Pitch = v, out error);
        if (Eq(field, "PitchVariance")) return SetFloat(value, v => snd.PitchVariance = v, out error);
        if (Eq(field, "FadeIn")) return SetInt(value, v => snd.FadeInRate = v, out error);
        if (Eq(field, "FadeOut")) return SetInt(value, v => snd.FadeOutRate = v, out error);
        if (Eq(field, "Priority")) return SetInt(value, v => snd.Priority = v, out error);
        if (Eq(field, "Channel")) return SetEnum<SoundChannel>(value, v => snd.Channel = v, out error);
        if (Eq(field, "Flags")) return SetEnum<SoundFlags>(value, v => snd.Flags = v, out error);
        if (Eq(field, "MinDistance")) return SetFloat(value, v => snd.MinDistance = v, out error);
        if (Eq(field, "MaxDistance")) return SetFloat(value, v => snd.MaxDistance = v, out error);
        if (Eq(field, "DistanceCutoff")) return SetFloat(value, v => snd.DistanceCutoff = v, out error);
        if (Eq(field, "ConeInside")) return SetFloat(value, v => snd.ConeAngleInside = v, out error);
        if (Eq(field, "ConeOutside")) return SetFloat(value, v => snd.ConeAngleOutside = v, out error);
        if (Eq(field, "ConeOutsideVolume")) return SetInt(value, v => snd.ConeOutsideVolume = v, out error);

        error = $"Unknown sound field '{field}'. Known fields: {string.Join(", ", EditableFields)}.";
        return false;
    }

    private static SoundOpResult Persist(MapDocument doc, MapSounds sounds, string message)
    {
        // MapSounds has a byte-faithful writer registered in MapDocument.SerializeEntry,
        // so the typed model is re-serialized on Save; no local serialization needed.
        doc.AddOrReplaceModelFile(FileName, sounds);
        return new SoundOpResult(true, message, sounds.Sounds.Count);
    }

    private static bool SetInt(string value, Action<int> set, out string error)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
        {
            error = $"'{value}' is not a valid integer.";
            return false;
        }
        set(i);
        error = string.Empty;
        return true;
    }

    private static bool SetFloat(string value, Action<float> set, out string error)
    {
        if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
        {
            error = $"'{value}' is not a valid number.";
            return false;
        }
        set(f);
        error = string.Empty;
        return true;
    }

    private static bool SetEnum<T>(string value, Action<T> set, out string error) where T : struct, Enum
    {
        // Accept either an integer code or a case-insensitive member name.
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var code))
        {
            set((T)(object)code);
            error = string.Empty;
            return true;
        }
        if (Enum.TryParse<T>(value, ignoreCase: true, out var parsed))
        {
            set(parsed);
            error = string.Empty;
            return true;
        }
        error = $"'{value}' is not a valid {typeof(T).Name} " +
                $"(use an integer or one of: {string.Join(", ", Enum.GetNames<T>())}).";
        return false;
    }

    /// <summary>
    /// Builds a serializable blank sound. War3Net's <c>Write(BinaryWriter, Sound, ...)</c>
    /// writes every string member, so all 11 must be non-null or the writer NREs on Save —
    /// leave the ancillary ones empty and let the caller tune them with <see cref="Set"/>.
    /// </summary>
    private static Sound NewSound(string name, string? file) => new()
    {
        Name = name,
        FilePath = file ?? string.Empty,
        EaxSetting = string.Empty,
        SoundName = string.Empty,
        Unk2 = string.Empty,
        Unk5 = string.Empty,
        Unk6 = string.Empty,
        Unk7 = string.Empty,
        FacialAnimationLabel = string.Empty,
        FacialAnimationGroupLabel = string.Empty,
        FacialAnimationSetFilepath = string.Empty,
    };

    private static MapSounds GetSounds(MapDocument doc) =>
        doc.GetFile(FileName)?.Model as MapSounds
        ?? throw new InvalidOperationException(
            $"{FileName} is missing or could not be parsed; sounds are not editable.");

    private static MapSounds GetOrCreateSounds(MapDocument doc)
    {
        if (doc.GetFile(FileName)?.Model is MapSounds existing) return existing;
        // No sound catalog yet: start an empty one at the latest known format version.
        var latest = (MapSoundsFormatVersion)Enum.GetValues<MapSoundsFormatVersion>().Cast<int>().Max();
        return new MapSounds(latest);
    }

    private static SoundFields ToFields(Sound s) => new(
        Name: s.Name ?? string.Empty,
        FilePath: s.FilePath ?? string.Empty,
        Channel: s.Channel.ToString(),
        Flags: s.Flags.ToString(),
        Volume: s.Volume, Pitch: s.Pitch, Priority: s.Priority,
        MinDistance: s.MinDistance, MaxDistance: s.MaxDistance);

    private static bool Eq(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    private static bool NameEq(string? a, string b) => string.Equals(a ?? string.Empty, b, StringComparison.OrdinalIgnoreCase);
}
