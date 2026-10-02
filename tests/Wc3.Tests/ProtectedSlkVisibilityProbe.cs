// tests/Wc3.Tests/ProtectedSlkVisibilityProbe.cs
using System.Text;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The Reforged 3.0.0 repair finds its work by file name. A protected map strips names out of the
/// listfile, so those entries load with FileName null and the repair cannot see them at all.
///
/// That matters because protection and the SLK editors that cause the crash go together. If a
/// protected map carries its UnitUI.slk unnamed, the repair reports a clean bill of health on a
/// map that will still crash, which is the worst possible answer.
///
/// This probe reads the unnamed entries of a real protected map and asks what they actually are,
/// by content signature rather than by name.
/// </summary>
public class ProtectedSlkVisibilityProbe
{
    private readonly ITestOutputHelper _out;
    public ProtectedSlkVisibilityProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void What_is_hiding_in_the_unnamed_entries()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "BleachVsOnepiece15.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        var doc = MapDocument.Load(path);
        var unnamed = doc.Files.Where(f => f.FileName is null).ToList();
        _out.WriteLine($"{doc.Files.Count} entries, {unnamed.Count} of them unnamed");

        int slk = 0, txtLike = 0, fdf = 0, other = 0;
        foreach (var e in unnamed)
        {
            var b = e.RawBytes;
            if (b.Length == 0) { other++; continue; }
            string head = Encoding.Latin1.GetString(b, 0, Math.Min(64, b.Length));

            if (head.StartsWith("ID;", StringComparison.Ordinal))
            {
                slk++;
                _out.WriteLine($"  SLK at block {e.BlockIndex}, {b.Length:N0} bytes");
                // An SLK's header row names its columns, which is what identifies the table
                // and whether it is one the repair would have had work to do in.
                string text = Encoding.Latin1.GetString(b, 0, Math.Min(4000, b.Length));
                foreach (var key in new[] { "unitUIID", "itemID", "alias", "upgradeid", "\"file\"" })
                    if (text.Contains(key, StringComparison.OrdinalIgnoreCase))
                        _out.WriteLine($"      carries {key}");
            }
            else if (head.Contains("Frame ", StringComparison.Ordinal)) { fdf++; }
            else if (IsMostlyText(b)) { txtLike++; }
            else other++;
        }

        _out.WriteLine("");
        _out.WriteLine($"unnamed breakdown: {slk} SLK, {fdf} fdf-like, {txtLike} text-like, {other} binary/other");
        _out.WriteLine(slk == 0
            ? "no SLK is hidden behind a stripped name in this map"
            : "SLK files ARE hidden from the repair in this map");
    }

    private static bool IsMostlyText(byte[] b)
    {
        int n = Math.Min(512, b.Length), printable = 0;
        for (int i = 0; i < n; i++)
            if (b[i] == 9 || b[i] == 10 || b[i] == 13 || (b[i] >= 32 && b[i] <= 126)) printable++;
        return n > 0 && printable * 100 / n > 90;
    }
}
