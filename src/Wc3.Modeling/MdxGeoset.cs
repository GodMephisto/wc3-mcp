// src/Wc3.Modeling/MdxGeoset.cs
using System.Buffers.Binary;
using System.Text;

namespace Wc3.Modeling;

/// <summary>
/// One geoset inside a GEOS chunk, as an ordered list of its tagged sub-chunks, so the per-vertex
/// arrays can be replaced after a decimation and every other part kept byte for byte. The existing
/// reader throws this structure away once it has the geometry. This keeps it, so a rewrite changes
/// only what the decimator actually reduced.
///
/// The sub-chunks in the order the game writes them. VRTX and NRMS carry the vertices and normals,
/// PVTX the triangle indices, GNDX a matrix-group index per vertex, UVAS a count followed by one
/// UVBS per UV set, and MATS the bone indices. The untagged section after MATS, the material id,
/// selection flags and per-sequence extents, is kept as a segment with an empty tag.
///
/// A segment's <see cref="Segment.Payload"/> is everything after its four character tag, the count
/// field included, so replacing a segment replaces the count with the array and the two can never
/// disagree.
/// </summary>
public sealed class MdxGeoset
{
    /// <summary>One sub-chunk. <see cref="Tag"/> is empty for the untagged section after MATS.
    /// <see cref="Payload"/> is the bytes after the tag, the count field included.</summary>
    public sealed record Segment(string Tag, byte[] Payload);

    private readonly List<Segment> _segments;
    private readonly uint _version;

    private MdxGeoset(List<Segment> segments, uint version)
    {
        _segments = segments;
        _version = version;
    }

    /// <summary>The sub-chunks in file order.</summary>
    public IReadOnlyList<Segment> Segments => _segments;

    /// <summary>
    /// Builds a geoset from an ordered segment list, for a model created from scratch rather than
    /// parsed. Each payload carries its own count field, exactly as <see cref="Parse"/> expects, so
    /// <see cref="Write"/> emits the same layout the game writes.
    /// </summary>
    public static MdxGeoset Create(IReadOnlyList<Segment> segments, uint version = 800)
    {
        ArgumentNullException.ThrowIfNull(segments);
        return new MdxGeoset(new List<Segment>(segments), version);
    }

    /// <summary>Parses one geoset, the bytes after its inclusiveSize field.</summary>
    public static MdxGeoset Parse(byte[] payload, uint version)
    {
        ArgumentNullException.ThrowIfNull(payload);
        var segments = new List<Segment>();
        int p = 0, end = payload.Length;
        while (p + 8 <= end)
        {
            string tag = Encoding.ASCII.GetString(payload, p, 4);
            int count = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(p + 4, 4)));
            int data = p + 8;
            int next = tag switch
            {
                "VRTX" or "NRMS" => data + count * 12,
                "PTYP" or "PCNT" or "MTGC" or "MATS" => data + count * 4,
                "PVTX" => data + count * 2,
                "GNDX" => data + count,
                "TANG" => data + count * 16,
                "SKIN" => data + count,
                _ => -1,
            };
            if (tag is "UVAS")
            {
                // count is the number of UV sets, each a following UVBS chunk.
                int q = data;
                for (int s = 0; s < count && q + 8 <= end; s++)
                {
                    if (Encoding.ASCII.GetString(payload, q, 4) != "UVBS") break;
                    int n = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(q + 4, 4)));
                    q += 8 + n * 8;
                }
                segments.Add(new Segment(tag, payload[(p + 4)..q]));
                p = q;
                continue;
            }
            if (tag is "MATS")
            {
                segments.Add(new Segment(tag, payload[(p + 4)..next]));
                // The untagged section, material id, selection flags and extents.
                int tailEnd = SkipTail(payload, next, end, version);
                segments.Add(new Segment("", payload[next..tailEnd]));
                p = tailEnd;
                continue;
            }
            if (next < 0 || next > end) break; // unknown or overrunning tag, stop rather than misparse
            segments.Add(new Segment(tag, payload[(p + 4)..next]));
            p = next;
        }

        return new MdxGeoset(segments, version);
    }

    private static int SkipTail(byte[] b, int p, int end, uint version)
    {
        int q = p + 12; // materialId, selectionGroup, selectionFlags
        if (version >= 900) q += 84; // u32 lod + char[80] name
        q += 28; // extent
        if (q + 4 <= end)
        {
            int seqExtents = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(q, 4)));
            if (seqExtents >= 0) q += 4 + seqExtents * 28;
        }
        return q <= end ? q : end;
    }

    /// <summary>Serializes the geoset back, the bytes that follow its inclusiveSize field.</summary>
    public byte[] Write()
    {
        long total = 0;
        foreach (var s in _segments)
            total += s.Tag.Length == 0 ? s.Payload.Length : 4 + s.Payload.Length;
        var outBytes = new byte[total];
        int p = 0;
        foreach (var s in _segments)
        {
            if (s.Tag.Length != 0)
            {
                Encoding.ASCII.GetBytes(s.Tag, outBytes.AsSpan(p, 4));
                p += 4;
            }
            s.Payload.CopyTo(outBytes, p);
            p += s.Payload.Length;
        }
        return outBytes;
    }

    /// <summary>Replaces a sub-chunk by tag, for a decimated array. The payload includes the count.</summary>
    public void Set(string tag, byte[] payload)
    {
        for (int i = 0; i < _segments.Count; i++)
            if (_segments[i].Tag == tag) { _segments[i] = _segments[i] with { Payload = payload }; return; }
        throw new InvalidOperationException($"geoset has no {tag} sub-chunk");
    }
}
