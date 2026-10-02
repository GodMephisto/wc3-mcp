// src/Wc3.Commands/RenderModelCommand.cs
using System.Text;
using Wc3.GameData;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>
/// Renders an object's model to PNG bytes. Map-imported models resolve first (model
/// file from the map archive, textures from the map's imports); models that live in
/// the base game fall back to CASC when a WC3 install is available, textures included.
/// Missing/ReplaceableId (team-colour) textures always fall back to flat shading.
/// </summary>
public static class RenderModelCommand
{
    /// <summary>Object-data field that names each kind's model file.</summary>
    private static readonly IReadOnlyDictionary<ObjectKind, string> ModelFieldByKind =
        new Dictionary<ObjectKind, string>
        {
            [ObjectKind.Unit] = "umdl",
            [ObjectKind.Doodad] = "dfil",
            [ObjectKind.Destructable] = "bfil",
            [ObjectKind.Item] = "ifil",
        };

    /// <summary>
    /// A parsed model plus its map-resolved textures, ready to render repeatedly
    /// at different camera angles (interactive rotation) without re-parsing the
    /// model or re-decoding BLPs on every frame.
    /// </summary>
    public sealed record PreparedModel(Model3D Model, IReadOnlyDictionary<int, TextureImage> Textures)
    {
        public byte[] RenderPng(
            int width = 512,
            int height = 512,
            float yawDegrees = ModelRenderer.DefaultYawDegrees,
            float pitchDegrees = ModelRenderer.DefaultPitchDegrees,
            float zoom = ModelRenderer.DefaultZoom)
            => ModelRenderer.RenderPng(Model, Textures, width, height, yawDegrees, pitchDegrees, zoom);
    }

    /// <summary>Parses the model at an internal map path and resolves its textures
    /// (map-imported only — no base-game fallback without a game-data context).</summary>
    public static PreparedModel Prepare(MapDocument doc, string modelInternalPath)
        => Prepare(doc, modelInternalPath, ctx: null);

    /// <summary>
    /// Parses the model and resolves its textures: the map's own files first, then —
    /// when <paramref name="ctx"/> is available — the base game via CASC, so units whose
    /// model was never imported (a plain Footman) render too. Both lookups try the usual
    /// slash and .mdx/.mdl variants; texture paths missing on both sides shade flat.
    /// </summary>
    public static PreparedModel Prepare(MapDocument doc, string modelInternalPath, GameDataContext? ctx)
    {
        if (FindModelEntry(doc, modelInternalPath) is { } entry)
        {
            var model = ModelParser.Parse(entry.RawBytes, entry.FileName!);
            return new PreparedModel(model, ResolveTextures(doc, model, ctx));
        }

        if (ctx is not null && TryReadCascModel(ctx, modelInternalPath, out var bytes, out var name))
        {
            var model = ModelParser.Parse(bytes, name);
            return new PreparedModel(model, ResolveTextures(doc, model, ctx));
        }

        throw new InvalidDataException(
            $"model '{modelInternalPath}' is not in the map"
            + (ctx is null
                ? " and no game install is available for a base-game (CASC) lookup"
                : " or the base game data"));
    }

    /// <summary>Convenience for front-ends: <see cref="Prepare(MapDocument, string, GameDataContext?)"/>
    /// with the game-data context opened from <paramref name="gameDir"/> (null = auto-detect;
    /// unavailable install degrades to map-only resolution).</summary>
    public static PreparedModel PrepareWithFallback(MapDocument doc, string modelInternalPath, string? gameDir)
        => Prepare(doc, modelInternalPath, OpenContext(gameDir));

    /// <summary>The base-game model bytes for a game path, trying the same slash/.mdx/.mdl
    /// candidates as the in-map resolver.</summary>
    private static bool TryReadCascModel(
        GameDataContext ctx, string path, out byte[] bytes, out string resolvedName)
    {
        foreach (var candidate in PathCandidates(path))
        {
            if (ctx.TryReadFile(candidate, out bytes))
            {
                resolvedName = candidate;
                return true;
            }
        }
        bytes = Array.Empty<byte>();
        resolvedName = path;
        return false;
    }

    private static GameDataContext? OpenContext(string? gameDir) =>
        GameData.GameData.TryOpen(gameDir, out var ctx, out _) ? ctx : null;

    /// <summary>Renders the model at an internal map path (slash and .mdx/.mdl variants
    /// tried; map-imported only).</summary>
    public static byte[] Execute(
        MapDocument doc,
        string modelInternalPath,
        float yawDegrees = ModelRenderer.DefaultYawDegrees,
        float pitchDegrees = ModelRenderer.DefaultPitchDegrees)
        => Prepare(doc, modelInternalPath).RenderPng(yawDegrees: yawDegrees, pitchDegrees: pitchDegrees);

    /// <summary>Renders the model at an internal or base-game path, falling back to CASC
    /// (via <paramref name="gameDir"/>) when the map doesn't contain it.</summary>
    public static byte[] ExecutePath(
        MapDocument doc,
        string modelPath,
        string? gameDir,
        float yawDegrees = ModelRenderer.DefaultYawDegrees,
        float pitchDegrees = ModelRenderer.DefaultPitchDegrees)
        => PrepareWithFallback(doc, modelPath, gameDir)
            .RenderPng(yawDegrees: yawDegrees, pitchDegrees: pitchDegrees);

    /// <summary>Renders one object's model, resolving the kind's model-file field.
    /// Base-game models fall back to CASC when the install is available.</summary>
    public static byte[] Execute(
        MapDocument doc,
        ObjectKind kind,
        string rawcode,
        string? gameDir,
        float yawDegrees = ModelRenderer.DefaultYawDegrees,
        float pitchDegrees = ModelRenderer.DefaultPitchDegrees)
    {
        if (!ModelFieldByKind.TryGetValue(kind, out var fieldCode))
            throw new InvalidDataException($"{kind} objects have no model-file field to render");
        var ctx = OpenContext(gameDir);
        var merged = ObjectGetCommand.Execute(doc, kind, rawcode, ctx, Array.Empty<string>());
        return Prepare(doc, ResolveModelPath(merged, rawcode, new[] { fieldCode }, kind, ctx), ctx)
            .RenderPng(yawDegrees: yawDegrees, pitchDegrees: pitchDegrees);
    }

    /// <summary>Kind-agnostic: probes every kind for the rawcode, then any model-file field.
    /// Base-game models fall back to CASC when the install is available.</summary>
    public static byte[] Execute(
        MapDocument doc,
        string rawcode,
        string? gameDir,
        float yawDegrees = ModelRenderer.DefaultYawDegrees,
        float pitchDegrees = ModelRenderer.DefaultPitchDegrees)
    {
        var ctx = OpenContext(gameDir);
        var merged = ObjectGetCommand.Execute(doc, rawcode, ctx, Array.Empty<string>());
        return Prepare(doc, ResolveModelPath(merged, rawcode, ModelFieldByKind.Values, kind: null, ctx), ctx)
            .RenderPng(yawDegrees: yawDegrees, pitchDegrees: pitchDegrees);
    }

    /// <summary>
    /// The object's model path: its merged model-file field first (a map delta always
    /// wins), then — for untouched base objects, whose art fields Reforged moved out of
    /// the SLKs into skin-profile TXTs the stores don't resolve — the "file" key of the
    /// kind's skin profile via CASC.
    /// </summary>
    private static string ResolveModelPath(
        MergedObjectResult merged, string rawcode, IEnumerable<string> fieldCodes,
        ObjectKind? kind, GameDataContext? ctx)
    {
        if (!merged.Found)
            throw new InvalidDataException($"object '{rawcode}' not found in the map or game data");
        foreach (var code in fieldCodes)
        {
            var field = merged.Fields.FirstOrDefault(
                f => string.Equals(f.Code, code, StringComparison.OrdinalIgnoreCase));
            // Variation-heavy fields can list several paths comma-separated; render the first.
            if (!string.IsNullOrWhiteSpace(field?.Value))
                return field.Value.Split(',')[0].Trim();
        }
        if (ctx is not null && BaseSkinModelPath(ctx, kind, rawcode) is { } skinPath)
            return skinPath;
        throw new InvalidDataException(
            $"object '{rawcode}' has no model-file field ({string.Join("/", fieldCodes)})");
    }

    /// <summary>
    /// Front-end hook: the base game's model path for an object that has no model-file
    /// field in its merged fields (Reforged skin profiles), or null when the install/CASC
    /// or the entry is unavailable. Kind-aware; never throws.
    /// </summary>
    public static string? BaseModelPath(ObjectKind kind, string rawcode, string? gameDir)
    {
        var ctx = OpenContext(gameDir);
        return ctx is null ? null : BaseSkinModelPath(ctx, kind, rawcode);
    }

    /// <summary>Skin-profile TXT per kind (Reforged's home for base art fields).</summary>
    private static readonly IReadOnlyDictionary<ObjectKind, string> SkinProfileByKind =
        new Dictionary<ObjectKind, string>
        {
            [ObjectKind.Unit] = @"units\unitskin.txt",
            [ObjectKind.Item] = @"units\itemskin.txt",
            [ObjectKind.Destructable] = @"units\destructableskin.txt",
            [ObjectKind.Doodad] = @"doodads\doodadskins.txt",
        };

    /// <summary>The "file" value of the object's skin-profile section ([hfoo] →
    /// file=units\human\Footman\Footman); null kind probes every profile.</summary>
    private static string? BaseSkinModelPath(GameDataContext ctx, ObjectKind? kind, string rawcode)
    {
        var profiles = kind is { } k
            ? SkinProfileByKind.TryGetValue(k, out var one) ? new[] { one } : Array.Empty<string>()
            : SkinProfileByKind.Values.ToArray();
        foreach (var profile in profiles)
        {
            if (!ctx.TryReadFile(profile, out var bytes)) continue;
            var value = IniValue(Encoding.UTF8.GetString(bytes), rawcode, "file");
            if (!string.IsNullOrWhiteSpace(value))
                return value.Split(',')[0].Trim().Trim('"');
        }
        return null;
    }

    /// <summary>Minimal INI lookup: the first "key=value" under "[section]".</summary>
    private static string? IniValue(string text, string section, string key)
    {
        bool inSection = false;
        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();
            if (line.StartsWith('['))
            {
                inSection = line.Equals($"[{section}]", StringComparison.OrdinalIgnoreCase);
                continue;
            }
            if (!inSection || line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal))
                continue;
            int eq = line.IndexOf('=');
            if (eq > 0 && line[..eq].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                return line[(eq + 1)..].Trim();
        }
        return null;
    }

    /// <summary>
    /// Resolves a model reference to the map entry that holds it, trying both slash
    /// conventions and the .mdx/.mdl extension swap (maps routinely reference a model as
    /// ".mdl" while storing the binary under ".mdx", and vice versa). Returns null when
    /// no in-map file backs the path (e.g. a base-game/CASC model). Public so every
    /// front-end resolves models identically — do not reimplement this lookup.
    /// </summary>
    public static MapFileEntry? FindModelEntry(MapDocument doc, string path)
    {
        foreach (var candidate in PathCandidates(path))
            if (doc.GetFile(candidate) is { } entry && entry.RawBytes.Length > 0)
                return entry;
        return null;
    }

    /// <summary>Both slash conventions x {as-given, sibling .mdx/.mdl, extensionless + either,
    /// variation-0 + either}. The variation-0 forms come last so they can never shadow an
    /// exact hit: classic doodads with variations store one model file per variation and no
    /// bare file (dfil says ...\Ruins_Flower; CASC only has ruins_flower0.mdx..ruins_flower4.mdx),
    /// so once the plain forms miss we try the "&lt;name&gt;0" file — variation 0 always exists
    /// when variations do, and is what the World Editor previews.</summary>
    private static IEnumerable<string> PathCandidates(string path)
    {
        foreach (var p in new[] { path, path.Replace('/', '\\'), path.Replace('\\', '/') }.Distinct())
        {
            yield return p;
            var ext = Path.GetExtension(p);
            bool hasModelExt = ext.Equals(".mdx", StringComparison.OrdinalIgnoreCase)
                || ext.Equals(".mdl", StringComparison.OrdinalIgnoreCase);
            if (hasModelExt)
                yield return Path.ChangeExtension(
                    p, ext.Equals(".mdx", StringComparison.OrdinalIgnoreCase) ? ".mdl" : ".mdx");
            else
            {
                yield return p + ".mdx";
                yield return p + ".mdl";
            }
            var stem = hasModelExt ? p[..^ext.Length] : p;
            yield return stem + "0.mdx";
            yield return stem + "0.mdl";
        }
    }

    /// <summary>
    /// Decodes every resolvable model texture, keyed by texture index: the map's own
    /// files win (imports can override base textures), then the base game via CASC when
    /// a context is available. ReplaceableId entries (team colour etc.), unresolvable
    /// paths and undecodable formats are simply absent — the renderer shades those
    /// geosets flat.
    /// </summary>
    private static IReadOnlyDictionary<int, TextureImage> ResolveTextures(
        MapDocument doc, Model3D model, GameDataContext? ctx = null)
    {
        var textures = new Dictionary<int, TextureImage>();
        for (int i = 0; i < model.Textures.Count; i++)
        {
            var path = model.Textures[i];
            if (string.IsNullOrWhiteSpace(path)
                || path.StartsWith("ReplaceableId:", StringComparison.OrdinalIgnoreCase))
                continue;
            var entry = doc.GetFile(path)
                ?? doc.GetFile(path.Replace('/', '\\'))
                ?? doc.GetFile(path.Replace('\\', '/'));
            byte[]? bytes = entry is { RawBytes.Length: > 0 } ? entry.RawBytes : null;
            if (bytes is null && ctx is not null)
                foreach (var candidate in CascTextureCandidates(path))
                    if (ctx.TryReadFile(candidate, out var cascBytes)) { bytes = cascBytes; break; }
            if (bytes is null) continue;
            if (TryDecodeTexture(bytes) is { } decoded)
                textures[i] = decoded;
        }
        return textures;
    }

    /// <summary>Model texture references keep classic .blp/.tga names, but Reforged's
    /// CASC stores those files repacked as .dds at the same path — try both.</summary>
    private static IEnumerable<string> CascTextureCandidates(string path)
    {
        yield return path;
        var ext = Path.GetExtension(path);
        if (ext.Equals(".blp", StringComparison.OrdinalIgnoreCase)
            || ext.Equals(".tga", StringComparison.OrdinalIgnoreCase))
            yield return Path.ChangeExtension(path, ".dds");
    }

    /// <summary>Sniffs the container (DDS magic, else BLP); undecodable bytes are null —
    /// the caller shades those geosets flat.</summary>
    private static TextureImage? TryDecodeTexture(byte[] bytes)
    {
        try
        {
            return DdsDecoder.LooksLikeDds(bytes) ? DdsDecoder.Decode(bytes) : BlpDecoder.Decode(bytes);
        }
        catch
        {
            return null;
        }
    }
}
