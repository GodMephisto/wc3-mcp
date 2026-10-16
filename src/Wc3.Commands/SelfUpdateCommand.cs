// src/Wc3.Commands/SelfUpdateCommand.cs
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;

namespace Wc3.Commands;

/// <summary>The outcome of a self-update file swap. FilesCopied counts every file written into the
/// target folder, FilesRenamed counts the ones whose previous version had to be moved aside first
/// because a running process held it open.</summary>
public sealed record UpdateApplyResult(int FilesCopied, int FilesRenamed);

/// <summary>Whether a newer GitHub release exists for the running build.</summary>
public sealed record UpdateCheckResult(
    bool Ok,
    bool UpToDate,
    string CurrentVersion,
    string LatestVersion,
    string? ReleaseUrl,
    string? AssetName,
    string Message);

/// <summary>The outcome of applying a release.</summary>
public sealed record UpdateResult(
    bool Ok,
    string Message,
    string? Version,
    int FilesCopied,
    int FilesRenamed);

/// <summary>
/// Replaces the running program's files from a GitHub release without needing the program to be
/// closed first. On Windows an open <c>.exe</c> or <c>.dll</c> cannot be overwritten, but it can
/// be renamed, so a locked target is moved to a short <c>*.old</c> name and the new copy takes its
/// place. The <c>*.old</c> files are cleaned up on the next run, by which time the process that
/// held them has exited.
///
/// Everything here works on plain paths and bytes, so it stays in Wc3.Commands with no reference
/// to the CLI, the Studio or the MCP server. The front-ends pass the folder to update and the
/// repository and asset to read from.
/// </summary>
public static class SelfUpdateCommand
{
    /// <summary>The release repository for the CLI and the Studio.</summary>
    public const string DefaultRepo = "GodMephisto/wc3ctl";

    private static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        // The GitHub API rejects a request with no User-Agent.
        client.DefaultRequestHeaders.UserAgent.ParseAdd("wc3ctl-updater");
        return client;
    }

    /// <summary>
    /// Deletes every <c>*.old</c> file under <paramref name="directory"/> and its children. A file
    /// still held open by the process that is being replaced is skipped silently, because that is
    /// the expected case, not a failure. Returns how many files were removed.
    /// </summary>
    public static int CleanupOldFiles(string directory)
    {
        if (!Directory.Exists(directory)) return 0;
        int removed = 0;
        IEnumerable<string> candidates;
        try
        {
            candidates = Directory.EnumerateFiles(directory, "*.old", SearchOption.AllDirectories);
        }
        catch (IOException) { return 0; }
        catch (UnauthorizedAccessException) { return 0; }

        foreach (var file in candidates)
        {
            // '*.old' also matches extensions that merely begin with "old" on Windows, so the
            // suffix is confirmed here rather than trusted to the search pattern.
            if (!Path.GetFileName(file).EndsWith(".old", StringComparison.OrdinalIgnoreCase))
                continue;
            try
            {
                File.Delete(file);
                removed++;
            }
            catch (IOException) { /* still open, try again next run */ }
            catch (UnauthorizedAccessException) { /* still open, try again next run */ }
        }
        return removed;
    }

    /// <summary>
    /// Copies every file from <paramref name="unpackedDir"/> into <paramref name="targetDir"/>,
    /// creating folders as needed. A target file that cannot be overwritten because a running
    /// process holds it is first renamed to <c>targetFile + "." + a short guid + ".old"</c>, then
    /// the new copy is written in its place.
    ///
    /// <paramref name="preserveDirs"/> names top level folders under
    /// <paramref name="targetDir"/> whose existing contents are left untouched, for the CLI folder
    /// that carries separately installed <c>studio</c> and <c>schemas</c> subtrees.
    /// </summary>
    public static UpdateApplyResult ReplaceDirectoryLockSafe(
        string unpackedDir, string targetDir, IReadOnlySet<string>? preserveDirs = null)
    {
        CleanupOldFiles(targetDir);
        Directory.CreateDirectory(targetDir);

        int copied = 0, renamed = 0;
        foreach (var source in Directory.EnumerateFiles(unpackedDir, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(unpackedDir, source);
            if (preserveDirs is not null && preserveDirs.Count > 0)
            {
                int slash = relative.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                string first = slash < 0 ? relative : relative[..slash];
                if (preserveDirs.Contains(first)) continue;
            }

            string destination = Path.Combine(targetDir, relative);
            if (Path.GetDirectoryName(destination) is { Length: > 0 } parent)
                Directory.CreateDirectory(parent);

            if (TryCopyLockSafe(source, destination, out bool didRename))
            {
                copied++;
                if (didRename) renamed++;
            }
        }
        return new UpdateApplyResult(copied, renamed);
    }

    private static bool TryCopyLockSafe(string source, string destination, out bool renamed)
    {
        renamed = false;
        try
        {
            File.Copy(source, destination, overwrite: true);
            return true;
        }
        catch (IOException) { /* locked, rename it aside below */ }
        catch (UnauthorizedAccessException) { /* locked, rename it aside below */ }

        if (!File.Exists(destination))
            return false;

        string aside = destination + "." + Guid.NewGuid().ToString("N")[..6] + ".old";
        try
        {
            File.Move(destination, aside, overwrite: true);
            renamed = true;
        }
        catch (IOException) { return false; }
        catch (UnauthorizedAccessException) { return false; }

        try
        {
            File.Copy(source, destination, overwrite: true);
            return true;
        }
        catch (IOException) { return false; }
        catch (UnauthorizedAccessException) { return false; }
    }

    /// <summary>Asks GitHub for the latest release and reports whether it is newer than
    /// <paramref name="currentVersion"/>. Reads no local files and writes nothing.</summary>
    public static async Task<UpdateCheckResult> CheckAsync(
        string repo, string currentVersion, string assetPrefix = "", CancellationToken ct = default)
    {
        using var document = await FetchLatestReleaseAsync(repo, ct).ConfigureAwait(false);
        var root = document.RootElement;
        string tag = GetString(root, "tag_name");
        string latest = Normalize(tag);
        string? url = GetStringOrNull(root, "html_url");
        string? asset = FindAsset(root, assetPrefix)?.Name;

        bool upToDate = Normalize(currentVersion) == latest;
        return new UpdateCheckResult(
            true, upToDate, currentVersion, latest, url, asset,
            upToDate
                ? $"{repo} is up to date ({currentVersion})."
                : $"A newer release is available, {latest} (you are on {currentVersion}).");
    }

    /// <summary>
    /// Fetches the latest release, verifies the asset against its <c>.sha256</c> sibling, unpacks
    /// it and applies it with <see cref="ReplaceDirectoryLockSafe"/>. Already current means nothing
    /// is downloaded.
    /// </summary>
    public static async Task<UpdateResult> UpdateFromReleaseAsync(
        string repo, string currentVersion, string targetDir,
        string assetPrefix = "", IReadOnlySet<string>? preserveDirs = null, CancellationToken ct = default)
    {
        using var document = await FetchLatestReleaseAsync(repo, ct).ConfigureAwait(false);
        var root = document.RootElement;
        string tag = GetString(root, "tag_name");
        string latest = Normalize(tag);

        if (Normalize(currentVersion) == latest)
            return new UpdateResult(true, $"Already on the latest release ({latest}).", currentVersion, 0, 0);

        if (FindAsset(root, assetPrefix) is not { } asset)
            return new UpdateResult(false, $"Release {tag} carries no {assetPrefix}*-win-x64.zip asset.", null, 0, 0);

        var sum = FindAssetByName(root, asset.Name + ".sha256");
        string work = Path.Combine(Path.GetTempPath(), "wc3ctl-update-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(work);
        try
        {
            string zipPath = Path.Combine(work, asset.Name);
            await DownloadAsync(asset.Url, zipPath, ct).ConfigureAwait(false);

            if (sum is not null)
            {
                string sumPath = zipPath + ".sha256";
                await DownloadAsync(sum.Value.Url, sumPath, ct).ConfigureAwait(false);
                if (!VerifySha256(zipPath, sumPath))
                    return new UpdateResult(false,
                        "The download did not match its checksum, so it was discarded.", null, 0, 0);
            }

            string unpacked = Path.Combine(work, "unpacked");
            ZipFile.ExtractToDirectory(zipPath, unpacked);
            var applied = ReplaceDirectoryLockSafe(unpacked, targetDir, preserveDirs);
            return new UpdateResult(true,
                $"Updated to {latest}. {applied.FilesCopied} file(s) written"
                + (applied.FilesRenamed > 0 ? $", {applied.FilesRenamed} replaced in place while locked." : "."),
                latest, applied.FilesCopied, applied.FilesRenamed);
        }
        finally
        {
            try { Directory.Delete(work, recursive: true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    private static async Task<JsonDocument> FetchLatestReleaseAsync(string repo, CancellationToken ct)
    {
        string url = $"https://api.github.com/repos/{repo}/releases/latest";
        using var response = await Http.GetAsync(url, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        return await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);
    }

    private static async Task DownloadAsync(string url, string path, CancellationToken ct)
    {
        using var response = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        await using var source = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        await using var file = File.Create(path);
        await source.CopyToAsync(file, ct).ConfigureAwait(false);
    }

    private static bool VerifySha256(string zipPath, string sumPath)
    {
        string text = File.ReadAllText(sumPath).Trim();
        string expected = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(zipPath);
        string actual = Convert.ToHexString(sha.ComputeHash(stream));
        return string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase);
    }

    private static (string Name, string Url)? FindAsset(JsonElement root, string prefix)
    {
        if (!root.TryGetProperty("assets", out var assets) || assets.ValueKind != JsonValueKind.Array)
            return null;
        foreach (var asset in assets.EnumerateArray())
        {
            string name = GetString(asset, "name");
            if (!name.EndsWith("-win-x64.zip", StringComparison.OrdinalIgnoreCase)) continue;
            if (prefix.Length > 0 && !name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
            string? url = GetStringOrNull(asset, "browser_download_url");
            if (url is not null) return (name, url);
        }
        return null;
    }

    private static (string Name, string Url)? FindAssetByName(JsonElement root, string name)
    {
        if (!root.TryGetProperty("assets", out var assets) || assets.ValueKind != JsonValueKind.Array)
            return null;
        foreach (var asset in assets.EnumerateArray())
        {
            if (!string.Equals(GetString(asset, "name"), name, StringComparison.OrdinalIgnoreCase)) continue;
            string? url = GetStringOrNull(asset, "browser_download_url");
            if (url is not null) return (name, url);
        }
        return null;
    }

    private static string GetString(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) ? value.GetString() ?? "" : "";

    private static string? GetStringOrNull(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() : null;

    /// <summary>A version with any leading "v" and whitespace removed, for comparison.</summary>
    private static string Normalize(string version) =>
        version.Trim().TrimStart('v', 'V').Trim();
}
