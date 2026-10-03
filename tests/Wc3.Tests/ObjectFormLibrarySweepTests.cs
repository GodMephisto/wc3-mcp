// tests/Wc3.Tests/ObjectFormLibrarySweepTests.cs
using Wc3.Commands;
using Wc3.GameData;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Object form completeness across the whole map library, not one map.
///
/// The seven kinds were verified against Anime_WOS2 alone. That map is a Reforged arena with a
/// war3mapSkin twin, which is one shape out of several. A classic map with no skin layer, an
/// optimized map whose object data was rewritten, and a map with custom objects whose base is
/// itself custom are all shapes the form has to handle, and none had ever been checked.
///
/// The invariant is the one that matters for an editor: shown plus hidden must account for every
/// field the metadata defines, because a field in neither bucket can be neither seen nor set.
/// </summary>
public class ObjectFormLibrarySweepTests
{
    private const string Install = @"D:\Warcraft III";
    private readonly ITestOutputHelper _out;
    public ObjectFormLibrarySweepTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    /// <summary>A spread of shapes, small enough to stay quick.</summary>
    public static TheoryData<string> Maps => new()
    {
        "HostTest.w3x",                     // near-empty, a floor case
        "Gem TD Inw Blitz 1.1.w3x",         // classic World Editor map
        "Tom_and_Jerry_2014_v1.05.w3x",     // optimized, CR separated, nested script
        "12331.w3x",
        "LASC7.05.w3x",
        "FarmerVsHunterX2.64c.w3x",
        "Anime_WOS2_0.30a1.w3x",            // Reforged, has a war3mapSkin twin
        "GGGA_V0.04g.w3x",                  // the largest readable arena
    };

    private static ObjectMetadata? MetaFor(GameDataContext ctx, ObjectKind kind) => kind switch
    {
        ObjectKind.Unit => ctx.Units.FieldMetadata,
        ObjectKind.Ability => ctx.Abilities.FieldMetadata,
        ObjectKind.Item => ctx.Items.Metadata,
        ObjectKind.Destructable => ctx.Destructables.Metadata,
        ObjectKind.Doodad => ctx.Doodads.Metadata,
        ObjectKind.Buff => ctx.Buffs.Metadata,
        ObjectKind.Upgrade => ctx.Upgrades.Metadata,
        _ => null,
    };

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "GameData")]
    public void Every_kind_on_every_map_accounts_for_every_field(string mapName)
    {
        if (!Directory.Exists(Install)) { _out.WriteLine("no install, skipped"); return; }
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        Assert.True(GameData.GameData.TryOpen(Install, out var ctx, out var why), why);

        var losing = new List<string>();
        _out.WriteLine($"{mapName}");
        _out.WriteLine("  kind            objects  defines  shown  hidden  rows");

        foreach (var kind in ObjectKinds.All)
        {
            var items = ObjectListCommand.Execute(doc, kind, Install).Items;
            var meta = MetaFor(ctx!, kind);
            if (meta is null) continue;
            if (items.Count == 0)
            {
                _out.WriteLine($"  {kind,-14} {0,7}  (no custom objects of this kind)");
                continue;
            }

            // A custom whose base differs is the interesting case, that is where UseSpecific has
            // to resolve through the base rather than the object's own rawcode.
            var subject = items.FirstOrDefault(o =>
                o.BaseRawcode is not null && o.BaseRawcode != o.Rawcode) ?? items[0];

            ObjectForm form;
            try
            {
                form = ObjectFormCommand.Execute(doc, kind, subject.Rawcode, Install);
            }
            catch (Exception ex)
            {
                losing.Add($"{kind} on {subject.Rawcode} threw {ex.GetType().Name}: {ex.Message}");
                continue;
            }

            int accounted = form.FieldCount + form.HiddenFieldCount;
            int rows = form.Groups.Sum(g => g.Fields.Count);
            _out.WriteLine($"  {kind,-14} {items.Count,7}  {meta.Fields.Count,7}  "
                         + $"{form.FieldCount,5}  {form.HiddenFieldCount,6}  {rows,4}");

            if (accounted < meta.Fields.Count)
                losing.Add($"{kind} on {subject.Rawcode} accounts for {accounted} of "
                         + $"{meta.Fields.Count} fields, losing {meta.Fields.Count - accounted}");

            // The rows a front-end renders must match the count the form reports, or the panel and
            // the summary line disagree about what is on screen.
            if (rows != form.FieldCount)
                losing.Add($"{kind} on {subject.Rawcode} reports {form.FieldCount} fields but "
                         + $"renders {rows} rows");
        }

        Assert.True(losing.Count == 0, string.Join("\n", losing));
    }

    /// <summary>
    /// The form must survive EVERY object in a map, not just the first one that looks interesting.
    /// A single rawcode whose metadata lookup throws would take the Objects panel down.
    /// </summary>
    /// <remarks>
    /// Deliberately exhaustive rather than sampled, and it costs what that costs. 11,709 forms
    /// across the eight maps, of which GGGA_V0.04g alone is 5,755 and takes about two minutes.
    /// Both traits are opt-in, so the price is only paid when asked for, and a sample would not
    /// have answered the question, which is whether ANY object anywhere breaks the panel.
    /// </remarks>
    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "GameData")]
    public void No_object_in_a_map_makes_the_form_throw(string mapName)
    {
        if (!Directory.Exists(Install)) { _out.WriteLine("no install, skipped"); return; }
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var failures = new List<string>();
        int built = 0;

        foreach (var kind in ObjectKinds.All)
        {
            foreach (var item in ObjectListCommand.Execute(doc, kind, Install).Items)
            {
                try
                {
                    var form = ObjectFormCommand.Execute(doc, kind, item.Rawcode, Install);
                    built++;
                    if (form.FieldCount == 0 && form.HiddenFieldCount == 0)
                        failures.Add($"{kind} {item.Rawcode} produced an empty form");
                }
                catch (Exception ex)
                {
                    failures.Add($"{kind} {item.Rawcode}: {ex.GetType().Name} {ex.Message}");
                }
                if (failures.Count > 6) break;   // enough to diagnose
            }
        }

        _out.WriteLine($"{mapName}: built {built:N0} form(s), {failures.Count} failure(s)");
        foreach (var f in failures.Take(6)) _out.WriteLine($"   {f}");
        Assert.Empty(failures);
    }
}
