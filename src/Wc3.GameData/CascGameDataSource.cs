using CascLib.NET;
namespace Wc3.GameData;

public sealed class CascGameDataSource : IGameDataSource
{
    private readonly CascStorage _storage;
    private CascGameDataSource(CascStorage storage) => _storage = storage;

    public static bool TryOpen(string installDir, out CascGameDataSource? source, out string? error)
    {
        source = null; error = null;
        try { source = new CascGameDataSource(new CascStorage(installDir)); return true; }
        catch (Exception ex) { error = ex.Message; return false; }
    }

    /// <summary>
    /// Every file name the storage's root/listfile knows, as full storage paths including
    /// the w3mod-layer prefixes (e.g. "war3.w3mod:doodads\...\ruins_flower0.mdx"). This is
    /// the discovery tool for "where does this asset actually live?" — names come from the
    /// listfile, so on a partial install an enumerated name may still fail to open.
    /// </summary>
    public IEnumerable<string> EnumerateFileNames()
    {
        foreach (var entry in _storage)
            if (!string.IsNullOrEmpty(entry.FileName))
                yield return entry.FileName;
    }

    public byte[]? ReadFile(string name)
    {
        if (!_storage.TryOpenFile(name, out var stream) || stream is null) return null;
        using (stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public void Dispose() => _storage.Dispose();
}
