// tests/Wc3.Tests/ToolSafetyRegressionTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using War3Net.Build.Info;

namespace Wc3.Tests;

/// <summary>
/// The guards added after a real map was damaged by two tool defects. script_to_lua silently
/// converted a Wurst-compiled map, whose package runtime does not survive the conversion, and the
/// map then failed in play with "Could not initialize package" as soon as a spell ran. And
/// repair portraits advertised a VERS re-stamp that the repository's own Reforged 3.0.0 report
/// records as crashing the map on load. Each test pins the refusal and the override.
/// </summary>
public class ToolSafetyRegressionTests
{
    [Fact]
    public void A_wurst_script_is_refused_by_to_lua_and_can_be_forced()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        var (file, source) = ScriptCommand.Read(doc);
        doc.AddOrReplaceRawFile(file,
            ScriptCommand.ScriptEncoding.GetBytes(source
            + "\nfunction WurstMarker takes nothing returns nothing\n"
            + "    local integer wurst_stack_depth = 0\nendfunction\n"));

        var ex = Assert.Throws<InvalidOperationException>(() => JassToLuaCommand.Convert(doc, "", ""));
        Assert.Contains("Wurst", ex.Message);

        // The override still converts, so a caller that has verified a Wurst map can proceed.
        var r = JassToLuaCommand.Convert(doc, "", "", allowWurst: true);
        Assert.True(r.Functions > 0);
    }

    [Fact]
    public void Version_stamping_a_portrait_model_is_refused_without_the_override()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        var ex = Assert.Throws<InvalidOperationException>(
            () => PortraitRepairCommand.Execute(doc, apply: false, stampVersion: 1800));
        Assert.Contains("crash", ex.Message, StringComparison.OrdinalIgnoreCase);

        var r = PortraitRepairCommand.Execute(doc, apply: false, stampVersion: 1800,
            upgradeGeometry: true, allowVersionStamp: true);
        Assert.NotNull(r);
    }

    [Fact]
    public void A_bj_effect_destroyed_or_stored_through_the_bj_global_is_not_a_leak()
    {
        static string Body(string lines) =>
            "function Tick takes nothing returns nothing\n" + lines + "endfunction\n"
            + "function main takes nothing returns nothing\n"
            + "    call TimerStart(CreateTimer(), 0.03, true, function Tick)\nendfunction\n";

        Assert.Empty(ScriptLeaksCommand.Analyze(Body(
            "    call AddSpecialEffectLocBJ(l, \"x.mdl\")\n"
            + "    call DestroyEffectBJ(GetLastCreatedEffectBJ())\n")).Leaks);
        Assert.Empty(ScriptLeaksCommand.Analyze(Body(
            "    call AddSpecialEffectTargetUnitBJ(\"origin\", u, \"x.mdl\")\n"
            + "    set e = bj_lastCreatedEffect\n")).Leaks);

        // A BJ effect with no destroy and no store before the next BJ creator is still a leak.
        var leak = Assert.Single(ScriptLeaksCommand.Analyze(Body(
            "    call AddSpecialEffectLocBJ(l, \"x.mdl\")\n")).Leaks);
        Assert.Equal(("discarded", "effect"), (leak.Rule, leak.Handle));
    }

    [Fact]
    public void Asset_unused_sees_a_texture_a_model_names_in_its_own_texture_chunk()
    {
        static byte[] Script() =>
            Encoding.ASCII.GetBytes("function main takes nothing returns nothing\nendfunction\n");

        var bytes = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Script(),
            [@"war3mapImported\Model.mdx"] =
                SyntheticModel.BuildMdx(texturePath: @"war3mapImported\Test.blp"),
            [@"war3mapImported\Test.blp"] = SyntheticModel.Blp(8, 8),
        });

        var unused = AssetUnusedCommand.Execute(MapDocument.Load(bytes)).Unused;

        // The texture is named only from the model's TEXS chunk, which no object field and no script
        // literal carries. It is used, so asset_unused must not offer it for deletion.
        Assert.DoesNotContain(unused, u =>
            u.Path.Equals(@"war3mapImported\Test.blp", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(unused, u =>
            u.Path.Equals(@"war3mapImported\Model.mdx", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Map_optimize_at_ratio_one_is_textures_only_and_warns_below_it()
    {
        var bytes = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.ASCII.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
            [@"war3mapImported\Grid.mdx"] = SyntheticModel.GridMdx(9),
            [@"war3mapImported\Tex.blp"] = SyntheticModel.Blp(64, 64),
        });

        var safe = MapOptimizeCommand.Execute(MapDocument.Load(bytes), 1.0f, 32);
        Assert.Equal(0, safe.ModelsOptimized);
        Assert.Equal(1, safe.TexturesOptimized);
        Assert.DoesNotContain(safe.Diagnostics, d => d.Contains("Decimation", StringComparison.OrdinalIgnoreCase));

        // The model is byte for byte the original in the saved map.
        var outDoc = MapDocument.Load(bytes);
        MapOptimizeCommand.Execute(outDoc, 1.0f, 32);
        var roundTripped = MapDocument.Load(outDoc.SaveToBytes());
        Assert.Equal(SyntheticModel.GridMdx(9),
            roundTripped.GetFile(@"war3mapImported\Grid.mdx")!.CurrentBytes);

        var decimated = MapOptimizeCommand.Execute(MapDocument.Load(bytes), 0.5f, 32);
        Assert.True(decimated.ModelsOptimized > 0);
        Assert.Contains(decimated.Diagnostics, d => d.Contains("Decimation", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void A_lua_map_carrying_the_wurst_runtime_is_flagged_by_script_check()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        doc.AddOrReplaceRawFile("war3map.lua", ScriptCommand.ScriptEncoding.GetBytes(
            "function main()\n  local wurst_stack_depth = 0\nend\n"));
        var info = (MapInfo)doc.GetFile("war3map.w3i")!.Model!;
        info.ScriptLanguage = ScriptLanguage.Lua;
        doc.AddOrReplaceRawFile("war3map.w3i", MapInfoCommand.Serialize(info)).Model = info;

        var r = ScriptCommand.Check(doc);
        Assert.Equal("lua", r.Language);
        Assert.True(r.Ok); // the script parses, so the runtime is the problem, not the syntax
        Assert.Contains(r.Issues, i =>
            i.Severity == DiagnosticSeverity.Warning && i.Message.Contains("Wurst", StringComparison.Ordinal));
    }

    [Fact]
    public void Asset_report_lists_every_entry_and_flags_every_fault_kind_it_knows()
    {
        var bytes = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.ASCII.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
            [@"war3mapImported\Used.mdx"] = SyntheticModel.GridMdx(4),
            [@"war3mapImported\Empty.blp"] = Array.Empty<byte>(),
        });

        var r = AssetReportCommand.Execute(MapDocument.Load(bytes), "test", includeModelLint: false);

        // Every imported entry is listed, and the empty one is flagged twice, empty and unreferenced.
        Assert.Equal(2, r.Entries);
        Assert.Contains(r.EntryList, e => e.Path.EndsWith("Used.mdx", StringComparison.Ordinal));
        Assert.Contains(r.EntryList, e => e.Path.EndsWith("Empty.blp", StringComparison.Ordinal) && e.Empty);
        Assert.Contains(r.Findings, f => f.Kind == "Empty" && f.Path.EndsWith("Empty.blp", StringComparison.Ordinal));
        Assert.Contains(r.Findings, f => f.Kind == "Unreferenced");
        Assert.True(r.Summary.ContainsKey("Unreferenced"));
    }

    [Fact]
    public void War3_log_reader_groups_the_engine_faults()
    {
        var log = Path.Combine(Path.GetTempPath(), "wc3log-" + Guid.NewGuid().ToString("N") + ".txt");
        File.WriteAllLines(log, new[]
        {
            "10/19 06:38:10.422  Solid texture substituted - Textures/Water/War3_Spray.tif",
            "10/19 06:38:10.422  Solid texture substituted - Textures/Water/War3_Spray.tif",
            "10/19 06:39:12.393  model creation failed - Broken.mdx",
            "10/19 06:38:28.611  Error, frame \"TW_Shop_Window\" already encountered!",
            "(tw_shop.fdf//3) Error, string /ReplayPanelStringTemplate already exists!",
            "10/19 06:38:09.000  an unrelated informational line",
        });

        var faults = War3LogFaults.Read(log);

        Assert.Equal(2, faults.Single(f => f.Kind == "SolidTextureSubstituted").Count);
        Assert.Contains(faults, f => f.Kind == "ModelCreationFailed" && f.Path == "Broken.mdx");
        Assert.Contains(faults, f => f.Kind == "DuplicateFrame" && f.Path == "TW_Shop_Window");
        Assert.Contains(faults, f => f.Kind == "DuplicateFrameString");
        Assert.DoesNotContain(faults, f => f.Path.Contains("unrelated", StringComparison.Ordinal));
        File.Delete(log);
    }

    [Fact]
    public void Wct_reconcile_trims_the_orphaned_tail_and_keeps_the_kept_slots()
    {
        static byte[] I32(int v) => new[] { (byte)v, (byte)(v >> 8), (byte)(v >> 16), (byte)(v >> 24) };
        static void Slot(List<byte> b, string code)
        {
            var payload = Encoding.ASCII.GetBytes(code).Concat(new byte[] { 0 }).ToArray();
            b.AddRange(I32(payload.Length));
            b.AddRange(payload);
        }

        var wct = new List<byte>();
        wct.AddRange(I32(0));   // version 0, no comment and no global block
        wct.AddRange(I32(3));   // three slots, one orphaned
        Slot(wct, "call A()");
        Slot(wct, "call B()");
        Slot(wct, "call C()");

        Assert.True(WctPairing.TryReconcileSlotCount(wct.ToArray(), 2, out var updated, out int old));
        Assert.Equal(3, old);
        Assert.Equal(2, BitConverter.ToInt32(updated, 4));
        var text = Encoding.ASCII.GetString(updated);
        Assert.Contains("call A()", text);
        Assert.Contains("call B()", text);
        Assert.DoesNotContain("call C()", text);

        // Already consistent, so nothing is changed.
        Assert.False(WctPairing.TryReconcileSlotCount(wct.ToArray(), 3, out _, out _));
    }
}
