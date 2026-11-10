// src/Wc3.Mcp/Setup/DoctorProbe.cs
using Wc3.Commands;

namespace Wc3.Mcp.Setup;

/// <summary>
/// Where doctor reads what it cannot compute from the setup environment alone: the folder the
/// exe sits in, and the latest published release. Both are injectable so a test can point at a
/// temp folder and answer the release question without touching the network.
/// <see cref="Current"/> is the real machine: the exe's own folder, and the GitHub release list
/// of the repository this product ships from.
/// </summary>
public sealed record DoctorProbe(string BaseFolder, Func<SetupProduct, UpdateCheckResult> LatestRelease)
{
    /// <summary>
    /// How long doctor waits for the release list. A doctor run must not hang for the HTTP
    /// client's full five minutes on an offline machine or a black-holed route, so a slow answer
    /// is treated the same as no answer: skipped.
    /// </summary>
    public static readonly TimeSpan ReleaseTimeout = TimeSpan.FromSeconds(10);

    public static DoctorProbe Current { get; } = new(
        AppContext.BaseDirectory,
        product =>
        {
            using var cts = new CancellationTokenSource(ReleaseTimeout);
            return SelfUpdateCommand.CheckAsync(
                SetupCli.ReleaseRepo(product), product.Version, SetupCli.ReleasePrefix(product), cts.Token)
                .GetAwaiter().GetResult();
        });
}
