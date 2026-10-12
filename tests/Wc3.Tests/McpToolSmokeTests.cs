// tests/Wc3.Tests/McpToolSmokeTests.cs
// Calls every MCP tool through the protocol, the way an AI client does. A read tool must answer
// with the data asked for. A write tool must leave its input map byte-for-byte unchanged, write a
// map that opens again, and the change must be there when read back through another tool.
// The tests are split by tool family into McpToolSmokeTests.<Family>.cs. Every test names the
// tools it covers with [Covers], and Every_tool_is_covered fails for any tool left out.
using Wc3.Model;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Wc3.Tests;

/// <summary>Names the MCP tools a smoke test exercises, for the coverage check.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class CoversAttribute : Attribute
{
    public CoversAttribute(params string[] tools) => Tools = tools;
    public string[] Tools { get; }
}

/// <summary>One blank map made through the new_map tool, shared by every smoke test.</summary>
public sealed class SmokeFixture : IDisposable
{
    public string Dir { get; } = Path.Combine(Path.GetTempPath(), "wc3mcp-smoke", Guid.NewGuid().ToString("N"));
    public string Map { get; }

    public SmokeFixture()
    {
        Directory.CreateDirectory(Dir);
        Map = Path.Combine(Dir, "fixture.w3x");
        McpTestClient.WithClient(async (client, ct) =>
        {
            var r = await client.CallToolAsync("new_map",
                new Dictionary<string, object?> { ["out_path"] = Map, ["name"] = "Smoke Fixture", ["tiles"] = 32 },
                cancellationToken: ct);
            Assert.True(r.IsError != true, "new_map failed, so no smoke test can run");
        }).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        try { Directory.Delete(Dir, recursive: true); } catch (IOException) { }
    }
}

/// <summary>Shared helpers for the per-area MCP smoke test classes. Split so xUnit can run the
/// areas in parallel, since it never runs two tests of one class at once.</summary>
public abstract class McpSmokeBase : IClassFixture<SmokeFixture>
{
    protected readonly SmokeFixture _f;
    protected McpSmokeBase(SmokeFixture f) => _f = f;

    /// <summary>The shared blank map. Never written to, every write test checks that.</summary>
    protected string Fixture => _f.Map;

    /// <summary>The user slots a blank map gets, which its script's SetPlayers sets up too.</summary>
    protected static readonly int BlankPlayers = new Wc3.Model.BlankMapOptions().PlayerCount;

    /// <summary>A fresh output path in the test folder.</summary>
    protected string NewOut(string label, string ext = ".w3x") =>
        Path.Combine(_f.Dir, $"{label}-{Guid.NewGuid():N}{ext}");

    /// <summary>Calls a tool and returns its JSON result. Fails with the tool's own message on an error.</summary>
    protected static async Task<JsonElement> Call(string tool, Dictionary<string, object?> args)
    {
        JsonElement root = default;
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var r = await client.CallToolAsync(tool, args, cancellationToken: ct);
            var text = r.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text;
            if (r.IsError == true) Assert.Fail($"{tool} returned an error. {text}");
            Assert.False(string.IsNullOrWhiteSpace(text), $"{tool} returned no text content");
            using var doc = JsonDocument.Parse(text!);
            root = doc.RootElement.Clone();
        });
        return root;
    }

    /// <summary>Calls a tool that must refuse, and returns its message.</summary>
    protected static async Task<string> CallError(string tool, Dictionary<string, object?> args)
    {
        string message = "";
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var r = await client.CallToolAsync(tool, args, cancellationToken: ct);
            message = r.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";
            Assert.True(r.IsError == true, $"{tool} should have refused but answered {message}");
        });
        Assert.False(string.IsNullOrWhiteSpace(message), $"{tool} refused without saying why");
        return message;
    }

    /// <summary>
    /// Calls a writing tool on <paramref name="map"/> (the fixture by default) with a new out_path.
    /// Asserts the input map's bytes did not change and the output opens through map_info.
    /// Returns the output path, so the caller can read the change back.
    /// </summary>
    protected async Task<string> Write(string tool, Dictionary<string, object?> args, string? map = null)
    {
        map ??= Fixture;
        var outPath = NewOut(tool);
        args["map"] = map;
        args["out_path"] = outPath;
        var before = Sha(map);
        await Call(tool, args);
        Assert.Equal(before, Sha(map));
        Assert.True(File.Exists(outPath), $"{tool} wrote nothing at {outPath}");
        await Call("map_info", new() { ["map"] = outPath });
        return outPath;
    }

    protected static string Sha(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    /// <summary>A property by name, ignoring case, since the server writes camelCase.</summary>
    protected static JsonElement Prop(JsonElement e, string name)
    {
        foreach (var p in e.EnumerateObject())
            if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)) return p.Value;
        Assert.Fail($"no property {name} in {e}");
        return default;
    }

    /// <summary>The rows of a result, whether it is a bare array or an object holding one array.</summary>
    protected static List<JsonElement> Rows(JsonElement e)
    {
        if (e.ValueKind == JsonValueKind.Array) return e.EnumerateArray().ToList();
        var arrays = e.EnumerateObject().Where(p => p.Value.ValueKind == JsonValueKind.Array).ToList();
        Assert.True(arrays.Count == 1, $"expected one array in {e}");
        return arrays[0].Value.EnumerateArray().ToList();
    }

    /// <summary>The fixture plus one footman, one tree and one region, all placed through MCP.</summary>
    protected async Task<string> PopulatedMap()
    {
        var withUnit = await Write("place_unit", Args(("type_rawcode", "hfoo"), ("owner_id", 0), ("x", 64f), ("y", -64f)));
        var withDoodad = await Write("place_doodad", Args(("type_rawcode", "LTlt"), ("x", 128f), ("y", 128f)), withUnit);
        return await Write("place_region", Args(("name", "Spawn"), ("left", -256f), ("bottom", -256f), ("right", 256f), ("top", 256f)), withDoodad);
    }


    protected string FolderFixture()
    {
        var folder = Path.Combine(_f.Dir, $"folder-{Guid.NewGuid():N}.w3x");
        foreach (var f in MapDocument.Load(Fixture).Files)
        {
            if (f.FileName is null || f.FileName.StartsWith('(')) continue;
            var path = Path.Combine(folder, f.FileName.Replace('\\', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, f.RawBytes);
        }
        return folder;
    }


    protected static Dictionary<string, object?> Args(params (string Key, object? Value)[] pairs) =>
        pairs.ToDictionary(p => p.Key, p => p.Value);

}

public sealed partial class McpToolSmokeTests : McpSmokeBase
{
    public McpToolSmokeTests(SmokeFixture f) : base(f) { }

    [Fact]
    public async Task Every_tool_is_covered()
    {
        var covered = typeof(McpSmokeBase).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(McpSmokeBase)))
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(m => m.GetCustomAttribute<CoversAttribute>()?.Tools ?? Array.Empty<string>())
            .ToHashSet(StringComparer.Ordinal);
        var served = new HashSet<string>(StringComparer.Ordinal);
        await McpTestClient.WithClient(async (client, ct) =>
        {
            foreach (var t in await client.ListToolsAsync(cancellationToken: ct)) served.Add(t.Name);
        });

        var untested = served.Except(covered).Order().ToList();
        var unknown = covered.Except(served).Order().ToList();
        Assert.True(untested.Count == 0, "tools with no smoke test: " + string.Join(", ", untested));
        Assert.True(unknown.Count == 0, "smoke tests naming tools the server does not have: " + string.Join(", ", unknown));
    }
}
