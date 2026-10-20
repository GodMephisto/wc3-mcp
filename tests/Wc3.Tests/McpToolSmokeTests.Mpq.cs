// tests/Wc3.Tests/McpToolSmokeTests.Mpq.cs
// Smoke coverage for the mpq container tools. Serial, because mpq_strip_bookkeeping
// flips a process-wide environment variable for the length of one save.
using Wc3.Model;

namespace Wc3.Tests;

[Collection("Serial")]
public sealed class McpSmokeMpqTests : McpSmokeBase
{
    public McpSmokeMpqTests(SmokeFixture f) : base(f) { }

    [Fact]
    public void An_unnamed_entry_serializes_a_null_name()
    {
        // The tool descriptions promise a null name for an unnamed entry, and a client reading
        // name off an omitted key gets undefined. The MCP serializer omits nulls, so the record
        // carries an attribute that forces this one out.
        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };
        string fileList = System.Text.Json.JsonSerializer.Serialize(
            new Wc3.Commands.FileState(null, 10, false, false), options);
        string listFiles = System.Text.Json.JsonSerializer.Serialize(
            new Wc3.Commands.FileEntryInfo(null, 10, false, false), options);

        Assert.Contains("\"name\":null", fileList);
        Assert.Contains("\"name\":null", listFiles);
    }

    [Fact, Covers("mpq_verify")]
    public async Task Mpq_verify_reports_the_fixture_as_readable()
    {
        var r = await Call("mpq_verify", Args(("map", Fixture)));
        Assert.True(Prop(r, "totalEntries").GetInt32() > 0);
        Assert.Equal(0, Prop(r, "unreadableEntries").GetInt32());
    }

    [Fact, Covers("mpq_listfile")]
    public async Task Mpq_listfile_counts_named_probe_recovered_and_unnamed()
    {
        var r = await Call("mpq_listfile", Args(("map", Fixture)));
        Assert.Equal(
            Prop(r, "totalEntries").GetInt32(),
            Prop(r, "namedByListfile").GetInt32()
            + Prop(r, "recoveredByStandardProbe").GetInt32()
            + Prop(r, "withoutName").GetInt32());
    }

    [Fact, Covers("mpq_name_recover")]
    public async Task Mpq_name_recover_reports_a_dictionary_hit()
    {
        var map = Path.Combine(_f.Dir, $"protected-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(map, SyntheticMap.BuildProtected(
            new Dictionary<string, byte[]> { ["war3map.j"] = new byte[] { 1 } },
            new Dictionary<string, byte[]> { ["hidden.w3e"] = new byte[] { 2 } }));
        var dict = Path.Combine(_f.Dir, $"dict-{Guid.NewGuid():N}.txt");
        File.WriteAllText(dict, "hidden.w3e\n");
        try
        {
            var r = await Call("mpq_name_recover", Args(("map", map), ("dictionary", dict)));
            var names = Rows(Prop(r, "names"));
            Assert.Contains(names, n => Prop(n, "name").GetString() == "hidden.w3e");
        }
        finally { File.Delete(map); File.Delete(dict); }
    }

    [Fact, Covers("mpq_strip_bookkeeping")]
    public async Task Mpq_strip_bookkeeping_writes_a_copy_without_a_listfile()
    {
        var outPath = await Write("mpq_strip_bookkeeping", Args());
        // The input still has it, the copy does not.
        Assert.True(MapDocument.Load(Fixture).HasFileByName("(listfile)"));
        Assert.False(MapDocument.Load(outPath).HasFileByName("(listfile)"));
    }

    [Fact, Covers("mpq_bytes_diff")]
    public async Task Mpq_bytes_diff_reports_a_map_against_itself_identical()
    {
        var r = await Call("mpq_bytes_diff", Args(("map", Fixture), ("other", Fixture)));
        Assert.True(Prop(r, "identical").GetBoolean());
    }

    [Fact, Covers("mpq_bytes_diff")]
    public async Task Mpq_bytes_diff_reports_a_real_change()
    {
        var changed = await Write("place_unit", Args(
            ("type_rawcode", "hfoo"), ("owner_id", 0), ("x", 64f), ("y", -64f)));
        var r = await Call("mpq_bytes_diff", Args(("map", Fixture), ("other", changed)));
        Assert.False(Prop(r, "identical").GetBoolean());
    }

    [Fact, Covers("mpq_rebuild")]
    public async Task Mpq_rebuild_writes_a_healthy_copy()
    {
        var outPath = await Write("mpq_rebuild", Args(("grow_hash", false)));
        Assert.True(File.Exists(outPath));
        var r = await Call("mpq_rebuild", Args(
            ("map", Fixture), ("out_path", NewOut("mpq_rebuild2")), ("grow_hash", true)));
        Assert.True(Prop(r, "reopens").GetBoolean());
        Assert.True(Prop(r, "outputEntries").GetInt32() > 0);
        Assert.True(Prop(r, "outputHashSlots").GetUInt32() > 0);
    }

    [Fact, Covers("mpq_merge")]
    public async Task Mpq_merge_combines_a_map_with_itself()
    {
        var outPath = NewOut("mpq_merge");
        var before = Sha(Fixture);
        var r = await Call("mpq_merge", Args(
            ("map", Fixture), ("other", Fixture), ("out_path", outPath), ("prefer", "a")));
        Assert.Equal(before, Sha(Fixture));
        Assert.True(Prop(r, "reopens").GetBoolean());
        Assert.True(Prop(r, "mergedEntries").GetInt32() > 0);
    }

    [Fact, Covers("mpq_add", "mpq_remove")]
    public async Task Mpq_add_then_remove_round_trips_an_internal_file()
    {
        var src = NewOut("payload", ".bin");
        File.WriteAllBytes(src, new byte[] { 11, 22, 33, 44 });
        var outPath = await Write("mpq_add", Args(
            ("name", @"war3mapImported\SmokeAdd.bin"), ("from", src)));
        var files = await Call("file_list", Args(("map", outPath)));
        Assert.Contains(Rows(files), f => Prop(f, "name").GetString() == @"war3mapImported\SmokeAdd.bin");

        var removed = await Write("mpq_remove",
            Args(("name", @"war3mapImported\SmokeAdd.bin")), outPath);
        var removedFiles = await Call("file_list", Args(("map", removed)));
        Assert.DoesNotContain(Rows(removedFiles),
            f => Prop(f, "name").GetString() == @"war3mapImported\SmokeAdd.bin");
    }

    [Fact, Covers("mpq_remove_many")]
    public async Task Mpq_remove_many_drops_several_names_in_one_copy()
    {
        var a = NewOut("a", ".bin");
        File.WriteAllBytes(a, new byte[] { 1, 2, 3 });
        var b = NewOut("b", ".bin");
        File.WriteAllBytes(b, new byte[] { 4, 5, 6 });
        var addA = await Write("mpq_add", Args(("name", @"war3mapImported\SmokeA.bin"), ("from", a)));
        var addB = await Write("mpq_add", Args(("name", @"war3mapImported\SmokeB.bin"), ("from", b)), addA);
        var names = new[] { @"war3mapImported\SmokeA.bin", @"war3mapImported\SmokeB.bin" };
        var removed = await Write("mpq_remove_many", Args(("names", names)), addB);
        var files = await Call("file_list", Args(("map", removed)));
        Assert.DoesNotContain(Rows(files), f => Prop(f, "name").GetString() == names[0]);
        Assert.DoesNotContain(Rows(files), f => Prop(f, "name").GetString() == names[1]);
    }

    [Fact, Covers("mpq_extract_all")]
    public async Task Mpq_extract_all_writes_every_entry()
    {
        var outDir = Path.Combine(_f.Dir, $"extractall-{Guid.NewGuid():N}");
        var r = await Call("mpq_extract_all", Args(("map", Fixture), ("out_dir", outDir)));
        Assert.True(Prop(r, "totalEntries").GetInt32() > 0);
        Assert.NotEmpty(Prop(r, "files").EnumerateArray());
        Assert.True(Directory.Exists(outDir));
        Assert.NotEmpty(Directory.EnumerateFiles(outDir, "*", SearchOption.AllDirectories));
    }
}
