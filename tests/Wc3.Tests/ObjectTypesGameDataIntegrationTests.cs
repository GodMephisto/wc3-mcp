using Wc3.GameData;
using GD = Wc3.GameData.GameData; // bare "GameData" resolves to the namespace here
namespace Wc3.Tests;

/// <summary>
/// Resolves a known base object of each of the five newer Object Editor types against
/// the live install. Sample rawcodes and expected values were confirmed by probing the
/// real SLKs (see ObjectDataStore's per-type factory notes).
/// </summary>
public class ObjectTypesGameDataIntegrationTests
{
    private const string Install = @"C:\Warcraft III";

    private static ObjectDataStore? Open(Func<IGameDataSource, ObjectDataStore> build)
    {
        if (!Directory.Exists(Install)) return null;
        Assert.True(CascGameDataSource.TryOpen(Install, out var src, out var err), err);
        using (src) return build(src!);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Resolves_claws_of_attack_item()
    {
        if (Open(ObjectDataStore.BuildItems) is not { } store) return;
        Assert.True(store.TryGet("ratf", out var fields), "Claws of Attack ratf should resolve");
        Assert.NotEmpty(fields);
        // Recognizable values: ilev = item level, icla = item class.
        Assert.Equal("7", fields["ilev"]);
        Assert.Equal("Artifact", fields["icla"]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Resolves_tree_wall_destructable()
    {
        if (Open(ObjectDataStore.BuildDestructables) is not { } store) return;
        Assert.True(store.TryGet("ATtr", out var fields), "Ashenvale Tree Wall ATtr should resolve");
        Assert.NotEmpty(fields);
        // Recognizable values: bhps = hit points, btar = targeted-as type.
        Assert.Equal("50", fields["bhps"]);
        Assert.Equal("tree", fields["btar"]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Resolves_hollow_stump_doodad()
    {
        if (Open(ObjectDataStore.BuildDoodads) is not { } store) return;
        Assert.True(store.TryGet("AOhs", out var fields), "Hollow Stump AOhs should resolve");
        Assert.NotEmpty(fields);
        // Recognizable value: dcat = doodad category (E = Environment).
        Assert.Equal("E", fields["dcat"]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Resolves_stun_buff()
    {
        if (Open(ObjectDataStore.BuildBuffs) is not { } store) return;
        Assert.True(store.TryGet("BSTN", out var fields), "Stun buff BSTN should resolve");
        Assert.NotEmpty(fields);
        // Only two buff fields are SLK-backed (the rest are Profile): frac = race.
        Assert.Equal("other", fields["frac"]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Resolves_melee_attack_upgrade()
    {
        if (Open(ObjectDataStore.BuildUpgrades) is not { } store) return;
        Assert.True(store.TryGet("Rhme", out var fields), "Human melee upgrade Rhme should resolve");
        Assert.NotEmpty(fields);
        // Recognizable values: grac = race, glvl = max level.
        Assert.Equal("human", fields["grac"]);
        Assert.Equal("3", fields["glvl"]);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void TryOpen_builds_all_seven_stores_from_one_open()
    {
        if (!Directory.Exists(Install)) return;
        Assert.True(GD.TryOpen(Install, out var ctx, out var diag), diag);
        Assert.Empty(ctx!.Diagnostics);
        Assert.True(ctx.Units.TryGetUnit("hfoo", out _));
        Assert.True(ctx.Abilities.TryGetAbility("AHbz", out _));
        Assert.True(ctx.Items.TryGet("ratf", out _));
        Assert.True(ctx.Destructables.TryGet("ATtr", out _));
        Assert.True(ctx.Doodads.TryGet("AOhs", out _));
        Assert.True(ctx.Buffs.TryGet("BSTN", out _));
        Assert.True(ctx.Upgrades.TryGet("Rhme", out _));
    }
}
