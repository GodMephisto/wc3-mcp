// src/Wc3.Commands/GameHangCommand.cs
using System.Diagnostics;

namespace Wc3.Commands;

public sealed record ThreadSample(
    int Id,
    double CpuMillisecondsUsed,
    string State,
    string WaitReason,
    string StartModule);

/// <summary>Where the busiest thread was actually executing, by loaded module.</summary>
public sealed record IpHit(string Module, int Samples, double Percent, string ExampleAddress);

/// <summary>One distinct instruction address the hot thread was caught at.</summary>
public sealed record IpAddress(string Module, string ModuleOffset, int Samples, double Percent);

public sealed record HangReport(
    bool Found,
    string Message,
    int ProcessId,
    double SampleSeconds,
    double ProcessCpuMilliseconds,
    double CpuCoresBusy,
    string Verdict,
    IReadOnlyList<ThreadSample> BusiestThreads,
    IReadOnlyList<IpHit> HotModules,
    IReadOnlyList<IpAddress> HotAddresses,
    string? HotThreadNote);

/// <summary>
/// Samples a running Warcraft III process to answer the question no static comparison could:
/// when a map sits forever on the loading screen, is the game SPINNING (an infinite loop in its
/// own code) or WAITING (blocked on I/O or a lock)? Those are different bugs with different
/// fixes, and "Not Responding" in the title bar looks identical for both.
///
/// This exists because a map whose archive is measurably equivalent to a working one still would
/// not load, so the artifact told us nothing and the behaviour had to be observed instead.
/// Deliberately read-only: it attaches nothing, injects nothing, and only reads counters the OS
/// already publishes, so it is safe to run against a live game.
/// </summary>
public static class GameHangCommand
{
    /// <summary>Process names Warcraft III ships under, newest first.</summary>
    private static readonly string[] ProcessNames = { "Warcraft III", "war3", "Frozen Throne" };

    public static HangReport Sample(double seconds = 5.0, int topThreads = 6,
        string? processName = null, int ipSamples = 0)
    {
        var names = processName is null ? ProcessNames : new[] { processName };
        var proc = names
            .SelectMany(n => { try { return Process.GetProcessesByName(n); } catch { return Array.Empty<Process>(); } })
            .FirstOrDefault();

        if (proc is null)
            return new(false,
                $"No running Warcraft III process found (looked for {string.Join(", ", names)}). "
                + "Start the game and let the map reach the loading screen, then run this again.",
                0, 0, 0, 0, "unknown", Array.Empty<ThreadSample>(), Array.Empty<IpHit>(), Array.Empty<IpAddress>(), null);

        try
        {
            var before = SnapshotThreads(proc);
            var cpuBefore = proc.TotalProcessorTime;
            var wall = Stopwatch.StartNew();
            Thread.Sleep((int)(seconds * 1000));
            wall.Stop();

            proc.Refresh();
            var cpuAfter = proc.TotalProcessorTime;
            var after = SnapshotThreads(proc);

            double elapsed = wall.Elapsed.TotalSeconds;
            double cpuMs = (cpuAfter - cpuBefore).TotalMilliseconds;
            // Cores' worth of work. ~1.0 means one thread pinned, which is a spin. Near 0 means
            // the process is asleep, so it is waiting on something rather than computing.
            double coresBusy = elapsed <= 0 ? 0 : cpuMs / (elapsed * 1000.0);

            var deltas = after
                .Select(a => new ThreadSample(a.Key,
                    Math.Round(a.Value.cpu - (before.TryGetValue(a.Key, out var b) ? b.cpu : 0), 1),
                    a.Value.state, a.Value.wait, a.Value.module))
                .OrderByDescending(t => t.CpuMillisecondsUsed)
                .Take(topThreads)
                .ToList();

            string verdict = coresBusy switch
            {
                >= 0.75 => "SPINNING - a thread is pinned, so the game is looping in its own code, "
                           + "not waiting on the file. Look at what that thread's module does.",
                >= 0.10 => "BUSY - real work is happening. It may simply be slow rather than hung, "
                           + "so sample again after a minute and compare.",
                _ => "WAITING - almost no CPU is being used, so the game is blocked on I/O or a lock "
                     + "rather than looping. Check the threads' wait reasons below.",
            };

            var hot = deltas.FirstOrDefault();
            var hotModules = Array.Empty<IpHit>() as IReadOnlyList<IpHit>;
            var hotAddrs = Array.Empty<IpAddress>() as IReadOnlyList<IpAddress>;
            string? hotNote = null;
            if (ipSamples > 0 && hot is not null && hot.CpuMillisecondsUsed > 0)
                (hotModules, hotAddrs, hotNote) = SampleHotThread(proc, hot.Id, ipSamples);
            else if (ipSamples > 0)
                hotNote = "no thread used measurable CPU, so there was nothing to locate.";

            return new(true, $"sampled '{proc.ProcessName}' for {elapsed:0.0}s",
                proc.Id, Math.Round(elapsed, 2), Math.Round(cpuMs, 1),
                Math.Round(coresBusy, 3), verdict, deltas, hotModules, hotAddrs, hotNote);
        }
        catch (Exception ex)
        {
            return new(false, $"could not sample the process ({ex.Message})",
                proc.Id, 0, 0, 0, "unknown", Array.Empty<ThreadSample>(), Array.Empty<IpHit>(), Array.Empty<IpAddress>(), null);
        }
        finally { proc.Dispose(); }
    }

    /// <summary>
    /// Repeatedly reads the busiest thread's live instruction pointer and buckets it by module.
    /// A loop concentrates its samples in one module, which is the attribution that thread START
    /// addresses cannot give (those all report ntdll, where Windows puts entry stubs).
    /// </summary>
    private static (IReadOnlyList<IpHit>, IReadOnlyList<IpAddress>, string?) SampleHotThread(Process proc, int threadId, int count)
    {
        var modules = ModuleRanges(proc);
        var byModule = new Dictionary<string, (int n, ulong example)>(StringComparer.OrdinalIgnoreCase);
        // Distinct addresses, so a tight loop (a few repeated addresses) is distinguishable from
        // broad computation (hundreds of them). Reported as module+offset, which stays comparable
        // across runs despite address-space layout randomisation moving the base.
        var byAddress = new Dictionary<(string mod, long off), int>();
        int taken = 0;
        for (int i = 0; i < count; i++)
        {
            var ip = ThreadIpSampler.CurrentInstructionPointer(threadId);
            if (ip is null) { Thread.Sleep(5); continue; }
            taken++;
            var name = ModuleOf(modules, unchecked((long)ip.Value));
            var prev = byModule.TryGetValue(name, out var v) ? v : (0, ip.Value);
            byModule[name] = (prev.Item1 + 1, prev.Item2);
            long baseAddr = modules.Where(m => m.name == name).Select(m => m.start).FirstOrDefault();
            var key = (name, unchecked((long)ip.Value) - baseAddr);
            byAddress[key] = byAddress.TryGetValue(key, out var c) ? c + 1 : 1;
            Thread.Sleep(5);
        }
        if (taken == 0)
            return (Array.Empty<IpHit>(), Array.Empty<IpAddress>(),
                "could not read the thread's instruction pointer (access denied, or the thread "
                + "exited). Running this from an elevated shell usually resolves it.");

        var hits = byModule
            .Select(kv => new IpHit(kv.Key, kv.Value.n, Math.Round(100.0 * kv.Value.n / taken, 1),
                "0x" + kv.Value.example.ToString("X")))
            .OrderByDescending(h => h.Samples)
            .ToList();
        var addrs = byAddress
            .Select(kv => new IpAddress(kv.Key.mod, "+0x" + kv.Key.off.ToString("X"), kv.Value,
                Math.Round(100.0 * kv.Value / taken, 1)))
            .OrderByDescending(a => a.Samples)
            .Take(15)
            .ToList();
        return (hits, addrs,
            $"{taken} of {count} sample(s) succeeded, hitting {byAddress.Count} distinct address(es).");
    }

    private static Dictionary<int, (double cpu, string state, string wait, string module)>
        SnapshotThreads(Process proc)
    {
        var modules = ModuleRanges(proc);
        var d = new Dictionary<int, (double, string, string, string)>();
        foreach (ProcessThread t in proc.Threads)
        {
            double cpu = 0; string state = "?", wait = "-", module = "?";
            try { cpu = t.TotalProcessorTime.TotalMilliseconds; } catch { }
            try { state = t.ThreadState.ToString(); } catch { }
            // WaitReason only exists while the thread is actually in Wait, and throws otherwise.
            try { wait = t.ThreadState == System.Diagnostics.ThreadState.Wait ? t.WaitReason.ToString() : "-"; }
            catch { wait = "-"; }
            try { module = ModuleOf(modules, t.StartAddress.ToInt64()); } catch { }
            d[t.Id] = (cpu, state, wait, module);
        }
        return d;
    }

    /// <summary>
    /// A thread's START address identifies which loaded module it began in, which is as much
    /// attribution as is available without symbols or a stack walk. It is still enough to
    /// separate "spinning inside the archive/IO layer" from "spinning in game logic".
    /// </summary>
    private static List<(long start, long end, string name)> ModuleRanges(Process proc)
    {
        var list = new List<(long, long, string)>();
        try
        {
            foreach (ProcessModule m in proc.Modules)
            {
                long b = m.BaseAddress.ToInt64();
                list.Add((b, b + m.ModuleMemorySize, m.ModuleName));
            }
        }
        catch { /* a 64-bit target from a differently-bitted host can refuse this */ }
        return list;
    }

    private static string ModuleOf(List<(long start, long end, string name)> modules, long address)
    {
        foreach (var (start, end, name) in modules)
            if (address >= start && address < end) return name;
        return "?";
    }
}
