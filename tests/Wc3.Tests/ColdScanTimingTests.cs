// tests/Wc3.Tests/ColdScanTimingTests.cs
// A measurement, not a guard. Times the identifier union scan phases against the real schema set,
// so a claim that a change made it faster can be checked in the same session.
using System.Diagnostics;
using Wc3.Commands;
using Xunit.Abstractions;

namespace Wc3.Tests;

public class ColdScanTimingTests
{
    private readonly ITestOutputHelper _out;
    public ColdScanTimingTests(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void Time_the_union_scan_phases()
    {
        var dir = Path.Combine(RepoLayout.FindRoot(), "data", "schemas");
        if (!Directory.Exists(dir)) { _out.WriteLine("schemas absent, skipped"); return; }

        var builds = Directory.EnumerateDirectories(dir)
            .Where(d => File.Exists(Path.Combine(d, "manifest.json")))
            .ToList();
        _out.WriteLine($"{builds.Count} build folder(s)");

        // One build, script only, the unit the scan repeats.
        var one = Stopwatch.StartNew();
        int names = 0;
        foreach (var b in builds.Take(1))
        {
            var (build, script) = GameSchemaCommand.LoadScript(b);
            names = script?.Values.Sum(a => a.Natives.Count + a.Globals.Count
                + a.Types.Count + a.Functions.Count) ?? 0;
            _out.WriteLine($"one LoadScript({build}): {names:N0} declarations");
        }
        one.Stop();
        _out.WriteLine($"one build: {one.ElapsedMilliseconds} ms");

        var all = Stopwatch.StartNew();
        int n = 0;
        foreach (var b in builds)
        {
            var (_, script) = GameSchemaCommand.LoadScript(b);
            n += script?.Count ?? 0;
        }
        all.Stop();
        _out.WriteLine($"all {builds.Count} serial: {all.ElapsedMilliseconds:N0} ms, {n} script entries");

        TargetCompatCommand.ResetIdentifierIndexCache();
        var cold = Stopwatch.StartNew();
        var index = TargetCompatCommand.LoadIdentifierIndex(dir);
        cold.Stop();
        _out.WriteLine($"cold union scan: {cold.ElapsedMilliseconds:N0} ms, {index.Kinds.Count:N0} ids");

        var warm = Stopwatch.StartNew();
        TargetCompatCommand.LoadIdentifierIndex(dir);
        warm.Stop();
        _out.WriteLine($"warm union scan: {warm.ElapsedMilliseconds:N0} ms");

        // Drop the in-memory copy so the next call reads the on-disk cache the cold scan just
        // wrote. That is the path a fresh CLI process takes, where the whole point is not to pay
        // the parse again.
        TargetCompatCommand.ResetIdentifierIndexCache();
        var fromDisk = Stopwatch.StartNew();
        var diskIndex = TargetCompatCommand.LoadIdentifierIndex(dir);
        fromDisk.Stop();
        _out.WriteLine($"union scan from the on-disk cache: {fromDisk.ElapsedMilliseconds:N0} ms, "
            + $"{diskIndex.Kinds.Count:N0} ids");

        // The two halves of compat explain, to see which one owns the wall clock.
        TargetCompatCommand.ResetIdentifierIndexCache();
        var explain = Stopwatch.StartNew();
        var result = CompatExplainCommand.Execute("UnitAddAbility", "Beta-ROC-v1.00", dir);
        explain.Stop();
        _out.WriteLine($"CompatExplainCommand.Execute (cold index): {explain.ElapsedMilliseconds:N0} ms, "
            + $"{result.Builds.Count} builds");

        var again2 = Stopwatch.StartNew();
        CompatExplainCommand.Execute("UnitAddAbility", "Beta-ROC-v1.00", dir);
        again2.Stop();
        _out.WriteLine($"CompatExplainCommand.Execute (warm index): {again2.ElapsedMilliseconds:N0} ms");

        var schemaOnly = Stopwatch.StartNew();
        TargetBuildSettings.LoadSchema("Beta-ROC-v1.00", dir);
        schemaOnly.Stop();
        _out.WriteLine($"TargetBuildSettings.LoadSchema: {schemaOnly.ElapsedMilliseconds:N0} ms");

        _out.WriteLine($"{Environment.ProcessorCount} processor(s) visible");

        var gam = Stopwatch.StartNew();
        var schemas = GamedataSupport.LoadAll(dir);
        gam.Stop();
        _out.WriteLine($"GamedataSupport.LoadAll: {gam.ElapsedMilliseconds:N0} ms, {schemas.Count} schemas");

        var again = Stopwatch.StartNew();
        var schemas2 = GamedataSupport.LoadAll(dir);
        again.Stop();
        _out.WriteLine($"GamedataSupport.LoadAll again, uncached: {again.ElapsedMilliseconds:N0} ms");

        // Would caching the loaded set be affordable? Keep both alive so nothing is collected.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        long held = GC.GetTotalMemory(forceFullCollection: true);
        _out.WriteLine($"{schemas.Count + schemas2.Count} schemas held, managed heap "
            + $"{held / 1024 / 1024:N0} MB");
    }
}
