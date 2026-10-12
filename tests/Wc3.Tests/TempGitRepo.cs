// tests/Wc3.Tests/TempGitRepo.cs
// A throwaway git repository for the release-tooling tests, so nothing reads or writes the real
// checkout. Keep it off any network and give every commit a fixed identity.
using System.Diagnostics;
using System.Text;

namespace Wc3.Tests;

internal sealed class TempGitRepo : IDisposable
{
    public string Dir { get; } = Path.Combine(Path.GetTempPath(), "wc3ctl-gitfixture", Guid.NewGuid().ToString("N"));

    public TempGitRepo()
    {
        Directory.CreateDirectory(Dir);
        Git("init", "-q");
        Git("config", "user.email", "test@example.test");
        Git("config", "user.name", "Release Test");
        Git("config", "commit.gpgsign", "false");
    }

    public void Write(string relative, string content)
    {
        string path = Path.Combine(Dir, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }

    public string Commit(string message)
    {
        Git("add", "-A");
        Git("commit", "-q", "-m", message);
        return Git("rev-parse", "HEAD").Trim();
    }

    public void Tag(string name) => Git("tag", name);

    public string Git(params string[] args)
    {
        var psi = new ProcessStartInfo("git")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Dir,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);
        using var p = Process.Start(psi) ?? throw new InvalidOperationException("git did not start");
        string stdout = p.StandardOutput.ReadToEnd();
        string stderr = p.StandardError.ReadToEnd();
        p.WaitForExit();
        if (p.ExitCode != 0)
            throw new InvalidOperationException($"git {string.Join(' ', args)} failed, {stderr.Trim()}");
        return stdout;
    }

    public void Dispose()
    {
        try
        {
            // Git marks its object files read-only on Windows, so clear that before deleting.
            foreach (var file in Directory.EnumerateFiles(Dir, "*", SearchOption.AllDirectories))
            {
                try { File.SetAttributes(file, FileAttributes.Normal); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            Directory.Delete(Dir, recursive: true);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
