// tests/Wc3.Tests/MapDocumentSaveTests.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class MapDocumentSaveTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-map-save", Guid.NewGuid().ToString("N"));

    public MapDocumentSaveTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        // Flat scratch folder this test made: delete the files it wrote, then the folder, without
        // a recursive walk that could follow a link out of it.
        try
        {
            foreach (var file in Directory.EnumerateFiles(_dir)) File.Delete(file);
            Directory.Delete(_dir);
        }
        catch { }
    }

    private static byte[] Map(string w3i) => SyntheticMap.Build(new Dictionary<string, byte[]>
    {
        ["war3map.w3i"] = Encoding.ASCII.GetBytes(w3i),
        ["war3map.j"]   = new byte[] { 1, 2, 3 },
    });

    private string[] FolderEntries() =>
        Directory.EnumerateFileSystemEntries(_dir)
            .Select(p => Path.GetFileName(p)!)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

    [Fact]
    public void Save_over_an_existing_map_replaces_it_parses_back_and_leaves_no_temp_file()
    {
        var path = Path.Combine(_dir, "map.w3x");
        MapDocument.Load(Map("old")).Save(path);
        Assert.Equal(new[] { "map.w3x" }, FolderEntries());

        // A second save over the file exercises the overwrite path, not just the create path.
        MapDocument.Load(Map("new")).Save(path);

        var reopened = MapDocument.Load(path);
        Assert.Equal(Encoding.ASCII.GetBytes("new"), reopened.GetFile("war3map.w3i")!.RawBytes);
        Assert.Equal(new byte[] { 1, 2, 3 }, reopened.GetFile("war3map.j")!.RawBytes);

        // Only the destination: a staged ".map.w3x.<8 hex>.tmp" would show up here.
        Assert.Equal(new[] { "map.w3x" }, FolderEntries());
        Assert.DoesNotContain(Directory.EnumerateFileSystemEntries(_dir),
            p => p.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void A_save_that_fails_after_staging_leaves_the_previous_file_byte_for_byte()
    {
        var path = Path.Combine(_dir, "map.w3x");
        MapDocument.Load(Map("original")).Save(path);
        var before = File.ReadAllBytes(path);

        var doc = MapDocument.Load(Map("replacement"));
        // Guard that the payload really differs from the file on disk, so the assertions below
        // cannot pass merely because the save would have produced identical bytes.
        Assert.NotEqual(before, doc.SaveToBytes());

        string? staged = null;
        byte[]? destinationAtRename = null;
        try
        {
            // The fault is injected at the only moment that matters: the staged bytes are written
            // and flushed, and the destination still holds the previous map. A permission denial
            // or a full disk cannot be produced reliably in a hermetic test, and a throw inside
            // SaveToBytes would fail before anything is staged, proving nothing about this window.
            // The hook is the smallest seam: internal, thread-static, null in production.
            MapDocument.BeforeRenameForTests = temp =>
            {
                staged = temp;
                destinationAtRename = File.ReadAllBytes(path);
                throw new IOException("injected failure before rename");
            };

            var thrown = Assert.Throws<IOException>(() => doc.Save(path));
            Assert.Equal("injected failure before rename", thrown.Message);
        }
        finally
        {
            MapDocument.BeforeRenameForTests = null;
        }

        // At the failure boundary the destination was still the old map, and the staged file was
        // its own ".map.w3x.<8 hex>.tmp" beside it, not the destination.
        Assert.Equal(before, destinationAtRename);
        Assert.NotNull(staged);
        Assert.Equal(_dir, Path.GetDirectoryName(staged!));
        Assert.Matches(@"^\.map\.w3x\.[0-9a-f]{8}\.tmp$", Path.GetFileName(staged!));

        // After the failed save: the old bytes are intact and the staging file is gone.
        Assert.Equal(before, File.ReadAllBytes(path));
        Assert.False(File.Exists(staged));
        Assert.Equal(new[] { "map.w3x" }, FolderEntries());
    }
}
