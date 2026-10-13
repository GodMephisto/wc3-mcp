// tests/Wc3.Tests/AssetCommandTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Import;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the five <c>wc3ctl asset</c> commands. Every map is built in memory,
/// no WC3 install and no corpus needed. Object data is written through the same War3Net writers
/// the map format uses, so the field scan reads exactly what a real map stores.
/// </summary>
public class AssetCommandTests
{
    // ---- asset unused --------------------------------------------------------

    [Fact]
    public void Unused_reports_imports_no_field_or_script_names()
    {
        var doc = MapDocument.Load(ReferencingMap());

        var r = AssetUnusedCommand.Execute(doc);

        // "Used.mdx" is named by the unit's umdl field, "ScriptUsed.mdx" by a script literal.
        // "Orphan.blp" is named nowhere, so it is the only unused entry.
        var orphan = Assert.Single(r.Unused);
        Assert.Equal(@"war3mapImported\Orphan.blp", orphan.Path);
        Assert.Equal(7, orphan.SizeBytes);
        Assert.Equal(3, r.ImportedEntries);
    }

    [Fact]
    public void Unused_does_not_flag_an_entry_named_extensionless()
    {
        // The field stores the model without an extension, the archive stores it with .mdx.
        // The reference expansion must still count the entry as used.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "umdl".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\Hero" });
        w3u.NewUnits.Add(unit);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            [@"war3mapImported\Hero.mdx"] = new byte[] { 1, 2, 3 },
        }));

        var r = AssetUnusedCommand.Execute(doc);

        Assert.Empty(r.Unused);
    }

    [Fact]
    public void Unused_does_not_flag_an_entry_a_skin_override_names()
    {
        // war3mapSkin.txt carries asset paths the object data never repeats
        // (InfoPanelIconArmorSmall=Gem\Armor\Red.blp). Harvesting it keeps a live import from
        // reading as unused, which would be a false lead to delete a file the map loads.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3mapSkin.txt"] = Encoding.UTF8.GetBytes(
                "[CustomSkin]\nInfoPanelIconArmorSmall=war3mapImported\\SkinIcon.blp\n"),
            [@"war3mapImported\SkinIcon.blp"] = new byte[] { 1, 2, 3 },
        }));

        var r = AssetUnusedCommand.Execute(doc);

        Assert.Empty(r.Unused);
    }

    // ---- asset duplicates ----------------------------------------------------

    [Fact]
    public void Duplicates_groups_identical_bytes_and_reports_recoverable_space()
    {
        var payload = Encoding.ASCII.GetBytes("same bytes here");
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\A.blp"] = payload,
            [@"war3mapImported\B.blp"] = payload,
            [@"war3mapImported\Different.blp"] = new byte[] { 9, 9, 9 },
            [@"war3mapImported\Empty.blp"] = Array.Empty<byte>(),
        }));

        var r = AssetDuplicatesCommand.Execute(doc);

        var group = Assert.Single(r.Groups);
        Assert.Equal(payload.Length, group.SizeBytes);
        Assert.Equal(new[] { @"war3mapImported\A.blp", @"war3mapImported\B.blp" }, group.Paths);
        Assert.Equal(payload.Length, r.RecoverableBytes);
        Assert.Equal(4, r.ImportedEntries);
    }

    // ---- asset audit ---------------------------------------------------------

    [Fact]
    public void Audit_flags_oversized_and_empty_entries()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Big.bin"] = new byte[64],
            [@"war3mapImported\Empty.bin"] = Array.Empty<byte>(),
        }));

        var r = AssetAuditCommand.Execute(doc, maxBytes: 16);

        Assert.Contains(r.Findings, f => f.Kind == AssetAuditKind.Oversized
            && f.Path == @"war3mapImported\Big.bin");
        Assert.Contains(r.Findings, f => f.Kind == AssetAuditKind.Empty
            && f.Path == @"war3mapImported\Empty.bin");
        Assert.Equal(16, r.MaxBytes);
    }

    [Fact]
    public void Audit_flags_a_mdx_with_no_sibling_mdl()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Lone.mdx"] = Mdx(),
            [@"war3mapImported\Paired.mdx"] = Mdx(),
            [@"war3mapImported\Paired.mdl"] = MdLText(),
        }));

        var r = AssetAuditCommand.Execute(doc, maxBytes: 1_000_000);

        Assert.Contains(r.Findings, f => f.Kind == AssetAuditKind.ModelWithoutSource
            && f.Path == @"war3mapImported\Lone.mdx");
        Assert.DoesNotContain(r.Findings, f => f.Kind == AssetAuditKind.ModelWithoutSource
            && f.Path == @"war3mapImported\Paired.mdx");
    }

    [Fact]
    public void Audit_flags_non_power_of_two_blp_and_passes_a_power_of_two_one()
    {
        var odd = Blp(48, 48);
        var even = Blp(64, 64);
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Odd.blp"] = odd,
            [@"war3mapImported\Even.blp"] = even,
        }));

        var r = AssetAuditCommand.Execute(doc);

        Assert.Contains(r.Findings, f => f.Kind == AssetAuditKind.NonPowerOfTwoTexture
            && f.Path == @"war3mapImported\Odd.blp");
        Assert.DoesNotContain(r.Findings, f => f.Kind == AssetAuditKind.NonPowerOfTwoTexture
            && f.Path == @"war3mapImported\Even.blp");
    }

    // ---- asset size-report ---------------------------------------------------

    [Fact]
    public void Size_report_totals_non_structural_entries_per_folder_largest_first()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\a.blp"] = new byte[30],
            [@"war3mapImported\b.blp"] = new byte[20],
            [@"Textures\c.blp"] = new byte[100],
            ["war3map.w3i"] = new byte[500],   // structural, never counted
            ["rooty.blp"] = new byte[10],
        }));

        var r = AssetSizeReportCommand.Execute(doc);

        Assert.Equal(3, r.Folders.Count);
        Assert.Equal(@"Textures", r.Folders[0].Folder);
        Assert.Equal(100, r.Folders[0].Bytes);
        var imported = r.Folders.Single(f => f.Folder == @"war3mapImported");
        Assert.Equal(50, imported.Bytes);
        Assert.Equal(2, imported.Entries);
        Assert.Equal("(root)", r.Folders.Single(f => f.Folder == "(root)").Folder);
        Assert.Equal(160, r.TotalBytes);
        Assert.Equal(4, r.TotalEntries);
    }

    // ---- asset replace -------------------------------------------------------

    [Fact]
    public void Replace_rewrites_field_script_and_import_table_then_round_trips()
    {
        var doc = MapDocument.Load(ReferencingMap());

        var r = AssetReplaceCommand.Execute(doc, @"war3mapImported\Used.mdx", @"war3mapImported\Renamed.mdx");

        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.ObjectFieldsChanged);
        Assert.Equal(1, r.ScriptLiteralsChanged);
        Assert.Equal(1, r.ImportTableEntriesChanged);

        // Saved and reloaded, so the rewrite is proven by the bytes on the way back in.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Contains(@"war3mapImported\Renamed.mdx", AssetReferences.ObjectFieldValues(reloaded));
        Assert.DoesNotContain(@"war3mapImported\Used.mdx", AssetReferences.ObjectFieldValues(reloaded));
        Assert.Contains(@"war3mapImported\Renamed.mdx",
            AssetPathCandidates.NamedInScript(ScriptText.GetString(reloaded.GetFile("war3map.j")!.CurrentBytes)));

        var imports = (ImportedFiles)reloaded.GetFile("war3map.imp")!.Model!;
        Assert.Contains(imports.Files, f => f.FullPath == @"war3mapImported\Renamed.mdx");
        Assert.DoesNotContain(imports.Files, f => f.FullPath == @"war3mapImported\Used.mdx");
    }

    [Fact]
    public void Replace_refuses_a_path_that_is_named_nowhere()
    {
        var doc = MapDocument.Load(ReferencingMap());

        var r = AssetReplaceCommand.Execute(doc, @"war3mapImported\NobodyNamesThis.blp", @"war3mapImported\X.blp");

        Assert.False(r.Ok);
        Assert.Contains("not referenced", r.Message);
        Assert.Equal(0, r.ObjectFieldsChanged + r.ScriptLiteralsChanged + r.ImportTableEntriesChanged);
    }

    // ---- fixtures ------------------------------------------------------------

    /// <summary>A map whose object data, script and import table all name
    /// war3mapImported\Used.mdx, plus a script-only import and one orphan.</summary>
    private static byte[] ReferencingMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "TRIGSTR_1" });
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "umdl".FromRawcode(), Type = ObjectDataType.String, Value = @"war3mapImported\Used.mdx" });
        w3u.NewUnits.Add(unit);

        var imports = new ImportedFiles(ImportedFilesFormatVersion.v1);
        imports.Files.Add(new ImportedFile
        { Flags = (ImportedFileFlags)10, FullPath = @"war3mapImported\Used.mdx" });

        var script = Encoding.Latin1.GetBytes(
            "function main takes nothing returns nothing\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\Used.mdx\", 0, 0)\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\ScriptUsed.mdx\", 0, 0)\n"
            + "endfunction\n");

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.imp"] = Serialize(w => w.Write(imports)),
            ["war3map.j"] = script,
            [@"war3mapImported\Used.mdx"] = new byte[] { 1, 2, 3, 4, 5 },
            [@"war3mapImported\ScriptUsed.mdx"] = new byte[] { 6, 7, 8 },
            [@"war3mapImported\Orphan.blp"] = new byte[] { 1, 1, 1, 1, 1, 1, 1 },
            ["war3map.w3i"] = new byte[] { 1 }, // structural, excluded from the import count
        });
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    private static byte[] Mdx() => Encoding.ASCII.GetBytes("MDLX....");

    private static byte[] MdLText() => Encoding.ASCII.GetBytes("// mdl source");

    /// <summary>A real BLP1 texture of the requested size, encoded by the shared converter, so
    /// the audit decodes a texture exactly as a map would carry it.</summary>
    private static byte[] Blp(int width, int height)
    {
        var rgba = new byte[width * height * 4];
        for (int i = 0; i < rgba.Length; i += 4)
        {
            rgba[i] = 200; rgba[i + 1] = 100; rgba[i + 2] = 50; rgba[i + 3] = 255;
        }
        var png = TexturePng.Encode(new TextureImage(width, height, rgba));
        return TextureConvert.Convert(png, ".png", ".blp");
    }
}
