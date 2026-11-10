// src/Wc3.Commands/ArtifactWriter.cs
using System.Security.Cryptography;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// An expected refusal from a write, carrying a stable code and an optional next step. The code is
/// the contract, so a front end may add its own formatting without changing it. This type lives in
/// the shared layer because the writer must not depend on any one front end (SAF-14).
/// </summary>
public sealed class ArtifactWriteException : Exception
{
    public ArtifactWriteException(string code, string message, string? next = null) : base(message)
    {
        Code = code;
        Next = next;
    }

    /// <summary>The stable refusal code, for example OUTPUT_EXISTS or STALE_INPUT.</summary>
    public string Code { get; }

    /// <summary>What the caller can do next, or null when there is nothing useful to say.</summary>
    public string? Next { get; }
}

/// <summary>What one write produced, including the digests a caller can verify (SAF-06).</summary>
public sealed record ArtifactWriteResult(
    string SavedTo,
    string? InputSha256,
    string OutputSha256,
    int FilesRewritten,
    long BytesWritten);

/// <summary>
/// Granted only when an existing destination would actually be replaced, so a fresh output needs
/// no grant. Wc3.Mcp supplies the user's permission check, the CLI and the Studio supply nothing,
/// and a refusal is raised by throwing rather than by returning a flag.
/// </summary>
public delegate void OverwriteGuard(string action, string file, string effect);

/// <summary>
/// The one path every map write should take, in the shared layer so the CLI, the Studio and the MCP
/// server cannot diverge. It resolves the output, refuses an output that is the input or inside a
/// folder map, refuses an existing output unless overwrite is set and the optional guard allows it,
/// compares an expected input digest, writes through a temporary file in the destination folder and
/// renames it into place, then reports both digests and what changed.
///
/// The guard is the only front-end-specific part. It is called after the free checks and before
/// anything is written, so a refusal leaves the destination untouched.
/// </summary>
public static class ArtifactWriter
{
    /// <summary>
    /// The output path a write will use. With no out_path this is a sibling <c>.edited</c> copy so
    /// the input is never the destination.
    /// </summary>
    public static string Resolve(string map, string? outPath)
    {
        string full;
        if (string.IsNullOrWhiteSpace(outPath))
        {
            string dir = Path.GetDirectoryName(Path.GetFullPath(map)) ?? ".";
            full = Path.TrimEndingDirectorySeparator(Path.Combine(dir,
                Path.GetFileNameWithoutExtension(map) + ".edited" + Path.GetExtension(map)));
        }
        else
        {
            full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(outPath));
        }

        string input = Path.TrimEndingDirectorySeparator(Path.GetFullPath(map));
        if (string.Equals(full, input, StringComparison.OrdinalIgnoreCase))
            throw new ArtifactWriteException("OUT_PATH_IS_INPUT",
                "out_path must not be the input map file itself",
                "omit out_path to write a sibling .edited copy, or name a different file");
        if (Directory.Exists(input) && full.StartsWith(input + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new ArtifactWriteException("OUT_PATH_INSIDE_INPUT",
                "out_path must not be inside the input map folder",
                "write beside the folder, not inside it");
        return full;
    }

    /// <summary>The lower-case hex SHA-256 of a file's bytes, the digest shape every result reports.</summary>
    public static string Sha256File(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    /// <summary>
    /// Save <paramref name="doc"/> to the resolved destination and report what happened.
    /// <paramref name="loadedInputSha"/> is the digest the document was loaded from, so a file
    /// changed underneath it is refused rather than saved. <paramref name="guard"/> is called only
    /// when an existing destination would be replaced and <paramref name="overwrite"/> is set.
    /// </summary>
    public static ArtifactWriteResult Save(
        MapDocument doc,
        string map,
        string? outPath,
        bool overwrite = false,
        string? expectInputSha256 = null,
        string? loadedInputSha = null,
        OverwriteGuard? guard = null)
    {
        string input = Path.TrimEndingDirectorySeparator(Path.GetFullPath(map));
        string full = Resolve(map, outPath);
        bool fileInput = File.Exists(input);
        string? currentSha = fileInput ? Sha256File(input) : null;

        // The digest the caller read is the loaded snapshot. If the file changed underneath it,
        // refuse rather than saving content that no longer matches the file it claims to be.
        if (fileInput && loadedInputSha is not null &&
            !string.Equals(loadedInputSha, currentSha, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArtifactWriteException("STALE_INPUT", "the input map changed after it was loaded",
                "read the map again and chain on the current output");
        }

        string? inputSha = fileInput ? (loadedInputSha ?? currentSha) : null;

        if (!string.IsNullOrWhiteSpace(expectInputSha256) &&
            !string.Equals(expectInputSha256.Trim(), inputSha, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArtifactWriteException("STALE_INPUT", "the input map is not the one the caller read",
                "read the map again and chain on the current output");
        }

        bool replacing = File.Exists(full) || Directory.Exists(full);
        if (replacing && !overwrite)
        {
            throw new ArtifactWriteException("OUTPUT_EXISTS", "the output already exists",
                "pass overwrite true to replace it, or name a new file");
        }

        // Overwrite is the one switch the writer enforces, and it is the user's to grant in their
        // permission file. It is required only when an existing destination would be replaced,
        // because that is the only write that destroys something. A fresh out_path destroys
        // nothing, and every write tool names one, so gating that path would refuse work the
        // user never had to consider. Nothing is written before this check, and the guard is the
        // only place a front end can refuse, so an exception it throws leaves the destination as
        // it was.
        if (replacing)
            guard?.Invoke("overwrite", full, "replaced");

        string dir = Path.GetDirectoryName(full) is { Length: > 0 } d ? d : ".";
        Directory.CreateDirectory(dir);

        if (!fileInput)
        {
            // A folder map cannot be renamed into place, so it is written directly. The unpack
            // stages and swaps its own directory, so a lost race surfaces here as an IOException.
            try
            {
                doc.Save(full);
            }
            catch (IOException) when (!overwrite && Directory.Exists(full))
            {
                throw OutputExists();
            }
        }
        else
        {
            string temp = Path.Combine(dir, "." + Path.GetFileName(full) + "." + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                doc.Save(temp);
                Commit(temp, full, overwrite);
            }
            finally
            {
                if (File.Exists(temp)) File.Delete(temp);
            }
        }

        string outputSha = fileInput ? Sha256File(full) : "";
        long bytes = fileInput
            ? new FileInfo(full).Length
            : Directory.EnumerateFiles(full, "*", SearchOption.AllDirectories).Sum(f => new FileInfo(f).Length);

        return new ArtifactWriteResult(
            full,
            inputSha,
            outputSha,
            doc.Files.Count(f => f.IsDirty),
            bytes);
    }

    /// <summary>Move the staged file into place, or report an existing destination.
    /// A rename without replace is the atomic check, so two writers that both passed the
    /// existence test cannot both publish, and the loser gets the same code as a caller
    /// that simply named a file that was already there.</summary>
    public static void Commit(string temp, string full, bool overwrite)
    {
        try
        {
            File.Move(temp, full, overwrite);
        }
        catch (IOException) when (!overwrite && File.Exists(full))
        {
            throw OutputExists();
        }
    }

    private static ArtifactWriteException OutputExists() =>
        new("OUTPUT_EXISTS", "the output already exists",
            "pass overwrite true to replace it, or name a new file");
}
