using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record Reforged3RepairResult(
    bool Ok,
    string Message,
    bool Applied,
    int FileColumnsRemoved,
    int ModelsMoved,
    int NumericCellsNormalized,
    int ButtonPositionsCompleted,
    int StrayCommentTerminatorsRemoved,
    int AbilityLevelColumnsAdded,
    IReadOnlyList<string> ChangedFiles)
{
    public int IssueCount =>
        FileColumnsRemoved + ModelsMoved + NumericCellsNormalized + ButtonPositionsCompleted
        + StrayCommentTerminatorsRemoved + AbilityLevelColumnsAdded;
}

/// <summary>
/// Repairs legacy SLK-mode maps rejected or misread by Warcraft III 3.0.0 build 24268.
/// The transformations are based on devoltz and Arakunido's public-domain repair guide:
/// model paths move from obsolete SLK file columns to merged skin profiles, numeric and
/// button-position values are normalized, unmatched FDF comment terminators are removed,
/// and missing ability level 5/6 columns inherit level 4 values.
/// </summary>
public static partial class Reforged3RepairCommand
{
    private static readonly string[] LevelFields =
    [
        "Area", "BuffID", "Cast", "Cool", "Cost",
        "DataA", "DataB", "DataC", "DataD", "DataE", "DataF", "DataG", "DataH", "DataI",
        "Dur", "EfctID", "HeroDur", "Rng", "UnitID", "targs",
    ];

    private static readonly HashSet<string> PositionKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "buttonpos", "researchbuttonpos", "unbuttonpos",
    };

    private static readonly Encoding BytePreservingText = Encoding.Latin1;

    [GeneratedRegex(@"^-?\d+\.?\d*$", RegexOptions.CultureInvariant)]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"^(-?\d+)\.$", RegexOptions.CultureInvariant)]
    private static partial Regex TrailingDotRegex();

    [GeneratedRegex("(?<prefix>(?:^|;)K)\\\"(?<value>[^\\\"]*)\\\"")]
    private static partial Regex QuotedKRegex();

    public static Reforged3RepairResult Execute(MapDocument doc, bool apply = true)
    {
        ArgumentNullException.ThrowIfNull(doc);

        int fileColumns = 0;
        int modelsMoved = 0;
        int numericCells = 0;
        int buttonPositions = 0;
        int fdfTerminators = 0;
        int levelColumns = 0;
        var changes = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        ProcessModelTable(doc, changes, @"Units\UnitUI.slk", "unitUIID", @"Units\UnitSkin.txt",
            ref fileColumns, ref modelsMoved);
        ProcessModelTable(doc, changes, @"Units\ItemData.slk", "itemID", @"Units\ItemSkin.txt",
            ref fileColumns, ref modelsMoved);

        foreach (var entry in FilesUnder(doc, changes, "Units", ".slk"))
        {
            var slk = SlkDocument.Parse(CurrentBytes(entry, changes));
            int changed = slk.NormalizeQuotedNumbers();
            if (changed == 0)
                continue;
            numericCells += changed;
            changes[entry.FileName!] = slk.ToBytes();
        }

        foreach (var entry in FilesUnder(doc, changes, "Units", ".txt"))
        {
            byte[] before = CurrentBytes(entry, changes);
            byte[] after = CompleteButtonPositions(before, out int changed);
            if (changed == 0)
                continue;
            buttonPositions += changed;
            changes[entry.FileName!] = after;
        }

        foreach (var entry in doc.Files.Where(f => f.FileName is not null
                     && f.FileName.EndsWith(".fdf", StringComparison.OrdinalIgnoreCase)))
        {
            byte[] before = CurrentBytes(entry, changes);
            string text = BytePreservingText.GetString(before);
            if (!text.Contains("*/", StringComparison.Ordinal)
                || text.Contains("/*", StringComparison.Ordinal))
                continue;

            int count = CountOccurrences(text, "*/");
            fdfTerminators += count;
            changes[entry.FileName!] = BytePreservingText.GetBytes(text.Replace("*/", "", StringComparison.Ordinal));
        }

        if (FindEntry(doc, @"Units\AbilityData.slk") is { } abilityEntry)
        {
            var slk = SlkDocument.Parse(CurrentBytes(abilityEntry, changes));
            levelColumns = slk.AddMissingAbilityLevels(LevelFields, [5, 6]);
            if (levelColumns > 0)
                changes[abilityEntry.FileName!] = slk.ToBytes();
        }

        if (apply)
        {
            foreach (var (name, bytes) in changes)
                doc.AddOrReplaceRawFile(name, bytes);
        }

        int issueCount = fileColumns + modelsMoved + numericCells + buttonPositions + fdfTerminators + levelColumns;
        string message = issueCount == 0
            ? "no known Warcraft III 3.0.0 SLK compatibility issues found"
            : apply
                ? $"repaired {issueCount} Warcraft III 3.0.0 compatibility issue(s) in {changes.Count} file(s)"
                : $"found {issueCount} Warcraft III 3.0.0 compatibility issue(s) in {changes.Count} file(s)";

        return new Reforged3RepairResult(
            true, message, apply, fileColumns, modelsMoved, numericCells, buttonPositions,
            fdfTerminators, levelColumns, changes.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList());
    }

    private static void ProcessModelTable(
        MapDocument doc,
        Dictionary<string, byte[]> changes,
        string slkName,
        string idColumn,
        string skinName,
        ref int fileColumns,
        ref int modelsMoved)
    {
        var entry = FindEntry(doc, slkName);
        if (entry is null)
            return;

        var slk = SlkDocument.Parse(CurrentBytes(entry, changes));
        if (!slk.TryGetColumn("file", out _))
            return;

        var models = slk.Rows()
            .Select(row => (Id: GetValue(row, idColumn), File: GetValue(row, "file")))
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Id) && !string.IsNullOrWhiteSpace(pair.File))
            // Malformed legacy tables occasionally repeat an id. Match the source
            // repair script's last-value-wins behaviour instead of aborting the map.
            .GroupBy(pair => pair.Id!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last().File!, StringComparer.OrdinalIgnoreCase);

        slk.DropColumn("file");
        changes[entry.FileName!] = slk.ToBytes();
        fileColumns++;

        if (models.Count == 0)
            return;

        var skinEntry = FindEntry(doc, skinName);
        string storedSkinName = skinEntry?.FileName ?? skinName;
        byte[]? existingSkin = skinEntry is null ? null : CurrentBytes(skinEntry, changes);
        changes[storedSkinName] = MergeSkinProfile(existingSkin, models);
        modelsMoved += models.Count;
    }

    private static string? GetValue(IReadOnlyDictionary<string, string> row, string key) =>
        row.TryGetValue(key, out var value) ? value : null;

    private static byte[] MergeSkinProfile(byte[]? existingBytes, IReadOnlyDictionary<string, string> models)
    {
        string newline = existingBytes is not null
            && !BytePreservingText.GetString(existingBytes).Contains("\r\n", StringComparison.Ordinal)
            ? "\n"
            : "\r\n";
        var lines = existingBytes is null
            ? new List<string>
            {
                "// Model paths migrated for Warcraft III 3.0.0 compatibility by wc3ctl.",
                "// Skin profiles merge with base game data, unlike replacement SLK tables.",
                "",
            }
            : SplitLines(BytePreservingText.GetString(existingBytes)).ToList();

        foreach (var (rawcode, modelPath) in models.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            UpsertProfileValue(lines, rawcode, "file", modelPath);

        return BytePreservingText.GetBytes(string.Join(newline, lines));
    }

    private static void UpsertProfileValue(List<string> lines, string section, string key, string value)
    {
        int sectionStart = -1;
        int sectionEnd = lines.Count;
        for (int i = 0; i < lines.Count; i++)
        {
            string trimmed = lines[i].Trim();
            if (!trimmed.StartsWith("[", StringComparison.Ordinal) || !trimmed.EndsWith("]", StringComparison.Ordinal))
                continue;
            string name = trimmed[1..^1];
            if (sectionStart >= 0)
            {
                sectionEnd = i;
                break;
            }
            if (string.Equals(name, section, StringComparison.OrdinalIgnoreCase))
                sectionStart = i;
        }

        if (sectionStart < 0)
        {
            if (lines.Count > 0 && lines[^1].Length != 0)
                lines.Add("");
            lines.Add($"[{section}]");
            lines.Add($"{key}={value}");
            lines.Add("");
            return;
        }

        for (int i = sectionStart + 1; i < sectionEnd; i++)
        {
            string trimmed = lines[i].Trim();
            int equals = trimmed.IndexOf('=');
            if (equals < 0 || !string.Equals(trimmed[..equals].Trim(), key, StringComparison.OrdinalIgnoreCase))
                continue;
            string indent = lines[i][..(lines[i].Length - lines[i].TrimStart().Length)];
            lines[i] = $"{indent}{key}={value}";
            return;
        }

        lines.Insert(sectionEnd, $"{key}={value}");
    }

    private static byte[] CompleteButtonPositions(byte[] bytes, out int changed)
    {
        string text = BytePreservingText.GetString(bytes);
        string newline = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var lines = SplitLines(text).ToArray();
        changed = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string trimmed = lines[i].Trim();
            if (trimmed.StartsWith("//", StringComparison.Ordinal))
                continue;
            int equals = trimmed.IndexOf('=');
            if (equals < 0 || !PositionKeys.Contains(trimmed[..equals].Trim()))
                continue;

            string key = trimmed[..equals];
            string value = trimmed[(equals + 1)..].Trim();
            string completed;
            int comma = value.IndexOf(',');
            if (comma < 0)
                completed = value.Length == 0 ? "0,0" : value + ",0";
            else
                completed = (value[..comma].Trim().Length == 0 ? "0" : value[..comma].Trim())
                    + "," + (value[(comma + 1)..].Trim().Length == 0 ? "0" : value[(comma + 1)..].Trim());

            if (completed == value)
                continue;
            string indent = lines[i][..(lines[i].Length - lines[i].TrimStart().Length)];
            lines[i] = $"{indent}{key}={completed}";
            changed++;
        }

        return changed == 0 ? bytes : BytePreservingText.GetBytes(string.Join(newline, lines));
    }

    private static IEnumerable<MapFileEntry> FilesUnder(
        MapDocument doc,
        IReadOnlyDictionary<string, byte[]> changes,
        string folder,
        string extension) => doc.Files.Where(f => f.FileName is not null
            && Normalize(f.FileName).StartsWith(folder + "\\", StringComparison.OrdinalIgnoreCase)
            && f.FileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

    private static MapFileEntry? FindEntry(MapDocument doc, string name) =>
        doc.Files.FirstOrDefault(f => f.FileName is not null
            && string.Equals(Normalize(f.FileName), Normalize(name), StringComparison.OrdinalIgnoreCase));

    private static byte[] CurrentBytes(MapFileEntry entry, IReadOnlyDictionary<string, byte[]> changes) =>
        changes.TryGetValue(entry.FileName!, out var changed) ? changed : entry.OverrideBytes ?? entry.RawBytes;

    private static string Normalize(string path) => path.Replace('/', '\\');

    private static string[] SplitLines(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int offset = 0;
        while ((offset = text.IndexOf(value, offset, StringComparison.Ordinal)) >= 0)
        {
            count++;
            offset += value.Length;
        }
        return count;
    }

    private sealed class SlkDocument
    {
        private readonly List<string> _lines;
        private readonly List<SlkCell> _cells;
        private readonly string _newline;

        private SlkDocument(List<string> lines, List<SlkCell> cells, string newline)
        {
            _lines = lines;
            _cells = cells;
            _newline = newline;
        }

        public static SlkDocument Parse(byte[] bytes)
        {
            string text = BytePreservingText.GetString(bytes);
            string newline = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
            var lines = SplitLines(text).ToList();
            var cells = new List<SlkCell>();
            int? currentX = null;
            int? currentY = null;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (!line.StartsWith("C;", StringComparison.Ordinal))
                    continue;
                int? x = ReadCoordinate(line, 'X');
                int? y = ReadCoordinate(line, 'Y');
                if (x.HasValue) currentX = x;
                if (y.HasValue) currentY = y;
                string? rawK = ReadRawK(line);
                cells.Add(new SlkCell(i, currentX, currentY, rawK));
            }

            return new SlkDocument(lines, cells, newline);
        }

        public bool TryGetColumn(string name, out int x)
        {
            foreach (var cell in _cells.Where(c => c.Y == 1 && c.X.HasValue && c.RawK is not null))
            {
                if (!string.Equals(Unquote(cell.RawK!), name, StringComparison.OrdinalIgnoreCase))
                    continue;
                x = cell.X!.Value;
                return true;
            }
            x = 0;
            return false;
        }

        public IEnumerable<IReadOnlyDictionary<string, string>> Rows()
        {
            var headers = _cells.Where(c => c.Y == 1 && c.X.HasValue && c.RawK is not null)
                .ToDictionary(c => c.X!.Value, c => Unquote(c.RawK!), EqualityComparer<int>.Default);
            foreach (var row in _cells.Where(c => c.Y is > 1 && c.X.HasValue && c.RawK is not null).GroupBy(c => c.Y!.Value))
            {
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in row)
                    if (headers.TryGetValue(cell.X!.Value, out var header))
                        values[header] = Unquote(cell.RawK!);
                yield return values;
            }
        }

        public void DropColumn(string name)
        {
            if (!TryGetColumn(name, out int droppedX))
                return;
            var keptColumns = _cells.Where(c => c.Y == 1 && c.X.HasValue && c.RawK is not null && c.X != droppedX)
                .Select(c => c.X!.Value).Distinct().OrderBy(x => x).ToArray();
            var remap = keptColumns.Select((oldX, index) => (oldX, NewX: index + 1))
                .ToDictionary(pair => pair.oldX, pair => pair.NewX);

            RewriteAllCells(_cells
                .Where(c => c.X.HasValue && c.Y.HasValue && c.RawK is not null && remap.ContainsKey(c.X.Value))
                .Select(c => c with { X = remap[c.X!.Value] })
                .ToList(), keptColumns.Length);
        }

        public int NormalizeQuotedNumbers()
        {
            var headers = _cells.Where(c => c.Y == 1 && c.X.HasValue && c.RawK is not null)
                .ToDictionary(c => c.X!.Value, c => Unquote(c.RawK!));
            var numericColumns = new HashSet<int>();
            foreach (var (x, _) in headers)
            {
                var values = _cells.Where(c => c.Y is > 1 && c.X == x && c.RawK is not null)
                    .Select(c => Unquote(c.RawK!)).Where(v => v.Length > 0).ToList();
                if (values.Count >= 4 && values.Count(v => NumberRegex().IsMatch(v)) >= values.Count * 0.9)
                    numericColumns.Add(x);
            }

            int changed = 0;
            foreach (var cell in _cells.Where(c => c.Y is > 1 && c.X.HasValue && numericColumns.Contains(c.X.Value)
                         && c.RawK is { Length: >= 2 } raw && raw[0] == '\"'))
            {
                var match = QuotedKRegex().Match(_lines[cell.LineIndex]);
                if (!match.Success)
                    continue;
                string value = match.Groups["value"].Value;
                var trailingDot = TrailingDotRegex().Match(value);
                if (trailingDot.Success)
                    value = trailingDot.Groups[1].Value;
                else if (!NumberRegex().IsMatch(value))
                    continue;
                _lines[cell.LineIndex] = QuotedKRegex().Replace(
                    _lines[cell.LineIndex], m => m.Groups["prefix"].Value + value, 1);
                changed++;
            }
            return changed;
        }

        public int AddMissingAbilityLevels(IReadOnlyList<string> fields, IReadOnlyList<int> levels)
        {
            var headers = _cells.Where(c => c.Y == 1 && c.X.HasValue && c.RawK is not null)
                .ToDictionary(c => Unquote(c.RawK!), c => c.X!.Value, StringComparer.OrdinalIgnoreCase);
            int lastX = _cells.Where(c => c.X.HasValue).Select(c => c.X!.Value).DefaultIfEmpty(0).Max();
            var plan = new List<(int SourceX, int NewX, string Header)>();
            foreach (string field in fields)
            {
                if (!headers.TryGetValue(field + "4", out int sourceX))
                    continue;
                foreach (int level in levels)
                {
                    string header = field + level.ToString(CultureInfo.InvariantCulture);
                    if (headers.ContainsKey(header))
                        continue;
                    plan.Add((sourceX, ++lastX, header));
                }
            }
            if (plan.Count == 0)
                return 0;

            var rewritten = _cells.Where(c => c.X.HasValue && c.Y.HasValue && c.RawK is not null).ToList();
            foreach (var item in plan)
                rewritten.Add(new SlkCell(int.MaxValue, item.NewX, 1, Quote(item.Header)));
            foreach (var source in _cells.Where(c => c.Y is > 1 && c.X.HasValue && c.RawK is not null))
                foreach (var item in plan.Where(p => p.SourceX == source.X))
                    rewritten.Add(new SlkCell(int.MaxValue, item.NewX, source.Y, source.RawK));

            RewriteAllCells(rewritten.OrderBy(c => c.Y).ThenBy(c => c.X).ToList(), lastX);
            return plan.Count;
        }

        public byte[] ToBytes() => BytePreservingText.GetBytes(string.Join(_newline, _lines));

        private void RewriteAllCells(List<SlkCell> cells, int columnCount)
        {
            var byLine = cells.Where(c => c.LineIndex != int.MaxValue)
                .ToDictionary(c => c.LineIndex);
            var additionsByRow = cells.Where(c => c.LineIndex == int.MaxValue)
                .GroupBy(c => c.Y!.Value).ToDictionary(g => g.Key, g => g.ToList());
            var rewritten = new List<string>();
            int? previousRow = null;
            foreach (var (line, index) in _lines.Select((line, index) => (line, index)))
            {
                if (line.StartsWith("C;", StringComparison.Ordinal))
                {
                    if (byLine.TryGetValue(index, out var cell))
                    {
                        if (previousRow.HasValue && cell.Y != previousRow)
                            AppendAdditions(rewritten, additionsByRow, previousRow.Value);
                        rewritten.Add(FormatCell(cell));
                        previousRow = cell.Y;
                    }
                    continue;
                }

                if (line.StartsWith("E", StringComparison.Ordinal) && previousRow.HasValue)
                {
                    AppendAdditions(rewritten, additionsByRow, previousRow.Value);
                    previousRow = null;
                }
                rewritten.Add(line.StartsWith("B;", StringComparison.Ordinal)
                    ? Regex.Replace(line, @"X\d+", "X" + columnCount.ToString(CultureInfo.InvariantCulture))
                    : line);
            }
            if (previousRow.HasValue)
                AppendAdditions(rewritten, additionsByRow, previousRow.Value);

            _lines.Clear();
            _lines.AddRange(rewritten);
            _cells.Clear();
            _cells.AddRange(Parse(ToBytes())._cells);
        }

        private static void AppendAdditions(
            ICollection<string> output,
            IReadOnlyDictionary<int, List<SlkCell>> additions,
            int row)
        {
            if (!additions.TryGetValue(row, out var cells))
                return;
            foreach (var cell in cells.OrderBy(c => c.X))
                output.Add(FormatCell(cell));
        }

        private static string FormatCell(SlkCell cell) =>
            $"C;X{cell.X!.Value.ToString(CultureInfo.InvariantCulture)};Y{cell.Y!.Value.ToString(CultureInfo.InvariantCulture)};K{cell.RawK}";

        private static int? ReadCoordinate(string line, char coordinate)
        {
            var match = Regex.Match(line, $@"(?:^|;){coordinate}(\d+)(?:;|$)");
            return match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int value)
                ? value
                : null;
        }

        private static string? ReadRawK(string line)
        {
            int marker = line.IndexOf(";K", StringComparison.Ordinal);
            return marker < 0 ? null : line[(marker + 2)..];
        }

        private static string Unquote(string value)
        {
            string trimmed = value.Trim();
            return trimmed.Length >= 2 && trimmed[0] == '\"' && trimmed[^1] == '\"'
                ? trimmed[1..^1].Replace("\"\"", "\"", StringComparison.Ordinal)
                : trimmed;
        }

        private static string Quote(string value) => "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
    }

    private sealed record SlkCell(int LineIndex, int? X, int? Y, string? RawK);
}
