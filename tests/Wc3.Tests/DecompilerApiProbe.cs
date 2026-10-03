// tests/Wc3.Tests/DecompilerApiProbe.cs
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// What War3Net.CodeAnalysis.Decompilers actually exposes.
///
/// The README claims it can "decompile GUI triggers and variables from JASS initialization
/// functions", which is the direction wc3ctl needs, rebuilding war3map.wtg FROM the compiled
/// war3map.j rather than the reverse. Adopting it on the strength of a README would be the
/// failure this project keeps recording, so the first step is to read the surface and the
/// second is to run it against a real map.
///
/// This probe only prints. It asserts nothing about behaviour, because there is nothing yet
/// measured to assert.
/// </summary>
public class DecompilerApiProbe
{
    private readonly ITestOutputHelper _out;
    public DecompilerApiProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    public void Print_the_decompiler_surface()
    {
        Assembly asm;
        try
        {
            asm = Assembly.Load("War3Net.CodeAnalysis.Decompilers");
        }
        catch (Exception ex)
        {
            _out.WriteLine($"could not load the assembly: {ex.GetType().Name} {ex.Message}");
            return;
        }

        _out.WriteLine($"{asm.GetName().Name} {asm.GetName().Version}");

        var types = asm.GetExportedTypes().OrderBy(t => t.FullName).ToList();
        _out.WriteLine($"{types.Count} public type(s)\n");

        foreach (var t in types)
        {
            _out.WriteLine($"=== {t.FullName}");

            // Only the members that could plausibly be an entry point. Printing every
            // property of every type buries the two methods that matter.
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance
                                     | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .OrderBy(m => m.Name);

            foreach (var m in methods)
            {
                string ps = string.Join(", ", m.GetParameters()
                    .Select(p => $"{Short(p.ParameterType)} {p.Name}"
                               + (p.IsOut ? " (out)" : "")));
                _out.WriteLine($"    {(m.IsStatic ? "static " : "")}{Short(m.ReturnType)} {m.Name}({ps})");
            }

            foreach (var c in t.GetConstructors())
            {
                string ps = string.Join(", ", c.GetParameters()
                    .Select(p => $"{Short(p.ParameterType)} {p.Name}"));
                _out.WriteLine($"    ctor({ps})");
            }
        }
    }

    private static string Short(Type t)
    {
        if (!t.IsGenericType) return t.Name;
        string b = t.Name[..t.Name.IndexOf('`')];
        return b + "<" + string.Join(", ", t.GetGenericArguments().Select(Short)) + ">";
    }
}
