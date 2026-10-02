// tests/Wc3.Tests/AbilityLevelGapProbe.cs
using War3Net.Build.Object;
using Wc3.GameData;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The wc3edit guide's fifth problem is that an ability SLK stops at level 4 while 3.0.0 reads
/// levels 5 and 6, so the higher levels come back as zero. That is told as an SLK story, but the
/// cause is not the SLK, it is that 3.0.0 raised the level count on base abilities. Any map whose
/// custom ability inherits its level count from a base is exposed the same way, including the
/// large majority that use the binary war3map.w3a rather than SLK object data.
///
/// The first version of this probe measured the wrong population and reported a clean result. It
/// only considered abilities that set "alev" explicitly, which are precisely the abilities that
/// are SAFE, because an explicit count cannot be changed underneath them. The exposed ones are
/// those that say nothing and inherit. Correcting that is the whole point of this file.
/// </summary>
public class AbilityLevelGapProbe
{
    private const string Install = @"C:\Warcraft III";
    private const int AlevId = 0x76656C61; // 'alev' as stored little endian

    private readonly ITestOutputHelper _out;
    public AbilityLevelGapProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "GameData")]
    public void How_many_base_abilities_now_carry_more_than_four_levels()
    {
        if (!Wc3.GameData.GameData.TryOpen(Install, out var ctx, out var diag) || ctx is null)
        {
            _out.WriteLine($"no game data, {diag}");
            return;
        }

        int total = 0, overFour = 0;
        var examples = new List<string>();
        foreach (var code in ctx.Abilities.Rawcodes)
        {
            if (!ctx.Abilities.TryGetAbility(code, out var fields)) continue;
            total++;
            if (!fields.TryGetValue("alev", out var raw) || !int.TryParse(raw, out int levels)) continue;
            if (levels > 4)
            {
                overFour++;
                if (examples.Count < 10) examples.Add($"{code}={levels}");
            }
        }
        _out.WriteLine($"base abilities in build 3.0.0.24268: {total}");
        _out.WriteLine($"declaring more than 4 levels: {overFour}");
        _out.WriteLine("examples: " + string.Join(", ", examples));
    }

    [Theory]
    [Trait("Category", "GameData")]
    [InlineData("BleachVsOnepiece15.w3x")]
    public void Which_custom_abilities_have_a_level_gap(string mapName)
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} not on disk"); return; }
        if (!Wc3.GameData.GameData.TryOpen(Install, out var ctx, out var diag) || ctx is null)
        {
            _out.WriteLine($"no game data, {diag}");
            return;
        }

        var doc = MapDocument.Load(path);
        if (doc.GetFile("war3map.w3a")?.Model is not AbilityObjectData w3a)
        {
            _out.WriteLine("no parsed war3map.w3a");
            return;
        }

        int total = 0, explicitCount = 0, inherited = 0, gaps = 0;
        var worst = new List<string>();

        foreach (var ability in w3a.BaseAbilities.Cast<LevelObjectModification>()
                     .Concat(w3a.NewAbilities.Cast<LevelObjectModification>()))
        {
            total++;
            string baseCode = Rawcode(ability.OldId);

            var alev = ability.Modifications.FirstOrDefault(m => m.Id == AlevId);
            int declared;
            if (alev is not null) { declared = alev.ValueAsInt; explicitCount++; }
            else
            {
                inherited++;
                declared = ctx.Abilities.TryGetAbility(baseCode, out var bf)
                           && bf.TryGetValue("alev", out var raw)
                           && int.TryParse(raw, out int lv) ? lv : 0;
            }

            // The highest level this map actually supplies any per-level value for.
            int supplied = 0;
            foreach (var m in ability.Modifications)
                if (m.Level > supplied) supplied = m.Level;

            if (declared > 1 && supplied > 0 && declared > supplied)
            {
                gaps++;
                if (worst.Count < 15)
                    worst.Add($"{Rawcode(ability.NewId != 0 ? ability.NewId : ability.OldId)} " +
                              $"(base {baseCode}) declares {declared}, supplies {supplied}");
            }
        }

        _out.WriteLine($"{mapName}");
        _out.WriteLine($"  {total} custom abilities, {explicitCount} set alev, {inherited} inherit it");
        _out.WriteLine($"  {gaps} have a level gap");
        foreach (var w in worst) _out.WriteLine("     " + w);
    }

    private static string Rawcode(int id)
    {
        var b = BitConverter.GetBytes(id);
        return new string(new[] { (char)b[3], (char)b[2], (char)b[1], (char)b[0] });
    }
}
