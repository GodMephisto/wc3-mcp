// tests/Wc3.Tests/TempDir.cs
namespace Wc3.Tests;

/// <summary>A throwaway empty directory removed on dispose.</summary>
internal sealed class TempDir : IDisposable
{
    public string Dir { get; } = Path.Combine(Path.GetTempPath(), "wc3ctl-tempdir", Guid.NewGuid().ToString("N"));

    public TempDir() => Directory.CreateDirectory(Dir);

    public void Dispose()
    {
        try { Directory.Delete(Dir, recursive: true); } catch (IOException) { }
    }
}
