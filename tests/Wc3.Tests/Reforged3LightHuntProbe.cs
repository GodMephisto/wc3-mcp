// tests/Wc3.Tests/Reforged3LightHuntProbe.cs
using System.Text;
using Wc3.GameData;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Hunts the 465 maps inside the installed 3.0.0 storage for one that actually uses the new
/// lighting, because war3map.w3l is in none of the 256 maps on this machine and a format cannot
/// be read off a file nobody has. Blizzard's own Reforged campaign is the likeliest author.
/// </summary>
public class Reforged3LightHuntProbe
{
    private readonly ITestOutputHelper _out;
    public Reforged3LightHuntProbe(ITestOutputHelper output) => _out = output;

    private const string Install = @"C:\Warcraft III";

    [Fact]
    [Trait("Category", "GameData")]
    public void Which_shipped_maps_carry_the_new_files()
    {
        if (!CascGameDataSource.TryOpen(Install, out var casc, out var error) || casc is null)
        {
            _out.WriteLine($"could not open CASC, {error}");
            return;
        }
        using var source = casc;

        var maps = source.EnumerateFileNames()
            .Where(n => n.EndsWith(".w3x", StringComparison.OrdinalIgnoreCase)
                     || n.EndsWith(".w3m", StringComparison.OrdinalIgnoreCase))
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        string[] wanted = { "war3map.w3l", "war3mappostprocessing.txt", "war3map.w3grp", "war3map.soundasset" };
        var hits = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var w in wanted) hits[w] = new List<string>();

        int read = 0, failed = 0;
        string outDir = Path.Combine(Path.GetTempPath(), "wc3ctl-casc-maps");
        Directory.CreateDirectory(outDir);

        foreach (var name in maps)
        {
            byte[]? bytes;
            try { bytes = source.ReadFile(name); } catch { failed++; continue; }
            if (bytes is null || bytes.Length == 0) { failed++; continue; }

            MapDocument doc;
            try { doc = MapDocument.Load(bytes); } catch { failed++; continue; }
            read++;

            foreach (var w in wanted)
            {
                if (!doc.Files.Any(f => string.Equals(f.FileName, w, StringComparison.OrdinalIgnoreCase)))
                    continue;
                hits[w].Add(name);
                // Keep the first carrier of each so the format can be worked on directly.
                if (hits[w].Count == 1)
                    File.WriteAllBytes(Path.Combine(outDir, SafeName(name)), bytes);
            }
        }

        _out.WriteLine($"read {read} of {maps.Count} shipped maps, {failed} unreadable");
        foreach (var w in wanted)
        {
            _out.WriteLine("");
            _out.WriteLine($"{w}: {hits[w].Count} map(s)");
            foreach (var m in hits[w].Take(12)) _out.WriteLine("   " + m);
        }
    }

    private static string SafeName(string cascPath)
    {
        var s = new StringBuilder();
        foreach (char c in cascPath)
            s.Append(Path.GetInvalidFileNameChars().Contains(c) ? '_' : c);
        return s.ToString();
    }
}
