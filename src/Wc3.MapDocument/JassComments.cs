// src/Wc3.MapDocument/JassComments.cs
using System.Text;

namespace Wc3.Model;

/// <summary>
/// Blanks line comments and block comments out of a JASS script, one array entry per input line,
/// same length and same indices as the input, so a caller can still report 1-based line numbers
/// against the original text. Shared so every script analysis pass (the wiring audit, the runtime
/// readiness check, and any future one) reasons over the same comment-free view instead of each
/// keeping its own copy of this scan.
/// </summary>
public static class JassComments
{
    /// <summary>Strips comments from already-split lines. A line comment drops the rest of its
    /// line, a block comment drops everything up to its closing marker, which may be lines later.</summary>
    public static string[] Strip(string[] lines)
    {
        var result = new string[lines.Length];
        bool inBlock = false;
        for (int i = 0; i < lines.Length; i++)
        {
            var sb = new StringBuilder(lines[i].Length);
            var line = lines[i];
            for (int c = 0; c < line.Length; c++)
            {
                if (inBlock)
                {
                    if (c + 1 < line.Length && line[c] == '*' && line[c + 1] == '/') { inBlock = false; c++; }
                    continue;
                }
                if (c + 1 < line.Length && line[c] == '/' && line[c + 1] == '/') break;
                if (c + 1 < line.Length && line[c] == '/' && line[c + 1] == '*') { inBlock = true; c++; continue; }
                sb.Append(line[c]);
            }
            result[i] = sb.ToString();
        }
        return result;
    }
}
