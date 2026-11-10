// tests/Wc3.Tests/PlatformGuardTests.cs
// A self-contained win-x64 binary meets the operating system's own error on any other platform,
// which reads like a corrupt download. The guard is a function of the measured platform, so these
// tests pin the refusal for platforms this machine is not, plus the pass case it runs on.
using System.Runtime.InteropServices;
using Wc3.Mcp;

namespace Wc3.Tests;

public sealed class PlatformGuardTests
{
    [Fact]
    public void A_linux_process_is_refused_and_names_win_x64()
    {
        string? refusal = Program.PlatformRefusal(OSPlatform.Linux, Architecture.X64);

        Assert.NotNull(refusal);
        Assert.Contains(Program.SupportedTarget, refusal);
        Assert.Contains("Linux", refusal);
    }

    [Fact]
    public void A_windows_arm64_process_is_refused_and_names_win_x64()
    {
        string? refusal = Program.PlatformRefusal(OSPlatform.Windows, Architecture.Arm64);

        Assert.NotNull(refusal);
        Assert.Contains(Program.SupportedTarget, refusal);
        Assert.Contains("arm64", refusal);
    }

    [Fact]
    public void A_macos_process_is_refused_and_names_win_x64()
    {
        string? refusal = Program.PlatformRefusal(OSPlatform.OSX, Architecture.X64);

        Assert.NotNull(refusal);
        Assert.Contains(Program.SupportedTarget, refusal);
    }

    [Fact]
    public void A_windows_x64_process_is_allowed()
    {
        Assert.Null(Program.PlatformRefusal(OSPlatform.Windows, Architecture.X64));
    }
}
