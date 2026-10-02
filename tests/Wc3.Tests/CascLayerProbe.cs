// tests/Wc3.Tests/CascLayerProbe.cs
using Wc3.GameData;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Reports the shape of the installed storage, so a per-patch snapshot can be taken of the right
/// things rather than of everything. Also answers whether an older data layer survives in the
/// install, which would allow a before-and-after comparison without having kept a baseline.
/// </summary>
public class CascLayerProbe
{
    private const string Install = @"C:\Warcraft III";
    private readonly ITestOutputHelper _out;
    public CascLayerProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "GameData")]
    public void What_layers_and_trees_does_the_install_have()
    {
        if (!CascGameDataSource.TryOpen(Install, out var casc, out _) || casc is null)
        {
            _out.WriteLine("no install");
            return;
        }
        using var source = casc;
        var names = source.EnumerateFileNames().ToList();
        _out.WriteLine($"{names.Count:N0} names");

        // The w3mod layer prefix, everything before the first colon.
        _out.WriteLine("");
        _out.WriteLine("mod layers");
        foreach (var g in names.Select(n => n.Contains(':') ? n[..(n.IndexOf(':') + 1)] : "(none)")
                     .GroupBy(p => p, StringComparer.OrdinalIgnoreCase)
                     .OrderByDescending(g => g.Count()))
            _out.WriteLine($"  {g.Count(),7:N0}  {g.Key}");

        // Top-level directory inside the main layer, which is what a snapshot would track.
        _out.WriteLine("");
        _out.WriteLine("top-level trees under war3.w3mod:");
        foreach (var g in names
                     .Where(n => n.StartsWith("war3.w3mod:", StringComparison.OrdinalIgnoreCase))
                     .Select(n => n["war3.w3mod:".Length..])
                     .Select(r => r.Contains('\\') ? r[..r.IndexOf('\\')] : "(root)")
                     .GroupBy(d => d, StringComparer.OrdinalIgnoreCase)
                     .OrderByDescending(g => g.Count()).Take(20))
            _out.WriteLine($"  {g.Count(),7:N0}  {g.Key}");

        _out.WriteLine("");
        _out.WriteLine("the text and data files a patch diff would care about");
        foreach (var ext in new[] { ".j", ".lua", ".slk", ".txt", ".fdf", ".ai" })
            _out.WriteLine($"  {names.Count(n => n.EndsWith(ext, StringComparison.OrdinalIgnoreCase)),7:N0}  *{ext}");
    }
}
