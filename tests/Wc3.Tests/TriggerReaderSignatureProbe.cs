// tests/Wc3.Tests/TriggerReaderSignatureProbe.cs
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// EcaShapeProbe reported that not one of 37 maps carries a single GUI trigger function. That is
/// not credible as a fact about the maps, so it is probably a fact about the reader.
///
/// TriggerReadCommandTests already hints at why, in a comment: "serialized function bodies need
/// TriggerData-consistent parameter counts to parse back". The wtg format stores a function's
/// parameters without storing how many there are, so a reader that does not know the World
/// Editor's function table cannot tell where one function's parameters end and the next begins.
///
/// If War3Net's reader takes a TriggerData argument and wc3ctl calls the overload that does not,
/// then every GUI trigger in every map reads back with an empty function list, the Studio shows
/// "Events (0) Conditions (0) Actions (0)" for triggers that have plenty, and ECA authoring has
/// no foundation to build on. This prints the available overloads.
/// </summary>
public class TriggerReaderSignatureProbe
{
    private readonly ITestOutputHelper _out;
    public TriggerReaderSignatureProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    public void What_overloads_does_the_trigger_reader_offer()
    {
        var ext = typeof(War3Net.Build.Extensions.BinaryReaderExtensions);
        foreach (var m in ext.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Where(m => m.Name.Contains("Trigger", StringComparison.Ordinal))
                     .OrderBy(m => m.Name, StringComparer.Ordinal))
        {
            _out.WriteLine($"{m.ReturnType.Name} {m.Name}("
                         + string.Join(", ", m.GetParameters().Select(p =>
                             $"{p.ParameterType.Name} {p.Name}"
                             + (p.HasDefaultValue ? $" = {p.DefaultValue ?? "null"}" : "")))
                         + ")");
        }

        _out.WriteLine("\n--- writer side ---");
        var wext = typeof(War3Net.Build.Extensions.BinaryWriterExtensions);
        foreach (var m in wext.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Where(m => m.GetParameters().Any(p =>
                         p.ParameterType.Name.Contains("Trigger", StringComparison.Ordinal)))
                     .OrderBy(m => m.Name, StringComparer.Ordinal))
        {
            _out.WriteLine($"{m.ReturnType.Name} {m.Name}("
                         + string.Join(", ", m.GetParameters().Select(p =>
                             $"{p.ParameterType.Name} {p.Name}"
                             + (p.HasDefaultValue ? $" = {p.DefaultValue ?? "null"}" : "")))
                         + ")");
        }

        _out.WriteLine("\n--- any TriggerData type War3Net exposes ---");
        var tdType = typeof(War3Net.Build.Script.TriggerData);
        _out.WriteLine("--- TriggerData construction ---");
        foreach (var c in tdType.GetConstructors())
            _out.WriteLine("  ctor(" + string.Join(", ", c.GetParameters()
                .Select(p => $"{p.ParameterType.Name} {p.Name}")) + ")");
        foreach (var m in tdType.GetMethods(BindingFlags.Public | BindingFlags.Static
                                          | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            _out.WriteLine($"  {(m.IsStatic ? "static " : "")}{m.ReturnType.Name} {m.Name}("
                         + string.Join(", ", m.GetParameters()
                             .Select(p => $"{p.ParameterType.Name} {p.Name}")) + ")");
        foreach (var p in tdType.GetProperties())
            _out.WriteLine($"  prop {p.PropertyType.Name} {p.Name}");
    }
}
