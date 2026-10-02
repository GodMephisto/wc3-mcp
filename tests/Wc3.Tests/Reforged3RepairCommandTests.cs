using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class Reforged3RepairCommandTests
{
    private static readonly Encoding Text = Encoding.Latin1;

    [Fact]
    public void Execute_DryRunReportsEveryRepairWithoutMutatingMap()
    {
        var doc = LoadAffectedMap();

        var result = Reforged3RepairCommand.Execute(doc, apply: false);

        Assert.True(result.Ok);
        Assert.False(result.Applied);
        Assert.Equal(1, result.FileColumnsRemoved);
        Assert.Equal(4, result.ModelsMoved);
        Assert.Equal(4, result.NumericCellsNormalized);
        Assert.Equal(2, result.ButtonPositionsCompleted);
        Assert.Equal(1, result.StrayCommentTerminatorsRemoved);
        Assert.Equal(4, result.AbilityLevelColumnsAdded);
        Assert.All(doc.Files, file => Assert.False(file.IsDirty));
    }

    [Fact]
    public void Execute_ApplyRepairsAndRoundTripsAllAffectedFiles()
    {
        var doc = LoadAffectedMap();

        var result = Reforged3RepairCommand.Execute(doc, apply: true);

        Assert.True(result.Ok);
        Assert.True(result.Applied);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        string unitUi = Read(reloaded, @"Units\UnitUI.slk");
        Assert.DoesNotContain("K\"file\"", unitUi);
        Assert.DoesNotContain("K\"280.\"", unitUi);
        Assert.Contains("K280", unitUi);

        string unitSkin = Read(reloaded, @"Units\UnitSkin.txt");
        Assert.Contains("[h001]", unitSkin);
        Assert.Contains(@"file=Models\Newest.mdl", unitSkin);
        Assert.Contains("buttonpos=0,2", unitSkin, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("researchbuttonpos=1,0", unitSkin, StringComparison.OrdinalIgnoreCase);

        string ability = Read(reloaded, @"Units\AbilityData.slk");
        Assert.Contains("K\"DataA5\"", ability);
        Assert.Contains("K\"DataA6\"", ability);
        Assert.Contains("K\"Cool5\"", ability);
        Assert.Contains("K\"Cool6\"", ability);
        Assert.DoesNotContain("*/", Read(reloaded, @"UI\Broken.fdf"));

        var second = Reforged3RepairCommand.Execute(reloaded, apply: true);
        Assert.Equal(0, second.IssueCount);
        Assert.Empty(second.ChangedFiles);
    }

    private static MapDocument LoadAffectedMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"Units\UnitUI.slk"] = Bytes("""
                ID;PWXL;N;E
                B;X4;Y5
                C;X1;Y1;K"unitUIID"
                C;X2;Y1;K"file"
                C;X3;Y1;K"walk"
                C;X4;Y1;K"name"
                C;X1;Y2;K"h001"
                C;X2;Y2;K"Models\Old.mdl"
                C;X3;Y2;K"280."
                C;X4;Y2;K"One"
                C;X1;Y3;K"h001"
                C;X2;Y3;K"Models\Newest.mdl"
                C;X3;Y3;K"281."
                C;X4;Y3;K"Duplicate id"
                C;X1;Y4;K"h002"
                C;X2;Y4;K"Models\Two.mdl"
                C;X3;Y4;K"282."
                C;X1;Y5;K"h003"
                C;X2;Y5;K"Models\Three.mdl"
                C;X3;Y5;K"283."
                C;X1;Y6;K"h004"
                C;X2;Y6;K"Models\Four.mdl"
                C;X3;Y6;K284
                E
                """),
            [@"Units\UnitSkin.txt"] = Bytes("""
                [h000]
                file=Models\Existing.mdl
                buttonpos=,2
                researchbuttonpos=1
                """),
            [@"Units\AbilityData.slk"] = Bytes("""
                ID;PWXL;N;E
                B;X3;Y2
                C;X1;Y1;K"alias"
                C;X2;Y1;K"DataA4"
                C;X3;Y1;K"Cool4"
                C;X1;Y2;K"A000"
                C;X2;Y2;K42
                C;X3;Y2;K3.5
                E
                """),
            [@"UI\Broken.fdf"] = Bytes("Frame \"Broken\" { }*/"),
        }));

    private static byte[] Bytes(string value) => Text.GetBytes(value.Replace("\n", "\r\n"));

    private static string Read(MapDocument doc, string name)
    {
        var file = Assert.IsType<MapFileEntry>(doc.GetFile(name));
        return Text.GetString(file.OverrideBytes ?? file.RawBytes);
    }
}
