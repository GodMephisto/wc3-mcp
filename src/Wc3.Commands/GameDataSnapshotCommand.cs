// src/Wc3.Commands/GameDataSnapshotCommand.cs
using System.Diagnostics;
using System.Text;
using Wc3.GameData;

namespace Wc3.Commands;

/// <summary>What a snapshot wrote, so a caller can report it without re-walking the tree.</summary>
public sealed record SnapshotResult(
    string OutDir,
    string? Build,
    int FilesWritten,
    int FilesSkipped,
    long BytesWritten,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Dumps the base game's text and data files to a directory, so they can be committed and diffed
/// across patches.
/// </summary>
/// <remarks>
/// Blizzard publishes no diff of what a patch changes inside the game data. The patch notes for
/// 3.0.0 say "new ability natives" without naming one, and say nothing at all about the base
/// stat and gameplay-constant changes that silently retune every custom map which inherits them.
/// The only authority is the shipped data, and the only way to see a change is to have kept the
/// previous copy.
///
/// The tracked set is deliberately small. The storage holds about 175,000 files, almost all of
/// them art, and the ones that decide behaviour are roughly 1,200 text files. That is a trivial
/// git repository and it diffs cleanly, where snapshotting everything would not.
///
/// This covers the DATA half only. The World Editor's own behaviour lives in its executable, not
/// in the storage, so a data snapshot will never show a new editor dialog or a new map file
/// format. <see cref="IncludeBinaryStrings"/> captures that half by dumping the printable strings
/// of the game and editor binaries, which is how war3map.w3l and the post-processing tool were
/// found in the first place.
/// </remarks>
public static class GameDataSnapshotCommand
{
    /// <summary>The extensions that decide behaviour. Everything else in the storage is art.</summary>
    public static readonly string[] TrackedExtensions = { ".j", ".lua", ".slk", ".txt", ".fdf", ".ai" };

    /// <summary>Binaries whose strings carry the editor and client half of a patch.</summary>
    public static readonly string[] BinaryStringSources = { "World Editor.exe", "Warcraft III.exe" };

    /// <summary>Runs a snapshot. Never throws for a missing install or an unreadable file, both
    /// are reported, because a partial snapshot is still worth having and an exception here would
    /// lose the files already written.</summary>
    public static SnapshotResult Run(
        string? gameDirOverride,
        string outDir,
        bool includeLocales = false,
        bool includeBinaryStrings = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outDir);
        var diagnostics = new List<string>();

        string? install = ResolveInstall(gameDirOverride, diagnostics);
        if (install is null)
            return new SnapshotResult(outDir, null, 0, 0, 0, diagnostics);

        string? build = ReadBuild(install);
        Directory.CreateDirectory(outDir);

        int written = 0, skipped = 0;
        long bytes = 0;

        if (!CascGameDataSource.TryOpen(install, out var casc, out var error) || casc is null)
        {
            diagnostics.Add($"could not open game data, {error}");
            return new SnapshotResult(outDir, build, 0, 0, 0, diagnostics);
        }

        using (casc)
        {
            foreach (var name in casc.EnumerateFileNames())
            {
                if (!TrackedExtensions.Any(e => name.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
                    continue;
                if (!includeLocales && name.Contains("_locales", StringComparison.OrdinalIgnoreCase))
                    continue;

                byte[]? payload;
                try { payload = casc.ReadFile(name); }
                catch (Exception ex) { skipped++; diagnostics.Add($"{name}, {ex.Message}"); continue; }
                if (payload is null) { skipped++; continue; }

                string dest = Path.Combine(outDir, "data", SafeRelativePath(name));
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                File.WriteAllBytes(dest, payload);
                written++;
                bytes += payload.Length;
            }
        }

        if (includeBinaryStrings)
        {
            foreach (var exe in BinaryStringSources)
            {
                string path = Path.Combine(install, "_retail_", "x86_64", exe);
                if (!File.Exists(path)) { diagnostics.Add($"binary not found, {path}"); continue; }
                try
                {
                    string dest = Path.Combine(outDir, "binary",
                        Path.GetFileNameWithoutExtension(exe).Replace(' ', '_') + ".strings.txt");
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    int count = WriteStrings(path, dest);
                    written++;
                    bytes += new FileInfo(dest).Length;
                    diagnostics.Add($"{exe}, {count:N0} distinct strings");
                }
                catch (Exception ex) { skipped++; diagnostics.Add($"{exe}, {ex.Message}"); }
            }
        }

        WriteManifest(outDir, install, build, written, bytes);
        return new SnapshotResult(outDir, build, written, skipped, bytes, diagnostics);
    }

    private static string? ResolveInstall(string? overrideDir, List<string> diagnostics)
    {
        var dir = GameInstall.Locate(overrideDir);
        if (dir is not null) return dir;
        diagnostics.Add("Warcraft III install not found (pass --game-dir <path>)");
        return null;
    }

    /// <summary>The build string, which is what a snapshot is named after. Read from the client
    /// binary because that is what the crash reports and the patch notes both quote.</summary>
    private static string? ReadBuild(string install)
    {
        string exe = Path.Combine(install, "_retail_", "x86_64", "Warcraft III.exe");
        if (!File.Exists(exe)) return null;
        try { return FileVersionInfo.GetVersionInfo(exe).FileVersion; }
        catch { return null; }
    }

    /// <summary>Turns a storage name into a relative path. The layer separator is a colon, which
    /// is not legal in a Windows path, so it becomes a directory level and the structure survives.</summary>
    public static string SafeRelativePath(string cascName)
    {
        var sb = new StringBuilder(cascName.Length);
        foreach (char c in cascName)
            sb.Append(c == ':' ? Path.DirectorySeparatorChar
                    : c == '/' ? Path.DirectorySeparatorChar
                    : Path.GetInvalidFileNameChars().Contains(c) && c != Path.DirectorySeparatorChar
                        && c != '\\' ? '_'
                    : c);
        return sb.ToString().Replace('\\', Path.DirectorySeparatorChar).Trim(Path.DirectorySeparatorChar);
    }

    /// <summary>Printable ASCII and UTF-16LE runs, sorted and deduplicated so the file diffs as a
    /// set rather than as a layout. A patch that moves code without changing text then produces no
    /// diff, which is the intent.</summary>
    private static int WriteStrings(string binary, string dest, int min = 4)
    {
        var bytes = File.ReadAllBytes(binary);
        var found = new HashSet<string>(StringComparer.Ordinal);

        var run = new StringBuilder();
        foreach (var b in bytes)
        {
            if (b >= 0x20 && b <= 0x7E) run.Append((char)b);
            else { if (run.Length >= min) found.Add(run.ToString()); run.Clear(); }
        }
        if (run.Length >= min) found.Add(run.ToString());

        run.Clear();
        for (int i = 0; i + 1 < bytes.Length; i += 2)
        {
            if (bytes[i + 1] == 0 && bytes[i] >= 0x20 && bytes[i] <= 0x7E) run.Append((char)bytes[i]);
            else { if (run.Length >= min) found.Add(run.ToString()); run.Clear(); i--; }
        }
        if (run.Length >= min) found.Add(run.ToString());

        File.WriteAllLines(dest, found.OrderBy(s => s, StringComparer.Ordinal));
        return found.Count;
    }

    private static void WriteManifest(string outDir, string install, string? build, int files, long bytes)
    {
        var lines = new[]
        {
            "# wc3ctl game data snapshot",
            $"build      {build ?? "unknown"}",
            $"install    {install}",
            $"takenUtc   {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}Z",
            $"files      {files}",
            $"bytes      {bytes}",
            "",
            "# Commit this directory and re-run the snapshot after every patch. git diff then",
            "# shows every added native, changed default and retuned constant, which Blizzard",
            "# does not publish. data/ is the shipped game data. binary/ is the printable",
            "# strings of the client and editor, which is where editor-only changes show up.",
        };
        File.WriteAllLines(Path.Combine(outDir, "MANIFEST.txt"), lines);
    }
}
