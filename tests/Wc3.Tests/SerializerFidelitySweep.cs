// tests/Wc3.Tests/SerializerFidelitySweep.cs
using War3Net.Build.Extensions;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The byte-faithfulness claim, measured per file type across the whole map library instead of
/// asserted on one map.
///
/// MapDocument.SerializeEntry can rebuild fifteen file types from their parsed model. Any entry
/// marked dirty takes that path, so every one of those types is a chance to rewrite a file the
/// user never asked to change. The promise is that an untouched model round-trips to the same
/// bytes, and until now that promise was pinned by MapWriteTests on a single map.
///
/// Two things this session showed make a single-map pin untrustworthy.
///
/// The wct writer round-trips CONTENT perfectly while destroying real bytes, because its decoder
/// turns anything it cannot interpret into U+FFFD and U+FFFD re-encodes to itself. A comparison
/// of parsed models is blind to that by construction, so only a byte comparison finds it.
///
/// And a rule measured on one map was wrong twice today: once about whether the writer emits its
/// item counts, once about how wct bodies pair to triggers. Both looked settled on the first map
/// tried.
///
/// So this loads every readable map, and for every entry carrying a parsed model, serializes that
/// UNTOUCHED model and compares byte for byte. It reports rather than fails, because the point is
/// to find out which types are exposed, and a failure would say only that something somewhere is.
/// </summary>
public class SerializerFidelitySweep
{
    private readonly ITestOutputHelper _out;
    public SerializerFidelitySweep(ITestOutputHelper output) => _out = output;

    private const char Replacement = '�';

    private static IEnumerable<string> Maps()
    {
        string[] folders =
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Warcraft III", "Maps", "Download"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Warcraft III", "Maps"),
        };
        foreach (var f in folders.Where(Directory.Exists))
        foreach (var p in Directory.EnumerateFiles(f, "*.w3?", SearchOption.TopDirectoryOnly)
                     .OrderBy(p => new FileInfo(p).Length))
            yield return p;
    }

    private sealed class Tally
    {
        public int Same, Differ, Threw;
        public long WorstDelta;
        public string WorstMap = string.Empty;
        public readonly List<string> Examples = new();
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Does_every_model_backed_file_round_trip_to_the_same_bytes()
    {
        var byType = new Dictionary<string, Tally>(StringComparer.Ordinal);
        int maps = 0, entries = 0;

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;

            MapDocument doc;
            try { doc = MapDocument.Load(path); }
            catch { continue; }
            maps++;

            foreach (var entry in doc.Files)
            {
                if (entry.Model is null) continue;
                // Only entries the save path would actually rebuild.
                if (!CanSerialize(entry.Model)) continue;

                entries++;
                string type = entry.Model.GetType().Name;
                if (!byType.TryGetValue(type, out var tally))
                    byType[type] = tally = new Tally();

                byte[] before;
                byte[] after;
                try
                {
                    before = entry.CurrentBytes;
                    // Through the document's own writer, tail and all, because that is what a
                    // save actually emits. Measuring a bare model serialize would report the
                    // dropped trailing bytes that MapDocument now re-attaches.
                    after = MapDocument.SerializeEntry(entry);
                }
                catch (Exception ex)
                {
                    tally.Threw++;
                    if (tally.Examples.Count < 3)
                        tally.Examples.Add($"{Path.GetFileName(path)} threw {ex.GetType().Name}");
                    continue;
                }

                if (before.AsSpan().SequenceEqual(after)) { tally.Same++; continue; }

                tally.Differ++;
                long delta = after.LongLength - before.LongLength;
                if (Math.Abs(delta) >= Math.Abs(tally.WorstDelta))
                {
                    tally.WorstDelta = delta;
                    tally.WorstMap = Path.GetFileName(path);
                }
                if (tally.Examples.Count < 3)
                {
                    int at = -1;
                    for (int i = 0; i < Math.Min(before.Length, after.Length); i++)
                        if (before[i] != after[i]) { at = i; break; }
                    tally.Examples.Add(
                        $"{Path.GetFileName(path)}: {entry.FileName} "
                        + $"{before.Length:N0} -> {after.Length:N0} bytes"
                        + (at < 0 ? " (identical prefix, differs in length only)"
                                  : $", first differing byte at {at:N0}"));
                }
            }
        }

        _out.WriteLine($"{maps} map(s) loaded, {entries} model-backed entr(ies) examined\n");
        _out.WriteLine($"{"model type",-26} {"same",6} {"differ",7} {"threw",6}  worst delta");
        _out.WriteLine(new string('-', 78));

        var exposed = new List<string>();
        foreach (var (type, t) in byType.OrderByDescending(kv => kv.Value.Differ)
                                        .ThenBy(kv => kv.Key, StringComparer.Ordinal))
        {
            _out.WriteLine($"{type,-26} {t.Same,6} {t.Differ,7} {t.Threw,6}  "
                         + (t.Differ == 0 ? "" : $"{t.WorstDelta:+#;-#;0} on {t.WorstMap}"));
            foreach (var e in t.Examples) _out.WriteLine($"    {e}");
            if (t.Differ > 0) exposed.Add($"{type} ({t.Differ})");
        }

        _out.WriteLine(exposed.Count == 0
            ? "\nVERDICT: every model-backed file in the library round-trips byte for byte. The "
            + "byte-faithfulness claim holds for every type the save path can rebuild."
            : "\nVERDICT: these types do NOT round-trip byte for byte, so saving a map that "
            + "touched one rewrites bytes nobody asked to change: " + string.Join(", ", exposed));

        // Asserted, not merely reported. This began as a report because the answer was unknown and
        // a bare failure would have said only that something somewhere was wrong. The answer is
        // now known and clean, so a regression has to fail rather than scroll past.
        Assert.True(exposed.Count == 0,
            "model-backed files no longer round-trip byte for byte: " + string.Join(", ", exposed));

        // A clean result means nothing if the sweep stopped finding maps, which is exactly how a
        // fidelity guard rots into a test that always passes.
        Assert.True(entries > 400,
            $"only {entries} model-backed entr(ies) were examined across {maps} map(s), so this "
            + "sweep is no longer covering the library");
    }

    /// <summary>
    /// The same sweep asked the other way: does DECODING lose bytes it cannot interpret? A model
    /// whose strings carry U+FFFD has already lost the original bytes, and every later write of
    /// that model bakes the loss in. Byte comparison catches this too, but only where the file is
    /// written, and this catches it wherever the text is merely read.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Does_any_map_decode_text_it_cannot_represent()
    {
        int maps = 0;
        var hits = new List<string>();

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;
            MapDocument doc;
            try { doc = MapDocument.Load(path); }
            catch { continue; }
            maps++;

            foreach (var entry in doc.Files)
            {
                if (entry.Model is null) continue;
                int bad = CountReplacements(entry.Model);
                if (bad > 0)
                    hits.Add($"{Path.GetFileName(path)}: {entry.FileName} "
                           + $"({entry.Model.GetType().Name}) carries {bad} undecodable "
                           + "character(s)");
            }
        }

        _out.WriteLine($"{maps} map(s) examined");
        if (hits.Count == 0)
        {
            _out.WriteLine("VERDICT: no parsed model in the library carries a replacement "
                         + "character, so nothing readable here is losing text on decode.");
            return;
        }
        foreach (var h in hits.Take(40)) _out.WriteLine("  " + h);
        _out.WriteLine($"\nVERDICT: {hits.Count} entr(ies) already lost bytes on decode. Writing "
                     + "any of them back bakes the loss in permanently.");
    }

    // ---------------------------------------------------------------- plumbing

    /// <summary>Mirrors MapDocument.SerializeEntry's dispatch. Kept as a list rather than a
    /// try/catch so a type the save path cannot rebuild is skipped rather than counted as a
    /// failure.</summary>
    private static bool CanSerialize(object model) => model switch
    {
        War3Net.Build.Object.UnitObjectData => true,
        War3Net.Build.Object.AbilityObjectData => true,
        War3Net.Build.Object.ItemObjectData => true,
        War3Net.Build.Object.DestructableObjectData => true,
        War3Net.Build.Object.DoodadObjectData => true,
        War3Net.Build.Object.BuffObjectData => true,
        War3Net.Build.Object.UpgradeObjectData => true,
        War3Net.Build.Import.ImportedFiles => true,
        War3Net.Build.Widget.MapUnits => true,
        War3Net.Build.Widget.MapDoodads => true,
        War3Net.Build.Environment.MapPathingMap => true,
        War3Net.Build.Environment.MapRegions => true,
        War3Net.Build.Environment.MapEnvironment => true,
        War3Net.Build.Audio.MapSounds => true,
        War3Net.Build.Script.MapTriggers => true,
        _ => false,
    };

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
                case War3Net.Build.Import.ImportedFiles m: w.Write(m); break;
                case War3Net.Build.Widget.MapUnits m: w.Write(m); break;
                case War3Net.Build.Widget.MapDoodads m: w.Write(m); break;
                case War3Net.Build.Environment.MapPathingMap m: w.Write(m); break;
                case War3Net.Build.Environment.MapRegions m: w.Write(m); break;
                case War3Net.Build.Environment.MapEnvironment m: w.Write(m); break;
                case War3Net.Build.Audio.MapSounds m: w.Write(m); break;
                case War3Net.Build.Script.MapTriggers m: w.Write(m); break;
                default: throw new NotSupportedException(model.GetType().Name);
            }
        }
        return ms.ToArray();
    }

    /// <summary>Counts U+FFFD across a model's public string surface, one level deep plus into
    /// enumerables, which reaches names, descriptions and script bodies without needing a
    /// per-type walker.</summary>
    private static int CountReplacements(object model, int depth = 0)
    {
        if (depth > 3) return 0;
        int n = 0;
        foreach (var p in model.GetType().GetProperties())
        {
            if (p.GetIndexParameters().Length > 0) continue;
            object? value;
            try { value = p.GetValue(model); }
            catch { continue; }
            switch (value)
            {
                case null: break;
                case string s: n += s.Count(c => c == Replacement); break;
                case System.Collections.IEnumerable seq when value is not byte[]:
                    foreach (var item in seq)
                    {
                        if (item is string si) n += si.Count(c => c == Replacement);
                        else if (item is not null && item.GetType().Namespace?.StartsWith("War3Net") == true)
                            n += CountReplacements(item, depth + 1);
                    }
                    break;
                default:
                    if (value.GetType().Namespace?.StartsWith("War3Net") == true)
                        n += CountReplacements(value, depth + 1);
                    break;
            }
        }
        return n;
    }
}
