// tests/Wc3.Tests/MapSkinObjectDataTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Tests;

public class MapSkinObjectDataTests
{
    [Theory]
    [InlineData("war3mapSkin.w3u")]
    [InlineData("war3mapSkin.w3t")]
    [InlineData("war3mapSkin.w3a")]
    [InlineData("war3mapSkin.w3b")]
    [InlineData("war3mapSkin.w3d")]
    [InlineData("war3mapSkin.w3h")]
    [InlineData("war3mapSkin.w3q")]
    public void Skin_object_data_files_are_known(string name) => Assert.True(MapFormatRegistry.IsKnown(name));

    [Fact]
    public void Skin_w3u_parses_to_UnitObjectData()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var mod = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = 0 };
        mod.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhpm".FromRawcode(), Type = ObjectDataType.Int, Value = 500 });
        w3u.BaseUnits.Add(mod);

        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) bw.Write(w3u);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3mapSkin.w3u"] = ms.ToArray(),
        }));

        var entry = doc.GetFile("war3mapSkin.w3u");
        Assert.NotNull(entry);
        Assert.True(entry!.IsKnown);
        var model = Assert.IsType<UnitObjectData>(entry.Model);
        Assert.Single(model.BaseUnits);
    }
}
