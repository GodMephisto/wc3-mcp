// tests/Wc3.Tests/EcaShapeProbe.cs
using System.Reflection;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Everything authoring an event, condition or action needs to know before a line of it is
/// written, measured rather than assumed. Three sessions' worth of evidence says a format rule
/// taken from one map, or from a model's property names, is wrong about as often as it is right.
///
/// Four questions:
///   1. What does a real TriggerFunction look like, field by field, including its parameters?
///   2. What parameter TYPES actually occur in real maps, and how is each one populated? A
///      parameter can be a literal, a preset, a variable, or a nested function call, and building
///      the wrong shape produces a trigger the World Editor cannot open.
///   3. Does the wtg writer survive a function being ADDED? Trigger items needed their count
///      dictionary maintained by hand; functions may have the same trap.
///   4. Do the parameter COUNTS have to match what TriggerData.txt declares for the function?
/// </summary>
public class EcaShapeProbe
{
    private readonly ITestOutputHelper _out;
    public EcaShapeProbe(ITestOutputHelper output) => _out = output;

    private static IEnumerable<string> Maps()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        if (!Directory.Exists(dir)) yield break;
        foreach (var p in Directory.EnumerateFiles(dir, "*.w3?")
                     .OrderBy(p => new FileInfo(p).Length))
            yield return p;
    }

    [Fact]
    public void Print_the_function_and_parameter_model_shape()
    {
        foreach (var t in new[] { typeof(TriggerFunction), typeof(TriggerFunctionParameter) })
        {
            _out.WriteLine($"=== {t.Name} ===");
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                _out.WriteLine($"  {p.PropertyType.Name,-32} {p.Name,-22} "
                             + $"{(p.CanWrite ? "get set" : "get")}");
            foreach (var c in t.GetConstructors())
                _out.WriteLine("  ctor(" + string.Join(", ", c.GetParameters()
                    .Select(x => $"{x.ParameterType.Name} {x.Name}"
                               + (x.HasDefaultValue ? $" = {x.DefaultValue}" : " (required)"))) + ")");
        }

        _out.WriteLine("=== TriggerFunctionType ===");
        foreach (var v in Enum.GetValues<TriggerFunctionType>())
            _out.WriteLine($"  {Convert.ToInt32(v),4} {v}");
        _out.WriteLine("=== TriggerFunctionParameterType ===");
        foreach (var v in Enum.GetValues<TriggerFunctionParameterType>())
            _out.WriteLine($"  {Convert.ToInt32(v),4} {v}");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Which_maps_actually_carry_gui_functions_and_what_do_they_look_like()
    {
        string? richest = null;
        int most = 0;

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;
            MapTriggers? wtg;
            try { wtg = MapDocument.Load(path).GetFile("war3map.wtg")?.Model as MapTriggers; }
            catch { continue; }
            if (wtg is null) continue;

            int fns = wtg.TriggerItems.OfType<TriggerDefinition>().Sum(d => d.Functions.Count);
            if (fns == 0) continue;
            _out.WriteLine($"{Path.GetFileName(path),-46} {fns} top-level function(s)");
            if (fns > most) { most = fns; richest = path; }
        }

        if (richest is null) { _out.WriteLine("no map carries GUI functions"); return; }
        _out.WriteLine($"\nrichest: {Path.GetFileName(richest)} with {most}\n");

        var doc = MapDocument.Load(richest);
        var triggers = (MapTriggers)doc.GetFile("war3map.wtg")!.Model!;

        // Parameter-type census, which is what says whether a builder must handle nesting.
        var typeCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        int nested = 0, withArrayIndex = 0, totalParams = 0;
        foreach (var f in triggers.TriggerItems.OfType<TriggerDefinition>().SelectMany(d => d.Functions))
            Census(f, typeCounts, ref nested, ref withArrayIndex, ref totalParams);

        _out.WriteLine($"parameter census: {totalParams} parameter(s), "
                     + $"{nested} carrying a nested function call, "
                     + $"{withArrayIndex} carrying an array index");
        foreach (var kv in typeCounts.OrderByDescending(k => k.Value))
            _out.WriteLine($"  {kv.Key,-16} {kv.Value}");

        // And three real functions in full, so the shape is concrete rather than described.
        int shown = 0;
        foreach (var d in triggers.TriggerItems.OfType<TriggerDefinition>())
        {
            foreach (var f in d.Functions)
            {
                if (shown++ >= 3) return;
                _out.WriteLine($"\n[{d.Name}] {Describe(f, 0)}");
            }
        }
    }

    private static void Census(TriggerFunction f, Dictionary<string, int> counts,
        ref int nested, ref int withArrayIndex, ref int total)
    {
        foreach (var p in f.Parameters)
        {
            total++;
            string k = p.Type.ToString();
            counts[k] = counts.TryGetValue(k, out int n) ? n + 1 : 1;
            if (p.Function is not null) nested++;
            if (p.ArrayIndexer is not null) withArrayIndex++;
            if (p.Function is not null)
                Census(p.Function, counts, ref nested, ref withArrayIndex, ref total);
        }
        foreach (var c in f.ChildFunctions)
            Census(c, counts, ref nested, ref withArrayIndex, ref total);
    }

    private static string Describe(TriggerFunction f, int depth)
    {
        string pad = new(' ', depth * 2);
        var s = new System.Text.StringBuilder();
        s.Append($"{pad}{f.Type} {f.Name} enabled={f.IsEnabled} "
               + $"params={f.Parameters.Count} children={f.ChildFunctions.Count}");
        foreach (var p in f.Parameters)
        {
            s.Append($"\n{pad}  param {p.Type} value='{p.Value}'");
            if (p.Function is not null)
                s.Append($"\n{Describe(p.Function, depth + 2)}");
        }
        foreach (var c in f.ChildFunctions)
            s.Append($"\n{Describe(c, depth + 1)}");
        return s.ToString();
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Can_the_writer_survive_an_added_function()
    {
        // The question that gates the whole feature. Adding a trigger ITEM needed its count
        // dictionary maintained by hand or the writer emitted a header disagreeing with the body.
        // Functions may carry the same trap, and it would look identical: fine here, broken in
        // the World Editor.
        string? path = Maps().FirstOrDefault(p =>
        {
            try
            {
                var m = MapDocument.Load(p).GetFile("war3map.wtg")?.Model as MapTriggers;
                return m?.TriggerItems.OfType<TriggerDefinition>()
                        .Any(d => !WctPairing.IsCustomText(d)) == true;
            }
            catch { return false; }
        });
        if (path is null) { _out.WriteLine("no map with a GUI trigger, skipped"); return; }

        var doc = MapDocument.Load(path);
        var triggers = (MapTriggers)doc.GetFile("war3map.wtg")!.Model!;
        var target = triggers.TriggerItems.OfType<TriggerDefinition>()
            .First(d => !WctPairing.IsCustomText(d));

        _out.WriteLine($"{Path.GetFileName(path)} / '{target.Name}' has "
                     + $"{target.Functions.Count} function(s)");

        var added = new TriggerFunction
        {
            Type = TriggerFunctionType.Action,
            Name = "DoNothing",
            IsEnabled = true,
        };
        target.Functions.Add(added);

        byte[] bytes;
        try { bytes = TriggerCommand.Serialize(triggers); }
        catch (Exception ex)
        {
            _out.WriteLine($"WRITE FAILED: {ex.GetType().Name}: {ex.Message}");
            _out.WriteLine("=> ECA authoring needs writer work first.");
            return;
        }
        doc.AddOrReplaceRawFile("war3map.wtg", bytes);

        try
        {
            var reloaded = MapDocument.Load(doc.SaveToBytes());
            if (reloaded.GetFile("war3map.wtg")?.Model is not MapTriggers back)
            { _out.WriteLine("REREAD FAILED: the written wtg no longer parses."); return; }

            var same = back.TriggerItems.OfType<TriggerDefinition>()
                .FirstOrDefault(d => d.Name == target.Name);
            _out.WriteLine(same is null
                ? "VERDICT: the trigger vanished."
                : $"VERDICT: reread '{same.Name}' with {same.Functions.Count} function(s), "
                  + $"last is '{same.Functions.LastOrDefault()?.Name ?? "(none)"}'");
            _out.WriteLine(same?.Functions.Any(f => f.Name == "DoNothing") == true
                ? "         the added action SURVIVED, so the writer tracks function counts."
                : "         the added action did NOT survive.");
        }
        catch (Exception ex)
        {
            _out.WriteLine($"RELOAD FAILED: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
