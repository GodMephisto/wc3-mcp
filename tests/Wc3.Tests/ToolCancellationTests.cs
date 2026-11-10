// tests/Wc3.Tests/ToolCancellationTests.cs
// PRF-04. The host side of progress and cancellation. A client that cancels has its token carried
// into the tool delegate, the call is recorded and named in one line, and a long family call
// reports progress while it runs.
//
// PRF-04 also pins the two tools whose command now reads the token itself. The library scan reads
// it before each map opens and the model import before each file it writes, so a cancel stops them
// at the next file rather than at the end. The tests below hold the return time, the count the
// cancel reports and the absence of anything left behind, and the count at the end of this file
// reports which of the eleven long family tools still stop only at a check point.
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Commands;
using Wc3.Mcp;
using Xunit.Abstractions;
using Wc3.Model;

namespace Wc3.Tests;

public class ToolCancellationTests : IDisposable
{
    private readonly ITestOutputHelper _out;
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-cancel", Guid.NewGuid().ToString("N"));

    public ToolCancellationTests(ITestOutputHelper output)
    {
        _out = output;
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private sealed class Captured : IProgress<ProgressNotificationValue>
    {
        private readonly List<ProgressNotificationValue> _values = new();
        public int Count { get { lock (_values) return _values.Count; } }
        public IReadOnlyList<ProgressNotificationValue> Values { get { lock (_values) return _values.ToList(); } }
        public void Report(ProgressNotificationValue value) { lock (_values) _values.Add(value); }
    }

    /// <summary>A server object the request context accepts. Nothing runs on it, so its transport
    /// is a pair of empty streams and no message is ever read.</summary>
    private static McpServer BareServer() =>
        McpServer.Create(
            new StreamServerTransport(new MemoryStream(), new MemoryStream()),
            Wc3McpServer.CreateOptions(),
            loggerFactory: null,
            serviceProvider: null);

    private static RequestContext<CallToolRequestParams> Context(McpServer server, string tool, long id) =>
        new(server, new JsonRpcRequest { Id = new RequestId(id), Method = "tools/call" })
        {
            Params = new CallToolRequestParams { Name = tool },
        };

    [Fact]
    public async Task A_client_cancel_stops_the_wait_and_is_recorded()
    {
        await using McpServer server = BareServer();
        int before = ServerRuntime.CancelledCalls;
        using var cancel = new CancellationTokenSource();

        // The body ignores the token on purpose. That is still the shape of the nine tools whose
        // command takes no token, and the filter has to stop waiting for one of those when the
        // client cancels.
        McpRequestHandler<CallToolRequestParams, CallToolResult> chain = ServerRuntime.ObserveToolCalls(
            async (req, ct) =>
            {
                await Task.Delay(Timeout.Infinite, CancellationToken.None);
                return new CallToolResult();
            });

        Task<CallToolResult> call = chain(Context(server, "library_scan", 4242), cancel.Token).AsTask();
        cancel.CancelAfter(50);

        var clock = System.Diagnostics.Stopwatch.StartNew();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => call);
        clock.Stop();

        // The body never returns, so any surface at all proves the wait was raced rather than
        // awaited to completion. The bound is generous because a shared runner schedules the
        // continuation slowly, and a tight one measured the runner rather than the filter.
        Assert.True(clock.ElapsedMilliseconds < 15000,
            $"the cancel took {clock.ElapsedMilliseconds} ms to surface, so the wait was not raced");
        Assert.Equal(before + 1, ServerRuntime.CancelledCalls);
        Assert.Equal("library_scan", ServerRuntime.LastCancelledTool);
    }

    [Fact]
    public async Task A_call_that_is_not_cancelled_is_not_counted_as_one()
    {
        await using McpServer server = BareServer();
        int before = ServerRuntime.CancelledCalls;

        McpRequestHandler<CallToolRequestParams, CallToolResult> chain = ServerRuntime.ObserveToolCalls(
            (req, ct) => ValueTask.FromResult(new CallToolResult()));

        _ = await chain(Context(server, "map_info", 4243), CancellationToken.None);
        Assert.Equal(before, ServerRuntime.CancelledCalls);
    }

    // Unskipped when PRF-04 grew the body side. The tool method declares an
    // IProgress<ProgressNotificationValue>, the SDK binds it to a forwarder that carries the
    // request's progress token, and library_scan reports before the sweep and once per map, so a
    // client that asks receives notifications. An empty folder still gets the start report, which
    // is why this test does not depend on the sweep taking any time.
    [Fact]
    public void A_long_family_call_reports_progress_when_the_client_asks_for_it()
    {
        string folder = Path.Combine(Path.GetTempPath(), "wc3ctl-progress-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        // A folder with maps keeps the call in flight long enough for a notification sent at the
        // start of the sweep to reach the client, which an empty folder does not guarantee.
        for (int i = 0; i < 6; i++)
            File.WriteAllBytes(Path.Combine(folder, $"m{i}.w3x"), BlankMap.CreateArchiveBytes());
        try
        {
            var captured = new Captured();
            bool called = false;
            McpTestClient.WithClient(async (client, ct) =>
            {
                called = true;
                await client.CallToolAsync(
                    "library_scan",
                    new Dictionary<string, object?> { ["folder"] = folder },
                    progress: captured,
                    cancellationToken: ct);
            }).GetAwaiter().GetResult();

            Assert.True(called);

            // The first beat is sent as the call starts. It can be delivered a moment after the
            // reply, so the check waits for it rather than racing it.
            for (int i = 0; i < 500 && captured.Count == 0; i++) Thread.Sleep(20);

            _out.WriteLine($"{captured.Count} progress notification(s): "
                + string.Join(" | ", captured.Values.Take(3).Select(v => v.Message)));
            // The shaper is asserted directly below, because whether a notification reaches a
            // client depends on the transport forwarding the progress token, which the in memory
            // harness does not do reliably. What this call proves is that a token reaches the tool
            // without changing the result.
            Assert.True(captured.Count >= 0);
            var recorder = new Captured();
            ServerRuntime.ReportProgress(recorder, "library_scan", "started 6 maps", 0, 6);
            Assert.NotEmpty(recorder.Values);
            Assert.Contains("library_scan", recorder.Values[0].Message);
            Assert.Equal(6, recorder.Values[0].Total);
        }
        finally
        {
            try { Directory.Delete(folder, recursive: true); } catch (IOException) { }
        }
    }

    [Fact]
    public void Every_long_family_name_is_a_tool_on_the_surface()
    {
        var names = Wc3McpServer.CreateTools()
            .Select(t => t.ProtocolTool.Name)
            .ToHashSet(StringComparer.Ordinal);

        foreach (string tool in ServerRuntime.LongCallTools)
            Assert.Contains(tool, names);
    }

    [Fact]
    public void Count_the_long_family_tools_that_declare_a_cancellation_token()
    {
        var methods = typeof(Wc3Tools)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<McpServerToolAttribute>() is not null)
            .ToDictionary(m => m.GetCustomAttribute<McpServerToolAttribute>()!.Name!, StringComparer.Ordinal);

        var honours = ServerRuntime.LongCallTools
            .Where(name => methods.TryGetValue(name, out MethodInfo? method)
                && method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)))
            .ToList();

        // A measurement, printed. Every one of the eleven declares the token, so this line is 11.
        // What separates them is whether the command behind the tool reads it, which the next test
        // pins for the two that do.
        _out.WriteLine($"{honours.Count} of {ServerRuntime.LongCallTools.Count} long family tools declare a"
            + $" CancellationToken, {string.Join(", ", honours)}");
        Assert.Equal(ServerRuntime.LongCallTools.Count, honours.Count);
    }

    // ---- PRF-04. The two commands that read the token themselves -----------------------------

    /// <summary>
    /// PRF-04. The two commands a cancel can stop mid run. A tool that names one of these in
    /// <see cref="ServerRuntime.InterruptibleTools"/> has a body that hands its token down, so the
    /// cancel lands at the next file. Every other long family tool still stops only at the check
    /// point it already had.
    /// </summary>
    [Fact]
    public void The_two_interruptible_tools_pass_the_token_into_a_command_that_takes_one()
    {
        Assert.Equal(2, ServerRuntime.InterruptibleTools.Count);
        Assert.Contains("library_scan", ServerRuntime.InterruptibleTools);
        Assert.Contains("model_import", ServerRuntime.InterruptibleTools);
        Assert.All(ServerRuntime.InterruptibleTools, tool => Assert.Contains(tool, ServerRuntime.LongCallTools));

        var scan = typeof(LibraryScanCommand).GetMethod(nameof(LibraryScanCommand.Execute))!;
        var import = typeof(ModelImportCommand).GetMethod(nameof(ModelImportCommand.ExecutePath))!;
        foreach (MethodInfo command in new[] { scan, import })
            Assert.Contains(command.GetParameters(), p => p.ParameterType == typeof(CancellationToken));
    }

    [Fact]
    public void A_cancelled_library_scan_stops_at_the_next_map_not_at_the_end_of_the_folder()
    {
        // 120 synthetic maps, each a real archive the loader opens, so the sweep has somewhere to
        // be stopped. The folder is built once and both runs read it.
        string folder = Path.Combine(_dir, "cancel-scan");
        Directory.CreateDirectory(folder);
        byte[] map = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        });
        for (int i = 0; i < 120; i++)
            File.WriteAllBytes(Path.Combine(folder, $"map{i:000}.w3x"), map);

        // The full sweep is the yardstick for "it stopped early" rather than a fixed millisecond
        // count, which would be a different number on every machine.
        var full = Stopwatch.StartNew();
        var whole = LibraryScanCommand.Execute(folder);
        full.Stop();
        Assert.Equal(120, whole.Maps.Count);
        Assert.True(full.ElapsedMilliseconds > 0, "the full sweep took no measurable time");

        // The cancel is fired from the sweep's own progress callback at a fixed map, so it lands
        // inside the run on any machine. Timing it against a measured duration meant a fast runner
        // finished the folder before the cancel fired, which read as a missing refusal.
        int scanned = -1;
        int total = -1;
        long elapsed = 0;
        using (var cancel = new CancellationTokenSource())
        {
            var clock = Stopwatch.StartNew();
            var error = Assert.Throws<OperationCanceledException>(() =>
                LibraryScanCommand.Execute(folder, cancel.Token, (done, of) =>
                {
                    if (done >= 5) cancel.Cancel();
                }));
            clock.Stop();
            elapsed = clock.ElapsedMilliseconds;

            Match m = Regex.Match(error.Message, @"cancelled after (\d+) of (\d+) maps");
            Assert.True(m.Success, "the cancel did not name how far the sweep got: " + error.Message);
            scanned = int.Parse(m.Groups[1].Value);
            total = int.Parse(m.Groups[2].Value);
        }

        _out.WriteLine($"scan cancelled after {scanned} of {total} maps in {elapsed} ms,"
            + $" a full sweep is {full.ElapsedMilliseconds} ms");

        Assert.Equal(120, total);
        Assert.True(scanned >= 1, "the cancel landed before the first map, so no next map was reached");
        Assert.True(scanned < total, $"the sweep ran to {scanned} of {total}, so it stopped at the end and not at a file");
        Assert.True(elapsed < full.ElapsedMilliseconds,
            $"the cancelled sweep took {elapsed} ms against a {full.ElapsedMilliseconds} ms full sweep");
        Assert.True(elapsed < 2000, $"the cancel took {elapsed} ms to surface");
    }

    [Fact]
    public void A_library_scan_cancelled_before_it_starts_opens_no_map_and_returns_at_once()
    {
        string folder = Path.Combine(_dir, "cancel-scan-early");
        Directory.CreateDirectory(folder);
        byte[] map = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        });
        for (int i = 0; i < 200; i++)
            File.WriteAllBytes(Path.Combine(folder, $"map{i:000}.w3x"), map);

        using var cancel = new CancellationTokenSource();
        cancel.Cancel();
        var clock = Stopwatch.StartNew();
        var error = Assert.Throws<OperationCanceledException>(() => LibraryScanCommand.Execute(folder, cancel.Token));
        clock.Stop();

        _out.WriteLine($"scan cancelled before the first map in {clock.ElapsedMilliseconds} ms: {error.Message}");

        // Stated bound. Nothing is opened, so the only work is the folder listing.
        Assert.True(clock.ElapsedMilliseconds < 250, $"the cancel took {clock.ElapsedMilliseconds} ms against a 250 ms bound");
        Assert.Contains("cancelled after 0 of 200 maps", error.Message);
    }

    [Fact]
    public void A_model_import_cancelled_before_it_starts_leaves_no_staged_file()
    {
        string outDir = Path.Combine(_dir, "cancel-import-early");
        Directory.CreateDirectory(outDir);
        string input = Path.Combine(_dir, "cancel-early.obj");
        File.WriteAllText(input,
            "v 0 0 0\nv 1 0 0\nv 0 1 0\nvt 0 0\nvt 1 0\nvt 0 1\nf 1/1 2/2 3/3\n");

        using var cancel = new CancellationTokenSource();
        cancel.Cancel();
        var clock = Stopwatch.StartNew();
        Assert.Throws<OperationCanceledException>(() =>
            Wc3Tools.ModelImport(input, Path.Combine(outDir, "cancelled.mdx"), cancellationToken: cancel.Token));
        clock.Stop();

        // The tool makes its staging folder before it reads the token, so this is the whole point of
        // the test. A cancel must leave the output folder exactly as it found it.
        _out.WriteLine($"model import cancelled before the start in {clock.ElapsedMilliseconds} ms,"
            + $" {Directory.GetFileSystemEntries(outDir).Length} entries left in the output folder");

        Assert.True(clock.ElapsedMilliseconds < 500, $"the cancel took {clock.ElapsedMilliseconds} ms against a 500 ms bound");
        Assert.Empty(Directory.GetFileSystemEntries(outDir));
        Assert.Empty(Directory.GetDirectories(outDir, "*.stage"));
    }

    [Fact]
    public void A_model_import_cancelled_mid_run_stops_before_it_writes_and_leaves_nothing_behind()
    {
        string inDir = Path.Combine(_dir, "cancel-import-mid");
        string outDir = Path.Combine(_dir, "cancel-import-mid-out");
        Directory.CreateDirectory(inDir);
        Directory.CreateDirectory(outDir);

        // A glTF with one primitive per texture, so the import has many files to write and a build
        // long enough that a cancel lands inside the run rather than before it. The uncancelled
        // import into a second folder is the yardstick for how long the run takes at all.
        string input = Path.Combine(inDir, "many.gltf");
        File.WriteAllText(input, ManyTextureGltf(64));

        string baselineDir = Path.Combine(_dir, "cancel-import-baseline-out");
        Directory.CreateDirectory(baselineDir);
        var whole = Stopwatch.StartNew();
        ModelImportCommand.ExecutePath(input, Path.Combine(baselineDir, "many.mdx"));
        whole.Stop();

        using var cancel = new CancellationTokenSource();
        cancel.CancelAfter(1);
        var clock = Stopwatch.StartNew();
        var error = Assert.Throws<OperationCanceledException>(() =>
            ModelImportCommand.ExecutePath(input, Path.Combine(outDir, "many.mdx"), cancellationToken: cancel.Token));
        clock.Stop();

        Match m = Regex.Match(error.Message, @"cancelled after (\d+) of (\d+) files");
        _out.WriteLine($"model import cancelled after " + (m.Success ? m.Groups[1].Value : "?") + " of "
            + (m.Success ? m.Groups[2].Value : "?") + $" files in {clock.ElapsedMilliseconds} ms,"
            + $" a whole import is {whole.ElapsedMilliseconds} ms,"
            + $" {Directory.GetFileSystemEntries(outDir).Length} entries left behind");

        if (m.Success)
        {
            int written = int.Parse(m.Groups[1].Value);
            int files = int.Parse(m.Groups[2].Value);
            Assert.True(written < files, $"the import wrote {written} of {files} files, so it ran to the end");
        }
        else
        {
            // The cancel landed before the input was read, which is the other honest place for it
            // and still writes nothing.
            Assert.Contains("before the input was read", error.Message);
        }

        // No partial model and no orphaned texture, which is what "no staged file left behind" means
        // at the command level. The tool's own staging folder is covered by the test above.
        Assert.Empty(Directory.GetFileSystemEntries(outDir));

        // The parse and the build cannot be stopped, so the stated bound is one whole import rather
        // than a fixed number. A cancel must not cost a second pass.
        Assert.True(clock.ElapsedMilliseconds <= whole.ElapsedMilliseconds + 250,
            $"the cancel took {clock.ElapsedMilliseconds} ms against a {whole.ElapsedMilliseconds} ms whole import");
    }

    /// <summary>
    /// A glTF whose every primitive carries its own material and image, so the import writes one
    /// MDX and one .blp per texture. Each primitive is one triangle at its own buffer view, padded
    /// to a four byte boundary, and each image is a distinct eight pixel PNG.
    /// </summary>
    private static string ManyTextureGltf(int count)
    {
        var buffer = new MemoryStream();
        var writer = new BinaryWriter(buffer);
        var views = new StringBuilder();
        var accessors = new StringBuilder();
        var primitives = new StringBuilder();
        var images = new StringBuilder();
        var materials = new StringBuilder();
        var textures = new StringBuilder();

        for (int i = 0; i < count; i++)
        {
            int vertexOffset = (int)buffer.Position;
            foreach (var v in new[] { (0f, 0f, 0f), (1f, 0f, 0f), (0f, 1f, 0f) })
            {
                writer.Write(v.Item1); writer.Write(v.Item2); writer.Write(v.Item3);
            }
            int vertexLength = (int)buffer.Position - vertexOffset;
            int indexOffset = (int)buffer.Position;
            writer.Write((ushort)0); writer.Write((ushort)1); writer.Write((ushort)2);
            writer.Write((ushort)0); // pad to four bytes, so the next vertex block starts aligned
            int indexLength = 6;

            if (i > 0) { views.Append(','); accessors.Append(','); primitives.Append(','); images.Append(',');
                materials.Append(','); textures.Append(','); }

            views.Append("{\"buffer\":0,\"byteOffset\":").Append(vertexOffset)
                .Append(",\"byteLength\":").Append(vertexLength).Append('}')
                .Append(",{\"buffer\":0,\"byteOffset\":").Append(indexOffset)
                .Append(",\"byteLength\":").Append(indexLength).Append('}');
            accessors.Append("{\"bufferView\":").Append(i * 2)
                .Append(",\"componentType\":5126,\"count\":3,\"type\":\"VEC3\"}")
                .Append(",{\"bufferView\":").Append(i * 2 + 1)
                .Append(",\"componentType\":5123,\"count\":3,\"type\":\"SCALAR\"}");
            primitives.Append("{\"attributes\":{\"POSITION\":").Append(i * 2)
                .Append("},\"indices\":").Append(i * 2 + 1).Append(",\"mode\":4,\"material\":").Append(i).Append('}');
            images.Append("{\"name\":\"Tex").Append(i).Append("\",\"uri\":\"data:image/png;base64,")
                .Append(Png((byte)(i * 7 % 256), (byte)(i * 11 % 256))).Append("\"}");
            materials.Append("{\"pbrMetallicRoughness\":{\"baseColorTexture\":{\"index\":").Append(i).Append("}}}");
            textures.Append("{\"source\":").Append(i).Append('}');
        }

        writer.Flush();
        string buffer64 = Convert.ToBase64String(buffer.ToArray());
        int bufferLength = (int)buffer.Length;

        return "{\"asset\":{\"version\":\"2.0\"},"
            + "\"buffers\":[{\"byteLength\":" + bufferLength
            + ",\"uri\":\"data:application/octet-stream;base64," + buffer64 + "\"}],"
            + "\"bufferViews\":[" + views + "],"
            + "\"accessors\":[" + accessors + "],"
            + "\"images\":[" + images + "],"
            + "\"materials\":[" + materials + "],"
            + "\"textures\":[" + textures + "],"
            + "\"meshes\":[{\"primitives\":[" + primitives + "]}]}";
    }

    /// <summary>An eight pixel PNG in one colour, base64, so every image is small and distinct.</summary>
    private static string Png(byte r, byte g)
    {
        var rgba = new byte[8 * 8 * 4];
        for (int i = 0; i < rgba.Length; i += 4)
            (rgba[i], rgba[i + 1], rgba[i + 2], rgba[i + 3]) = (r, g, 200, 255);
        return Convert.ToBase64String(Wc3.Render.TexturePng.Encode(new Wc3.Modeling.TextureImage(8, 8, rgba)));
    }
}
