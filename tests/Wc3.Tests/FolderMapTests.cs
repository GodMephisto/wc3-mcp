// tests/Wc3.Tests/FolderMapTests.cs
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>Maps saved in the World Editor's folder mode, a directory of loose files.</summary>
public class FolderMapTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "wc3-folder", Guid.NewGuid().ToString("N"));

    public FolderMapTests() => Directory.CreateDirectory(_dir);
    public void Dispose() { try { Directory.Delete(_dir, recursive: true); } catch { } }

    /// <summary>Unpacks a blank archive map into a folder map, the shape the editor saves.</summary>
    private string BlankFolder()
    {
        var folder = Path.Combine(_dir, "Blank.w3x");
        foreach (var f in BlankMap.Create().Files)
        {
            if (f.FileName!.StartsWith('(')) continue;
            var path = Path.Combine(folder, f.FileName.Replace('\\', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, f.RawBytes);
        }
        Directory.CreateDirectory(Path.Combine(folder, "war3mapImported"));
        File.WriteAllBytes(Path.Combine(folder, "war3mapImported", "icon.blp"), new byte[] { 1, 2, 3, 250 });
        return folder;
    }

    private static Dictionary<string, string> Hashes(string folder) =>
        Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).ToDictionary(
            p => Path.GetRelativePath(folder, p),
            p => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(p))));

    [Fact]
    public void Load_reads_every_file_with_its_subfolder_as_a_backslash_path()
    {
        var folder = BlankFolder();
        var doc = MapDocument.Load(folder);
        Assert.Equal(Path.GetFullPath(folder), doc.SourceFolder);
        Assert.NotNull(doc.GetFile("war3map.w3i"));
        Assert.Equal(new byte[] { 1, 2, 3, 250 }, doc.GetFile(@"war3mapImported\icon.blp")!.RawBytes);
    }

    [Fact]
    public void Save_untouched_writes_a_byte_identical_folder_without_bookkeeping()
    {
        var folder = BlankFolder();
        var copy = Path.Combine(_dir, "Copy.w3x");
        MapDocument.Load(folder).Save(copy);
        Assert.True(Directory.Exists(copy));
        Assert.Equal(Hashes(folder), Hashes(copy));
    }

    [Fact]
    public void Save_after_an_edit_changes_that_file_only()
    {
        var folder = BlankFolder();
        var doc = MapDocument.Load(folder);
        doc.AddOrReplaceRawFile(@"war3mapImported\icon.blp", new byte[] { 9 });
        var copy = Path.Combine(_dir, "Edited.w3x");
        doc.Save(copy);

        var before = Hashes(folder);
        var after = Hashes(copy);
        var changed = before.Keys.Where(k => before[k] != after[k]).ToList();
        Assert.Equal(new[] { Path.Combine("war3mapImported", "icon.blp") }, changed);
        Assert.Equal(new byte[] { 9 }, MapDocument.Load(copy).GetFile(@"war3mapImported\icon.blp")!.RawBytes);
    }

    [Fact]
    public void Save_over_an_existing_folder_replaces_it_and_drops_removed_files()
    {
        var folder = BlankFolder();
        var target = Path.Combine(_dir, "Target.w3x");
        Directory.CreateDirectory(target);
        File.WriteAllText(Path.Combine(target, "stale.txt"), "old");
        MapDocument.Load(folder).Save(target);
        Assert.False(File.Exists(Path.Combine(target, "stale.txt")));
        Assert.Equal(Hashes(folder), Hashes(target));
        Assert.Empty(Directory.EnumerateDirectories(_dir, "*.saving-*"));
    }

    [Fact]
    public void An_empty_folder_is_refused()
    {
        var empty = Path.Combine(_dir, "Empty.w3x");
        Directory.CreateDirectory(empty);
        Assert.Throws<InvalidDataException>(() => MapDocument.Load(empty));
    }
}
