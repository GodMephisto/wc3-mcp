// src/Wc3.MapDocument/FolderMap.cs
using War3Net.IO.Mpq;

namespace Wc3.Model;

/// <summary>
/// A map saved in folder mode, which the Reforged World Editor offers beside the archive form. The
/// map is a directory named like the archive (Name.w3x) holding the same files loose, with the
/// archive's backslash paths as subfolders.
/// </summary>
/// <remarks>
/// Who uses it. <see cref="MapDocument.Load(string)"/> when the path is a directory, and
/// <see cref="MapDocument.Save(string)"/> for a document that came from one, so every command,
/// the CLI, the MCP server and the Studio read and write folder maps without a second code path.
///
/// Why pack rather than teach the document a second storage. The document, its deferred reads and
/// its save are all built on an archive. Packing the folder into an archive in memory reuses every
/// one of them, and saving unpacks the saved archive, so a dirty entry is serialized by exactly the
/// code that serializes it for a .w3x. A file nobody edited is written back with its own bytes.
///
/// What is left out. The archive bookkeeping, (listfile), (attributes) and (signature), is never
/// written to a folder, since a folder has no hash table for it to describe.
/// </remarks>
public static class FolderMap
{
    private static readonly HashSet<string> Bookkeeping = new(StringComparer.OrdinalIgnoreCase)
    {
        "(listfile)", "(attributes)", "(signature)",
    };

    /// <summary>The folder's files as one MPQ archive in memory, named by their relative paths.</summary>
    public static byte[] Pack(string folder)
    {
        var root = Path.GetFullPath(folder);
        var files = new List<MpqFile>();
        foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var name = Path.GetRelativePath(root, path).Replace('/', '\\');
            if (Bookkeeping.Contains(name)) continue;
            files.Add(MpqFile.New(new MemoryStream(File.ReadAllBytes(path)), name));
        }
        if (files.Count == 0) throw new InvalidDataException($"{folder} holds no files, so it is not a folder map.");

        using var mpq = new MemoryStream();
        using (MpqArchive.Create(mpq, files, new MpqArchiveCreateOptions
               {
                   ListFileCreateMode = MpqFileCreateMode.Overwrite,
                   HashTableSize = HashTableFor(files.Count),
               }, leaveOpen: true)) { }
        return mpq.ToArray();
    }

    /// <summary>
    /// Writes a saved archive out as a folder map at <paramref name="folder"/>. The files land in a
    /// fresh sibling folder first and replace the target only when every one is written, so a
    /// failure part way leaves the target as it was.
    /// </summary>
    public static void Unpack(byte[] archive, string folder)
    {
        var target = Path.GetFullPath(folder);
        if (File.Exists(target))
            throw new IOException($"{target} is a file. A folder map is saved as a folder, pick another path.");

        var saved = MapDocument.Load(archive);
        var unnamed = saved.Files.Count(f => f.FileName is null);
        if (unnamed > 0)
            throw new InvalidDataException($"{unnamed} entries have no name, and a folder map cannot hold an unnamed file.");

        var staging = target.TrimEnd(Path.DirectorySeparatorChar) + ".saving-" + Guid.NewGuid().ToString("N")[..8];
        Directory.CreateDirectory(staging);
        try
        {
            foreach (var f in saved.Files)
            {
                if (Bookkeeping.Contains(f.FileName!)) continue;
                var path = Path.Combine(staging, f.FileName!.Replace('\\', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllBytes(path, f.RawBytes);
            }

            if (Directory.Exists(target))
            {
                var old = staging + ".old";
                Directory.Move(target, old);
                Directory.Move(staging, target);
                Directory.Delete(old, recursive: true);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                Directory.Move(staging, target);
            }
        }
        catch
        {
            try { if (Directory.Exists(staging)) Directory.Delete(staging, recursive: true); } catch { }
            throw;
        }
    }

    /// <summary>A power of two with room for every file plus the bookkeeping, at most half full.</summary>
    private static ushort HashTableFor(int files)
    {
        int size = 16;
        while (size < (files + 3) * 2 && size < 0x8000) size <<= 1;
        return (ushort)size;
    }
}
