// tests/Wc3.Tests/McpWriteGateTests.cs
using Wc3.Commands;
using Wc3.Mcp;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Slice one of the safe chaining work. The gate is the one path a write takes, so these tests
/// pin its refusals, its digests and its atomic write without going through the protocol.
///
/// A replacement now needs the user's overwrite grant (see Permissions), so the fixture writes a
/// permission file into its own temporary folder and points WC3_MCP_PERMISSIONS at it. The
/// machine's own permissions.json is never read or touched. The class joins the Serial
/// collection because an environment variable is process wide.
/// </summary>
[Collection("Serial")]
public sealed class McpWriteGateTests : IDisposable
{
    private readonly string _dir;
    private readonly string _map;
    private readonly string? _savedPermissions;

    public McpWriteGateTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "wc3gate-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        _map = Path.Combine(_dir, "in.w3x");
        File.WriteAllBytes(_map, BlankMap.CreateArchiveBytes());

        // A test process is the user here, so it grants overwrite. Denial is proven in
        // PermissionsTests, which is the suite that owns that behaviour.
        File.WriteAllText(Path.Combine(_dir, "permissions.json"), "{\"overwrite\": true}");
        _savedPermissions = Environment.GetEnvironmentVariable(Permissions.EnvironmentVariable);
        Environment.SetEnvironmentVariable(Permissions.EnvironmentVariable,
            Path.Combine(_dir, "permissions.json"));
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(Permissions.EnvironmentVariable, _savedPermissions);
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    private MapDocument Load() => MapDocument.Load(_map);

    private static string Code(Action action)
    {
        var ex = Assert.Throws<ToolError>(action);
        return ex.Code;
    }

    [Fact]
    public void An_output_equal_to_the_input_is_refused()
    {
        Assert.Equal("OUT_PATH_IS_INPUT", Code(() => WriteGate.Save(Load(), _map, _map)));
    }

    [Fact]
    public void An_existing_output_is_refused_without_overwrite()
    {
        string output = Path.Combine(_dir, "out.w3x");
        File.WriteAllBytes(output, new byte[] { 1 });
        Assert.Equal("OUTPUT_EXISTS", Code(() => WriteGate.Save(Load(), _map, output)));
    }

    [Fact]
    public void Overwrite_writes_the_file_and_reports_both_digests()
    {
        string output = Path.Combine(_dir, "out.w3x");
        File.WriteAllBytes(output, new byte[] { 1 });

        var w = WriteGate.Save(Load(), _map, output, overwrite: true);

        Assert.True(File.Exists(output));
        Assert.Equal(WriteGate.Sha256File(_map), w.InputSha256);
        Assert.Equal(WriteGate.Sha256File(output), w.OutputSha256);
        Assert.Equal(new FileInfo(output).Length, w.BytesWritten);
        Assert.Empty(Directory.GetFiles(_dir, "*.tmp"));
    }

    [Fact]
    public void A_stale_input_digest_is_refused()
    {
        string output = Path.Combine(_dir, "out.w3x");
        Assert.Equal("STALE_INPUT",
            Code(() => WriteGate.Save(Load(), _map, output, expectInputSha256: new string('0', 64))));
        Assert.False(File.Exists(output));
    }

    [Fact]
    public void The_matching_input_digest_passes_and_is_reported()
    {
        string output = Path.Combine(_dir, "out.w3x");
        string sha = WriteGate.Sha256File(_map);

        var w = WriteGate.Save(Load(), _map, output, expectInputSha256: sha);

        Assert.Equal(sha, w.InputSha256);
        Assert.True(File.Exists(output));
    }
    [Fact]
    public void An_existing_output_folder_is_refused_without_overwrite()
    {
        string output = Path.Combine(_dir, "already-a-folder.w3x");
        Directory.CreateDirectory(output);

        Assert.Equal("OUTPUT_EXISTS", Code(() => WriteGate.Save(Load(), _map, output)));
    }

    [Fact]
    public void An_input_replaced_after_load_is_refused_rather_than_saved()
    {
        string output = Path.Combine(_dir, "out.w3x");
        string loadedSha = WriteGate.Sha256File(_map);
        var doc = Load();

        // Something else rewrote the base while the document was open.
        File.WriteAllBytes(_map, BlankMap.CreateArchiveBytes().Concat(new byte[] { 0 }).ToArray());

        Assert.Equal("STALE_INPUT",
            Code(() => WriteGate.Save(doc, _map, output, loadedInputSha: loadedSha)));
        Assert.False(File.Exists(output));
    }

    [Fact]
    public void An_input_untouched_since_load_saves_and_reports_the_loaded_digest()
    {
        string output = Path.Combine(_dir, "out.w3x");
        string loadedSha = WriteGate.Sha256File(_map);

        var w = WriteGate.Save(Load(), _map, output, loadedInputSha: loadedSha);

        Assert.Equal(loadedSha, w.InputSha256);
        Assert.True(File.Exists(output));
    }

    [Fact]
    public void A_destination_that_appears_before_the_commit_is_reported_as_existing()
    {
        string temp = Path.Combine(_dir, "staged.tmp");
        string output = Path.Combine(_dir, "raced.w3x");
        File.WriteAllBytes(temp, new byte[] { 1, 2, 3 });
        File.WriteAllBytes(output, new byte[] { 4, 5, 6 });

        Assert.Equal("OUTPUT_EXISTS", Code(() => WriteGate.Commit(temp, output, overwrite: false)));
        Assert.Equal(new byte[] { 4, 5, 6 }, File.ReadAllBytes(output));
        Assert.True(File.Exists(temp), "a refused commit leaves the staged file for the caller to clean up");
    }

    [Fact]
    public void A_commit_with_overwrite_replaces_the_destination()
    {
        string temp = Path.Combine(_dir, "staged2.tmp");
        string output = Path.Combine(_dir, "replaced.w3x");
        File.WriteAllBytes(temp, new byte[] { 1, 2, 3 });
        File.WriteAllBytes(output, new byte[] { 4, 5, 6 });

        WriteGate.Commit(temp, output, overwrite: true);

        Assert.Equal(new byte[] { 1, 2, 3 }, File.ReadAllBytes(output));
        Assert.False(File.Exists(temp));
    }

    [Fact]
    public void A_folder_map_is_written_as_a_folder_and_keeps_the_default_outside_it()
    {
        string folder = Path.Combine(_dir, "folder.w3x");
        var src = MapDocument.Load(BlankMap.CreateArchiveBytes());
        foreach (var f in src.Files)
        {
            if (f.FileName is null || f.FileName.StartsWith('(')) continue;
            string path = Path.Combine(folder, f.FileName.Replace('\\', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, f.RawBytes);
        }

        Assert.False(File.Exists(folder));
        var doc = MapDocument.Load(folder);
        string output = Path.Combine(_dir, "folder-out.w3x");
        var w = WriteGate.Save(doc, folder, output);

        Assert.True(Directory.Exists(output));
        Assert.Equal("", w.OutputSha256);
    }

    [Fact]
    public void An_output_path_through_a_link_is_refused_and_names_the_link()
    {
        string real = Path.Combine(_dir, "real");
        Directory.CreateDirectory(real);
        string link = Path.Combine(_dir, "link");
        CreateDirectoryLink(link, real);
        try
        {
            string output = Path.Combine(link, "out.w3x");

            var ex = Assert.Throws<ToolError>(() => WriteGate.Save(Load(), _map, output));

            Assert.Equal("OUTPUT_IS_LINK", ex.Code);
            Assert.Contains(link, ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.False(File.Exists(Path.Combine(real, "out.w3x")),
                "a refused write must leave the link's target untouched");
        }
        finally
        {
            if (Directory.Exists(link)) Directory.Delete(link, recursive: false);
        }
    }

    // ---- the file level gate (SAF-15) ------------------------------------
    // The same five refusals and the same digests the map path reports, for every writing tool
    // whose output is not the map archive. Each test pins one clause of that contract.

    [Fact]
    public void A_new_file_lands_and_both_digests_are_reported()
    {
        string output = Path.Combine(_dir, "report.txt");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes("hello gate");

        var w = WriteGate.WriteBytes(output, bytes, new[] { _map }, overwrite: false, expectInputSha256: null);

        Assert.True(File.Exists(output));
        Assert.Equal(bytes, File.ReadAllBytes(output));
        Assert.Equal(WriteGate.Sha256File(_map), w.InputSha256);
        Assert.Equal(WriteGate.Sha256File(output), w.OutputSha256);
        Assert.Equal(bytes.Length, w.BytesWritten);
        Assert.Equal(1, w.FilesRewritten);
        Assert.Single(w.Files);
        Assert.Empty(Directory.GetFiles(_dir, "*.tmp*"));
    }

    [Fact]
    public void An_existing_file_is_refused_without_overwrite_and_replaced_with_it()
    {
        string output = Path.Combine(_dir, "existing.txt");
        File.WriteAllText(output, "old");

        Assert.Equal("OUTPUT_EXISTS",
            Code(() => WriteGate.WriteText(output, "new", Array.Empty<string>(), overwrite: false, null)));
        Assert.Equal("old", File.ReadAllText(output));

        var w = WriteGate.WriteText(output, "new", Array.Empty<string>(), overwrite: true, null);

        Assert.Equal("new", File.ReadAllText(output));
        Assert.Equal(WriteGate.Sha256File(output), w.OutputSha256);
        Assert.Equal(3, w.BytesWritten);
    }

    [Fact]
    public void A_file_output_equal_to_an_input_is_refused()
    {
        string output = Path.Combine(_dir, "same.txt");
        File.WriteAllText(output, "x");

        Assert.Equal("OUT_PATH_IS_INPUT",
            Code(() => WriteGate.WriteText(output, "y", new[] { output }, overwrite: true, null)));
        Assert.Equal("x", File.ReadAllText(output));
    }

    [Fact]
    public void A_file_output_through_a_link_is_refused_and_names_the_link()
    {
        string real = Path.Combine(_dir, "file-real");
        Directory.CreateDirectory(real);
        string link = Path.Combine(_dir, "file-link");
        CreateDirectoryLink(link, real);
        try
        {
            string output = Path.Combine(link, "out.txt");

            var ex = Assert.Throws<ToolError>(() =>
                WriteGate.WriteText(output, "x", Array.Empty<string>(), overwrite: false, null));

            Assert.Equal("OUTPUT_IS_LINK", ex.Code);
            Assert.Contains(link, ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.False(File.Exists(Path.Combine(real, "out.txt")),
                "a refused write must leave the link's target untouched");
        }
        finally
        {
            if (Directory.Exists(link)) Directory.Delete(link, recursive: false);
        }
    }

    [Fact]
    public void A_failure_between_stage_and_rename_leaves_no_partial_file()
    {
        string output = Path.Combine(_dir, "partial.txt");

        Assert.Throws<InvalidOperationException>(() => WriteGate.WriteFile(output,
            Array.Empty<string>(), overwrite: false, expectInputSha256: null, write: temp =>
            {
                File.WriteAllText(temp, "half");
                throw new InvalidOperationException("the renderer failed mid stage");
            }));

        Assert.False(File.Exists(output));
        Assert.Empty(Directory.GetFiles(_dir, "*.tmp*"));
    }


    private static void CreateDirectoryLink(string link, string target)
    {
        try
        {
            Directory.CreateSymbolicLink(link, target);
            return;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            string symlinkError = ex.GetType().Name + ", " + ex.Message;
            var psi = new System.Diagnostics.ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            foreach (var a in new[] { "/c", "mklink", "/J", link, target }) psi.ArgumentList.Add(a);
            try
            {
                using var p = System.Diagnostics.Process.Start(psi);
                if (p is not null)
                {
                    string outText = p.StandardOutput.ReadToEnd();
                    string errText = p.StandardError.ReadToEnd();
                    p.WaitForExit();
                    if (p.ExitCode == 0 && Directory.Exists(link)) return;
                    Assert.Fail("could not create a directory link. symbolic link, " + symlinkError
                        + ". junction, exit " + p.ExitCode + ", " + (outText + errText).Trim());
                }
            }
            catch (Exception junctionError)
            {
                Assert.Fail("could not create a directory link. symbolic link, " + symlinkError
                    + ". junction, " + junctionError.Message);
            }
            Assert.Fail("could not create a directory link. symbolic link, " + symlinkError);
        }
    }

}
