// tests/Wc3.Tests/PermissionPolicyTests.cs
// SAF-17 and SAF-18. The policy file names the tools refused outright and the ones that need a
// person's approval, and the request filter applies it before a tool body runs. A denial beats a
// call carrying confirm true, and a missing file leaves the surface exactly as it is today.
//
// Only the missing file test touches WC3_MCP_PERMISSIONS, and only to point it at a path that does
// not exist, so no other collection can see a rule it did not write. The class still joins the
// Serial collection because an environment variable is process wide.
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Mcp;

namespace Wc3.Tests;

[Collection("Serial")]
public sealed class PermissionPolicyTests : IDisposable
{
    private readonly string _dir;
    private readonly string? _saved;

    public PermissionPolicyTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "wc3policy-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        _saved = Environment.GetEnvironmentVariable(PermissionPolicy.EnvironmentVariable);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(PermissionPolicy.EnvironmentVariable, _saved);
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    private string WritePolicy(string json, string name = "permissions.json")
    {
        string path = Path.Combine(_dir, name);
        File.WriteAllText(path, json);
        return path;
    }

    private static McpServer BareServer() =>
        McpServer.Create(
            new StreamServerTransport(new MemoryStream(), new MemoryStream()),
            Wc3McpServer.CreateOptions(),
            loggerFactory: null,
            serviceProvider: null);

    private static RequestContext<CallToolRequestParams> Context(
        McpServer server, string tool, long id, bool? confirm = null)
    {
        // An object initializer works whether the SDK exposes the property with a setter or an init.
        var parameters = new CallToolRequestParams
        {
            Name = tool,
            Arguments = confirm is { } value
                ? new Dictionary<string, JsonElement>
                {
                    [PermissionPolicy.ConfirmArgument] = JsonSerializer.SerializeToElement(value),
                }
                : null,
        };
        return new RequestContext<CallToolRequestParams>(
            server, new JsonRpcRequest { Id = new RequestId(id), Method = "tools/call" })
        {
            Params = parameters,
        };
    }

    /// <summary>The single text block a refusal carries, with the code still on the front.</summary>
    private static string Text(CallToolResult result) =>
        Assert.IsType<TextContentBlock>(Assert.Single(result.Content!)).Text ?? "";

    [Fact]
    public async Task A_denied_tool_is_refused_with_POLICY_DENIED_naming_the_tool_and_the_list()
    {
        string path = WritePolicy("{\"deny\":[\"map_info\"],\"note\":\"no reads until the review\"}");
        var policy = PermissionPolicy.Load(path, _ => { });
        Assert.True(policy.Present);
        Assert.Null(policy.Fault);

        bool ran = false;
        var chain = PermissionPolicy.Filter(policy)((request, token) =>
        {
            ran = true;
            return ValueTask.FromResult(new CallToolResult());
        });

        await using McpServer server = BareServer();
        CallToolResult result = await chain(Context(server, "map_info", 9101), CancellationToken.None);

        Assert.True(result.IsError);
        string text = Text(result);
        Assert.StartsWith("[POLICY_DENIED]", text, StringComparison.Ordinal);
        Assert.Contains("map_info", text);
        Assert.Contains("\"" + PermissionPolicy.DenyKey + "\"", text);
        Assert.Contains(path, text);
        Assert.False(ran, "the tool body ran for a tool the file denies");

        // A family id as the rule matches every tool in that family, which is what the schema is for.
        string familyPath = WritePolicy("{\"deny\":[\"terrain\"]}", "family.json");
        var byFamily = PermissionPolicy.Load(familyPath, _ => { });
        Assert.Equal("terrain", byFamily.DeniedRule("terrain_smooth"));
        Assert.Equal("terrain", byFamily.DeniedRule("terrain_fill"));
        Assert.Null(byFamily.DeniedRule("map_info"));

        // The surface registers the policy filter on the call tool pipeline, so a client reaches it
        // without naming a file and the same refusal is what a bug report sees. Four wrappers carry
        // the aliases, the policy, the error result and the call clock, and ERR-06 adds the schema
        // check.
        var options = Wc3McpServer.CreateOptions();
        Assert.Equal(5, options.Filters.Request.CallToolFilters.Count);
    }

    [Fact]
    public async Task A_denied_tool_is_refused_even_when_the_call_carries_confirm_true()
    {
        string path = WritePolicy("{\"deny\":[\"terrain_fill\"]}");
        var policy = PermissionPolicy.Load(path, _ => { });

        bool ran = false;
        var chain = PermissionPolicy.Filter(policy)((request, token) =>
        {
            ran = true;
            return ValueTask.FromResult(new CallToolResult());
        });

        await using McpServer server = BareServer();
        CallToolResult result = await chain(
            Context(server, "terrain_fill", 9102, confirm: true), CancellationToken.None);

        Assert.True(result.IsError);
        string text = Text(result);
        Assert.StartsWith("[POLICY_DENIED]", text, StringComparison.Ordinal);
        Assert.Contains("terrain_fill", text);
        Assert.Contains("never grants permission", text);
        Assert.Contains("confirm true", text);
        Assert.False(ran, "confirm true opened a rule the file denies");
    }

    [Fact]
    public async Task A_tool_under_require_approval_is_refused_without_confirm_and_allowed_with_it()
    {
        string path = WritePolicy("{\"require_approval\":[\"assets\"]}");
        var policy = PermissionPolicy.Load(path, _ => { });
        Assert.Equal("assets", policy.ApprovalRule("model_optimize"));

        var ran = new List<string>();
        McpRequestHandler<CallToolRequestParams, CallToolResult> handler = (request, token) =>
        {
            ran.Add(request.Params!.Name!);
            return ValueTask.FromResult(new CallToolResult { Content = new List<ContentBlock>() });
        };
        var chain = PermissionPolicy.Filter(policy)(handler);

        await using McpServer server = BareServer();

        CallToolResult withoutConfirm = await chain(
            Context(server, "model_optimize", 9103), CancellationToken.None);

        Assert.True(withoutConfirm.IsError);
        string text = Text(withoutConfirm);
        Assert.StartsWith("[POLICY_DENIED]", text, StringComparison.Ordinal);
        Assert.Contains("model_optimize", text);
        Assert.Contains("\"" + PermissionPolicy.ApprovalKey + "\"", text);
        Assert.Contains("\"assets\"", text);
        Assert.Contains("file decides", text);
        Assert.Contains("only states intent", text);
        Assert.Empty(ran);

        CallToolResult withConfirm = await chain(
            Context(server, "model_optimize", 9104, confirm: true), CancellationToken.None);

        Assert.NotEqual(true, withConfirm.IsError);
        Assert.Equal(new[] { "model_optimize" }, ran);
    }

    [Fact]
    public async Task A_missing_file_allows_every_tool()
    {
        // The default is the local app data wc3ctl folder, outside the executable folder, which is
        // what the search order falls back to when the environment names nothing.
        Environment.SetEnvironmentVariable(PermissionPolicy.EnvironmentVariable, null);
        var byDefault = PermissionPolicy.Load(notice: _ => { });
        Assert.Equal(PermissionPolicy.DefaultPath(), byDefault.PathName);
        Assert.Contains(Path.Combine("wc3ctl", "permissions.json"), byDefault.PathName);
        Assert.False(
            byDefault.PathName.StartsWith(AppContext.BaseDirectory, StringComparison.OrdinalIgnoreCase),
            "the default path must sit outside the executable folder");

        string missing = Path.Combine(_dir, "not-there.json");
        Environment.SetEnvironmentVariable(PermissionPolicy.EnvironmentVariable, missing);

        var policy = PermissionPolicy.Load(notice: _ => { });

        Assert.Equal(missing, policy.PathName);
        Assert.False(policy.Present);
        Assert.Null(policy.Fault);
        Assert.Null(policy.IgnoredReason);
        Assert.Empty(policy.Deny);
        Assert.Empty(policy.RequireApproval);
        Assert.Contains("absent", policy.Describe());

        bool ran = false;
        var chain = PermissionPolicy.Filter(policy)((request, token) =>
        {
            ran = true;
            return ValueTask.FromResult(new CallToolResult());
        });

        await using McpServer server = BareServer();
        CallToolResult result = await chain(
            Context(server, "terrain_fill", 9105, confirm: true), CancellationToken.None);

        Assert.NotEqual(true, result.IsError);
        Assert.True(ran, "a missing file must leave the surface as it is");
        Assert.Null(policy.RefusalFor("mpq_rebuild", confirm: false));
    }

    [Fact]
    public void A_file_inside_the_executable_folder_is_ignored_with_a_line_saying_why()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory, "permissions-ignored-" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(path, "{\"deny\":[\"map_info\"]}");
        var lines = new List<string>();

        try
        {
            var policy = PermissionPolicy.Load(path, lines.Add);

            Assert.False(policy.Present, "a file inside the executable folder must not apply");
            Assert.Null(policy.Fault);
            Assert.NotNull(policy.IgnoredReason);
            Assert.Contains("executable folder", policy.IgnoredReason!);
            Assert.Empty(policy.Deny);
            Assert.Null(policy.RefusalFor("map_info", confirm: false));

            string line = Assert.Single(lines);
            Assert.Contains(path, line);
            Assert.Contains("ignored", line);
            Assert.Contains("executable folder", line);
            Assert.Contains("outside every folder the server writes", line);
            Assert.Contains("ignored", policy.Describe());
        }
        finally
        {
            try { File.Delete(path); } catch { /* best effort */ }
        }
    }

}
