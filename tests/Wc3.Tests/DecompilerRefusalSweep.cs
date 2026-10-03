// tests/Wc3.Tests/DecompilerRefusalSweep.cs
using System.Reflection;
using War3Net.Build;
using War3Net.Build.Script;
using War3Net.CodeAnalysis.Decompilers;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// How often does the JASS decompiler actually work, and when it refuses, why.
///
/// DecompilerFidelityProbe ran it on one map and got a clean False. One map answers nothing,
/// so this sweeps the library. The decision it informs is whether a `trigger sync-from-script`
/// verb can rest on this library at all.
///
/// The refusal reason is not returned by the API, so it is inferred from the script itself.
/// A GUI-authored map's compiled script carries `InitGlobals`, `InitCustomTriggers` and
/// `RunInitializationTriggers` by those exact names, because the World Editor emits them.
/// A map whose script was hand-written, optimised or obfuscated will not have them, and a
/// decompiler that keys on them cannot possibly succeed. Distinguishing "the decompiler is
/// weak" from "this map was never GUI-authored" is the whole point, because they call for
/// completely different decisions.
/// </summary>
public class DecompilerRefusalSweep
{
    private readonly ITestOutputHelper _out;
    public DecompilerRefusalSweep(ITestOutputHelper output) => _out = output;

    private static readonly string[] WeMarkers =
        { "InitGlobals", "InitCustomTriggers", "RunInitializationTriggers" };

    private static IEnumerable<string> Maps()
    {
        string[] folders =
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Warcraft III", "Maps", "Download"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Warcraft III", "Maps"),
        };
        foreach (var f in folders)
        {
            if (!Directory.Exists(f)) continue;
            foreach (var p in Directory.EnumerateFiles(f, "*.w3?", SearchOption.TopDirectoryOnly)
                         .OrderBy(p => new FileInfo(p).Length))
                yield return p;
        }
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void How_often_does_the_decompiler_succeed_and_why_does_it_refuse()
    {
        var openMethod = typeof(Map).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Open" && m.GetParameters().Length == 2
                     && m.GetParameters()[0].ParameterType == typeof(string));
        var flagsType = openMethod.GetParameters()[1].ParameterType;
        object allFlags = Enum.ToObject(flagsType,
            Enum.GetValues(flagsType).Cast<object>()
                .Select(Convert.ToInt64).Aggregate(0L, (a, b) => a | b));

        int seen = 0, opened = 0, succeeded = 0, refused = 0, threw = 0;
        int refusedWithMarkers = 0, refusedWithoutMarkers = 0;
        var recalls = new List<double>();

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 120L * 1024 * 1024) continue;
            seen++;

            MapTriggers? own = null;
            try
            {
                var doc = MapDocument.Load(path);
                own = doc.GetFile("war3map.wtg")?.Model as MapTriggers;
            }
            catch { /* reported below via the opened counter */ }

            Map map;
            try
            {
                map = (Map)openMethod.Invoke(null, new object?[] { path, allFlags })!;
            }
            catch
            {
                continue;
            }
            opened++;

            string script = map.Script ?? string.Empty;
            int markers = WeMarkers.Count(m => script.Contains(m, StringComparison.Ordinal));

            bool ok = false;
            MapTriggers? rebuilt = null;
            try
            {
                ok = new JassScriptDecompiler(map, TriggerData.Default).TryDecompileMapTriggers(
                    own?.FormatVersion ?? MapTriggersFormatVersion.v7, own?.SubVersion, out rebuilt);
            }
            catch (Exception ex)
            {
                threw++;
                _out.WriteLine($"{Trim(path),-42} THREW {ex.GetType().Name}");
                continue;
            }

            if (ok && rebuilt is not null)
            {
                succeeded++;
                double recall = -1;
                if (own is not null)
                {
                    var a = own.TriggerItems.OfType<TriggerDefinition>()
                        .Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var b = rebuilt.TriggerItems.OfType<TriggerDefinition>()
                        .Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    if (a.Count > 0)
                    {
                        recall = 100.0 * a.Intersect(b, StringComparer.OrdinalIgnoreCase).Count() / a.Count;
                        recalls.Add(recall);
                    }
                }
                _out.WriteLine($"{Trim(path),-42} OK    markers={markers}/3 "
                             + $"own={own?.TriggerItems.Count ?? -1,-5} rebuilt={rebuilt.TriggerItems.Count,-5} "
                             + $"recall={(recall < 0 ? "n/a" : recall.ToString("F0") + "%")}");
            }
            else
            {
                refused++;
                if (markers == WeMarkers.Length) refusedWithMarkers++;
                else refusedWithoutMarkers++;
                _out.WriteLine($"{Trim(path),-42} refused markers={markers}/3 "
                             + $"own={own?.TriggerItems.Count ?? -1}");
            }
        }

        _out.WriteLine($"\n{seen} map(s) seen, {opened} opened by War3Net, {threw} threw");
        _out.WriteLine($"{succeeded} decompiled, {refused} refused");
        _out.WriteLine($"  of the refusals, {refusedWithMarkers} DID carry all three World Editor "
                     + $"markers and {refusedWithoutMarkers} did not");
        if (recalls.Count > 0)
            _out.WriteLine($"  name recall on successes, min {recalls.Min():F0}% "
                         + $"median {Median(recalls):F0}% max {recalls.Max():F0}%");

        _out.WriteLine("\nreading:");
        _out.WriteLine(refusedWithMarkers == 0
            ? "  Every refusal was a map whose script lacks the World Editor's own init "
            + "functions, so the decompiler is refusing exactly the maps it cannot know "
            + "anything about. That is correct behaviour and NOT a weakness."
            : $"  {refusedWithMarkers} map(s) carried all three markers and were still refused, "
            + "so the decompiler is leaving recoverable trees on the table.");
    }

    private static double Median(List<double> xs)
    {
        var s = xs.OrderBy(x => x).ToList();
        return s.Count % 2 == 1 ? s[s.Count / 2] : (s[s.Count / 2 - 1] + s[s.Count / 2]) / 2.0;
    }

    private static string Trim(string p)
    {
        string n = Path.GetFileName(p);
        return n.Length <= 40 ? n : n[..40];
    }
}
