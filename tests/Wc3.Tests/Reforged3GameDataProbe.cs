// tests/Wc3.Tests/Reforged3GameDataProbe.cs
using System.Text;
using Wc3.GameData;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Checks the Reforged 3.0.0 claims against the installed game's own data rather than against
/// forum prose. The install here is build 3.0.0.24268, which is the build the patch notes and the
/// wc3edit thread are both about, so CASC is the primary source for what actually changed.
///
/// The ability level claim matters most. The shipped repair duplicates level 4 values into new
/// level 5 and 6 columns, and that rule appears nowhere in Blizzard's patch notes. It comes only
/// from one community measurement. If the base AbilityData.slk really carries level 5 and 6
/// columns, the rule is grounded. If it does not, the repair is writing columns the game never
/// reads, on 80 columns across two of this machine's maps.
/// </summary>
public class Reforged3GameDataProbe
{
    private readonly ITestOutputHelper _out;
    public Reforged3GameDataProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "GameData")]
    public void Does_the_base_ability_table_carry_levels_five_and_six()
    {
        if (!TryOpen(out var ctx)) return;

        if (!ctx!.TryReadFile(@"units\abilitydata.slk", out var bytes))
        {
            _out.WriteLine("could not read units\\abilitydata.slk from CASC");
            return;
        }

        string text = Encoding.Latin1.GetString(bytes);
        _out.WriteLine($"abilitydata.slk is {bytes.Length:N0} bytes");

        // The header row names every column. Levels appear as a field name with the level
        // appended, DataA1 through DataA4 classically.
        foreach (var stem in new[] { "DataA", "DataB", "Cool", "Cast", "Dur", "Rng", "Cost" })
        {
            var present = new List<int>();
            for (int lvl = 1; lvl <= 8; lvl++)
                if (text.Contains($"\"{stem}{lvl}\"", StringComparison.OrdinalIgnoreCase))
                    present.Add(lvl);
            _out.WriteLine($"  {stem,-6} levels present: {(present.Count == 0 ? "none" : string.Join(",", present))}");
        }
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void What_does_a_blizzard_authored_3_0_0_map_look_like()
    {
        if (!TryOpen(out var ctx)) return;

        // A Blizzard map shipped inside this build is the only 3.0.0-saved map reachable without
        // opening the editor by hand, which makes it the ground truth for whether war3map.doo and
        // the new war3map.w3l changed shape.
        string[] candidates =
        {
            @"maps\frozenthrone\campaign\nightelfx01.w3x",
            @"maps\campaign\humanx01.w3m",
            @"maps\(2)echoisles.w3x",
            @"maps\frozenthrone\(2)echoisles.w3x",
            @"war3.w3mod:maps\(2)echoisles.w3x",
            @"maps\test\alteracmountains.w3x",
        };

        string outDir = Path.Combine(Path.GetTempPath(), "wc3ctl-casc-maps");
        Directory.CreateDirectory(outDir);

        bool any = false;
        foreach (var path in candidates)
        {
            if (!ctx!.TryReadFile(path, out var bytes) || bytes.Length == 0) continue;
            any = true;
            string dest = Path.Combine(outDir, Path.GetFileName(path));
            File.WriteAllBytes(dest, bytes);
            _out.WriteLine($"READ {path}  {bytes.Length:N0} bytes  -> {dest}");
        }
        if (!any)
            _out.WriteLine("none of the candidate map paths resolved, CASC map naming needs a listfile route");
    }

    private bool TryOpen(out GameDataContext? ctx)
    {
        ctx = null;
        string install = @"C:\Warcraft III";
        if (!Directory.Exists(install)) { _out.WriteLine("no install"); return false; }
        if (!Wc3.GameData.GameData.TryOpen(install, out ctx, out var error))
        {
            _out.WriteLine($"could not open game data, {error}");
            return false;
        }
        return true;
    }
}
