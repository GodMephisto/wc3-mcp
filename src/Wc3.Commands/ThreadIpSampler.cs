// src/Wc3.Commands/ThreadIpSampler.cs
using System.Runtime.InteropServices;

namespace Wc3.Commands;

/// <summary>
/// Samples a thread's CURRENT instruction pointer and attributes it to a loaded module.
///
/// This replaces attribution by thread START address, which is useless: Windows puts every
/// thread's entry stub in ntdll, so every thread reports ntdll no matter what it is executing.
/// Knowing a hung game is spinning without knowing WHERE is only half an answer, so this reads
/// RIP directly.
///
/// Doing that requires briefly suspending the thread, which is why it is opt-in rather than part
/// of the default sample. Each suspend lasts microseconds and is always paired with a resume in a
/// finally block, but suspending a thread in a live game is not free of risk and the caller should
/// be choosing it deliberately.
/// </summary>
internal static class ThreadIpSampler
{
    private const int ThreadSuspendResume = 0x0002;
    private const int ThreadGetContext = 0x0008;
    private const int ThreadQueryInformation = 0x0040;

    // AMD64 CONTEXT: 1232 bytes, 16-byte aligned, with RIP at 0xF8. CONTEXT_CONTROL asks for
    // just the control registers, which is all RIP needs.
    private const int ContextSize = 1232;
    private const int ContextAlignment = 16;
    private const int ContextFlagsOffset = 0x30;
    private const int RipOffset = 0xF8;
    private const uint ContextControlAmd64 = 0x00100001;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenThread(int access, bool inherit, uint threadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern int ResumeThread(IntPtr hThread);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetThreadContext(IntPtr hThread, IntPtr lpContext);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr h);

    /// <summary>
    /// Reads the thread's instruction pointer once, or null if it cannot be obtained (access
    /// denied, thread exited, or a non-Windows/non-x64 host). Never leaves a thread suspended.
    /// </summary>
    public static ulong? CurrentInstructionPointer(int threadId)
    {
        if (!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X64)
            return null;

        var h = OpenThread(ThreadSuspendResume | ThreadGetContext | ThreadQueryInformation,
            false, (uint)threadId);
        if (h == IntPtr.Zero) return null;

        IntPtr raw = IntPtr.Zero;
        bool suspended = false;
        try
        {
            raw = Marshal.AllocHGlobal(ContextSize + ContextAlignment);
            long aligned = (raw.ToInt64() + ContextAlignment - 1) & ~((long)ContextAlignment - 1);
            var ctx = new IntPtr(aligned);
            for (int i = 0; i < ContextSize; i++) Marshal.WriteByte(ctx, i, 0);
            Marshal.WriteInt32(ctx, ContextFlagsOffset, unchecked((int)ContextControlAmd64));

            if (SuspendThread(h) == unchecked((uint)-1)) return null;
            suspended = true;

            // The context is only meaningful once the thread is actually suspended, which is the
            // whole reason for the suspend/resume pair.
            if (!GetThreadContext(h, ctx)) return null;
            return unchecked((ulong)Marshal.ReadInt64(ctx, RipOffset));
        }
        catch { return null; }
        finally
        {
            if (suspended) ResumeThread(h);
            if (raw != IntPtr.Zero) Marshal.FreeHGlobal(raw);
            CloseHandle(h);
        }
    }
}
