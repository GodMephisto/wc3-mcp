// tests/Wc3.Tests/TrailingBytesProbe.cs
using War3Net.Build.Extensions;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// SerializerFidelitySweep found 9 entries across 6 model types where re-serializing an UNTOUCHED
/// model produces a strict PREFIX of the original file, short by exactly 4 bytes for every object
/// table and by 1 byte for one war3mapUnits.doo. A strict prefix means nothing was rewritten, the
/// writer simply stops earlier than the original ended.
///
/// Whether that matters depends entirely on what those bytes are, and there are three very
/// different answers hiding behind the same nine numbers. Padding the World Editor happens to
/// write is harmless to drop and only a fidelity blemish. A trailing count field for a table
/// section the writer omits when empty is a format difference worth matching. Real content the
/// parser never read would mean the parse is lossy, and every save of an edited table has been
/// discarding map data.
///
/// So this prints the bytes.
/// </summary>
public class TrailingBytesProbe
{
    private readonly ITestOutputHelper _out;
    public TrailingBytesProbe(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    public static IEnumerable<object[]> Cases()
    {
        yield return new object[] { "12331.w3x", "war3map.w3a" };
        yield return new object[] { "12331.w3x", "war3map.w3b" };
        yield return new object[] { "12331.w3x", "war3map.w3d" };
        yield return new object[] { "U9_PumpkinZ_v4.7d.w3x", "war3map.w3u" };
        yield return new object[] { "U9_PumpkinZ_v4.7d.w3x", "war3map.w3h" };
        yield return new object[] { "Random Farm TD 0.63 Beta.w3x", "war3mapUnits.doo" };
        yield return new object[] { "Random Farm TD 0.63 Beta.w3x", "war3map.w3d" };
    }

    [Theory]
    [MemberData(nameof(Cases))]
    [Trait("Category", "Corpus")]
    public void What_are_the_bytes_the_writer_leaves_off(string map, string file)
    {
        string path = Path.Combine(Dir, map);
        if (!File.Exists(path)) { _out.WriteLine($"{map} absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var entry = doc.GetFile(file);
        if (entry?.Model is null) { _out.WriteLine($"{file} has no parsed model, skipped"); return; }

        byte[] before = entry.CurrentBytes;
        byte[] after = Serialize(entry.Model);

        _out.WriteLine($"{map} / {file} ({entry.Model.GetType().Name})");
        _out.WriteLine($"  original {before.Length:N0} bytes, written {after.Length:N0} bytes, "
                     + $"delta {after.Length - before.Length:+#;-#;0}");

        if (after.Length >= before.Length)
        {
            _out.WriteLine("  not shorter, nothing to report");
            return;
        }

        bool isPrefix = before.AsSpan(0, after.Length).SequenceEqual(after);
        _out.WriteLine($"  written output is a strict prefix of the original: {isPrefix}");
        if (!isPrefix)
        {
            _out.WriteLine("  => NOT a trailing-bytes case, the content itself differs");
            return;
        }

        var tail = before.AsSpan(after.Length).ToArray();
        _out.WriteLine($"  the {tail.Length} dropped byte(s): "
                     + string.Join(" ", tail.Select(b => b.ToString("X2"))));
        _out.WriteLine($"  as text: "
                     + new string(tail.Select(b => b >= 32 && b < 127 ? (char)b : '.').ToArray()));
        if (tail.Length == 4)
            _out.WriteLine($"  as int32 little-endian: {BitConverter.ToInt32(tail, 0)}");
        _out.WriteLine(tail.All(b => b == 0)
            ? "  VERDICT: all zero, so this is padding or an empty trailing field, not content."
            : "  VERDICT: NON-ZERO, so the parser may be leaving real data unread.");
    }

    private static byte[] Serialize(object model)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            switch (model)
            {
                case War3Net.Build.Object.UnitObjectData m: w.Write(m); break;
                case War3Net.Build.Object.AbilityObjectData m: w.Write(m); break;
                case War3Net.Build.Object.ItemObjectData m: w.Write(m); break;
                case War3Net.Build.Object.DestructableObjectData m: w.Write(m); break;
                case War3Net.Build.Object.DoodadObjectData m: w.Write(m); break;
                case War3Net.Build.Object.BuffObjectData m: w.Write(m); break;
                case War3Net.Build.Object.UpgradeObjectData m: w.Write(m); break;
                case War3Net.Build.Widget.MapUnits m: w.Write(m); break;
                case War3Net.Build.Widget.MapDoodads m: w.Write(m); break;
                default: throw new NotSupportedException(model.GetType().Name);
            }
        }
        return ms.ToArray();
    }
}
