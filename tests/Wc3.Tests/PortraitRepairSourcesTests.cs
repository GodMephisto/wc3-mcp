// tests/Wc3.Tests/PortraitRepairSourcesTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// The portrait repair has to see unit models wherever a map declares them.
///
/// Object data (<c>umdl</c>) is only one of three homes. A Reforged or repaired map carries the
/// skin profile <c>Units\UnitSkin.txt</c>, and a pre-Reforged SLK map carries a <c>file</c>
/// column in <c>Units\UnitUI.slk</c>. Reading only object data is the blindness that made the
/// repair report "0 at risk" on a real map whose black-portrait heroes live entirely in the SLK
/// and skin profile. These tests pin that all three homes are read, and that the scope stays the
/// camera-bearing models the black pane needs.
/// </summary>
public class PortraitRepairSourcesTests
{
    // ---- a minimal camera-bearing 800 era model, enough for the header reader to judge ----

    private static byte[] Chunk(string tag, byte[] body)
    {
        var size = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(size, (uint)body.Length);
        return Encoding.ASCII.GetBytes(tag).Concat(size).Concat(body).ToArray();
    }

    private static byte[] Model(int version, bool camera)
    {
        var versionBody = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(versionBody, (uint)version);
        var bytes = new List<byte>(Encoding.ASCII.GetBytes("MDLX"));
        bytes.AddRange(Chunk("VERS", versionBody));
        bytes.AddRange(Chunk("MODL", new byte[0x150]));
        if (camera)
            bytes.AddRange(Chunk("CAMS", new byte[4]));
        return bytes.ToArray();
    }

    private static byte[] Bytes(string value) => Encoding.Latin1.GetBytes(value.Replace("\n", "\r\n"));

    private static (int? Version, bool Camera) Header(byte[] data)
    {
        int? version = null;
        bool camera = false;
        int p = 4;
        while (p + 8 <= data.Length)
        {
            string tag = Encoding.ASCII.GetString(data, p, 4);
            int size = (int)BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(p + 4, 4));
            if (tag == "VERS") version = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(p + 8, 4));
            if (tag == "CAMS") camera = true;
            p += 8 + size;
        }
        return (version, camera);
    }

    private static MapDocument MapWithSkin(string modelPath, byte[] model)
    {
        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"Units\UnitSkin.txt"] = Bytes($"[h000]\nfile={modelPath}\n"),
            [modelPath] = model,
        }));
    }

    [Fact]
    public void A_protected_map_resolves_its_skin_profile_by_hash()
    {
        // A protected map strips the listfile, so Units\UnitSkin.txt opens unnamed and a GetFile
        // lookup finds nothing. The MPQ hash probe recovers the profile and its model.
        var doc = MapDocument.Load(SyntheticMap.BuildProtected(
            new Dictionary<string, byte[]> { ["war3map.j"] = Bytes("function main takes nothing returns nothing\nendfunction\n") },
            new Dictionary<string, byte[]>
            {
                [@"Units\UnitSkin.txt"] = Bytes(@"[h000]" + "\n" + @"file=war3mapImported\hero.mdx" + "\n"),
                [@"war3mapImported\hero.mdx"] = Model(800, camera: true),
            }));

        var r = PortraitRepairCommand.Execute(doc, apply: false);

        Assert.Equal(1, r.ModelsResolved);
        Assert.Equal(1, r.ModelsAtRisk);
    }

    [Fact]
    public void A_skin_profile_unit_model_with_a_camera_is_found()
    {
        // The blindness under test: this model is declared only in Units\UnitSkin.txt, never in
        // war3map.w3u, so the object-data-only scan returned 0.
        var doc = MapWithSkin(@"war3mapImported\hero.mdx", Model(800, camera: true));

        var r = PortraitRepairCommand.Execute(doc, apply: false, stampVersion: 1800);

        Assert.Equal(1, r.ModelsResolved);
        Assert.Equal(1, r.ModelsAtRisk);
        Assert.Contains(r.Models, m => m.Name.Equals(@"war3mapImported\hero.mdx",
            StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void A_legacy_slk_unit_model_is_found()
    {
        // The pre-Reforged home, a file column in Units\UnitUI.slk. A map that has not been data
        // repaired yet declares its unit models here.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"Units\UnitUI.slk"] = Bytes("""
                ID;PWXL;N;E
                B;X2;Y1
                C;X1;Y1;K"unitUIID"
                C;X2;Y1;K"file"
                C;X1;Y2;K"h000"
                C;X2;Y2;K"war3mapImported\slkhero.mdx"
                E
                """),
            [@"war3mapImported\slkhero.mdx"] = Model(800, camera: true),
        }));

        var r = PortraitRepairCommand.Execute(doc, apply: false);

        Assert.Equal(1, r.ModelsResolved);
        Assert.Equal(1, r.ModelsAtRisk);
    }

    [Fact]
    public void A_cameraless_model_is_left_alone_even_with_a_stamp_requested()
    {
        // The scope line. A cameraless model renders correctly through the engine fallback, so
        // restamping it is not a portrait fix and only risks the geometry layout question.
        var doc = MapWithSkin(@"war3mapImported\plain.mdx", Model(800, camera: false));

        var r = PortraitRepairCommand.Execute(doc, apply: false, stampVersion: 1800);

        Assert.Equal(1, r.ModelsResolved);
        Assert.Equal(0, r.ModelsAtRisk);
        Assert.Equal(0, r.ModelsRepaired);
    }

    [Fact]
    public void A_declared_mdl_resolves_to_the_archived_mdx_in_the_same_directory()
    {
        // The declared extension and the stored one differ, .mdl against .mdx, and the file lives
        // under war3mapImported. The stem fallback must keep that directory, or it hashes a root
        // "hero.mdx" that does not exist and the model is reported as missing.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"Units\UnitSkin.txt"] = Bytes("[h000]\nfile=war3mapImported\\hero.mdl\n"),
            [@"war3mapImported\hero.mdx"] = Model(800, camera: true),
        }));

        var r = PortraitRepairCommand.Execute(doc, apply: false, stampVersion: 1800);

        Assert.Equal(1, r.ModelsResolved);
        Assert.Equal(1, r.ModelsAtRisk);
    }

    [Fact]
    public void Apply_removes_the_camera_and_stamps_the_version_on_a_skin_model()
    {
        var doc = MapWithSkin(@"war3mapImported\hero.mdx", Model(800, camera: true));

        var r = PortraitRepairCommand.Execute(doc, apply: true, stampVersion: 1800);
        Assert.Equal(1, r.ModelsRepaired);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var file = Assert.IsType<MapFileEntry>(reloaded.GetFile(@"war3mapImported\hero.mdx"));
        var (version, camera) = Header(file.OverrideBytes ?? file.RawBytes);

        Assert.False(camera);
        Assert.Equal(1800, version);
    }
}
