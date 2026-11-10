// src/Wc3.Mcp/ServerRuntime.cs
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// The stdio host side of a tool call, kept out of the tool bodies. OBS-02 and OBS-04 name one slow
/// call and one failed call in a single line each, TRN-03 holds the input open until a call in
/// flight has written its response, and PRF-05 puts the working set on the start line when a folder
/// tool has pushed the process past a gigabyte. Both hooks are the SDK 1.4.1 request and message
/// filters, registered on the options at tool build time in <see cref="Wc3McpServer.CreateOptions"/>.
/// No line here carries a parameter value or any map content.
///
/// PRF-04 adds two behaviours at the same request filter. A tool in the long families gets a
/// progress heartbeat when the client asked for one, and every call records a client cancel so the
/// line names the tool that was stopped. The two tools that can say where they are send their own
/// reports from the tool body, through <see cref="ReportProgress"/> and the IProgress the SDK binds
/// for the call, so the client sees a map count and a file count rather than only a beat.
/// PRF-05 refuses a folder call over an entry ceiling before
/// a single map opens, and PRF-09 refuses an input whose measured projection is past the stated
/// memory budget. PRF-01 records the four cold start phases and writes them as one line, and the
/// descriptor build that the initialize reply no longer waits for gets its own line when it runs.
/// </summary>
internal static class ServerRuntime
{
    /// <summary>The logger category the two per call lines use. Program keeps it at information.</summary>
    public const string Category = "Wc3.Mcp.ToolCalls";

    /// <summary>A call longer than this gets its own line even when it succeeded.</summary>
    public static readonly TimeSpan SlowCall = TimeSpan.FromSeconds(2);

    /// <summary>How long the read side waits for calls in flight once stdin has reported end of stream.</summary>
    public static readonly TimeSpan DrainTimeout = TimeSpan.FromMinutes(5);

    /// <summary>After the handlers are idle, how long a response still gets to reach the pipe.</summary>
    public static readonly TimeSpan ResponseGrace = TimeSpan.FromSeconds(5);

    /// <summary>PRF-04. How often a call in a long family reports that it is still running.</summary>
    public static readonly TimeSpan ProgressInterval = TimeSpan.FromSeconds(2);

    /// <summary>PRF-05, one gibibyte of working set, the point where the start line gains the note.</summary>
    public const long MemoryLimitBytes = 1024L * 1024 * 1024;

    /// <summary>
    /// PRF-05. The most map files one folder call may name. The reference corpus folder holds more
    /// than this, and at 500 maps a whole folder load is what the bound exists to stop. A call over
    /// the ceiling is refused before a map opens, and the refusal names the ceiling.
    /// </summary>
    public const int MaxFolderMaps = 256;

    /// <summary>The JSON-RPC code the SDK uses for a request the client cancelled.</summary>
    private const int RequestCancelled = -32800;

    private static readonly object Gate = new();
    private static readonly Dictionary<RequestId, PendingCall> Calls = new();

    private static int _handlers;
    private static int _responses;
    private static int _messagesInFlight;
    private static string? _currentTool;
    private static int _memoryNoteWritten;
    private static int _cancelledCalls;
    private static string? _lastCancelledTool;

    /// <summary>
    /// PRF-04. The families the specification names as expected to exceed two seconds. A client that
    /// asked for progress gets a heartbeat while one of these runs, and this set is the whole of the
    /// tools the host reports for. A name here that is not a tool fails LongCallTests.
    /// </summary>
    public static readonly IReadOnlyCollection<string> LongCallTools = new HashSet<string>(StringComparer.Ordinal)
    {
        "map_optimize",
        "library_scan", "library_audit", "library_validate", "library_report", "library_compare",
        "model_import", "model_export_obj",
        "asset_convert_batch", "asset_thumbnails",
        "doctor_fix",
    };

    /// <summary>PRF-04. True when the tool is in a family the specification names as slow.</summary>
    public static bool IsLongCall(string tool) => LongCallTools.Contains(tool);

    /// <summary>
    /// PRF-04. One progress report from inside a tool body, which is the half of the requirement
    /// the request filter cannot answer. SDK 1.4.1 binds a parameter of type
    /// <c>IProgress&lt;ProgressNotificationValue&gt;</c> to a forwarder
    /// made for the call, and that forwarder sends notifications/progress under the progress token
    /// the request named. A call that named no token gets an instance that drops every report, so a
    /// tool that reports unconditionally still works and still says nothing to a client that did not
    /// ask. The message always starts with the tool name, because that is what the client shows.
    /// A report that cannot be written, because the pipe is gone, is dropped rather than failing a
    /// sweep that is otherwise making progress.
    /// </summary>
    public static void ReportProgress(
        IProgress<ProgressNotificationValue>? progress,
        string tool,
        string phrase,
        int done = 0,
        int total = 0)
    {
        if (progress is null) return;
        var value = new ProgressNotificationValue
        {
            Progress = done,
            Message = tool + " " + phrase,
            Total = total > 0 ? total : null,
        };
        try { progress.Report(value); }
        catch (Exception) { /* the client is gone, the tool keeps working */ }
    }

    /// <summary>
    /// PRF-04. The two long family tools whose command reads the token itself. The library scan
    /// reads it before each map opens and the model import before each file it writes, so a cancel
    /// stops them at the next file rather than at the end of the run. The other nine take the token
    /// and read it only at the check points the tool owns, so a cancel there still waits for the one
    /// step that is already running. Kept beside <see cref="LongCallTools"/> so the two lists are
    /// read together, and a test pins both.
    /// </summary>
    public static readonly IReadOnlyList<string> InterruptibleTools = new[] { "library_scan", "model_import" };

    /// <summary>PRF-04. How many calls a client cancelled in this process.</summary>
    public static int CancelledCalls => Volatile.Read(ref _cancelledCalls);

    /// <summary>PRF-04. The tool the newest client cancel stopped, or null when none was seen.</summary>
    public static string? LastCancelledTool
    {
        get { lock (Gate) return _lastCancelledTool; }
    }

    /// <summary>PRF-05. The tool the newest call started, or null when nothing is running.</summary>
    public static string? CurrentTool
    {
        get { lock (Gate) return _currentTool; }
    }

    /// <summary>PRF-05. True once the process working set is over the line.</summary>
    public static bool IsOverMemoryLimit() => Environment.WorkingSet > MemoryLimitBytes;

    /// <summary>PRF-05. The working set in whole mebibytes, for the start line note.</summary>
    public static long MemoryMegabytes() => Environment.WorkingSet / (1024 * 1024);

    /// <summary>
    /// OBS-02 and OBS-04. The call tool filter the server registers on its options. It applies the
    /// PRF-05 folder ceiling and the PRF-09 memory budget before the tool runs, starts the progress
    /// heartbeat for a long family when the client asked for one, starts the clock, names the tool
    /// while it runs, and on a failure logs the code and the elapsed time once before rethrowing. A
    /// success is left to the outgoing message filter, which sees the finished response and can
    /// count its bytes.
    /// </summary>
    public static McpRequestHandler<CallToolRequestParams, CallToolResult> ObserveToolCalls(
        McpRequestHandler<CallToolRequestParams, CallToolResult> next)
    {
        ArgumentNullException.ThrowIfNull(next);
        return async (request, cancellationToken) =>
        {
            string tool = request.Params?.Name ?? "unknown";

            // Both guards refuse before the clock starts, so a refused call is not reported as a
            // slow one and the response is the same shape a tool error takes.
            if (RefuseBeforeStart(request, tool) is { } refusal) return refusal;

            RequestId id = request.JsonRpcRequest?.Id ?? new RequestId(0);
            long started = Stopwatch.GetTimestamp();
            Begin(id, tool);
            NoteMemoryPressure();

            using IDisposable? beats = ProgressHeartbeat.Start(request, tool);
            Task<CallToolResult>? work = null;
            try
            {
                work = next(request, cancellationToken).AsTask();

                // PRF-04. The wait is raced against the client's token rather than relying on the
                // tool body to notice it, because a tool body that takes no token would otherwise
                // hold the request open for its whole run after the client has gone.
                CallToolResult result = await work.WaitAsync(cancellationToken).ConfigureAwait(false);
                End(id, Stopwatch.GetElapsedTime(started).TotalMilliseconds, failed: false);
                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // PRF-04. A client cancel is not a failure the operator can act on, and the SDK
                // drops the reply, so this is one information line naming the tool and the time.
                // The body keeps running because its signature takes no token, so the abandoned
                // task is observed here rather than left to the finalizer.
                double elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                End(id, elapsed, failed: true);
                RecordCancel(tool);
                if (work is not null)
                {
                    _ = work.ContinueWith(
                        finished => _ = finished.Exception,
                        CancellationToken.None,
                        TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                }
                LoggerFor(request).Log(
                    LogLevel.Information,
                    "wc3 tool {Tool} cancelled by the client after {Elapsed} ms",
                    tool,
                    (long)elapsed);
                throw;
            }
            catch (Exception error)
            {
                double elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                End(id, elapsed, failed: true);
                // A shutdown cancellation is not a failed call the operator can act on, and the SDK
                // drops the reply for it, so the line would be noise.
                if (!cancellationToken.IsCancellationRequested)
                {
                    LoggerFor(request).Log(
                        LogLevel.Warning,
                        "wc3 tool {Tool} failed with code {Code} after {Elapsed} ms",
                        tool,
                        ErrorCode(error),
                        (long)elapsed);
                }
                throw;
            }
        };
    }

    /// <summary>
    /// SUR-02 and SUR-03. The alias window, applied at the call path rather than inside a tool
    /// body. A call under an old name is renamed to the current tool before the lookup, so the
    /// arguments travel untouched, no tool body learns that a second name exists and tools/list
    /// keeps one entry per tool. A successful call gains one line saying which name the caller
    /// used, and an alias whose removal version has been reached is refused with a code naming
    /// that version instead of being dispatched. A name that is not in the table is left alone,
    /// so an unknown tool still reaches the protocol layer and fails there as it always has.
    /// <para>
    /// SUR-02. The SDK matches the request against the tool collection before any call filter
    /// runs, so the match it holds is the name the caller used and an old name matches nothing.
    /// The rewrite therefore re-resolves the name it wrote, through the same collection, and the
    /// dispatch that follows reaches the tool the alias names. That lookup materializes the
    /// deferred surface if nothing has read it yet, so a rename works on a process that never
    /// answered a tools/list.
    /// </para></summary>
    public static McpRequestHandler<CallToolRequestParams, CallToolResult> AliasToolCalls(
        McpRequestHandler<CallToolRequestParams, CallToolResult> next,
        IReadOnlyDictionary<string, ToolAlias>? renames = null,
        DeferredToolCollection? tools = null)
    {
        ArgumentNullException.ThrowIfNull(next);
        IReadOnlyDictionary<string, ToolAlias> table = renames ?? McpToolCatalog.Renames;
        return async (request, cancellationToken) =>
        {
            string called = request.Params?.Name ?? "";
            if (request.Params is null || !table.TryGetValue(called, out ToolAlias? alias))
                return await next(request, cancellationToken).ConfigureAwait(false);

            if (McpToolCatalog.RemovalReached(alias.RemovalVersion, ServerIdentity.Version))
                return Refusal(AliasExpiredCode, AliasExpiredMessage(alias));

            // The one rewrite. Everything after this point, the permission policy included, sees
            // the name the surface serves.
            string served = McpToolCatalog.ServedName(called, table);
            request.Params.Name = served;

            // The match the SDK made before the filters ran still names the caller's name. Left in
            // place, the dispatch finds no tool and answers Unknown tool for the name it has just
            // been given. Resolving the served name here answers that lookup from the one deferred
            // build, so the call lands on the tool the window points at.
            request.MatchedPrimitive = tools?.Find(served) ?? FindTool(served);

            CallToolResult result = await next(request, cancellationToken).ConfigureAwait(false);
            NoteAlias(result, alias);
            return result;
        };
    }

    /// <summary>
    /// SUR-02. The same name lookup for a window that was built without the collection, which a
    /// hand-built window can be. It reads the one memoized surface, so a rewrite always has a way
    /// to resolve the name it wrote.
    /// </summary>
    private static McpServerTool? FindTool(string name) =>
        Wc3McpServer.CreateTools()
            .FirstOrDefault(t => string.Equals(t.ProtocolTool.Name, name, StringComparison.Ordinal));

    /// <summary>SUR-03. The code an alias past its removal version is refused with.</summary>
    internal const string AliasExpiredCode = "ALIAS_EXPIRED";

    /// <summary>The refusal, in the shape a tool error takes, naming the version that ended the window.</summary>
    private static string AliasExpiredMessage(ToolAlias alias) =>
        "The name \"" + alias.OldName + "\" was removed in " + alias.RemovalVersion
        + ", so the alias window has closed and this build is " + ServerIdentity.Version
        + ". Nothing ran. Call \"" + alias.CurrentName + "\" instead, which is the name the"
        + " surface serves.";

    /// <summary>
    /// SUR-03. One text block after the tool's own content, so a client can see that the answer
    /// came from a name it did not list. The tool's own block is left exactly as the tool wrote it.
    /// </summary>
    private static void NoteAlias(CallToolResult result, ToolAlias alias)
    {
        string note = "[ALIAS] called as \"" + alias.OldName + "\" and served by \"" + alias.CurrentName
            + "\". The old name is deprecated and is removed in " + alias.RemovalVersion + ".";
        IList<ContentBlock> content = result.Content ?? new List<ContentBlock>();
        content.Add(new TextContentBlock { Text = note });
        result.Content = content;
    }

    /// <summary>
    /// PRF-01. The incoming message filter. It remembers the id of the initialize request, so the
    /// outgoing filter can tell the handshake reply from every later response and close the cold
    /// start clock on the reply rather than on the request. TRN-03 rides the same filter, which the
    /// SDK runs around the whole of a message, dispatch included. The count it keeps is therefore
    /// the exact answer to whether a message that was read is still being answered, and it covers
    /// the first tool call, where the descriptor build happens between the queue and the handler.
    /// </summary>
    public static McpMessageHandler ObserveIncomingMessages(McpMessageHandler next)
    {
        ArgumentNullException.ThrowIfNull(next);
        return async (context, cancellationToken) =>
        {
            if (context.JsonRpcMessage is JsonRpcRequest { Method: "initialize" } request)
                ColdStart.InitializeRequested(request.Id);
            Interlocked.Increment(ref _messagesInFlight);
            try
            {
                await next(context, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Decrement(ref _messagesInFlight);
            }
        };
    }

    /// <summary>
    /// OBS-02. The outgoing message filter. It runs after the response has been written, so the
    /// drain in TRN-03 only releases the input once the bytes are on the pipe, and it is the place
    /// where a slow call over two seconds gets its line with the response size.
    /// </summary>
    public static McpMessageHandler ObserveOutgoingResponses(McpMessageHandler next)
    {
        ArgumentNullException.ThrowIfNull(next);
        return async (context, cancellationToken) =>
        {
            try
            {
                await next(context, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ColdStart.ReplyWritten(context.JsonRpcMessage);
                FinishResponse(context, context.JsonRpcMessage);
            }
        };
    }

    /// <summary>
    /// TRN-03. Waits for the calls in flight to finish and their responses to be written, through
    /// the gate the stdio stream holds. Kept for the exit path, which takes the last look after the
    /// host has stopped, and for a test that wants the same answer without a stream.
    /// </summary>
    public static Task WaitForDrainAsync(TimeSpan timeout, CancellationToken cancellationToken = default) =>
        new DrainGate().WaitAsync(timeout, cancellationToken);

    /// <summary>
    /// TRN-03. What the stream waits on at end of stream. A call is in flight while a tool request
    /// is running, while its response has not reached the pipe, and while a message read from stdin
    /// is still queued in the transport. The third case is the one that was missed. A client that
    /// writes a call and closes the pipe at once hits end of stream before the handler has picked
    /// the request up, so a gate that counted only dispatched calls let the transport disconnect
    /// first, and the transport drops a response for a disconnected peer without an error.
    /// </summary>
    public sealed class DrainGate
    {
        private readonly Func<bool> _messagesIdle;
        private readonly Func<bool> _responsesIdle;
        private Func<int>? _queued;

        public DrainGate()
            : this(null, null)
        {
        }

        /// <summary>
        /// A gate over the counters a test supplies, so the gate can be driven without a message
        /// arriving on the shared process.
        /// </summary>
        internal DrainGate(Func<bool>? messagesIdle, Func<bool>? responsesIdle)
        {
            _messagesIdle = messagesIdle ?? (() => MessagesIdle);
            _responsesIdle = responsesIdle ?? (() => ResponsesIdle);
        }

        /// <summary>
        /// The transport's pending message count, attached once the transport exists. A count that
        /// the channel cannot answer is read as nothing queued, which is the old behaviour rather
        /// than a wait that never ends.
        /// </summary>
        public void Attach(Func<int> queued) =>
            _queued = queued ?? throw new ArgumentNullException(nameof(queued));

        /// <summary>Messages read from stdin that no handler has picked up yet.</summary>
        public int Queued()
        {
            Func<int>? queued = Volatile.Read(ref _queued);
            if (queued is null) return 0;
            try { return queued(); }
            catch (NotSupportedException) { return 0; }
        }

        /// <summary>True when no message is being handled.</summary>
        internal bool MessagesIdle => Volatile.Read(ref _messagesInFlight) == 0;

        /// <summary>True when no tool response is still on its way to the pipe.</summary>
        internal bool ResponsesIdle
        {
            get { lock (Gate) return _responses == 0; }
        }

        /// <summary>True when nothing that was read is still waiting for an answer.</summary>
        internal bool IsIdle => Queued() == 0 && _messagesIdle();

        /// <summary>
        /// Waits until the queue and the messages are both quiet, then until the last response has
        /// been written. The quiet reading has to hold over two samples, because the instant
        /// between a message leaving the transport and its filter starting is invisible to both
        /// counters. <paramref name="timeout"/> bounds the first wait and
        /// <see cref="ResponseGrace"/> bounds the second, so a response that is never sent cannot
        /// hold the process for the whole drain window.
        /// </summary>
        public async Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            await WaitUntilAsync(() => IsIdle, timeout, cancellationToken).ConfigureAwait(false);
            await WaitUntilAsync(_responsesIdle, ResponseGrace, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>How long between two reads of the drain counters.</summary>
    private static readonly TimeSpan QuietPoll = TimeSpan.FromMilliseconds(10);

    /// <summary>
    /// How long a quiet reading must hold before it counts as no work left. The SDK dequeues a
    /// message and yields before it starts the incoming filter, so two adjacent samples can both
    /// miss a request that is about to be dispatched. A held reading covers that gap.
    /// </summary>
    private static readonly TimeSpan QuietHold = TimeSpan.FromMilliseconds(250);

    /// <summary>Waits for a condition to hold continuously for a while, bounded by a timeout.</summary>
    private static async Task WaitUntilAsync(
        Func<bool> condition, TimeSpan timeout, CancellationToken cancellationToken, TimeSpan? hold = null)
    {
        TimeSpan need = hold ?? QuietHold;
        Stopwatch clock = Stopwatch.StartNew();
        Stopwatch steady = new Stopwatch();
        bool holding = false;
        while (true)
        {
            if (condition())
            {
                if (!holding) { holding = true; steady.Restart(); }
                else if (steady.Elapsed >= need) return;
            }
            else
            {
                holding = false;
                steady.Reset();
            }
            if (clock.Elapsed >= timeout) return;
            try { await Task.Delay(QuietPoll, cancellationToken).ConfigureAwait(false); }
            catch (OperationCanceledException) { return; }
            catch (ObjectDisposedException) { return; }
        }
    }

    /// <summary>
    /// TRN-03. Standard input with one difference. When the inner stream reports end of stream the
    /// read waits on the gate instead of reporting it, so a call that was already read, or is still
    /// running, answers before the transport sees the end of stream and disconnects.
    /// </summary>
    public sealed class DrainingStdinStream : Stream
    {
        private readonly Stream _inner;
        private readonly DrainGate _gate;

        public DrainingStdinStream(Stream inner, DrainGate? gate = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _gate = gate ?? new DrainGate();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = _inner.Read(buffer, offset, count);
            if (read == 0) _gate.WaitAsync(DrainTimeout).GetAwaiter().GetResult();
            return read;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int read = await _inner.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if (read == 0) await _gate.WaitAsync(DrainTimeout, cancellationToken).ConfigureAwait(false);
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int read = await _inner.ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);
            if (read == 0) await _gate.WaitAsync(DrainTimeout, cancellationToken).ConfigureAwait(false);
            return read;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing) _inner.Dispose();
            base.Dispose(disposing);
        }
    }

    private sealed class PendingCall
    {
        public PendingCall(string tool) => Tool = tool;

        public string Tool { get; }
        public double ElapsedMs { get; set; }
        public bool Failed { get; set; }
    }

    private static void Begin(RequestId id, string tool)
    {
        lock (Gate)
        {
            _handlers++;
            _responses++;
            _currentTool = tool;
            Calls[id] = new PendingCall(tool);
        }
    }

    private static void End(RequestId id, double elapsedMs, bool failed)
    {
        lock (Gate)
        {
            if (Calls.TryGetValue(id, out PendingCall? call))
            {
                call.ElapsedMs = elapsedMs;
                call.Failed = failed;
            }
            if (_handlers > 0) _handlers--;
            if (_handlers == 0) _currentTool = null;
        }
    }

    private static void FinishResponse(MessageContext context, JsonRpcMessage message)
    {
        // A server initiated request can reuse a client id value, so only a response or an error
        // may end a pending tool call.
        if (message is not (JsonRpcResponse or JsonRpcError)) return;
        JsonRpcMessageWithId withId = (JsonRpcMessageWithId)message;

        PendingCall? call;
        lock (Gate)
        {
            if (!Calls.Remove(withId.Id, out call)) return;
            if (_responses > 0) _responses--;
        }

        // A failed call already has its warning line, and a fast call is not worth a line.
        if (call.Failed || call.ElapsedMs < SlowCall.TotalMilliseconds) return;

        LoggerFor(context).Log(
            LogLevel.Information,
            "wc3 tool {Tool} finished in {Elapsed} ms with a {Bytes} byte response",
            call.Tool,
            (long)call.ElapsedMs,
            WireBytes(message));
    }

    /// <summary>
    /// The bytes the stdio transport will write for this message, counted the same way it
    /// serializes, plus the newline it appends. The serializer writes into a counting stream rather
    /// than a byte array, so measuring a large response does not double its footprint. Only called
    /// for a slow call, so the second serialization costs nothing on the common path.
    /// </summary>
    private static int WireBytes(JsonRpcMessage message)
    {
        try
        {
            var counter = new CountingStream();
            JsonSerializer.Serialize(counter, message, typeof(JsonRpcMessage), McpJsonUtilities.DefaultOptions);
            return (int)Math.Min(int.MaxValue, counter.Count + 1);
        }
        catch (Exception)
        {
            // A byte count is not worth breaking a response that is already written.
            return 0;
        }
    }

    /// <summary>A sink that keeps only the count, used to measure a response without holding it twice.</summary>
    private sealed class CountingStream : Stream
    {
        public long Count { get; private set; }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => Count;
        public override long Position
        {
            get => Count;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => Count += count;
        public override void Write(ReadOnlySpan<byte> buffer) => Count += buffer.Length;
        public override void WriteByte(byte value) => Count++;
    }

    /// <summary>
    /// PRF-05. When the process is already over a gigabyte as a call starts, the start line is
    /// repeated on stderr with the working set and the tool in flight. Once per process, so a
    /// folder tool that stays over the line does not fill the log.
    /// </summary>
    private static void NoteMemoryPressure()
    {
        if (!IsOverMemoryLimit()) return;
        if (Interlocked.Exchange(ref _memoryNoteWritten, 1) != 0) return;
        Console.Error.WriteLine(Wc3McpServer.BuildStartLine());
    }

    /// <summary>
    /// PRF-05 and PRF-09. The two guards that run before a tool body does. A call over the folder
    /// ceiling or past the memory budget is answered with a tool error naming the limit, because a
    /// refusal the client can read is worth more than a bound the operator cannot see.
    /// </summary>
    private static CallToolResult? RefuseBeforeStart(RequestContext<CallToolRequestParams> request, string tool)
    {
        IDictionary<string, JsonElement>? arguments = request.Params?.Arguments;
        if (arguments is null) return null;

        // PRF-09 first, so a 495 MB archive is named for its size rather than counted as a folder.
        foreach (string key in FileInputKeys)
        {
            if (!arguments.TryGetValue(key, out JsonElement value) || value.ValueKind != JsonValueKind.String) continue;
            string path = value.GetString() ?? "";
            long bytes;
            try
            {
                var file = new FileInfo(path);
                if (!file.Exists) continue;
                bytes = file.Length;
            }
            catch (Exception)
            {
                // An unreadable path is the tool's error to report, not the guard's.
                continue;
            }
            if (MemoryBudget.WouldExceed(bytes))
                return Refusal("PRF09_MEMORY_BUDGET", MemoryBudget.Explain(bytes));
        }

        // PRF-05, the folder ceiling. Counting stops one past the ceiling, so a folder of thousands
        // costs one directory walk and never a load.
        foreach (string key in FolderInputKeys)
        {
            if (!arguments.TryGetValue(key, out JsonElement value) || value.ValueKind != JsonValueKind.String) continue;
            string folder = value.GetString() ?? "";
            if (!Directory.Exists(folder)) continue;
            int found = CountMaps(folder, MaxFolderMaps + 1);
            if (found > MaxFolderMaps)
                return Refusal("PRF05_FOLDER_ENTRIES",
                    tool + " counted at least " + found + " map files under the folder and the ceiling is "
                    + MaxFolderMaps + " per call. A whole folder loaded at once is what PRF-05 bounds, so"
                    + " split the folder or call the tool once per subfolder.");
        }

        return null;
    }

    /// <summary>The tool arguments that name one map archive, which is what the input budget measures.</summary>
    private static readonly string[] FileInputKeys = { "map" };

    /// <summary>The tool arguments that name a folder of maps, which is what the entry ceiling counts.</summary>
    private static readonly string[] FolderInputKeys = { "folder", "folder_a", "folder_b" };

    /// <summary>How many map files sit under a folder, stopping at <paramref name="stopAt"/>.</summary>
    private static int CountMaps(string folder, int stopAt)
    {
        int found = 0;
        try
        {
            foreach (string path in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(path);
                if (!extension.Equals(".w3x", StringComparison.OrdinalIgnoreCase)
                    && !extension.Equals(".w3m", StringComparison.OrdinalIgnoreCase)) continue;
                if (++found >= stopAt) return found;
            }
        }
        catch (Exception)
        {
            // An unreadable tree is the tool's error to report.
        }
        return found;
    }

    /// <summary>A refusal in the shape a tool error takes, with the code first.</summary>
    private static CallToolResult Refusal(string code, string message) =>
        new()
        {
            IsError = true,
            Content = new List<ContentBlock> { new TextContentBlock { Text = "[" + code + "] " + message } },
        };

    /// <summary>PRF-04. Counts a client cancel and keeps the tool name for the tests and the line.</summary>
    private static void RecordCancel(string tool)
    {
        Interlocked.Increment(ref _cancelledCalls);
        lock (Gate) _lastCancelledTool = tool;
    }

    /// <summary>
    /// PRF-09. The stated memory budget. The measured anchor is the reference machine on 2026-11-10,
    /// where map_overview peaked at 201 MB of working set for a 38 MB archive and 204 MB for the
    /// 767 KB reference map, so the fixed part is the surface itself and the parsed sections grow
    /// with the archive. Eight bytes of working set per input byte is that growth rounded up.
    /// </summary>
    public static class MemoryBudget
    {
        /// <summary>The working set one call may reach on the reference machine.</summary>
        public const long BudgetBytes = 1024L * 1024 * 1024;

        /// <summary>The surface and the runtime, measured at 204 MB and rounded up for headroom.</summary>
        public const long FixedBytes = 320L * 1024 * 1024;

        /// <summary>Working set added per byte of input archive, rounded up from the measurement.</summary>
        public const int BytesPerInputByte = 8;

        /// <summary>The largest archive this budget supports, which the projection puts at 88 MiB.</summary>
        public const long LargestSupportedInputBytes = (BudgetBytes - FixedBytes) / BytesPerInputByte;

        /// <summary>What a call on an input of this size is projected to need.</summary>
        public static long ProjectedBytes(long inputBytes) => FixedBytes + inputBytes * BytesPerInputByte;

        /// <summary>
        /// True when the projection for this input is past the budget. The current working set is
        /// not part of it, because a process that is already large is a note on the start line
        /// rather than a reason to refuse a small input.
        /// </summary>
        public static bool WouldExceed(long inputBytes) => ProjectedBytes(inputBytes) > BudgetBytes;

        /// <summary>One sentence a client can act on, naming the budget and the machine it was measured on.</summary>
        public static string Explain(long inputBytes) =>
            "This call is refused rather than attempted. The input is " + inputBytes + " bytes and the"
            + " projection is " + ProjectedBytes(inputBytes) + " bytes of working set, against a budget of "
            + BudgetBytes + " bytes per call on the reference machine (Windows 10 x64, 32 GB, the win-x64"
            + " self contained build, measured 201 MB peak for a 38 MB map on 2026-11-10). The largest"
            + " supported input is " + LargestSupportedInputBytes + " bytes.";
    }

    /// <summary>
    /// PRF-04. The progress heartbeat. A client that sent a progress token for a tool in a long
    /// family gets one notification as the call starts and one every two seconds after it, so a
    /// four minute folder sweep is not a silent pipe. No line here carries a parameter value.
    /// </summary>
    private sealed class ProgressHeartbeat : IDisposable
    {
        private readonly McpServer _server;
        private readonly ProgressToken _token;
        private readonly string _tool;
        private readonly CancellationTokenSource _stop = new();

        private ProgressHeartbeat(McpServer server, ProgressToken token, string tool)
        {
            _server = server;
            _token = token;
            _tool = tool;
        }

        /// <summary>The beat for this call, or null when the tool is short or the client asked for none.</summary>
        public static IDisposable? Start(RequestContext<CallToolRequestParams> request, string tool)
        {
            if (!IsLongCall(tool)) return null;
            if (request.Params?.ProgressToken is not { } token) return null;
            if (request.Server is not { } server) return null;
            var beat = new ProgressHeartbeat(server, token, tool);
            _ = Task.Run(beat.RunAsync);
            return beat;
        }

        private async Task RunAsync()
        {
            Stopwatch clock = Stopwatch.StartNew();
            int beat = 0;
            while (!_stop.IsCancellationRequested)
            {
                try
                {
                    // The send is not cancelled with the beat, so the notification a client is
                    // waiting for is not withdrawn a millisecond after the call began.
                    await _server.SendNotificationAsync(
                        "notifications/progress",
                        new ProgressNotificationParams
                        {
                            ProgressToken = _token,
                            Progress = new ProgressNotificationValue
                            {
                                Progress = beat,
                                Message = _tool + " still running at "
                                    + (long)clock.Elapsed.TotalMilliseconds + " ms",
                            },
                        },
                        McpJsonUtilities.DefaultOptions,
                        CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // A pipe that closed or a client that went away ends the beat. The call itself
                    // is watched by the cancellation token the SDK handed the handler.
                    return;
                }

                beat++;
                try { await Task.Delay(ProgressInterval, _stop.Token).ConfigureAwait(false); }
                catch (OperationCanceledException) { return; }
                catch (ObjectDisposedException) { return; }
            }
        }

        public void Dispose()
        {
            try { _stop.Cancel(); } catch (ObjectDisposedException) { }
        }
    }

    /// <summary>
    /// PRF-01. The three phases of a cold start, recorded once per process and written as one line
    /// on stderr when the binary serves and the initialize reply has gone out. The origin is the
    /// first touch of this type, which is the first statement of Program.Main, so startup carries
    /// the runtime and the JIT, host carries the host build, surface carries the tool collection,
    /// and handshake carries everything from the later of host and surface to the reply.
    /// </summary>
    internal static class ColdStart
    {
        private static readonly object Sync = new();
        private static readonly long Origin = Stopwatch.GetTimestamp();

        private static long _startLineTicks = -1;
        private static long _hostBuiltTicks = -1;
        private static long _surfaceDoneTicks = -1;
        private static RequestId? _initializeId;
        private static int _enabled;
        private static int _written;

        private static double _startupMs = -1;
        private static double _hostMs = -1;
        private static double _surfaceMs = -1;
        private static double _handshakeMs = -1;
        private static double _totalMs = -1;
        private static double _descriptorsMs = -1;
        private static int _descriptorCount;
        private static string? _line;

        /// <summary>What the surface build cost, or -1 before it was recorded.</summary>
        public static double SurfaceMs { get { lock (Sync) return _surfaceMs; } }

        /// <summary>PRF-01. What the deferred descriptor build cost, or -1 before it ran.</summary>
        public static double DescriptorsMs { get { lock (Sync) return _descriptorsMs; } }

        /// <summary>PRF-01. How many descriptors the deferred build produced, or 0 before it ran.</summary>
        public static int DescriptorCount { get { lock (Sync) return _descriptorCount; } }

        /// <summary>The handshake from the later of host and surface to the initialize reply.</summary>
        public static double HandshakeMs { get { lock (Sync) return _handshakeMs; } }

        /// <summary>Process start to the initialize reply.</summary>
        public static double TotalMs { get { lock (Sync) return _totalMs; } }

        /// <summary>The one line, or null until the initialize reply has gone out.</summary>
        public static string? Line { get { lock (Sync) return _line; } }

        /// <summary>The binary serves, so the line belongs on stderr. A test leaves this off.</summary>
        public static void Enable() => Volatile.Write(ref _enabled, 1);

        /// <summary>The start line has reached stderr, which ends the startup phase.</summary>
        public static void StartLineWritten()
        {
            lock (Sync) { if (_startLineTicks < 0) _startLineTicks = Stopwatch.GetTimestamp(); }
        }

        /// <summary>The host is built, which ends the host build phase.</summary>
        public static void HostBuilt()
        {
            lock (Sync) { if (_hostBuiltTicks < 0) _hostBuiltTicks = Stopwatch.GetTimestamp(); }
        }

        /// <summary>The tool collection is built, timed from the caller's start tick.</summary>
        public static void SurfaceBuilt(long startedTicks)
        {
            long done = Stopwatch.GetTimestamp();
            lock (Sync)
            {
                if (_surfaceMs >= 0) return;
                _surfaceMs = (done - startedTicks) * 1000.0 / Stopwatch.Frequency;
                _surfaceDoneTicks = done;
            }
        }

        /// <summary>
        /// PRF-01. The deferred descriptor build finished, so its cost is written once on stderr
        /// rather than absorbed into a phase that no longer contains it. The exe enables the line,
        /// a test reads <see cref="DescriptorsMs"/> instead.
        /// </summary>
        public static void DescriptorsBuilt(int count, TimeSpan elapsed)
        {
            string line;
            lock (Sync)
            {
                if (_descriptorsMs >= 0) return;
                _descriptorsMs = elapsed.TotalMilliseconds;
                _descriptorCount = count;
                line = "wc3 tool descriptors " + _descriptorCount + " built in "
                    + Round(_descriptorsMs) + " ms on first use";
            }
            if (Volatile.Read(ref _enabled) != 0) Console.Error.WriteLine(line);
        }

        /// <summary>The initialize request arrived, so the handshake has a reply to wait for.</summary>
        public static void InitializeRequested(RequestId id)
        {
            lock (Sync) _initializeId ??= id;
        }

        /// <summary>The reply to the initialize request has been written.</summary>
        public static void ReplyWritten(JsonRpcMessage message)
        {
            string? line;
            lock (Sync)
            {
                if (_written != 0 || _initializeId is not { } id) return;
                if (message is not (JsonRpcResponse or JsonRpcError)) return;
                if (!((JsonRpcMessageWithId)message).Id.Equals(id)) return;
                _written = 1;
                line = Format();
                _line = line;
            }
            if (Volatile.Read(ref _enabled) != 0 && line is not null) Console.Error.WriteLine(line);
        }

        private static string Format()
        {
            long now = Stopwatch.GetTimestamp();
            _startupMs = _startLineTicks < 0 ? 0 : Ms(_startLineTicks);
            _hostMs = _hostBuiltTicks < 0 ? 0 : Ms(_hostBuiltTicks) - _startupMs;
            _totalMs = Ms(now);
            long ready = Math.Max(_hostBuiltTicks, _surfaceDoneTicks);
            _handshakeMs = ready < 0 ? _totalMs : _totalMs - Ms(ready);
            return "wc3 cold start startup " + Round(_startupMs)
                + " ms host " + Round(_hostMs)
                + " ms surface " + Round(_surfaceMs)
                + " ms handshake " + Round(_handshakeMs)
                + " ms total " + Round(_totalMs) + " ms";
        }

        private static double Ms(long ticks) => (ticks - Origin) * 1000.0 / Stopwatch.Frequency;

        private static long Round(double milliseconds) => (long)Math.Round(milliseconds);
    }

    private static ILogger LoggerFor(MessageContext context) =>
        context.Services?.GetService(typeof(ILoggerFactory)) is ILoggerFactory factory
            ? factory.CreateLogger(Category)
            : NullLogger.Instance;

    private static int ErrorCode(Exception error) => error switch
    {
        McpProtocolException protocol => (int)protocol.ErrorCode,
        McpException => (int)McpErrorCode.InternalError,
        OperationCanceledException => RequestCancelled,
        _ => (int)McpErrorCode.InternalError,
    };
}
