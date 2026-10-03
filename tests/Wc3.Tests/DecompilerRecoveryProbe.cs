// tests/Wc3.Tests/DecompilerRecoveryProbe.cs
using System.Reflection;
using War3Net.Build;
using War3Net.Build.Script;
using War3Net.CodeAnalysis.Decompilers;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The one case where the JASS decompiler is worth having.
///
/// DecompilerRefusalSweep measured that on maps which already carry a war3map.wtg the
/// decompiler is useless and worse than useless, it returned True on RATankD_516.2 while
/// producing 1 trigger item against that map's real 626, at 0% name recall. Anything built
/// on that would replace a correct tree with garbage and report success.
///
/// But the sweep also found Angel-samurai-Z-v332A, which carries scripts\war3map.j and NO
/// trigger tree at all, and there the decompiler produced 998 items. That is the additive
/// case, a stripped map where there is no tree to lose and a browsable one is pure gain.
///
/// This probe checks the recovered items are real rather than 998 empty shells, because
/// "it produced a big number" is exactly the kind of result that reads as success and is not.
/// </summary>
public class DecompilerRecoveryProbe
{
    private readonly ITestOutputHelper _out;
    public DecompilerRecoveryProbe(ITestOutputHelper output) => _out = output;

    private static string MapPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download", "Angel-samurai-Z-v332A.w3x");

    [Fact]
    [Trait("Category", "Corpus")]
    public void Are_the_recovered_triggers_real()
    {
        if (!File.Exists(MapPath))
        {
            _out.WriteLine("map not on disk, nothing measured");
            return;
        }

        var openMethod = typeof(Map).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Open" && m.GetParameters().Length == 2
                     && m.GetParameters()[0].ParameterType == typeof(string));
        var flagsType = openMethod.GetParameters()[1].ParameterType;
        object allFlags = Enum.ToObject(flagsType,
            Enum.GetValues(flagsType).Cast<object>()
                .Select(Convert.ToInt64).Aggregate(0L, (a, b) => a | b));

        var map = (Map)openMethod.Invoke(null, new object?[] { MapPath, allFlags })!;
        bool ok = new JassScriptDecompiler(map, TriggerData.Default).TryDecompileMapTriggers(
            MapTriggersFormatVersion.v7, null, out var tree);

        _out.WriteLine($"decompile returned {ok}, tree is {(tree is null ? "null" : "present")}");
        if (!ok || tree is null) return;

        var trigs = tree.TriggerItems.OfType<TriggerDefinition>().ToList();
        var cats = tree.TriggerItems.OfType<TriggerCategoryDefinition>().ToList();

        _out.WriteLine($"{tree.TriggerItems.Count} item(s), {trigs.Count} trigger(s), "
                     + $"{cats.Count} categor(ies), {tree.Variables.Count} variable(s)");

        int named = trigs.Count(t => !string.IsNullOrWhiteSpace(t.Name));
        int withFunctions = trigs.Count(t => t.Functions is { Count: > 0 });
        int totalFunctions = trigs.Sum(t => t.Functions?.Count ?? 0);

        _out.WriteLine($"  {named} carry a name");
        _out.WriteLine($"  {withFunctions} carry at least one event, condition or action");
        _out.WriteLine($"  {totalFunctions} functions in total across the tree");

        _out.WriteLine("\nfirst 12 trigger names");
        foreach (var t in trigs.Take(12))
            _out.WriteLine($"  {t.Name,-42} functions={t.Functions?.Count ?? 0}");

        _out.WriteLine("\nfirst 8 variables");
        foreach (var v in tree.Variables.Take(8))
            _out.WriteLine($"  {v.Name,-38} type={v.Type} array={v.IsArray}");

        // A recovered tree earns its keep only if most entries carry actual content. A wall
        // of named-but-empty triggers would browse like a real tree and teach the reader
        // nothing, which is the failure this check exists to catch.
        double contentRate = trigs.Count == 0 ? 0 : 100.0 * withFunctions / trigs.Count;
        _out.WriteLine($"\n{contentRate:F0}% of recovered triggers carry content");
        _out.WriteLine(contentRate > 50
            ? "VERDICT: the recovery is substantive, so a recover-from-script verb is worth "
            + "building FOR MAPS WITH NO TREE ONLY."
            : "VERDICT: the recovered tree is mostly empty shells and would mislead a reader.");
    }
}
