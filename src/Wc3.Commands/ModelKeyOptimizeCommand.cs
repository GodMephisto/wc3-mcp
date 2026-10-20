using System.Buffers.Binary;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>
/// Lossless animation-key reduction. For every track chunk (KGTR translation, KGRT rotation,
/// KGSC scaling, KATV material alpha, KMTF texture frame) it drops a key whose value is
/// bit-identical to the value before it, which samples to the same motion because the
/// interpolator already holds that constant across the span. It never changes a key value,
/// never reorders keys, and leaves every other chunk byte for byte. That makes it safe where
/// geometry decimation is not, and it targets the duplicated keys Blender and Wurst exports
/// leave on constant tracks.
/// </summary>
public static class ModelKeyOptimizeCommand
{
    private sealed record Result(int ChunksTouched, int KeysBefore, int KeysAfter, long BytesBefore, long BytesAfter);

    private static readonly Dictionary<string, int> ValueFloatsByTag = new(StringComparer.Ordinal)
    {
        ["KGTR"] = 3,
        ["KGRT"] = 4,
        ["KGSC"] = 3,
        ["KATV"] = 1,
        ["KMTF"] = 1,
    };

    public static ModelKeyOptimizeResult ExecutePath(string inputPath, string outputPath)
    {
        var bytes = File.ReadAllBytes(inputPath);
        if (!MdxDocument.HasMagic(bytes))
            return new(false, $"'{inputPath}' is not an MDX model", 0, 0, 0, 0, 0);

        var doc = MdxDocument.Parse(bytes);
        int chunksTouched = 0, keysBefore = 0, keysAfter = 0;
        var rebuilt = new List<MdxDocument.Chunk>(doc.Chunks.Count);

        foreach (var chunk in doc.Chunks)
        {
            if (!ValueFloatsByTag.TryGetValue(chunk.Tag, out int valueFloats))
            {
                rebuilt.Add(chunk);
                continue;
            }

            if (!TryRebuildTrack(chunk.Payload, valueFloats, out var payload, out int before, out int after))
            {
                rebuilt.Add(chunk);
                continue;
            }

            if (after == before)
            {
                rebuilt.Add(chunk);
                continue;
            }

            chunksTouched++;
            keysBefore += before;
            keysAfter += after;
            rebuilt.Add(new MdxDocument.Chunk(chunk.Tag, payload));
        }

        if (chunksTouched == 0)
        {
            // Nothing to do, but still emit a valid copy so callers get a file at the requested path.
            File.WriteAllBytes(outputPath, bytes);
            return new(true, "no duplicate animation keys found", 0, 0, 0, bytes.Length, bytes.Length);
        }

        var output = MdxDocument.Create(rebuilt).Write();
        File.WriteAllBytes(outputPath, output);
        return new(true, $"dropped {keysBefore - keysAfter} duplicate key(s) in {chunksTouched} track(s)",
            chunksTouched, keysBefore, keysAfter, bytes.Length, output.Length);
    }

    private static bool TryRebuildTrack(byte[] payload, int valueFloats, out byte[] rebuilt,
        out int keysBefore, out int keysAfter)
    {
        rebuilt = payload;
        keysBefore = keysAfter = 0;

        int keyBytes = 4 + valueFloats * 4;
        int trackHeader = 12;
        if (payload.Length < trackHeader || (payload.Length - trackHeader) % keyBytes != 0)
            return false;

        uint frames = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(8, 4));
        if (frames == 0 || payload.Length != trackHeader + (int)frames * keyBytes)
            return false;

        var keep = new bool[frames];
        int kept = 0;
        for (int i = 0; i < frames; i++)
        {
            int offset = trackHeader + i * keyBytes;
            bool duplicate = i > 0 && ValueEquals(payload, offset - keyBytes, offset, valueFloats);
            keep[i] = !duplicate;
            if (keep[i]) kept++;
        }

        keysBefore = (int)frames;
        keysAfter = kept;
        if (kept == frames)
            return false;

        // A track always keeps at least its first key; a lone key is already minimal.
        if (kept == 0)
            return false;

        var outBytes = new byte[trackHeader + kept * keyBytes];
        payload.AsSpan(0, trackHeader).CopyTo(outBytes.AsSpan(0));
        BinaryPrimitives.WriteUInt32LittleEndian(outBytes.AsSpan(8, 4), (uint)kept);
        int write = trackHeader;
        for (int i = 0; i < frames; i++)
        {
            if (!keep[i]) continue;
            payload.AsSpan(trackHeader + i * keyBytes, keyBytes).CopyTo(outBytes.AsSpan(write));
            write += keyBytes;
        }

        rebuilt = outBytes;
        return true;
    }

    private static bool ValueEquals(byte[] data, int a, int b, int floats)
    {
        for (int i = 0; i < floats; i++)
        {
            if (BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(a + 4 + i * 4, 4))
                != BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(b + 4 + i * 4, 4)))
                return false;
        }
        return true;
    }
}

public sealed record ModelKeyOptimizeResult(
    bool Ok, string Message, int ChunksTouched, int KeysBefore, int KeysAfter,
    long BytesBefore, long BytesAfter);
