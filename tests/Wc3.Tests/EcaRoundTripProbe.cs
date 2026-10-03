// tests/Wc3.Tests/EcaRoundTripProbe.cs
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The experiment that decides how ECA authoring has to be built.
///
/// War3Net offers ReadMapTriggers(reader) and ReadMapTriggers(reader, TriggerData). wc3ctl calls
/// the first. The wtg format does not record how many parameters a function takes, so a reader
/// without the World Editor's function table cannot know where one function ends. That is fine
/// for every map in this library, because they are optimizer-stripped and their GUI triggers
/// genuinely hold no functions (which is also why all 14 wtg files round-trip byte for byte).
///
/// It stops being fine the moment wc3ctl WRITES a function. An action nobody can read back is
/// worse than no action at all, because the map still saves and the loss shows up later.
///
/// So: build a trigger carrying real functions, write it with the same writer TriggerCommand
/// uses, and read it back through the parser wc3ctl actually wires. Whatever comes back is the
/// answer.
/// </summary>
public class EcaRoundTripProbe
{
    private readonly ITestOutputHelper _out;
    public EcaRoundTripProbe(ITestOutputHelper output) => _out = output;

    private static MapTriggers WithFunctions()
    {
        var t = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
        t.TriggerItems.Add(new TriggerCategoryDefinition
        { Id = 0x02000001, Name = "Cats", ParentId = -1 });

        var trig = new TriggerDefinition(TriggerItemType.Gui)
        {
            Id = 0x03000001, Name = "Has functions", Description = string.Empty,
            ParentId = 0x02000001, IsEnabled = true, IsInitiallyOn = true,
        };

        // An event with no parameters, the simplest possible case.
        trig.Functions.Add(new TriggerFunction
        {
            Type = TriggerFunctionType.Event, Name = "MapInitializationEvent", IsEnabled = true,
        });

        // An action with one string parameter, the next simplest.
        var action = new TriggerFunction
        {
            Type = TriggerFunctionType.Action, Name = "DisplayTextToForce", IsEnabled = true,
        };
        action.Parameters.Add(new TriggerFunctionParameter
        { Type = TriggerFunctionParameterType.String, Value = "hello" });
        trig.Functions.Add(action);

        t.TriggerItems.Add(trig);
        foreach (var type in Enum.GetValues<TriggerItemType>())
            t.TriggerItemCounts[type] = t.TriggerItems.Count(i => i.Type == type);
        return t;
    }

    [Fact]
    public void Does_a_written_function_survive_the_parser_wc3ctl_wires()
    {
        var original = WithFunctions();
        var target = original.TriggerItems.OfType<TriggerDefinition>().Single();
        _out.WriteLine($"wrote '{target.Name}' with {target.Functions.Count} function(s): "
                     + string.Join(", ", target.Functions.Select(f => $"{f.Type} {f.Name}"
                                                                   + $"({f.Parameters.Count}p)")));

        byte[] bytes;
        try { bytes = TriggerCommand.Serialize(original); }
        catch (Exception ex)
        {
            _out.WriteLine($"WRITE FAILED: {ex.GetType().Name}: {ex.Message}");
            return;
        }
        _out.WriteLine($"serialized to {bytes.Length} bytes");

        // Route 1: the parser wc3ctl actually wires, with no TriggerData.
        try
        {
            using var ms = new MemoryStream(bytes);
            using var r = new BinaryReader(ms);
            var back = r.ReadMapTriggers();
            var td = back.TriggerItems.OfType<TriggerDefinition>().SingleOrDefault();
            _out.WriteLine(td is null
                ? "  no TriggerData: the trigger did not come back"
                : $"  no TriggerData: '{td.Name}' came back with {td.Functions.Count} function(s)");
            if (td is not null)
                foreach (var f in td.Functions)
                    _out.WriteLine($"      {f.Type} {f.Name} ({f.Parameters.Count} param(s))");
            _out.WriteLine($"  consumed {ms.Position} of {bytes.Length} bytes");
        }
        catch (Exception ex)
        {
            _out.WriteLine($"  no TriggerData: THREW {ex.GetType().Name}: {ex.Message}");
            _out.WriteLine("  => authoring an ECA would produce a map wc3ctl can no longer read.");
        }

        // Route 2: through MapDocument, which is what every front-end uses.
        try
        {
            var doc = BlankMap.Create();
            doc.AddOrReplaceRawFile("war3map.wtg", bytes);
            var reloaded = MapDocument.Load(doc.SaveToBytes());
            var model = reloaded.GetFile("war3map.wtg")?.Model as MapTriggers;
            _out.WriteLine(model is null
                ? "  via MapDocument: the wtg did not parse at all (kept as raw)"
                : $"  via MapDocument: {model.TriggerItems.Count} item(s), "
                  + $"{model.TriggerItems.OfType<TriggerDefinition>().Sum(d => d.Functions.Count)} "
                  + "function(s)");

            var read = TriggerReadCommand.GetTriggers(reloaded);
            _out.WriteLine($"  via TriggerReadCommand: {read.Triggers.Count} trigger(s), "
                         + $"{read.Triggers.Sum(t => t.Functions.Count)} function(s)");
        }
        catch (Exception ex)
        {
            _out.WriteLine($"  via MapDocument: THREW {ex.GetType().Name}: {ex.Message}");
        }
    }

    [Fact]
    public void Does_the_built_in_TriggerData_read_them_back()
    {
        // War3Net ships the stock World-Editor function table as TriggerData.Default, with no
        // constructor and no loader, so this needs no game install and no reference from
        // Wc3.MapDocument to Wc3.GameData. That is what makes wiring it into the parser cheap.
        var bytes = TriggerCommand.Serialize(WithFunctions());
        using var ms = new MemoryStream(bytes);
        using var r = new BinaryReader(ms);

        MapTriggers back;
        try { back = r.ReadMapTriggers(TriggerData.Default); }
        catch (Exception ex)
        {
            _out.WriteLine($"THREW {ex.GetType().Name}: {ex.Message}");
            return;
        }

        var td = back.TriggerItems.OfType<TriggerDefinition>().Single();
        _out.WriteLine($"'{td.Name}' came back with {td.Functions.Count} function(s), "
                     + $"consumed {ms.Position} of {bytes.Length} bytes");
        foreach (var f in td.Functions)
            _out.WriteLine($"   {f.Type} {f.Name} ({f.Parameters.Count} param(s))"
                         + string.Concat(f.Parameters.Select(p => $" [{p.Type}='{p.Value}']")));

        Assert.Equal(2, td.Functions.Count);
        Assert.Equal("MapInitializationEvent", td.Functions[0].Name);
        Assert.Equal("DisplayTextToForce", td.Functions[1].Name);
        Assert.Equal("hello", td.Functions[1].Parameters.Single().Value);
        Assert.Equal(bytes.Length, ms.Position);
    }

    [Fact]
    public void What_arity_does_the_table_declare_and_does_matching_it_fix_the_round_trip()
    {
        // The previous test failed the same way with and without TriggerData, which rules the
        // reader out and points at the writer. The wtg format stores a function's parameters but
        // NOT how many there are, so the count is taken from the table on read. A function written
        // with the wrong number of parameters therefore makes the reader run off the end of the
        // stream, which is exactly the error seen.
        var td = TriggerData.Default;
        _out.WriteLine($"table: {td.TriggerEvents.Count} event(s), "
                     + $"{td.TriggerConditions.Count} condition(s), "
                     + $"{td.TriggerActions.Count} action(s), {td.TriggerCalls.Count} call(s)");

        foreach (var name in new[] { "MapInitializationEvent", "DisplayTextToForce" })
        {
            var found = Lookup(td, name);
            _out.WriteLine($"  {name}: {(found is null ? "NOT FOUND" : found)}");
        }
    }

    /// <summary>Prints a function's declared argument types, whichever table it lives in.</summary>
    private static string? Lookup(TriggerData td, string name)
    {
        foreach (var (label, table) in new (string, System.Collections.IDictionary)[]
                 {
                     ("event", td.TriggerEvents),
                     ("condition", td.TriggerConditions),
                     ("action", td.TriggerActions),
                     ("call", td.TriggerCalls),
                 })
        {
            if (!table.Contains(name)) continue;
            var entry = table[name];
            if (entry is null) return $"{label}, (null entry)";
            var props = entry.GetType().GetProperties()
                .Select(p => $"{p.Name}={Render(p.GetValue(entry))}");
            return $"{label}, {string.Join(", ", props)}";
        }
        return null;
    }

    private static string Render(object? v) => v switch
    {
        null => "null",
        string s => s,
        System.Collections.IEnumerable e when v is not string =>
            "[" + string.Join(" ", e.Cast<object?>().Select(x => x?.ToString() ?? "null")) + "]",
        _ => v.ToString() ?? "null",
    };
}
