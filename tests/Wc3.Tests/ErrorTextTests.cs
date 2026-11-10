// tests/Wc3.Tests/ErrorTextTests.cs
// ERR-06, the half a scanner can hold. An error travels to a model provider and may sit in a log a
// user pastes into an issue, so it may name a path and a rule and must never carry the body of the
// file it was reading. A map built with a distinctive marker in its script and its string table is
// the cheapest way to prove that, because the marker is a string no message could invent.
using ModelContextProtocol;
using Wc3.Mcp;

namespace Wc3.Tests;

public sealed class ErrorTextTests : IDisposable
{
    private const string Marker = "ZZMARKERZ_CONTENT_THAT_MUST_NOT_TRAVEL_ZZ";

    private readonly string _dir = Path.Combine(Path.GetTempPath(), "wc3err-" + Guid.NewGuid().ToString("N"));

    public ErrorTextTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private string BuildMarkedMap()
    {
        var map = Path.Combine(_dir, "marked.w3x");
        File.WriteAllBytes(map, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(
                "// " + Marker + "\nfunction main takes nothing returns nothing\nendfunction\n"),
            ["war3map.wts"] = System.Text.Encoding.UTF8.GetBytes(
                "STRING 1\n{\n" + Marker + "\n}\n"),
        }));
        return map;
    }

    [Fact]
    public void No_tool_error_carries_the_script_or_the_string_table_of_the_map_it_read()
    {
        string map = BuildMarkedMap();
        var calls = new (string Name, Func<object> Run)[]
        {
            ("file_get_text", () => Wc3Tools.FileGetText(map, "war3map.j.nope")),
            ("object_get", () => Wc3Tools.ObjectGet(map, "unit", "ZZZZ")),
            ("map_info", () => Wc3Tools.MapInfo(@"Z:\nowhere\missing.w3x")),
            ("mpq_remove", () => Wc3Tools.MpqRemove(map, "war3map.j.nope", null, confirm: true)),
        };

        foreach (var (name, run) in calls)
        {
            string message;
            try
            {
                run();
                continue;
            }
            catch (McpException e) { message = e.Message; }
            catch (Exception e) { message = e.Message; }

            Assert.False(message.Contains(Marker, StringComparison.Ordinal),
                name + " put map content in its error, " + message);
        }
    }

    [Fact]
    public void A_failed_call_names_the_path_or_the_rule()
    {
        string map = BuildMarkedMap();
        var e = Assert.Throws<McpException>(() => Wc3Tools.FileGetText(map, "war3map.j.nope"));
        Assert.False(string.IsNullOrWhiteSpace(e.Message));
        Assert.Contains("nope", e.Message, StringComparison.OrdinalIgnoreCase);
    }
}
