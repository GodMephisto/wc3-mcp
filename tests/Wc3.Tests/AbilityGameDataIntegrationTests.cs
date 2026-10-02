using Wc3.GameData;
namespace Wc3.Tests;
public class AbilityGameDataIntegrationTests
{
    private const string Install = @"C:\Warcraft III";

    [Fact]
    [Trait("Category", "GameData")]
    public void Resolves_blizzard_base_fields()
    {
        if (!Directory.Exists(Install)) return;
        Assert.True(CascGameDataSource.TryOpen(Install, out var src, out var err), err);
        using (src)
        {
            var store = BaseAbilityStore.Build(src!);
            Assert.True(store.TryGetAbility("AHbz", out var fields), "Blizzard AHbz should resolve");
            Assert.NotEmpty(fields);
            // Recognizable values: aher = is hero ability, arac = race.
            Assert.Equal("1", fields["aher"]);
            Assert.Equal("human", fields["arac"]);
        }
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Resolves_blizzard_leveled_fields_per_level()
    {
        if (!Directory.Exists(Install)) return;
        Assert.True(CascGameDataSource.TryOpen(Install, out var src, out var err), err);
        using (src)
        {
            var store = BaseAbilityStore.Build(src!);
            Assert.True(store.TryGetAbility("AHbz", out var fields), "Blizzard AHbz should resolve");

            // Hbz2 is Blizzard's damage-per-wave data field (metadata data=2, datab columns).
            // A real per-level table means the values differ across levels 1..3.
            Assert.True(fields.TryGetValue("Hbz2:1", out var dmg1), "Hbz2:1 should resolve");
            Assert.True(fields.TryGetValue("Hbz2:2", out var dmg2), "Hbz2:2 should resolve");
            Assert.True(fields.TryGetValue("Hbz2:3", out var dmg3), "Hbz2:3 should resolve");
            Assert.NotEqual(dmg1, dmg2);
            Assert.NotEqual(dmg2, dmg3);

            // Bare code keeps level 1 so flat lookups stay stable.
            Assert.Equal(dmg1, fields["Hbz2"]);

            // Cooldown (acdn, cool columns) resolves per level too, and the expansion
            // clamps at Blizzard's real level count of 3 despite cool4 existing as padding.
            Assert.True(fields.ContainsKey("acdn:1"), "acdn:1 should resolve");
            Assert.True(fields.ContainsKey("acdn:3"), "acdn:3 should resolve");
            Assert.False(fields.ContainsKey("acdn:4"), "acdn:4 is SLK padding beyond levels=3");

            // useSpecific scoping: Storm Bolt's data field must not leak into Blizzard
            // even though both read the same lettered data columns.
            Assert.False(fields.ContainsKey("Htb1"), "Htb1 is scoped to AHtb by useSpecific");
        }
    }
}
