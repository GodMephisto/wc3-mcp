// tests/Wc3.Tests/McpContractTests.cs
// Contract facts that are not protocol plumbing: the log level the server runs at (ENV-05).
// The initialize capabilities are measured from the published exe by tools/measure_surface.py,
// because the pinned SDK derives them from the registered collections and ignores the options.
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Wc3.Commands;
using Wc3.Mcp;

namespace Wc3.Tests;

public class McpContractTests
{
    [Theory]
    [InlineData("error", LogLevel.Error)]
    [InlineData("warn", LogLevel.Warning)]
    [InlineData(" WARN ", LogLevel.Warning)]
    [InlineData("warning", LogLevel.Warning)]
    [InlineData("info", LogLevel.Information)]
    [InlineData("debug", LogLevel.Debug)]
    [InlineData("trace", LogLevel.Warning)]
    [InlineData("payload", LogLevel.Warning)]
    [InlineData("", LogLevel.Warning)]
    [InlineData(null, LogLevel.Warning)]
    public void Wc3_mcp_log_maps_to_a_level_and_an_unknown_value_falls_back_to_warn(string? value, LogLevel expected)
    {
        Assert.Equal(expected, McpLogLevel.Parse(value));
    }

    [Fact]
    public void The_start_line_carries_the_build_the_target_and_the_profile()
    {
        string line = Wc3McpServer.BuildStartLine();

        Assert.Contains(Wc3.Mcp.ServerIdentity.ProductId, line);
        Assert.Contains(Wc3.Mcp.ServerIdentity.Version, line);
        Assert.Contains("protocol ", line);
        Assert.Contains("tools " + Wc3McpServer.ToolCount(), line);
        Assert.Contains("target ", line);
        Assert.Contains("game ", line);
        Assert.Contains("profile " + Wc3McpServer.DefaultProfile, line);
        Assert.DoesNotContain("\n", line);
    }

    [Fact]
    public async Task The_stated_protocol_revision_is_the_one_the_handshake_answers()
    {
        await McpTestClient.WithClient((client, ct) =>
        {
            Assert.Equal(Wc3McpServer.ProtocolRevision, client.NegotiatedProtocolVersion);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public void The_instructions_carry_the_product_the_rules_and_the_routing_hint()
    {
        string text = Wc3McpServer.CreateOptions().ServerInstructions ?? "";

        Assert.Contains(Wc3.Mcp.ServerIdentity.ProductId, text);
        Assert.Contains("target", text);
        Assert.Contains(".edited", text);
        Assert.Contains("quickstart", text);
        Assert.True(text.Length < 1000, "the instructions are " + text.Length + " characters");
    }

    [Fact]
    public void A_folder_is_not_a_map()
    {
        string folder = Path.Combine(Path.GetTempPath(), "wc3-map-folder-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            var error = Assert.Throws<ModelContextProtocol.McpException>(() => Wc3Tools.MapInfo(folder));
            Assert.Contains("folder", error.Message);
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void A_destructive_tool_refuses_without_confirm_and_names_what_would_go()
    {
        var error = Assert.Throws<ModelContextProtocol.McpException>(
            () => Wc3Tools.PlacedUnitRemove("Z:/nowhere/missing.w3x", 7));

        Assert.Contains("CONFIRM_REQUIRED", error.Message);
        Assert.Contains("removes placed unit 7", error.Message);
    }

    [Fact]
    public void A_destructive_tool_with_confirm_reaches_the_map_load()
    {
        // The guard runs before the load, so with the flag the failure is the missing map and
        // not the confirmation. That ordering is what makes the refusal free.
        var error = Assert.Throws<ModelContextProtocol.McpException>(
            () => Wc3Tools.PlacedUnitRemove("Z:/nowhere/missing.w3x", 7, confirm: true));

        Assert.DoesNotContain("CONFIRM_REQUIRED", error.Message);
        Assert.Contains("map not found", error.Message);
    }

    [Fact]
    public void Every_quickstart_workflow_has_a_prompt()
    {
        // PRM-02. A workflow a caller can route to with the quickstart tool must also have a plan
        // to read, otherwise the router names a job and stops there.
        var workflows = QuickstartCommand.All.Select(w => w.Id).ToList();
        Assert.Equal(12, workflows.Count);

        var missing = workflows.Where(id => !Wc3Prompts.WorkflowPrompts.ContainsKey(id)).ToList();
        Assert.True(missing.Count == 0, "workflows with no prompt: " + string.Join(", ", missing));

        var declared = typeof(Wc3Prompts)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(m => m.GetCustomAttribute<McpServerPromptAttribute>()?.Name)
            .Where(n => n is not null)
            .ToHashSet(StringComparer.Ordinal);

        var unknown = Wc3Prompts.WorkflowPrompts.Values.Where(v => !declared.Contains(v)).ToList();
        Assert.True(unknown.Count == 0, "mapped prompts that do not exist: " + string.Join(", ", unknown));
        Assert.Equal(13, declared.Count);
    }

    [Fact]
    public void Every_prompt_is_pullable_with_no_arguments_and_names_real_tools()
    {
        // PRM-01, PRM-03 and PRM-04. A prompt is pulled before a map is chosen, so every argument
        // must have a default, the text must number its steps, and every name it tells the caller to
        // call must be a tool that exists. The parameter names a prompt may mention are listed here,
        // which is what keeps the check from flagging legitimate text such as out_path.
        var surface = McpToolFamilies.All.Keys.ToHashSet(StringComparer.Ordinal);
        var parameterNames = new HashSet<string>(StringComparer.Ordinal) { "out_path" };
        var methods = typeof(Wc3Prompts)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<McpServerPromptAttribute>() is not null)
            .ToList();
        Assert.Equal(13, methods.Count);

        foreach (var method in methods)
        {
            foreach (var parameter in method.GetParameters())
                Assert.True(parameter.HasDefaultValue, method.Name + " has a required argument " + parameter.Name);

            var args = method.GetParameters().Select(x => x.HasDefaultValue ? x.DefaultValue : null).ToArray();
            string text = (string)method.Invoke(null, args)!;
            Assert.False(string.IsNullOrWhiteSpace(text));
            Assert.Contains("1.", text);
            Assert.Contains("2.", text);

            var names = Regex.Matches(text, "[a-z][a-z0-9]*(?:_[a-z0-9]+)+")
                .Select(m => m.Value).Distinct().ToList();
            Assert.True(names.Count(surface.Contains) >= 2,
                method.Name + " names fewer than two tools");

            var undefined = names.Where(n => !surface.Contains(n) && !parameterNames.Contains(n)).ToList();
            Assert.True(undefined.Count == 0,
                method.Name + " names something that is neither a tool nor a parameter: " + string.Join(", ", undefined));
        }
    }

    [Fact]
    public void Every_family_is_reachable_from_a_quickstart_workflow()
    {
        // DSC-02. A family that no workflow names is a part of the surface a newcomer cannot reach
        // from the documented route, and a workflow that names a tool which no longer exists sends
        // the caller to a dead end.
        var reached = QuickstartCommand.All.SelectMany(w => w.McpTools).Distinct().ToList();

        var missing = McpToolFamilies.Families
            .Select(f => f.Id)
            .Where(id => !reached.Any(t => McpToolFamilies.Of(t) == id))
            .ToList();
        Assert.True(missing.Count == 0, "families with no workflow: " + string.Join(", ", missing));

        var unknown = reached.Where(t => McpToolFamilies.Of(t) is null).ToList();
        Assert.True(unknown.Count == 0, "workflow tools that are not on the surface: " + string.Join(", ", unknown));

        // DSC-02's other half. Each workflow names the prompt that carries the same plan, and that
        // name must be a prompt the server actually serves.
        var served = typeof(Wc3Prompts)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(m => m.GetCustomAttribute<McpServerPromptAttribute>()?.Name)
            .Where(n => n is not null)
            .ToHashSet(StringComparer.Ordinal);
        var noPrompt = QuickstartCommand.All.Where(w => string.IsNullOrWhiteSpace(w.Prompt)).Select(w => w.Id).ToList();
        Assert.True(noPrompt.Count == 0, "workflows with no prompt name: " + string.Join(", ", noPrompt));
        var deadPrompt = QuickstartCommand.All.Where(w => !served.Contains(w.Prompt)).Select(w => w.Prompt).ToList();
        Assert.True(deadPrompt.Count == 0, "workflows naming a prompt that is not served: " + string.Join(", ", deadPrompt));
    }

    [Fact]
    public void The_default_level_is_warn()
    {
        // No variable set on the build machine, so FromEnvironment reads nothing and lands on warn.
        Assert.Equal(LogLevel.Warning, McpLogLevel.Parse(null));
    }
}
