// tests/Wc3.Tests/Reforged3ImportsClassificationTests.cs
using Wc3.Commands;
using Wc3.GameData;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// An import is a file the map author added. Everything else in the archive is either MPQ
/// bookkeeping or a standard map file, and ImportsCommand decides which by asking
/// MapFormatRegistry and then falling back to a hand-maintained exclusion list.
///
/// That list predates Reforged 3.0.0, so the files the new editor writes are not on it and are
/// not registered either. The consequence is that a map's own lighting, its conversation data and
/// its per-locale string tables get reported as things the author imported, on maps Blizzard
/// themselves ship. Anything that trusts the import list, a port or an asset audit, then carries
/// or counts engine data as user content.
/// </summary>
public class Reforged3ImportsClassificationTests
{
    private const string Install = @"C:\Warcraft III";
    private readonly ITestOutputHelper _out;
    public Reforged3ImportsClassificationTests(ITestOutputHelper output) => _out = output;

    [Theory]
    [Trait("Category", "GameData")]
    [InlineData(@"war3.w3mod:maps\frozenthrone\(2)echoisles.w3x")]
    [InlineData(@"war3.w3mod:maps\scenario\facefxcinematicstest.w3x")]
    public void A_blizzard_map_has_no_user_imports(string cascPath)
    {
        if (!CascGameDataSource.TryOpen(Install, out var casc, out _) || casc is null)
        {
            _out.WriteLine("no install");
            return;
        }
        using var source = casc;

        var bytes = source.ReadFile(cascPath);
        if (bytes is null || bytes.Length == 0) { _out.WriteLine($"cannot read {cascPath}"); return; }

        var result = ImportsCommand.Execute(MapDocument.Load(bytes));
        foreach (var e in result.Entries)
            _out.WriteLine($"  reported as import: {e.Path}  ({e.SizeBytes?.ToString("N0") ?? "absent"} bytes)");

        // A map Blizzard ships with no war3mapImported folder has no user imports. Every row here
        // is the toolkit mistaking engine data for author content.
        Assert.Empty(result.Entries);
    }
}
