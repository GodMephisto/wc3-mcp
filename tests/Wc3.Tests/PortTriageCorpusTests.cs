// tests/Wc3.Tests/PortTriageCorpusTests.cs
// Sweeps a whole library of real custom maps, ports several heroes out of each into a fresh blank
// map, and checks whether the result would still compile. The point is to find the porting patterns
// that break BEFORE they are hit in game, rather than one hero at a time in a host lobby.
//
// Needs real maps on disk, so it is Category=Corpus (excluded from the hermetic suite) and skips
// silently when the library is absent. Writes a ranked report next to the maps.
//
//   dotnet test --filter "Category=Corpus&FullyQualifiedName~PortTriage"
using System.Diagnostics;
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

public class PortTriageCorpusTests
{
    private static readonly string MapDir = TestCorpus.Directory;

    /// <summary>Heroes to port per map. Some of the user's maps are hero-arena games with
    /// hundreds of playable heroes (Anime Choice Arena alone has 296), so this can never be
    /// exhaustive, it trades that off against a run that finishes in a few minutes. Raised from
    /// 4, which under-sampled every map that actually has more than a handful of heroes and so
    /// under-reported this sweep's whole reason for existing.</summary>
    private const int HeroesPerMap = 8;

    private readonly ITestOutputHelper _out;
    public PortTriageCorpusTests(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void Triage_ports_across_the_map_library()
    {
        if (!Directory.Exists(MapDir)) return;

        var maps = UniqueCustomMaps(MapDir);
        if (maps.Count == 0) return;

        var failures = new List<(string Kind, string Map, string Hero, string Detail)>();
        // One block of report text per map, plus a severity score so the worst offenders can be
        // printed first. A map is only actionable in proportion to how broken it is, and the old
        // report ordered maps by file size, so the single most useful map to look at could be
        // buried on page three. A dead compile (refused/broken/threw) outweighs any number of
        // wiring problems, a hero that does not even build is strictly worse than one that
        // builds with a dead spell, so it is weighted far higher.
        var mapBlocks = new List<(int Severity, int CleanHeroes, int ProblemHeroes, string Text)>();
        int attempted = 0, compiled = 0, refused = 0, repaired = 0, mapErrors = 0;
        var sw = Stopwatch.StartNew();

        foreach (var map in maps)
        {
            string name = Path.GetFileName(map);
            var block = new StringBuilder();
            MapDocument source;
            try { source = MapDocument.Load(map); }
            catch (Exception ex)
            {
                mapErrors++;
                failures.Add(("map-load-failed", name, "", Trunc(ex.Message)));
                block.AppendLine($"## {name}\n  MAP LOAD FAILED: {Trunc(ex.Message)}\n");
                mapBlocks.Add((100, 0, 0, block.ToString()));
                continue;
            }

            var allHeroes = Heroes(source).ToList();
            var heroes = allHeroes.Take(HeroesPerMap).ToList();
            int cleanHeroes = 0, problemHeroes = 0, severity = 0;
            var heroLines = new StringBuilder();

            foreach (var (rawcode, heroName) in heroes)
            {
                attempted++;
                try
                {
                    var target = MapDocument.Load(BlankMap.Create().SaveToBytes());
                    var bundle = BundleCommand.ResolveObject(source, ObjectKind.Unit, rawcode, gameDirOverride: null);
                    var result = PortCommand.PortUnit(source, bundle, target);

                    // The gate's verdict, then an independent check of what actually landed.
                    var script = result.Script;
                    if (script is { Written: false })
                    {
                        refused++; problemHeroes++; severity += 50;
                        var why = script.Notes.FirstOrDefault(n => n.Contains("REFUSED", StringComparison.Ordinal)) ?? "";
                        failures.Add(("port-refused-uncompilable", name, rawcode, Trunc(why)));
                        heroLines.AppendLine($"  {rawcode} {heroName}: REFUSED (would not compile)");
                        continue;
                    }
                    if (script?.Notes.Any(n => n.Contains("repaired", StringComparison.OrdinalIgnoreCase)) == true)
                        repaired++;

                    var entry = target.GetFile("war3map.j");
                    string jass = Encoding.Latin1.GetString(entry!.OverrideBytes ?? entry.RawBytes);
                    var issues = JassScriptCheck.Check(jass);
                    if (!JassScriptCheck.IsCompilable(issues))
                    {
                        var worst = issues.First(i =>
                            i.Severity == DiagnosticSeverity.Error && JassScriptCheck.BlocksCompilation(i.Kind));
                        problemHeroes++; severity += 50;
                        failures.Add((worst.Kind.ToString(), name, rawcode, Trunc(worst.Message)));
                        heroLines.AppendLine($"  {rawcode} {heroName}: BROKEN {worst.Kind} - {Trunc(worst.Message)}");
                        continue;
                    }

                    compiled++;

                    // The script compiles, but does the hero actually work? Place it, wire the map up
                    // the way a real test map would, then audit every ability. This is what turns the
                    // sweep from "did it port" into "would it play", so the next broken pattern is
                    // found here rather than by someone launching the game.
                    PlacementCommand.PlaceUnit(target, rawcode, ownerId: 0, x: 0f, y: 0f);
                    var wiring = HeroWiringAudit.Audit(target, rawcode, ownerId: 0);
                    foreach (var g in wiring.Problems.GroupBy(x => x.Status))
                        failures.Add(($"wiring:{g.Key}", name, rawcode,
                            Trunc(string.Join(", ", g.Select(x => x.Ability)))));

                    // Score against CASTABLE abilities, not the total. Counting passives in the
                    // denominator meant a hero with zero problems still read as partial, which is
                    // the same overstating this sweep exists to avoid.
                    string fnNote = script is null ? "no script carried" : $"{script.Functions} fn";
                    string wiringNote = $"wiring {wiring.Wired}/{wiring.Abilities.Count - wiring.NotCastable}"
                        + (wiring.NotCastable > 0 ? $" +{wiring.NotCastable}passive" : "");

                    if (wiring.Problems.Count == 0)
                    {
                        cleanHeroes++;
                        heroLines.AppendLine($"  {rawcode} {heroName}: CLEAN  ({wiringNote}, {fnNote})");
                    }
                    else
                    {
                        problemHeroes++; severity += wiring.Problems.Count;
                        // Named right on the hero's own line, grouped by status, so the offending
                        // abilities are visible without cross-referencing the ranked section below.
                        string byStatus = string.Join("  ", wiring.Problems.GroupBy(p => p.Status)
                            .OrderByDescending(g => g.Count())
                            .Select(g => $"{g.Key}[{string.Join(",", g.Select(p => p.Ability))}]"));
                        heroLines.AppendLine($"  {rawcode} {heroName}: {wiring.Problems.Count} PROBLEM(S)  "
                            + $"({wiringNote}, {fnNote})\n      {byStatus}");
                    }
                }
                catch (Exception ex)
                {
                    problemHeroes++; severity += 50;
                    failures.Add((ex.GetType().Name, name, rawcode, Trunc(ex.Message)));
                    heroLines.AppendLine($"  {rawcode} {heroName}: THREW {ex.GetType().Name} - {Trunc(ex.Message)}");
                }
            }

            string verdict = heroes.Count == 0 ? "no custom heroes found"
                : problemHeroes == 0 ? $"all {cleanHeroes} clean"
                : $"{problemHeroes} of {heroes.Count} with problems";
            block.AppendLine($"## {name}  ({heroes.Count} of {allHeroes.Count} custom hero(es) sampled, {verdict})");
            if (heroes.Count == 0) block.AppendLine("  no custom heroes found");
            else block.Append(heroLines);
            block.AppendLine();
            mapBlocks.Add((severity, cleanHeroes, problemHeroes, block.ToString()));
        }

        var report = new StringBuilder();
        report.AppendLine($"# Port triage over {maps.Count} unique custom maps, worst offenders first");
        report.AppendLine();
        foreach (var (_, _, _, text) in mapBlocks.OrderByDescending(b => b.Severity))
            report.Append(text);

        report.AppendLine("# Ranked failure classes");
        foreach (var g in failures.GroupBy(f => f.Kind).OrderByDescending(g => g.Count()))
        {
            report.AppendLine($"\n## {g.Key}  ({g.Count()})");
            foreach (var f in g.Take(12))
                report.AppendLine($"  - {f.Map} {f.Hero}: {f.Detail}");
        }

        int cleanTotal = mapBlocks.Sum(b => b.CleanHeroes);
        int problemTotal = mapBlocks.Sum(b => b.ProblemHeroes);
        string summary = $"maps={maps.Count} heroes attempted={attempted} compiled={compiled} "
            + $"auto-repaired={repaired} refused={refused} broken={failures.Count} "
            + $"map-load-failed={mapErrors} clean-heroes={cleanTotal} problem-heroes={problemTotal} "
            + $"elapsed={sw.Elapsed:hh\\:mm\\:ss}";
        report.Insert(0, summary + "\n\n");

        string outPath = Path.Combine(Path.GetTempPath(), "wc3ctl-port-triage.md");
        File.WriteAllText(outPath, report.ToString());
        _out.WriteLine(summary);
        _out.WriteLine("report: " + outPath);

        // Reporting is the deliverable, so a broken hero does not fail the run. Only the guarantee
        // this work exists to enforce is asserted: a port never writes a script that cannot compile.
        var shipped = failures.Where(f => !f.Kind.StartsWith("wiring:", StringComparison.Ordinal)
                                       && !f.Kind.StartsWith("port-refused", StringComparison.Ordinal)
                                       && !f.Kind.Contains("map-load", StringComparison.Ordinal)).ToList();
        Assert.True(shipped.Count == 0,
            $"{shipped.Count} port(s) wrote a script that cannot compile, see {outPath}:\n"
            + string.Join("\n", shipped.Take(10).Select(f => $"  {f.Map} {f.Hero}: {f.Kind} {f.Detail}")));
    }

    /// <summary>Custom heroes, by the World Editor convention that a hero's rawcode starts with an
    /// uppercase letter while an ordinary unit's starts lowercase (Hpal/H000 vs hfoo/h000).</summary>
    private static IEnumerable<(string Rawcode, string Name)> Heroes(MapDocument doc)
    {
        var list = ObjectListCommand.Execute(doc, ObjectKind.Unit, gameDirOverride: null);
        return list.Items
            .Where(i => i.Rawcode.Length == 4 && char.IsUpper(i.Rawcode[0]))
            .Where(i => !string.Equals(i.Rawcode, i.BaseRawcode, StringComparison.Ordinal)) // custom, not a tweaked standard
            .Select(i => (i.Rawcode, i.Name ?? ""));
    }

    /// <summary>One map per title, newest-largest file wins, Blizzard melee maps excluded. Version
    /// suffixes vary wildly across this library, so the key is the name with trailing version-ish
    /// tokens stripped.</summary>
    private static List<string> UniqueCustomMaps(string dir)
    {
        var best = new Dictionary<string, (string Path, long Size)>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in Directory.EnumerateFiles(dir, "*.w3?", SearchOption.AllDirectories))
        {
            var ext = Path.GetExtension(path);
            if (!ext.Equals(".w3x", StringComparison.OrdinalIgnoreCase)
                && !ext.Equals(".w3m", StringComparison.OrdinalIgnoreCase)) continue;

            string file = Path.GetFileNameWithoutExtension(path);
            if (file.StartsWith("(", StringComparison.Ordinal)) continue;                 // (2)EchoIsles, ladder melee
            if (file.Contains("NewMap", StringComparison.OrdinalIgnoreCase)) continue;    // our own scratch targets
            if (file.EndsWith(".ported", StringComparison.OrdinalIgnoreCase)
                || file.EndsWith(".edited", StringComparison.OrdinalIgnoreCase)
                || file.EndsWith(".repaired", StringComparison.OrdinalIgnoreCase)) continue;

            string key = System.Text.RegularExpressions.Regex.Replace(
                file, @"[ _.\-]*[vV]?[0-9]+([._\-][0-9A-Za-z]+)*(\[[A-Za-z]\])?$", "").Trim();
            if (key.Length == 0) key = file;

            long size = new FileInfo(path).Length;
            if (!best.TryGetValue(key, out var cur) || size > cur.Size) best[key] = (path, size);
        }
        // Bounded, because loading the whole library reads gigabytes of MPQ and pushed this
        // category past a ten minute timeout. CorpusSweep.Describe reports what was left out,
        // so a sampled run never reads as a complete one.
        return CorpusSweep.Bound(
            best.Values.OrderBy(v => v.Size).Select(v => v.Path));
    }

    private static string Trunc(string s, int max = 160)
    {
        s = s.Replace('\n', ' ').Replace('\r', ' ').Trim();
        return s.Length <= max ? s : s[..max] + "...";
    }
}
