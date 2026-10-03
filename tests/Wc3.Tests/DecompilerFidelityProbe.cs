// tests/Wc3.Tests/DecompilerFidelityProbe.cs
using System.Reflection;
using War3Net.Build;
using War3Net.Build.Script;
using War3Net.CodeAnalysis.Decompilers;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Does the decompiler actually rebuild a trigger tree that resembles the map's own?
///
/// DecompilerApiProbe read the surface. This runs it. The claim under test is narrow and
/// falsifiable, if `TryDecompileMapTriggers` is handed a real map it should produce a
/// MapTriggers whose shape is comparable to the war3map.wtg that map already carries.
///
/// This matters before anything is wired, because the whole point of decompiling is to
/// rebuild a STALE tree from an authoritative script. A decompiler that produces a
/// plausible but different tree would silently replace a correct tree with a worse one,
/// which is precisely the failure mode this project keeps paying for.
///
/// Prints only. Nothing is asserted about quality yet, because nothing is measured yet.
/// </summary>
public class DecompilerFidelityProbe
{
    private readonly ITestOutputHelper _out;
    public DecompilerFidelityProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void Can_the_decompiler_rebuild_a_real_maps_trigger_tree()
    {
        string path = CorpusMap.PathOrEmpty;
        if (!File.Exists(path))
        {
            _out.WriteLine("no corpus map on disk, nothing measured");
            return;
        }
        _out.WriteLine($"map: {Path.GetFileName(path)}");

        // The map's OWN tree, read through the path wc3ctl already trusts.
        MapTriggers? own = null;
        try
        {
            var doc = MapDocument.Load(path);
            own = doc.GetFile("war3map.wtg")?.Model as MapTriggers;
        }
        catch (Exception ex)
        {
            _out.WriteLine($"MapDocument.Load threw {ex.GetType().Name}: {ex.Message}");
        }

        if (own is null)
        {
            _out.WriteLine("this map carries no readable war3map.wtg, so there is nothing to compare against");
        }
        else
        {
            _out.WriteLine($"own tree:  format={own.FormatVersion} sub={own.SubVersion?.ToString() ?? "none"} "
                         + $"items={own.TriggerItems.Count} vars={own.Variables.Count}");
        }

        // How do you get a War3Net Map from a path? wc3ctl never does this, it builds the
        // types by hand, so the entry point has to be discovered rather than assumed.
        var mapType = typeof(Map);
        var factories = mapType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.ReturnType == mapType)
            .ToList();
        _out.WriteLine($"\nWar3Net.Build.Map static factories returning Map ({factories.Count})");
        foreach (var m in factories)
            _out.WriteLine("  " + m.Name + "(" + string.Join(", ",
                m.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name)) + ")");

        var open = factories.FirstOrDefault(m => m.Name == "Open"
                                              && m.GetParameters().Length == 2
                                              && m.GetParameters()[0].ParameterType == typeof(string));
        if (open is null)
        {
            _out.WriteLine("\nno Map.Open(string, MapFiles) found, stopping here rather than guessing");
            return;
        }

        // MapFiles is a [Flags] enum and this probe wants everything, so take the widest
        // value the enum actually defines rather than naming a member that may not exist.
        var flagsType = open.GetParameters()[1].ParameterType;
        object allFlags = Enum.ToObject(flagsType,
            Enum.GetValues(flagsType).Cast<object>()
                .Select(Convert.ToInt64).Aggregate(0L, (a, b) => a | b));
        _out.WriteLine($"opening with {flagsType.Name} = {allFlags}");

        Map map;
        try
        {
            map = (Map)open.Invoke(null, new object?[] { path, allFlags })!;
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException ?? ex;
            _out.WriteLine($"\nMap.Open threw {inner.GetType().Name}: {inner.Message}");
            return;
        }
        _out.WriteLine($"\nMap.Open succeeded. script present={map.Script is not null}, "
                     + $"length={map.Script?.Length ?? 0:N0}");

        // TriggerData.Default is the stock table and needs no game install.
        var decompiler = new JassScriptDecompiler(map, TriggerData.Default);

        bool ok;
        MapTriggers? rebuilt;
        try
        {
            ok = decompiler.TryDecompileMapTriggers(
                own?.FormatVersion ?? MapTriggersFormatVersion.v7,
                own?.SubVersion,
                out rebuilt);
        }
        catch (Exception ex)
        {
            _out.WriteLine($"TryDecompileMapTriggers threw {ex.GetType().Name}: {ex.Message}");
            return;
        }

        _out.WriteLine($"\nTryDecompileMapTriggers returned {ok}");
        if (!ok || rebuilt is null)
        {
            _out.WriteLine("VERDICT: the decompiler declined this map. That is a clean refusal "
                         + "rather than a wrong answer, but it means the feature cannot rest on "
                         + "it until the refusal rate across the library is known.");
            return;
        }

        _out.WriteLine($"rebuilt:   format={rebuilt.FormatVersion} sub={rebuilt.SubVersion?.ToString() ?? "none"} "
                     + $"items={rebuilt.TriggerItems.Count} vars={rebuilt.Variables.Count}");

        if (own is null) return;

        int ownTrig = own.TriggerItems.OfType<TriggerDefinition>().Count();
        int newTrig = rebuilt.TriggerItems.OfType<TriggerDefinition>().Count();
        int ownCat = own.TriggerItems.OfType<TriggerCategoryDefinition>().Count();
        int newCat = rebuilt.TriggerItems.OfType<TriggerCategoryDefinition>().Count();

        _out.WriteLine($"\n{"",-14}{"own",8}{"rebuilt",10}");
        _out.WriteLine($"{"triggers",-14}{ownTrig,8}{newTrig,10}");
        _out.WriteLine($"{"categories",-14}{ownCat,8}{newCat,10}");
        _out.WriteLine($"{"variables",-14}{own.Variables.Count,8}{rebuilt.Variables.Count,10}");

        var ownNames = own.TriggerItems.OfType<TriggerDefinition>()
            .Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var newNames = rebuilt.TriggerItems.OfType<TriggerDefinition>()
            .Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        int shared = ownNames.Intersect(newNames, StringComparer.OrdinalIgnoreCase).Count();

        _out.WriteLine($"\ntrigger names, {shared} shared of {ownNames.Count} own and {newNames.Count} rebuilt");
        foreach (var missing in ownNames.Except(newNames, StringComparer.OrdinalIgnoreCase).Take(8))
            _out.WriteLine($"  only in own:     {missing}");
        foreach (var extra in newNames.Except(ownNames, StringComparer.OrdinalIgnoreCase).Take(8))
            _out.WriteLine($"  only in rebuilt: {extra}");

        double recall = ownNames.Count == 0 ? 0 : 100.0 * shared / ownNames.Count;
        _out.WriteLine($"\nname recall {recall:F1}%");
        _out.WriteLine(recall > 90
            ? "VERDICT: the rebuilt tree recovers essentially the same triggers, so a "
            + "sync-from-script verb can rest on this."
            : "VERDICT: the rebuilt tree is NOT the same tree. Replacing a map's wtg with this "
            + "would lose work, so any verb built on it must be additive or diff-first.");
    }
}
