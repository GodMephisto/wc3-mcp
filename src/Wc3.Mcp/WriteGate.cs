// src/Wc3.Mcp/WriteGate.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Mcp;

/// <summary>An expected failure with a stable code and an optional next step (ERR-02).</summary>
internal sealed class ToolError : Exception
{
    public ToolError(string code, string message, string? next = null) : base(message)
    {
        Code = code;
        Next = next;
    }

    public string Code { get; }
    public string? Next { get; }
}

/// <summary>What one write produced, including the digests a caller can verify (SAF-06).</summary>
internal sealed record WriteOutcome(
    string SavedTo,
    string? InputSha256,
    string OutputSha256,
    int FilesRewritten,
    long BytesWritten);

/// <summary>One file a write published, with the digest and size a caller can check (SAF-15).</summary>
internal sealed record WrittenFile(string Path, string Sha256, long Bytes);

/// <summary>
/// A write that has not happened yet. The path is where the file must end up and the callback
/// produces the bytes at the staging path it is handed, so the gate owns the rename.
/// </summary>
internal sealed record StagedWrite(string Path, Action<string> Write);

/// <summary>
/// What a file level write produced. It carries the same five fields the map gate reports plus
/// the full list of files published, so a tool that writes several files names all of them and
/// keeps <see cref="SavedTo"/> as the primary (SAF-15).
/// </summary>
internal sealed record FileWriteOutcome(
    string SavedTo,
    string? InputSha256,
    string OutputSha256,
    int FilesRewritten,
    long BytesWritten,
    IReadOnlyList<WrittenFile> Files);

/// <summary>
/// The MCP front end of the shared writer (see Wc3.Commands.ArtifactWriter), kept as a thin wrapper
/// so both the CLI and the MCP server take one write path and every existing call site and result
/// shape stays unchanged (SAF-14).
///
/// The only MCP-specific part is the guard, which is the user's permission check. The shared writer
/// calls it when an existing destination would be replaced, and this wrapper maps the writer's
/// refusals onto <see cref="ToolError"/> without changing their codes.
/// </summary>
internal static class WriteGate
{
    internal static string Resolve(string map, string? outPath) => ArtifactWriter.Resolve(map, outPath);

    internal static string Sha256File(string path) => ArtifactWriter.Sha256File(path);

    internal static WriteOutcome Save(
        MapDocument doc,
        string map,
        string? outPath,
        bool overwrite = false,
        string? expectInputSha256 = null,
        string? loadedInputSha = null)
    {
        // Resolve first, so the check runs against the file the writer would really touch. The
        // resolve step is inside Map, so an OUT_PATH_IS_INPUT refusal keeps its ToolError shape.
        RefuseReparsePoint(Map(() => Resolve(map, outPath)));

        var result = Map(() => ArtifactWriter.Save(
            doc, map, outPath, overwrite, expectInputSha256, loadedInputSha, Permissions.Current.Require));

        return new WriteOutcome(
            result.SavedTo,
            result.InputSha256,
            result.OutputSha256,
            result.FilesRewritten,
            result.BytesWritten);
    }

    /// <summary>
    /// The file level gate. Every writing tool that produces a file which is not the map archive
    /// comes through here, so a report, a PNG, an export or a rebuilt model takes the same
    /// refusals, the same staging and the same digests the map path takes.
    ///
    /// <paramref name="inputs"/> is every file the tool read to produce the output. An output
    /// equal to any of them is refused, so a write can never land on its own input. The first
    /// input that exists is the one whose digest is reported and compared against
    /// <paramref name="expectInputSha256"/>.
    /// </summary>
    internal static FileWriteOutcome WriteFile(
        string outPath,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256,
        Action<string> write)
    {
        string? inputSha = PrepareTarget(outPath, inputs, overwrite, expectInputSha256, out string full);
        if (Directory.Exists(full))
            throw new ToolError("OUTPUT_IS_FOLDER", "a folder occupies the output path",
                "name a file path, or remove the folder");
        string temp = TempPathFor(full);
        try
        {
            write(temp);
            Commit(temp, full, overwrite);
        }
        finally
        {
            Discard(temp);
        }
        return Outcome(full, inputSha, new[] { full });
    }

    /// <summary>The byte array form of <see cref="WriteFile"/>, for a tool that already holds the output.</summary>
    internal static FileWriteOutcome WriteBytes(
        string outPath,
        byte[] bytes,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256)
        => WriteFile(outPath, inputs, overwrite, expectInputSha256, temp => File.WriteAllBytes(temp, bytes));

    /// <summary>The text form of <see cref="WriteFile"/>, written as UTF-8 without a byte order mark.</summary>
    internal static FileWriteOutcome WriteText(
        string outPath,
        string text,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256)
        => WriteFile(outPath, inputs, overwrite, expectInputSha256,
            temp => File.WriteAllText(temp, text, new System.Text.UTF8Encoding(false)));

    /// <summary>
    /// Several files published as one step from content the gate itself writes. Every destination
    /// is checked before anything is staged, and every file is staged before any of them is
    /// renamed, so a failure while the content is being produced leaves no destination touched.
    /// The first path is the primary.
    /// </summary>
    internal static FileWriteOutcome WriteFiles(
        IReadOnlyList<StagedWrite> writes,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256)
    {
        if (writes.Count == 0)
            throw new ToolError("NO_OUTPUT", "the write named no output file", "name at least one file to write");

        // Check every destination first, so a refusal happens before any content is produced.
        string? inputSha = null;
        var fulls = new List<string>(writes.Count);
        foreach (var w in writes)
        {
            string? sha = PrepareTarget(w.Path, inputs, overwrite, expectInputSha256, out string full);
            if (Directory.Exists(full))
                throw new ToolError("OUTPUT_IS_FOLDER", "a folder occupies the output path",
                    "name a file path, or remove the folder");
            if (inputSha is null) inputSha = sha;
            fulls.Add(full);
        }

        // Then stage every file. A throw here removes every staged file and touches no destination.
        var temps = new List<string>(writes.Count);
        try
        {
            for (int i = 0; i < writes.Count; i++)
            {
                string temp = TempPathFor(fulls[i]);
                temps.Add(temp);
                Directory.CreateDirectory(Path.GetDirectoryName(temp)!);
                writes[i].Write(temp);
            }
            for (int i = 0; i < writes.Count; i++) Commit(temps[i], fulls[i], overwrite);
        }
        catch
        {
            foreach (string t in temps) Discard(t);
            throw;
        }

        return Outcome(fulls[0], inputSha, fulls);
    }

    /// <summary>
    /// Publish files a command already wrote into a staging folder. Every destination is checked
    /// before any file is moved, so a refusal leaves the staging folder intact and nothing at the
    /// destination touched, and a publish is a rename rather than a second write (SAF-15).
    /// </summary>
    internal static FileWriteOutcome Publish(
        IReadOnlyList<(string Source, string Target)> files,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256)
    {
        if (files.Count == 0)
            throw new ToolError("NO_OUTPUT", "the write produced no file", "check the input and try again");

        string? inputSha = null;
        var targets = new List<(string Source, string Full)>(files.Count);
        foreach (var (source, target) in files)
        {
            string? sha = PrepareTarget(target, inputs, overwrite, expectInputSha256, out string full);
            if (Directory.Exists(full))
                throw new ToolError("OUTPUT_IS_FOLDER", "a folder occupies the output path",
                    "name a file path, or remove the folder");
            if (inputSha is null) inputSha = sha;
            targets.Add((source, full));
        }

        foreach (var (source, full) in targets)
        {
            if (!File.Exists(source)) continue;
            Commit(source, full, overwrite);
        }

        return Outcome(targets[0].Full, inputSha, targets.Select(t => t.Full).ToList());
    }

    /// <summary>
    /// A folder of output files published as one step. The content is produced under a temporary
    /// folder beside the destination and the folder is renamed into place, so a run that fails
    /// halfway leaves no half written folder. An existing destination is refused without overwrite.
    /// </summary>
    internal static FileWriteOutcome WriteFolder(
        string outDir,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256,
        Action<string> write)
    {
        string? inputSha = PrepareTarget(outDir, inputs, overwrite, expectInputSha256, out string full);
        string parent = Path.GetDirectoryName(full) is { Length: > 0 } p ? p : ".";
        string temp = Path.Combine(parent, "." + Path.GetFileName(full) + "." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            // The folder is not created here. A tool that builds a package refuses a folder that
            // already exists, so the writer creates it and the gate creates it only when the
            // writer produced nothing.
            write(temp);
            Directory.CreateDirectory(temp);
            if (Directory.Exists(full)) Directory.Delete(full, recursive: true);
            Directory.Move(temp, full);
        }
        finally
        {
            if (Directory.Exists(temp))
                try { Directory.Delete(temp, recursive: true); } catch { /* best effort */ }
        }

        var files = Directory.EnumerateFiles(full, "*", SearchOption.AllDirectories)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .Select(f => new WrittenFile(f, Sha256File(f), new FileInfo(f).Length))
            .ToList();
        return new FileWriteOutcome(full, inputSha,
            files.Count == 1 ? files[0].Sha256 : "",
            files.Count, files.Sum(f => f.Bytes), files);
    }

    /// <summary>
    /// The refusals every file level write takes, in the order the caller expects them. Returns
    /// the input digest the write reports, and the resolved destination through
    /// <paramref name="full"/>.
    /// </summary>
    private static string? PrepareTarget(
        string path,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256,
        out string full)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ToolError("NO_OUTPUT", "the output path is empty", "name a file to write");

        full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));

        foreach (string input in inputs)
        {
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (string.Equals(full, Path.GetFullPath(input), StringComparison.OrdinalIgnoreCase))
                throw new ToolError("OUT_PATH_IS_INPUT",
                    "out_path must not be the input file ''" + Path.GetFileName(full) + "''",
                    "omit out_path to write a sibling copy, or name a different file");
        }

        RefuseReparsePoint(full);

        string? inputSha = null;
        foreach (string input in inputs)
        {
            if (string.IsNullOrWhiteSpace(input) || !File.Exists(input)) continue;
            inputSha = Sha256File(input);
            break;
        }

        if (!string.IsNullOrWhiteSpace(expectInputSha256) &&
            !string.Equals(expectInputSha256.Trim(), inputSha, StringComparison.OrdinalIgnoreCase))
            throw new ToolError("STALE_INPUT", "the input is not the one the caller read",
                "read the input again and chain on the current output");

        bool replacing = File.Exists(full) || Directory.Exists(full);
        if (replacing && !overwrite)
            throw new ToolError("OUTPUT_EXISTS", "the output already exists",
                "pass overwrite true to replace it, or name a new file");
        if (replacing)
            Permissions.Current.Require("overwrite", full, "replaced");

        string dir = Path.GetDirectoryName(full) is { Length: > 0 } d ? d : ".";
        Directory.CreateDirectory(dir);
        return inputSha;
    }

    /// <summary>A sibling staging path, hidden and unique, so two writers cannot share a stage.
    /// The input's extension is kept, because a writer may choose its output format from it.</summary>
    private static string TempPathFor(string full) =>
        Path.Combine(Path.GetDirectoryName(full) is { Length: > 0 } d ? d : ".",
            "." + Path.GetFileNameWithoutExtension(full) + "." + Guid.NewGuid().ToString("N")
                + ".tmp" + Path.GetExtension(full));

    private static void Discard(string temp)
    {
        if (!File.Exists(temp)) return;
        try { File.Delete(temp); } catch { /* best effort */ }
    }

    /// <summary>The one result shape a file level write reports, digests read back from disk.</summary>
    private static FileWriteOutcome Outcome(string primary, string? inputSha, IReadOnlyList<string> fulls)
    {
        var files = fulls.Select(f => new WrittenFile(f, Sha256File(f), new FileInfo(f).Length)).ToList();
        return new FileWriteOutcome(primary, inputSha,
            files.Count == 1 ? files[0].Sha256 : "",
            files.Count, files.Sum(f => f.Bytes), files);
    }

    internal static void Commit(string temp, string full, bool overwrite)
    {
        RefuseReparsePoint(full);
        Map(() => ArtifactWriter.Commit(temp, full, overwrite));
    }

    /// <summary>
    /// Refuse a destination whose path crosses a reparse point, a symbolic link or a junction, so a
    /// write cannot follow a link out of the folder the caller named. Every existing component is
    /// checked, the destination itself included, and the refusal names the link it found.
    /// </summary>
    internal static void RefuseReparsePoint(string full)
    {
        foreach (var part in Components(full))
        {
            if (!File.Exists(part) && !Directory.Exists(part)) continue;
            FileAttributes attributes;
            try { attributes = File.GetAttributes(part); }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { continue; }
            if ((attributes & FileAttributes.ReparsePoint) != 0)
                throw new ToolError("OUTPUT_IS_LINK",
                    "the output path crosses a symbolic link at '" + part + "'",
                    "name an output path whose folders are real directories, or write beside the link");
        }
    }

    /// <summary>The full path and each of its parents, outermost first, so a link anywhere is seen.</summary>
    private static IEnumerable<string> Components(string full)
    {
        var parts = new List<string>();
        string? current = Path.GetFullPath(full);
        while (!string.IsNullOrEmpty(current))
        {
            parts.Add(current);
            string? parent = Path.GetDirectoryName(current);
            if (parent is null || string.Equals(parent, current, StringComparison.OrdinalIgnoreCase)) break;
            current = parent;
        }
        parts.Reverse();
        return parts;
    }

    /// <summary>Translate a shared refusal into the MCP error shape. The code is carried through
    /// unchanged, including PERMISSION_DENIED from the guard.</summary>
    private static T Map<T>(Func<T> body)
    {
        try
        {
            return body();
        }
        catch (ArtifactWriteException ex)
        {
            throw new ToolError(ex.Code, ex.Message, ex.Next);
        }
    }

    private static void Map(Action body)
    {
        try
        {
            body();
        }
        catch (ArtifactWriteException ex)
        {
            throw new ToolError(ex.Code, ex.Message, ex.Next);
        }
    }
}
