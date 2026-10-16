// tests/Wc3.Tests/SelfUpdateCommandTests.cs
// Hermetic tests for the lock-safe self-updater. Everything runs against temp folders, and the
// locked cases hold a real FileStream open with FileShare.Read | FileShare.Delete, which is the
// exact sharing a running process uses, so the rename-then-replace path is exercised for real.
using Wc3.Commands;

namespace Wc3.Tests;

public class SelfUpdateCommandTests
{
    [Fact]
    public void CleanupOldFiles_removes_old_files_in_the_tree_and_keeps_the_rest()
    {
        using var tmp = new TempDir();
        string sub = Path.Combine(tmp.Dir, "sub");
        Directory.CreateDirectory(sub);
        File.WriteAllText(Path.Combine(tmp.Dir, "wc3ctl.exe.abc123.old"), "x");
        File.WriteAllText(Path.Combine(sub, "wc3-mcp.old"), "y");
        File.WriteAllText(Path.Combine(sub, "keep.dll"), "z");

        int removed = SelfUpdateCommand.CleanupOldFiles(tmp.Dir);

        Assert.Equal(2, removed);
        Assert.False(File.Exists(Path.Combine(tmp.Dir, "wc3ctl.exe.abc123.old")));
        Assert.False(File.Exists(Path.Combine(sub, "wc3-mcp.old")));
        Assert.True(File.Exists(Path.Combine(sub, "keep.dll")));
    }

    [Fact]
    public void CleanupOldFiles_skips_an_old_file_still_held_open()
    {
        using var tmp = new TempDir();
        string locked = Path.Combine(tmp.Dir, "locked.old");
        File.WriteAllText(locked, "x");

        // FileShare.Read alone denies delete, so the remove fails and must be swallowed.
        using (new FileStream(locked, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            int removed = SelfUpdateCommand.CleanupOldFiles(tmp.Dir);
            Assert.Equal(0, removed);
        }
    }

    [Fact]
    public void ReplaceDirectoryLockSafe_replaces_a_locked_file_via_an_old_rename()
    {
        using var tmp = new TempDir();
        string unpacked = Path.Combine(tmp.Dir, "unpacked");
        string target = Path.Combine(tmp.Dir, "target");
        Directory.CreateDirectory(unpacked);
        Directory.CreateDirectory(target);
        string targetExe = Path.Combine(target, "wc3ctl.exe");
        File.WriteAllText(targetExe, "old");
        File.WriteAllText(Path.Combine(unpacked, "wc3ctl.exe"), "new");

        UpdateApplyResult result;
        using (new FileStream(targetExe, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
        {
            result = SelfUpdateCommand.ReplaceDirectoryLockSafe(unpacked, target);

            // The running file was moved aside, so a fresh copy could land at the same path.
            Assert.Equal(1, result.FilesCopied);
            Assert.Equal(1, result.FilesRenamed);
            Assert.Equal("new", File.ReadAllText(targetExe));
        }

        var aside = Directory.GetFiles(target, "*.old");
        Assert.Single(aside);
        Assert.Equal("old", File.ReadAllText(aside[0]));
    }

    [Fact]
    public void ReplaceDirectoryLockSafe_copies_subfolders_and_rewrites_an_unlocked_file()
    {
        using var tmp = new TempDir();
        string unpacked = Path.Combine(tmp.Dir, "unpacked");
        string target = Path.Combine(tmp.Dir, "target");
        Directory.CreateDirectory(Path.Combine(unpacked, "schemas"));
        Directory.CreateDirectory(target);
        File.WriteAllText(Path.Combine(unpacked, "wc3ctl.exe"), "cli-v2");
        File.WriteAllText(Path.Combine(unpacked, "schemas", "index.json"), "schema-v2");

        var result = SelfUpdateCommand.ReplaceDirectoryLockSafe(unpacked, target);

        Assert.Equal(2, result.FilesCopied);
        Assert.Equal(0, result.FilesRenamed);
        Assert.Equal("cli-v2", File.ReadAllText(Path.Combine(target, "wc3ctl.exe")));
        Assert.Equal("schema-v2", File.ReadAllText(Path.Combine(target, "schemas", "index.json")));
    }

    [Fact]
    public void ReplaceDirectoryLockSafe_leaves_a_preserved_folder_untouched()
    {
        using var tmp = new TempDir();
        string unpacked = Path.Combine(tmp.Dir, "unpacked");
        string target = Path.Combine(tmp.Dir, "target");
        Directory.CreateDirectory(Path.Combine(unpacked, "schemas"));
        Directory.CreateDirectory(Path.Combine(target, "schemas"));
        File.WriteAllText(Path.Combine(unpacked, "schemas", "index.json"), "from-zip");
        File.WriteAllText(Path.Combine(target, "schemas", "index.json"), "installed");
        File.WriteAllText(Path.Combine(unpacked, "wc3ctl.exe"), "cli-v2");

        var preserve = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "schemas" };
        var result = SelfUpdateCommand.ReplaceDirectoryLockSafe(unpacked, target, preserve);

        Assert.Equal(1, result.FilesCopied);
        Assert.Equal("installed", File.ReadAllText(Path.Combine(target, "schemas", "index.json")));
        Assert.Equal("cli-v2", File.ReadAllText(Path.Combine(target, "wc3ctl.exe")));
    }
}
