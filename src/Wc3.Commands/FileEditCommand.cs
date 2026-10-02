// src/Wc3.Commands/FileEditCommand.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Outcome of an add/replace (or text write). <see cref="Replaced"/> is true when
/// an existing entry's payload was superseded, false when a new entry was added.
/// <see cref="SizeBytes"/> is the size of the payload now pending for the next Save.</summary>
public sealed record FileEditResult(string Name, bool Replaced, int SizeBytes);

/// <summary>Current state of one archive entry. <see cref="SizeBytes"/> reflects the pending
/// in-memory replacement when one exists (<c>OverrideBytes</c>), never the stale original.
/// <see cref="Name"/> is null for unnamed entries (protected maps).</summary>
public sealed record FileState(string? Name, int SizeBytes, bool IsDirty, bool HasOverride);

/// <summary>
/// Raw and text file editing on a <see cref="MapDocument"/>, byte-faithful by construction:
/// writes go through <see cref="MapDocument.AddOrReplaceRawFile"/> (which stages the payload
/// as <c>OverrideBytes</c> and marks the entry dirty; <c>RawBytes</c> stays the immutable
/// original), and every read helper resolves <c>OverrideBytes ?? RawBytes</c> so a read
/// always reflects a prior write in the same session.
///
/// Contracts:
/// <list type="bullet">
/// <item><see cref="AddOrReplace"/> with an existing name REPLACES the payload (mirroring
/// <c>MapDocument.AddOrReplaceRawFile</c>); the result reports which case occurred.</item>
/// <item>Hard-removing an entry is not supported by the model — see <see cref="Remove"/>.</item>
/// <item>Text: default encoding is UTF-8. <see cref="WriteText"/> never emits a BOM and
/// <see cref="ReadText"/> strips a leading BOM, so WriteText→ReadText is the identity and
/// reading a BOM'd original yields clean text.</item>
/// </list>
/// </summary>
public static class FileEditCommand
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// Adds a new file with the given raw bytes, or replaces an existing file's payload
    /// verbatim. Thin validating wrapper over <see cref="MapDocument.AddOrReplaceRawFile"/>:
    /// the bytes are staged as <c>OverrideBytes</c> and written unchanged on the next Save.
    /// </summary>
    public static FileEditResult AddOrReplace(MapDocument doc, string name, byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(bytes);
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("File name must be non-empty.", nameof(name));

        bool replaced = doc.GetFile(name) is not null;
        doc.AddOrReplaceRawFile(name, bytes);
        return new FileEditResult(name, replaced, bytes.Length);
    }

    /// <summary>
    /// Hard-removing a file is NOT supported by the model, so this fails loudly.
    /// <c>MapDocument</c> exposes no way to delete an entry (<c>Files</c> is read-only), and
    /// <c>SaveToBytes</c> rebuilds from the original archive by only ADDING dirty entries —
    /// removal is deliberately unwired because <c>MpqArchiveBuilder.RemoveFile</c> +
    /// <c>AddFile</c> of the same name silently drops the file. The nearest supported
    /// operation is <see cref="AddOrReplace"/> with an empty payload, which blanks the
    /// content while keeping the entry.
    /// </summary>
    /// <exception cref="NotSupportedException">Always.</exception>
    public static void Remove(MapDocument doc, string name) =>
        throw new NotSupportedException(
            $"Cannot remove '{name}': MapDocument has no removal API and its save pipeline " +
            "only adds/shadows dirty entries when rebuilding the original archive. " +
            "Use AddOrReplace with an empty payload to blank the content instead.");

    /// <summary>Current bytes of a file — the pending in-memory replacement when one exists,
    /// else the original bytes. Returns a defensive copy so callers can't mutate the
    /// document's buffers (RawBytes is the immutable original).</summary>
    public static byte[] ReadBytes(MapDocument doc, string name)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var bytes = CurrentBytes(RequireEntry(doc, name));
        return bytes.ToArray();
    }

    /// <summary>Current state (size, dirtiness, pending override) of a single file,
    /// reflecting any in-session replacement.</summary>
    public static FileState Stat(MapDocument doc, string name)
    {
        ArgumentNullException.ThrowIfNull(doc);
        return ToState(RequireEntry(doc, name));
    }

    /// <summary>Lists every archive entry with its CURRENT size — unlike the raw listing,
    /// a replaced file reports the pending payload's size, not the stale original's.</summary>
    public static IReadOnlyList<FileState> ListFiles(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        return doc.Files.Select(ToState).ToList();
    }

    /// <summary>
    /// Decodes a file as text (default UTF-8), reflecting any prior
    /// <see cref="WriteText"/>/<see cref="AddOrReplace"/> in the same session. A leading
    /// BOM (U+FEFF) is stripped after decoding, whatever the encoding.
    /// </summary>
    public static string ReadText(MapDocument doc, string name, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var text = (encoding ?? Utf8NoBom).GetString(CurrentBytes(RequireEntry(doc, name)));
        return text.Length > 0 && text[0] == '\uFEFF' ? text[1..] : text;
    }

    /// <summary>
    /// Encodes text (default UTF-8, no BOM emitted) and stages it via
    /// <see cref="AddOrReplace"/>. A subsequent <see cref="ReadText"/> with the same
    /// encoding returns exactly <paramref name="text"/>.
    /// </summary>
    public static FileEditResult WriteText(MapDocument doc, string name, string text, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        return AddOrReplace(doc, name, (encoding ?? Utf8NoBom).GetBytes(text));
    }

    private static MapFileEntry RequireEntry(MapDocument doc, string name) =>
        doc.GetFile(name) ?? throw new FileNotFoundException($"'{name}' is not in the map", name);

    /// <summary>The one rule that keeps reads honest: a pending replacement
    /// (OverrideBytes) wins over the immutable original (RawBytes).</summary>
    private static byte[] CurrentBytes(MapFileEntry entry) => entry.OverrideBytes ?? entry.RawBytes;

    private static FileState ToState(MapFileEntry f) =>
        new(f.FileName, CurrentBytes(f).Length, f.IsDirty, f.OverrideBytes is not null);
}
