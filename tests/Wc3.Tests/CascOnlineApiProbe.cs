// tests/Wc3.Tests/CascOnlineApiProbe.cs
using System.Reflection;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Asks the CascLib binding whether it can open a storage other than the installed one.
///
/// This matters because the install caches the PREVIOUS build's configuration alongside the
/// active one. Here that is 2.0.4.23745 beside 3.0.0.24268, and Blizzard's CDN still serves the
/// old build's config. If the binding can open an online storage at a named build, then the
/// pre-3.0.0 baseline is reachable and a real before-and-after diff becomes possible without
/// anyone having kept a snapshot in advance.
/// </summary>
public class CascOnlineApiProbe
{
    private readonly ITestOutputHelper _out;
    public CascOnlineApiProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    public void What_can_the_binding_open()
    {
        var asm = typeof(CascLib.NET.CascStorage).Assembly;
        _out.WriteLine($"assembly {asm.GetName().Name} {asm.GetName().Version}");

        foreach (var t in asm.GetExportedTypes().OrderBy(t => t.FullName, StringComparer.Ordinal))
        {
            var members = t.GetMembers(BindingFlags.Public | BindingFlags.Static
                                       | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.MemberType is MemberTypes.Method or MemberTypes.Constructor
                                         or MemberTypes.Property or MemberTypes.Field)
                .Select(m => m.ToString() ?? "")
                .Where(s => s.Length > 0)
                .ToList();
            if (members.Count == 0) continue;

            _out.WriteLine("");
            _out.WriteLine($"== {t.FullName}");
            foreach (var m in members.OrderBy(s => s, StringComparer.Ordinal).Take(24))
                _out.WriteLine("   " + m);
        }
    }
}
