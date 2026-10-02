// tests/Wc3.Tests/PortraitVersionUpgradeTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// The version re-stamp and the geometry widening that goes with it.
///
/// The shipped repair removed the camera and left VERS at 800, and the player reported the
/// portrait pane still black. The map's author says the version has to be re-stamped too.
///
/// That raises a structural question this file pins. A version 900 or above geoset carries a
/// u32 lod plus a char[80] lod name that an 800 era geoset does not, and a material carries a
/// char[80] shader name, both read off this repository's own MdxParser. So stamping 1800
/// without inserting those leaves a file whose declared version and real layout disagree.
///
/// These tests do not decide which remedy the GAME wants, because only the game can. They pin
/// that each remedy does exactly and only what it says.
/// </summary>
public class PortraitVersionUpgradeTests
{
    // ---- a synthetic 800 era model, small enough to reason about by hand ----

    private static byte[] U32(int v)
    {
        var b = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(b, (uint)v);
        return b;
    }

    private static byte[] Chunk(string tag, byte[] body)
        => Encoding.ASCII.GetBytes(tag).Concat(U32(body.Length)).Concat(body).ToArray();

    /// <summary>One geoset with a single triangle, in the version 800 layout.</summary>
    private static byte[] Geoset()
    {
        var inner = new List<byte>();
        inner.AddRange(Encoding.ASCII.GetBytes("VRTX"));
        inner.AddRange(U32(3));
        inner.AddRange(new byte[3 * 12]);
        inner.AddRange(Encoding.ASCII.GetBytes("NRMS"));
        inner.AddRange(U32(3));
        inner.AddRange(new byte[3 * 12]);
        inner.AddRange(Encoding.ASCII.GetBytes("PTYP"));
        inner.AddRange(U32(1));
        inner.AddRange(new byte[4]);
        inner.AddRange(Encoding.ASCII.GetBytes("PCNT"));
        inner.AddRange(U32(1));
        inner.AddRange(new byte[4]);
        inner.AddRange(Encoding.ASCII.GetBytes("PVTX"));
        inner.AddRange(U32(3));
        inner.AddRange(new byte[3 * 2]);
        inner.AddRange(Encoding.ASCII.GetBytes("GNDX"));
        inner.AddRange(U32(3));
        inner.AddRange(new byte[3]);
        inner.AddRange(Encoding.ASCII.GetBytes("MTGC"));
        inner.AddRange(U32(1));
        inner.AddRange(new byte[4]);
        inner.AddRange(Encoding.ASCII.GetBytes("MATS"));
        inner.AddRange(U32(1));
        inner.AddRange(new byte[4]);
        inner.AddRange(new byte[12]);          // material id, selection group, selection flags
        inner.AddRange(new byte[28]);          // extent
        inner.AddRange(U32(0));                // no per sequence extents
        inner.AddRange(Encoding.ASCII.GetBytes("UVAS"));
        inner.AddRange(U32(1));
        inner.AddRange(Encoding.ASCII.GetBytes("UVBS"));
        inner.AddRange(U32(3));
        inner.AddRange(new byte[3 * 8]);

        // The record leads with its own INCLUSIVE size, so that count includes the count.
        return U32(inner.Count + 4).Concat(inner).ToArray();
    }

    /// <summary>One material with one layer, in the version 800 layout.</summary>
    private static byte[] Material()
    {
        var layer = U32(28).Concat(new byte[24]).ToArray();       // inclusive size then fields
        var lays = Encoding.ASCII.GetBytes("LAYS").Concat(U32(1)).Concat(layer).ToArray();
        var body = new byte[8].Concat(lays).ToArray();            // priority plane, flags
        return U32(body.Length + 4).Concat(body).ToArray();
    }

    private static byte[] Model800()
        => Encoding.ASCII.GetBytes("MDLX")
            .Concat(Chunk("VERS", U32(800)))
            .Concat(Chunk("MODL", new byte[0x150]))
            .Concat(Chunk("CAMS", new byte[120]))
            .Concat(Chunk("MTLS", Material()))
            .Concat(Chunk("GEOS", Geoset()))
            .ToArray();

    private static Dictionary<string, (int Body, int Size)> Walk(byte[] d)
    {
        var map = new Dictionary<string, (int, int)>();
        int off = 4;
        while (off + 8 <= d.Length)
        {
            string tag = Encoding.ASCII.GetString(d, off, 4);
            int size = (int)BinaryPrimitives.ReadUInt32LittleEndian(d.AsSpan(off + 4, 4));
            map[tag] = (off + 8, size);
            off += 8 + size;
        }
        Assert.Equal(d.Length, off);           // the chunk list must land exactly on the end
        return map;
    }

    // ---- the fixture itself has to be right, or every test below is theatre ----

    [Fact]
    public void The_synthetic_model_really_is_a_walkable_800_era_model()
    {
        var m = Model800();
        var tags = Walk(m);
        Assert.Equal(800, (int)BinaryPrimitives.ReadUInt32LittleEndian(
            m.AsSpan(tags["VERS"].Body, 4)));
        Assert.True(tags.ContainsKey("CAMS"));
        Assert.True(tags.ContainsKey("GEOS"));
    }

    // ---- the version stamp ----

    [Fact]
    public void Stamp_version_rewrites_VERS_and_changes_nothing_else()
    {
        var m = Model800();
        int before = m.Length;
        Assert.True(PortraitRepairCommand.StampVersion(m, 1800));

        Assert.Equal(before, m.Length);        // a stamp must never resize the file
        var tags = Walk(m);
        Assert.Equal(1800, (int)BinaryPrimitives.ReadUInt32LittleEndian(
            m.AsSpan(tags["VERS"].Body, 4)));
        Assert.Equal(Model800().Length - 0, m.Length);
    }

    [Fact]
    public void Stamp_version_refuses_a_model_with_no_VERS_chunk()
    {
        // Refusing matters more than succeeding. A silent false here would let the caller ship
        // a model it believes is stamped and is not.
        var m = Encoding.ASCII.GetBytes("MDLX").Concat(Chunk("MODL", new byte[0x150])).ToArray();
        Assert.False(PortraitRepairCommand.StampVersion(m, 1800));
    }

    // ---- the geometry widening ----

    [Fact]
    public void Upgrade_inserts_exactly_84_bytes_per_geoset_and_80_per_material()
    {
        var m = Model800();
        var up = PortraitRepairCommand.UpgradeToV900Layout(m);
        Assert.NotNull(up);

        // One geoset and one material, so the file grows by exactly 164 bytes and by nothing
        // else. An off-by-one in either insert shows up here rather than in game.
        Assert.Equal(m.Length + 84 + 80, up!.Length);

        var before = Walk(m);
        var after = Walk(up);
        Assert.Equal(before["GEOS"].Size + 84, after["GEOS"].Size);
        Assert.Equal(before["MTLS"].Size + 80, after["MTLS"].Size);
        Assert.Equal(before["MODL"].Size, after["MODL"].Size);   // untouched chunks stay put
    }

    [Fact]
    public void The_widened_geoset_declares_its_new_inclusive_size()
    {
        // The record's own leading size must move with it, or a reader walking the list lands
        // mid-record on the next one and everything after is garbage.
        var m = Model800();
        var up = PortraitRepairCommand.UpgradeToV900Layout(m)!;
        var before = Walk(m);
        var after = Walk(up);
        int sizeBefore = (int)BinaryPrimitives.ReadUInt32LittleEndian(
            m.AsSpan(before["GEOS"].Body, 4));
        int sizeAfter = (int)BinaryPrimitives.ReadUInt32LittleEndian(
            up.AsSpan(after["GEOS"].Body, 4));
        Assert.Equal(sizeBefore + 84, sizeAfter);
        Assert.Equal(after["GEOS"].Size, sizeAfter);             // one geoset fills the chunk
    }

    [Fact]
    public void The_inserted_lod_block_is_zeroed_and_sits_after_the_selection_flags()
    {
        var m = Model800();
        var up = PortraitRepairCommand.UpgradeToV900Layout(m)!;
        var after = Walk(up);

        // Walk the geoset's tagged sub-chunks to the same point the writer used, then read the
        // 84 bytes that should now be there. Computing the offset independently is the point,
        // since asserting on the writer's own arithmetic would prove nothing.
        int p = after["GEOS"].Body + 4;
        foreach (var (tag, stride) in new[]
                 {
                     ("VRTX", 12), ("NRMS", 12), ("PTYP", 4), ("PCNT", 4),
                     ("PVTX", 2), ("GNDX", 1), ("MTGC", 4), ("MATS", 4),
                 })
        {
            Assert.Equal(tag, Encoding.ASCII.GetString(up, p, 4));
            int count = (int)BinaryPrimitives.ReadUInt32LittleEndian(up.AsSpan(p + 4, 4));
            p += 8 + count * stride;
        }
        p += 12;
        Assert.All(up[p..(p + 84)], b => Assert.Equal(0, b));
    }

    [Fact]
    public void Upgrade_refuses_a_geoset_whose_sub_chunks_are_out_of_order()
    {
        // A model this routine cannot account for byte for byte must be left alone, because a
        // partially widened geoset is worse than one that was never touched.
        var m = Model800();
        var tags = Walk(m);
        int p = tags["GEOS"].Body + 4;
        Assert.Equal("VRTX", Encoding.ASCII.GetString(m, p, 4));
        Encoding.ASCII.GetBytes("XXXX").CopyTo(m, p);            // break the very first tag
        Assert.Null(PortraitRepairCommand.UpgradeToV900Layout(m));
    }

    [Fact]
    public void Upgrade_and_stamp_compose_into_a_model_that_still_walks_exactly()
    {
        // The end to end shape the repair actually ships, camera gone, geometry widened,
        // version stamped, and the chunk list still landing exactly on the end of the file.
        var m = Model800();
        var stripped = PortraitRepairCommand.RemoveChunk(m, "CAMS"u8);
        Assert.NotNull(stripped);
        var up = PortraitRepairCommand.UpgradeToV900Layout(stripped!);
        Assert.NotNull(up);
        Assert.True(PortraitRepairCommand.StampVersion(up!, 1800));

        var tags = Walk(up!);                                    // Walk asserts the exact landing
        Assert.False(tags.ContainsKey("CAMS"));
        Assert.Equal(1800, (int)BinaryPrimitives.ReadUInt32LittleEndian(
            up.AsSpan(tags["VERS"].Body, 4)));
    }
}
