// src/Wc3.Commands/ModelImport/GltfImporter.cs
using System.Buffers.Binary;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace Wc3.Commands;

/// <summary>
/// A parsed glTF scene, the primitives it draws, the animations it names and any diagnostics raised
/// while resolving their materials and textures. Each primitive becomes a <see cref="MeshData"/> and
/// carries its resolved <see cref="MeshMaterial"/> when the glTF named one. Each animation becomes a
/// <see cref="MeshAnimation"/> carrying only its name and time range, not its channels.
/// </summary>
internal sealed record GltfScene(
    IReadOnlyList<MeshData> Meshes,
    IReadOnlyList<MeshAnimation> Animations,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Parses a glTF 2.0 model into a list of <see cref="MeshData"/>, both the .gltf JSON form and the
/// .glb binary container. Every mesh primitive's POSITION, NORMAL and TEXCOORD_0 accessors and its
/// indices are read, honouring the accessor componentType (float, unsigned byte, unsigned short,
/// unsigned int, signed views), the normalized flag, and the bufferView byteOffset and byteStride.
/// A buffer URI is either a data URI holding base64 or a path relative to the .gltf, and a .glb
/// buffer with no URI is the binary chunk. Triangle lists are used directly, and a triangle strip or
/// fan is converted. glTF texture coordinates already have a top-left origin, so V is left unflipped.
///
/// Each primitive's material is resolved through its pbrMetallicRoughness.baseColorTexture (or the
/// legacy top level baseColorTexture) to a texture, then the texture's source image, then the image
/// bytes, from a bufferView for .glb or a data URI or sibling file for .gltf. The bytes are carried
/// whole in the mesh's <see cref="MeshTexture"/>, so the MDX writer converts them to .blp. An image
/// that cannot be read leaves its material with a null texture and raises a diagnostic, never fails
/// the import.
///
/// Every animation is read for its name and its overall time range, the union of its samplers' input
/// accessors, and reported as a <see cref="MeshAnimation"/>. The channels and their keyframes are not
/// read, so the one honest output is a named MDX sequence interval and not a false bone track.
/// </summary>
internal static class GltfImporter
{
    public static GltfScene ParseGltf(string json, string name, string? baseDir)
    {
        ArgumentNullException.ThrowIfNull(json);
        using var doc = JsonDocument.Parse(json);
        var buffers = ResolveBuffers(doc.RootElement, baseDir, name, glbBin: null);
        return Extract(doc.RootElement, buffers, name, baseDir);
    }

    public static GltfScene ParseGlb(byte[] bytes, string name, string? baseDir = null)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 12 || bytes[0] != 'g' || bytes[1] != 'l' || bytes[2] != 'T' || bytes[3] != 'F')
            throw new InvalidDataException("not a GLB file, no glTF magic");

        int p = 12;
        string? json = null;
        byte[]? bin = null;
        while (p + 8 <= bytes.Length)
        {
            int chunkLength = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(p, 4)));
            uint chunkType = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(p + 4, 4));
            p += 8;
            if (chunkLength < 0 || p + chunkLength > bytes.Length) break;
            var span = bytes.AsSpan(p, chunkLength);
            if (chunkType == 0x4E4F534A) json = Encoding.UTF8.GetString(span);       // "JSON"
            else if (chunkType == 0x004E4942) bin = span.ToArray();                  // "BIN\0"
            p += chunkLength;
        }
        if (json is null) throw new InvalidDataException("GLB has no JSON chunk");

        using var doc = JsonDocument.Parse(json);
        var buffers = ResolveBuffers(doc.RootElement, baseDir, name, bin);
        return Extract(doc.RootElement, buffers, name, baseDir);
    }

    private static List<byte[]> ResolveBuffers(JsonElement root, string? baseDir, string name, byte[]? glbBin)
    {
        var buffers = new List<byte[]>();
        if (!root.TryGetProperty("buffers", out var arr) || arr.ValueKind != JsonValueKind.Array)
            return buffers;

        foreach (var buffer in arr.EnumerateArray())
        {
            if (buffer.TryGetProperty("uri", out var uriEl) && uriEl.ValueKind == JsonValueKind.String)
                buffers.Add(LoadUri(uriEl.GetString()!, baseDir, name));
            else if (glbBin is not null)
                buffers.Add(glbBin);
            else
                throw new InvalidDataException($"glTF '{name}' has a buffer with no URI and no binary chunk");
        }
        return buffers;
    }

    private static byte[] LoadUri(string uri, string? baseDir, string name)
    {
        int comma = uri.IndexOf(',');
        if (uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma >= 0)
        {
            string meta = uri[..comma];
            string payload = uri[(comma + 1)..];
            return meta.Contains(";base64", StringComparison.OrdinalIgnoreCase)
                ? Convert.FromBase64String(payload)
                : Encoding.UTF8.GetBytes(Uri.UnescapeDataString(payload));
        }
        if (baseDir is null)
            throw new InvalidDataException($"glTF '{name}' references the external buffer '{uri}', which needs the file on disk");
        return File.ReadAllBytes(Path.Combine(baseDir, Uri.UnescapeDataString(uri).Replace('/', Path.DirectorySeparatorChar)));
    }

    private static GltfScene Extract(JsonElement root, List<byte[]> buffers, string name, string? baseDir)
    {
        if (!root.TryGetProperty("meshes", out var meshes) || meshes.ValueKind != JsonValueKind.Array || meshes.GetArrayLength() == 0)
            throw new ArgumentException($"glTF '{name}' has no meshes");

        var diagnostics = new List<string>();
        var images = new ImageResolver(root, buffers, name, baseDir, diagnostics);
        JsonElement materials = root.TryGetProperty("materials", out var matEl) && matEl.ValueKind == JsonValueKind.Array
            ? matEl
            : default;

        var result = new List<MeshData>();
        foreach (var mesh in meshes.EnumerateArray())
        {
            if (!mesh.TryGetProperty("primitives", out var primitives) || primitives.ValueKind != JsonValueKind.Array)
                continue;
            foreach (var primitive in primitives.EnumerateArray())
                result.Add(ExtractPrimitive(root, buffers, primitive, materials, images, name));
        }
        if (result.Count == 0)
            throw new ArgumentException($"glTF '{name}' has no primitives");

        var animations = ExtractAnimations(root, buffers);
        return new GltfScene(result, animations, diagnostics);
    }

    /// <summary>
    /// Reads every animation's name and its overall time range in seconds. The range is the union of
    /// the input accessors of all the animation's samplers, taken from the accessor's own min/max when
    /// the exporter wrote them and otherwise from the input floats themselves. An animation with no
    /// sampler that yields a range is skipped rather than given a made-up interval, so the sequence
    /// list only ever holds intervals the file actually supports.
    /// </summary>
    private static List<MeshAnimation> ExtractAnimations(JsonElement root, List<byte[]> buffers)
    {
        var result = new List<MeshAnimation>();
        if (!root.TryGetProperty("animations", out var animations) || animations.ValueKind != JsonValueKind.Array)
            return result;
        if (!root.TryGetProperty("accessors", out var accessors) || accessors.ValueKind != JsonValueKind.Array)
            return result;

        int positional = 0;
        foreach (var animation in animations.EnumerateArray())
        {
            string animName = animation.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String
                ? n.GetString()!.Trim()
                : string.Empty;
            if (animName.Length == 0) animName = $"Anim{positional}";
            positional++;

            float? min = null, max = null;
            if (animation.TryGetProperty("samplers", out var samplers) && samplers.ValueKind == JsonValueKind.Array)
            {
                foreach (var sampler in samplers.EnumerateArray())
                {
                    if (!sampler.TryGetProperty("input", out var inputEl) || inputEl.ValueKind != JsonValueKind.Number)
                        continue;
                    int inputIndex = inputEl.GetInt32();
                    if (inputIndex < 0 || inputIndex >= accessors.GetArrayLength()) continue;
                    var (lo, hi) = AccessorTimeRange(root, buffers, accessors[inputIndex], inputIndex);
                    if (lo is null || hi is null) continue;
                    min = min is null ? lo : MathF.Min(min.Value, lo.Value);
                    max = max is null ? hi : MathF.Max(max.Value, hi.Value);
                }
            }
            if (min is null || max is null) continue;
            result.Add(new MeshAnimation(animName, min.Value, max.Value));
        }
        return result;
    }

    /// <summary>
    /// One input accessor's time range. The glTF spec requires min and max on an animation input, so
    /// they are used when present. When an exporter omits them the SCALAR floats are read and their
    /// own extremes taken. A null pair means the accessor could not yield a range.
    /// </summary>
    private static (float? Min, float? Max) AccessorTimeRange(
        JsonElement root, List<byte[]> buffers, JsonElement accessor, int index)
    {
        if (TryFirstNumber(accessor, "min", out float lo) && TryFirstNumber(accessor, "max", out float hi))
            return (lo, hi);

        float[] times;
        try
        {
            times = ReadFloats(root, buffers, index, 1);
        }
        catch (Exception)
        {
            return (null, null);
        }
        if (times.Length == 0) return (null, null);

        float min = times[0], max = times[0];
        foreach (float t in times)
        {
            if (t < min) min = t;
            if (t > max) max = t;
        }
        return (min, max);
    }

    private static bool TryFirstNumber(JsonElement owner, string key, out float value)
    {
        value = 0f;
        if (!owner.TryGetProperty(key, out var arr) || arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() == 0)
            return false;
        if (arr[0].ValueKind != JsonValueKind.Number)
            return false;
        value = arr[0].GetSingle();
        return true;
    }

    private static MeshData ExtractPrimitive(
        JsonElement root, List<byte[]> buffers, JsonElement primitive, JsonElement materials,
        ImageResolver images, string name)
    {
        if (!primitive.TryGetProperty("attributes", out var attributes) || attributes.ValueKind != JsonValueKind.Object)
            throw new ArgumentException($"glTF '{name}' primitive has no attributes");

        if (!attributes.TryGetProperty("POSITION", out var positionAccessor))
            throw new ArgumentException($"glTF '{name}' primitive has no POSITION attribute");

        var positions = ToVectors(ReadFloats(root, buffers, positionAccessor.GetInt32(), 3), 3);
        int vertexCount = positions.Count;

        var normals = attributes.TryGetProperty("NORMAL", out var normalAccessor)
            ? ToVectors(ReadFloats(root, buffers, normalAccessor.GetInt32(), 3), 3)
            : new List<Vector3>();
        var uvs = attributes.TryGetProperty("TEXCOORD_0", out var uvAccessor)
            ? ToVectors2(ReadFloats(root, buffers, uvAccessor.GetInt32(), 2))
            : new List<Vector2>();

        bool hasNormals = normals.Count == vertexCount;
        bool hasUvs = uvs.Count == vertexCount;

        int[] indices;
        if (primitive.TryGetProperty("indices", out var indexAccessor))
            indices = ReadIndices(root, buffers, indexAccessor.GetInt32());
        else
        {
            indices = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++) indices[i] = i;
        }

        int mode = primitive.TryGetProperty("mode", out var modeEl) ? modeEl.GetInt32() : 4;
        indices = ToTriangles(indices, mode, name);

        var faces = new List<MeshCorner[]>(indices.Length / 3);
        for (int t = 0; t + 2 < indices.Length; t += 3)
        {
            int a = indices[t], b = indices[t + 1], c = indices[t + 2];
            faces.Add(new[]
            {
                new MeshCorner(a, hasNormals ? a : -1, hasUvs ? a : -1),
                new MeshCorner(b, hasNormals ? b : -1, hasUvs ? b : -1),
                new MeshCorner(c, hasNormals ? c : -1, hasUvs ? c : -1),
            });
        }

        var material = ResolveMaterial(primitive, materials, images);
        return new MeshData(positions, normals, uvs, faces, FlipV: false, material);
    }

    private static MeshMaterial? ResolveMaterial(JsonElement primitive, JsonElement materials, ImageResolver images)
    {
        if (materials.ValueKind != JsonValueKind.Array)
            return null;
        if (!primitive.TryGetProperty("material", out var matEl) || matEl.ValueKind != JsonValueKind.Number)
            return null;
        int matIndex = matEl.GetInt32();
        if (matIndex < 0 || matIndex >= materials.GetArrayLength())
            return null;

        int textureIndex = BaseColorTextureIndex(materials[matIndex]);
        if (textureIndex < 0)
            return new MeshMaterial(null);
        return new MeshMaterial(images.Resolve(textureIndex));
    }

    private static int BaseColorTextureIndex(JsonElement material)
    {
        if (material.TryGetProperty("pbrMetallicRoughness", out var pbr)
            && pbr.ValueKind == JsonValueKind.Object
            && TryTextureIndex(pbr, "baseColorTexture", out int metallic))
            return metallic;
        // Older exporters put baseColorTexture at the material top level.
        return TryTextureIndex(material, "baseColorTexture", out int legacy) ? legacy : -1;
    }

    private static bool TryTextureIndex(JsonElement owner, string key, out int index)
    {
        index = -1;
        if (owner.ValueKind != JsonValueKind.Object || !owner.TryGetProperty(key, out var tex) || tex.ValueKind != JsonValueKind.Object)
            return false;
        if (!tex.TryGetProperty("index", out var idx) || idx.ValueKind != JsonValueKind.Number)
            return false;
        index = idx.GetInt32();
        return index >= 0;
    }

    /// <summary>
    /// Resolves a glTF texture index to a decodable image, caching per image index so two materials
    /// that share an image share one <see cref="MeshTexture"/>. A failure is reported once and the
    /// caller keeps its material with a null texture.
    /// </summary>
    private sealed class ImageResolver
    {
        private readonly JsonElement _root;
        private readonly List<byte[]> _buffers;
        private readonly string _modelName;
        private readonly string? _baseDir;
        private readonly List<string> _diagnostics;
        private readonly Dictionary<int, MeshTexture?> _cache = new();
        private readonly HashSet<string> _usedNames = new(StringComparer.OrdinalIgnoreCase);

        public ImageResolver(JsonElement root, List<byte[]> buffers, string modelName, string? baseDir, List<string> diagnostics)
        {
            _root = root;
            _buffers = buffers;
            _modelName = modelName;
            _baseDir = baseDir;
            _diagnostics = diagnostics;
        }

        public MeshTexture? Resolve(int textureIndex)
        {
            if (!_root.TryGetProperty("textures", out var textures) || textures.ValueKind != JsonValueKind.Array
                || textureIndex < 0 || textureIndex >= textures.GetArrayLength())
            {
                Report($"texture {textureIndex} is missing, its material keeps no texture");
                return null;
            }

            int imageIndex = SourceImageIndex(textures[textureIndex]);
            if (imageIndex < 0)
            {
                Report($"texture {textureIndex} names no source image, its material keeps no texture");
                return null;
            }
            if (_cache.TryGetValue(imageIndex, out var cached)) return cached;
            var resolved = LoadImage(imageIndex);
            _cache[imageIndex] = resolved;
            return resolved;
        }

        private static int SourceImageIndex(JsonElement texture)
        {
            if (texture.TryGetProperty("source", out var s) && s.ValueKind == JsonValueKind.Number)
                return s.GetInt32();
            if (texture.TryGetProperty("extensions", out var ext) && ext.ValueKind == JsonValueKind.Object
                && ext.TryGetProperty("KHR_texture_basisu", out var basisu) && basisu.ValueKind == JsonValueKind.Object
                && basisu.TryGetProperty("source", out var bs) && bs.ValueKind == JsonValueKind.Number)
                return bs.GetInt32();
            return -1;
        }

        private MeshTexture? LoadImage(int imageIndex)
        {
            if (!_root.TryGetProperty("images", out var images) || images.ValueKind != JsonValueKind.Array
                || imageIndex < 0 || imageIndex >= images.GetArrayLength())
            {
                Report($"image {imageIndex} is missing, its material keeps no texture");
                return null;
            }

            var image = images[imageIndex];
            string? name = image.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String ? n.GetString() : null;
            try
            {
                if (image.TryGetProperty("bufferView", out var bv) && bv.ValueKind == JsonValueKind.Number)
                {
                    var bytes = ReadBufferView(bv.GetInt32());
                    string mime = image.TryGetProperty("mimeType", out var m) && m.ValueKind == JsonValueKind.String ? m.GetString()! : null!;
                    string ext = ImageExtension(mime, uri: null);
                    return new MeshTexture(UniqueName(name, imageIndex), bytes, ext);
                }
                if (image.TryGetProperty("uri", out var uriEl) && uriEl.ValueKind == JsonValueKind.String)
                {
                    string uri = uriEl.GetString()!;
                    var bytes = LoadImageUri(uri);
                    string ext = ImageExtension(mimeType: null, uri);
                    return new MeshTexture(UniqueName(name, imageIndex), bytes, ext);
                }
                Report($"image '{name ?? imageIndex.ToString()}' carries neither a bufferView nor a URI, its material keeps no texture");
                return null;
            }
            catch (Exception ex)
            {
                Report($"image '{name ?? imageIndex.ToString()}' could not be read ({ex.Message}), its material keeps no texture");
                return null;
            }
        }

        private byte[] ReadBufferView(int viewIndex)
        {
            var views = _root.GetProperty("bufferViews");
            if (viewIndex < 0 || viewIndex >= views.GetArrayLength())
                throw new InvalidDataException($"bufferView {viewIndex} is out of range");
            var view = views[viewIndex];
            int bufferIndex = view.GetProperty("buffer").GetInt32();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new InvalidDataException($"bufferView {viewIndex} names buffer {bufferIndex}, which is absent");
            int offset = view.TryGetProperty("byteOffset", out var o) ? o.GetInt32() : 0;
            int length = view.GetProperty("byteLength").GetInt32();
            var buffer = _buffers[bufferIndex];
            if (offset < 0 || length < 0 || offset + length > buffer.Length)
                throw new InvalidDataException($"bufferView {viewIndex} overruns its buffer");
            var bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, length);
            return bytes;
        }

        private byte[] LoadImageUri(string uri)
        {
            int comma = uri.IndexOf(',');
            if (uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma >= 0)
            {
                string payload = uri[(comma + 1)..];
                return uri[..comma].Contains(";base64", StringComparison.OrdinalIgnoreCase)
                    ? Convert.FromBase64String(payload)
                    : Encoding.UTF8.GetBytes(Uri.UnescapeDataString(payload));
            }
            if (_baseDir is null)
                throw new InvalidDataException($"references the external image '{uri}', which needs the file on disk");
            return File.ReadAllBytes(Path.Combine(_baseDir, Uri.UnescapeDataString(uri).Replace('/', Path.DirectorySeparatorChar)));
        }

        private string UniqueName(string? name, int imageIndex)
        {
            string baseName = string.IsNullOrWhiteSpace(name) ? $"image{imageIndex}" : Path.GetFileNameWithoutExtension(name);
            if (string.IsNullOrWhiteSpace(baseName)) baseName = $"image{imageIndex}";
            foreach (char c in Path.GetInvalidFileNameChars())
                baseName = baseName.Replace(c, '_');

            string candidate = baseName;
            int suffix = 1;
            while (!_usedNames.Add(candidate))
                candidate = baseName + "_" + suffix++;
            return candidate;
        }

        private void Report(string detail) => _diagnostics.Add($"glTF '{_modelName}' {detail}");
    }

    private static string ImageExtension(string? mimeType, string? uri)
    {
        if (!string.IsNullOrWhiteSpace(mimeType))
            return ExtFromSubtype(mimeType);
        if (string.IsNullOrWhiteSpace(uri))
            return ".png";
        if (uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            int end = uri.IndexOf(';');
            int comma = uri.IndexOf(',');
            if (end < 0 || (comma >= 0 && comma < end)) end = comma;
            if (end > 5)
            {
                int slash = uri.IndexOf('/', 5);
                if (slash > 5 && slash < end)
                    return ExtFromSubtype("x/" + uri[(slash + 1)..end]);
            }
            return ".png";
        }
        string ext = Path.GetExtension(Uri.UnescapeDataString(uri)).ToLowerInvariant();
        if (ext is ".jpeg") ext = ".jpg";
        return string.IsNullOrEmpty(ext) ? ".png" : ext;
    }

    private static string ExtFromSubtype(string mime)
    {
        int slash = mime.IndexOf('/');
        string sub = (slash >= 0 ? mime[(slash + 1)..] : mime).ToLowerInvariant();
        if (sub is "jpeg") sub = "jpg";
        return "." + sub;
    }

    private static int[] ToTriangles(int[] indices, int mode, string name)
    {
        switch (mode)
        {
            case 4: // TRIANGLES
                {
                    int usable = indices.Length - indices.Length % 3;
                    return usable == indices.Length ? indices : indices[..usable];
                }
            case 5: // TRIANGLE_STRIP
                {
                    var tri = new List<int>();
                    for (int i = 0; i + 2 < indices.Length; i++)
                    {
                        if ((i & 1) == 0) { tri.Add(indices[i]); tri.Add(indices[i + 1]); tri.Add(indices[i + 2]); }
                        else { tri.Add(indices[i + 1]); tri.Add(indices[i]); tri.Add(indices[i + 2]); }
                    }
                    return tri.ToArray();
                }
            case 6: // TRIANGLE_FAN
                {
                    var tri = new List<int>();
                    for (int i = 1; i + 1 < indices.Length; i++)
                    {
                        tri.Add(indices[0]); tri.Add(indices[i]); tri.Add(indices[i + 1]);
                    }
                    return tri.ToArray();
                }
            default:
                throw new NotSupportedException($"glTF '{name}' primitive mode {mode} is not drawable geometry (need triangles)");
        }
    }

    private readonly record struct Accessor(
        int ComponentType, int Count, bool Normalized, bool HasView, int BufferIndex, int ByteOffset, int Stride, int Comps);

    private static Accessor ReadAccessor(JsonElement root, int index, int comps)
    {
        var acc = root.GetProperty("accessors")[index];
        int componentType = acc.GetProperty("componentType").GetInt32();
        int count = acc.GetProperty("count").GetInt32();
        bool normalized = acc.TryGetProperty("normalized", out var nz) && nz.GetBoolean();
        if (!acc.TryGetProperty("bufferView", out var viewEl))
            return new Accessor(componentType, count, normalized, false, 0, 0, 0, comps);

        var view = root.GetProperty("bufferViews")[viewEl.GetInt32()];
        int bufferIndex = view.GetProperty("buffer").GetInt32();
        int viewOffset = view.TryGetProperty("byteOffset", out var vo) ? vo.GetInt32() : 0;
        int stride = view.TryGetProperty("byteStride", out var bs) ? bs.GetInt32() : 0;
        int accOffset = acc.TryGetProperty("byteOffset", out var ao) ? ao.GetInt32() : 0;
        int compSize = ComponentSize(componentType);
        if (stride <= 0) stride = compSize * comps;
        return new Accessor(componentType, count, normalized, true, bufferIndex, viewOffset + accOffset, stride, comps);
    }

    private static float[] ReadFloats(JsonElement root, List<byte[]> buffers, int index, int comps)
    {
        var acc = ReadAccessor(root, index, comps);
        var result = new float[acc.Count * comps];
        if (!acc.HasView) return result; // no bufferView means the accessor is zero-filled
        var buffer = buffers[acc.BufferIndex];
        int compSize = ComponentSize(acc.ComponentType);
        for (int i = 0; i < acc.Count; i++)
            for (int c = 0; c < comps; c++)
            {
                int off = acc.ByteOffset + i * acc.Stride + c * compSize;
                result[i * comps + c] = ReadComponent(buffer, off, acc.ComponentType, acc.Normalized);
            }
        return result;
    }

    private static int[] ReadIndices(JsonElement root, List<byte[]> buffers, int index)
    {
        var acc = ReadAccessor(root, index, 1);
        var result = new int[acc.Count];
        if (!acc.HasView) return result;
        var buffer = buffers[acc.BufferIndex];
        int compSize = ComponentSize(acc.ComponentType);
        for (int i = 0; i < acc.Count; i++)
        {
            int off = acc.ByteOffset + i * acc.Stride;
            result[i] = (int)ReadComponent(buffer, off, acc.ComponentType, false);
        }
        return result;
    }

    private static float ReadComponent(byte[] buffer, int offset, int componentType, bool normalized)
    {
        switch (componentType)
        {
            case 5126: return BinaryPrimitives.ReadSingleLittleEndian(buffer.AsSpan(offset, 4));
            case 5125: return BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(offset, 4));
            case 5123:
                {
                    ushort v = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset, 2));
                    return normalized ? v / 65535f : v;
                }
            case 5122:
                {
                    short v = BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(offset, 2));
                    return normalized ? MathF.Max(v / 32767f, -1f) : v;
                }
            case 5121:
                {
                    byte v = buffer[offset];
                    return normalized ? v / 255f : v;
                }
            case 5120:
                {
                    sbyte v = unchecked((sbyte)buffer[offset]);
                    return normalized ? MathF.Max(v / 127f, -1f) : v;
                }
            default:
                throw new InvalidDataException($"glTF has an unknown accessor componentType {componentType}");
        }
    }

    private static int ComponentSize(int componentType) => componentType switch
    {
        5120 or 5121 => 1,
        5122 or 5123 => 2,
        5125 or 5126 => 4,
        _ => throw new InvalidDataException($"glTF has an unknown accessor componentType {componentType}"),
    };

    private static List<Vector3> ToVectors(float[] values, int comps)
    {
        var list = new List<Vector3>(values.Length / comps);
        for (int i = 0; i + comps <= values.Length; i += comps)
            list.Add(new Vector3(values[i], values[i + 1], values[i + 2]));
        return list;
    }

    private static List<Vector2> ToVectors2(float[] values)
    {
        var list = new List<Vector2>(values.Length / 2);
        for (int i = 0; i + 2 <= values.Length; i += 2)
            list.Add(new Vector2(values[i], values[i + 1]));
        return list;
    }
}
