// tests/Wc3.Tests/StdioDrainTests.cs
// TRN-03. The read side of the stdio transport must not report end of stream while a message it
// already read is still waiting for a handler. A client that writes a call and closes the pipe at
// once hits end of stream before the handler has started, and the transport then drops the response
// for a peer it considers gone. The queue is the counter the gate was missing, so these tests drive
// the gate through a stream that is already at end of stream.
using System.Diagnostics;
using Wc3.Mcp;

namespace Wc3.Tests;

public class StdioDrainTests
{
    [Fact]
    public async Task End_of_stream_is_held_while_a_queued_message_waits_for_a_handler()
    {
        int queued = 1;
        bool handlersIdle = true;
        var gate = new ServerRuntime.DrainGate(
            () => Volatile.Read(ref handlersIdle), () => true);
        gate.Attach(() => Volatile.Read(ref queued));
        var stdin = new ServerRuntime.DrainingStdinStream(new MemoryStream(), gate);

        Task<int> read = stdin.ReadAsync(new byte[64], 0, 64);

        // A message that was read from the pipe but not dispatched yet still holds the end of
        // stream. This is the case that dropped a response before the gate knew about the queue.
        await Task.Delay(120);
        Assert.False(read.IsCompleted, "end of stream was reported while a message was still queued");

        // The handler picked it up, so a running call holds the stream next.
        queued = 0;
        handlersIdle = false;
        await Task.Delay(120);
        Assert.False(read.IsCompleted, "end of stream was reported while a call was still running");

        // The call finished and its response is on the pipe.
        handlersIdle = true;
        int count = await read.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(0, count);
        Assert.Equal(0, gate.Queued());
    }

    [Fact]
    public async Task End_of_stream_returns_once_nothing_is_pending()
    {
        var gate = new ServerRuntime.DrainGate(() => true, () => true);
        gate.Attach(() => 0);
        var stdin = new ServerRuntime.DrainingStdinStream(new MemoryStream(), gate);

        var clock = Stopwatch.StartNew();
        int count = await stdin.ReadAsync(new byte[64], 0, 64).WaitAsync(TimeSpan.FromSeconds(5));
        clock.Stop();

        Assert.Equal(0, count);
        // Two samples at the quiet poll, so a normal close costs a few milliseconds and no more.
        Assert.True(clock.ElapsedMilliseconds < 1000,
            "an idle stream took " + clock.ElapsedMilliseconds + " ms to report end of stream");
    }

    [Fact]
    public void A_count_the_channel_cannot_answer_reads_as_nothing_queued()
    {
        var gate = new ServerRuntime.DrainGate(() => true, () => true);
        gate.Attach(() => throw new NotSupportedException());

        Assert.Equal(0, gate.Queued());
        Assert.True(gate.IsIdle);
    }
}
