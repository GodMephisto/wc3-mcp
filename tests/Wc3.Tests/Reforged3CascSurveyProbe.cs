// tests/Wc3.Tests/Reforged3CascSurveyProbe.cs
using System.Text;
using Wc3.GameData;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Surveys the installed 3.0.0 storage for the things the new editor features must be backed by.
/// The point is to find a map that actually uses the new lighting, because war3map.w3l appears in
/// none of the 256 maps on this machine and cannot be reverse engineered from a file nobody has.
/// </summary>
public class Reforged3CascSurveyProbe
{
    private readonly ITestOutputHelper _out;
    public Reforged3CascSurveyProbe(ITestOutputHelper output) => _out = output;

    private const string Install = @"C:\Warcraft III";

    [Fact]
    [Trait("Category", "GameData")]
    public void Survey_the_storage_for_maps_and_the_new_feature_files()
    {
        if (!CascGameDataSource.TryOpen(Install, out var casc, out var error) || casc is null)
        {
            _out.WriteLine($"could not open CASC, {error}");
            return;
        }
        using var source = casc;

        var names = source.EnumerateFileNames().ToList();
        _out.WriteLine($"storage knows {names.Count:N0} names");

        var maps = names.Where(n => n.EndsWith(".w3x", StringComparison.OrdinalIgnoreCase)
                                 || n.EndsWith(".w3m", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
        _out.WriteLine($"maps: {maps.Count}");
        foreach (var m in maps.Take(60)) _out.WriteLine("  " + m);

        _out.WriteLine("");
        _out.WriteLine("post-processing and lighting files");
        foreach (var n in names.Where(n =>
                     n.Contains("postprocess", StringComparison.OrdinalIgnoreCase)
                     || n.Contains("lighting", StringComparison.OrdinalIgnoreCase)
                     || n.EndsWith(".w3l", StringComparison.OrdinalIgnoreCase)
                     || n.Contains("terrainlight", StringComparison.OrdinalIgnoreCase))
                 .OrderBy(n => n, StringComparer.OrdinalIgnoreCase).Take(40))
            _out.WriteLine("  " + n);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Read_the_post_processing_schema()
    {
        if (!CascGameDataSource.TryOpen(Install, out var casc, out var error) || casc is null)
        {
            _out.WriteLine($"could not open CASC, {error}");
            return;
        }
        using var source = casc;

        // The editor names these directly, so if they ship they carry the exact key spelling and
        // the defaults, which is a better schema than anything inferred from a UI string list.
        foreach (var candidate in source.EnumerateFileNames()
                     .Where(n => n.Contains("postprocess", StringComparison.OrdinalIgnoreCase))
                     .Take(6))
        {
            var bytes = source.ReadFile(candidate);
            if (bytes is null || bytes.Length == 0) { _out.WriteLine($"{candidate} unreadable"); continue; }
            _out.WriteLine($"===== {candidate}  ({bytes.Length:N0} bytes) =====");
            string text = Encoding.UTF8.GetString(bytes);
            foreach (var line in text.Split('\n').Take(60))
                _out.WriteLine("  " + line.TrimEnd('\r'));
        }
    }
}
