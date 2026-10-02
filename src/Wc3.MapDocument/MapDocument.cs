// src/Wc3.MapDocument/MapDocument.cs
using War3Net.Build.Audio;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;
using War3Net.Build.Import;
using War3Net.Build.Object;
using War3Net.Build.Widget;
using War3Net.IO.Mpq;

namespace Wc3.Model;

public sealed class MapDocument
{
    private readonly byte[] _originalBytes;
    private readonly List<MapFileEntry> _files = new();
    private readonly List<Diagnostic> _diagnostics = new();

    public IReadOnlyList<MapFileEntry> Files => _files;
    public byte[] PreArchiveData { get; private set; } = Array.Empty<byte>();
    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    static MapDocument() => DefaultParsers.RegisterDefaults();

    private MapDocument(byte[] originalBytes) => _originalBytes = originalBytes;

    public static MapDocument Load(string path) => Load(File.ReadAllBytes(path));

    public static MapDocument Load(byte[] fileBytes)
    {
        var doc = new MapDocument(fileBytes);

        int offset = MpqHeader.FindArchiveOffset(fileBytes);
        if (offset < 0)
            throw new InvalidDataException("No MPQ archive magic found in file.");
        doc.PreArchiveData = fileBytes[..offset];

        using var stream = new MemoryStream(fileBytes);
        using var archive = MpqArchive.Open(stream, loadListFile: true);

        // The locale of the hash slot each block is reached through. Needed because a protected
        // map can point two slots at two different blocks under one name, and an edit written
        // under the wrong locale lands in a slot the name does not resolve to.
        // Deliberately NOT filtered on IsAvailable. The slot carrying the odd locale in a
        // protected map does not report as available, and filtering it out made every entry read
        // back as Neutral, which is exactly the bug this exists to fix. Empty and deleted slots
        // hold a block index of 0xFFFFFFFF or 0xFFFFFFFE, which never matches a real block, so
        // letting them through costs nothing.
        var localeByBlock = new Dictionary<uint, uint>();
        foreach (var hash in archive.EnumerateHashes())
            localeByBlock.TryAdd(hash.BlockIndex, (uint)hash.Locale);

        int block = 0;
        foreach (var entry in archive)
        {
            string? name = entry.FileName;
            uint locale = localeByBlock.TryGetValue((uint)block, out var l) ? l : 0u;
            try
            {
                byte[] raw;
                using (var fs = archive.OpenFile(entry))
                {
                    using var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    raw = ms.ToArray();
                }

                bool known = name is not null && MapFormatRegistry.IsKnown(name);
                doc._files.Add(new MapFileEntry
                {
                    FileName = name,
                    BlockIndex = block++,
                    RawBytes = raw,
                    IsKnown = known,
                    Locale = locale,
                });
            }
            catch (Exception ex)
            {
                // Unreadable entry (encrypted/unnamed/corrupt): keep a placeholder
                // instead of aborting Load. The real bytes are preserved because
                // Save rebuilds via MpqArchiveBuilder(originalArchive).
                doc._diagnostics.Add(new Diagnostic(DiagnosticSeverity.Warning, name ?? "(unnamed)",
                    $"Could not read file data, preserved via archive rebuild: {ex.Message}"));
                doc._files.Add(new MapFileEntry
                {
                    FileName = name,
                    BlockIndex = block++,
                    RawBytes = Array.Empty<byte>(),
                    IsKnown = false,
                    Locale = locale,
                });
            }
        }

        doc.RecoverInternalNames();
        doc.ParseKnownFiles();
        return doc;
    }

    // The fixed internal names every map has. Nothing else can be guessed, but these never vary.
    private static readonly string[] InternalNames =
    {
        // BOTH script paths. Reforged moved the script under scripts\, and a map may use either.
        // BleachVsOnepiece13 holds a 4.4 MB script at scripts\war3map.j and nothing at
        // war3map.j, so omitting the second spelling made the audit report 166 orphan abilities
        // that are all implemented, purely because the script it searched was empty.
        "war3map.w3i", "war3map.j", "war3map.lua",
        @"scripts\war3map.j", @"scripts\war3map.lua",
        "war3map.w3e", "war3map.wpm", "war3map.doo",
        "war3mapUnits.doo", "war3map.w3r", "war3map.w3c", "war3map.w3s", "war3map.w3u",
        "war3map.w3t", "war3map.w3a", "war3map.w3b", "war3map.w3d", "war3map.w3h", "war3map.w3q",
        "war3map.wts", "war3map.wtg", "war3map.wct", "war3map.shd", "war3map.imp",
        "war3mapSkin.txt", "war3mapMisc.txt", "war3mapExtra.txt", "war3mapMap.blp",
        "war3map.mmp", "war3map.w3o", "war3map.w3v",
    };

    /// <summary>
    /// Names the standard <c>war3map.*</c> entries a protected map stripped from its listfile.
    ///
    /// <para>Without this the toolkit reads a heavily protected map as EMPTY and says so
    /// cleanly. Measured on one real map, BleachVsOnepiece13 names 2 of its 851 entries, so
    /// every known file lookup missed, nothing was parsed, and the audit reported "OK, 0
    /// abilities checked, 0 errors" for a map holding 897 abilities. A clean pass on a map that
    /// was never read is the most dangerous result this toolkit can produce.</para>
    ///
    /// <para>Only the fixed names are recovered here. Imported assets need the full recovery in
    /// <see cref="RecoverNames"/>, which takes a dictionary.</para>
    /// </summary>
    private void RecoverInternalNames()
    {
        var missing = InternalNames
            .Where(n => _files.All(f => !string.Equals(f.FileName, n, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        if (missing.Count == 0) return;

        var byBlock = _files.ToDictionary(f => f.BlockIndex);
        using var stream = new MemoryStream(_originalBytes);
        using var archive = MpqArchive.Open(stream, loadListFile: false);
        var blockByName = new Dictionary<ulong, uint>();
        foreach (var hash in archive.EnumerateHashes())
            blockByName.TryAdd(hash.Name, hash.BlockIndex);

        int found = 0;
        foreach (var name in missing)
        {
            if (!blockByName.TryGetValue(MpqHash.GetHashedFileName(name), out var blk)) continue;
            if (!byBlock.TryGetValue((int)blk, out var entry)) continue;
            if (entry.FileName is not null || entry.RecoveredFileName is not null) continue;
            entry.RecoveredFileName = name;
            found++;
        }
        if (found > 0)
            _diagnostics.Add(new Diagnostic(DiagnosticSeverity.Info, "(listfile)",
                $"Listfile is stripped or incomplete, recovered {found} standard war3map.* "
                + "name(s) by hash so the map could be read at all."));
    }

    private void ParseKnownFiles()
    {
        foreach (var entry in _files)
        {
            // A recovered name counts. On a protected map the listfile names almost nothing, and
            // gating on IsKnown alone left every war3map.* file unparsed while reporting success.
            string? name = entry.FileName ?? entry.RecoveredFileName;
            if (name is null) continue;
            if (entry.FileName is not null && !entry.IsKnown) continue;
            if (!MapFormatRegistry.TryGetParser(name, out var parse))
                continue;
            try
            {
                entry.Model = parse(entry.RawBytes);
            }
            catch (Exception ex)
            {
                _diagnostics.Add(new Diagnostic(DiagnosticSeverity.Warning, name,
                    $"Parse failed, preserved as raw: {ex.Message}"));
            }
        }
    }

    public MapFileEntry? GetFile(string fileName) =>
        _files.FirstOrDefault(f => string.Equals(f.FileName, fileName, StringComparison.OrdinalIgnoreCase))
        ?? _files.FirstOrDefault(f => string.Equals(f.RecoveredFileName, fileName, StringComparison.OrdinalIgnoreCase));

    // Bytes resolved by MPQ name hash, cached per name. null means the archive does not hold it.
    private readonly Dictionary<string, byte[]?> _byNameCache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Reads a file by name through the archive's HASH table rather than through the listfile.
    ///
    /// <para><see cref="GetFile"/> matches on <see cref="MapFileEntry.FileName"/>, which comes
    /// from the listfile, and a protected map has no listfile. Measured on one real map, 793 of
    /// its 851 entries carry no name at all, so <c>GetFile("Robin.mdl")</c> returns null for a
    /// file the archive plainly contains. That is not an error and reads exactly like absence,
    /// which is how an audit of imported models reported 3 affected units out of 113.</para>
    ///
    /// <para>MPQ addresses a file by the hash of its name, so a lookup by name works with no
    /// listfile whatsoever. Prefer this over <see cref="GetFile"/> for any file the MAP names
    /// (a model path, an icon, an imported asset), and keep <see cref="GetFile"/> for the known
    /// <c>war3map.*</c> entries, which are always named.</para>
    ///
    /// <para>It reads the ORIGINAL archive, so a pending <see cref="MapFileEntry.OverrideBytes"/>
    /// replacement is checked first and wins, matching what a later Save will write.</para>
    /// </summary>
    public bool TryReadFileByName(string fileName, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        if (string.IsNullOrWhiteSpace(fileName)) return false;

        var named = GetFile(fileName);
        if (named is not null)
        {
            bytes = named.OverrideBytes ?? named.RawBytes;
            return bytes.Length > 0;
        }

        if (!_byNameCache.TryGetValue(fileName, out var cached))
        {
            cached = null;
            try
            {
                using var source = new MemoryStream(_originalBytes);
                using var archive = MpqArchive.Open(source, loadListFile: false);
                using var fs = archive.OpenFile(fileName);
                using var ms = new MemoryStream();
                fs.CopyTo(ms);
                cached = ms.ToArray();
            }
            catch (Exception)
            {
                // Not present, or unreadable. Both are "the archive cannot give you this name",
                // and neither is worth a diagnostic, because callers probe several candidate
                // spellings of a model path and most probes are expected to miss.
                cached = null;
            }
            _byNameCache[fileName] = cached;
        }

        if (cached is null) return false;
        bytes = cached;
        return bytes.Length > 0;
    }

    // Hashed names of every entry in the ORIGINAL archive, built on first use.
    private HashSet<ulong>? _hashedNames;

    /// <summary>
    /// Whether the archive holds a file under this name, answered from the hash table alone.
    ///
    /// <para><see cref="TryReadFileByName"/> answers the same question by DECOMPRESSING the file,
    /// which is the wrong price for an existence test. Anime WOS2 0.32d references 3,740 model
    /// paths against 1,116 imported models in a 285 MB archive, and a reference check probes
    /// several spellings of each. This builds one set of hashed names and never reads a byte of
    /// payload. Names are matched the way the engine looks them up, case-insensitive and with a
    /// forward slash treated as a backslash. A file added since load counts as present.</para>
    /// </summary>
    public bool HasFileByName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;
        string name = fileName.Replace('/', '\\');
        if (GetFile(name) is not null) return true;
        if (!System.Text.Ascii.IsValid(name)) return false;
        if (_hashedNames is null)
        {
            _hashedNames = new HashSet<ulong>();
            using var stream = new MemoryStream(_originalBytes);
            using var archive = MpqArchive.Open(stream, loadListFile: false);
            // 0xFFFFFFFF marks an empty slot and 0xFFFFFFFE a deleted one, neither is a file.
            foreach (var hash in archive.EnumerateHashes())
                if (hash.BlockIndex < 0xFFFFFFFE) _hashedNames.Add(hash.Name);
        }
        return _hashedNames.Contains(MpqHash.GetHashedFileName(name));
    }

    /// <summary>
    /// Matches candidate file names against the archive's hash table and returns the block index
    /// of every one that hits. This is name recovery, the readable half of deprotection.
    ///
    /// <para>A protected map ships no <c>(listfile)</c>, so most entries have no
    /// <see cref="MapFileEntry.FileName"/>. Worse, most blocks are ENCRYPTED and an MPQ block's
    /// key is derived from its own basename, so such a block cannot be read, or even identified,
    /// without its name. Measured on one real map, 833 of 851 blocks are encrypted. No scan of
    /// the bytes can ever recover those, which is why a name dictionary is the only route.</para>
    ///
    /// <para>The caller supplies the dictionary. Useful sources are the map's own object data and
    /// script literals, a community listfile, and the listfiles of sibling maps.</para>
    /// </summary>
    /// <returns>Block index to the name that resolved it, for blocks that had no name before.</returns>
    public IReadOnlyDictionary<int, string> RecoverNames(IEnumerable<string> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        var already = _files.Where(f => f.FileName is not null)
            .Select(f => f.BlockIndex).ToHashSet();

        using var stream = new MemoryStream(_originalBytes);
        using var archive = MpqArchive.Open(stream, loadListFile: false);

        var blockByName = new Dictionary<ulong, uint>();
        foreach (var hash in archive.EnumerateHashes())
            blockByName.TryAdd(hash.Name, hash.BlockIndex);

        var found = new Dictionary<int, string>();
        foreach (var candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate)) continue;
            // The hasher throws on any character at or above 0x200, and candidates harvested
            // from script literals and raw block bytes routinely contain one. A real MPQ path
            // is ASCII, so skipping the rest costs nothing and a throw here would abort the
            // whole recovery over a single stray byte.
            if (!System.Text.Ascii.IsValid(candidate)) continue;
            if (!blockByName.TryGetValue(MpqHash.GetHashedFileName(candidate), out var blk))
                continue;
            int index = (int)blk;
            if (already.Contains(index) || found.ContainsKey(index)) continue;
            found[index] = candidate;
        }
        return found;
    }

    /// <summary>
    /// Replaces the bytes of an entry resolved by MPQ name hash, which is the only way to edit
    /// a file in a protected map whose listfile was stripped.
    /// </summary>
    /// <remarks>
    /// <see cref="AddOrReplaceRawFile"/> matches on <see cref="MapFileEntry.FileName"/> and so
    /// cannot see a nameless entry at all. It would silently ADD a second entry under the same
    /// hashed name instead of replacing the payload, which reads as success. This resolves the
    /// real entry through the hash table and stamps the name on it so Save writes it back under
    /// that name. Returns false when the archive does not hold the name.
    /// </remarks>
    public bool TryReplaceFileByName(string fileName, byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (string.IsNullOrWhiteSpace(fileName)) return false;
        if (!System.Text.Ascii.IsValid(fileName)) return false;

        var named = GetFile(fileName);
        if (named is not null)
        {
            named.OverrideBytes = bytes;
            named.Model = null;
            named.IsDirty = true;
            _byNameCache.Remove(fileName);
            return true;
        }

        using var stream = new MemoryStream(_originalBytes);
        using var archive = MpqArchive.Open(stream, loadListFile: false);
        ulong key = MpqHash.GetHashedFileName(fileName);
        foreach (var hash in archive.EnumerateHashes())
        {
            if (hash.Name != key) continue;
            var entry = _files.FirstOrDefault(f => f.BlockIndex == (int)hash.BlockIndex);
            if (entry is null) continue;
            entry.OverrideBytes = bytes;
            entry.Model = null;
            entry.IsDirty = true;
            entry.RecoveredFileName ??= fileName;
            _byNameCache.Remove(fileName);
            return true;
        }
        return false;
    }

    /// <summary>Block index to payload size, for every entry with no name.</summary>
    public IReadOnlyDictionary<int, int> UnnamedBlocks() =>
        _files.Where(f => f.FileName is null)
            .ToDictionary(f => f.BlockIndex, f => f.RawBytes.Length);

    /// <summary>
    /// What an entry actually is, read from its own bytes rather than from its name.
    /// </summary>
    /// <remarks>
    /// Useful precisely where the name is missing. The four byte magic identifies every asset
    /// type a map carries, and an MDX additionally states its own model name in its MODL chunk,
    /// so a nameless model can still say what it is called.
    /// </remarks>
    public static (string Kind, string SelfName) IdentifyBytes(ReadOnlySpan<byte> data)
    {
        if (data.Length < 4) return ("empty", "");
        if (data[..4].SequenceEqual("MDLX"u8)) return ("MDX model", MdxModelName(data));
        if (data[..4].SequenceEqual("BLP1"u8) || data[..4].SequenceEqual("BLP2"u8))
            return ("BLP texture", "");
        if (data[..4].SequenceEqual("RIFF"u8)) return ("WAV audio", "");
        if (data[..4].SequenceEqual("DDS "u8)) return ("DDS texture", "");
        if (data.Length >= 3 && data[0] == 'I' && data[1] == 'D' && data[2] == '3')
            return ("MP3 audio", "");
        if (data[0] == 'B' && data[1] == 'M') return ("BMP image", "");
        bool text = true;
        for (int i = 0; i < Math.Min(data.Length, 120); i++)
            if (data[i] is not ((>= 0x20 and < 0x7f) or (byte)'\r' or (byte)'\n' or (byte)'\t'))
            {
                text = false;
                break;
            }
        return (text ? "text" : "unknown", "");
    }

    private static string MdxModelName(ReadOnlySpan<byte> data)
    {
        int p = 4;
        while (p + 8 <= data.Length)
        {
            var tag = data.Slice(p, 4);
            uint size = BitConverter.ToUInt32(data.Slice(p + 4, 4));
            if (tag.SequenceEqual("MODL"u8) && p + 8 + 0x50 <= data.Length)
            {
                var body = data.Slice(p + 8, 0x50);
                int end = body.IndexOf((byte)0);
                return System.Text.Encoding.Latin1.GetString(end < 0 ? body : body[..end]);
            }
            if (size == 0) break;
            p += 8 + (int)size;
        }
        return "";
    }

    public void Save(string path) => File.WriteAllBytes(path, SaveToBytes());

    public byte[] SaveToBytes()
    {
        using var source = new MemoryStream(_originalBytes);
        using var archive = MpqArchive.Open(source, loadListFile: true);
        var builder = new MpqArchiveBuilder(archive);

        // An added file with the same hashed name shadows the original at save. RemoveFile must
        // NOT be called first, because its removal set also filters the replacement, dropping the
        // file from the archive entirely.
        //
        // The hash covers the locale as well as the name, so the entry's own locale has to go with
        // it. A protected map can hold two slots for one name, one Neutral and one carrying
        // 0xFF000000, and adding under Neutral alone shadowed only one of them. The other kept its
        // original bytes, the name resolved to that stale copy, and the edit vanished on reload
        // while every command still reported it applied.
        foreach (var entry in _files.Where(f => f.IsDirty && f.FileName is not null))
            builder.AddFile(MpqFile.New(
                new MemoryStream(SerializeEntry(entry)), entry.FileName!, (MpqLocale)entry.Locale));

        // Entries whose name was recovered by a deprotect pass. Re-added under that name so the
        // builder's regenerated listfile carries it, which is the only route that works, since
        // a (listfile) supplied as an ordinary file is discarded.
        //
        // These bytes are re-added rather than passed through, so those entries are recompressed
        // and are NOT byte-faithful afterwards. That is the deliberate cost of naming them and
        // it applies only to entries the caller explicitly asked to name.
        foreach (var entry in _files.Where(f => f.FileName is null
                                               && f.RecoveredFileName is not null))
            builder.AddFile(MpqFile.New(
                new MemoryStream(entry.OverrideBytes ?? entry.RawBytes),
                entry.RecoveredFileName!, (MpqLocale)entry.Locale));

        using var mpq = new MemoryStream();
        // SaveTo disposes the target stream unless leaveOpen — we still need to read it back.
        builder.SaveTo(mpq, leaveOpen: true);

        using var outStream = new MemoryStream();
        outStream.Write(PreArchiveData, 0, PreArchiveData.Length);
        mpq.Position = 0;
        mpq.CopyTo(outStream);
        return outStream.ToArray();
    }

    /// <summary>
    /// Installs an edited model on <paramref name="entry"/> and marks it dirty so the
    /// next Save re-serializes it (non-dirty entries keep their original bytes).
    /// </summary>
    public void ReplaceModel(MapFileEntry entry, object newModel)
    {
        if (!_files.Contains(entry))
            throw new ArgumentException("Entry does not belong to this document.", nameof(entry));
        entry.Model = newModel;
        entry.OverrideBytes = null; // a fresh model supersedes any pending raw override
        entry.IsDirty = true;
    }

    /// <summary>
    /// Adds a file with the given raw bytes, or replaces an existing file's bytes
    /// verbatim (the payload is written unchanged on Save). Used for imported assets,
    /// script text, or any format without a byte-faithful model writer.
    /// </summary>
    /// <summary>
    /// Writes a raw payload under <paramref name="fileName"/>, adding the file when it is absent.
    /// </summary>
    /// <remarks>
    /// An MPQ addresses a file by the hash of its name AND its locale, so one archive can hold
    /// several entries that all report this same name. This used to resolve the name with GetFile,
    /// which is FirstOrDefault, attach the override to whichever entry loaded first, and leave the
    /// rest holding their original bytes. Save writes every entry, the name then resolves to one of
    /// the untouched copies, and the edit is gone while having been reported as applied.
    ///
    /// Measured on Naruto Autobattle ENGv3ch11.w3x, where six files are duplicated this way. The
    /// Reforged 3.0.0 repair reported 973 button positions completed, saved, and a second pass over
    /// its own output found the same 973 still malformed.
    ///
    /// So every entry sharing the name is written. Which one the game resolves to depends on its
    /// locale, which is not knowable here, and the only safe answer is that they all agree.
    /// </remarks>
    public MapFileEntry AddOrReplaceRawFile(string fileName, byte[] bytes)
    {
        var existing = _files
            .Where(f => string.Equals(f.FileName, fileName, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (existing.Count > 0)
        {
            foreach (var duplicate in existing)
            {
                duplicate.OverrideBytes = bytes;
                duplicate.Model = null; // raw payload wins; drop any stale parsed model
                duplicate.IsDirty = true;
            }
            return existing[0];
        }
        var entry = new MapFileEntry
        {
            FileName = fileName,
            BlockIndex = NextBlockIndex(),
            RawBytes = bytes,
            IsKnown = MapFormatRegistry.IsKnown(fileName),
            OverrideBytes = bytes,
            IsDirty = true,
        };
        _files.Add(entry);
        return entry;
    }

    /// <summary>
    /// Adds a file backed by a typed model, or replaces an existing file's model.
    /// Serialized on Save via <see cref="SerializeEntry"/> (only byte-faithful formats
    /// are supported — see that method). Used to inject/replace object-data files, etc.
    /// </summary>
    public MapFileEntry AddOrReplaceModelFile(string fileName, object model)
    {
        if (GetFile(fileName) is { } existing)
        {
            existing.Model = model;
            existing.OverrideBytes = null;
            existing.IsDirty = true;
            return existing;
        }
        var entry = new MapFileEntry
        {
            FileName = fileName,
            BlockIndex = NextBlockIndex(),
            RawBytes = Array.Empty<byte>(),
            IsKnown = MapFormatRegistry.IsKnown(fileName),
            Model = model,
            IsDirty = true,
        };
        _files.Add(entry);
        return entry;
    }

    private int NextBlockIndex() => _files.Count == 0 ? 0 : _files.Max(f => f.BlockIndex) + 1;

    // Serialization by model type. A raw OverrideBytes payload always wins; otherwise
    // the model's War3Net writer is used. Only formats with a byte-faithful writer are
    // handled — anything else must stay non-dirty (raw) so Save preserves original bytes.
    private static byte[] SerializeEntry(MapFileEntry entry)
    {
        // Raw override (added assets, replaced script text, non-faithful formats).
        if (entry.OverrideBytes is not null) return entry.OverrideBytes;

        // Typed models with a byte-faithful War3Net writer (all seven object-data
        // kinds share the ReadXxxObjectData/Write(xxx) round-trip; skin twins carry
        // the same model types, so dispatch on the model, not the file name).
        switch (entry.Model)
        {
            case UnitObjectData m: return WriteBinary(w => w.Write(m));
            case AbilityObjectData m: return WriteBinary(w => w.Write(m));
            case ItemObjectData m: return WriteBinary(w => w.Write(m));
            case DestructableObjectData m: return WriteBinary(w => w.Write(m));
            case DoodadObjectData m: return WriteBinary(w => w.Write(m));
            case BuffObjectData m: return WriteBinary(w => w.Write(m));
            case UpgradeObjectData m: return WriteBinary(w => w.Write(m));
            case ImportedFiles m: return WriteBinary(w => w.Write(m));
            // Widget placement files (war3mapUnits.doo / war3map.doo). The read path
            // (Parsers.cs) parses these into MapUnits/MapDoodads, so the write path must
            // match or a dirtied placement edit would hit the throw below. Confirmed via
            // reflection: BinaryWriterExtensions.Write(BinaryWriter, MapUnits/MapDoodads).
            case MapUnits m: return WriteBinary(w => w.Write(m));
            case MapDoodads m: return WriteBinary(w => w.Write(m));
            // Pathing map (war3map.wpm). Read via ReadMapPathingMap (Parsers.cs); write
            // confirmed via reflection: BinaryWriterExtensions.Write(BinaryWriter, MapPathingMap).
            case MapPathingMap m: return WriteBinary(w => w.Write(m));
            // Regions (war3map.w3r). Read via ReadMapRegions (Parsers.cs); write
            // confirmed via reflection: BinaryWriterExtensions.Write(BinaryWriter, MapRegions).
            case MapRegions m: return WriteBinary(w => w.Write(m));
            // Terrain environment (war3map.w3e). Read via ReadMapEnvironment (Parsers.cs);
            // byte-faithfulness of the War3Net writer is pinned by
            // MapWriteTests.Environment_writer_is_byte_faithful_on_real_map.
            case MapEnvironment m: return WriteBinary(w => w.Write(m));
            // Sound definitions (war3map.w3s). Read via ReadMapSounds (Parsers.cs); the
            // byte-faithful writer BinaryWriterExtensions.Write(BinaryWriter, MapSounds)
            // was confirmed by MapSoundsProbe (reflection dump) and is pinned by
            // MapWriteTests.Sounds_writer_is_byte_faithful_on_real_map.
            case MapSounds m: return WriteBinary(w => w.Write(m));
        }

        // A dirty entry with no override and no supported model would silently lose
        // the caller's intent if we passed the original bytes through — fail loudly.
        // Formats without a byte-faithful model writer (e.g. war3map.wts) must be
        // written via AddOrReplaceRawFile instead.
        throw new NotSupportedException(
            $"No byte-faithful serializer for '{entry.FileName ?? "(unnamed)"}' " +
            $"(model: {entry.Model?.GetType().Name ?? "none"}). Use AddOrReplaceRawFile for raw payloads.");
    }

    private static byte[] WriteBinary(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            write(writer);
        return ms.ToArray();
    }
}
