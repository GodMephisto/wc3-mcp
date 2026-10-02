// tests/Wc3.Tests/DuplicateLocaleProbe.cs
using System.Reflection;
using War3Net.IO.Mpq;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Measures what the duplicate entries in the affected maps actually are, rather than assuming.
/// An MPQ hash slot is keyed by name hash plus locale, so a duplicate name is normally a localized
/// twin, and the right repair strategy depends entirely on which locales are present.
/// </summary>
public class DuplicateLocaleProbe
{
    private readonly ITestOutputHelper _out;
    public DuplicateLocaleProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void What_are_the_duplicate_slots()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "Naruto Autobattle ENGv3ch11.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        using var fs = new MemoryStream(File.ReadAllBytes(path));
        using var archive = MpqArchive.Open(fs, loadListFile: true);

        // Block index to locale, read off the hash table.
        var localesByBlock = new Dictionary<uint, List<MpqLocale>>();
        foreach (var hash in archive.EnumerateHashes())
        {
            var t = hash.GetType();
            object? blockObj = t.GetProperty("BlockIndex")?.GetValue(hash);
            object? locObj = t.GetProperty("Locale")?.GetValue(hash);
            if (blockObj is null || locObj is null) continue;
            uint block = Convert.ToUInt32(blockObj);
            if (!localesByBlock.TryGetValue(block, out var list))
                localesByBlock[block] = list = new List<MpqLocale>();
            list.Add((MpqLocale)locObj);
        }
        _out.WriteLine($"hash slots carrying a block index: {localesByBlock.Count}");

        int i = 0;
        var byName = new Dictionary<string, List<(int Index, MpqLocale[] Locales, uint Size)>>(
            StringComparer.OrdinalIgnoreCase);
        foreach (var entry in archive)
        {
            string name = entry.FileName ?? $"(unnamed {i})";
            var locales = localesByBlock.TryGetValue((uint)i, out var l) ? l.ToArray() : Array.Empty<MpqLocale>();
            if (!byName.TryGetValue(name, out var rows))
                byName[name] = rows = new List<(int, MpqLocale[], uint)>();
            rows.Add((i, locales, entry.FileSize));
            i++;
        }

        _out.WriteLine("");
        _out.WriteLine("names appearing more than once:");
        foreach (var (name, rows) in byName.Where(kv => kv.Value.Count > 1).OrderBy(kv => kv.Key))
        {
            _out.WriteLine($"  {name}");
            foreach (var (index, locales, size) in rows)
                _out.WriteLine($"      block {index,5}  size {size,9}  locales [{string.Join(", ", locales)}]");
        }

        _out.WriteLine("");
        _out.WriteLine("MpqFile.New overloads:");
        foreach (var m in typeof(MpqFile).GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Where(m => m.Name == "New"))
            _out.WriteLine("  " + m);

        _out.WriteLine("MpqKnownFile constructors:");
        foreach (var c in typeof(MpqKnownFile).GetConstructors())
            _out.WriteLine("  " + c);

        _out.WriteLine("MpqHash members:");
        foreach (var p in typeof(MpqHash).GetProperties())
            _out.WriteLine($"  prop {p.PropertyType.Name} {p.Name}");
        foreach (var c in typeof(MpqHash).GetConstructors())
            _out.WriteLine("  ctor " + c);

        _out.WriteLine("MpqArchiveBuilder methods:");
        foreach (var m in typeof(MpqArchiveBuilder).GetMethods(
                     BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            _out.WriteLine("  " + m);
    }
}
