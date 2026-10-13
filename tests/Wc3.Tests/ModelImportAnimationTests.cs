// tests/Wc3.Tests/ModelImportAnimationTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// A glTF animation becomes a named MDX SEQS sequence. The fixture is a hermetic triangle whose
/// single animation, "Walk", spans zero to one second, so at the default 30 fps the sequence must be
/// frames 0 to 30. The SEQS chunk is read back through the public <see cref="MdxDocument"/> reader
/// and the whole file through <see cref="ModelParser"/>, so a green test proves both the sequence
/// bytes and that the produced MDX parses. The tracks are out of scope, so the sequence name and
/// interval are the whole claim.
/// </summary>
public sealed class ModelImportAnimationTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-import-anim", Guid.NewGuid().ToString("N"));

    public ModelImportAnimationTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    [Fact]
    public void A_gltf_animation_becomes_a_named_sequence_with_the_expected_range()
    {
        var path = Write("walk.gltf", AnimatedTriangleGltf(includeTimeMinMax: true));
        var outPath = Path.Combine(_dir, "walk.mdx");

        var r = ModelImportCommand.ExecutePath(path, outPath); // default 30 fps

        Assert.Equal(1, r.Sequences);
        Assert.Equal(new[] { "Walk" }, r.SequenceNames);

        var mdx = File.ReadAllBytes(outPath);
        var sequence = Assert.Single(ReadSequences(mdx));
        Assert.Equal("Walk", sequence.Name);
        Assert.Equal(0, sequence.Start);
        Assert.Equal(30, sequence.End); // one second at 30 fps

        var model = ModelParser.Parse(mdx, "walk.mdx"); // the produced MDX loads
        Assert.Single(model.Geosets);
    }

    [Fact]
    public void The_fps_option_rescales_the_frame_interval()
    {
        var path = Write("walk15.gltf", AnimatedTriangleGltf(includeTimeMinMax: true));
        var outPath = Path.Combine(_dir, "walk15.mdx");

        var r = ModelImportCommand.ExecutePath(path, outPath, texturePath: null, fps: 15);

        Assert.Equal(1, r.Sequences);
        var sequence = Assert.Single(ReadSequences(File.ReadAllBytes(outPath)));
        Assert.Equal(15, sequence.End); // one second at 15 fps
    }

    [Fact]
    public void A_time_accessor_without_min_max_still_yields_the_range()
    {
        // The glTF spec requires min and max on an animation input, but an exporter may omit them,
        // so the input floats are read as the fallback. This fixture strips them.
        var path = Write("no-minmax.gltf", AnimatedTriangleGltf(includeTimeMinMax: false));
        var outPath = Path.Combine(_dir, "no-minmax.mdx");

        var r = ModelImportCommand.ExecutePath(path, outPath);

        Assert.Equal(1, r.Sequences);
        var sequence = Assert.Single(ReadSequences(File.ReadAllBytes(outPath)));
        Assert.Equal(30, sequence.End);
    }

    [Fact]
    public void A_gltf_without_animations_writes_no_sequence()
    {
        var path = Write("static.gltf", StaticTriangleGltf());
        var outPath = Path.Combine(_dir, "static.mdx");

        var r = ModelImportCommand.ExecutePath(path, outPath);

        Assert.Equal(0, r.Sequences);
        Assert.Empty(r.SequenceNames!);
        Assert.Empty(ReadSequences(File.ReadAllBytes(outPath)));
        Assert.Single(ModelParser.Parse(File.ReadAllBytes(outPath), "static.mdx").Geosets);
    }

    /// <summary>The SEQS entries of an MDX, read from the raw chunk so the sequence survives even
    /// though this import writes no bone nodes and the high level parser needs both to build a
    /// skeleton. One 132-byte record is char[80] name, u32 start, u32 end, then the rest.</summary>
    private static List<(string Name, int Start, int End)> ReadSequences(byte[] mdx)
    {
        var document = MdxDocument.Parse(mdx);
        var result = new List<(string, int, int)>();
        foreach (var chunk in document.Chunks)
        {
            if (chunk.Tag != "SEQS") continue;
            for (int o = 0; o + 132 <= chunk.Payload.Length; o += 132)
            {
                string name = Encoding.UTF8.GetString(chunk.Payload, o, 80).TrimEnd('\0');
                int start = (int)BinaryPrimitives.ReadUInt32LittleEndian(chunk.Payload.AsSpan(o + 80, 4));
                int end = (int)BinaryPrimitives.ReadUInt32LittleEndian(chunk.Payload.AsSpan(o + 84, 4));
                result.Add((name, start, end));
            }
        }
        return result;
    }

    private string Write(string name, string text)
    {
        var path = Path.Combine(_dir, name);
        File.WriteAllText(path, text);
        return path;
    }

    /// <summary>
    /// A triangle with POSITION, NORMAL and TEXCOORD_0 plus an animation named "Walk" whose single
    /// sampler input spans 0.0 to 1.0 seconds. <paramref name="includeTimeMinMax"/> controls whether
    /// the time accessor carries the spec-required min and max, so both code paths are covered.
    /// </summary>
    private static string AnimatedTriangleGltf(bool includeTimeMinMax)
    {
        var buffer = TriangleBufferWithTimes(new[] { 0f, 0.5f, 1f });
        var b64 = Convert.ToBase64String(buffer);
        string minMax = includeTimeMinMax ? ", \"min\": [0.0], \"max\": [1.0]" : string.Empty;
        return $$"""
        {
          "asset": { "version": "2.0" },
          "buffers": [ { "byteLength": {{buffer.Length}}, "uri": "data:application/octet-stream;base64,{{b64}}" } ],
          "bufferViews": [
            { "buffer": 0, "byteOffset": 0, "byteLength": 36, "byteStride": 12 },
            { "buffer": 0, "byteOffset": 36, "byteLength": 36 },
            { "buffer": 0, "byteOffset": 72, "byteLength": 24 },
            { "buffer": 0, "byteOffset": 96, "byteLength": 6 },
            { "buffer": 0, "byteOffset": 104, "byteLength": 12 }
          ],
          "accessors": [
            { "bufferView": 0, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 1, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 2, "componentType": 5126, "count": 3, "type": "VEC2" },
            { "bufferView": 3, "componentType": 5123, "count": 3, "type": "SCALAR" },
            { "bufferView": 4, "componentType": 5126, "count": 3, "type": "SCALAR"{{minMax}} }
          ],
          "meshes": [ { "primitives": [ {
            "attributes": { "POSITION": 0, "NORMAL": 1, "TEXCOORD_0": 2 },
            "indices": 3, "mode": 4
          } ] } ],
          "animations": [ {
            "name": "Walk",
            "samplers": [ { "input": 4, "output": 0, "interpolation": "LINEAR" } ],
            "channels": [ { "sampler": 0, "target": { "node": 0, "path": "translation" } } ]
          } ]
        }
        """;
    }

    private static string StaticTriangleGltf()
    {
        var buffer = TriangleBufferWithTimes(Array.Empty<float>());
        var b64 = Convert.ToBase64String(buffer);
        return $$"""
        {
          "asset": { "version": "2.0" },
          "buffers": [ { "byteLength": {{buffer.Length}}, "uri": "data:application/octet-stream;base64,{{b64}}" } ],
          "bufferViews": [
            { "buffer": 0, "byteOffset": 0, "byteLength": 36, "byteStride": 12 },
            { "buffer": 0, "byteOffset": 36, "byteLength": 36 },
            { "buffer": 0, "byteOffset": 72, "byteLength": 24 },
            { "buffer": 0, "byteOffset": 96, "byteLength": 6 }
          ],
          "accessors": [
            { "bufferView": 0, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 1, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 2, "componentType": 5126, "count": 3, "type": "VEC2" },
            { "bufferView": 3, "componentType": 5123, "count": 3, "type": "SCALAR" }
          ],
          "meshes": [ { "primitives": [ {
            "attributes": { "POSITION": 0, "NORMAL": 1, "TEXCOORD_0": 2 },
            "indices": 3, "mode": 4
          } ] } ]
        }
        """;
    }

    /// <summary>Positions, normals and UVs for one triangle, its indices, two padding bytes and the
    /// time samples packed to the offsets the fixture's bufferViews declare.</summary>
    private static byte[] TriangleBufferWithTimes(IReadOnlyList<float> times)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        foreach (var v in new[] { (0f, 0f, 0f), (1f, 0f, 0f), (0f, 1f, 0f) })
            { w.Write(v.Item1); w.Write(v.Item2); w.Write(v.Item3); }
        for (int i = 0; i < 3; i++) { w.Write(0f); w.Write(0f); w.Write(1f); }
        foreach (var uv in new[] { (0f, 0f), (1f, 0f), (0f, 1f) }) { w.Write(uv.Item1); w.Write(uv.Item2); }
        w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)2);
        w.Write((ushort)0); // two bytes of padding so the time block is four byte aligned
        foreach (float t in times) w.Write(t);
        w.Flush();
        return ms.ToArray();
    }
}
