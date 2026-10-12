// tests/Wc3.Tests/McpToolSmokeTests.Library.cs
// The library MCP tools, exercised through the protocol against temporary folders of synthetic
// maps, so the coverage check has a smoke test for each and no test needs data/schemas or a real
// map on disk.
using System.Text;

namespace Wc3.Tests;

public sealed class McpSmokeLibraryTests : McpSmokeBase
{
    public McpSmokeLibraryTests(SmokeFixture f) : base(f) { }

    private static byte[] Map(string script) =>
        SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = new byte[] { 1, 2, 3, 4 },
            ["war3map.w3e"] = new byte[] { 1, 2, 3, 4 },
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        });

    private const string ValidScript =
        "function config takes nothing returns nothing\nendfunction\n"
        + "function main takes nothing returns nothing\nendfunction\n";

    private string Folder(string label, params (string Name, byte[] Bytes)[] maps)
    {
        var folder = Path.Combine(_f.Dir, $"{label}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        foreach (var (name, bytes) in maps)
            File.WriteAllBytes(Path.Combine(folder, name), bytes);
        return folder;
    }

    [Fact, Covers("library_scan", "library_validate")]
    public async Task Library_scan_and_validate_sweep_a_folder()
    {
        var folder = Folder("lib-scan",
            ("good.w3x", Map(ValidScript)),
            ("bad.w3x", new byte[] { 1, 2, 3, 4 }));

        var scan = await Call("library_scan", Args(("folder", folder)));
        Assert.Equal(2, Rows(Prop(scan, "maps")).Count);
        Assert.Equal(1, Prop(scan, "loaded").GetInt32());
        Assert.Equal(1, Prop(scan, "failed").GetInt32());

        var val = await Call("library_validate", Args(("folder", folder)));
        Assert.Equal(2, Rows(Prop(val, "maps")).Count);
        Assert.Equal(1, Prop(val, "invalid").GetInt32());
    }

    [Fact, Covers("library_audit")]
    public async Task Library_audit_sweeps_a_folder()
    {
        var folder = Folder("lib-audit",
            ("a.w3x", Map(ValidScript)),
            ("b.w3x", Map(ValidScript)));

        var r = await Call("library_audit", Args(("folder", folder)));

        Assert.Equal(2, Rows(Prop(r, "maps")).Count);
        // Synthetic maps carry no object data, so the behavioural audit reports no findings.
        Assert.Empty(Rows(Prop(r, "checks")));
        Assert.Equal(0, Prop(r, "totalFindings").GetInt32());
    }

    [Fact, Covers("library_compare")]
    public async Task Library_compare_diffs_two_folders()
    {
        var shared = Map(ValidScript);
        var a = Folder("lib-a", ("onlya.w3x", Map("A")), ("same.w3x", shared));
        var b = Folder("lib-b", ("onlyb.w3x", Map("B")), ("same.w3x", shared));

        var r = await Call("library_compare", Args(("folder_a", a), ("folder_b", b)));

        Assert.Single(Rows(Prop(r, "onlyInA")));
        Assert.Single(Rows(Prop(r, "onlyInB")));
        Assert.Equal(1, Prop(r, "same").GetInt32());
        Assert.Empty(Rows(Prop(r, "changed")));
    }

    [Fact, Covers("library_report")]
    public async Task Library_report_writes_a_markdown_file()
    {
        var folder = Folder("lib-report",
            ("a.w3x", Map(ValidScript)));
        string outPath = Path.Combine(_f.Dir, "lib-report-" + Guid.NewGuid().ToString("N") + ".md");

        var r = await Call("library_report", Args(("folder", folder), ("out_path", outPath)));

        Assert.True(File.Exists(outPath), "library_report wrote nothing");
        Assert.Equal(1, Prop(r, "maps").GetInt32());
        Assert.Contains("# Map library report", File.ReadAllText(outPath));
    }
}
