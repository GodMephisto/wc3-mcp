// tests/Wc3.Tests/MutatingCommandReadBackTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using War3Net.Build.Environment;   // PathingType
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Set-then-read-back for the mutating commands that are NOT object data, through a real save and
/// reload.
///
/// Object set/read-back was swept and came back clean. Everything else that writes was untested:
/// the string table, map info, players and forces, placed unit and doodad edits, terrain height,
/// terrain tiles, and the pathing mask. Each is a separate writer with its own file and its own
/// reader, which is exactly the shape where a value gets lost between the two.
///
/// Every case saves to bytes and reloads before reading, because an in-memory check would pass on
/// a writer that never serialized.
/// </summary>
public class MutatingCommandReadBackTests
{
    private readonly ITestOutputHelper _out;
    public MutatingCommandReadBackTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    private static MapDocument Reload(MapDocument doc) => MapDocument.Load(doc.SaveToBytes());

    /// <summary>A real map with placed units, doodads, a string table and custom forces.</summary>
    private MapDocument? RealMap(string name = "Gem TD Inw Blitz 1.1.w3x")
    {
        var path = Path.Combine(Dir, name);
        if (!File.Exists(path)) { _out.WriteLine($"{name} absent, skipped"); return null; }
        return MapDocument.Load(path);
    }

    // ---- the string table ----------------------------------------------------------------

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_string_table_entry_reads_back_as_written()
    {
        if (RealMap() is not { } doc) return;

        var wts = doc.GetFile("war3map.wts");
        if (wts is null) { _out.WriteLine("map has no war3map.wts, skipped"); return; }

        var source = Encoding.UTF8.GetString(wts.CurrentBytes);
        var first = StringsCommand.Parse(source).FirstOrDefault();
        if (first is null) { _out.WriteLine("empty string table, skipped"); return; }

        const string Probe = "wc3ctl read-back probe";
        var updated = StringsCommand.SetEntry(source, first.Id, Probe);
        doc.AddOrReplaceRawFile("war3map.wts", Encoding.UTF8.GetBytes(updated));

        var reloaded = Reload(doc);
        var back = Encoding.UTF8.GetString(reloaded.GetFile("war3map.wts")!.CurrentBytes);
        var entry = StringsCommand.Get(back, first.Id);

        _out.WriteLine($"id {first.Id}: '{Trim(first.Text)}' -> '{Trim(entry?.Text ?? "")}'");
        Assert.NotNull(entry);
        Assert.Equal(Probe, entry!.Text);

        // And every other entry survives, since a table rewrite is where neighbours get eaten.
        var before = StringsCommand.Parse(source);
        var after = StringsCommand.Parse(back);
        _out.WriteLine($"{before.Count} entries before, {after.Count} after");
        Assert.Equal(before.Count, after.Count);
        foreach (var b in before.Where(b => b.Id != first.Id))
            Assert.Equal(b.Text, StringsCommand.Get(back, b.Id)?.Text);
    }

    private static string Trim(string s) =>
        s.Length <= 30 ? s.Replace('\n', ' ') : s[..30].Replace('\n', ' ') + "...";

    // ---- map info ------------------------------------------------------------------------

    /// <summary>
    /// Every editable map-info field, driven from the command's OWN list rather than a hardcoded
    /// one. The contract asserted is the honest one: if the command ACCEPTS a value it must store
    /// it, and if it refuses it must say so rather than pretend.
    /// </summary>
    /// <remarks>
    /// Driven from MapInfoCommand.EditableFields because the first version hardcoded three names
    /// and got one of them wrong, "MapAuthor" where the field is "Author". The command refused with
    /// a message listing all 37 editable fields, which was exactly right, and the test was what
    /// needed fixing. Reading the list means the test cannot drift from the vocabulary again, and
    /// it covers 37 fields instead of 3.
    /// </remarks>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Every_editable_map_info_field_stores_what_it_accepts()
    {
        if (RealMap() is not { } probe) return;

        var current = MapInfoCommand.Read(probe);
        var props = typeof(MapInfoFields).GetProperties()
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        var wrong = new List<string>();
        int stored = 0, refused = 0, unreadable = 0;

        foreach (var field in MapInfoCommand.EditableFields)
        {
            if (!props.TryGetValue(field, out var prop))
            {
                // An editable field with no matching read-back property cannot be verified, which
                // is itself worth reporting rather than skipping quietly.
                unreadable++;
                _out.WriteLine($"  {field,-32} EDITABLE BUT NOT READABLE");
                wrong.Add($"{field}: editable but MapInfoFields exposes no property to read it back");
                continue;
            }

            var was = prop.GetValue(current)?.ToString() ?? "";
            var value = ProbeValue(field, was);

            var doc = RealMap()!;
            string message;
            try
            {
                MapInfoCommand.Set(doc, field, value);
                message = "accepted";
            }
            catch (Exception ex)
            {
                refused++;
                _out.WriteLine($"  {field,-32} refused '{value}': {Short(ex.Message)}");
                continue;
            }

            var back = MapInfoCommand.Read(Reload(doc));
            var got = prop.GetValue(back)?.ToString() ?? "";
            bool same = string.Equals(value, got, StringComparison.OrdinalIgnoreCase);
            stored++;
            _out.WriteLine($"  {field,-32} was '{Short(was)}' wrote '{Short(value)}' "
                         + $"read '{Short(got)}'  {(same ? "ok" : "MISMATCH")}  {message}");
            if (!same)
                wrong.Add($"{field}: accepted '{value}' and stored '{got}'");
        }

        _out.WriteLine($"{MapInfoCommand.EditableFields.Count} editable, {stored} stored, "
                     + $"{refused} refused, {unreadable} not readable, {wrong.Count} wrong");
        Assert.True(wrong.Count == 0, string.Join("\n", wrong));
    }

    /// <summary>A value of the same shape as the current one, so a bool stays a bool and a number
    /// stays a number. A field whose value space is constrained will refuse, which is fine.</summary>
    private static string ProbeValue(string field, string current)
    {
        if (bool.TryParse(current, out var b)) return (!b).ToString().ToLowerInvariant();
        if (int.TryParse(current, out var i)) return (i + 1).ToString();
        if (float.TryParse(current, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var f))
            return (f + 1.5f).ToString(System.Globalization.CultureInfo.InvariantCulture);
        return "probe " + field;
    }

    private static string Short(string s) =>
        s.Length <= 26 ? s.Replace('\n', ' ') : s[..26].Replace('\n', ' ') + "...";

    // ---- players and forces --------------------------------------------------------------

    [Fact]
    [Trait("Category", "Corpus")]
    public void Moving_a_player_between_forces_reads_back()
    {
        if (RealMap() is not { } doc) return;

        var forces = PlayerForceCommand.GetForces(doc);
        var players = PlayerForceCommand.GetPlayers(doc);
        if (forces.Count == 0 || players.Count == 0)
        { _out.WriteLine("no forces or players, skipped"); return; }

        int player = players[0].Id;
        int target = forces.Count > 1 ? 1 : 0;

        var result = PlayerForceCommand.SetPlayerForce(doc, player, target);
        _out.WriteLine($"player {player} -> force {target}: ok={result.Ok} {result.Message}");
        if (!result.Ok) { _out.WriteLine("refused, nothing to verify"); return; }

        var back = PlayerForceCommand.GetForces(Reload(doc));
        Assert.Contains(player, back[target].PlayerIds);
        // And nowhere else, since a move that only adds is a duplicate rather than a move.
        for (int i = 0; i < back.Count; i++)
            if (i != target)
                Assert.DoesNotContain(player, back[i].PlayerIds);
    }

    // ---- placed units and doodads --------------------------------------------------------

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_placed_units_owner_reads_back()
    {
        if (RealMap() is not { } doc) return;

        var unit = UnitInstanceCommand.List(doc).FirstOrDefault();
        if (unit is null) { _out.WriteLine("no placed units, skipped"); return; }

        int newOwner = unit.OwnerId == 5 ? 6 : 5;
        var result = UnitInstanceCommand.SetOwner(doc, unit.CreationNumber, newOwner);
        _out.WriteLine($"unit #{unit.CreationNumber} owner {unit.OwnerId} -> {newOwner}: "
                     + $"ok={result.Ok} {result.Message}");
        if (!result.Ok) { Assert.Fail(result.Message); return; }

        var back = UnitInstanceCommand.Get(Reload(doc), unit.CreationNumber);
        Assert.NotNull(back);
        Assert.Equal(newOwner, back!.OwnerId);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_placed_doodads_position_reads_back()
    {
        if (RealMap() is not { } doc) return;

        var doodad = DoodadInstanceCommand.List(doc).FirstOrDefault();
        if (doodad is null) { _out.WriteLine("no placed doodads, skipped"); return; }

        const float X = 1234.5f, Y = -678.25f;
        var result = DoodadInstanceCommand.SetPosition(doc, doodad.CreationNumber, X, Y, doodad.Z);
        _out.WriteLine($"doodad #{doodad.CreationNumber} -> ({X}, {Y}): "
                     + $"ok={result.Ok} {result.Message}");
        if (!result.Ok) { Assert.Fail(result.Message); return; }

        var back = DoodadInstanceCommand.Get(Reload(doc), doodad.CreationNumber);
        Assert.NotNull(back);
        Assert.Equal(X, back!.X, 2);
        Assert.Equal(Y, back.Y, 2);
    }

    // ---- terrain and pathing -------------------------------------------------------------

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_terrain_deform_survives_a_save()
    {
        if (RealMap() is not { } doc) return;

        var before = TerrainCommand.Stats(doc);
        var result = TerrainCommand.Deform(doc, 0, 0, 4, TerrainCommand.HeightOp.Raise, amount: 64f);
        _out.WriteLine($"deform ok={result.Ok} {result.Message}");
        if (!result.Ok) { Assert.Fail(result.Message); return; }

        var after = TerrainCommand.Stats(Reload(doc));
        _out.WriteLine($"height range {before.MinHeight}..{before.MaxHeight} "
                     + $"-> {after.MinHeight}..{after.MaxHeight}");

        // Raising ground must move the maximum, and must not silently rewrite the whole grid.
        Assert.True(after.MaxHeight > before.MaxHeight,
            $"max height did not rise, {before.MaxHeight} -> {after.MaxHeight}");
        // And the grid itself is untouched, since a deform that rewrote every tile would also
        // raise the maximum and look like a pass.
        Assert.Equal(before.TileCount, after.TileCount);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_pathing_paint_survives_a_save()
    {
        if (RealMap() is not { } doc) return;

        var wpm = doc.GetFile("war3map.wpm");
        if (wpm is null) { _out.WriteLine("map has no war3map.wpm, skipped"); return; }
        var before = wpm.CurrentBytes.ToArray();

        var result = PathingCommand.Paint(doc, 0, 0, 4, PathingType.Walk);
        _out.WriteLine($"pathing paint ok={result.Ok} {result.Message}");
        if (!result.Ok) { Assert.Fail(result.Message); return; }

        var after = Reload(doc).GetFile("war3map.wpm")!.CurrentBytes;
        _out.WriteLine($"war3map.wpm {before.Length:N0} -> {after.Length:N0} bytes, "
                     + $"{before.Zip(after).Count(p => p.First != p.Second)} byte(s) differ");

        Assert.Equal(before.Length, after.Length);   // the grid must never change size

        // Differ if and only if cells changed. Asserting the bytes MUST differ was wrong, the
        // first run painted Walk over cells that already had it and legitimately changed nothing,
        // reporting "0 cells changed". This pins the real contract in both directions, so a paint
        // that claims a change without making one fails too.
        bool bytesDiffer = !before.SequenceEqual(after);
        Assert.Equal(result.CellsChanged > 0, bytesDiffer);
    }
}
