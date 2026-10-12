// tests/Wc3.Tests/ReplayDetailCommandTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// The five replay report commands. Each is driven by a synthetic .w3g written to a temp file, so
/// the file parse is exercised too, and the pure resolution, ordering, diffing and aggregation are
/// exercised directly over an in memory record set. Team, race and action counts are not carried by
/// this parser, so those axes are covered by the pure builders rather than the parse.
/// </summary>
public class ReplayDetailCommandTests
{
    private static string Temp(byte[] bytes)
    {
        string path = Path.Combine(Path.GetTempPath(), $"wc3ctl-replay-{Guid.NewGuid():N}.w3g");
        return ReplayFixtures.Write(path, bytes);
    }

    private static ReplayGame Game(
        string map = @"Maps\Download\BaseMap_Repro.w3x",
        TimeSpan? length = null,
        ReplayPlayer[]? players = null,
        ReplayLeave[]? leaves = null,
        ReplayChat[]? chats = null,
        string? problem = null) => new(
        "x.w3g", DateTime.UnixEpoch, map, length ?? TimeSpan.FromSeconds(60), 7000,
        players ?? Array.Empty<ReplayPlayer>(),
        leaves ?? Array.Empty<ReplayLeave>(),
        chats ?? Array.Empty<ReplayChat>(),
        leaves?.Any(l => l.Result == 0x01) ?? false, problem);

    // ---- players ------------------------------------------------------------------------------

    [Fact]
    public void Players_lists_each_player_with_result_and_leave_time()
    {
        string path = Temp(ReplayFixtures.Build(events: new ReplayFixtures.Event[]
        {
            new ReplayFixtures.Leave(1000, 2, 0x0C, 0x07),
        }));
        try
        {
            var r = ReplayPlayersCommand.Execute(path);

            Assert.Null(r.Problem);
            Assert.Equal(2, r.Players.Count);
            var host = r.Players.Single(p => p.Slot == 1);
            Assert.Equal("Host#1", host.Name);
            Assert.Null(host.Result);
            var guest = r.Players.Single(p => p.Slot == 2);
            Assert.Equal("Guest#2", guest.Name);
            Assert.Equal(7, guest.Result);
            Assert.Equal("left", guest.LeaveMeaning);
            Assert.Equal(TimeSpan.FromSeconds(1), guest.LeftAt);
            Assert.Equal(0x0C, guest.LeaveReason);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Players_resolves_team_and_race_when_the_record_carries_them()
    {
        // The parser does not populate these, so the resolution is pinned directly.
        var g = Game(players: new[]
        {
            new ReplayPlayer(1, "Host#1", 0, "Human", null),
            new ReplayPlayer(2, "Guest#2", 1, "Orc", null),
        });

        var rows = ReplayPlayersCommand.Resolve(g);

        Assert.Equal(0, rows.Single(p => p.Slot == 1).Team);
        Assert.Equal("Human", rows.Single(p => p.Slot == 1).Race);
        Assert.Equal(1, rows.Single(p => p.Slot == 2).Team);
        Assert.Equal("Orc", rows.Single(p => p.Slot == 2).Race);
    }

    [Fact]
    public void Players_uses_the_last_leave_for_a_slot()
    {
        var g = Game(
            players: new[] { new ReplayPlayer(1, "Host#1", null, null, null) },
            leaves: new[]
            {
                new ReplayLeave(1, "Host#1", TimeSpan.FromSeconds(10), 0x0C, 0x07, "left"),
                new ReplayLeave(1, "Host#1", TimeSpan.FromSeconds(30), 0x0C, 0x09, "won"),
            });

        var row = Assert.Single(ReplayPlayersCommand.Resolve(g));
        Assert.Equal(TimeSpan.FromSeconds(30), row.LeftAt);
        Assert.Equal("won", row.LeaveMeaning);
    }

    // ---- timeline -----------------------------------------------------------------------------

    [Fact]
    public void Timeline_orders_start_chat_leave_and_end()
    {
        string path = Temp(ReplayFixtures.Build(lengthMs: 2000, events: new ReplayFixtures.Event[]
        {
            new ReplayFixtures.Chat(500, 1, "hello"),
            new ReplayFixtures.Leave(1000, 2, 0x0C, 0x07),
        }));
        try
        {
            var r = ReplayTimelineCommand.Execute(path);

            Assert.Null(r.Problem);
            Assert.Equal(new[] { "game start", "chat", "leave", "game end" },
                r.Events.Select(e => e.Kind).ToArray());
            Assert.Equal(TimeSpan.FromMilliseconds(500), r.Events[1].At);
            Assert.Equal("hello", r.Events[1].Text);
            Assert.Equal(TimeSpan.FromSeconds(1), r.Events[2].At);
            Assert.Equal("left", r.Events[2].Text);
            Assert.Equal(TimeSpan.FromMilliseconds(2000), r.Events[3].At);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Timeline_keeps_game_start_before_a_chat_at_zero()
    {
        var g = Game(chats: new[] { new ReplayChat(TimeSpan.Zero, 1, "Host#1", "hi") });
        var r = ReplayTimelineCommand.From(g);
        Assert.Equal("game start", r.Events[0].Kind);
        Assert.Equal("chat", r.Events[1].Kind);
    }

    [Fact]
    public void Timeline_reports_an_unreadable_record_as_empty_with_its_problem()
    {
        var g = Game(problem: "not a Warcraft III replay");
        var r = ReplayTimelineCommand.From(g);
        Assert.Empty(r.Events);
        Assert.Equal("not a Warcraft III replay", r.Problem);
    }

    // ---- diff ---------------------------------------------------------------------------------

    [Fact]
    public void Diff_reports_same_map_length_delta_and_leave_delta()
    {
        string a = Temp(ReplayFixtures.Build(lengthMs: 1000, events: new ReplayFixtures.Event[]
        {
            new ReplayFixtures.Leave(500, 2, 0x0C, 0x07),
        }));
        string b = Temp(ReplayFixtures.Build(lengthMs: 1500, events: new ReplayFixtures.Event[]
        {
            new ReplayFixtures.Leave(900, 2, 0x0C, 0x07),
        }));
        try
        {
            var r = ReplayDiffCommand.Execute(a, b);

            Assert.True(r.SameMap);
            Assert.Equal(TimeSpan.FromMilliseconds(1000), r.LengthA);
            Assert.Equal(TimeSpan.FromMilliseconds(1500), r.LengthB);
            Assert.Equal(TimeSpan.FromMilliseconds(500), r.LengthDelta);
            var slot2 = r.LeaveDeltas.Single(d => d.PlayerId == 2);
            Assert.Equal(TimeSpan.FromMilliseconds(400), slot2.Delta);
        }
        finally { File.Delete(a); File.Delete(b); }
    }

    [Fact]
    public void Diff_reports_a_different_map()
    {
        var r = ReplayDiffCommand.Compare(
            Game(map: @"Maps\Download\A.w3x"),
            Game(map: @"Maps\Download\B.w3x"));
        Assert.False(r.SameMap);
    }

    [Fact]
    public void Diff_leaves_a_one_sided_leave_delta_null()
    {
        var r = ReplayDiffCommand.Compare(
            Game(players: new[] { new ReplayPlayer(1, "Host#1", null, null, null) },
                leaves: new[] { new ReplayLeave(1, "Host#1", TimeSpan.FromSeconds(5), 0x0C, 0x07, "left") }),
            Game(players: new[] { new ReplayPlayer(1, "Host#1", null, null, null) }));

        var delta = Assert.Single(r.LeaveDeltas);
        Assert.Equal(TimeSpan.FromSeconds(5), delta.AtA);
        Assert.Null(delta.AtB);
        Assert.Null(delta.Delta);
    }

    // ---- find-map -----------------------------------------------------------------------------

    [Fact]
    public void Find_map_resolves_the_recorded_map_in_the_folder()
    {
        string folder = Path.Combine(Path.GetTempPath(), $"wc3ctl-maps-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(folder, "Download"));
        string mapFile = Path.Combine(folder, "Download", "BaseMap_Repro.w3x");
        File.WriteAllBytes(mapFile, new byte[] { 1 });
        string replay = Temp(ReplayFixtures.Build(map: @"Maps\Download\BaseMap_Repro.w3x"));
        try
        {
            var r = ReplayFindMapCommand.Execute(replay, folder);

            Assert.True(r.Found);
            Assert.Equal(mapFile, r.ResolvedPath);
            Assert.Equal(@"Maps\Download\BaseMap_Repro.w3x", r.CleanMapPath);
        }
        finally
        {
            File.Delete(replay);
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void Find_map_reports_not_found_when_the_map_is_absent()
    {
        string folder = Path.Combine(Path.GetTempPath(), $"wc3ctl-maps-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        string replay = Temp(ReplayFixtures.Build());
        try
        {
            var r = ReplayFindMapCommand.Execute(replay, folder);
            Assert.False(r.Found);
            Assert.Null(r.ResolvedPath);
        }
        finally
        {
            File.Delete(replay);
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void Find_map_prefers_a_full_path_match_over_a_bare_name()
    {
        string folder = Path.Combine(Path.GetTempPath(), $"wc3ctl-maps-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(folder, "Download"));
        Directory.CreateDirectory(Path.Combine(folder, "Other"));
        string want = Path.Combine(folder, "Download", "BaseMap_Repro.w3x");
        File.WriteAllBytes(want, new byte[] { 1 });
        File.WriteAllBytes(Path.Combine(folder, "Other", "BaseMap_Repro.w3x"), new byte[] { 2 });
        try
        {
            var (resolved, candidates) = ReplayFindMapCommand.Resolve(
                @"Maps\Download\BaseMap_Repro.w3x", folder);
            Assert.Equal(want, resolved);
            Assert.Equal(2, candidates.Count);
        }
        finally { Directory.Delete(folder, recursive: true); }
    }

    // ---- stats --------------------------------------------------------------------------------

    [Fact]
    public void Stats_aggregates_duration_players_and_disconnects()
    {
        string path = Temp(ReplayFixtures.Build(lengthMs: 120_000, events: new ReplayFixtures.Event[]
        {
            new ReplayFixtures.Leave(3000, 2, 0x0C, 0x01),
        }));
        try
        {
            var r = ReplayStatsCommand.Execute(path);

            Assert.Equal(TimeSpan.FromMinutes(2), r.Duration);
            Assert.Equal(2, r.PlayerCount);
            Assert.Equal(1, r.Disconnects);
            Assert.False(r.ActionsAvailable);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Stats_computes_actions_per_minute_when_carried()
    {
        var g = Game(
            length: TimeSpan.FromMinutes(2),
            players: new[] { new ReplayPlayer(1, "Host#1", null, null, 120) });

        var r = ReplayStatsCommand.Summarize(g);

        Assert.True(r.ActionsAvailable);
        Assert.Equal(60d, Assert.Single(r.Players).ActionsPerMinute);
    }

    [Fact]
    public void Stats_reports_an_absent_action_count_as_unavailable_not_zero()
    {
        var g = Game(
            length: TimeSpan.FromMinutes(2),
            players: new[] { new ReplayPlayer(1, "Host#1", null, null, null) });

        var r = ReplayStatsCommand.Summarize(g);

        Assert.False(r.ActionsAvailable);
        Assert.Null(Assert.Single(r.Players).ActionsPerMinute);
    }

    // ---- the parse path the commands share ----------------------------------------------------

    [Fact]
    public void Parse_captures_chat_lines_with_their_time_and_player()
    {
        string path = Temp(ReplayFixtures.Build(events: new ReplayFixtures.Event[]
        {
            new ReplayFixtures.Chat(750, 2, "gg"),
        }));
        try
        {
            var g = ReplayCommand.ReadGame(path);

            Assert.Null(g.Problem);
            var chat = Assert.Single(g.Chats);
            Assert.Equal("gg", chat.Text);
            Assert.Equal(2, chat.PlayerId);
            Assert.Equal("Guest#2", chat.Name);
            Assert.Equal(TimeSpan.FromMilliseconds(750), chat.At);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Chat_text_drops_the_five_byte_prefix()
    {
        var raw = new byte[] { 0x20, 0, 0, 0, 0 }.Concat(System.Text.Encoding.UTF8.GetBytes("hi\0")).ToArray();
        Assert.Equal("hi", ReplayCommand.ChatText(raw));
    }

    [Fact]
    public void Clean_map_path_trims_to_the_maps_root()
    {
        Assert.Equal(@"Maps\Download\A.w3x", ReplayCommand.CleanMapPath("\uFFFD\uFFFDABQMaps\\Download\\A.w3x"));
        Assert.Equal("Maps/Download/A.w3x", ReplayCommand.CleanMapPath("junkMaps/Download/A.w3x"));
        Assert.Null(ReplayCommand.CleanMapPath(null));
    }
}
