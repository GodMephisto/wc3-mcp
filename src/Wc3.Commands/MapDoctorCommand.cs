// src/Wc3.Commands/MapDoctorCommand.cs
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

namespace Wc3.Commands;

/// <summary>What one run of Devo's Map Doctor returned.</summary>
/// <param name="Command">The doctor command that ran, such as check or fix.</param>
/// <param name="ExitCode">0 done, 1 failed, 2 wrong command line, as the doctor defines them.</param>
/// <param name="Result">The doctor's own --json output, passed through unchanged.</param>
/// <param name="Output">For fix, the repaired copy that was written. Null for read-only commands.</param>
/// <param name="Doctor">The doctor build that answered, as version and sha256.</param>
/// <param name="Stderr">The last lines of the doctor's error output, for a failed run.</param>
public sealed record MapDoctorResult(string Command, int ExitCode, JsonElement? Result,
    string? Output, string Doctor, string? Stderr);

/// <summary>
/// Runs Devo's Map Doctor (github.com/devoltzz/devos-map-doctor, MIT) as an external program and
/// returns its JSON. The doctor covers ground wc3ctl does not, the SLK and profile tables a 3.0
/// engine refuses, model defects that hang the game, the "Runs on Reforged?" check, and KK and
/// M16 ports, so it is wrapped rather than reimplemented. The pieces worth owning get ported to
/// this layer one at a time, driven by what the wrapper shows matters.
///
/// Who calls it. The MCP tools doctor_check, doctor_diag, doctor_compare and doctor_fix.
/// Where the binary lives. WC3_DOCTOR names DevosMapDoctor.exe, otherwise the newest folder under
/// %LOCALAPPDATA%\wc3ctl\doctor\ is used. Nothing is downloaded here, and the binary is never
/// committed.
/// Which builds run. Only a build whose sha256 is in <see cref="PinnedBuilds"/>, because the
/// wrapper executes it on the user's maps. WC3_DOCTOR_UNPINNED=1 lifts that for a newer release
/// being evaluated.
/// How fix stays off the original. The map is copied into an empty work folder, the doctor
/// repairs the copy there, and the result is moved to the requested output. An existing output
/// is never overwritten.
/// </summary>
public static class MapDoctorCommand
{
    /// <summary>Doctor releases this wrapper has been checked against, by sha256 of the exe.</summary>
    public static readonly IReadOnlyDictionary<string, string> PinnedBuilds =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["522f9bda060535d7589fa56db24eacae737924391e68b5c2d8f115f09e703462"] = "1.7.6",
        };

    public const string ExeName = "DevosMapDoctor.exe";

    /// <summary>Read-only doctor commands that take one map.</summary>
    public static readonly IReadOnlySet<string> ReadOnlyCommands =
        new HashSet<string>(StringComparer.Ordinal) { "check", "diag", "files", "card", "rawcodes", "recipes" };

    /// <summary>
    /// The fix steps and extras the doctor accepts, from its --help in 1.7.6. Anything else is
    /// refused here, so a caller cannot pass an option such as --translation that reads another
    /// file, or one a later release gives a different meaning.
    /// </summary>
    public static readonly IReadOnlySet<string> FixOptions = new HashSet<string>(StringComparer.Ordinal)
    {
        "unprotect", "fake-files", "listfile", "ids", "data", "script-back", "editor-files", "counts",
        "doodads", "gui-triggers", "script-objects", "safe-units",
        "models", "model-names", "portraits", "data-pointers", "kk-textures", "green-icons", "uabi",
        "preload", "single-player", "shrink", "strip-indent",
    };

    public static MapDoctorResult Check(string map, string? game = null)
    {
        var args = new List<string> { "check", RequireMap(map) };
        if (game is not null)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(game, @"^1\.\d{2}(\.\d+)?[a-z]?$"))
                throw new ArgumentException($"game must be a version such as 1.27 or 1.28.5, got '{game}'.");
            args.Add("--game=" + game);
        }
        return Run("check", args, null);
    }

    public static MapDoctorResult ReadOnly(string command, string map)
    {
        if (!ReadOnlyCommands.Contains(command))
            throw new ArgumentException($"'{command}' is not a read-only doctor command. "
                + $"Allowed: {string.Join(", ", ReadOnlyCommands)}.");
        return Run(command, new List<string> { command, RequireMap(map) }, null);
    }

    public static MapDoctorResult Compare(string map, string other) =>
        Run("compare", new List<string> { "compare", RequireMap(map), RequireMap(other) }, null);

    /// <summary>
    /// Repairs a copy of <paramref name="map"/> and writes it to <paramref name="output"/>, by
    /// default <c>&lt;map&gt;_doctor.&lt;ext&gt;</c> beside the input. With no options the doctor
    /// applies the steps its window ticks for this map. A name prefixed "no-" drops that step.
    /// </summary>
    public static MapDoctorResult Fix(string map, IEnumerable<string>? options = null, string? output = null)
    {
        var src = Path.GetFullPath(RequireMap(map));
        var ext = Path.GetExtension(src);
        output = Path.GetFullPath(output ?? Path.Combine(Path.GetDirectoryName(src)!,
            Path.GetFileNameWithoutExtension(src) + "_doctor" + ext));
        if (string.Equals(output, src, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("output must differ from the map, the original is never written.");
        if (File.Exists(output))
            throw new IOException($"{output} already exists. Pass another output, it is never overwritten.");

        var args = new List<string> { "fix" };
        foreach (var o in FixArgs(options)) args.Add(o);

        var work = Path.Combine(Path.GetTempPath(), "wc3ctl-doctor", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(work);
        try
        {
            var copy = Path.Combine(work, "map" + ext);
            File.Copy(src, copy);
            args.Insert(1, copy);
            var r = Run("fix", args, work);
            if (r.ExitCode != 0) return r;

            // The doctor names its result <map>_fixed.<ext> beside the input it was given.
            var produced = Path.Combine(work, "map_fixed" + ext);

            // On a map with nothing to repair the doctor says so, outcome "nothing" and file null,
            // and writes no file. That is an answer, so it is returned with no output path.
            if (!File.Exists(produced) && r.Result is { } json
                && json.TryGetProperty("outcome", out var outcome) && outcome.GetString() == "nothing")
                return r with { Output = null };
            if (!File.Exists(produced))
                throw new InvalidOperationException(
                    $"the doctor exited 0 but wrote no {Path.GetFileName(produced)} in {work}.");
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            File.Move(produced, output);
            return r with { Output = output };
        }
        finally
        {
            try { Directory.Delete(work, recursive: true); } catch { }
        }
    }

    internal static IEnumerable<string> FixArgs(IEnumerable<string>? options)
    {
        foreach (var raw in options ?? Array.Empty<string>())
        {
            var o = raw.Trim().TrimStart('-');
            var name = o.StartsWith("no-", StringComparison.Ordinal) ? o[3..] : o;
            if (!FixOptions.Contains(name))
                throw new ArgumentException($"'{raw}' is not a fix option this wrapper allows. "
                    + $"Allowed, each optionally prefixed no-: {string.Join(", ", FixOptions)}.");
            yield return "--" + o;
        }
    }

    private static string RequireMap(string map)
    {
        if (string.IsNullOrWhiteSpace(map) || !File.Exists(map))
            throw new FileNotFoundException($"no map at {map}");
        return map;
    }

    /// <summary>Finds the doctor and checks it against the pinned builds.</summary>
    public static (string Exe, string Version, string Sha256) Locate(
        Func<string, string?>? env = null, string? localAppData = null)
    {
        env ??= Environment.GetEnvironmentVariable;
        localAppData ??= Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string? exe = env("WC3_DOCTOR");
        if (string.IsNullOrEmpty(exe))
        {
            var root = Path.Combine(localAppData, "wc3ctl", "doctor");
            exe = Directory.Exists(root)
                ? Directory.GetDirectories(root)
                    .Select(d => Path.Combine(d, ExeName)).Where(File.Exists)
                    .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault()
                : null;
            if (exe is null)
                throw new FileNotFoundException(
                    $"Devo's Map Doctor not found. Put {ExeName} and names.npz from "
                    + "github.com/devoltzz/devos-map-doctor/releases in "
                    + $"{Path.Combine(root, "<version>")}, or set WC3_DOCTOR to the exe.");
        }
        if (!File.Exists(exe)) throw new FileNotFoundException($"WC3_DOCTOR names {exe}, which does not exist.");

        string sha;
        using (var s = File.OpenRead(exe)) sha = Convert.ToHexString(SHA256.HashData(s)).ToLowerInvariant();
        if (PinnedBuilds.TryGetValue(sha, out var version)) return (exe, version, sha);
        if (env("WC3_DOCTOR_UNPINNED") == "1") return (exe, "unpinned", sha);
        throw new InvalidOperationException(
            $"{exe} has sha256 {sha}, which is not a doctor build this wrapper was checked against "
            + $"({string.Join(", ", PinnedBuilds.Values)}). Set WC3_DOCTOR_UNPINNED=1 to run it anyway.");
    }

    private static MapDoctorResult Run(string command, List<string> args, string? workDir)
    {
        var (exe, version, sha) = Locate();
        var psi = new ProcessStartInfo(exe)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            // The doctor reads nothing from stdin, and a CLI left with an open stdin can wait on it.
            RedirectStandardInput = true,
            CreateNoWindow = true,
            WorkingDirectory = workDir ?? Path.GetDirectoryName(exe)!,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);
        psi.ArgumentList.Add("--json");
        psi.ArgumentList.Add("--quiet");

        using var p = Process.Start(psi) ?? throw new InvalidOperationException($"could not start {exe}");
        p.StandardInput.Close();
        var stderrTask = p.StandardError.ReadToEndAsync();
        var stdout = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        var stderr = stderrTask.Result;

        JsonElement? json = null;
        if (!string.IsNullOrWhiteSpace(stdout))
        {
            try { json = JsonDocument.Parse(stdout).RootElement.Clone(); }
            catch (JsonException)
            {
                // Some commands print text even with --json. Keep it rather than drop it.
                json = JsonSerializer.SerializeToElement(new { text = stdout });
            }
        }
        var tail = string.Join('\n', stderr.Split('\n').TakeLast(20)).Trim();
        return new(command, p.ExitCode, json, null, $"{version} sha256:{sha[..12]}",
            tail.Length == 0 ? null : tail);
    }
}
