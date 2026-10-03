// tests/Wc3.Tests/EditFidelityTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Whether an edit changes ONLY what it should.
///
/// This is the project's actual promise, "untouched files write original bytes, only dirty entries
/// re-serialize", and ObjectSetCommand states it outright, "re-serializes it (that file only)".
/// Until now it was verified for ZERO edits. Both round-trip tests load a map and save it back
/// untouched, which proves the rebuild is faithful and says nothing about what one edit disturbs.
///
/// Each case loads a map, makes exactly one edit through a real command, saves, reloads, and diffs
/// against a fresh load of the original file. The expected file set is named exactly, so touching
/// an extra file fails and failing to touch the intended one fails too.
///
/// Asserting on NAMES and not on the change kind, because whether a section is "added" or
/// "modified" depends on whether that map already had one, and both are correct. The first version
/// of this file asserted "modified" everywhere and reported three failures that were entirely its
/// own expectation being wrong.
/// </summary>
public class EditFidelityTests
{
    private readonly ITestOutputHelper _out;
    public EditFidelityTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    /// <summary>
    /// Deliberately varied and small enough to be quick. Tom_and_Jerry is here because it is CR
    /// separated and keeps its script under scripts\, the two shapes that were broken earlier.
    /// Anime_WOS2 is here because it is a Reforged map with a war3mapSkin twin, which is where
    /// collateral damage is most likely, and it is the map the user actually opens.
    /// </summary>
    public static TheoryData<string> Maps => new()
    {
        "HostTest.w3x",
        "Gem TD Inw Blitz 1.1.w3x",
        "Tom_and_Jerry_2014_v1.05.w3x",
        "12331.w3x",
        "LASC7.05.w3x",
        "Anime_WOS2_0.30a1.w3x",
    };

    /// <summary>
    /// Applies one edit, saves, reloads, and returns the internal names whose bytes differ from the
    /// original file. MPQ bookkeeping is excluded, it is regenerated on every save by design.
    /// </summary>
    private static (List<string> Names, List<string> Detail, string Note) ChangedBy(
        string path, Func<MapDocument, string> edit)
    {
        var before = MapDocument.Load(path);
        var working = MapDocument.Load(path);
        var note = edit(working);
        var after = MapDocument.Load(working.SaveToBytes());

        var entries = DiffCommand.Execute(before, after).Entries
            .Where(e => !RoundtripCommand.MpqSpecialFiles.Contains(e.Name))
            .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return (entries.Select(e => e.Name).ToList(),
                entries.Select(e => $"{e.Name}:{e.Change}").ToList(),
                note);
    }

    private void Expect(string mapName, Func<MapDocument, string> edit, params string[] expected)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var (names, detail, note) = ChangedBy(path, edit);
        _out.WriteLine($"{mapName}: {note}");
        _out.WriteLine($"  changed: {(detail.Count == 0 ? "(nothing)" : string.Join(", ", detail))}");

        if (note.StartsWith("SKIP", StringComparison.Ordinal)) return;

        Assert.Equal(
            expected.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList(),
            names);
    }

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Adding_a_region_touches_only_the_region_file(string mapName)
    {
        Expect(mapName, doc =>
        {
            var r = PlacementCommand.PlaceRegion(doc, "FidelityProbe", -256f, -256f, 256f, 256f);
            return r.Ok ? "placed a region" : $"SKIP {r.Message}";
        }, "war3map.w3r");
    }

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Adding_a_camera_touches_only_the_camera_file(string mapName)
    {
        Expect(mapName, doc =>
        {
            var r = CameraCommand.Add(doc, "FidelityProbe", 0f, 0f);
            return r.Ok ? "added a camera" : $"SKIP {r.Message}";
        }, "war3map.w3c");
    }

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Editing_the_script_touches_only_the_script(string mapName)
    {
        Expect(mapName, doc =>
        {
            var (file, source) = ScriptCommand.Read(doc);
            doc.AddOrReplaceRawFile(file, Encoding.UTF8.GetBytes(source + "\n// fidelity probe\n"));
            return $"appended a comment to {file}";
        },
        // Named per map, because 13 of 34 keep the script under scripts\.
        ScriptNameFor(mapName));
    }

    private static string ScriptNameFor(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) return "war3map.j";
        return ScriptCommand.Read(MapDocument.Load(path)).ScriptFile;
    }

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Setting_a_unit_field_touches_only_one_object_data_file(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var probe = MapDocument.Load(path);
        var unit = ObjectListCommand.Execute(probe, ObjectKind.Unit, (string?)null)
            .Items.FirstOrDefault();
        if (unit is null) { _out.WriteLine($"{mapName} has no custom units, skipped"); return; }

        // Which layer this lands in is the map's own business, war3map.w3u on a classic map and
        // possibly the war3mapSkin twin on a Reforged one, so the expectation is resolved by
        // asking rather than assumed. What is asserted is that ONE file changes.
        var layer = SkinFieldPartition.Learn(probe, ObjectKind.Unit).LayerFor("ugol");
        var expected = layer == ObjectLayer.Skin ? "war3mapSkin.w3u" : "war3map.w3u";

        Expect(mapName, doc =>
        {
            var r = ObjectSetCommand.Execute(doc, ObjectKind.Unit, unit.Rawcode, "ugol", "123");
            return r.Ok ? $"set ugol on {unit.Rawcode}, expecting {expected}" : $"SKIP {r.Message}";
        }, expected);
    }

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Setting_an_ability_field_touches_only_one_object_data_file(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var probe = MapDocument.Load(path);
        var ability = ObjectListCommand.Execute(probe, ObjectKind.Ability, (string?)null)
            .Items.FirstOrDefault();
        if (ability is null) { _out.WriteLine($"{mapName} has no custom abilities, skipped"); return; }

        // acdn, cooldown, a per-level real every ability carries.
        var layer = SkinFieldPartition.Learn(probe, ObjectKind.Ability).LayerFor("acdn");
        var expected = layer == ObjectLayer.Skin ? "war3mapSkin.w3a" : "war3map.w3a";

        Expect(mapName, doc =>
        {
            var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, ability.Rawcode, "acdn:1", "7.5");
            return r.Ok ? $"set acdn on {ability.Rawcode}, expecting {expected}" : $"SKIP {r.Message}";
        }, expected);
    }

    /// <summary>
    /// Placing a unit legitimately touches TWO files, the placement table and the script, because
    /// a preplaced unit that no script creates never appears in the game. What this pins is that
    /// the script it touches is the map's OWN script entry.
    /// </summary>
    /// <remarks>
    /// This is the case the sweep was worth writing for. The generator wrote to a hardcoded
    /// "war3map.j", so on a map keeping its script at scripts\war3map.j it read the nested one,
    /// appended the spawn code, and wrote the result to the ROOT name. Tom_and_Jerry went from 71
    /// entries to 73 and ended up with two scripts, the original 249,947 byte one untouched under
    /// scripts\ and a 250,245 byte edited copy at the root.
    /// </remarks>
    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Placing_a_unit_touches_the_placement_table_and_the_maps_own_script(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var (names, detail, note) = ChangedBy(path, doc =>
        {
            var r = PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f, rotation: 270f);
            return r.Ok ? "placed a footman" : $"SKIP {r.Message}";
        });
        _out.WriteLine($"{mapName}: {note}");
        _out.WriteLine($"  changed: {string.Join(", ", detail)}");
        if (note.StartsWith("SKIP", StringComparison.Ordinal)) return;

        var ownScript = ScriptNameFor(mapName);
        // The placement table always. The script only when the generator had to wire a spawn call,
        // which it deliberately skips on a real World-Editor map that already has its own
        // CreateAllUnits, so requiring it everywhere would fail on a correctly untouched map.
        Assert.Contains("war3mapUnits.doo", names);
        var allowed = new[] { "war3mapUnits.doo", ownScript };
        Assert.All(names, n => Assert.Contains(n, allowed, StringComparer.OrdinalIgnoreCase));
    }

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Placing_a_unit_never_leaves_a_second_script_behind(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var before = MapDocument.Load(path);
        int scriptsBefore = CountScripts(before);

        var working = MapDocument.Load(path);
        var r = PlacementCommand.PlaceUnit(working, "hfoo", 0, 0f, 0f, rotation: 270f);
        if (!r.Ok) { _out.WriteLine($"{mapName}: SKIP {r.Message}"); return; }

        var after = MapDocument.Load(working.SaveToBytes());
        int scriptsAfter = CountScripts(after);
        _out.WriteLine($"{mapName}: {scriptsBefore} script entr(ies) before, {scriptsAfter} after "
                     + $"({before.Files.Count} -> {after.Files.Count} entries)");

        Assert.Equal(scriptsBefore, scriptsAfter);
    }

    private static int CountScripts(MapDocument doc) => doc.Files.Count(
        f => f.FileName is not null
             && f.FileName.EndsWith("war3map.j", StringComparison.OrdinalIgnoreCase));

    [Theory]
    [MemberData(nameof(Maps))]
    [Trait("Category", "Corpus")]
    public void Saving_with_no_edit_at_all_changes_nothing(string mapName)
    {
        // The control. If this fails, none of the above means anything.
        Expect(mapName, _ => "no edit");
    }
}
