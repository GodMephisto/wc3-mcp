// tests/Wc3.Tests/TestCorpus.cs
namespace Wc3.Tests;

/// <summary>
/// Where the corpus tests find real maps. Set WC3_CORPUS_DIR to a folder of .w3x maps, or leave it
/// unset to use Documents\Warcraft III\Maps\Download. A corpus test whose map is missing skips.
/// </summary>
internal static class TestCorpus
{
    public static string MapsRoot { get; } = System.IO.Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps");

    public static string Directory { get; } =
        System.Environment.GetEnvironmentVariable("WC3_CORPUS_DIR") is { Length: > 0 } dir
            ? dir
            : System.IO.Path.Combine(MapsRoot, "Download");

    public static string Map(string fileName) => System.IO.Path.Combine(Directory, fileName);
}
