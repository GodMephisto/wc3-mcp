// tests/Wc3.Tests/PermissionsTests.cs
// The permission file is the user's, and the agent can neither grant a switch nor write the file.
// These facts pin the defaults, the refusals and the three skills the server serves as resources,
// and every one of them points WC3_MCP_PERMISSIONS at a temporary folder so the machine's own
// permissions.json is never read or written. The class joins the Serial collection because an
// environment variable is process wide.
using System.Reflection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Commands;
using Wc3.Mcp;
using Wc3.Model;

namespace Wc3.Tests;

[Collection("Serial")]
public sealed class PermissionsTests : IDisposable
{
    private readonly string _dir;
    private readonly string _permissionFile;
    private readonly string? _saved;

    public PermissionsTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "wc3perm-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        _permissionFile = Path.Combine(_dir, "permissions.json");

        _saved = Environment.GetEnvironmentVariable(Permissions.EnvironmentVariable);
        Environment.SetEnvironmentVariable(Permissions.EnvironmentVariable, _permissionFile);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(Permissions.EnvironmentVariable, _saved);
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    private void WriteFile(string json) => File.WriteAllText(_permissionFile, json);

    private static ToolError Refusal(Action action) => Assert.Throws<ToolError>(action);

    [Fact]
    public void No_file_means_every_action_is_denied()
    {
        // Nothing was written, so the path in the environment names a file that is not there.
        Assert.False(File.Exists(_permissionFile));

        var permissions = Permissions.Current;

        Assert.False(permissions.Present);
        Assert.Null(permissions.Fault);
        foreach (string action in Permissions.Actions)
            Assert.False(permissions.IsAllowed(action), action + " must be denied with no file");
        Assert.Contains("absent", permissions.Describe());
        Assert.Contains(_permissionFile, permissions.Describe());
    }

    [Fact]
    public void A_file_that_grants_overwrite_allows_it()
    {
        WriteFile("{\"overwrite\": true}");

        var permissions = Permissions.Current;

        Assert.True(permissions.IsAllowed("overwrite"));
        Assert.True(permissions.IsAllowed("OVERWRITE"), "a switch name is matched without regard to case");
        Assert.Contains("overwrite granted", permissions.Describe());
    }

    [Fact]
    public void Every_switch_defaults_to_false_when_the_file_omits_it()
    {
        // One switch is granted and the other four are absent from the file.
        WriteFile("{\"publish\": true}");

        var permissions = Permissions.Current;

        Assert.True(permissions.IsAllowed("publish"));
        Assert.False(permissions.IsAllowed("overwrite"));
        Assert.False(permissions.IsAllowed("mirror_export"));
        Assert.False(permissions.IsAllowed("npm"));
        Assert.False(permissions.IsAllowed("delete"));
        Assert.Contains("overwrite denied, the switch is absent", permissions.Describe());
    }

    [Fact]
    public void An_unknown_action_is_denied()
    {
        WriteFile("{\"overwrite\": true}");

        var permissions = Permissions.Current;

        Assert.False(permissions.IsAllowed("delete_everything"));
        Assert.False(permissions.IsAllowed("grant"));
    }

    [Fact]
    public void A_false_switch_is_denied_and_says_so()
    {
        WriteFile("{\"overwrite\": false}");

        var permissions = Permissions.Current;

        Assert.False(permissions.IsAllowed("overwrite"));
        Assert.Contains("overwrite denied, the switch is false", permissions.Describe());
    }

    [Fact]
    public void Malformed_json_is_refused_with_a_clear_message()
    {
        WriteFile("{ overwrite: true ");

        var permissions = Permissions.Current;

        Assert.NotNull(permissions.Fault);
        Assert.Contains("not valid JSON", permissions.Fault);
        Assert.Contains(_permissionFile, permissions.Fault, StringComparison.Ordinal);

        var refusal = Refusal(() => permissions.IsAllowed("overwrite"));
        Assert.Equal("PERMISSION_DENIED", refusal.Code);
        Assert.Contains("not valid JSON", refusal.Message);
        Assert.Contains(_permissionFile, refusal.Message);
        Assert.Contains("invalid", permissions.Describe());
    }

    [Fact]
    public void A_file_that_is_not_an_object_of_switches_is_refused()
    {
        WriteFile("[\"overwrite\"]");

        var permissions = Permissions.Current;

        Assert.NotNull(permissions.Fault);
        Assert.Contains("JSON object of switches", permissions.Fault);
        Assert.Equal("PERMISSION_DENIED", Refusal(() => permissions.IsAllowed("overwrite")).Code);
    }

    [Fact]
    public void A_switch_with_a_non_boolean_value_is_refused_rather_than_read_as_true()
    {
        WriteFile("{\"overwrite\": \"yes\"}");

        var permissions = Permissions.Current;

        Assert.NotNull(permissions.Fault);
        Assert.Contains("\"overwrite\" switch is not true or false", permissions.Fault);
        Assert.Equal("PERMISSION_DENIED", Refusal(() => permissions.IsAllowed("overwrite")).Code);
    }

    [Fact]
    public void An_ignored_key_is_reported_and_grants_nothing()
    {
        WriteFile("{\"overwrit\": true}");

        var permissions = Permissions.Current;

        Assert.Null(permissions.Fault);
        Assert.Equal(new[] { "overwrit" }, permissions.IgnoredKeys);
        Assert.Contains("ignored unknown switches overwrit", permissions.Describe());
        Assert.False(permissions.IsAllowed("overwrit"));
        Assert.False(permissions.IsAllowed("overwrite"));
    }

    [Fact]
    public void WriteGate_refuses_an_overwrite_without_the_grant_and_writes_nothing()
    {
        string map = Path.Combine(_dir, "in.w3x");
        File.WriteAllBytes(map, BlankMap.CreateArchiveBytes());
        string output = Path.Combine(_dir, "out.w3x");
        byte[] sentinel = { 7, 7, 7 };
        File.WriteAllBytes(output, sentinel);

        var doc = MapDocument.Load(map);
        var refusal = Refusal(() => WriteGate.Save(doc, map, output, overwrite: true));

        Assert.Equal("PERMISSION_DENIED", refusal.Code);
        Assert.Contains("overwrite", refusal.Message);
        Assert.Contains(output, refusal.Message);
        Assert.Contains(_permissionFile, refusal.Message);
        Assert.Equal(sentinel, File.ReadAllBytes(output));
        Assert.Empty(Directory.GetFiles(_dir, "*.tmp"));
        Assert.Empty(Directory.GetFiles(_dir, "." + Path.GetFileName(output) + ".*"));
        Assert.Equal(new[] { map, output }.OrderBy(p => p, StringComparer.Ordinal),
            Directory.GetFiles(_dir).OrderBy(p => p, StringComparer.Ordinal));
    }

    [Fact]
    public void WriteGate_replaces_an_existing_output_once_overwrite_is_granted()
    {
        WriteFile("{\"overwrite\": true}");
        string map = Path.Combine(_dir, "in.w3x");
        File.WriteAllBytes(map, BlankMap.CreateArchiveBytes());
        string output = Path.Combine(_dir, "out.w3x");
        File.WriteAllBytes(output, new byte[] { 1 });

        var outcome = WriteGate.Save(MapDocument.Load(map), map, output, overwrite: true);

        Assert.Equal(output, outcome.SavedTo);
        Assert.Equal(WriteGate.Sha256File(output), outcome.OutputSha256);
        Assert.True(new FileInfo(output).Length > 3, "the replaced file must be a real map");
    }

    [Fact]
    public void WriteGate_writes_a_fresh_output_with_no_grant_because_nothing_is_replaced()
    {
        // The grant covers replacement. A new out_path destroys nothing and the write tools name
        // one for every edit, so it does not need a switch the user was never asked about.
        string map = Path.Combine(_dir, "in.w3x");
        File.WriteAllBytes(map, BlankMap.CreateArchiveBytes());
        string output = Path.Combine(_dir, "fresh.w3x");

        var outcome = WriteGate.Save(MapDocument.Load(map), map, output, overwrite: true);

        Assert.True(File.Exists(output));
        Assert.Equal(output, outcome.SavedTo);
    }

    [Fact]
    public async Task The_three_skills_are_served_as_non_empty_resources()
    {
        string[] uris = { "wc3://skills/map-safety", "wc3://skills/hero-package", "wc3://skills/publish" };

        await McpTestClient.WithClient(async (client, ct) =>
        {
            var resources = await client.ListResourcesAsync(cancellationToken: ct);
            foreach (string uri in uris)
                Assert.Contains(resources, r => r.Uri == uri);

            foreach (string uri in uris)
            {
                var contents = await client.ReadResourceAsync(uri, cancellationToken: ct);
                string text = Assert.IsType<TextResourceContents>(Assert.Single(contents.Contents)).Text;
                Assert.False(string.IsNullOrWhiteSpace(text), uri + " served no text");
                Assert.True(text.Length > 200, uri + " served " + text.Length + " characters");
                Assert.StartsWith("# ", text);
            }
        });
    }

    [Fact]
    public void The_skill_files_exist_beside_the_exe_and_both_copies_agree()
    {
        string[] ids = { "map-safety", "hero-package", "publish" };
        foreach (string id in ids)
        {
            string beside = Path.Combine(AppContext.BaseDirectory, "docs", "skills", id + ".md");
            string context = Path.Combine(AppContext.BaseDirectory, "context", "skills", id + ".md");

            Assert.True(File.Exists(beside), beside + " is missing");
            Assert.True(File.Exists(context), context + " is missing");
            Assert.Equal(File.ReadAllText(beside), File.ReadAllText(context));
            Assert.False(string.IsNullOrWhiteSpace(Wc3Skills.Read(id)));
        }
    }

    [Fact]
    public void The_skill_map_safety_names_the_three_rules()
    {
        string text = Wc3Skills.Read("map-safety");

        Assert.Contains("Read a map before you change it", text);
        Assert.Contains("Never overwrite the input", text);
        Assert.Contains("PERMISSION_DENIED", text);
    }

    [Fact]
    public void No_tool_grants_a_permission()
    {
        // The whole point of the file is that the agent cannot grant itself a switch. A tool whose
        // name carries permission, grant or the file's name must be read-only and non-destructive.
        var toolMethods = new[] { typeof(Wc3Tools), typeof(Wc3Context) }
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.GetCustomAttribute<McpServerToolAttribute>() is not null)
            .ToList();
        Assert.NotEmpty(toolMethods);

        string[] forbidden = { "permission", "grant", "granted", "permissions.json" };
        var suspicious = toolMethods
            .Where(m => forbidden.Any(word =>
                m.Name.Contains(word, StringComparison.OrdinalIgnoreCase)
                || (m.GetCustomAttribute<McpServerToolAttribute>()?.Name ?? "")
                    .Contains(word, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (var method in suspicious)
        {
            var attribute = method.GetCustomAttribute<McpServerToolAttribute>()!;
            Assert.True(attribute.ReadOnly == true,
                "tool " + method.Name + " mentions permissions and is not read-only");
            Assert.False(attribute.Destructive == true,
                "tool " + method.Name + " mentions permissions and is destructive");
        }
    }

    [Fact]
    public void The_served_switch_list_is_the_five_the_gate_knows()
    {
        // The parser refuses a non-boolean value for a known switch, so a new switch is a decision
        // recorded here rather than a key someone hopes works.
        Assert.Equal(
            new[] { "overwrite", "publish", "mirror_export", "npm", "delete" },
            Permissions.Actions);
    }
}
