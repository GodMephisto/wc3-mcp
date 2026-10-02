// tests/Wc3.Tests/ModelPathTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Model paths that resolve to no file, the shape Anime WOS2 0.32d ships. The engine logged
/// 4,754 "model creation failed" lines in one 12 minute match, nearly all one destructable whose
/// model reads ".mdl .mdl", plus script effects whose paths drifted from the files imported.
/// </summary>
public class ModelPathTests
{
    // No game install, so nothing depends on the machine. Every path here is imported.
    private const string NoGame = @"Z:\no-game-here";

    private static byte[] Sample()
    {
        var skin = new DestructableObjectData(ObjectDataFormatVersion.v2);
        var dest = new SimpleObjectModification { OldId = "YTct".FromRawcode(), NewId = "B017".FromRawcode() };
        dest.Modifications.Add(new SimpleObjectDataModification
        { Id = "bfil".FromRawcode(), Type = ObjectDataType.String, Value = ".mdl .mdl" });
        skin.NewDestructables.Add(dest);

        // Three script paths. A doubled folder whose file exists, a name missing the wos_ prefix
        // the file carries, and a name nothing in the archive resembles.
        string script =
            "function F takes nothing returns nothing\r\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\war3mapImported\\\\wos_fire.mdl\", 0, 0)\r\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\war3mapImported\\\\wos_fire.mdl\", 1, 1)\r\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\tsfx-1.mdl\", 0, 0)\r\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\nothing_like_it.mdl\", 0, 0)\r\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\wos_ok.mdl\", 0, 0)\r\n"
            + "endfunction\r\n";

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3mapSkin.w3b"] = Serialize(w => w.Write(skin)),
            ["war3map.j"] = System.Text.Encoding.Latin1.GetBytes(script),
            [@"war3mapImported\wos_fire.mdx"] = ModelPathCommand.EmptyModel(),
            [@"war3mapImported\wos_tsfx-1.mdx"] = ModelPathCommand.EmptyModel(),
            [@"war3mapImported\wos_ok.mdx"] = ModelPathCommand.EmptyModel(),
        });
    }

    [Fact]
    public void Scan_reports_placeholders_and_drifted_paths_but_not_the_ones_that_load()
    {
        var scan = ModelPathCommand.Scan(MapDocument.Load(Sample()), NoGame);

        var paths = scan.Missing.Select(m => m.Path).ToList();
        Assert.Contains(".mdl .mdl", paths);
        Assert.Contains(@"war3mapImported\war3mapImported\wos_fire.mdl", paths);
        Assert.Contains(@"war3mapImported\tsfx-1.mdl", paths);
        Assert.Contains(@"war3mapImported\nothing_like_it.mdl", paths);
        // wos_ok.mdl is served by wos_ok.mdx, which is how the engine resolves it.
        Assert.DoesNotContain(@"war3mapImported\wos_ok.mdl", paths);
        Assert.Equal(4, scan.Missing.Count);

        var placeholder = scan.Missing.Single(m => m.Path == ".mdl .mdl");
        Assert.True(placeholder.Placeholder);
        Assert.Equal("war3mapSkin.w3b", placeholder.Source);
        Assert.Equal(2, scan.Missing.Single(m => m.Path.Contains("wos_fire")).Uses);
        Assert.Equal(@"war3mapImported\wos_fire.mdl",
            scan.Missing.Single(m => m.Path.Contains("wos_fire")).Suggestion);
        Assert.Equal(@"war3mapImported\wos_tsfx-1.mdl",
            scan.Missing.Single(m => m.Path.Contains("tsfx")).Suggestion);
        Assert.Null(scan.Missing.Single(m => m.Path.Contains("nothing_like_it")).Suggestion);
    }

    [Fact]
    public void A_literal_joined_by_plus_is_a_fragment_and_is_never_touched()
    {
        // Otaku Defense v2.6.2 builds paths as "...z4 (" + I2S(i) + ").mdx" and "...Emoji_0" + I2S(k)
        // + ".mdx". Read as paths, ".mdx" looked like a placeholder, and the repair would have
        // pointed the tail of every such path at the empty model.
        string script =
            "function F takes integer i returns nothing\r\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\Emoji_0\"+I2S(i)+\".mdx\", 0, 0)\r\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\z4 (\" + I2S(i) + \").mdx\", 0, 0)\r\n"
            + "  call AddSpecialEffect(\".mdx\", 0, 0)\r\n"
            + "endfunction\r\n";
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = System.Text.Encoding.Latin1.GetBytes(script),
        }));
        var scan = ModelPathCommand.Scan(doc, NoGame);
        var only = Assert.Single(scan.Missing);
        Assert.Equal((".mdx", 1), (only.Path, only.Uses));

        ModelPathCommand.Repair(doc, NoGame);
        Assert.True(doc.TryReadFileByName("war3map.j", out var j));
        string text = System.Text.Encoding.Latin1.GetString(j);
        Assert.Contains("+I2S(i)+\".mdx\"", text);
        Assert.Contains("+ I2S(i) + \").mdx\"", text);
        Assert.Contains("AddSpecialEffect(\"war3mapImported\\\\wc3ctl_empty.mdl\", 0, 0)", text);
    }

    [Fact]
    public void Repair_fixes_what_has_one_answer_and_the_rescan_agrees()
    {
        var doc = MapDocument.Load(Sample());
        var r = ModelPathCommand.Repair(doc, NoGame);

        Assert.Equal(new[] { @"war3mapImported\nothing_like_it.mdl" }, r.LeftAlone.Select(m => m.Path));

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var after = ModelPathCommand.Scan(rebuilt, NoGame);
        Assert.Equal(new[] { @"war3mapImported\nothing_like_it.mdl" }, after.Missing.Select(m => m.Path));

        Assert.True(rebuilt.HasFileByName(ModelPathCommand.EmptyModelPath));
        var b017 = ObjectKinds.MergedEntries(rebuilt, ObjectKinds.Info(ObjectKind.Destructable))
            .Single(e => e.Id == "B017".FromRawcode());
        Assert.Equal(@"war3mapImported\wc3ctl_empty.mdl", ObjectKinds.ModsToDict(b017.Mods)["bfil"]);

        Assert.True(rebuilt.TryReadFileByName("war3map.j", out var j));
        string text = System.Text.Encoding.Latin1.GetString(j);
        Assert.Equal(2, CountOf(text, "\"war3mapImported\\\\wos_fire.mdl\""));
        Assert.Contains("\"war3mapImported\\\\wos_tsfx-1.mdl\"", text);
        // The script's own line endings survive the rewrite.
        Assert.Equal(7, CountOf(text, "\r\n"));
    }

    [Fact]
    public void Keeping_placeholders_leaves_them_and_adds_no_file()
    {
        var doc = MapDocument.Load(Sample());
        var r = ModelPathCommand.Repair(doc, NoGame, emptyModel: false);

        Assert.Contains(r.LeftAlone, m => m.Path == ".mdl .mdl");
        Assert.False(MapDocument.Load(doc.SaveToBytes()).HasFileByName(ModelPathCommand.EmptyModelPath));
    }

    [Fact]
    public void The_empty_model_is_a_well_formed_mdx_1800()
    {
        var bytes = ModelPathCommand.EmptyModel();
        Assert.Equal((1800, false), AuditCommand.ReadMdxHeader(bytes));
        // Every chunk's declared size lands exactly on the end of the file.
        Assert.Equal(4 + (8 + 4) + (8 + 372) + (8 + 132), bytes.Length);
    }

    /// <summary>
    /// The bundled model is not a guess at what the engine accepts. It is the file Blizzard
    /// ships and loads itself, so this proves it without starting the game.
    /// </summary>
    [Fact]
    [Trait("Category", "GameData")]
    public void The_empty_model_is_byte_identical_to_the_one_the_game_ships()
    {
        Assert.True(GameData.GameData.TryOpen(@"C:\Warcraft III", out var ctx, out var diag), diag);
        Assert.True(ctx!.TryReadFile(@"war3.w3mod:_de.w3mod:doodads\cinematic\empty\empty.mdx", out var shipped),
            "the installed game no longer ships doodads\\cinematic\\empty\\empty.mdx");
        Assert.Equal(shipped, ModelPathCommand.EmptyModel());
    }

    [Theory]
    [InlineData(".mdl", true)]
    [InlineData(" .mdl", true)]
    [InlineData(".mdl .mdl", true)]
    [InlineData(@"war3mapImported\.mdx", true)]
    [InlineData(@"war3mapImported\wos_.mdl", false)]
    [InlineData(@"Units\Human\Footman\Footman.mdl", false)]
    public void Placeholder_means_no_file_name_at_all(string path, bool expected) =>
        Assert.Equal(expected, ModelPathCommand.IsPlaceholder(path));

    private static int CountOf(string text, string needle)
    {
        int n = 0, i = 0;
        while ((i = text.IndexOf(needle, i, StringComparison.Ordinal)) >= 0) { n++; i += needle.Length; }
        return n;
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
