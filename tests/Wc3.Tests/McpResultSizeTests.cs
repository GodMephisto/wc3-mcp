// tests/Wc3.Tests/McpResultSizeTests.cs
// TST-09. Every family must answer a listing with a bounded result on a small map, because a
// listing that streams a whole map into a model's context costs more than the answer is worth.
// The map is synthesized in process, so the bound holds with no game install, no network and no
// fixture on disk beyond a temp file.
using System.Text;
using System.Text.Json;
using ModelContextProtocol.Protocol;
using Wc3.Commands;
using Wc3.Mcp;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>One list tool per family, and the two facts TST-09 asks of a paged one.</summary>
public sealed class McpResultSizeTests : IDisposable
{
    /// <summary>The TST-09 cap on one list result, 256 KB of UTF-8 JSON.</summary>
    private const int ResultCapBytes = 256 * 1024;

    private readonly string _dir = Path.Combine(Path.GetTempPath(), "wc3size-" + Guid.NewGuid().ToString("N"));

    public McpResultSizeTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    /// <summary>One list returning tool per family, the cheapest read that walks a collection.</summary>
    private static readonly (string Family, string Tool, Func<string, Dictionary<string, object?>> Build)[] ListTools =
    {
        ("map", "list_files", m => Args(("map", m))),
        ("objects", "object_list", m => Args(("map", m))),
        ("placed", "placed_units_list", m => Args(("map", m))),
        ("terrain", "terrain_audit", m => Args(("map", m))),
        ("scripts", "script_functions", m => Args(("map", m))),
        ("triggers", "triggers_read", m => Args(("map", m))),
        ("assets", "strings_list", m => Args(("map", m))),
        ("porting", "bundle_unit", m => Args(("map", m), ("rawcode", "hfoo"))),
        // The two families whose listing takes no map take the surface level read instead.
        ("repair", "target_list", _ => Args()),
        ("leaks", "leak_list", m => Args(("map", m))),
        ("product", "quickstart", _ => Args()),
    };

    [Fact]
    public async Task Every_family_has_a_bounded_result()
    {
        // The table is the inventory. A family with no entry, or an entry naming a tool outside its
        // family, fails here rather than passing over a family nobody measured.
        var families = McpToolFamilies.Families.Select(f => f.Id).ToHashSet(StringComparer.Ordinal);
        var covered = ListTools.Select(t => t.Family).ToHashSet(StringComparer.Ordinal);
        Assert.True(covered.SetEquals(families),
            "families with no list tool, " + string.Join(", ", families.Except(covered).OrderBy(f => f, StringComparer.Ordinal)));
        Assert.Equal(families.Count, ListTools.Length);

        string map = BlankMapPath();
        var measured = new List<string>();

        foreach (var (family, tool, build) in ListTools)
        {
            Assert.Equal(family, McpToolFamilies.Of(tool));

            string text = await Call(tool, build(map));
            int bytes = Encoding.UTF8.GetByteCount(text);
            Assert.True(bytes < ResultCapBytes,
                family + " " + tool + " answered " + bytes + " bytes, over the " + ResultCapBytes + " byte cap");

            using var doc = JsonDocument.Parse(text);
            Assert.True(HasList(doc.RootElement), family + " " + tool + " answered no list");

            measured.Add(family + " " + tool + " " + bytes + " bytes");
        }

        Assert.Equal(families.Count, measured.Count);

        // The paged half. list_files brief answers with one page and the four names every paged
        // result shares, returnedCount, totalCount, hasMore and nextOffset, so a caller learns how
        // big a list is without paying for every entry and cannot read the page size as the total.
        string paged = ArchivePath(60);
        var brief = Parse(await Call("list_files", Args(("map", paged), ("brief", true))));

        int totalCount = brief.GetProperty("totalCount").GetInt32();
        int returnedCount = brief.GetProperty("returnedCount").GetInt32();
        bool hasMore = brief.GetProperty("hasMore").GetBoolean();
        int nextOffset = brief.GetProperty("nextOffset").GetInt32();

        Assert.True(totalCount > BriefListing.PageSize,
            "the paged map holds " + totalCount + " entries, which is not more than one page");
        Assert.Equal(BriefListing.PageSize, returnedCount);
        Assert.Equal(BriefListing.PageSize, brief.GetProperty("files").GetArrayLength());
        Assert.True(returnedCount < totalCount, "the page is not smaller than the total");
        Assert.True(hasMore, "a cut page must say so");
        Assert.Equal(returnedCount, nextOffset);

        // RES-10, the alias this release keeps. list_files served total and truncated before the
        // page names, so both stay for one minor release and carry the same two facts.
        Assert.Equal(totalCount, brief.GetProperty("total").GetInt32());
        Assert.True(brief.GetProperty("truncated").GetBoolean());
    }

    /// <summary>A blank 32 tile map, synthesized in process and written once for the whole test.</summary>
    private string BlankMapPath()
    {
        var path = Path.Combine(_dir, "blank.w3x");
        File.WriteAllBytes(path, BlankMap.CreateArchiveBytes());
        return path;
    }

    /// <summary>An archive holding more entries than one page, so the brief listing has to cut.</summary>
    private string ArchivePath(int entries)
    {
        var files = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        for (int i = 0; i < entries; i++)
            files[@"war3mapImported\page" + i.ToString("00") + ".bin"] = new[] { (byte)i };

        var path = Path.Combine(_dir, "many.w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(files));
        return path;
    }

    /// <summary>Calls a read tool over the in-memory server and returns its JSON text.</summary>
    private static async Task<string> Call(string tool, Dictionary<string, object?> args)
    {
        string text = "";
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var r = await client.CallToolAsync(tool, args, cancellationToken: ct);
            text = r.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";
            Assert.True(r.IsError != true, tool + " returned an error. " + text);
            Assert.False(string.IsNullOrWhiteSpace(text), tool + " returned no text content");
        });
        return text;
    }

    private static JsonElement Parse(string text)
    {
        using var doc = JsonDocument.Parse(text);
        return doc.RootElement.Clone();
    }

    /// <summary>Whether the result is a list, or an object carrying at least one.</summary>
    private static bool HasList(JsonElement root) =>
        root.ValueKind == JsonValueKind.Array
        || (root.ValueKind == JsonValueKind.Object
            && root.EnumerateObject().Any(p => p.Value.ValueKind == JsonValueKind.Array));

    private static Dictionary<string, object?> Args(params (string Key, object? Value)[] pairs)
    {
        var args = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var (key, value) in pairs) args[key] = value;
        return args;
    }
}
