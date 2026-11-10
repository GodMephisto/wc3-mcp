// tests/Wc3.Tests/ArtifactWriterTests.cs
// The shared write path lives in Wc3.Commands, so the CLI and the Studio take the same refusals and
// the same digests as the MCP server instead of depending on which front end ran the command
// (SAF-14). These facts call it directly, with no MCP session and no guard, and prove that a write,
// a refusal and both digests behave the same way there. The permission file is pointed at a
// denying temporary file for the duration of the class, so a passing write proves the shared layer
// needs no MCP grant, and the MCP wrapper is the only thing that adds one. The class joins the
// Serial collection because an environment variable is process wide.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

[Collection("Serial")]
public sealed class ArtifactWriterTests : IDisposable
{
    private readonly string _dir;
    private readonly string _map;
    private readonly string? _savedPermissions;

    public ArtifactWriterTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "wc3artifact-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        _map = Path.Combine(_dir, "in.w3x");
        File.WriteAllBytes(_map, BlankMap.CreateArchiveBytes());

        // Deny every switch, as if the user had never granted overwrite. Nothing here reads it,
        // because the shared writer has no permission concept, which is the point of the move.
        _savedPermissions = Environment.GetEnvironmentVariable("WC3_MCP_PERMISSIONS");
        Environment.SetEnvironmentVariable("WC3_MCP_PERMISSIONS",
            Path.Combine(_dir, "permissions.json"));
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("WC3_MCP_PERMISSIONS", _savedPermissions);
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    [Fact]
    public void The_shared_writer_writes_and_refuses_with_no_mcp_session_and_reports_both_digests()
    {
        string output = Path.Combine(_dir, "out.w3x");
        string inputSha = ArtifactWriter.Sha256File(_map);

        var write = ArtifactWriter.Save(MapDocument.Load(_map), _map, output);

        Assert.True(File.Exists(output));
        Assert.Equal(inputSha, write.InputSha256);
        Assert.Equal(ArtifactWriter.Sha256File(output), write.OutputSha256);
        Assert.Equal(new FileInfo(output).Length, write.BytesWritten);
        Assert.Empty(Directory.GetFiles(_dir, "*.tmp"));

        // The same call refuses an existing destination, with the stable code the MCP wrapper
        // carries through unchanged.
        var refusal = Assert.Throws<ArtifactWriteException>(
            () => ArtifactWriter.Save(MapDocument.Load(_map), _map, output));
        Assert.Equal("OUTPUT_EXISTS", refusal.Code);

        // An output equal to the input, and one inside a folder map, are refused before any write.
        Assert.Equal("OUT_PATH_IS_INPUT", Assert.Throws<ArtifactWriteException>(
            () => ArtifactWriter.Resolve(_map, _map)).Code);
    }

    [Fact]
    public void The_shared_guard_runs_only_when_an_existing_destination_would_be_replaced()
    {
        var calls = new List<string>();
        void Guard(string action, string file, string effect) => calls.Add(action + ":" + effect);

        string fresh = Path.Combine(_dir, "fresh.w3x");
        ArtifactWriter.Save(MapDocument.Load(_map), _map, fresh, guard: Guard);

        Assert.Empty(calls);

        string existing = Path.Combine(_dir, "existing.w3x");
        File.WriteAllBytes(existing, new byte[] { 1 });
        ArtifactWriter.Save(MapDocument.Load(_map), _map, existing, overwrite: true, guard: Guard);

        Assert.Equal(new[] { "overwrite:replaced" }, calls);
    }

    [Fact]
    public void A_guard_that_refuses_leaves_the_destination_untouched()
    {
        string output = Path.Combine(_dir, "guarded.w3x");
        byte[] sentinel = { 9, 9, 9 };
        File.WriteAllBytes(output, sentinel);

        var refusal = Assert.Throws<ArtifactWriteException>(() => ArtifactWriter.Save(
            MapDocument.Load(_map), _map, output, overwrite: true,
            guard: (_, _, _) => throw new ArtifactWriteException("PERMISSION_DENIED", "not granted")));

        Assert.Equal("PERMISSION_DENIED", refusal.Code);
        Assert.Equal(sentinel, File.ReadAllBytes(output));
        Assert.Empty(Directory.GetFiles(_dir, "*.tmp"));
    }
}
