// tests/Wc3.Tests/FilePreviewTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class FilePreviewTests
{
    private static MapDocument Map() => MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
    {
        ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        ["readme.txt"] = Encoding.UTF8.GetBytes("hello world"),
        ["noext"] = Encoding.UTF8.GetBytes("plain text with no extension"),
        ["blob.bin"] = new byte[] { 0, 1, 2, 3, 0, 255, 128, 7 },
        ["war3mapImported\\sound.mp3"] = new byte[] { 0x49, 0x44, 0x33, 4, 2, 1 },
    }));

    [Fact]
    public void Audio_file_is_classified_as_audio()
    {
        var p = FilePreviewCommand.Execute(Map(), "war3mapImported\\sound.mp3");
        Assert.Equal("audio", p.Kind);
        Assert.Contains("MP3", p.Info);
    }

    [Fact]
    public void Text_file_previews_as_decoded_text()
    {
        var p = FilePreviewCommand.Execute(Map(), "readme.txt");
        Assert.Equal("text", p.Kind);
        Assert.Equal("hello world", p.Text);
        Assert.Null(p.Png);
    }

    [Fact]
    public void Extensionless_but_printable_content_is_detected_as_text()
    {
        var p = FilePreviewCommand.Execute(Map(), "noext");
        Assert.Equal("text", p.Kind);
        Assert.Contains("no extension", p.Text);
    }

    [Fact]
    public void Binary_file_previews_as_a_hex_dump()
    {
        var p = FilePreviewCommand.Execute(Map(), "blob.bin");
        Assert.Equal("binary", p.Kind);
        Assert.NotNull(p.Text);
        Assert.Contains("00 01 02 03", p.Text); // hex bytes present
        Assert.Null(p.Png);
    }

    [Fact]
    public void Missing_file_throws_file_not_found()
    {
        Assert.Throws<FileNotFoundException>(() => FilePreviewCommand.Execute(Map(), "nope.dat"));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Real_blp_icon_decodes_to_a_png()
    {
        string path = CorpusMap.PathOrEmpty;
        if (!File.Exists(path)) return;
        var doc = MapDocument.Load(path);
        var blp = doc.Files.FirstOrDefault(f => f.FileName?.EndsWith(".blp", StringComparison.OrdinalIgnoreCase) == true);
        if (blp?.FileName is null) return;

        var p = FilePreviewCommand.Execute(doc, blp.FileName);
        Assert.Equal("image", p.Kind);
        Assert.NotNull(p.Png);
        Assert.True(p.Png!.Length > 0);
        // PNG magic.
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, p.Png[..4]);
    }
}
