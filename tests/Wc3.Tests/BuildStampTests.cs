// tests/Wc3.Tests/BuildStampTests.cs
// The build stamp is best effort. A source tree without git must bake "nogit" and a build time,
// never the text git printed when it failed, so this runs the real stamping target from the
// repository's Directory.Build.props against a folder git does not see as a repository.
using System.Diagnostics;
using System.Globalization;
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class BuildStampTests
{
    [Fact]
    public void A_build_without_git_stamps_nogit_and_a_build_time()
    {
        string? props = RepoLayout.FindBuildProps();
        Assert.True(props is not null && File.Exists(props),
            "Directory.Build.props was not found above the test binaries");

        using var tmp = new TempDir();
        var folder = NotAGitRepository(tmp.Dir);
        try
        {
            string probe = Path.Combine(folder.Dir, "probe.proj");
            string stamp = Path.Combine(folder.Dir, "stamp.txt");
            File.WriteAllText(probe,
                "<Project>\n"
                + "  <Import Project=\"" + props + "\" />\n"
                + "  <Target Name=\"EchoStamp\" DependsOnTargets=\"StampBuildInfo\">\n"
                + "    <WriteLinesToFile File=\"$(StampFile)\" Lines=\"$(_GitHash)|$(_BuildTimeUtc)\" Overwrite=\"true\" />\n"
                + "  </Target>\n"
                + "</Project>\n");

            var (exit, stdout, stderr) = Run(folder.Dir,
                "dotnet", "msbuild", probe, "-t:EchoStamp", "-p:StampFile=" + stamp, "-nologo", "-v:m");

            Assert.True(exit == 0, "dotnet msbuild failed in a folder without git. " + stdout + stderr);
            Assert.True(File.Exists(stamp), "the stamping target wrote no property file. " + stdout + stderr);

            string[] parts = File.ReadAllText(stamp).Trim().Split('|');
            Assert.Equal(2, parts.Length);
            Assert.Equal(ReleaseVersionCommand.NoGit, parts[0]);
            Assert.False(string.IsNullOrWhiteSpace(parts[1]), "the build time was empty");
            Assert.True(DateTime.TryParseExact(parts[1], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _), "the build time was not a UTC stamp, got " + parts[1]);
            // The failure git printed must not leak into the stamp, which is what the fallback is for.
            Assert.DoesNotContain("fatal", parts[0], StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (folder.Owns) { try { Directory.Delete(folder.Dir, recursive: true); } catch (IOException) { } }
        }
    }

    /// <summary>One folder git does not resolve to a repository, preferring the temp folder.</summary>
    private static (string Dir, bool Owns) NotAGitRepository(string seed)
    {
        if (!IsInGit(seed)) return (seed, false);
        string root = Path.GetPathRoot(seed) ?? "C:\\";
        string alt = Path.Combine(root, "wc3-nogit-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(alt);
        Assert.False(IsInGit(alt), "both " + seed + " and " + alt + " sit inside a git repository");
        return (alt, true);
    }

    private static bool IsInGit(string dir)
    {
        var (exit, stdout, _) = Run(dir, "git", "rev-parse", "--show-toplevel");
        return exit == 0 && !string.IsNullOrWhiteSpace(stdout);
    }

    private static (int Exit, string Stdout, string Stderr) Run(string dir, string exe, params string[] args)
    {
        var psi = new ProcessStartInfo(exe)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = dir,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);
        try
        {
            using var p = Process.Start(psi);
            if (p is null) return (127, string.Empty, "could not start " + exe);
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            return (p.ExitCode, stdout, stderr);
        }
        catch (Exception ex)
        {
            return (127, string.Empty, ex.Message);
        }
    }
}
