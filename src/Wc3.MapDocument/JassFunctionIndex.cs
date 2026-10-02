// src/Wc3.MapDocument/JassFunctionIndex.cs
namespace Wc3.Model;

public sealed record JassFunction(string Name, int StartLine, int EndLine, string Signature);

public static class JassFunctionIndex
{
    // Lists the functions declared in a JASS/vJASS script. Pure text scan:
    // a function starts at a line whose first token is "function" followed by a name,
    // and ends at the next line that is exactly "endfunction" (JASS functions do not nest).
    // Line-comment lines ("//...") are skipped; "function interface" (vJASS) is not a function.
    public static IReadOnlyList<JassFunction> Parse(string jass)
    {
        var result = new List<JassFunction>();
        if (string.IsNullOrEmpty(jass)) return result;

        var lines = jass.Split('\n');
        string? name = null;
        int startLine = 0;
        string signature = "";
        for (int i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();
            if (trimmed.StartsWith("//", StringComparison.Ordinal)) continue;
            if (name is null)
            {
                var tokens = trimmed.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 2 || tokens[0] != "function") continue;
                if (tokens[1] == "interface") continue;   // vJASS function interface, no body
                name = tokens[1];
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
