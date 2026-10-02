using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// TRIGSTR_ display resolution: the merged field grid must show the wts-resolved text,
/// while the raw TRIGSTR_ reference stays in Value so editing/write-back never clobbers
/// the string table. (Complements PortCommand, which INLINES strings for cross-map ports.)
/// </summary>
public class ObjectGetDisplayTests
{
    [Fact]
    public void Resolves_trigstr_for_display_keeps_raw_for_edit()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = "U000".FromRawcode() };
        unit.Modifications.Add(Str("unam", "TRIGSTR_100"));
        unit.Modifications.Add(Str("utip", "TRIGSTR_101"));
        unit.Modifications.Add(Str("uabi", "A000,A001")); // non-TRIGSTR value: Display == Value
        w3u.NewUnits.Add(unit);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.wts"] = Encoding.UTF8.GetBytes(
                "STRING 100\n{\nRaiden Ei\n}\nSTRING 101\n{\nSummons lightning.\n}\n"),
        }));

        var result = ObjectGetCommand.Execute(doc, ObjectKind.Unit, "U000", gameDirOverride: null);

        var name = result.Fields.Single(f => f.Code == "unam");
        Assert.Equal("TRIGSTR_100", name.Value);     // raw preserved for write-back
        Assert.Equal("Raiden Ei", name.Display);      // resolved for the grid

        var tip = result.Fields.Single(f => f.Code == "utip");
        Assert.Equal("Summons lightning.", tip.Display);
        Assert.DoesNotContain("TRIGSTR", tip.Display);

        var abil = result.Fields.Single(f => f.Code == "uabi");
        Assert.Equal(abil.Value, abil.Display);       // non-reference values pass through unchanged
    }

    private static SimpleObjectDataModification Str(string code, string value) =>
        new() { Id = code.FromRawcode(), Type = ObjectDataType.String, Value = value };

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
