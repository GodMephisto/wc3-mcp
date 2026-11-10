// src/Wc3.Mcp/Permissions.cs
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// The permission file, which belongs to the user and never to the agent (adopted from the
/// Roblox Studio MCP model). It sits beside the executable as permissions.json, or wherever
/// <c>WC3_MCP_PERMISSIONS</c> points, and holds one boolean switch per action the user is
/// willing to grant. Every switch defaults to false, a missing file grants nothing, and a file
/// that cannot be understood is a refusal rather than a silent allow.
///
/// Nothing in this server writes the file, and there is no tool that grants a switch. The user
/// edits it. A confirmation typed into a conversation is intent, not permission.
///
/// The file is read on every check rather than cached, so a user who edits it does not have to
/// restart the server, and a test can point <c>WC3_MCP_PERMISSIONS</c> at a temporary file
/// without touching the machine's own copy. The cost is one small file read per gated action.
/// </summary>
internal sealed class Permissions
{
    /// <summary>Names another permission file, which is how a test or a second install pins its own.</summary>
    internal const string EnvironmentVariable = "WC3_MCP_PERMISSIONS";

    /// <summary>The file's name beside the executable.</summary>
    internal const string FileName = "permissions.json";

    /// <summary>Every switch the file may carry, in the order the diagnostics print them.</summary>
    internal static readonly IReadOnlyList<string> Actions =
        new[] { "overwrite", "publish", "mirror_export", "npm", "delete" };

    private readonly Dictionary<string, bool> _switches;

    private Permissions(
        string pathName, bool present, string? fault,
        Dictionary<string, bool> switches, IReadOnlyList<string> ignoredKeys)
    {
        PathName = pathName;
        Present = present;
        Fault = fault;
        _switches = switches;
        IgnoredKeys = ignoredKeys;
    }

    /// <summary>The file this instance read, resolved from the environment when it names one.</summary>
    internal string PathName { get; }

    /// <summary>Whether a file was found. False means every action is denied.</summary>
    internal bool Present { get; }

    /// <summary>Why the file cannot be trusted, or null when it parsed. Non-null denies everything.</summary>
    internal string? Fault { get; }

    /// <summary>Keys the file carries that no action uses. Reported, never fatal.</summary>
    internal IReadOnlyList<string> IgnoredKeys { get; }

    /// <summary>The file as it is right now, which is what the gate reads.</summary>
    internal static Permissions Current => Load();

    /// <summary>Read a permission file, or the one the environment or the install names.</summary>
    internal static Permissions Load(string? path = null)
    {
        string full = Resolve(path);
        if (!File.Exists(full))
            return new Permissions(full, present: false, fault: null,
                new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase), Array.Empty<string>());

        string text;
        try
        {
            text = File.ReadAllText(full);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException
            or ArgumentException)
        {
            return Invalid(full, "the file could not be read. " + ex.Message);
        }

        JsonNode? root;
        try
        {
            root = JsonNode.Parse(text);
        }
        catch (Exception ex) when (ex is JsonException or ArgumentException or FormatException or InvalidOperationException)
        {
            return Invalid(full, "the file is not valid JSON, so no switch is granted. " + ex.Message);
        }

        if (root is not JsonObject obj)
            return Invalid(full, "the file must hold a JSON object of switches, one per action.");

        var switches = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        var ignored = new List<string>();
        foreach (var pair in obj)
        {
            string name = pair.Key;
            if (!Actions.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                ignored.Add(name);
                continue;
            }
            if (pair.Value is null || pair.Value.GetValueKind() is not (JsonValueKind.True or JsonValueKind.False))
                return Invalid(full, "the \"" + name + "\" switch is not true or false.");
            switches[name] = pair.Value.GetValue<bool>();
        }

        return new Permissions(full, present: true, fault: null, switches, ignored);
    }

    /// <summary>
    /// Whether one action is granted. An action the file does not name is denied, and so is every
    /// action when no file exists. A file that cannot be trusted throws rather than answering,
    /// because false would read as a plain denial and lose the reason the user needs.
    /// </summary>
    internal bool IsAllowed(string action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        if (Fault is not null) throw Faulted();
        return _switches.TryGetValue(action.Trim(), out bool granted) && granted;
    }

    /// <summary>
    /// Refuse unless one action is granted, naming the switch, the permission file and the file the
    /// caller wanted to touch. This is the shape every gated path reports.
    /// </summary>
    internal void Require(string action, string file, string effect)
    {
        if (IsAllowed(action)) return;
        throw new ToolError(
            "PERMISSION_DENIED",
            "\"" + action + "\" is not granted in " + PathName + ", so " + file + " was not " + effect + ".",
            "ask the user to set \"" + action + "\": true in " + PathName + ", then repeat the call. "
                + "The server cannot grant it and there is no tool that does.");
    }

    /// <summary>One diagnostic block for a client or a bug report. Plain text, no side effects.</summary>
    internal string Describe()
    {
        var text = new StringBuilder();
        text.Append("permissions file ").Append(PathName)
            .Append(Fault is not null ? ", invalid" : Present ? ", loaded" : ", absent")
            .AppendLine(".");
        if (Fault is not null) text.AppendLine(Fault);
        foreach (string action in Actions)
        {
            string verdict = _switches.TryGetValue(action, out bool granted)
                ? granted ? "granted" : "denied, the switch is false"
                : "denied, the switch is absent";
            text.Append(action).Append(' ').Append(verdict).AppendLine(".");
        }
        if (IgnoredKeys.Count > 0)
            text.Append("ignored unknown switches ").Append(string.Join(", ", IgnoredKeys)).AppendLine(".");
        if (!Present && Fault is null)
            text.AppendLine("Nothing is granted until the user creates this file. Ask, never assume.");
        return text.ToString().TrimEnd();
    }

    /// <summary>The named path, else the environment's, else the file beside the executable.</summary>
    private static string Resolve(string? path)
    {
        string? named = string.IsNullOrWhiteSpace(path)
            ? Environment.GetEnvironmentVariable(EnvironmentVariable)
            : path;
        return string.IsNullOrWhiteSpace(named)
            ? Path.Combine(AppContext.BaseDirectory, FileName)
            : named.Trim();
    }

    private static Permissions Invalid(string path, string detail) =>
        new(path, present: true, fault: path + " is not usable. " + detail,
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase), Array.Empty<string>());

    private ToolError Faulted() =>
        new("PERMISSION_DENIED", Fault!,
            "fix the file or remove it so the defaults apply, then repeat the call. "
                + "Nothing is granted from a file that cannot be read.");
}

/// <summary>
/// SAF-17 and SAF-18. The policy file the user owns. It is a JSON object with three keys.
/// <c>deny</c> lists the tools and families refused outright, <c>require_approval</c> lists the
/// ones that need a person's approval, and <c>note</c> is an optional line for the person reading
/// the file. A missing file leaves every tool as it is, which is the behaviour the server has
/// today. The file is read once when the server surface is built, so an edit takes effect on the
/// next start, and nothing in the server writes it.
///
/// The search order is <c>WC3_MCP_PERMISSIONS</c> when it names a path, else
/// <c>%LOCALAPPDATA%\wc3ctl\permissions.json</c>. A file that sits inside the executable folder or
/// beside a map archive is ignored with one line on stderr naming the reason, because the
/// specification puts the policy outside every folder the server writes. A map folder is content
/// the agent can write into, so a file there is not the user's word.
///
/// <c>confirm</c> is not permission. It states that the caller meant the call. A rule in
/// <c>deny</c> refuses the call even when the argument is true, and a rule in
/// <c>require_approval</c> is satisfied by the argument only because the file asks for it, which
/// is SAF-18.
///
/// This is the tool gate. The <see cref="Permissions"/> class above is the separate write gate
/// that decides whether an existing output may be replaced. Both are the user's, and both are
/// read from the path <c>WC3_MCP_PERMISSIONS</c> names.
/// </summary>
internal sealed class PermissionPolicy
{
    /// <summary>Names the policy file, which is how a test or a second install pins its own.</summary>
    internal const string EnvironmentVariable = "WC3_MCP_PERMISSIONS";

    /// <summary>The file name inside the local app data wc3ctl folder.</summary>
    internal const string FileName = "permissions.json";

    /// <summary>The list of tools and families refused outright.</summary>
    internal const string DenyKey = "deny";

    /// <summary>The list of tools and families that need a person's approval.</summary>
    internal const string ApprovalKey = "require_approval";

    /// <summary>An optional line for the person who edits the file.</summary>
    internal const string NoteKey = "note";

    /// <summary>The code every refusal carries, so a client can branch on it.</summary>
    internal const string DeniedCode = "POLICY_DENIED";

    /// <summary>The tool argument that states intent. It is never a grant.</summary>
    internal const string ConfirmArgument = "confirm";

    private static readonly string[] KnownKeys = { DenyKey, ApprovalKey, NoteKey };

    private readonly IReadOnlyList<string> _deny;
    private readonly IReadOnlyList<string> _approval;

    private PermissionPolicy(
        string pathName, bool present, string? fault, string? ignoredReason,
        IReadOnlyList<string> deny, IReadOnlyList<string> approval, string? note,
        IReadOnlyList<string> ignoredKeys, IReadOnlyList<string> unknownRules)
    {
        PathName = pathName;
        Present = present;
        Fault = fault;
        IgnoredReason = ignoredReason;
        _deny = deny;
        _approval = approval;
        Note = note;
        IgnoredKeys = ignoredKeys;
        UnknownRules = unknownRules;
    }

    /// <summary>The file this instance resolved, from the environment or the local app data folder.</summary>
    internal string PathName { get; }

    /// <summary>Whether a policy file was read and applies. False means no tool is restricted.</summary>
    internal bool Present { get; }

    /// <summary>Why the file cannot be trusted, or null when it parsed. Non-null refuses every call.</summary>
    internal string? Fault { get; }

    /// <summary>Why a file that exists was left alone, or null when none was found there.</summary>
    internal string? IgnoredReason { get; }

    /// <summary>The rules in <c>deny</c>, as the user wrote them.</summary>
    internal IReadOnlyList<string> Deny => _deny;

    /// <summary>The rules in <c>require_approval</c>, as the user wrote them.</summary>
    internal IReadOnlyList<string> RequireApproval => _approval;

    /// <summary>The optional line the user wrote for a reader of the file.</summary>
    internal string? Note { get; }

    /// <summary>Keys the file carries that no rule uses. Reported, never fatal.</summary>
    internal IReadOnlyList<string> IgnoredKeys { get; }

    /// <summary>Rules that name neither a tool nor a family. Reported, never fatal.</summary>
    internal IReadOnlyList<string> UnknownRules { get; }

    /// <summary>Where the file lives when nothing in the environment names another path.</summary>
    internal static string DefaultPath()
    {
        string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(root))
            root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        return Path.Combine(root, "wc3ctl", FileName);
    }

    /// <summary>
    /// Read the policy file, at the path named here, else the environment's, else the local app
    /// data folder. A file found inside the executable folder or beside a map archive is ignored
    /// and reported through <paramref name="notice"/>, which defaults to one line on stderr.
    /// </summary>
    internal static PermissionPolicy Load(string? path = null, Action<string>? notice = null)
    {
        notice ??= static line => Console.Error.WriteLine(line);
        string full = Normalize(Resolve(path));

        if (!File.Exists(full)) return new PermissionPolicy(full, present: false, fault: null,
            ignoredReason: null, Array.Empty<string>(), Array.Empty<string>(), note: null,
            Array.Empty<string>(), Array.Empty<string>());

        // SAF-17. The file belongs to the user, so it cannot live in a folder the server writes.
        if (IsInside(full, AppContext.BaseDirectory))
        {
            return Ignore(full, "It is inside the executable folder " + Normalize(AppContext.BaseDirectory),
                notice);
        }
        string? mapFolder = MapFolderHolding(full);
        if (mapFolder is not null)
            return Ignore(full, "It sits in a map folder " + mapFolder, notice);

        string text;
        try
        {
            text = File.ReadAllText(full);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException
            or ArgumentException)
        {
            return Invalid(full, "the file could not be read. " + ex.Message);
        }

        JsonNode? root;
        try
        {
            root = JsonNode.Parse(text);
        }
        catch (Exception ex) when (ex is JsonException or ArgumentException or FormatException or InvalidOperationException)
        {
            return Invalid(full, "the file is not valid JSON, so no tool can be cleared to run. " + ex.Message);
        }

        if (root is not JsonObject obj)
            return Invalid(full, "the file must be a JSON object holding \"" + DenyKey + "\", \""
                + ApprovalKey + "\" and an optional \"" + NoteKey + "\".");

        if (!TryReadList(obj, DenyKey, out List<string> deny, out string? denyFault))
            return Invalid(full, denyFault!);
        if (!TryReadList(obj, ApprovalKey, out List<string> approval, out string? approvalFault))
            return Invalid(full, approvalFault!);

        string? note = null;
        if (obj.TryGetPropertyValue(NoteKey, out JsonNode? noteNode))
        {
            if (noteNode is null || noteNode.GetValueKind() != JsonValueKind.String)
                return Invalid(full, "the \"" + NoteKey + "\" key must be a string.");
            note = noteNode.GetValue<string>();
        }

        var ignoredKeys = new List<string>();
        foreach (var pair in obj)
        {
            if (!KnownKeys.Contains(pair.Key, StringComparer.OrdinalIgnoreCase))
                ignoredKeys.Add(pair.Key);
        }

        var unknown = new List<string>();
        foreach (string rule in deny.Concat(approval))
        {
            if (!NamesSomething(rule) && !unknown.Contains(rule, StringComparer.OrdinalIgnoreCase))
                unknown.Add(rule);
        }

        return new PermissionPolicy(full, present: true, fault: null, ignoredReason: null,
            deny, approval, note, ignoredKeys, unknown);
    }

    /// <summary>The rule in <c>deny</c> that covers this tool, or null when the file refuses nothing.</summary>
    internal string? DeniedRule(string tool) => RuleFor(_deny, tool);

    /// <summary>The rule in <c>require_approval</c> that covers this tool, or null when none does.</summary>
    internal string? ApprovalRule(string tool) => RuleFor(_approval, tool);

    /// <summary>
    /// The refusal for one call, or null when the call may run. A file that cannot be read refuses
    /// everything, because a rule nobody can see is not a rule the user agreed to. A rule in
    /// <c>deny</c> is checked first, so a denial beats a call carrying confirm true.
    /// </summary>
    internal string? RefusalFor(string tool, bool confirm)
    {
        if (string.IsNullOrWhiteSpace(tool)) return null;
        if (Fault is not null)
            return "nothing ran, because the permission file " + PathName + " cannot be read as a policy. "
                + Fault + " Fix the file or remove it, then repeat the call.";
        if (DeniedRule(tool) is { } denied) return DenyMessage(tool, denied, confirm);
        if (ApprovalRule(tool) is { } approval && !confirm) return ApprovalMessage(tool, approval);
        return null;
    }

    /// <summary>
    /// SAF-17. The request filter that applies the policy before the tool body runs, so a refused
    /// call never opens a map or touches a file. It answers with the same shape a tool error takes,
    /// with <see cref="DeniedCode"/> first and the rule that matched beside the tool name.
    /// </summary>
    internal static McpRequestFilter<CallToolRequestParams, CallToolResult> Filter(PermissionPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);
        return next =>
        {
            ArgumentNullException.ThrowIfNull(next);
            return (request, cancellationToken) =>
            {
                string tool = request.Params?.Name ?? "";
                if (policy.RefusalFor(tool, CarriesConfirm(request)) is { } refusal)
                    return ValueTask.FromResult(Refusal(refusal));
                return next(request, cancellationToken);
            };
        };
    }

    /// <summary>One diagnostic block for a client or a bug report. Plain text, no side effects.</summary>
    internal string Describe()
    {
        var text = new StringBuilder();
        text.Append("policy file ").Append(PathName)
            .Append(Fault is not null ? ", invalid" : Present ? ", loaded" : ", absent")
            .AppendLine(".");
        if (Fault is not null) text.AppendLine(Fault);
        if (IgnoredReason is not null) text.Append("ignored, ").Append(IgnoredReason).AppendLine(".");
        text.Append(DenyKey).Append(' ').Append(Listing(_deny)).AppendLine(".");
        text.Append(ApprovalKey).Append(' ').Append(Listing(_approval)).AppendLine(".");
        if (Note is not null) text.Append("note ").Append(Note).AppendLine(".");
        if (IgnoredKeys.Count > 0)
            text.Append("ignored unknown keys ").Append(string.Join(", ", IgnoredKeys)).AppendLine(".");
        if (UnknownRules.Count > 0)
            text.Append("rules that name no tool and no family ").Append(string.Join(", ", UnknownRules)).AppendLine(".");
        if (!Present && Fault is null && IgnoredReason is null)
            text.AppendLine("No tool is restricted until the user creates this file.");
        return text.ToString().TrimEnd();
    }

    private string DenyMessage(string tool, string rule, bool confirm) =>
        "\"" + tool + "\" is refused by the permission file " + PathName + ", from its \"" + DenyKey
        + "\" list, where the rule \"" + rule + "\" matched. The tool did not run, so no file was touched."
        + (confirm
            ? " The call carried " + ConfirmArgument + " true, and a "
                + ConfirmArgument + " flag only states intent and never grants permission, so the file"
                + " decides and the flag cannot open a rule the file denies."
            : " Only the user can change the file. There is no tool that does.");

    private string ApprovalMessage(string tool, string rule) =>
        "\"" + tool + "\" needs a person's approval. The permission file " + PathName + " lists it under \""
        + ApprovalKey + "\", where the rule \"" + rule + "\" matched, and this call did not carry "
        + ConfirmArgument + " true. The file decides which tools need approval and the "
        + ConfirmArgument + " flag only states intent, so " + ConfirmArgument + " true repeats the call"
        + " only while the file keeps the tool out of \"" + DenyKey + "\".";

    private static string Listing(IReadOnlyList<string> rules) =>
        rules.Count == 0 ? "empty" : string.Join(", ", rules);

    private static string RuleFor(IReadOnlyList<string> rules, string tool)
    {
        if (string.IsNullOrWhiteSpace(tool)) return null;
        string? family = McpToolFamilies.Of(tool);
        foreach (string rule in rules)
        {
            if (rule.Equals(tool, StringComparison.OrdinalIgnoreCase)) return rule;
            if (family is not null && rule.Equals(family, StringComparison.OrdinalIgnoreCase)) return rule;
        }
        return null;
    }

    private static bool CarriesConfirm(RequestContext<CallToolRequestParams> request)
    {
        IDictionary<string, JsonElement>? arguments = request.Params?.Arguments;
        return arguments is not null
            && arguments.TryGetValue(ConfirmArgument, out JsonElement value)
            && value.ValueKind == JsonValueKind.True;
    }

    private static CallToolResult Refusal(string message) =>
        new()
        {
            IsError = true,
            Content = new List<ContentBlock> { new TextContentBlock { Text = "[" + DeniedCode + "] " + message } },
        };

    private static bool TryReadList(JsonObject obj, string key, out List<string> rules, out string? fault)
    {
        rules = new List<string>();
        fault = null;
        if (!obj.TryGetPropertyValue(key, out JsonNode? node)) return true;
        if (node is not JsonArray array)
        {
            fault = "the \"" + key + "\" key must be an array of tool names or family ids.";
            return false;
        }
        foreach (JsonNode? item in array)
        {
            if (item is null || item.GetValueKind() != JsonValueKind.String)
            {
                fault = "every entry in \"" + key + "\" must be a string naming a tool or a family.";
                return false;
            }
            string entry = (item.GetValue<string>() ?? "").Trim();
            if (entry.Length == 0)
            {
                fault = "an entry in \"" + key + "\" is empty. Name a tool or a family.";
                return false;
            }
            if (!rules.Contains(entry, StringComparer.OrdinalIgnoreCase)) rules.Add(entry);
        }
        return true;
    }

    private static bool NamesSomething(string rule)
    {
        foreach (var family in McpToolFamilies.Families)
        {
            if (family.Id.Equals(rule, StringComparison.OrdinalIgnoreCase)) return true;
        }
        foreach (var tool in McpToolFamilies.All)
        {
            if (tool.Key.Equals(rule, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    private static PermissionPolicy Ignore(string path, string why, Action<string> notice)
    {
        notice("wc3 permissions ignored " + path + ". " + why + ", and the policy file must sit outside"
            + " every folder the server writes, so no rule in it applies. Put it at " + DefaultPath()
            + " or point " + EnvironmentVariable + " at a file outside the install and the map folders.");

        return new PermissionPolicy(path, present: false, fault: null, ignoredReason: why,
            Array.Empty<string>(), Array.Empty<string>(), note: null,
            Array.Empty<string>(), Array.Empty<string>());
    }

    private static PermissionPolicy Invalid(string path, string detail) =>
        new(path, present: true, fault: path + " is not usable. " + detail,
            ignoredReason: null, Array.Empty<string>(), Array.Empty<string>(), note: null,
            Array.Empty<string>(), Array.Empty<string>());

    private static string Resolve(string? path)
    {
        string? named = string.IsNullOrWhiteSpace(path)
            ? Environment.GetEnvironmentVariable(EnvironmentVariable)
            : path;
        return string.IsNullOrWhiteSpace(named) ? DefaultPath() : named.Trim();
    }

    private static string Normalize(string path)
    {
        try
        {
            return Path.GetFullPath(path);
        }
        catch (Exception)
        {
            return path;
        }
    }

    private static bool IsInside(string file, string folder)
    {
        string root = Normalize(folder);
        if (!root.EndsWith(Path.DirectorySeparatorChar)) root += Path.DirectorySeparatorChar;
        return file.StartsWith(root, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>The folder holding the file, when a map archive sits beside it, else null.</summary>
    private static string? MapFolderHolding(string file)
    {
        try
        {
            string? folder = Path.GetDirectoryName(file);
            if (folder is null || !Directory.Exists(folder)) return null;
            foreach (string candidate in Directory.EnumerateFiles(folder))
            {
                string extension = Path.GetExtension(candidate);
                if (extension.Equals(".w3x", StringComparison.OrdinalIgnoreCase)
                    || extension.Equals(".w3m", StringComparison.OrdinalIgnoreCase)) return folder;
            }
        }
        catch (Exception)
        {
            // An unreadable folder is not a map folder as far as this check is concerned.
        }
        return null;
    }
}
