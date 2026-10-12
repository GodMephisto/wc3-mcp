// src/Wc3.Commands/GamedataExportSchemasCommand.cs
using System.IO.Compression;

namespace Wc3.Commands;

/// <summary>
/// The outcome of exporting the schema dataset. Builds names every schema folder written, Files and
/// Bytes count the whole export, and Zipped is true when the target is one archive rather than a
/// directory.
/// </summary>
public sealed record GamedataExportResult(
    string Target, bool Zipped, IReadOnlyList<string> Builds, int Files, long Bytes);

/// <summary>
/// Copies the whole schema dataset into a target directory, or packs it into a single zip when the
/// target ends in .zip, so the untracked dataset can be attached to a release. The layout is the same
/// either way, one folder per build plus any top level index.json or sources.json, and file order is
/// ordinal so an export is reproducible.
/// </summary>
public static class GamedataExportSchemasCommand
{
    public static GamedataExportResult Execute(string target, string? schemasDir = null)
    {
        var dir = GamedataSupport.ResolveDir(schemasDir);
        var full = Path.GetFullPath(target);
        bool zipped = full.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
            .Select(f => (Full: f, Rel: Path.GetRelativePath(dir, f).Replace('\\', '/')))
            .OrderBy(x => x.Rel, StringComparer.Ordinal)
            .ToList();

        long bytes = files.Sum(f => new FileInfo(f.Full).Length);
        if (zipped) WriteZip(full, files);
        else CopyTree(full, files);

        return new GamedataExportResult(full, zipped, EnumerateBuilds(dir), files.Count, bytes);
    }

    /// <summary>Every schema folder name, ordered ordinal, so the report names what was carried.</summary>
    private static List<string> EnumerateBuilds(string dir) =>
        Directory.EnumerateDirectories(dir)
            .Where(d => File.Exists(Path.Combine(d, "manifest.json")))
            .Select(d => Path.GetFileName(d))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

    private static void CopyTree(string target, IReadOnlyList<(string Full, string Rel)> files)
    {
        Directory.CreateDirectory(target);
        foreach (var f in files)
        {
            var dest = Path.Combine(target, f.Rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(f.Full, dest, overwrite: true);
        }
    }

    private static void WriteZip(string target, IReadOnlyList<(string Full, string Rel)> files)
    {
        var parent = Path.GetDirectoryName(target);
        if (!string.IsNullOrEmpty(parent)) Directory.CreateDirectory(parent);
        if (File.Exists(target)) File.Delete(target);
        using var zip = ZipFile.Open(target, ZipArchiveMode.Create);
        foreach (var f in files)
            zip.CreateEntryFromFile(f.Full, f.Rel, CompressionLevel.Optimal);
    }
}
