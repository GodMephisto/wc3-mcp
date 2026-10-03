// tests/Wc3.Tests/ProtectedMapSaveProbe.cs
using System.Reflection;
using War3Net.IO.Mpq;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Two maps in the library cannot be saved at all. Both are protected, and the standing
/// explanation is "65,534 of 65,536 hash slots stuffed", which is a symptom rather than a
/// diagnosis: it says the table is full, not what the rebuild actually needs and cannot get.
///
/// Before proposing an MPQ change, this establishes the facts. What exactly throws, from where,
/// how full each table really is, whether the builder exposes any control over the table it
/// creates, and whether the block count or the hash count is the binding constraint.
/// </summary>
public class ProtectedMapSaveProbe
{
    private readonly ITestOutputHelper _out;
    public ProtectedMapSaveProbe(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    public static IEnumerable<object[]> Cases()
    {
        yield return new object[] { "ORDR_S2_2.305[R]_english.w3x" };
        yield return new object[] { "PumpkinTD_v2.3b.w3x" };
        // A healthy map, as the control. Without one, "it throws" says nothing about protection.
        yield return new object[] { "GGGA_V0.04g.w3x" };
    }

    [Theory]
    [MemberData(nameof(Cases))]
    [Trait("Category", "Corpus")]
    public void What_exactly_stops_this_map_being_saved(string name)
    {
        string path = Path.Combine(Dir, name);
        if (!File.Exists(path)) { _out.WriteLine($"{name} absent, skipped"); return; }

        _out.WriteLine($"=== {name} ({new FileInfo(path).Length:N0} bytes) ===");

        MapDocument doc;
        try { doc = MapDocument.Load(path); }
        catch (Exception ex)
        {
            _out.WriteLine($"LOAD FAILED: {ex.GetType().Name}: {ex.Message}");
            return;
        }

        int named = doc.Files.Count(f => f.FileName is not null);
        _out.WriteLine($"loaded: {doc.Files.Count:N0} entr(ies), {named:N0} named, "
                     + $"{doc.Files.Count - named:N0} unnamed, "
                     + $"{doc.Diagnostics.Count:N0} diagnostic(s)");

        // The archive's own header numbers, which is where "65,534 of 65,536" comes from.
        DumpArchiveShape(path);

        // The actual save attempt, unmodified. A map nobody edited must still round-trip.
        try
        {
            byte[] saved = doc.SaveToBytes();
            _out.WriteLine($"SAVE OK: {saved.Length:N0} bytes");
        }
        catch (Exception ex)
        {
            _out.WriteLine($"SAVE FAILED: {ex.GetType().Name}");
            for (var e = ex; e is not null; e = e.InnerException)
                _out.WriteLine($"   {e.GetType().Name}: {e.Message}");
            // The wrapper deliberately hides the underlying failure behind a readable message,
            // which is right for a user and useless for diagnosis, so dig the real one out.
            var root = ex;
            while (root.InnerException is not null) root = root.InnerException;
            _out.WriteLine($"   ROOT {root.GetType().FullName}");
            foreach (var line in (root.StackTrace ?? "(no stack captured)").Split('\n').Take(16))
                _out.WriteLine("     " + line.Trim());
        }
    }

    /// <summary>Reads the MPQ header directly, so the table sizes are the file's own numbers
    /// rather than something inferred.</summary>
    private void DumpArchiveShape(string path)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            int offset = Wc3.Model.MpqHeader.FindArchiveOffset(bytes);
            if (offset < 0) { _out.WriteLine("no MPQ header found"); return; }

            // MPQ header v1: 'MPQ\x1A', headerSize, archiveSize, formatVersion, blockSize,
            // hashTablePos, blockTablePos, hashTableSize, blockTableSize.
            int b = offset;
            uint headerSize = BitConverter.ToUInt32(bytes, b + 0x04);
            uint archiveSize = BitConverter.ToUInt32(bytes, b + 0x08);
            ushort formatVersion = BitConverter.ToUInt16(bytes, b + 0x0C);
            ushort blockSizeShift = BitConverter.ToUInt16(bytes, b + 0x0E);
            uint hashTableSize = BitConverter.ToUInt32(bytes, b + 0x18);
            uint blockTableSize = BitConverter.ToUInt32(bytes, b + 0x1C);

            _out.WriteLine($"header at 0x{offset:X}: headerSize={headerSize}, "
                         + $"archiveSize={archiveSize:N0}, format=v{formatVersion}, "
                         + $"sector={512 << blockSizeShift:N0} (shift {blockSizeShift})");
            _out.WriteLine($"  hash table {hashTableSize:N0} slot(s), "
                         + $"block table {blockTableSize:N0} entr(ies)");

            using var fs = File.OpenRead(path);
            using var archive = MpqArchive.Open(fs, loadListFile: true);
            int enumerated = archive.Count();
            _out.WriteLine($"  enumerable entries {enumerated:N0}, "
                         + $"so {hashTableSize - enumerated:N0} hash slot(s) hold no live file");
        }
        catch (Exception ex)
        {
            _out.WriteLine($"  shape read failed: {ex.GetType().Name}: {ex.Message}");
        }
    }

    [Fact]
    public void What_control_does_the_builder_expose_over_the_table_it_creates()
    {
        // If the rebuild can be told to allocate a bigger hash table, the fix is a parameter
        // rather than a fork. If it cannot, that is the finding.
        foreach (var t in new[] { typeof(MpqArchiveBuilder), typeof(MpqArchiveCreateOptions) })
        {
            _out.WriteLine($"=== {t.Name} ===");
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                _out.WriteLine($"  prop {p.PropertyType.Name,-28} {p.Name}"
                             + (p.CanWrite ? " (settable)" : " (read-only)"));
            foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance
                                         | BindingFlags.Static | BindingFlags.DeclaredOnly))
                _out.WriteLine($"  {m.ReturnType.Name} {m.Name}("
                             + string.Join(", ", m.GetParameters()
                                 .Select(x => $"{x.ParameterType.Name} {x.Name}")) + ")");
            foreach (var c in t.GetConstructors())
                _out.WriteLine("  ctor(" + string.Join(", ", c.GetParameters()
                    .Select(x => $"{x.ParameterType.Name} {x.Name}")) + ")");
        }
    }

    [Fact]
    public void What_can_an_MpqFile_be_built_from()
    {
        // The rebuild dies inside MpqArchiveBuilder(originalArchive), which eagerly opens every
        // entry. A fallback has to construct the archive from scratch instead, and that is only
        // possible if an entry with NO recoverable name can still be carried, since 65,516 of
        // 65,536 entries on these maps have none. An MPQ locates a file by the hash of its name,
        // so the question is whether the API accepts a hash in place of a name.
        foreach (var t in new[] { typeof(MpqFile), typeof(MpqKnownFile), typeof(MpqUnknownFile) })
        {
            _out.WriteLine($"=== {t.Name} ===");
            foreach (var c in t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic
                                              | BindingFlags.Instance))
                _out.WriteLine($"  {(c.IsPublic ? "public" : "internal")} ctor("
                             + string.Join(", ", c.GetParameters()
                                 .Select(x => $"{x.ParameterType.Name} {x.Name}")) + ")");
            foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static
                                         | BindingFlags.DeclaredOnly))
                _out.WriteLine($"  static {m.ReturnType.Name} {m.Name}("
                             + string.Join(", ", m.GetParameters()
                                 .Select(x => $"{x.ParameterType.Name} {x.Name}")) + ")");
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                _out.WriteLine($"  prop {p.PropertyType.Name} {p.Name}");
        }
    }
}
