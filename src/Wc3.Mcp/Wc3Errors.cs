// src/Wc3.Mcp/Wc3Errors.cs
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>One stable error code and the single line that says what it means (ERR-02).</summary>
/// <param name="Code">The value a caller branches on, upper snake case, never reworded once shipped.</param>
/// <param name="Meaning">One sentence for the caller, the same sentence <c>error_codes</c> returns.</param>
public sealed record Wc3ErrorCode(string Code, string Meaning);

/// <summary>
/// Every code this server can return, in one place (ERR-02, ERR-03). A refusal names one of these
/// constants rather than a bare string, so a typo cannot invent a code, and the tool
/// <c>error_codes</c> serves this same table to a caller that wants to branch without reading prose.
///
/// The first seventeen are the initial set from section 6.4 of the specification. The rest are the
/// codes this surface already returned before the table existed, kept under their old spelling so a
/// client that branches on one keeps working, plus the ones the tool and command layers need to
/// report a file or a shared refusal the caller can act on.
/// </summary>
public static class Wc3ErrorCodes
{
    // The initial set, section 6.4 of the specification.

    /// <summary>The map path does not exist.</summary>
    public static readonly Wc3ErrorCode MapNotFound = new("MAP_NOT_FOUND", "The map path does not exist.");

    /// <summary>The archive could not be opened, or it is not a w3x or w3m.</summary>
    public static readonly Wc3ErrorCode MapUnreadable = new("MAP_UNREADABLE", "The archive could not be opened or is not a w3x or w3m.");

    /// <summary>The named rawcode, archive entry or script function is not in the map.</summary>
    public static readonly Wc3ErrorCode ObjectNotFound = new("OBJECT_NOT_FOUND", "The rawcode, archive entry or script function is in neither the map nor the base game.");

    /// <summary>The tool needs base game data and none was found.</summary>
    public static readonly Wc3ErrorCode GameDataMissing = new("GAME_DATA_MISSING", "The tool needs base game data and none was found.");

    /// <summary>The output path is the input map itself.</summary>
    public static readonly Wc3ErrorCode OutPathIsInput = new("OUT_PATH_IS_INPUT", "The output is the input map itself.");

    /// <summary>The output path sits inside the input folder map.</summary>
    public static readonly Wc3ErrorCode OutPathInsideInput = new("OUT_PATH_INSIDE_INPUT", "The output sits inside the input folder map.");

    /// <summary>The output exists and overwrite was not set.</summary>
    public static readonly Wc3ErrorCode OutputExists = new("OUTPUT_EXISTS", "The output exists and overwrite was not set.");

    /// <summary>The input is not the one the caller read, so the write was refused.</summary>
    public static readonly Wc3ErrorCode StaleInput = new("STALE_INPUT", "The input is not the one the caller read, so the write was refused.");

    /// <summary>A destructive call did not state its intent.</summary>
    public static readonly Wc3ErrorCode ConfirmRequired = new("CONFIRM_REQUIRED", "A destructive call did not state its intent, so it did not run.");

    /// <summary>The permission file refuses this tool or family.</summary>
    public static readonly Wc3ErrorCode PolicyDenied = new("POLICY_DENIED", "The permission file refuses this tool or family.");

    /// <summary>The action is not granted in the permission file.</summary>
    public static readonly Wc3ErrorCode PermissionDenied = new("PERMISSION_DENIED", "The action is not granted in the permission file.");

    /// <summary>A value is missing or outside its legal set, and the message names the set.</summary>
    public static readonly Wc3ErrorCode InvalidArgument = new("INVALID_ARGUMENT", "A value is missing or outside its legal set, and the message names the set.");

    /// <summary>The text would not survive a UTF-8 decode.</summary>
    public static readonly Wc3ErrorCode LossyDecode = new("LOSSY_DECODE", "The text would not survive a UTF-8 decode.");

    /// <summary>The result would exceed the page limit.</summary>
    public static readonly Wc3ErrorCode ResultTooLarge = new("RESULT_TOO_LARGE", "The result would exceed the page limit, so ask for a page or a narrower query.");

    /// <summary>A required library or file is absent, such as CascLib.dll.</summary>
    public static readonly Wc3ErrorCode DependencyMissing = new("DEPENDENCY_MISSING", "A required library or file is absent.");

    /// <summary>The build does not run on this platform.</summary>
    public static readonly Wc3ErrorCode UnsupportedPlatform = new("UNSUPPORTED_PLATFORM", "The build does not run on this platform.");

    /// <summary>An unexpected failure, reported with the tool name and the stderr line.</summary>
    public static readonly Wc3ErrorCode Internal = new("INTERNAL", "An unexpected failure, reported with the tool name and the stderr line.");

    // The codes this surface already returned, kept stable.

    /// <summary>The archive holds more entries than the entry count limit allows.</summary>
    public static readonly Wc3ErrorCode ArchiveEntries = new("ARCHIVE_ENTRIES", "The archive holds more entries than the entry count limit allows.");

    /// <summary>One entry decompresses past the per entry limit.</summary>
    public static readonly Wc3ErrorCode EntryBytes = new("ENTRY_BYTES", "One entry decompresses past the per entry limit.");

    /// <summary>One call decompresses past its total limit.</summary>
    public static readonly Wc3ErrorCode CallBytes = new("CALL_BYTES", "One call decompresses past its total limit.");

    /// <summary>The destination volume has less free space than a staged write needs.</summary>
    public static readonly Wc3ErrorCode TempDisk = new("TEMP_DISK", "The destination volume has less free space than a staged write needs.");

    /// <summary>A folder occupies the output path.</summary>
    public static readonly Wc3ErrorCode OutputIsFolder = new("OUTPUT_IS_FOLDER", "A folder occupies the output path.");

    /// <summary>A symbolic link occupies the output path.</summary>
    public static readonly Wc3ErrorCode OutputIsLink = new("OUTPUT_IS_LINK", "A symbolic link occupies the output path.");

    /// <summary>The call used a renamed tool whose removal version has passed.</summary>
    public static readonly Wc3ErrorCode AliasExpired = new("ALIAS_EXPIRED", "The tool name was renamed and the old name has passed its removal version.");

    /// <summary>The write named no output file, or produced none.</summary>
    public static readonly Wc3ErrorCode NoOutput = new("NO_OUTPUT", "The write named no output file, or produced none.");

    /// <summary>A file argument other than the map does not exist.</summary>
    public static readonly Wc3ErrorCode InputNotFound = new("INPUT_NOT_FOUND", "A file argument other than the map does not exist.");

    /// <summary>A file argument other than the map could not be read.</summary>
    public static readonly Wc3ErrorCode InputUnreadable = new("INPUT_UNREADABLE", "A file argument other than the map could not be read.");

    /// <summary>A file would land outside the output directory the call named.</summary>
    public static readonly Wc3ErrorCode OutsideOutputDirectory = new("OUTSIDE_OUTPUT_DIRECTORY", "A file would land outside the output directory the call named.");

    /// <summary>A shared command refused the request and named its own reason in the message.</summary>
    public static readonly Wc3ErrorCode CommandRejected = new("COMMAND_REJECTED", "A shared command refused the request and named its reason in the message.");

    /// <summary>Every code, in the order the table serves them.</summary>
    public static readonly IReadOnlyList<Wc3ErrorCode> All = new[]
    {
        MapNotFound, MapUnreadable, ObjectNotFound, GameDataMissing,
        OutPathIsInput, OutPathInsideInput, OutputExists, StaleInput,
        ConfirmRequired, PolicyDenied, PermissionDenied, AliasExpired,
        InvalidArgument, LossyDecode, ResultTooLarge, DependencyMissing,
        UnsupportedPlatform, Internal,
        ArchiveEntries, EntryBytes, CallBytes, TempDisk,
        OutputIsFolder, OutputIsLink, NoOutput,
        InputNotFound, InputUnreadable, OutsideOutputDirectory, CommandRejected,
    };

    /// <summary>How many codes the table holds.</summary>
    public static int Count => All.Count;

    /// <summary>The meaning of one code, or null when the value is not in the table.</summary>
    public static string? MeaningOf(string? code) =>
        code is null ? null : All.FirstOrDefault(c => c.Code == code)?.Meaning;

    /// <summary>True when the value is one of the codes this server returns.</summary>
    public static bool IsCode(string? code) => code is not null && All.Any(c => c.Code == code);

    /// <summary>
    /// The table entry for one code value. A value that is not in the table is reported as
    /// <see cref="Internal"/> rather than dropped, so a code from the shared writer that was never
    /// added here still reaches the caller with a code it can read.
    /// </summary>
    public static Wc3ErrorCode Find(string? code) =>
        (code is null ? null : All.FirstOrDefault(c => c.Code == code)) ?? Internal;
}

/// <summary>What <c>error_codes</c> returns, the whole table and its size in one call (ERR-02).</summary>
/// <param name="Count">How many codes the table holds.</param>
/// <param name="Codes">Every code with its one line meaning, in table order.</param>
public sealed record ErrorCodesResult(int Count, IReadOnlyList<Wc3ErrorCode> Codes);

/// <summary>
/// The one shape every refusal takes (ERR-02, ERR-03). The message keeps the wording the surface
/// already used, so a client that reads the text sees no change, while the four fields travel in
/// <see cref="Exception.Data"/> and in the structured content of the error result, so a caller can
/// branch on the code, the tool and the value it passed instead of parsing prose.
/// </summary>
internal static class Wc3Errors
{
    /// <summary>The key holding the code in <see cref="Exception.Data"/>.</summary>
    public const string CodeKey = "wc3.code";

    /// <summary>The key holding the message without the bracketed code.</summary>
    public const string MessageKey = "wc3.message";

    /// <summary>The key holding the tool name, filled in by the filter that knows it.</summary>
    public const string ToolKey = "wc3.tool";

    /// <summary>The key holding the offending value, when the refusal has one.</summary>
    public const string ValueKey = "wc3.value";

    /// <summary>The key holding the one retry that turns the dead end into progress, when one exists.</summary>
    public const string NextKey = "wc3.next";

    /// <summary>
    /// A refusal with a table code. The message reads as it always did, with the bracketed code
    /// first and the next step last, and the same four parts are also attached as data.
    /// </summary>
    public static McpException Refuse(Wc3ErrorCode code, string message, string? value = null, string? next = null)
    {
        string text = "[" + code.Code + "] " + message
            + (next is null ? "" : ". Next, " + next);
        var error = new McpException(text);
        error.Data[CodeKey] = code.Code;
        error.Data[MessageKey] = message;
        if (value is not null) error.Data[ValueKey] = value;
        if (next is not null) error.Data[NextKey] = next;
        return error;
    }

    /// <summary>
    /// The same refusal, for a code the shared writer already owns as a string. The table entry is
    /// looked up so the code stays in the one list even when the writer raised it.
    /// </summary>
    public static McpException Refuse(string code, string message, string? value = null, string? next = null) =>
        Refuse(Wc3ErrorCodes.Find(code), message, value, next);

    /// <summary>
    /// The structured content an error result carries, as the four separate fields ERR-03 asks for.
    /// A refusal built by <see cref="Refuse"/> fills all four. Any other exception fills the message
    /// from its own text and reports <see cref="Wc3ErrorCodes.CommandRejected"/>, because a throw
    /// that reached the filter came from a command and not from a bug in the surface.
    /// <para>
    /// ERR-06. A binding failure raises an ArgumentException whose ParamName holds the argument the
    /// caller left out, and the text of that exception does not always carry it once the layers
    /// between the binder and this filter have wrapped it. The name is read from the exception
    /// chain and kept in the message and in the value field, so the caller learns which argument
    /// to pass instead of reading a sentence that names only the tool.
    /// </para>
    /// </summary>
    public static JsonElement Payload(Exception error, string? tool)
    {
        string message = error.Data[MessageKey] as string ?? Text(error);
        string code = error.Data[CodeKey] as string ?? Wc3ErrorCodes.CommandRejected.Code;
        string? value = error.Data[ValueKey] as string ?? ArgumentName(error);
        string? next = error.Data[NextKey] as string;
        return JsonSerializer.SerializeToElement(new ErrorPayload(code, message, tool, value, next),
            Wc3McpServer.JsonOptions);
    }

    /// <summary>
    /// The error result itself. The text content is exactly what the SDK would have built from the
    /// thrown message, so every client that reads the text today keeps working, and the structured
    /// content adds the four fields beside it. A binding failure gains the argument name in the
    /// text as well, which is the half of ERR-06 a client that reads only the text can act on.
    /// </summary>
    public static CallToolResult Result(Exception error, string? tool)
    {
        if (tool is not null) error.Data[ToolKey] = tool;
        return new()
        {
            IsError = true,
            Content = new List<ContentBlock> { new TextContentBlock { Text = Text(error) } },
            StructuredContent = Payload(error, tool),
        };
    }

    /// <summary>
    /// ERR-06. The text content of a failure. A refusal already reads as its own sentence, so it is
    /// returned untouched. Any other exception that names a parameter in its chain gets that name
    /// appended, because the sentence the binding layer produces names the tool and nothing else.
    /// </summary>
    private static string Text(Exception error)
    {
        if (error.Data[MessageKey] is string) return error.Message;
        return ArgumentName(error) is { } argument && !error.Message.Contains(argument, StringComparison.Ordinal)
            ? error.Message + " The argument \"" + argument + "\" is the one that could not be bound."
            : error.Message;
    }

    /// <summary>
    /// ERR-06. The parameter an ArgumentException refused, read from the exception or anything it
    /// wraps, or null when no exception in the chain named one.
    /// </summary>
    private static string? ArgumentName(Exception? error)
    {
        for (Exception? current = error; current is not null; current = current.InnerException)
        {
            if (current is ArgumentException argument && !string.IsNullOrWhiteSpace(argument.ParamName))
                return argument.ParamName;
        }
        return null;
    }

    /// <summary>
    /// ERR-06. The schema check, made before the tool body and before the binding layer reads the
    /// arguments. A call that leaves out a name the tool's input schema marks required is refused
    /// here, with the name in the message and in the value field, which is the one place the name
    /// is still in hand. A surface that let the binder answer instead would return one sentence
    /// that names only the tool, and a model that reads it retries the same call.
    /// </summary>
    public static McpRequestHandler<CallToolRequestParams, CallToolResult> RequiredArguments(
        McpRequestHandler<CallToolRequestParams, CallToolResult> next,
        Func<string, McpServerTool?> resolve)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(resolve);
        return async (request, cancellationToken) =>
        {
            string? name = request.Params?.Name;
            if (name is not null && resolve(name) is { } tool
                && MissingRequired(tool.ProtocolTool.InputSchema, request.Params?.Arguments) is { } missing)
            {
                throw Refuse(Wc3ErrorCodes.InvalidArgument,
                    "the call to \"" + name + "\" is missing the required argument \"" + missing + "\"",
                    missing,
                    "pass \"" + missing + "\" and call again");
            }
            return await next(request, cancellationToken).ConfigureAwait(false);
        };
    }

    /// <summary>
    /// ERR-06. The first name the input schema lists as required that the call does not carry, or
    /// null when it carries them all. The schema is the same one the client read, so the answer is
    /// the name the caller saw and not a name this server invented.
    /// <para>
    /// A nullable parameter that declares no default is listed as required by the schema while the
    /// binding layer accepts null for it, so a name whose schema admits null is left to the tool,
    /// which is where the meaning of an omitted value lives. Only a name that cannot be null and
    /// was not passed is refused here, and that is the call the binder would have failed.
    /// </para>
    /// </summary>
    private static string? MissingRequired(JsonElement schema, IDictionary<string, JsonElement>? arguments)
    {
        if (schema.ValueKind != JsonValueKind.Object
            || !schema.TryGetProperty("required", out JsonElement required)
            || required.ValueKind != JsonValueKind.Array)
            return null;

        schema.TryGetProperty("properties", out JsonElement properties);
        foreach (JsonElement item in required.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String) continue;
            string? parameter = item.GetString();
            if (string.IsNullOrEmpty(parameter)) continue;
            if (arguments is not null && arguments.ContainsKey(parameter)) continue;
            if (AdmitsNull(properties, parameter)) continue;
            return parameter;
        }
        return null;
    }

    /// <summary>ERR-06. True when the parameter's own schema lists null among its types.</summary>
    private static bool AdmitsNull(JsonElement properties, string name)
    {
        if (properties.ValueKind != JsonValueKind.Object
            || !properties.TryGetProperty(name, out JsonElement property)
            || !property.TryGetProperty("type", out JsonElement type))
            return false;
        if (type.ValueKind == JsonValueKind.String)
            return string.Equals(type.GetString(), "null", StringComparison.Ordinal);
        if (type.ValueKind != JsonValueKind.Array) return false;
        foreach (JsonElement item in type.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.String
                && string.Equals(item.GetString(), "null", StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    /// <summary>
    /// The call tool filter that turns a thrown refusal into the structured error result. It is the
    /// only place that knows the tool name, so it also fills that field. A client cancel is rethrown
    /// untouched, since the SDK drops the reply for one and it is not a failed call.
    /// </summary>
    public static McpRequestHandler<CallToolRequestParams, CallToolResult> StructuredErrors(
        McpRequestHandler<CallToolRequestParams, CallToolResult> next)
    {
        ArgumentNullException.ThrowIfNull(next);
        return async (request, cancellationToken) =>
        {
            try
            {
                return await next(request, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception) when (!IsServedTool(request.Params?.Name))
            {
                // A name no tool answers to is a protocol error, not a tool failure. The SDK raises
                // -32602 for it and converting that into a result would hide a caller's typo.
                throw;
            }
            catch (System.Text.Json.JsonException error)
            {
                // An argument of the wrong shape reaches the binder before any tool body, so it
                // used to surface as a raw deserializer sentence with no code. It is a caller
                // error and it names the tool and the value it could not read.
                return Result(
                    new ToolError("INVALID_ARGUMENT", "an argument could not be read as the type the tool declares, " + error.Message),
                    request.Params?.Name);
            }
            catch (Exception error)
            {
                return Result(error, request.Params?.Name);
            }
        };
    }

    /// <summary>True when some tool answers to the name, aliases included.</summary>
    private static bool IsServedTool(string? name) =>
        name is not null && McpToolFamilies.All.ContainsKey(name);
}

/// <summary>The four fields of a failure, plus the optional next step (ERR-03).</summary>
/// <param name="Code">The stable code, one of <see cref="Wc3ErrorCodes.All"/>.</param>
/// <param name="Message">The same sentence the text content carries, without the bracketed code.</param>
/// <param name="Tool">The tool that refused, as the client named it.</param>
/// <param name="Value">The offending value the caller passed, or null when the refusal names none.</param>
/// <param name="Next">The one retry that resolves the refusal, or null when none exists.</param>
public sealed record ErrorPayload(string Code, string Message, string? Tool, string? Value, string? Next);

/// <summary>
/// RES-03. A collection of unknown size, capped at <see cref="Wc3Limit.MaxItems"/> and carrying the
/// cap it was held to and whether the cap cut anything, so a caller never reads a truncated list as
/// the whole answer. <c>list_files</c> is the model, a page plus a flag, applied here to every tool
/// that returns a bare collection so the shape is the same across the surface.
/// </summary>
/// <param name="Cap">The most items this result may carry.</param>
/// <param name="Truncated">True when the source held more than <paramref name="Cap"/> items.</param>
/// <param name="Total">How many items the source held before the cap.</param>
/// <param name="Items">The items returned, never more than <paramref name="Cap"/>.</param>
public sealed record CappedItems<T>(int Cap, bool Truncated, int Total, IReadOnlyList<T> Items);

/// <summary>RES-03. The one cap and the one place it is applied to a bare collection.</summary>
public static class Wc3Limit
{
    /// <summary>
    /// RES-03. The most items a tool returns in one result. 500 is the figure the requirement names.
    /// A caller that needs the rest narrows the query, because every tool here filters by a name, a
    /// path or a kind rather than paging by offset.
    /// </summary>
    public const int MaxItems = 500;

    /// <summary>Cap one collection, naming the cap and whether it cut anything.</summary>
    public static CappedItems<T> Cap<T>(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (items.Count <= MaxItems)
            return new CappedItems<T>(MaxItems, false, items.Count, items);
        return new CappedItems<T>(MaxItems, true, items.Count, items.Take(MaxItems).ToArray());
    }
}
