// src/Wc3.Commands/ModelImportCommand.cs
using System.Text;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>
/// What a model import produced, with the counts the caller reports and the MDX bytes to write.
/// <see cref="Mdx"/> is the assembled model, so a caller can write it wherever it likes, and
/// <see cref="Bytes"/> is its length. <see cref="Geosets"/> is one per source primitive,
/// <see cref="Materials"/> one per primitive and <see cref="Textures"/> the distinct TEXS entries.
/// <see cref="TextureFiles"/> are the .blp file names written beside the MDX. <see cref="Sequences"/>
/// is the number of named MDX sequences written and <see cref="SequenceNames"/> their names in order.
/// </summary>
public sealed record ModelImportResult(
    string Path,
    int Geosets,
    int Vertices,
    int Faces,
    int Textures,
    int Materials,
    long Bytes,
    byte[] Mdx,
    IReadOnlyList<string> TextureFiles,
    IReadOnlyList<string> Diagnostics,
    int Sequences = 0,
    IReadOnlyList<string>? SequenceNames = null);

/// <summary>
/// Imports a foreign model into a Warcraft III MDX. The format is chosen by extension and then by
/// content, so a renamed or extensionless file is still read correctly, and every format runs
/// through the same MDX path.
///
/// Wavefront OBJ reads position, texture and normal declarations and the v/vt/vn face forms, and its
/// V axis is flipped back to WC3's top-down sense, so an <see cref="ObjExporter"/> export followed
/// by an import round-trips the geometry. glTF 2.0 (.gltf JSON with external or embedded base64
/// buffers, and the .glb binary container) reads every mesh primitive's POSITION, NORMAL and
/// TEXCOORD_0 accessors and its indices, and resolves each primitive's material to its
/// baseColorTexture image, which is written as .blp and referenced from a TEXS entry. STL reads the
/// binary form (a triangle count at offset 80) and the ASCII form, and carries no UVs. PLY reads
/// ASCII and binary little-endian vertex x/y/z, nx/ny/nz and s/t plus the face index list. FBX
/// reads its ASCII text form, the Geometry node's Vertices, PolygonVertexIndex, LayerElementNormal
/// and LayerElementUV and the Model nodes' transforms. The binary FBX form needs a dependency and is
/// refused with a clear message rather than guessed.
///
/// The result is one v800 GEOS geoset per primitive, one TEXS entry per distinct texture and one
/// MTLS material per primitive. A glTF with no materials, and a format with no texture of its own,
/// keeps the legacy single default texture. An n-gon face is split into a triangle fan. When a
/// format carries no normals a per-face normal is computed and used for that face's corners, so flat
/// shading survives the import, and when it carries no UVs the corners get a zero UV.
///
/// Every glTF animation becomes a named MDX SEQS entry whose integer frame interval is the
/// animation's seconds range scaled by <c>fps</c> (30 by default). Bone keyframe tracks are not
/// written, so the sequence names the interval without inventing a track that would not match the
/// source. The result reports the sequence count and names alongside the geometry counts.
/// </summary>
public static class ModelImportCommand
{
    /// <summary>
    /// Imports the model at <paramref name="modelPath"/> and writes the MDX to
    /// <paramref name="outPath"/>, plus any .blp textures it referenced beside it. The format is
    /// detected from the file, not the extension alone.
    /// <para>
    /// PRF-04. The token is read before the input is opened, again before the first write and
    /// before each texture after that, so a cancel stops the import at the next file rather than
    /// at the end of it. The parse and the MDX build run in memory and cannot be split, so the
    /// honest unit here is one model and one texture. A cancel after anything has been written
    /// removes the files this call wrote and then throws, so a cancelled import leaves no partial
    /// model and no orphaned texture beside it.
    /// </para>
    /// <para>
    /// PRF-04. <paramref name="progress"/> is called after each file is written, with how many are
    /// written and how many the call will write, so the tool in front of this command can report
    /// where the import is. Null reports nothing, which is what the CLI passes.
    /// </para>
    /// </summary>
    public static ModelImportResult ExecutePath(string modelPath, string outPath, string? texturePath = null, double fps = 30,
        CancellationToken cancellationToken = default, Action<int, int>? progress = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelPath);
        // PRF-04. Before the input is opened, so a cancel that arrived before the call writes
        // nothing at all and says so rather than reporting a count it does not have yet.
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("model import cancelled before the input was read");
        if (!File.Exists(modelPath)) throw new FileNotFoundException($"Model not found: {modelPath}", modelPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);

        var bytes = File.ReadAllBytes(modelPath);
        string? baseDir = Path.GetDirectoryName(Path.GetFullPath(modelPath));
        var result = Import(bytes, modelPath, baseDir, texturePath, fps, out var output, out var textures);

        var dir = Path.GetDirectoryName(Path.GetFullPath(outPath));
        if (dir is { Length: > 0 }) Directory.CreateDirectory(dir);
        string target = dir is { Length: > 0 } ? dir : ".";

        // PRF-04. The first file. The check sits after the build because the build is the one step
        // that cannot be stopped, and before the write because a cancel at this point should cost
        // nothing rather than a model that is then deleted again.
        if (cancellationToken.IsCancellationRequested)
            throw Cancelled(0, textures.Count + 1);

        var written = new List<string>(textures.Count + 1);
        try
        {
            File.WriteAllBytes(outPath, output);
            written.Add(outPath);
            progress?.Invoke(written.Count, textures.Count + 1);
            foreach (var texture in textures)
            {
                // PRF-04. The next file. A cancel between two textures stops here rather than
                // after the last one.
                if (cancellationToken.IsCancellationRequested)
                    throw Cancelled(written.Count, textures.Count + 1);

                string path = Path.Combine(target, texture.FileName);
                File.WriteAllBytes(path, texture.Bytes);
                written.Add(path);
                progress?.Invoke(written.Count, textures.Count + 1);
            }
        }
        catch (OperationCanceledException)
        {
            foreach (string path in written)
            {
                try { File.Delete(path); } catch (IOException) { } catch (UnauthorizedAccessException) { }
            }
            throw;
        }
        return result with { Path = outPath };
    }

    /// <summary>PRF-04. The cancel a caller can act on, naming how many of the files were written.</summary>
    private static OperationCanceledException Cancelled(int written, int total) =>
        new("model import cancelled after " + written + " of " + total + " files");

    /// <summary>
    /// Imports model text already in memory, kept for the OBJ text path. <paramref name="name"/> is
    /// used for reporting, format detection and the default texture path. Throws
    /// <see cref="ArgumentException"/> on a model with no drawable geometry or an out-of-range index.
    /// </summary>
    public static ModelImportResult Execute(string modelText, string name, out byte[] output,
        string? texturePath = null, double fps = 30)
    {
        ArgumentNullException.ThrowIfNull(modelText);
        return Import(Encoding.UTF8.GetBytes(modelText), name, baseDir: null, texturePath, fps, out output, out _);
    }

    /// <summary>Imports raw model bytes, for a caller that already read the file.</summary>
    public static ModelImportResult Execute(byte[] modelBytes, string name, out byte[] output,
        string? texturePath = null, string? baseDir = null, double fps = 30)
    {
        ArgumentNullException.ThrowIfNull(modelBytes);
        return Import(modelBytes, name, baseDir, texturePath, fps, out output, out _);
    }

    private static ModelImportResult Import(byte[] bytes, string name, string? baseDir, string? texturePath,
        double fps, out byte[] output, out IReadOnlyList<MdxMeshBuilder.BuiltTexture> textures)
    {
        var format = ModelFormatDetector.Detect(name, bytes);
        var diagnostics = new List<string>();

        IReadOnlyList<MeshData> meshes;
        IReadOnlyList<MeshAnimation> animations = Array.Empty<MeshAnimation>();
        switch (format)
        {
            case ModelFormat.Obj:
                meshes = new[] { ObjImporter.Parse(DecodeText(bytes), name) };
                break;
            case ModelFormat.Gltf:
                {
                    var scene = ParseGltf(DecodeText(bytes), name, baseDir, diagnostics);
                    meshes = scene.Meshes;
                    animations = scene.Animations;
                    break;
                }
            case ModelFormat.Glb:
                {
                    var scene = ParseGlb(bytes, name, baseDir, diagnostics);
                    meshes = scene.Meshes;
                    animations = scene.Animations;
                    break;
                }
            case ModelFormat.Stl:
                meshes = new[] { StlImporter.Parse(bytes) };
                break;
            case ModelFormat.Ply:
                meshes = new[] { PlyImporter.Parse(bytes) };
                break;
            case ModelFormat.Fbx:
                {
                    var scene = FbxImporter.Parse(DecodeText(bytes), name);
                    diagnostics.AddRange(scene.Diagnostics);
                    meshes = scene.Meshes;
                    break;
                }
            default:
                throw new NotSupportedException($"Unsupported model format for '{name}'");
        }

        var usable = meshes.Where(m => m.Positions.Count > 0 && m.Faces.Count > 0).ToList();
        if (usable.Count == 0)
            throw new ArgumentException($"'{name}' holds no drawable geometry");

        var sequences = BuildSequences(animations, fps);
        return MdxMeshBuilder.Build(usable, name, texturePath, sequences, diagnostics, out output, out textures);
    }

    /// <summary>
    /// Turns an animation's seconds range into an MDX frame interval at <paramref name="fps"/>. The
    /// start and end are the range scaled by the fps and rounded to the nearest frame, clamped at
    /// zero, with the end never before the start. A non-positive fps falls back to the WC3 default
    /// of 30, so a caller cannot ask for a nonsensical timeline.
    /// </summary>
    private static IReadOnlyList<MeshSequence> BuildSequences(IReadOnlyList<MeshAnimation> animations, double fps)
    {
        if (animations.Count == 0) return Array.Empty<MeshSequence>();
        if (!(fps > 0)) fps = 30;

        var sequences = new List<MeshSequence>(animations.Count);
        foreach (var animation in animations)
        {
            int start = ToFrame(animation.StartSeconds, fps);
            int end = ToFrame(animation.EndSeconds, fps);
            if (end < start) end = start;
            sequences.Add(new MeshSequence(animation.Name, start, end));
        }
        return sequences;
    }

    private static int ToFrame(float seconds, double fps)
    {
        double frame = Math.Round(seconds * fps, MidpointRounding.AwayFromZero);
        if (frame < 0) return 0;
        return frame > int.MaxValue ? int.MaxValue : (int)frame;
    }

    private static GltfScene ParseGltf(string json, string name, string? baseDir, List<string> diagnostics)
    {
        var scene = GltfImporter.ParseGltf(json, name, baseDir);
        diagnostics.AddRange(scene.Diagnostics);
        return scene;
    }

    private static GltfScene ParseGlb(byte[] bytes, string name, string? baseDir, List<string> diagnostics)
    {
        var scene = GltfImporter.ParseGlb(bytes, name, baseDir);
        diagnostics.AddRange(scene.Diagnostics);
        return scene;
    }

    private static string DecodeText(byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes);
        return text.Length > 0 && text[0] == '﻿' ? text[1..] : text; // strip BOM
    }
}
