using CascLib.NET;
namespace Wc3.GameData;

/// <summary>
/// The one lock that guards every CASC storage open in the process. The native
/// CascLib open is not safe to run concurrently, measured at 2026-10-13 with
/// 32 simultaneous opens of the same install, where one threw
/// "The system cannot find the file specified" and a concurrent
/// <see cref="GameData.TryOpen"/> build failed on 24 to 30 of 32 threads with
/// "unitmetadata.slk not found in game data". Serializing the open makes a cold
/// cache safe under the parallel test run. Ongoing reads on an already-open
/// handle are outside this gate.
/// </summary>
internal static class CascGate
{
    internal static readonly object Sync = new();
}

public sealed class CascGameDataSource : IGameDataSource
{
    private readonly CascStorage _storage;
    private CascGameDataSource(CascStorage storage) => _storage = storage;

    public static bool TryOpen(string installDir, out CascGameDataSource? source, out string? error)
    {
        source = null; error = null;
        try
        {
            // The native open races itself process wide, so one open at a time.
            CascGameDataSource? opened;
            lock (CascGate.Sync)
                opened = new CascGameDataSource(new CascStorage(installDir));
            source = opened;
            return true;
        }
        catch (Exception ex) { error = ex.Message; return false; }
    }

    /// <summary>
    /// Every file name the storage's root/listfile knows, as full storage paths including
    /// the w3mod-layer prefixes (e.g. "war3.w3mod:doodads\...\ruins_flower0.mdx"). This is
    /// the discovery tool for "where does this asset actually live?", names come from the
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
