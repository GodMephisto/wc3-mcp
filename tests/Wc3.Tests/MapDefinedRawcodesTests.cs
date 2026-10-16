// tests/Wc3.Tests/MapDefinedRawcodesTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// The set of rawcodes a map defines decides whether a reference is dangling. A map can define
/// its objects in replacement SLK tables, not just in object data, and reading only object data
/// made the audit report 21 phantom dangling-buff errors on a map whose buffs live in
/// Units\AbilityBuffData.slk, and would have had repair audit-errors rewrite 21 real fields.
/// </summary>
public class MapDefinedRawcodesTests
{
    private static byte[] Bytes(string value) => Encoding.Latin1.GetBytes(value.Replace("\n", "\r\n"));

    private static MapDocument Map(IReadOnlyDictionary<string, byte[]> files) =>
        MapDocument.Load(SyntheticMap.Build(files));

    [Fact]
    public void An_slk_buff_alias_counts_as_defined_by_the_map()
    {
        var doc = Map(new Dictionary<string, byte[]>
        {
            [@"Units\AbilityBuffData.slk"] = Bytes("""
                ID;PWXL;N;E
                B;X2;Y2
                C;X1;Y1;K"alias"
                C;X2;Y1;K"code"
                C;X1;Y2;K"B00S"
                C;X2;Y2;K"BOac"
                E
                """),
        });

        Assert.Contains("B00S", AuditCommand.MapDefinedRawcodes(doc));
    }

    [Fact]
    public void A_metadata_slk_field_id_is_not_whitelisted()
    {
        // UnitMetaData.slk's leading column is a field code, not an object id. Reading it would
        // wrongly whitelist arbitrary rawcodes and hide genuine dangling references.
        var doc = Map(new Dictionary<string, byte[]>
        {
            [@"Units\UnitMetaData.slk"] = Bytes("""
                ID;PWXL;N;E
                B;X1;Y2
                C;X1;Y1;K"ID"
                C;X1;Y2;K"bogus"
                E
                """),
        });

        Assert.DoesNotContain("bogus", AuditCommand.MapDefinedRawcodes(doc));
    }

    [Fact]
    public void A_forward_slash_archive_path_still_resolves_the_slk_file_name()
    {
        // An archive name uses the backslash as its separator on every platform, but a map can
        // store the forward slash instead. System.IO.Path.GetFileName returns the whole string on
        // macOS and Linux, so the file name test would miss and the SLK would read as undefined.
        var doc = Map(new Dictionary<string, byte[]>
        {
            ["Units/AbilityBuffData.slk"] = Bytes("""
                ID;PWXL;N;E
                B;X2;Y2
                C;X1;Y1;K"alias"
                C;X2;Y1;K"code"
                C;X1;Y2;K"B00S"
                C;X2;Y2;K"BOac"
                E
                """),
        });

        Assert.Contains("B00S", AuditCommand.MapDefinedRawcodes(doc));
    }

    [Fact]
    public void A_protected_map_resolves_its_slk_defined_rawcodes_by_hash()
    {
        // A protected map strips the listfile, so the SLK opens unnamed and the named-file loop
        // sees nothing. The standard path probe through the MPQ hash recovers it.
        var doc = MapDocument.Load(SyntheticMap.BuildProtected(
            new Dictionary<string, byte[]> { ["war3map.j"] = Bytes("function main takes nothing returns nothing\nendfunction\n") },
            new Dictionary<string, byte[]>
            {
                [@"Units\AbilityBuffData.slk"] = Bytes("""
                    ID;PWXL;N;E
                    B;X2;Y2
                    C;X1;Y1;K"alias"
                    C;X2;Y1;K"code"
                    C;X1;Y2;K"B00S"
                    C;X2;Y2;K"BOac"
                    E
                    """),
            }));

        Assert.Contains("B00S", AuditCommand.MapDefinedRawcodes(doc));
    }

    [Fact]
    public void Object_data_ids_are_still_defined()
    {
        // The pre-existing half of the contract, so the SLK addition does not regress it.
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        w3u.NewUnits.Add(new SimpleObjectModification
        {
            OldId = "hfoo".FromRawcode(),
            NewId = "h000".FromRawcode(),
        });
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) bw.Write(w3u);

        var doc = Map(new Dictionary<string, byte[]> { ["war3map.w3u"] = ms.ToArray() });

        Assert.Contains("h000", AuditCommand.MapDefinedRawcodes(doc));
    }
}
