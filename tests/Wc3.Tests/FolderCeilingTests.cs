// tests/Wc3.Tests/FolderCeilingTests.cs
// PRF-05. A folder call over the stated ceiling is refused before a single map opens, and the
// refusal names the ceiling. The files here are empty, so a call that reached the loader would
// report a map error instead, which is how the test tells the guard apart from the loader.
using ModelContextProtocol.Protocol;
using Wc3.Mcp;

namespace Wc3.Tests;

public sealed class FolderCeilingTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-folder-ceiling", Guid.NewGuid().ToString("N"));

    public FolderCeilingTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private static string? TextOf(CallToolResult result) =>
        result.Content?.OfType<TextContentBlock>().FirstOrDefault()?.Text;

    private CallToolResult Call(string tool, string folder)
    {
        CallToolResult? result = null;
        McpTestClient.WithClient(async (client, ct) =>
        {
            result = await client.CallToolAsync(
                tool,
                new Dictionary<string, object?> { ["folder"] = folder },
                cancellationToken: ct);
        }).GetAwaiter().GetResult();
        Assert.NotNull(result);
        return result!;
    }

    [Fact]
    public void A_folder_one_map_over_the_ceiling_is_refused_and_the_ceiling_is_named()
    {
        for (int i = 0; i <= ServerRuntime.MaxFolderMaps; i++)
            File.WriteAllBytes(Path.Combine(_dir, $"map{i:000}.w3x"), Array.Empty<byte>());

        CallToolResult result = Call("library_scan", _dir);
        string? text = TextOf(result);

        Assert.True(result.IsError == true, "a folder of 257 maps was accepted by library_scan");
        Assert.Contains("PRF05_FOLDER_ENTRIES", text);
        Assert.Contains(ServerRuntime.MaxFolderMaps.ToString(), text);
        Assert.Contains("ceiling", text);
    }

    [Fact]
    public void A_folder_under_the_ceiling_reaches_the_loader_instead_of_the_guard()
    {
        for (int i = 0; i < 3; i++)
            File.WriteAllBytes(Path.Combine(_dir, $"map{i:000}.w3x"), Array.Empty<byte>());

        CallToolResult result = Call("library_scan", _dir);
        string? text = TextOf(result) ?? "";

        // The fixtures are not maps, so the call fails at the loader. What it must not do is fail
        // at the ceiling, which is the only thing this test is about.
        Assert.DoesNotContain("PRF05_FOLDER_ENTRIES", text);
    }

    [Fact]
    public void Both_folder_arguments_of_a_compare_are_checked()
    {
        string second = Path.Combine(_dir, "second");
        Directory.CreateDirectory(second);
        for (int i = 0; i <= ServerRuntime.MaxFolderMaps; i++)
            File.WriteAllBytes(Path.Combine(second, $"map{i:000}.w3x"), Array.Empty<byte>());

        CallToolResult? result = null;
        McpTestClient.WithClient(async (client, ct) =>
        {
            result = await client.CallToolAsync(
                "library_compare",
                new Dictionary<string, object?> { ["folder_a"] = _dir, ["folder_b"] = second },
                cancellationToken: ct);
        }).GetAwaiter().GetResult();

        Assert.NotNull(result);
        Assert.True(result!.IsError == true, "an over ceiling second folder was accepted by library_compare");
        Assert.Contains("PRF05_FOLDER_ENTRIES", TextOf(result) ?? "");
    }
}
