// tests/Wc3.Tests/ObjectDataBinaryProbe.cs
using System.Text;

namespace Wc3.Tests;

/// <summary>
/// A deliberately independent reader and writer for the object-data binary format
/// (war3map.w3u/w3a/w3t/w3b/w3d/w3h/w3q and their war3mapSkin twins).
///
/// It shares no code with War3Net or with Wc3.MapDocument on purpose. The crash this
/// probe exists to pin (a version 3 object written without its modification set prefix)
/// survived validate, lint, roundtrip and pjass for two weeks precisely because every one
/// of those checks read the bytes back through the same reader that had written them, so a
/// missing field cancelled itself out. A second, independent parser cannot cancel out, and
/// it derails at exactly the byte the game's parser derails at.
///
/// Layout, per the WC3 object-data format. Strings are Latin-1, never UTF-8.
///
///   int32 fileVersion
///   int32 originalObjectCount, then that many objects
///   int32 customObjectCount,   then that many objects
///
///   object = int32 oldId, int32 newId,
///            version 3 only: int32 setCount, then per set { int32 setFlags, int32 modCount, mods }
///            version 1 and 2: int32 modCount, then mods
///
///   mod    = int32 fieldId, int32 varType,
///            extended kinds (w3a/w3q/w3d) only: int32 level or variation, int32 dataPointer,
///            value (int32 / float / float / Latin-1 null-terminated string), int32 endMarker
/// </summary>
internal static class ObjectDataBinaryProbe
{
    internal sealed record Mod(string FieldId, int VarType, int? Slot, int? Pointer, object Value, int EndMarker);

    internal sealed record Obj(int Table, string OldId, string NewId, int SetCount,
        IReadOnlyList<int> SetFlags, IReadOnlyList<Mod> Mods);

    internal sealed record Parsed(int Version, IReadOnlyList<Obj> Objects, int Consumed, int Length)
    {
        /// <summary>Bytes the declared structure never accounted for. Must always be 0 —
        /// a real map's object data ends exactly where its object tables end.</summary>
        internal int Trailing => Length - Consumed;
    }

    /// <summary>The kinds whose modifications carry a level or variation plus a data pointer.</summary>
    internal static bool IsExtended(string fileName) =>
        fileName.EndsWith(".w3a", StringComparison.OrdinalIgnoreCase)
        || fileName.EndsWith(".w3q", StringComparison.OrdinalIgnoreCase)
        || fileName.EndsWith(".w3d", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Parses strictly. Throws <see cref="InvalidDataException"/> on the first byte that does
    /// not fit the format, which is what an added object with no set prefix produces.
    /// </summary>
    internal static Parsed Read(byte[] bytes, string fileName)
    {
        bool extended = IsExtended(fileName);
        int p = 0;

        int I32()
        {
            if (p + 4 > bytes.Length)
                throw new InvalidDataException($"{fileName}: ran off the end at 0x{p:x}");
            int v = BitConverter.ToInt32(bytes, p);
            p += 4;
            return v;
        }

        string Raw()
        {
            int at = p;
            _ = I32();
            return Encoding.Latin1.GetString(bytes, at, 4);
        }

        string Cstr()
        {
            int end = Array.IndexOf(bytes, (byte)0, p);
            if (end < 0) throw new InvalidDataException($"{fileName}: unterminated string at 0x{p:x}");
            var s = Encoding.Latin1.GetString(bytes, p, end - p);
            p = end + 1;
            return s;
        }

        int version = I32();
        if (version is < 1 or > 3)
            throw new InvalidDataException($"{fileName}: implausible fileVersion {version}");

        var objects = new List<Obj>();
        for (int table = 0; table < 2; table++)
        {
            int count = I32();
            if (count < 0 || count > 1_000_000)
                throw new InvalidDataException($"{fileName}: implausible object count {count} in table {table}");

            for (int i = 0; i < count; i++)
            {
                string oldId = Raw(), newId = Raw();
                int setCount = version >= 3 ? I32() : 1;
                if (setCount < 0 || setCount > 64)
                    throw new InvalidDataException(
                        $"{fileName}: object {oldId}->{newId} declares {setCount} modification sets");

                var flags = new List<int>();
                var mods = new List<Mod>();
                for (int s = 0; s < setCount; s++)
                {
                    if (version >= 3) flags.Add(I32());
                    int modCount = I32();
                    if (modCount < 0 || modCount > 100_000)
                        throw new InvalidDataException(
                            $"{fileName}: object {oldId}->{newId} declares {modCount} modifications");

                    for (int m = 0; m < modCount; m++)
                    {
                        string fieldId = Raw();
                        int varType = I32();
                        int? slot = extended ? I32() : null;
                        int? pointer = extended ? I32() : null;
                        object value = varType switch
                        {
                            0 => I32(),
                            1 or 2 => BitConverter.Int32BitsToSingle(I32()),
                            3 => Cstr(),
                            _ => throw new InvalidDataException(
                                $"{fileName}: object {oldId}->{newId} field {fieldId} has varType {varType} at 0x{p - 4:x}"),
                        };
                        mods.Add(new Mod(fieldId, varType, slot, pointer, value, I32()));
                    }
                }
                objects.Add(new Obj(table, oldId, newId, setCount, flags, mods));
            }
        }
        return new Parsed(version, objects, p, bytes.Length);
    }

    /// <summary>
    /// Writes a minimal but format-correct file, used to hand tests a believable "already
    /// on disk" object-data payload without going near the production writer.
    /// </summary>
    internal static byte[] Write(int version, string fileName,
        IReadOnlyList<(int Table, string OldId, string NewId, IReadOnlyList<Mod> Mods)> objects)
    {
        bool extended = IsExtended(fileName);
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms, Encoding.Latin1, leaveOpen: true);

        void Raw(string code) => w.Write(BitConverter.ToInt32(Encoding.Latin1.GetBytes(code), 0));

        w.Write(version);
        for (int table = 0; table < 2; table++)
        {
            var here = objects.Where(o => o.Table == table).ToList();
            w.Write(here.Count);
            foreach (var o in here)
            {
                Raw(o.OldId);
                if (o.NewId.Length == 4) Raw(o.NewId); else w.Write(0);
                if (version >= 3)
                {
                    w.Write(1);                                   // exactly one set, as every real map has
                    w.Write(Wc3.Model.ObjectDataSets.DefaultSetFlags);
                }
                w.Write(o.Mods.Count);
                foreach (var m in o.Mods)
                {
                    Raw(m.FieldId);
                    w.Write(m.VarType);
                    if (extended) { w.Write(m.Slot ?? 0); w.Write(m.Pointer ?? 0); }
                    switch (m.VarType)
                    {
                        case 0: w.Write(Convert.ToInt32(m.Value)); break;
                        case 1:
                        case 2: w.Write(Convert.ToSingle(m.Value)); break;
                        default:
                            w.Write(Encoding.Latin1.GetBytes((string)m.Value));
                            w.Write((byte)0);
                            break;
                    }
                    w.Write(m.EndMarker);
                }
            }
        }
        w.Flush();
        return ms.ToArray();
    }
}
