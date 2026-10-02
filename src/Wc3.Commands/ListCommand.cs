using Wc3.Model;

namespace Wc3.Commands;

public static class ListCommand
{
    public static FileListResult Execute(MapDocument doc) => new(
        doc.Files.Select(f => new FileEntryInfo(f.FileName, f.RawBytes.Length, f.IsKnown, f.IsParsed)).ToList());
}
