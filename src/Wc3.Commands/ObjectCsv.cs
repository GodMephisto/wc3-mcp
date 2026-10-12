// src/Wc3.Commands/ObjectCsv.cs
using System.Text;

namespace Wc3.Commands;

/// <summary>
/// One CSV reader and writer for the object export/import pair. Both directions live here
/// rather than one in each command, so a value the exporter quotes is a value the importer
/// reads back the same way. The rules are RFC 4180 with the two conventions this toolkit
/// needs, a header row is mandatory and a field is quoted only when it has to be.
/// </summary>
internal static class ObjectCsv
{
    /// <summary>Serializes one field. A value holding a comma, quote, carriage return or line
    /// feed is wrapped in quotes with each internal quote doubled, otherwise it is written bare.</summary>
    public static string Escape(string value)
    {
        if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0) return value;
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    /// <summary>Serializes a row of fields, comma separated, with no trailing newline.</summary>
    public static string Row(IEnumerable<string> fields) =>
        string.Join(",", fields.Select(Escape));

    /// <summary>
    /// Parses a CSV document into rows of fields. A quoted field may hold commas and line
    /// feeds, an escaped quote is two quotes, and both CRLF and LF end a record. A trailing
    /// empty line is dropped, and a blank document yields no rows. Never throws, a malformed
    /// quote ends the final field and the caller validates the shape.
    /// </summary>
    public static List<List<string>> Parse(string text)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new StringBuilder();
        bool inQuotes = false;
        bool fieldStarted = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                    else inQuotes = false;
                }
                else field.Append(c);
                continue;
            }

            switch (c)
            {
                case '"':
                    inQuotes = true;
                    fieldStarted = true;
                    break;
                case ',':
                    row.Add(field.ToString());
                    field.Clear();
                    fieldStarted = false;
                    break;
                case '\r':
                    if (i + 1 < text.Length && text[i + 1] == '\n') i++;
                    goto case '\n';
                case '\n':
                    if (fieldStarted || field.Length > 0 || row.Count > 0)
                    {
                        row.Add(field.ToString());
                        rows.Add(row);
                        row = new List<string>();
                    }
                    field.Clear();
                    fieldStarted = false;
                    break;
                default:
                    field.Append(c);
                    fieldStarted = true;
                    break;
            }
        }

        if (fieldStarted || field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }
        return rows;
    }
}
