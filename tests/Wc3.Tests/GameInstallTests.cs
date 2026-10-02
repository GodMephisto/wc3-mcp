using Wc3.GameData;
namespace Wc3.Tests;
public class GameInstallTests
{
    [Fact] public void Override_that_exists_is_returned()
    {
        var dir = Directory.CreateTempSubdirectory().FullName;
        try { Assert.Equal(dir, GameInstall.Locate(dir)); }
        finally { Directory.Delete(dir); }
    }
    [Fact] public void Nonexistent_override_falls_through_or_null()
    {
        var bogus = Path.Combine(Path.GetTempPath(), "no_such_wc3_" + System.Guid.NewGuid().ToString("N"));
        var result = GameInstall.Locate(bogus);
        Assert.NotEqual(bogus, result); // never returns a path that doesn't exist
    }
}
