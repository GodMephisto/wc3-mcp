// src/Wc3.Commands/PortraitRepairCommand.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One model the repair looked at, and what it decided.</summary>
public sealed record PortraitModel(string Name, int? Version, bool Camera, string Action,
    int BytesBefore, int BytesAfter);

public sealed record PortraitRepairResult(
    int UnitsScanned,
    int ModelsResolved,
    int ModelsAtRisk,
    int ModelsRepaired,
    IReadOnlyList<PortraitModel> Models,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Repairs the black portrait pane Reforged 3.0.0 renders for a model that ships a camera at a
/// model version below 900.
///
/// The evidence for the remedy, and three remedies that were tried and failed first.
///
/// Measured on one real map, 109 hero units resolve to models split three ways. Those with NO
/// camera render correctly, because the engine falls back to its own. Those with a camera at
/// version 800 render black. So the working group and the broken group differ by exactly one
/// chunk, and removing it makes the broken group look like the working one.
///
/// AND THAT WAS NOT ENOUGH. A build shipping the camera removal alone was play-tested and the
/// pane was still black, so the reasoning above is incomplete. The map's author states the
/// version must be re-stamped as well, which `--stamp-version` does.
///
/// The open structural question, stated rather than guessed. A version 900 or above geoset
/// carries a u32 lod plus a char[80] lod name that an 800 era geoset does not, and a material
/// carries a char[80] shader name. Both were read off this repository's own MdxParser, and
/// measured across 311 models in one real map, all 311 are version 800 and all of them walk
/// exactly under the 800 layout and not at all under the 900 layout. So a bare re-stamp leaves
/// the declared version and the real layout disagreeing.
///
/// Two readings follow and they are mutually exclusive. If the engine honours the declared
/// version then `--upgrade-geometry` is required. If it reads geometry the old way regardless
/// then the widening is what breaks it. That map contains no model at 900 or above, so it
/// cannot settle its own case, and only running the game can. Build both and look.
///
/// Editing the camera's own fields changed nothing. Replacing the model wholesale is not a repair.
///
/// Why a chunk delete is safe where a version re-stamp is not. MDX is a FLAT chunk list, MDLX
/// followed by a tag, a uint32 size and a body, so removing one chunk is an exact splice with
/// no offset anywhere to repair. Nothing in the format points at a byte offset.
///
/// This remains a structural edit to a binary asset, so the result ships as a separate file and
/// the caller is told to test it. A clean parse is not proof that the game renders it.
/// </summary>
public static class PortraitRepairCommand
{
    /// <param name="apply">False reports what it would do and changes nothing.</param>
    /// <param name="stampVersion">
    /// When set, the model's VERS is rewritten to this value after the camera is removed.
    /// Removing the camera alone was reported by the map's author as not enough, and the
    /// version has to be re-stamped too.
    /// </param>
    /// <param name="upgradeGeometry">
    /// Insert the fields a version 900 or above model carries and an 800 era one does not,
    /// rather than only changing the number. Measured on this repository's own MdxParser, a
    /// geoset gains a u32 lod plus a char[80] lod name (84 bytes) and a material gains a
    /// char[80] shader name. Stamping 1800 without inserting those leaves a file whose declared
    /// version and actual layout disagree.
    ///
    /// Left as a separate switch on purpose, because the two readings are mutually exclusive
    /// and only the game can settle which is right. If the engine honours the declared version,
    /// the insert is required. If it reads geometry the old way regardless, the insert is what
    /// breaks it. Build both and look.
    /// </param>
    public static PortraitRepairResult Execute(
        MapDocument doc, bool apply, int? stampVersion = null, bool upgradeGeometry = false)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var diagnostics = new List<string>();
        var models = new List<PortraitModel>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var units = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit)).ToList();
        int resolved = 0, atRisk = 0, repaired = 0;

        foreach (var unit in units)
        {
            string? declared = unit.Mods
                .FirstOrDefault(m => m.Key.Equals("umdl", StringComparison.OrdinalIgnoreCase))
                .Value;
            if (string.IsNullOrWhiteSpace(declared)) continue;

            string baseName = declared.Replace('/', '\\').Split('\\').Last();
            int dot = baseName.LastIndexOf('.');
            string stem = dot > 0 ? baseName[..dot] : baseName;
            if (!seen.Add(stem)) continue;

            // Resolved by MPQ name hash, NOT through GetFile, because a protected map lists
            // almost nothing. On one real map that difference was 3 models found against 178.
            byte[]? bytes = null;
            string? hit = null;
            foreach (var candidate in new[] { declared, stem + ".mdx", stem + ".mdl" })
                if (doc.TryReadFileByName(candidate, out var b) && b.Length >= 12)
                {
                    bytes = b;
                    hit = candidate;
                    break;
                }
            if (bytes is null || hit is null) continue;   // a base game path, nothing to repair
            resolved++;

            var (version, camera) = AuditCommand.ReadMdxHeader(bytes);
            if (version is not int ver)
            {
                models.Add(new PortraitModel(hit, version, camera, "fine, no version",
                    bytes.Length, bytes.Length));
                continue;
            }

            // Without a version stamp the risk is the CONJUNCTION, a camera at a version below
            // 900, because only those render the black pane. With one, the scope is every model
            // below the target version, camera or not. That second case is what reaches a model
            // whose camera an earlier pass already removed, which is exactly the build that was
            // shipped and reported still broken.
            int target = stampVersion ?? 900;
            bool inScope = ver < target && (camera || stampVersion is not null);
            if (!inScope)
            {
                models.Add(new PortraitModel(hit, ver, camera,
                    ver >= target ? "fine, modern version" : "fine, no camera",
                    bytes.Length, bytes.Length));
                continue;
            }

            atRisk++;
            var stripped = RemoveChunk(bytes, "CAMS"u8);
            // RemoveChunk hands back the SAME array when the tag was not there, which is the
            // cameraless case that --stamp-version now brings into scope. Stamping that array
            // would write through into the document's own bytes before the verify step has run.
            if (ReferenceEquals(stripped, bytes)) stripped = (byte[])bytes.Clone();
            if (stripped is null)
            {
                models.Add(new PortraitModel(hit, ver, camera, "at risk, chunk walk failed",
                    bytes.Length, bytes.Length));
                diagnostics.Add($"{hit} has a camera at version {ver} but its chunk list could "
                    + "not be walked cleanly, so it was left alone");
                continue;
            }

            if (upgradeGeometry)
            {
                var upgraded = UpgradeToV900Layout(stripped);
                if (upgraded is null)
                {
                    models.Add(new PortraitModel(hit, ver, camera, "at risk, geometry upgrade failed",
                        bytes.Length, stripped.Length));
                    diagnostics.Add($"{hit} could not have its geosets or materials widened to "
                        + "the version 900 layout, so it was left alone");
                    continue;
                }
                stripped = upgraded;
            }

            if (stampVersion is int stamp && !StampVersion(stripped, stamp))
            {
                models.Add(new PortraitModel(hit, ver, camera, "at risk, version stamp failed",
                    bytes.Length, stripped.Length));
                diagnostics.Add($"{hit} has no readable VERS chunk to stamp, so it was left alone");
                continue;
            }

            // Re-read the result rather than trusting the splice. A model that still reports a
            // camera, or whose version is not what we meant it to be, has been damaged rather
            // than repaired.
            int expectVer = stampVersion ?? ver;
            var (verAfter, camAfter) = AuditCommand.ReadMdxHeader(stripped);
            if (camAfter || verAfter != expectVer)
            {
                models.Add(new PortraitModel(hit, ver, camera, "at risk, splice verify failed",
                    bytes.Length, stripped.Length));
                diagnostics.Add($"{hit} did not verify after the splice, version {verAfter} "
                    + $"camera {camAfter}, so it was left alone");
                continue;
            }

            if (apply && !doc.TryReplaceFileByName(hit, stripped))
            {
                models.Add(new PortraitModel(hit, ver, camera, "at risk, write failed",
                    bytes.Length, stripped.Length));
                diagnostics.Add($"{hit} could not be written back");
                continue;
            }

            repaired++;
            string what = stampVersion is int s2
                ? (upgradeGeometry
                    ? $"camera removed, widened and stamped v{s2}"
                    : $"camera removed, stamped v{s2}")
                : "camera removed";
            models.Add(new PortraitModel(hit, ver, camera,
                apply ? what : "would " + what, bytes.Length, stripped.Length));
        }

        diagnostics.Add($"{units.Count} units, {resolved} distinct models resolved from the "
            + $"archive, {atRisk} at risk, {repaired} {(apply ? "repaired" : "repairable")}");
        return new PortraitRepairResult(units.Count, resolved, atRisk, repaired,
            models.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase).ToList(), diagnostics);
    }

    /// <summary>
    /// Rewrites the VERS chunk's value in place. False when there is no readable one, in which
    /// case the caller must leave the model alone rather than ship a half-edit.
    /// </summary>
    internal static bool StampVersion(byte[] data, int version)
    {
        ArgumentNullException.ThrowIfNull(data);
        foreach (var (tag, body, size) in Walk(data))
            if (tag == "VERS" && size >= 4)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(body, 4), (uint)version);
                return true;
            }
        return false;
    }

    /// <summary>
    /// Widens an 800 era model to the layout a version 900 or above model actually has, by
    /// inserting the two fields that were added and fixing every size that then changes.
    ///
    /// The two deltas, both read off this repository's own <c>MdxParser</c> rather than from
    /// memory. A geoset gains a u32 lod and a char[80] lod name, 84 bytes, sitting after the
    /// material id, selection group and selection flags and before the extent block. A material
    /// gains a char[80] shader name, sitting after the inclusive size, priority plane and flags
    /// and before its LAYS. The inserted bytes are zero, which is lod 0 and an empty name, the
    /// same thing an exporter writes for a model with no level of detail.
    ///
    /// Returns null and touches nothing if any structure does not walk exactly, because a
    /// partially widened model is worse than an un-widened one.
    /// </summary>
    internal static byte[]? UpgradeToV900Layout(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var chunks = Walk(data).ToList();
        if (chunks.Count == 0) return null;

        var outBytes = new MemoryStream();
        outBytes.Write(data.AsSpan(0, 4));                       // MDLX
        foreach (var (tag, body, size) in chunks)
        {
            byte[]? rebuilt = tag switch
            {
                "GEOS" => WidenSubChunks(data, body, size, 12, 84),
                "MTLS" => WidenSubChunks(data, body, size, 8, 80),
                _ => null,
            };
            if (tag is "GEOS" or "MTLS" && rebuilt is null) return null;

            var payload = rebuilt ?? data[body..(body + size)];
            outBytes.Write(System.Text.Encoding.ASCII.GetBytes(tag));
            Span<byte> len = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(len, (uint)payload.Length);
            outBytes.Write(len);
            outBytes.Write(payload);
        }
        return outBytes.ToArray();
    }

    /// <summary>
    /// GEOS and MTLS are both lists of records that lead with their own inclusive size, so one
    /// routine widens both. <paramref name="afterMats"/> is where the new field goes, measured
    /// for MTLS from the record start, and for GEOS from the end of the MATS array, which is
    /// why the geoset case has to walk its tagged sub-chunks to find that point.
    /// </summary>
    private static byte[]? WidenSubChunks(byte[] data, int body, int size, int afterMats, int pad)
    {
        var o = new MemoryStream();
        int p = body, end = body + size;
        while (p < end)
        {
            if (p + 4 > end) return null;
            int inclusive = (int)BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(p, 4));
            if (inclusive < 8 || p + inclusive > end) return null;

            int insertAt = pad == 80
                ? p + afterMats                     // material, straight after flags
                : GeosetInsertPoint(data, p, p + inclusive);
            if (insertAt < 0) return null;

            Span<byte> len = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(len, (uint)(inclusive + pad));
            o.Write(len);                                        // the new inclusive size
            o.Write(data.AsSpan(p + 4, insertAt - (p + 4)));      // everything before the field
            o.Write(new byte[pad]);                              // the field itself, zeroed
            o.Write(data.AsSpan(insertAt, p + inclusive - insertAt));
            p += inclusive;
        }
        return o.ToArray();
    }

    /// <summary>
    /// Where the lod block goes inside one geoset, which is immediately after the material id,
    /// selection group and selection flags that follow the MATS array. Returns -1 if the tagged
    /// sub-chunks do not appear in the order the format defines.
    /// </summary>
    private static int GeosetInsertPoint(byte[] data, int start, int end)
    {
        int p = start + 4;
        foreach (var (tag, stride) in new[]
                 {
                     ("VRTX", 12), ("NRMS", 12), ("PTYP", 4), ("PCNT", 4),
                     ("PVTX", 2), ("GNDX", 1), ("MTGC", 4), ("MATS", 4),
                 })
        {
            if (p + 8 > end || System.Text.Encoding.ASCII.GetString(data, p, 4) != tag) return -1;
            long count = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(p + 4, 4));
            p += 8 + checked((int)(count * stride));
            if (p > end) return -1;
        }
        p += 12;                            // material id, selection group, selection flags
        return p <= end ? p : -1;
    }

    /// <summary>Every top level chunk as (tag, body offset, size), stopping at the first ragged one.</summary>
    private static IEnumerable<(string Tag, int Body, int Size)> Walk(byte[] data)
    {
        if (data.Length < 8 || System.Text.Encoding.ASCII.GetString(data, 0, 4) != "MDLX")
            yield break;
        int off = 4;
        while (off + 8 <= data.Length)
        {
            string tag = System.Text.Encoding.ASCII.GetString(data, off, 4);
            int size = (int)BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(off + 4, 4));
            if (size < 0 || off + 8 + size > data.Length) yield break;
            yield return (tag, off + 8, size);
            off += 8 + size;
        }
    }

    /// <summary>
    /// The model with every chunk of that tag spliced out, or null when the chunk list does not
    /// walk cleanly to the end, in which case the file is not touched at all.
    /// </summary>
    internal static byte[]? RemoveChunk(byte[] data, ReadOnlySpan<byte> tag)
    {
        if (data.Length < 12 || data[0] != 'M' || data[1] != 'D' || data[2] != 'L' || data[3] != 'X')
            return null;
        var keep = new List<(int Offset, int Length)> { (0, 4) };
        int p = 4;
        bool found = false;
        while (p + 8 <= data.Length)
        {
            uint size = BitConverter.ToUInt32(data, p + 4);
            long end = (long)p + 8 + size;
            if (end > data.Length) return null;          // truncated or misread, refuse
            if (data.AsSpan(p, 4).SequenceEqual(tag)) found = true;
            else keep.Add((p, 8 + (int)size));
            p = (int)end;
        }
        if (p != data.Length) return null;               // trailing bytes, refuse
        if (!found) return data;

        var outBytes = new byte[keep.Sum(k => k.Length)];
        int w = 0;
        foreach (var (offset, length) in keep)
        {
            Buffer.BlockCopy(data, offset, outBytes, w, length);
            w += length;
        }
        return outBytes;
    }
}
