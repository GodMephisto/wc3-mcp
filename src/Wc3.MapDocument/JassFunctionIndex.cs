// src/Wc3.MapDocument/JassFunctionIndex.cs
namespace Wc3.Model;

public sealed record JassFunction(string Name, int StartLine, int EndLine, string Signature);

public static class JassFunctionIndex
{
    // Lists the functions declared in a JASS/vJASS script. Pure text scan.
    // A function starts at a line whose first token is "function" (optionally preceded
    // by "constant") followed by a name, and ends at the next line that is "endfunction"
    // once any trailing line comment is stripped (JASS functions do not nest).
    // Line-comment lines ("//...") are skipped, "function interface" (vJASS) is not a function.
    public static IReadOnlyList<JassFunction> Parse(string jass)
    {
        var result = new List<JassFunction>();
        if (string.IsNullOrEmpty(jass)) return result;

        // JassLines, not Split on a newline. 13 of 34 maps measured in the user's Maps
        // folder separate their script with a BARE CARRIAGE RETURN, and every one of them
        // reported zero functions here. See JassLines for the measurement.
        var lines = JassLines.Split(jass);
        string? name = null;
        int startLine = 0;
        string signature = "";
        for (int i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();
            if (trimmed.StartsWith("//", StringComparison.Ordinal)) continue;
            // Strip a trailing line comment so "endfunction // note" still closes the body
            // and start lines tokenize cleanly.
            var commentAt = trimmed.IndexOf("//", StringComparison.Ordinal);
            if (commentAt >= 0) trimmed = trimmed[..commentAt].TrimEnd();
            if (name is null)
            {
                var tokens = trimmed.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                var fn = tokens.Length > 0 && tokens[0] == "constant" ? 1 : 0;   // optional "constant function"
                if (tokens.Length < fn + 2 || tokens[fn] != "function") continue;
                if (tokens[fn + 1] == "interface") continue;   // vJASS function interface, no body
                name = tokens[fn + 1];
                startLine = i + 1;
                signature = trimmed;
            }
            else if (trimmed == "endfunction")
            {
                result.Add(new JassFunction(name, startLine, i + 1, signature));
                name = null;
            }
        }
        return result;
    }
}
