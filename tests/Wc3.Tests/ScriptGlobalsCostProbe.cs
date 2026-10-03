// tests/Wc3.Tests/ScriptGlobalsCostProbe.cs
using System.Diagnostics;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// What does reading the globals block actually cost, on the largest maps that exist here.
///
/// The trigger panel now calls ScriptGlobalsCommand.Count on every render, so it can say where
/// a map's variables live instead of showing no Variables section at all. That call decodes
/// the whole compiled script and walks it line by line. On GGGA_V0.05 that is a 12.4MB script
/// of 240,332 lines inside a 277MB archive, and on a panel render path a cost like that is not
/// a detail, it is the difference between a panel and a freeze.
///
/// Measuring it is the point. A fix that trades a blank section for a stall is not a fix.
/// </summary>
public class ScriptGlobalsCostProbe
{
    private readonly ITestOutputHelper _out;
    public ScriptGlobalsCostProbe(ITestOutputHelper output) => _out = output;

    private static IEnumerable<string> BigMaps()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        if (!Directory.Exists(dir)) yield break;
        foreach (var p in Directory.EnumerateFiles(dir, "*.w3?")
                     .OrderByDescending(p => new FileInfo(p).Length)
                     .Take(4))
            yield return p;
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void How_long_does_a_globals_read_take_on_the_biggest_maps()
    {
        foreach (var path in BigMaps())
        {
            long size = new FileInfo(path).Length;
            MapDocument doc;
            var load = Stopwatch.StartNew();
            try { doc = MapDocument.Load(path); }
            catch (Exception ex)
            {
                _out.WriteLine($"{Name(path),-40} load threw {ex.GetType().Name}");
                continue;
            }
            load.Stop();

            // First call, cold. This is what a panel render pays.
            var first = Stopwatch.StartNew();
            int n = ScriptGlobalsCommand.Count(doc);
            first.Stop();

            // Second call, to show whether anything is cached. Nothing is, at time of writing,
            // which is exactly what makes a per-render call worth questioning.
            var second = Stopwatch.StartNew();
            ScriptGlobalsCommand.Count(doc);
            second.Stop();

            _out.WriteLine(
                $"{Name(path),-40} {size / 1024 / 1024,5}MB  load {load.ElapsedMilliseconds,6}ms  "
                + $"globals {n,6}  first {first.ElapsedMilliseconds,5}ms  "
                + $"second {second.ElapsedMilliseconds,5}ms");
        }

        _out.WriteLine("\nA panel render must stay responsive, so anything above roughly 100ms "
                     + "here needs caching rather than a note in a comment.");
    }

    private static string Name(string p)
    {
        string n = Path.GetFileName(p);
        return n.Length <= 38 ? n : n[..38];
    }
}
