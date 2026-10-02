// src/Wc3.MapDocument/MapFileEntry.cs
namespace Wc3.Model;

public sealed class MapFileEntry
{
    public required string? FileName { get; init; }   // null => unnamed (protected map)
    public required int BlockIndex { get; init; }
    public required byte[] RawBytes { get; init; }     // original DECOMPRESSED bytes
    public required bool IsKnown { get; init; }
    public object? Model { get; set; }
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
}
