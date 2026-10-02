// tests/Wc3.Tests/DuplicateSaveProbe.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Follows one duplicated file through load, repair and save, printing what each slot holds at
/// every step. Written because two plausible fixes in a row failed while the counter stayed at
/// exactly 973, which means the assumption underneath both of them was wrong rather than the code.
/// </summary>
public class DuplicateSaveProbe
{
    private const string Name = @"Units\CampaignUnitStrings.txt";
    private readonly ITestOutputHelper _out;
    public DuplicateSaveProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void Follow_one_duplicated_file_through_a_repair()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "Naruto Autobattle ENGv3ch11.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        var doc = MapDocument.Load(path);
        Dump("LOADED", doc);

        var r = Reforged3RepairCommand.Execute(doc, apply: true);
        _out.WriteLine($"repair reported buttons={r.ButtonPositionsCompleted} changed={r.ChangedFiles.Count}");
        Dump("AFTER APPLY, in memory", doc);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Dump("AFTER SAVE AND RELOAD", reloaded);
    }

    private void Dump(string stage, MapDocument doc)
    {
        _out.WriteLine("");
        _out.WriteLine($"--- {stage} ---");
        foreach (var e in doc.Files.Where(f =>
            string.Equals(f.FileName, Name, StringComparison.OrdinalIgnoreCase)))
        {
            byte[] bytes = e.OverrideBytes ?? e.RawBytes;
            string text = Encoding.Latin1.GetString(bytes);
            int bad = CountOf(text, "Buttonpos=,");
            int good = CountOf(text, "Buttonpos=0,");
            _out.WriteLine($"  block {e.BlockIndex,5}  locale 0x{e.Locale:X8}  dirty={e.IsDirty,-5} "
                         + $"len {bytes.Length,8}  malformed={bad,4}  fixed={good,4}");
        }
    }

    private static int CountOf(string haystack, string needle)
    {
        int n = 0, i = 0;
        while ((i = haystack.IndexOf(needle, i, StringComparison.OrdinalIgnoreCase)) >= 0) { n++; i += needle.Length; }
        return n;
    }
}
