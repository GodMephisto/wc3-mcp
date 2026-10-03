// tests/Wc3.Tests/TriggerModelShapeProbe.cs
using System.Reflection;
using War3Net.Build.Script;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Prints the exact shape of the War3Net trigger model, so adding an item sets every field the
/// writer reads instead of the fields I assumed it reads.
///
/// The specific unknown is MapTriggers.TriggerItemCounts. If the writer emits those stored counts
/// rather than recomputing them, adding an item without updating the array writes a header that
/// disagrees with the body, and the corruption would only show up in the World Editor. Rather than
/// decompile the writer to find out, the plan is to keep the array correct either way, which needs
/// its element type and indexing to be known rather than guessed.
/// </summary>
public class TriggerModelShapeProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerModelShapeProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    public void Print_the_trigger_model_shape()
    {
        foreach (var t in new[]
                 {
                     typeof(MapTriggers), typeof(TriggerItem), typeof(TriggerDefinition),
                     typeof(TriggerCategoryDefinition), typeof(VariableDefinition),
                     typeof(CustomTextTrigger), typeof(MapCustomTextTriggers),
                 })
        {
            _out.WriteLine($"=== {t.Name} ===");
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                _out.WriteLine($"  {p.PropertyType.Name,-28} {p.Name,-28} "
                             + $"{(p.CanRead ? "get" : "")}{(p.CanWrite ? " set" : "")}");
            foreach (var c in t.GetConstructors())
                _out.WriteLine("  ctor(" + string.Join(", ",
                    c.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}"
                        + (x.HasDefaultValue ? $" = {x.DefaultValue}" : " (required)"))) + ")");
        }

        // What a default-constructed item actually is, since the add path will use exactly this.
        _out.WriteLine("=== defaults from a parameterless construction ===");
        var defCat = new TriggerCategoryDefinition();
        var defTrig = new TriggerDefinition();
        _out.WriteLine($"  TriggerCategoryDefinition().Type = {defCat.Type}");
        _out.WriteLine($"  TriggerDefinition().Type         = {defTrig.Type}");
        _out.WriteLine($"  TriggerDefinition() flags: enabled={defTrig.IsEnabled}, "
                     + $"initiallyOn={defTrig.IsInitiallyOn}, mapInit={defTrig.RunOnMapInit}, "
                     + $"comment={defTrig.IsComment}, customText={defTrig.IsCustomTextTrigger}, "
                     + $"name={defTrig.Name ?? "(null)"}, desc={defTrig.Description ?? "(null)"}, "
                     + $"functions={defTrig.Functions?.Count.ToString() ?? "(null)"}");

        _out.WriteLine("=== TriggerItemType values ===");
        foreach (var v in Enum.GetValues<TriggerItemType>())
            _out.WriteLine($"  {Convert.ToInt32(v),4} {v}");
    }
}
