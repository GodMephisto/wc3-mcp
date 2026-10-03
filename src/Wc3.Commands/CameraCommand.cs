// src/Wc3.Commands/CameraCommand.cs
using System.Globalization;
using System.Numerics;
using Wc3.Model;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;

namespace Wc3.Commands;

/// <summary>One camera's editable fields, flattened for read-back and listing.
/// <see cref="Camera.TargetPosition"/> is split into <see cref="TargetX"/>/<see cref="TargetY"/>.</summary>
public sealed record CameraFields(
    string Name,
    float TargetX, float TargetY,
    float ZOffset, float Rotation, float AngleOfAttack, float TargetDistance,
    float Roll, float FieldOfView, float FarClippingPlane, float NearClippingPlane,
    float LocalPitch, float LocalYaw, float LocalRoll);

/// <summary>Result of a mutating camera op. <see cref="Count"/> is the camera count after
/// the op (-1 when the op failed).</summary>
public sealed record CameraOpResult(bool Ok, string Message, int Count = -1);

/// <summary>
/// Read/write access to the map-cameras file (war3map.w3c). The write side mutates the
/// already-parsed War3Net <see cref="MapCameras"/> model and re-serializes the WHOLE file
/// via War3Net's writer (the inverse of the ReadMapCameras parser wired in Parsers), so
/// untouched cameras survive byte-for-byte. MapDocument.SerializeEntry has no MapCameras
/// case, so the bytes go back through <see cref="MapDocument.AddOrReplaceRawFile"/> (raw
/// payloads are written verbatim on Save) and the parsed model is restored afterwards so
/// in-memory readers keep seeing the mutation.
/// </summary>
public static class CameraCommand
{
    public const string FileName = "war3map.w3c";

    /// <summary>Field names <see cref="Set"/> accepts (matched case-insensitively).</summary>
    public static readonly IReadOnlyList<string> EditableFields = new[]
    {
        "Name", "TargetX", "TargetY", "ZOffset", "Rotation", "AngleOfAttack",
        "TargetDistance", "Roll", "FieldOfView", "FarClippingPlane",
        "NearClippingPlane", "LocalPitch", "LocalYaw", "LocalRoll",
    };

    /// <summary>Lists every camera in document order.</summary>
    // A map with no war3map.w3c simply has no cameras - return an empty list rather than
    // throwing, so listing works everywhere (add creates the file on first use).
    public static IReadOnlyList<CameraFields> List(MapDocument doc) =>
        doc.GetFile(FileName)?.Model is MapCameras c
            ? c.Cameras.Select(ToFields).ToList()
            : Array.Empty<CameraFields>();

    /// <summary>Adds a new camera at the given target position and writes the re-serialized
    /// w3c back into the in-memory document (persisted on the next Save). Rejects a blank
    /// name or a name that collides with an existing camera (case-insensitive).</summary>
    public static CameraOpResult Add(MapDocument doc, string name, float targetX, float targetY)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new CameraOpResult(false, "Camera name must not be blank.");

        var cameras = GetOrCreateCameras(doc);
        if (cameras.Cameras.Any(c => NameEq(c.Name, name)))
            return new CameraOpResult(false, $"A camera named '{name}' already exists.");

        cameras.Cameras.Add(new Camera
        {
            Name = name,
            TargetPosition = new Vector2(targetX, targetY),
        });
        return Persist(doc, cameras, $"Added camera '{name}'.");
    }

    /// <summary>Sets one editable field on the named camera and writes the re-serialized
    /// w3c back into the in-memory document. Rejects an unknown field, an unparseable
    /// value, a rename onto an existing camera, or a missing target camera.</summary>
    public static CameraOpResult Set(MapDocument doc, string name, string field, string value)
    {
        var cameras = GetOrCreateCameras(doc);
        var cam = cameras.Cameras.FirstOrDefault(c => NameEq(c.Name, name));
        if (cam is null)
            return new CameraOpResult(false, $"No camera named '{name}'.");

        // Guard a rename collision before mutating anything.
        if (Eq(field, "Name") && !NameEq(cam.Name, value)
            && cameras.Cameras.Any(c => !ReferenceEquals(c, cam) && NameEq(c.Name, value)))
            return new CameraOpResult(false, $"A camera named '{value}' already exists.");

        if (!ApplyField(cam, field, value, out var error))
            return new CameraOpResult(false, error);

        return Persist(doc, cameras, $"Updated camera '{name}'.");
    }

    /// <summary>Removes the named camera (case-insensitive) and writes the re-serialized
    /// w3c back into the in-memory document.</summary>
    public static CameraOpResult Remove(MapDocument doc, string name)
    {
        var cameras = GetOrCreateCameras(doc);
        var cam = cameras.Cameras.FirstOrDefault(c => NameEq(c.Name, name));
        if (cam is null)
            return new CameraOpResult(false, $"No camera named '{name}'.");

        cameras.Cameras.Remove(cam);
        return Persist(doc, cameras, $"Removed camera '{name}'.");
    }

    /// <summary>Pure field mapping onto a War3Net <see cref="Camera"/> (hermetically
    /// testable). Returns false with a message on an unknown field or unparseable value.</summary>
    public static bool ApplyField(Camera cam, string field, string value, out string error)
    {
        error = string.Empty;
        if (Eq(field, "Name"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                error = "Camera name must not be blank.";
                return false;
            }
            cam.Name = value;
            return true;
        }

        if (Eq(field, "TargetX")) return SetFloat(value, v => cam.TargetPosition = new Vector2(v, cam.TargetPosition.Y), out error);
        if (Eq(field, "TargetY")) return SetFloat(value, v => cam.TargetPosition = new Vector2(cam.TargetPosition.X, v), out error);
        if (Eq(field, "ZOffset")) return SetFloat(value, v => cam.ZOffset = v, out error);
        if (Eq(field, "Rotation")) return SetFloat(value, v => cam.Rotation = v, out error);
        if (Eq(field, "AngleOfAttack")) return SetFloat(value, v => cam.AngleOfAttack = v, out error);
        if (Eq(field, "TargetDistance")) return SetFloat(value, v => cam.TargetDistance = v, out error);
        if (Eq(field, "Roll")) return SetFloat(value, v => cam.Roll = v, out error);
        if (Eq(field, "FieldOfView")) return SetFloat(value, v => cam.FieldOfView = v, out error);
        if (Eq(field, "FarClippingPlane")) return SetFloat(value, v => cam.FarClippingPlane = v, out error);
        if (Eq(field, "NearClippingPlane")) return SetFloat(value, v => cam.NearClippingPlane = v, out error);
        if (Eq(field, "LocalPitch")) return SetFloat(value, v => cam.LocalPitch = v, out error);
        if (Eq(field, "LocalYaw")) return SetFloat(value, v => cam.LocalYaw = v, out error);
        if (Eq(field, "LocalRoll")) return SetFloat(value, v => cam.LocalRoll = v, out error);

        error = $"Unknown camera field '{field}'. Known fields: {string.Join(", ", EditableFields)}.";
        return false;
    }

    /// <summary>Serializes a MapCameras with War3Net's writer (the exact inverse of the
    /// ReadMapCameras parser wired in Parsers).</summary>
    public static byte[] Serialize(MapCameras cameras)
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            writer.Write(cameras);
        return ms.ToArray();
    }

    private static CameraOpResult Persist(MapDocument doc, MapCameras cameras, string message)
    {
        var entry = doc.AddOrReplaceRawFile(FileName, Serialize(cameras));
        // AddOrReplaceRawFile drops the parsed model (raw payload wins on Save); restore it
        // so in-memory readers keep seeing the mutation. The override bytes were serialized
        // from this very model, so the two stay consistent.
        entry.Model = cameras;
        return new CameraOpResult(true, message, cameras.Cameras.Count);
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

    /// <summary>
    /// The map's parsed cameras, creating an empty modern section if the map has none.
    /// </summary>
    /// <remarks>
    /// A map with no war3map.w3c has no cameras, which is not the same as having cameras that
    /// cannot be edited. Throwing here made <see cref="Add"/> refuse on every map that never
    /// defined one, so the Cameras panel could show a map's cameras but never give it a first
    /// one, and the reason surfaced as an exception rather than a message. That contradicted
    /// this file's own documented intent, "add creates the file on first use", and it made
    /// cameras behave unlike regions, where PlacementCommand.GetOrCreateRegions has always
    /// created the section on demand. The two now match.
    ///
    /// Set and Remove reach through here too and are still correct, an absent section yields
    /// zero cameras, so they report "no camera named X" instead of raising.
    /// </remarks>
    internal static MapCameras GetOrCreateCameras(MapDocument doc) =>
        doc.GetFile(FileName)?.Model as MapCameras
        ?? new MapCameras(MapCamerasFormatVersion.v0, useNewFormat: false);

    private static CameraFields ToFields(Camera c) => new(
        Name: c.Name ?? string.Empty,
        TargetX: c.TargetPosition.X, TargetY: c.TargetPosition.Y,
        ZOffset: c.ZOffset, Rotation: c.Rotation, AngleOfAttack: c.AngleOfAttack,
        TargetDistance: c.TargetDistance, Roll: c.Roll, FieldOfView: c.FieldOfView,
        FarClippingPlane: c.FarClippingPlane, NearClippingPlane: c.NearClippingPlane,
        LocalPitch: c.LocalPitch, LocalYaw: c.LocalYaw, LocalRoll: c.LocalRoll);

    private static bool Eq(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    private static bool NameEq(string? a, string b) => string.Equals(a ?? string.Empty, b, StringComparison.OrdinalIgnoreCase);
}
