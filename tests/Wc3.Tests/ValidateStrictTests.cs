// tests/Wc3.Tests/ValidateStrictTests.cs
// Hermetic tests for ValidateStrictCommand. The missing-listfile and no-entry-point checks need no
// game install, so both are exercised here, and the default validate is shown not to carry them.
using System.Text;
using War3Net.IO.Mpq;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class ValidateStrictTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-validate-strict", Guid.NewGuid().ToString("N"));

    public ValidateStrictTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    /// <summary>A map whose (listfile) names every entry, so entries resolve by name.</summary>
    private string MapWithListfile(IReadOnlyDictionary<string, byte[]> files)
    {
        var builder = new MpqArchiveBuilder();
        foreach (var (name, data) in files)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));
        var listfile = string.Join("\r\n", files.Keys) + "\r\n";
        builder.AddFile(MpqFile.New(new MemoryStream(Encoding.ASCII.GetBytes(listfile)), "(listfile)"));
        return Write(map => builder.SaveTo(map,
            new MpqArchiveCreateOptions { ListFileCreateMode = MpqFileCreateMode.None }, leaveOpen: true));
    }

    /// <summary>A protected-shape map with no (listfile) at all.</summary>
    private string MapWithoutListfile(IReadOnlyDictionary<string, byte[]> files)
    {
        var builder = new MpqArchiveBuilder();
        foreach (var (name, data) in files)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));
        return Write(map => builder.SaveTo(map,
            new MpqArchiveCreateOptions { ListFileCreateMode = MpqFileCreateMode.None }, leaveOpen: true));
    }

    private string Write(Action<Stream> save)
    {
        using var mpq = new MemoryStream();
        save(mpq);
        using var outStream = new MemoryStream();
        var header = new byte[0x200];
        header[0] = (byte)'H'; header[1] = (byte)'M'; header[2] = (byte)'3'; header[3] = (byte)'W';
        outStream.Write(header, 0, header.Length);
        mpq.Position = 0;
        mpq.CopyTo(outStream);

        var path = Path.Combine(_dir, "map-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(path, outStream.ToArray());
        return path;
    }

    private static byte[] Script(string text) => Encoding.ASCII.GetBytes(text);

    [Fact]
    public void Strict_reports_a_script_with_no_entry_point()
    {
        var map = MapWithListfile(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Script("// a script that declares nothing at all\n"),
        });
        var doc = MapDocument.Load(map);

        var strict = ValidateStrictCommand.Execute(doc);

        Assert.Contains(strict.StrictIssues, i =>
            i.Category == "script-entry-point" && i.Severity == DiagnosticSeverity.Error);

        // The default validate does not carry this check.
        var plain = ValidateCommand.Execute(doc);
        Assert.DoesNotContain(plain.Issues, i => i.Category == "script-entry-point");
    }

    [Fact]
    public void Strict_reports_a_missing_listfile()
    {
        var map = MapWithoutListfile(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Script("function main takes nothing returns nothing\nendfunction\n"),
        });
        var doc = MapDocument.Load(map);

        var strict = ValidateStrictCommand.Execute(doc);

        Assert.Contains(strict.StrictIssues, i => i.Category == "missing-listfile");

        var plain = ValidateCommand.Execute(doc);
        Assert.DoesNotContain(plain.Issues, i => i.Category == "missing-listfile");
    }

    [Fact]
    public void Strict_is_silent_on_a_script_with_an_entry_point_and_a_listfile()
    {
        var map = MapWithListfile(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Script("function main takes nothing returns nothing\nendfunction\n"),
        });
        var doc = MapDocument.Load(map);

        var strict = ValidateStrictCommand.Execute(doc);

        Assert.DoesNotContain(strict.StrictIssues, i => i.Category == "script-entry-point");
        Assert.DoesNotContain(strict.StrictIssues, i => i.Category == "missing-listfile");
    }
}
