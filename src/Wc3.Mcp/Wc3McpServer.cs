// src/Wc3.Mcp/Wc3McpServer.cs
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// Single source of truth for the server identity and tool set, shared by the
/// stdio entry point (Program) and the in-memory protocol tests, so what the
/// tests verify is exactly what a real MCP client sees.
/// </summary>
public static class Wc3McpServer
{
    /// <summary>Result POCOs serialize like the CLI's --json, enums as names ("Unit"), not numbers.</summary>
    public static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(McpJsonUtilities.DefaultOptions);
        options.Converters.Add(new JsonStringEnumConverter());
        // A placed unit's or doodad's scale is a tuple, which printed as {} (see the converter).
        options.Converters.Add(new Wc3.Commands.ScaleJsonConverter());
        return options;
    }

    /// <summary>Server options with every wc3ctl tool registered. PRF-01. The 276 descriptors are
    /// not built here. Both the tool collection and the catalog resource that embeds every schema
    /// are deferred to the first read, so the initialize reply does not carry a schema build no
    /// client has asked for yet. The build itself still happens once per process and every session
    /// shares the one result.
    /// <para>
    /// SUR-03. <paramref name="renames"/> is the alias window. The shipped table is the default,
    /// and a test supplies its own so the filter can be exercised over a rename that this tree has
    /// not made yet. A table that cannot be trusted fails here rather than at the first call, which
    /// is the startup validation rule the config carries.
    /// </para></summary>
    public static McpServerOptions CreateOptions(IReadOnlyDictionary<string, ToolAlias>? renames = null)
    {
        long started = System.Diagnostics.Stopwatch.GetTimestamp();

        IReadOnlyDictionary<string, ToolAlias> aliases = renames ?? McpToolCatalog.Renames;
        if (aliases.Count > 0 && McpToolCatalog.InspectRenames(aliases) is { Count: > 0 } aliasProblems)
        {
            throw new InvalidOperationException(
                "The tool alias window is not usable, so the server did not start. "
                + string.Join(" ", aliasProblems));
        }

        var tools = new DeferredToolCollection(ToolList, ToolMethodList.Value.Count);

        var resources = new McpServerResourceCollection();
        resources.Add(new DeferredCatalogResource());
        foreach (var resource in Wc3Context.CreateResources())
            resources.Add(resource);
        foreach (var resource in Wc3Skills.CreateResources())
            resources.Add(resource);

        var prompts = new McpServerPrimitiveCollection<McpServerPrompt>();
        foreach (var prompt in CreatePrompts())
            prompts.Add(prompt);

        var options = new McpServerOptions
        {
            ServerInfo = new Implementation { Name = ServerIdentity.ProductId, Version = ServerIdentity.Version },
            ToolCollection = tools,
            PromptCollection = prompts,
            ResourceCollection = resources,
            ServerInstructions = Instructions.Value,
        };

        // SUR-03. The alias window sits outermost, so a call under an old name is renamed before
        // the policy reads the name and before the tool body runs. A refusal by either the window
        // or the policy still answers through this filter, which is why the reply a client gets
        // names the name it used.
        //
        // SUR-02. The collection is handed over as well, because the SDK resolves the request
        // against it before this filter runs and the dispatch that follows reads that one answer.
        // The window re-resolves the name it rewrote, which is what lets an old name reach a
        // descriptor that was never listed.
        options.Filters.Request.CallToolFilters.Add(next => ServerRuntime.AliasToolCalls(next, aliases, tools));

        // SAF-17. The policy is read here, when the surface is built, and the filter refuses before
        // the tool body runs, so a denied call never opens a map or touches a file.
        options.Filters.Request.CallToolFilters.Add(PermissionPolicy.Filter(PermissionPolicy.Load()));

        // ERR-03. The outermost filter turns a thrown refusal into the error result that carries
        // the code, the message, the tool and the offending value as separate fields beside the
        // text, so a caller branches on the code rather than parsing prose.
        options.Filters.Request.CallToolFilters.Insert(0, Wc3Errors.StructuredErrors);

        // OBS-02, OBS-04, PRF-01, PRF-04 and TRN-03 ride the SDK 1.4.1 filter pipeline, which is
        // built from these lists when the server starts. The request filter wraps the tool
        // invocation, the incoming message filter catches the initialize request and the outgoing
        // one sees the response after it reaches the transport, so a client that closed stdin mid
        // call still gets the bytes before the read side reports end of stream.
        options.Filters.Request.CallToolFilters.Add(ServerRuntime.ObserveToolCalls);

        // ERR-06. The schema check sits closest to the tool, after the rewrite and after the clock
        // starts, so a call that leaves out a required argument is refused with the name it left
        // out and the refusal is reported like any other failed call. The SDK binds the arguments
        // one layer further in, and it answers a binding failure with one sentence that names only
        // the tool, so the name has to be read from the schema while it is still in hand.
        options.Filters.Request.CallToolFilters.Add(next => Wc3Errors.RequiredArguments(next, tools.Find));
        options.Filters.Message.IncomingFilters.Add(ServerRuntime.ObserveIncomingMessages);
        options.Filters.Message.OutgoingFilters.Add(ServerRuntime.ObserveOutgoingResponses);

        ServerRuntime.ColdStart.SurfaceBuilt(started);
        return options;
    }

    /// <summary>The only profile there is, until SUR-07 adds a second one.</summary>
    public const string DefaultProfile = "full";

    /// <summary>
    /// The newest MCP revision the pinned SDK speaks, and what an unknown offer falls back to.
    /// Measured on 2026-11-08 over a raw pipe, every revision the SDK knows is echoed back
    /// (2024-11-05, 2025-03-26, 2025-06-18 and 2025-11-25), and an unknown or malformed offer gets
    /// this value with no error. The SDK keeps its constants internal, so ProtocolRevisionTests
    /// pins the matrix and fails if a pin changes an answer.
    /// </summary>
    public const string ProtocolRevision = "2025-11-25";

    /// <summary>What the target and the game install resolved to, read once per process.</summary>
    /// <param name="Target">The build the compatibility tools check against.</param>
    /// <param name="Source">Where that build came from, such as settings or the environment.</param>
    /// <param name="GameBuild">The installed game build, or null when there is no install.</param>
    public sealed record StartupFacts(string Target, string Source, string? GameBuild);

    /// <summary>
    /// Resolved once per process. Reading it walks the schemas folder, so recomputing it for every
    /// session made each in-memory MCP test pay for a full scan and the suite took minutes instead
    /// of seconds. The instructions and the start line both read this one value.
    /// </summary>
    public static readonly Lazy<StartupFacts> Startup =
        new(ReadStartup, LazyThreadSafetyMode.ExecutionAndPublication);

    private static StartupFacts ReadStartup()
    {
        // PRF-01. The two probes are independent, so they start together and the line waits for the
        // longer one rather than for their sum. The settings probe walks the schemas folder, which
        // is the largest single item in the start line. On the published exe the startup phase is
        // 258 ms with the schemas folder present and 133 ms with WC3_SCHEMAS_DIR pointed at an empty
        // folder, so that walk is about 125 ms of it. The install probe reads two registry keys and
        // one .build.info. Ten runs of the same exe put startup at a 237 ms median against a 129 ms
        // host phase, which is why the line is the phase this change attacks.
        Task<Wc3.Commands.TargetBuildResult?> settings = Task.Run(ReadTargetBuild);
        Task<string?> install = Task.Run(ReadInstallBuild);
        try
        {
            Wc3.Commands.TargetBuildResult? target = settings.GetAwaiter().GetResult();
            if (target is null) return new StartupFacts("unresolved", "unavailable", null);
            string source = target.Source switch
            {
                Wc3.Commands.TargetSource.Option => "option",
                Wc3.Commands.TargetSource.Environment => "environment variable",
                Wc3.Commands.TargetSource.Settings => "settings",
                Wc3.Commands.TargetSource.Default => "default",
                _ => "unknown",
            };
            return new StartupFacts(target.Build, source, install.GetAwaiter().GetResult());
        }
        catch
        {
            return new StartupFacts("unresolved", "unavailable", null);
        }
    }

    /// <summary>The target build and where it came from, or null when the settings cannot be read.</summary>
    private static Wc3.Commands.TargetBuildResult? ReadTargetBuild()
    {
        try { return Wc3.Commands.TargetBuildSettings.Get(); }
        catch { return null; }
    }

    /// <summary>The installed game build, or null when there is no install or no build file.</summary>
    private static string? ReadInstallBuild()
    {
        try
        {
            string? installDir = Wc3.GameData.GameInstall.Locate();
            return installDir is null ? null : Wc3.GameData.GameInstall.ReadBuildVersion(installDir);
        }
        catch
        {
            return null;
        }
    }

    private static readonly Lazy<string> Instructions =
        new(BuildInstructions, LazyThreadSafetyMode.ExecutionAndPublication);

    private static string BuildInstructions()
    {
        StartupFacts facts = Startup.Value;
        string target = facts.Source == "unavailable"
            ? "target unresolved (no schemas directory configured), game data not found"
            : "target " + facts.Target + " (" + facts.Source + "), game data "
                + (facts.GameBuild is null ? "not found" : "available (" + facts.GameBuild + ")");
        // DSC-01. The three rules and the routing hint are the point. A caller that reads this far
        // and nothing else still cannot overwrite its input.
        return ServerIdentity.ProductId + " " + ServerIdentity.Version + ", " + target + ". "
            + "Three rules. Read a map before you change it. "
            + "A write never overwrites its input, it writes a sibling .edited copy or the out_path you name. "
            + "Chain edits by passing the previous output as the next call's map. "
            + "Call quickstart when the request names an outcome rather than a tool, "
            + "and read wc3://tools for the whole surface.";
    }

    /// <summary>
    /// The one start line ENV-04 requires, on stderr, so the first question in a client bug report
    /// has an answer. The tool count is read from the same reflection the surface uses.
    /// </summary>
    public static string BuildStartLine()
    {
        // PRF-01. The facts probe and the attribute scan are independent, so they run together and
        // the line costs the longer one rather than both. They were two statements in a row, which
        // put their full cost on the clock the initialize reply waits for.
        Task<StartupFacts> probe = Task.Run(() => Startup.Value);
        int tools = ToolCount();
        StartupFacts facts = probe.GetAwaiter().GetResult();
        string line = ServerIdentity.ProductId + " " + ServerIdentity.Version
            + "  protocol " + ProtocolRevision
            + "  tools " + tools
            + "  target " + facts.Target + " (" + facts.Source + ")"
            + "  game " + (facts.GameBuild ?? "none")
            + "  profile " + DefaultProfile;

        // PRF-05. Only when this process is already over a gigabyte, and only then naming the work
        // that is running. At startup nothing is in flight, so the plain line is the normal case.
        if (ServerRuntime.IsOverMemoryLimit())
        {
            line += "  memory " + ServerRuntime.MemoryMegabytes() + " MB"
                + " in tool " + (ServerRuntime.CurrentTool ?? "none");
        }
        return line;
    }

    /// <summary>The methods that carry a tool, before any of them is turned into a handler. Scanned
    /// once per process, because the start line needs the count before the host builds and the
    /// surface then scans the same list. Two scans of 275 attributes cost part of the cold start
    /// budget for no gain. The test is <c>IsDefined</c> rather than <c>GetCustomAttribute</c>,
    /// because the scan needs to know that the attribute is there and never reads its value, so
    /// constructing 275 attribute instances on the start line path was work nobody used. The
    /// surface asks for the attribute itself when it binds each method.</summary>
    private static readonly Lazy<IReadOnlyList<MethodInfo>> ToolMethodList = new(
        () => new[] { typeof(Wc3Tools), typeof(Wc3Context) }
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.IsDefined(typeof(McpServerToolAttribute), inherit: false))
            .ToList(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>The methods that carry a tool.</summary>
    private static IEnumerable<MethodInfo> ToolMethods() => ToolMethodList.Value;

    /// <summary>
    /// Every tool, bound once per process. Binding is a schema build per method, so a second
    /// session in the same process was paying for the whole surface again. The bound tools hold no
    /// state, so one list serves every session.
    /// </summary>
    private static readonly Lazy<IReadOnlyList<McpServerTool>> ToolList = new(
        BuildTools, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// PRF-01. One schema build per method, on the first use rather than before the initialize
    /// reply. The time it took is written once as its own line, so the cost this defers stays
    /// visible instead of disappearing into the surface phase.
    /// </summary>
    private static IReadOnlyList<McpServerTool> BuildTools()
    {
        long started = System.Diagnostics.Stopwatch.GetTimestamp();
        List<McpServerTool> tools = ToolMethodList.Value
            .Select(m => McpServerTool.Create(m, target: null,
                new McpServerToolCreateOptions { SerializerOptions = JsonOptions }))
            .ToList();
        ServerRuntime.ColdStart.DescriptorsBuilt(
            tools.Count, System.Diagnostics.Stopwatch.GetElapsedTime(started));
        return tools;
    }

    /// <summary>Every [McpServerTool] method on <see cref="Wc3Tools"/> and <see cref="Wc3Context"/>, bound with our JSON options.</summary>
    public static IReadOnlyList<McpServerTool> CreateTools() => ToolList.Value;

    /// <summary>
    /// How many tools the surface holds, counted without building a handler for each one. The start
    /// line needs the number before the host builds the surface, and the count comes from the one
    /// method scan rather than a second one.
    /// </summary>
    public static int ToolCount() => ToolMethodList.Value.Count;

    /// <summary>Every [McpServerPrompt] method on <see cref="Wc3Prompts"/>, bound with our JSON options.</summary>
    public static IReadOnlyList<McpServerPrompt> CreatePrompts() =>
        typeof(Wc3Prompts)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<McpServerPromptAttribute>() is not null)
            .Select(m => McpServerPrompt.Create(m, target: null,
                new McpServerPromptCreateOptions { SerializerOptions = JsonOptions }))
            .ToList();

    /// <summary>
    /// Populates a host's options from <see cref="CreateOptions"/>. Both the stdio entry point and
    /// the in-memory protocol tests call this one method, so what the tests verify is exactly what a
    /// client receives, ServerInstructions included. Copying the fields by hand instead is what left
    /// the instructions off the initialize reply.
    /// </summary>
    public static void ApplyTo(McpServerOptions options) => ApplyTo(options, CreateOptions());

    /// <summary>
    /// The same copy from a surface that was already built. PRF-01 uses it so the tool collection is
    /// built on the thread pool while the host builds, rather than one phase after the other.
    /// </summary>
    public static void ApplyTo(McpServerOptions options, McpServerOptions configured)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(configured);
        options.ServerInfo = configured.ServerInfo;
        options.Capabilities = configured.Capabilities;
        options.ServerInstructions = configured.ServerInstructions;
        options.ToolCollection = configured.ToolCollection;
        options.PromptCollection = configured.PromptCollection;
        options.ResourceCollection = configured.ResourceCollection;
        // The filters carry OBS-02, OBS-04, PRF-01, PRF-04 and TRN-03. Copying them by hand like the
        // rest is what keeps a host that calls ApplyTo from running without them.
        options.Filters = configured.Filters;
    }
}

/// <summary>
/// PRF-01. The tool collection the server serves, built on first use. The reads the SDK makes, the
/// enumeration behind tools/list and the name lookup behind tools/call, materialize the descriptors
/// once and answer from the result. A name lookup builds the same way, so a call that arrives
/// before anything has listed the surface still finds its tool and the build is not repeated. The
/// base dictionary is left empty on purpose. Filling it would
/// raise the collection's Changed event once per tool, and the server answers that event with a
/// list_changed notification, so a first tools/list would put 276 notices on the pipe. Nothing the
/// SDK reads for tools needs that dictionary, and <see cref="Count"/> is the one member it would
/// have served, which is why <see cref="Expected"/> carries the number instead.
/// </summary>
internal sealed class DeferredToolCollection : McpServerPrimitiveCollection<McpServerTool>
{
    private readonly Lazy<IReadOnlyList<McpServerTool>> _source;
    private IReadOnlyDictionary<string, McpServerTool>? _byName;

    internal DeferredToolCollection(Lazy<IReadOnlyList<McpServerTool>> source, int expected)
        : base(StringComparer.Ordinal)
    {
        _source = source;
        Expected = expected;
    }

    /// <summary>
    /// How many descriptors the surface holds, from the one method scan rather than from the
    /// dictionary, so the number is right before the first use.
    /// </summary>
    internal int Expected { get; }

    /// <summary>The one descriptor build, made when the first client reads the surface.</summary>
    internal IReadOnlyList<McpServerTool> Materialized => _source.Value;

    private IReadOnlyDictionary<string, McpServerTool> ByName
    {
        get
        {
            IReadOnlyDictionary<string, McpServerTool>? byName = Volatile.Read(ref _byName);
            if (byName is not null) return byName;
            var map = new Dictionary<string, McpServerTool>(StringComparer.Ordinal);
            foreach (McpServerTool tool in Materialized) map[tool.ProtocolTool.Name] = tool;
            Volatile.Write(ref _byName, map);
            return map;
        }
    }

    public override bool TryGetPrimitive(string name, out McpServerTool? primitive) =>
        ByName.TryGetValue(name, out primitive);

    /// <summary>
    /// SUR-02. The same lookup, as a nullable answer, for the call path that needs a tool by name
    /// rather than a hit. The SDK asks for the name the caller used before any filter runs, so a
    /// rewritten name is looked up here a second time. A name that has never been asked for
    /// materializes the surface on that call, which is what makes the answer independent of a
    /// tools/list having happened first, and the one build per process is unchanged.
    /// </summary>
    internal McpServerTool? Find(string name) =>
        ByName.TryGetValue(name, out McpServerTool? primitive) ? primitive : null;

    public override IEnumerator<McpServerTool> GetEnumerator() => Materialized.GetEnumerator();

    public override McpServerTool[] ToArray() => Materialized.ToArray();

    public override void CopyTo(McpServerTool[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        for (int i = 0; i < Materialized.Count; i++) array[arrayIndex + i] = Materialized[i];
    }

    public override bool Contains(McpServerTool primitive) => Materialized.Contains(primitive);

    public override ICollection<string> PrimitiveNames => ByName.Keys.ToArray();
}

/// <summary>
/// PRF-01. The catalog resource carries every tool description and input schema, so building it
/// forces the whole descriptor set. It is built on the first read of the resource or the first
/// resources/list, and the build is memoized for the process. The protocol template is answered
/// from the constants the catalog is published under, because the base collection reads it to key
/// the resource, so adding this resource costs nothing.
/// </summary>
internal sealed class DeferredCatalogResource : McpServerResource
{
    internal const string Uri = "wc3://tools";
    internal const string Name = "tool_catalog";
    internal const string MimeType = "application/json";
    internal const string Description =
        "The MCP surface as JSON. Every tool with its description, its family, the four annotations,"
        + " its aliases and its input schema, and every prompt with its title, description and"
        + " argument names.";

    private static readonly Lazy<McpServerResource> Inner =
        new(McpToolCatalog.CreateCatalogResource, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>The catalog a read serves, built once per process on first use.</summary>
    internal static McpServerResource Materialized => Inner.Value;

    public override ResourceTemplate ProtocolResourceTemplate => new()
    {
        UriTemplate = Uri,
        Name = Name,
        Description = Description,
        MimeType = MimeType,
    };

    public override Resource? ProtocolResource => Inner.Value.ProtocolResource;

    public override IReadOnlyList<object> Metadata => Array.Empty<object>();

    public override bool IsMatch(string uri) =>
        string.Equals(uri, Uri, StringComparison.OrdinalIgnoreCase);

    public override ValueTask<ReadResourceResult> ReadAsync(
        RequestContext<ReadResourceRequestParams> request,
        CancellationToken cancellationToken = default) =>
        Inner.Value.ReadAsync(request, cancellationToken);
}
