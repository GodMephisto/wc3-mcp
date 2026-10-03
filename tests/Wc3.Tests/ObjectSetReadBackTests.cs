// tests/Wc3.Tests/ObjectSetReadBackTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Setting a field and reading it back, for every kind, through a real save and reload.
///
/// The form has been checked for completeness, and the edit has been checked for not disturbing
/// other files. What was never checked is the simplest thing an editor does, whether the value you
/// asked for is the value that comes back. A form that shows all 223 fields and stores the wrong
/// number in one of them is worse than a form that hides it.
///
/// Through save and reload rather than in memory, because the object-data writer and reader are
/// separate code and a round trip is where a type or a level index gets lost.
///
/// The result: every kind round-trips, including all three storage types and both shapes. The
/// first version of this test reported ability and upgrade per-level fields as mismatched, and it
/// was the test that was wrong. A leveled modification is keyed by level, so its read-back Code is
/// "acdn:1", while the merged result also carries a bare "acdn" row holding the base value.
/// Comparing against the bare code picked the base row every time.
/// </summary>
public class ObjectSetReadBackTests
{
    private const string Install = @"D:\Warcraft III";
    private readonly ITestOutputHelper _out;
    public ObjectSetReadBackTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    public static TheoryData<string> Maps => new()
    {
        "Gem TD Inw Blitz 1.1.w3x",
        "Tom_and_Jerry_2014_v1.05.w3x",
        "LASC7.05.w3x",
        "Anime_WOS2_0.30a1.w3x",
    };

    /// <summary>
    /// One field per kind, chosen to cover the three storage types and both shapes. Simple kinds
    /// take a bare code, leveled kinds take code:N.
    /// </summary>
    private static readonly (ObjectKind Kind, string Field, string Value)[] Probes =
    {
        (ObjectKind.Unit, "ugol", "1234"),            // int
        (ObjectKind.Unit, "usca", "1.75"),            // unreal
        (ObjectKind.Unit, "unam", "Probe Name"),      // string
        (ObjectKind.Item, "igol", "777"),             // int, item shape
        (ObjectKind.Ability, "acdn:1", "9.25"),       // real, per level
        (ObjectKind.Ability, "anam:1", "Probe Spell"),// string, per level
        (ObjectKind.Destructable, "bhps", "4321"),    // int
        (ObjectKind.Doodad, "dmas", "2.5"),           // unreal
        (ObjectKind.Buff, "fnam", "Probe Buff"),      // string
        (ObjectKind.Upgrade, "gnam:1", "Probe Upg"),  // string, per level
    };

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "GameData")]
    public void A_field_set_on_any_kind_reads_back_as_written(string mapName)
    {
        if (!Directory.Exists(Install)) { _out.WriteLine("no install, skipped"); return; }
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var wrong = new List<string>();
        _out.WriteLine($"{mapName}");

        foreach (var (kind, field, value) in Probes)
        {
            var doc = MapDocument.Load(path);
            var subject = ObjectListCommand.Execute(doc, kind, Install).Items.FirstOrDefault();
            if (subject is null)
            {
                _out.WriteLine($"  {kind,-13} {field,-10} no object of this kind, skipped");
                continue;
            }

            var set = ObjectSetCommand.Execute(doc, kind, subject.Rawcode, field, value);
            if (!set.Ok)
            {
                _out.WriteLine($"  {kind,-13} {field,-10} REFUSED {set.Message}");
                wrong.Add($"{kind} {field}: refused, {set.Message}");
                continue;
            }

            // Through the archive, so the writer and the reader both get exercised.
            var reloaded = MapDocument.Load(doc.SaveToBytes());
            var back = ObjectGetCommand.Execute(reloaded, kind, subject.Rawcode, Install);
            // Match the FULL token, not the bare code. A leveled kind stores the modification
            // keyed by level, so the read-back carries Code "acdn:1", and the merged result also
            // carries a separate bare "acdn" row holding the BASE value. Comparing against the
            // bare code picked that base row and reported a mismatch on a value that was written,
            // stored and read back perfectly. The tool was right and this test was wrong.
            var got = back.Fields.FirstOrDefault(
                f => string.Equals(f.Code, field, StringComparison.OrdinalIgnoreCase));

            if (got is null)
            {
                _out.WriteLine($"  {kind,-13} {field,-10} WROTE but reads back ABSENT");
                wrong.Add($"{kind} {field} on {subject.Rawcode}: written, then absent on reload");
                continue;
            }

            bool same = Equivalent(value, got.Value);
            _out.WriteLine($"  {kind,-13} {field,-10} {subject.Rawcode}  wrote '{value}' "
                         + $"read '{got.Value}'  {(same ? "ok" : "MISMATCH")}");
            if (!same)
                wrong.Add($"{kind} {field} on {subject.Rawcode}: wrote '{value}', read '{got.Value}'");
        }

        Assert.True(wrong.Count == 0, string.Join("\n", wrong));
    }

    /// <summary>
    /// Whether a read-back value means the same as what was written. Numbers compare numerically,
    /// because "1.75" may legitimately come back as "1.750" and a string comparison would call
    /// that a defect.
    /// </summary>
    private static bool Equivalent(string wrote, string read)
    {
        if (string.Equals(wrote, read, StringComparison.Ordinal)) return true;
        if (decimal.TryParse(wrote, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var a)
            && decimal.TryParse(read, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var b))
            return a == b;
        return false;
    }
}
