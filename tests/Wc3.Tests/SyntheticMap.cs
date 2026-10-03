// tests/Wc3.Tests/SyntheticMap.cs
using System.Text;
using War3Net.IO.Mpq;
namespace Wc3.Tests;

public static class SyntheticMap
{
    public static byte[] Build(IReadOnlyDictionary<string, byte[]> files) =>
        Build(files, Array.Empty<byte[]>());

    /// <summary>
    /// Builds a map that additionally contains <paramref name="unnamedFiles"/> as
    /// entries present in the archive but absent from the (listfile) — the shape a
    /// protected map presents on load (FileName == null).
    /// </summary>
    public static byte[] Build(IReadOnlyDictionary<string, byte[]> files, IReadOnlyList<byte[]> unnamedFiles)
    {
        var builder = new MpqArchiveBuilder();
        foreach (var (name, data) in files)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));

        using var mpq = new MemoryStream();
        if (unnamedFiles.Count == 0)
        {
            builder.SaveTo(mpq, leaveOpen: true);
        }
        else
        {
            // Each unnamed payload needs a name at build time to receive a hash/block
            // entry, but the hand-written (listfile) below omits those names, so the
            // entries reopen unnamed (a real protection tool strips names the same way).
            for (int i = 0; i < unnamedFiles.Count; i++)
                builder.AddFile(MpqFile.New(new MemoryStream(unnamedFiles[i]), $"__unnamed{i}__.bin"));
            var listfile = string.Join("\r\n", files.Keys) + "\r\n";
            builder.AddFile(MpqFile.New(new MemoryStream(Encoding.ASCII.GetBytes(listfile)), "(listfile)"));

            var options = new MpqArchiveCreateOptions { ListFileCreateMode = MpqFileCreateMode.None };
            builder.SaveTo(mpq, options, leaveOpen: true);
        }

        using var outStream = new MemoryStream();
        var header = new byte[0x200];
        header[0] = (byte)'H'; header[1] = (byte)'M'; header[2] = (byte)'3'; header[3] = (byte)'W';
        outStream.Write(header, 0, header.Length);
        mpq.Position = 0;
        mpq.CopyTo(outStream);
        return outStream.ToArray();
    }

    /// <summary>
    /// Builds a map whose (listfile) advertises only <paramref name="visibleFiles"/>, while
    /// <paramref name="hiddenFiles"/> are added to the archive under their real, standard
    /// names but left out of the listfile entirely. This is the shape a real protected map
    /// presents, the hidden files are present in the hash table and openable by that real
    /// name, but load with FileName == null until something probes for that name, unlike
    /// <see cref="Build(IReadOnlyDictionary{string, byte[]}, IReadOnlyList{byte[]})"/>'s
    /// unnamedFiles, whose real archive name is a synthetic placeholder, not a name worth
    /// probing for.
    /// </summary>
    public static byte[] BuildProtected(
        IReadOnlyDictionary<string, byte[]> visibleFiles,
        IReadOnlyDictionary<string, byte[]> hiddenFiles)
    {
        var builder = new MpqArchiveBuilder();
        foreach (var (name, data) in visibleFiles)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));
        foreach (var (name, data) in hiddenFiles)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));

        var listfile = string.Join("\r\n", visibleFiles.Keys) + "\r\n";
        builder.AddFile(MpqFile.New(new MemoryStream(Encoding.ASCII.GetBytes(listfile)), "(listfile)"));

        using var mpq = new MemoryStream();
        var options = new MpqArchiveCreateOptions { ListFileCreateMode = MpqFileCreateMode.None };
        builder.SaveTo(mpq, options, leaveOpen: true);

        using var outStream = new MemoryStream();
        var header = new byte[0x200];
        header[0] = (byte)'H'; header[1] = (byte)'M'; header[2] = (byte)'3'; header[3] = (byte)'W';
        outStream.Write(header, 0, header.Length);
        mpq.Position = 0;
        mpq.CopyTo(outStream);
        return outStream.ToArray();
    }
}
