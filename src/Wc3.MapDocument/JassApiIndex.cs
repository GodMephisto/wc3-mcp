namespace Wc3.Model;

/// <summary>One parameter of a native or function, as declared.</summary>
public sealed record JassParameter(string Type, string Name);

/// <summary><c>type NAME extends PARENT</c>.</summary>
public sealed record JassTypeDecl(string Name, string Parent);

/// <summary><c>[constant] native NAME takes ... returns ...</c>.</summary>
public sealed record JassNativeDecl(string Name, IReadOnlyList<JassParameter> Parameters, string Returns, bool IsConstant);

/// <summary><c>[constant] function NAME takes ... returns ...</c>, the header only.</summary>
public sealed record JassFunctionDecl(string Name, IReadOnlyList<JassParameter> Parameters, string Returns, bool IsConstant);

/// <summary>A variable from a <c>globals</c> block. <see cref="Value"/> is the initialiser text, or null.</summary>
public sealed record JassGlobalDecl(string Type, string Name, bool IsConstant, bool IsArray, string? Value);

/// <summary>Everything a script file declares at the top level.</summary>
public sealed record JassApi(
    IReadOnlyList<JassTypeDecl> Types,
    IReadOnlyList<JassNativeDecl> Natives,
    IReadOnlyList<JassGlobalDecl> Globals,
    IReadOnlyList<JassFunctionDecl> Functions);

/// <summary>
/// Reads the declarations of a JASS file, the shape of <c>common.j</c> and <c>blizzard.j</c>:
/// types, natives, globals and function headers. It is what decides whether a map's script
/// compiles against a given game build, since a native or constant the build lacks is a compile
/// error there. Function bodies are skipped, <see cref="JassFunctionIndex"/> covers them.
/// </summary>
public static class JassApiIndex
{
    public static JassApi Parse(string jass)
    {
        var types = new List<JassTypeDecl>();
        var natives = new List<JassNativeDecl>();
        var globals = new List<JassGlobalDecl>();
        var functions = new List<JassFunctionDecl>();
        bool inGlobals = false, inFunction = false;

        foreach (var raw in jass.Split('\n'))
        {
            var line = StripComment(raw).Trim();
            if (line.Length == 0) continue;
            var tokens = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            if (inFunction)
            {
                if (tokens[0] == "endfunction") inFunction = false;
                continue;
            }
            if (tokens[0] == "globals") { inGlobals = true; continue; }
            if (tokens[0] == "endglobals") { inGlobals = false; continue; }

            if (inGlobals)
            {
                if (ParseGlobal(line) is { } g) globals.Add(g);
                continue;
            }

            int at = tokens[0] == "constant" ? 1 : 0;
            if (tokens.Length > at + 1 && tokens[at] == "native")
            {
                if (ParseHeader(line, "native") is var (name, ps, ret)) natives.Add(new(name, ps, ret, at == 1));
            }
            else if (tokens.Length > at + 1 && tokens[at] == "function")
            {
                if (ParseHeader(line, "function") is var (name, ps, ret)) functions.Add(new(name, ps, ret, at == 1));
                inFunction = true;
            }
            else if (tokens[0] == "type" && tokens.Length >= 4 && tokens[2] == "extends")
            {
                types.Add(new(tokens[1], tokens[3]));
            }
        }
        return new JassApi(types, natives, globals, functions);
    }

    /// <summary>Removes a <c>//</c> comment, leaving one that sits inside a string literal alone.</summary>
    private static string StripComment(string line)
    {
        bool inString = false;
        for (int i = 0; i < line.Length - 1; i++)
        {
            if (line[i] == '\\' && inString) { i++; continue; }
            if (line[i] == '"') inString = !inString;
            else if (!inString && line[i] == '/' && line[i + 1] == '/') return line[..i];
        }
        return line;
    }

    /// <summary>Tokenise on whitespace (spaces, tabs), then find keyword / takes / returns
    /// by index so the line may be tab or space aligned.</summary>
    private static (string Name, IReadOnlyList<JassParameter> Parameters, string Returns)? ParseHeader(string line, string keyword)
    {
        var tokens = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int kwIdx = -1, takesIdx = -1, returnsIdx = -1;
        for (int i = 0; i < tokens.Length; i++)
        {
            if (kwIdx < 0 && tokens[i] == keyword) { kwIdx = i; }
            else if (kwIdx >= 0 && takesIdx < 0 && tokens[i] == "takes") { takesIdx = i; }
            else if (takesIdx >= 0 && tokens[i] == "returns") { returnsIdx = i; break; }
        }
        if (kwIdx < 0 || takesIdx < 0 || returnsIdx <= takesIdx) return null;
        // name is tokens[kwIdx+1 .. takesIdx-1]
        var nameTokens = tokens[(kwIdx + 1)..takesIdx];
        var name = string.Join(" ", nameTokens);
        // collect parameter tokens from takesIdx+1 .. returnsIdx-1
        var paramTokens = tokens[(takesIdx + 1)..returnsIdx];
        var paramText = string.Join(" ", paramTokens);
        var ret = string.Join(" ", tokens[(returnsIdx + 1)..]);
        var ps = new List<JassParameter>();
        if (paramText != "nothing")
            foreach (var p in paramText.Split(','))
            {
                var parts = p.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2) ps.Add(new(parts[0], parts[1]));
            }
        return (name, ps, ret);
    }

    private static JassGlobalDecl? ParseGlobal(string line)
    {
        bool isConstant = line.StartsWith("constant ", StringComparison.Ordinal);
        var rest = isConstant ? line[9..].TrimStart() : line;
        string? value = null;
        int eq = rest.IndexOf('=');
        if (eq >= 0) { value = rest[(eq + 1)..].Trim(); rest = rest[..eq].Trim(); }
        var parts = rest.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 3 && parts[1] == "array") return new(parts[0], parts[2], isConstant, true, value);
        if (parts.Length == 2) return new(parts[0], parts[1], isConstant, false, value);
        return null;
    }
}
