namespace Wc3.GameData;

/// <summary>
/// Game data read from a folder of loose files instead of a CASC install, under the same storage
/// names the CASC source answers to (<c>war3.w3mod:units\unitmetadata.slk</c>). It accepts the
/// layouts the files are usually found in.
/// <list type="bullet">
/// <item>A <c>gamedata snapshot</c> folder, where each layer is a directory
/// (<c>war3.w3mod\_locales\enus.w3mod\ui\...</c>).</item>
/// <item>The same without the root layer (<c>_locales\enus.w3mod\ui\...</c>).</item>
/// <item>A flat extract that keeps only the path inside the layer (<c>scripts\common.j</c>,
/// <c>units\unitmetadata.slk</c>), as jass-history and most mod tool repositories store them.</item>
/// </list>
/// The first of those that exists wins, so a layered folder is never answered from a flat
/// fallback by mistake.
/// </summary>
public sealed class FolderGameDataSource : IGameDataSource
{
    private readonly string _root;

    public FolderGameDataSource(string root)
    {
        if (!Directory.Exists(root)) throw new DirectoryNotFoundException($"folder not found: {root}");
        _root = Path.GetFullPath(root);
    }

    /// <summary>The paths under the root that a storage name may live at, most specific first.</summary>
    public static IReadOnlyList<string> CandidatePaths(string name)
    {
        static string Rel(string s) => s.Replace(':', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)
            .Trim(Path.DirectorySeparatorChar);

        var list = new List<string> { Rel(name) };
        int firstColon = name.IndexOf(':');
        if (firstColon >= 0) list.Add(Rel(name[(firstColon + 1)..]));
        int lastColon = name.LastIndexOf(':');
        if (lastColon >= 0) list.Add(Rel(name[(lastColon + 1)..]));
        // Fallback: some repos store UI strings under enus-strings/ instead of
        // _locales\enus.w3mod/. Try the same path with enus-strings/ replacing
        // _locales\enus.w3mod:.
        var idxBack = name.IndexOf("_locales\\enus.w3mod:", StringComparison.OrdinalIgnoreCase);
        var idxFwd = name.IndexOf("_locales/enus.w3mod:", StringComparison.OrdinalIgnoreCase);
        if (idxBack >= 0 || idxFwd >= 0)
        {
            var idx = idxBack >= 0 ? idxBack : idxFwd;
            list.Add(Rel("enus-strings/" + name[(idx + 19)..]));
        }
        return list.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>Which file under the root answers a storage name, or null.</summary>
    public string? Resolve(string name) =>
        CandidatePaths(name).Select(rel => Path.Combine(_root, rel)).FirstOrDefault(File.Exists);

    public byte[]? ReadFile(string name) => Resolve(name) is { } path ? File.ReadAllBytes(path) : null;

    public void Dispose() { }
}
