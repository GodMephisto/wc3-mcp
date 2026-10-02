using System.Text;
namespace Wc3.GameData;

public sealed class SlkTable
{
    private readonly List<string> _headers = new();
    private readonly Dictionary<string, Dictionary<string, string>> _rowsByKey = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<string> Headers => _headers;

    public IEnumerable<string> RowKeys => _rowsByKey.Keys;

    public bool TryGetRow(string firstColKey, out IReadOnlyDictionary<string, string> row)
    {
        if (_rowsByKey.TryGetValue(firstColKey, out var r)) { row = r; return true; }
        row = new Dictionary<string, string>();
        return false;
    }

    public static SlkTable Parse(byte[] bytes)
    {
        var table = new SlkTable();
        // cells[row][col] (1-based indices as they appear in SLK)
        var cells = new Dictionary<int, Dictionary<int, string>>();
        int curX = 1, curY = 1, maxCol = 0;

        using var reader = new StringReader(Encoding.UTF8.GetString(bytes));
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Length == 0) continue;
            char rec = line[0];
            if (rec != 'C' && rec != 'F') continue;           // only cell + format carry X/Y/K
            string? k = null;
            foreach (var field in SplitSlk(line.Substring(1)))
            {
                if (field.Length < 1) continue;
                char tag = field[0];
                string rest = field.Substring(1);
                switch (tag)
                {
                    case 'X': if (int.TryParse(rest, out var x)) curX = x; break;
                    case 'Y': if (int.TryParse(rest, out var y)) curY = y; break;
                    case 'K': k = Unquote(rest); break;
                }
            }
            if (rec == 'C' && k != null)
            {
                if (!cells.TryGetValue(curY, out var rowMap)) cells[curY] = rowMap = new();
                rowMap[curX] = k;
                if (curX > maxCol) maxCol = curX;
            }
        }

        // Row 1 = headers.
        if (cells.TryGetValue(1, out var header))
            for (int c = 1; c <= maxCol; c++)
                table._headers.Add(header.TryGetValue(c, out var h) ? h.Trim().ToLowerInvariant() : $"col{c}");

        // Data rows keyed by first column's value.
        foreach (var (y, rowMap) in cells)
        {
            if (y == 1) continue;
            if (!rowMap.TryGetValue(1, out var key) || string.IsNullOrWhiteSpace(key)) continue;
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int c = 1; c <= table._headers.Count; c++)
                if (rowMap.TryGetValue(c, out var v)) dict[table._headers[c - 1]] = v;
            table._rowsByKey[key] = dict;
        }
        return table;
    }

    // SLK fields are ';'-separated; a ';;' inside a value is a literal ';'.
    private static IEnumerable<string> SplitSlk(string s)
    {
        var parts = new List<string>();
        var sb = new StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ';')
            {
                if (i + 1 < s.Length && s[i + 1] == ';') { sb.Append(';'); i++; }
                else { parts.Add(sb.ToString()); sb.Clear(); }
            }
            else sb.Append(s[i]);
        }
        parts.Add(sb.ToString());
        return parts;
    }

    private static string Unquote(string v)
    {
        v = v.Trim();
        if (v.Length >= 2 && v[0] == '"' && v[^1] == '"') v = v.Substring(1, v.Length - 2).Replace("\"\"", "\"");
        return v;
    }
}
