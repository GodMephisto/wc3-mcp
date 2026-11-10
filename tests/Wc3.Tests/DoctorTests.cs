// tests/Wc3.Tests/DoctorTests.cs
// The doctor over a temp home and a temp exe folder, with the release lookup injected, so no real
// client config, game install or network is ever consulted.
using Wc3.Commands;
using Wc3.Mcp.Setup;

namespace Wc3.Tests;

public sealed class DoctorTests : IDisposable
{
    private static readonly SetupProduct Product = SetupProduct.Standalone;

    private static readonly string[] ContextItems =
    {
        "ui-frames/ui-frames.md",
        "abilities/ability-insight.md",
        "skills/map-safety.md",
        "skills/hero-package.md",
        "skills/publish.md",
    };

    private readonly string _root = Path.Combine(Path.GetTempPath(), "wc3-doctor-" + Guid.NewGuid().ToString("N"));

    public DoctorTests()
    {
        Directory.CreateDirectory(Home);
        Directory.CreateDirectory(Exe);
        // Only its presence is checked, so the content does not matter.
        File.WriteAllText(Path.Combine(Exe, "CascLib.dll"), "not a real library");
    }

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch (IOException) { }
    }

    private string Home => Path.Combine(_root, "home");
    private string Exe => Path.Combine(_root, "exe");

    private SetupEnvironment Env => new(
        Home,
        Path.Combine(_root, "appdata"),
        Path.Combine(_root, "local"),
        Path.Combine(Home, ".codex"),
        _ => null,
        (_, _) => (0, ""),
        () => null,
        _ => { });

    private (int Code, string Output) Doctor(Func<SetupProduct, UpdateCheckResult> latest)
    {
        var sw = new StringWriter();
        int code = SetupCli.Run(new[] { "doctor" }, Env, sw, Product, entryOverride: null,
            doctor: new DoctorProbe(Exe, latest));
        return (code, sw.ToString());
    }

    private static UpdateCheckResult Release(string latest, bool upToDate) =>
        new(true, upToDate, Product.Version, latest, null, null, "");

    /// <summary>Writes the context folder beside the temp exe, minus the items named in <paramref name="skip"/>.</summary>
    private void InstallContext(params string[] skip)
    {
        foreach (var relative in ContextItems)
        {
            if (skip.Contains(relative)) continue;
            var path = Path.Combine(Exe, "context", relative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, "# " + relative);
        }
    }

    [Fact]
    public void A_missing_context_document_is_reported_and_fails_the_doctor()
    {
        InstallContext(skip: "abilities/ability-insight.md");

        var (code, output) = Doctor(_ => Release(Product.Version, upToDate: true));

        Assert.Equal(1, code);
        Assert.Contains("abilities/ability-insight.md", output);
        Assert.Contains("MISSING", output);
        // The message names the folder the file was expected in, so the report is actionable.
        Assert.Contains(Path.Combine(Exe, "context", "abilities"), output);
        // The items that are present are still reported, so a partial install shows its shape.
        Assert.Contains("skills/map-safety.md", output);
        Assert.Contains("present", output);
    }

    [Fact]
    public void A_complete_context_folder_passes()
    {
        InstallContext();

        var (code, output) = Doctor(_ => Release(Product.Version, upToDate: true));

        Assert.Equal(0, code);
        Assert.Contains(Path.Combine(Exe, "context"), output);
        foreach (var item in ContextItems) Assert.Contains(item, output);
        Assert.DoesNotContain("MISSING", output);
    }

    [Fact]
    public void A_missing_context_folder_is_reported_against_the_exe_folder()
    {
        var (code, output) = Doctor(_ => Release(Product.Version, upToDate: true));

        Assert.Equal(1, code);
        Assert.Contains("context", output);
        Assert.Contains("beside the exe", output);
    }

    [Fact]
    public void A_network_failure_is_skipped_and_leaves_the_install_healthy()
    {
        InstallContext();

        var (code, output) = Doctor(_ => throw new HttpRequestException("no network"));

        Assert.Equal(0, code);
        Assert.Contains("skipped", output);
        Assert.Contains("could not read the latest release", output);
        Assert.DoesNotContain("MISSING", output);
    }

    [Fact]
    public void A_newer_release_is_a_warning_not_a_failure()
    {
        InstallContext();

        var (code, output) = Doctor(_ => Release("99.0.0", upToDate: false));

        Assert.Equal(0, code);
        Assert.Contains("99.0.0", output);
        Assert.Contains("warning", output);
        Assert.DoesNotContain("MISSING", output);
    }
}
