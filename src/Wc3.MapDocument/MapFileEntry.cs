// src/Wc3.MapDocument/MapFileEntry.cs
namespace Wc3.Model;

public sealed class MapFileEntry
{
    // FileName/IsKnown are settable, not init-only: MapDocument.HarvestAssetNames names an
    // entry after construction, when a second-pass probe resolves a name Load could not.
    public required string? FileName { get; set; }     // null => unnamed (protected map)
    public required int BlockIndex { get; init; }

    private byte[]? _rawBytes;
    private Lazy<byte[]>? _deferred;
    private int _deferredSize;
    private Func<int, byte[]>? _prefixReader;

    /// <summary>Cached verdict for <see cref="ContentTypeSniffer.Sniff(MapFileEntry)"/>, so
    /// listing the same map twice sniffs each entry once. Only the original archive bytes get
    /// cached, a pending replacement is sniffed fresh because it can change again.</summary>
    internal SniffedContentType? SniffCache;

    /// <summary>True when <see cref="MapDocument.HarvestAssetNames"/> recovered this entry's
    /// name from the map's own script or object data. False for a name the archive's listfile
    /// carried, or for a standard name Load probed. Listings surface this so a caller knows
    /// which names are recovered guesses that happened to resolve, rather than stored names.</summary>
    public bool NameFromHarvest { get; internal set; }

    /// <summary>
    /// The entry's original DECOMPRESSED bytes. Decompression is deferred until first
    /// access, because measurement on a 240 MB map put 2.7 of Load's 3.3 seconds in
    /// decompressing all 7914 entries (339 MB, mostly models and music) that most
    /// callers never touch. Load parses the known formats eagerly, so those entries
    /// materialize during Load exactly as before, everything else materializes on
    /// first read. Fidelity is unaffected, Save rebuilds untouched entries from the
    /// original archive bytes and never reads this property for them.
    /// </summary>
    public byte[] RawBytes
    {
        get => _rawBytes ?? _deferred?.Value ?? Array.Empty<byte>();
        init => _rawBytes = value;
    }

    /// <summary>
    /// The size <see cref="RawBytes"/> has or will have, answered without triggering
    /// decompression (the deferred size comes from the archive's block table). Callers
    /// that sweep every entry for a size listing should use this, an all-entries sweep
    /// of <c>RawBytes.Length</c> silently pays the full decompression Load deferred.
    /// </summary>
    public int RawSize => _rawBytes?.Length
        ?? (_deferred is { IsValueCreated: true } d ? d.Value.Length : _deferredSize);

    /// <summary>True once the raw bytes exist in memory. Test hook for the laziness
    /// contract, production code should never need to ask.</summary>
    internal bool IsMaterialized => _rawBytes is not null || _deferred is { IsValueCreated: true };

    /// <summary>Arms deferred decompression. <paramref name="read"/> must be idempotent
    /// and safe to call from any thread (Lazy with ExecutionAndPublication runs it once).
    /// <paramref name="size"/> is the block table's decompressed size, served by
    /// <see cref="RawSize"/> until the bytes actually materialize.</summary>
    internal void DeferRawBytes(Lazy<byte[]> read, int size)
    {
        _deferred = read;
        _deferredSize = size;
    }

    /// <summary>Arms cheap prefix reads for a deferred entry (see <see cref="ReadPrefix"/>).
    /// <paramref name="readPrefix"/> takes a byte count and returns at most that many leading
    /// bytes of the entry, decompressing only the sectors it touches.</summary>
    internal void DeferPrefixRead(Func<int, byte[]> readPrefix) => _prefixReader = readPrefix;

    /// <summary>
    /// The first <paramref name="count"/> bytes of <see cref="CurrentBytes"/>, without
    /// materializing the rest. Bytes already in memory (a pending replacement, an eager or
    /// already-deferred read) are sliced. A still-deferred entry goes through the prefix
    /// reader Load armed, which decompresses only the entry's leading sectors, so sweeping
    /// every entry for a content sniff stays cheap on a 250 MB archive. The full-decompression
    /// fallback at the bottom only runs for entries no reader was armed for, which are
    /// placeholder entries whose <see cref="RawBytes"/> is already an empty array.
    /// </summary>
    public byte[] ReadPrefix(int count)
    {
        if (count <= 0) return Array.Empty<byte>();
        if (OverrideBytes is { } pending) return Slice(pending, count);
        if (_rawBytes is { } eager) return Slice(eager, count);
        if (_deferred is { IsValueCreated: true } done) return Slice(done.Value, count);
        if (_prefixReader is { } read) return read(count);
        return Slice(RawBytes, count);

        static byte[] Slice(byte[] all, int wanted) => all.Length <= wanted ? all : all[..wanted];
    }

    public required bool IsKnown { get; set; }
    public object? Model { get; set; }

    /// <summary>
    /// Bytes past the end of what this file's parser consumed, preserved so that rebuilding the
    /// file from its model does not truncate it.
    ///
    /// Empty for almost every file. Not empty for the ones that matter: an object table ending in
    /// a trailing int32 zero that War3Net's writer omits, or a war3mapUnits.doo ending in a stray
    /// newline. Without this, editing one unit in such a map shortens the file by four bytes that
    /// nobody asked to remove. See MapFormatRegistry.ParsedModel.
    /// </summary>
    public byte[] UnreadTail { get; set; } = Array.Empty<byte>();
    public bool IsParsed => Model is not null;
    public bool IsDirty { get; set; }

    /// <summary>
    /// The locale field of the hash-table slot this entry was reached through. An MPQ addresses a
    /// file by the hash of its name AND this value, so one archive can hold several entries that
    /// all report the same <see cref="FileName"/>, and each has to be rewritten into its own slot.
    /// </summary>
    /// <remarks>
    /// This is not really about languages. In the maps that need it, the duplicate slot carries
    /// 0xFF000000 rather than any real locale, which is a protection trick. Naruto Autobattle
    /// ENGv3ch11.w3x has nine such pairs, and in three of them the two copies are different sizes,
    /// so they are genuinely different payloads rather than one file listed twice.
    ///
    /// Save writes an edited entry back under this exact value. Writing it under Neutral instead
    /// left the other slot holding the original bytes, and the name then resolved to that stale
    /// copy, which is how an edit could be reported as applied and not be there on reload.
    /// Defaults to 0 (Neutral), which is what a newly added file gets.
    /// </remarks>
    public uint Locale { get; init; }

    /// <summary>
    /// A raw byte payload that, when set, is written verbatim on the next Save
    /// (taking precedence over any <see cref="Model"/> serialization). Used to add
    /// imported assets, replace the script text, or inject a file whose format has
    /// no byte-faithful model writer. <see cref="RawBytes"/> stays the immutable
    /// original bytes; this is the pending replacement.
    /// </summary>
    public byte[]? OverrideBytes { get; set; }

    /// <summary>
    /// A name recovered for an entry the archive lists without one, set by a deprotect pass.
    /// </summary>
    /// <remarks>
    /// <see cref="FileName"/> stays null, because it records what the archive's own listfile
    /// said and that is a fact about the file rather than about our analysis. Save re-adds an
    /// entry carrying this under the recovered name, which is the only way the name survives.
    /// Writing a <c>(listfile)</c> as a normal file does NOT work, because MpqArchiveBuilder
    /// regenerates that from the names it knows and discards the supplied one. That was tried
    /// first and reported 58 named entries before and after, a clean silent no-op.
    /// </remarks>
    public string? RecoveredFileName { get; set; }

    /// <summary>
    /// What this entry's bytes are NOW, the pending replacement if there is one and the
    /// original bytes otherwise. This is what Save will write, so it is what a reader that
    /// asks "what does this file contain" almost always means.
    /// </summary>
    /// <remarks>
    /// This exists because the choice was being made 67 times by hand. 39 sites wrote
    /// <c>OverrideBytes ?? RawBytes</c> inline, one of them behind a private helper of its
    /// own, and 28 read <see cref="RawBytes"/> bare. The bare reads are not a style
    /// difference, they are a stale read, and several were user-visible. The Files panel
    /// previewed and hex-dumped the pre-edit content of any file another panel had just
    /// changed. Lint asked "does the script compile" of the script on disk rather than the
    /// script about to be saved, so a broken edit passed. Extract wrote the old bytes.
    ///
    /// Reach for <see cref="RawBytes"/> only when the ORIGINAL bytes are the point, which
    /// means round-trip fidelity checks and nothing else. Every other reader wants this.
    /// </remarks>
    public byte[] CurrentBytes => OverrideBytes ?? RawBytes;

    /// <summary>
    /// The size <see cref="CurrentBytes"/> has, answered without triggering decompression
    /// when nothing has replaced the entry. The deferred-decompression counterpart of
    /// <see cref="RawSize"/> for callers that sweep every entry.
    /// </summary>
    public int CurrentSize => OverrideBytes?.Length ?? RawSize;
}
