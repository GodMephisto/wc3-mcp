// src/Wc3.Commands/ModelExportObjCommand.cs
using System.Security.Cryptography;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>What an export published, and anything noted after the publish had already succeeded.
/// <see cref="SavedTo"/> is the .obj, <see cref="Mtl"/> the .mtl beside it, and
/// <see cref="Textures"/> the names of the textures written in the same folder.</summary>
public sealed record ModelExportObjWriteResult(
    string SavedTo,
    string Mtl,
    IReadOnlyList<string> Textures,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Exports a map-imported model to Wavefront OBJ. <see cref="Execute"/> does the work of parsing
/// the model through the shared parser and converting every map-resolvable texture to PNG, and
/// <see cref="Write"/> publishes the result as a file set that is either wholly there or wholly
/// absent. It exists as its own command so the CLI and MCP surfaces have a stable verb for the
/// operation rather than reaching into the general convert command.
/// </summary>
public static class ModelExportObjCommand
{
    /// <summary>The infix a staging folder carries, <c>&lt;target&gt;.saving-&lt;8 hex&gt;</c>, so
    /// an export under construction is visible beside its target and never inside it.</summary>
    public const string StagingInfix = ".saving-";

    /// <summary>The infix a replaced artifact is parked under for the length of one publish.</summary>
    private const string ReplacedInfix = ".replaced-";

    /// <summary>Parses the model at an internal map path and returns its OBJ text, its MTL text
    /// and the PNG bytes of every texture the map can supply, keyed by the file name the MTL
    /// references. The .mdx/.mdl path swap is handled by the shared model finder.</summary>
    public static ModelExport Execute(MapDocument doc, string modelInternalPath, string? mtlName = null) =>
        ConvertCommand.ExportModelToObj(doc, modelInternalPath, mtlName);

    /// <summary>
    /// Exports the model to <paramref name="outPath"/> and publishes the whole file set, the .obj,
    /// the .mtl and every converted texture, into the target's own folder.
    ///
    /// A raw write is not safe here, because a failure midway leaves an .obj that renders against a
    /// missing .mtl or a missing texture, which looks like a finished export. So every artifact is
    /// first written into a staging folder named <c>&lt;target&gt;.saving-&lt;8 hex&gt;</c> beside
    /// the target, all of them are verified to exist and hold bytes there, and only then are they
    /// moved into place. A failure removes the staging folder and leaves the previous state of every
    /// artifact as it was.
    ///
    /// A target file set cannot be swapped as one inode the way a folder can, so the publish is one
    /// move per artifact under a rollback. If a move throws, every move already made is undone and
    /// every parked original is restored, so the guarantee is still all or nothing. An existing
    /// .obj, .mtl or texture is refused unless <paramref name="overwrite"/> is true, in which case it
    /// is parked, swapped, and deleted only after the swap succeeds.
    /// </summary>
    /// <exception cref="IOException">An artifact already exists and <paramref name="overwrite"/> is
    /// false, the target is a folder, or the publish failed. Nothing partial is left behind.</exception>
    public static ModelExportObjWriteResult Write(
        MapDocument doc, string modelInternalPath, string outPath, bool overwrite = false)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelInternalPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);

        string full = Path.GetFullPath(outPath);
        string? parent = Path.GetDirectoryName(full);
        string objName = Path.GetFileName(full);
        string stem = Path.GetFileNameWithoutExtension(full);
        if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(objName) || string.IsNullOrEmpty(stem))
            throw new ArgumentException("outPath must name an .obj file, not a drive or directory root",
                nameof(outPath));

        string mtlName = stem + ".mtl";

        // Refusals happen before anything is staged or written, so an existing export is never
        // touched on a path that cannot publish. A folder where a file belongs is refused for the
        // same reason, a move onto it would fail halfway.
        foreach (string name in new[] { objName, mtlName })
        {
            string path = Path.Combine(parent, name);
            if (Directory.Exists(path))
                throw new IOException($"'{path}' is a folder, not a file, and an export will not replace it");
            if (File.Exists(path) && !overwrite)
                throw new IOException(Refusal(path));
        }

        // The mtl name is threaded through, so the OBJ's mtllib line names the .mtl this export
        // actually publishes beside it rather than the model's own name.
        var export = Execute(doc, modelInternalPath, stem);

        var artifacts = new List<string> { objName, mtlName };
        foreach (string name in export.Textures.Keys)
        {
            // A texture that shares a name with another artifact would be written once and one of
            // the two would vanish, so the collision is refused rather than silently resolved.
            if (artifacts.Contains(name, StringComparer.OrdinalIgnoreCase))
                throw new InvalidDataException(
                    $"the export would write '{name}' twice, its texture name collides with another artifact");
            artifacts.Add(name);
        }

        // Every artifact lands in the target's folder, so anything that is not a bare file name
        // would write outside it. The exporter builds these names, this is the boundary check.
        foreach (string name in artifacts)
            if (name.Length == 0 || !string.Equals(Path.GetFileName(name), name, StringComparison.Ordinal))
                throw new InvalidDataException(
                    $"the export names '{name}', which is not a bare file name in the target folder");

        if (!overwrite)
            foreach (string name in artifacts)
            {
                string existing = Path.Combine(parent, name);
                if (File.Exists(existing))
                    throw new IOException(Refusal(existing));
            }

        string staging = Path.Combine(parent, objName + StagingInfix + Hex8());
        var diagnostics = new List<string>();

        try
        {
            // Creating the parent can legitimately fail when a component of it is a file, and that
            // failure is total, so nothing is left in staging or at the target.
            Directory.CreateDirectory(parent);
            Directory.CreateDirectory(staging);

            File.WriteAllText(Path.Combine(staging, objName), export.Obj);
            File.WriteAllText(Path.Combine(staging, mtlName), export.Mtl);
            foreach (var (name, bytes) in export.Textures)
                File.WriteAllBytes(Path.Combine(staging, name), bytes);

            // Nothing reaches the target until every artifact is on disk in staging with bytes.
            Verify(staging, artifacts);
            Publish(staging, parent, artifacts, overwrite, diagnostics);

            // The staging folder is empty once every artifact has moved out. Removing it here keeps
            // the target folder clean, and a folder that will not delete is reported, not thrown.
            try
            {
                Directory.Delete(staging, recursive: true);
            }
            catch (Exception leftover) when (leftover is IOException or UnauthorizedAccessException)
            {
                diagnostics.Add(
                    $"the empty staging folder could not be removed and is kept at {staging}, "
                    + leftover.Message);
            }
        }
        catch
        {
            // The staging folder is the only place a partial export can live, so removing it is
            // what makes the failure total. An existing target was never opened unless the publish
            // had begun, and that path rolls itself back before it throws.
            TryDelete(staging);
            throw;
        }

        // The paths are reported from the target, after the publish, because the staging folder no
        // longer exists and a caller must not be handed a path that cannot be opened.
        var textures = export.Textures.Keys.ToList();
        return new ModelExportObjWriteResult(
            Path.Combine(parent, objName),
            Path.Combine(parent, mtlName),
            textures,
            diagnostics);
    }

    private static string Refusal(string path) =>
        $"'{path}' already exists. Pass overwrite to replace it, an export is never merged into an "
        + "existing set of files";

    /// <summary>
    /// Refuses to publish unless every artifact exists in the staging folder with bytes. The check
    /// is on the files a consumer will open, not on the object still held in memory.
    /// </summary>
    private static void Verify(string staging, IReadOnlyList<string> artifacts)
    {
        foreach (string name in artifacts)
        {
            string path = Path.Combine(staging, name);
            long bytes = File.Exists(path) ? new FileInfo(path).Length : -1;
            if (bytes <= 0)
                throw new InvalidDataException(
                    $"'{name}' holds no bytes at {path}, the export is incomplete");
        }
    }

    /// <summary>
    /// Moves the finished staging files into the target folder, which is the publish itself. An
    /// existing artifact is refused unless <paramref name="overwrite"/> is true, and a replaced one
    /// is moved aside and deleted only once every move has succeeded, so a failed publish restores
    /// it. Because one file set cannot be swapped in a single rename, every completed move is
    /// recorded and undone in reverse when a later one fails.
    /// </summary>
    private static void Publish(
        string staging, string parent, IReadOnlyList<string> artifacts, bool overwrite,
        List<string> diagnostics)
    {
        var placed = new List<(string Dest, string? Parked)>();
        string? pendingParked = null;
        string? pendingDest = null;

        try
        {
            foreach (string name in artifacts)
            {
                string staged = Path.Combine(staging, name);
                string dest = Path.Combine(parent, name);

                if (File.Exists(dest))
                {
                    // Checked again here, after the write, so a file that appeared while the export
                    // was being staged is refused rather than replaced.
                    if (!overwrite)
                        throw new IOException(
                            $"'{dest}' appeared while the export was being written. Pass overwrite to replace it");

                    pendingDest = dest;
                    pendingParked = dest + ReplacedInfix + Hex8();
                    File.Move(dest, pendingParked);
                }

                File.Move(staged, dest);
                placed.Add((dest, pendingParked));
                pendingParked = null;
                pendingDest = null;
            }
        }
        catch (Exception swapFailure)
        {
            var stranded = new List<string>();

            // A file parked for the artifact whose own move failed was never replaced, so it is
            // restored first, then every completed move is undone in reverse.
            if (pendingParked is not null && pendingDest is not null)
                Undo(pendingParked, pendingDest, stranded);

            for (int i = placed.Count - 1; i >= 0; i--)
            {
                var (dest, parked) = placed[i];
                try
                {
                    if (File.Exists(dest)) File.Delete(dest);
                }
                catch (Exception undoFailure)
                {
                    stranded.Add($"{dest} could not be removed, {undoFailure.Message}");
                }
                if (parked is not null)
                    Undo(parked, dest, stranded);
            }

            if (stranded.Count > 0)
                throw new IOException(
                    $"the export could not be published into '{parent}' and these previous files "
                    + $"could not be restored, {string.Join(", ", stranded)}", swapFailure);
            throw;
        }

        // The publish succeeded, so a parked file that will not delete is reported and not thrown.
        foreach (var (_, parked) in placed)
        {
            if (parked is null) continue;
            try
            {
                File.Delete(parked);
            }
            catch (Exception leftover) when (leftover is IOException or UnauthorizedAccessException)
            {
                diagnostics.Add(
                    $"the replaced file could not be removed and is kept at {parked}, {leftover.Message}");
            }
        }
    }

    /// <summary>Moves a parked original back to its own path, recording a failure rather than
    /// throwing, because the caller is already unwinding a failed publish.</summary>
    private static void Undo(string parked, string dest, List<string> stranded)
    {
        try
        {
            if (File.Exists(parked)) File.Move(parked, dest, overwrite: true);
        }
        catch (Exception restoreFailure)
        {
            stranded.Add($"{parked} could not be restored, {restoreFailure.Message}");
        }
    }

    private static void TryDelete(string folder)
    {
        try
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    /// <summary>Eight random hex characters, so two exports into one folder never pick the same
    /// staging folder even when they are concurrent.</summary>
    private static string Hex8() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
}
