// src/Wc3.Commands/Lua/LuaLexer.cs
using System.Text;

namespace Wc3.Commands.Lua;

/// <summary>A Lua syntax error, with the line and the message luac 5.3 gives for it.</summary>
public sealed class LuaSyntaxError : Exception
{
    public int Line { get; }
    public string Detail { get; }

    public LuaSyntaxError(int line, string detail) : base($"line {line}: {detail}")
    {
        Line = line;
        Detail = detail;
    }
}

/// <summary>A comment kept from the source, with the line it starts on.</summary>
public sealed record LuaComment(string Text, int Line)
{
    /// <summary>The comment without its dashes and, for a long comment, its brackets.</summary>
    public string Value
    {
        get
        {
            var body = Text[2..];
            int level = LuaLexer.LongOpenerLevel(body, 0);
            return level < 0 ? body : body[(level + 2)..(body.Length - level - 2)];
        }
    }
}

/// <summary>
/// The tokens of a Lua 5.3 chunk, as parallel arrays. Kind is the keyword or operator itself,
/// or NAME, NUMBER, STRING, CHAR (an unknown character), ERROR (the lexer stopped, Text holds
/// the message) and EOF. Three EOF tokens close the stream so the parser can look ahead.
/// </summary>
internal sealed class LuaTokens
{
    public readonly List<string> Kinds = new();
    public readonly List<string> Texts = new();
    public readonly List<int> Lines = new();
    public readonly List<int> Offsets = new();
    public readonly List<LuaComment> Comments = new();
    /// <summary>For each comment, the index of the token that follows it.</summary>
    public readonly List<int> CommentTokens = new();
    /// <summary>Decoded values of quoted strings that held an escape, by token index.</summary>
    public readonly Dictionary<int, string> StringValues = new();
}

/// <summary>
/// Lua 5.3 lexer, ported from lua_ast.py in Devo's Map Doctor (MIT,
/// github.com/devoltzz/devos-map-doctor), which follows llex.c of the reference implementation,
/// error messages included.
///
/// Source text is handled one char per byte (Latin-1), so a byte above 127 survives unchanged,
/// a name can never contain one (as in luac), and offsets are byte offsets into the file.
/// </summary>
public static class LuaLexer
{
    public static readonly IReadOnlySet<string> Keywords = new HashSet<string>(StringComparer.Ordinal)
    {
        "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "goto", "if", "in",
        "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while",
    };

    private static readonly string[] Operators3 = { "..." };
    private static readonly string[] Operators2 = { "..", "==", "~=", "<=", ">=", "<<", ">>", "//", "::" };
    private const string Operators1 = "-+*/%^#&~|<>=(){}[];:,.";

    /// <summary>Encodes source bytes as the one-char-per-byte string the lexer works on.</summary>
    public static string FromBytes(byte[] bytes) => Encoding.Latin1.GetString(bytes);

    internal static bool IsBlank(char c) => c is ' ' or '\t' or '\n' or '\r' or '\f' or '\v';
    internal static bool IsNameStart(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_';
    internal static bool IsNameChar(char c) => IsNameStart(c) || c is >= '0' and <= '9';
    internal static bool IsDigit(char c) => c is >= '0' and <= '9';
    internal static bool IsHex(char c) => IsDigit(c) || c is (>= 'a' and <= 'f') or (>= 'A' and <= 'F');

    /// <summary>Counts line breaks the way Lua does, where \r\n and \n\r are one.</summary>
    internal static int NewlineCount(string s, int start = 0, int end = -1)
    {
        if (end < 0) end = s.Length;
        int n = 0;
        for (int i = start; i < end; i++)
        {
            char c = s[i];
            if (c != '\n' && c != '\r') continue;
            n++;
            if (i + 1 < end && (s[i + 1] == '\n' || s[i + 1] == '\r') && s[i + 1] != c) i++;
        }
        return n;
    }

    /// <summary>The level of a long bracket opener "[==[" at pos, or -1 when there is none.</summary>
    internal static int LongOpenerLevel(string s, int pos)
    {
        if (pos >= s.Length || s[pos] != '[') return -1;
        int j = pos + 1;
        while (j < s.Length && s[j] == '=') j++;
        return j < s.Length && s[j] == '[' ? j - pos - 1 : -1;
    }

    /// <summary>The end (exclusive) of the long bracket closing level, searched from pos, or -1.</summary>
    private static int LongClose(string s, int pos, int level)
    {
        var closer = "]" + new string('=', level) + "]";
        int k = s.IndexOf(closer, pos, StringComparison.Ordinal);
        return k < 0 ? -1 : k + closer.Length;
    }

    internal static string NearText(string s) => "'" + (s.Length <= 40 ? s : s[..37] + "...") + "'";

    internal static LuaTokens Lex(string text)
    {
        var t = new LuaTokens();
        int line = 1, pos = 0, n = text.Length;
        (int Line, string Message)? error = null;

        void Add(string kind, int start, int end)
        {
            t.Kinds.Add(kind);
            t.Texts.Add(text[start..end]);
            t.Lines.Add(line);
            t.Offsets.Add(start);
        }

        while (pos < n)
        {
            char c = text[pos];
            if (IsBlank(c))
            {
                int j = pos;
                while (j < n && IsBlank(text[j])) j++;
                line += NewlineCount(text, pos, j);
                pos = j;
                continue;
            }
            if (IsNameStart(c))
            {
                int j = pos + 1;
                while (j < n && IsNameChar(text[j])) j++;
                var word = text[pos..j];
                Add(Keywords.Contains(word) ? word : "NAME", pos, j);
                pos = j;
                continue;
            }
            if (c == '-' && pos + 1 < n && text[pos + 1] == '-')
            {
                int level = LongOpenerLevel(text, pos + 2);
                int end;
                if (level >= 0)
                {
                    end = LongClose(text, pos + 2 + level + 2, level);
                    if (end < 0)
                    {
                        error = (line + NewlineCount(text, pos),
                            $"unfinished long comment (starting at line {line}) near <eof>");
                        break;
                    }
                }
                else
                {
                    end = pos + 2;
                    while (end < n && text[end] != '\n' && text[end] != '\r') end++;
                }
                t.Comments.Add(new LuaComment(text[pos..end], line));
                t.CommentTokens.Add(t.Kinds.Count);
                line += NewlineCount(text, pos, end);
                pos = end;
                continue;
            }
            if (c == '[')
            {
                int level = LongOpenerLevel(text, pos);
                if (level >= 0)
                {
                    int end = LongClose(text, pos + level + 2, level);
                    if (end < 0)
                    {
                        error = (line + NewlineCount(text, pos),
                            $"unfinished long string (starting at line {line}) near <eof>");
                        break;
                    }
                    Add("STRING", pos, end);
                    line += NewlineCount(text, pos, end);
                    pos = end;
                    continue;
                }
                if (pos + 1 < n && text[pos + 1] == '=')
                {
                    int j = pos + 1;
                    while (j < n && text[j] == '=') j++;
                    error = (line, "invalid long string delimiter near " + NearText(text[pos..j]));
                    break;
                }
                Add("[", pos, pos + 1);
                pos++;
                continue;
            }
            if (IsDigit(c) || (c == '.' && pos + 1 < n && IsDigit(text[pos + 1])))
            {
                int j = ScanNumeral(text, pos);
                var s = text[pos..j];
                if (!IsWellFormedNumber(s))
                {
                    error = (line, "malformed number near " + NearText(s));
                    break;
                }
                Add("NUMBER", pos, j);
                pos = j;
                continue;
            }
            if (c == '"' || c == '\'')
            {
                var (end, closed) = ScanQuoted(text, pos);
                var part = text[pos..end];
                if (!closed)
                {
                    var bad = part.Contains('\\') ? Unescape(part[1..]).Error : null;
                    if (bad is { } b) error = EscapeError(text[pos..], b, line);
                    else
                    {
                        var rest = text[end..];
                        error = rest is "" or "\\"
                            ? (line + NewlineCount(part), "unfinished string near <eof>")
                            : (line + NewlineCount(part), "unfinished string near " + NearText(part));
                    }
                    break;
                }
                if (part.Contains('\\'))
                {
                    var (value, bad) = Unescape(part[1..^1]);
                    if (bad is { } b) { error = EscapeError(text[pos..], b, line); break; }
                    t.StringValues[t.Kinds.Count] = value!;
                }
                Add("STRING", pos, end);
                line += NewlineCount(part);
                pos = end;
                continue;
            }
            int len = OperatorLength(text, pos);
            if (len > 0)
            {
                Add(text.Substring(pos, len), pos, pos + len);
                pos += len;
                continue;
            }
            Add("CHAR", pos, pos + 1);
            pos++;
        }

        if (error is { } e)
        {
            t.Kinds.Add("ERROR");
            t.Texts.Add(e.Message);
            t.Lines.Add(e.Line);
            t.Offsets.Add(n);
            line = e.Line;
        }
        for (int i = 0; i < 3; i++)
        {
            t.Kinds.Add("EOF");
            t.Texts.Add("");
            t.Lines.Add(line);
            t.Offsets.Add(n);
        }
        return t;
    }

    private static int OperatorLength(string text, int pos)
    {
        foreach (var op in Operators3)
            if (string.CompareOrdinal(text, pos, op, 0, op.Length) == 0) return op.Length;
        foreach (var op in Operators2)
            if (string.CompareOrdinal(text, pos, op, 0, op.Length) == 0) return op.Length;
        return Operators1.Contains(text[pos]) ? 1 : 0;
    }

    /// <summary>
    /// The extent of a numeral as llex.c reads it, before it is checked. Hex and decimal digits,
    /// dots, and an exponent letter with an optional sign, so "1..2" and "3e" are read whole and
    /// then refused as malformed.
    /// </summary>
    private static int ScanNumeral(string s, int pos)
    {
        int j = pos;
        bool hex = s[j] == '0' && j + 1 < s.Length && (s[j + 1] == 'x' || s[j + 1] == 'X');
        if (hex) j += 2;
        while (j < s.Length)
        {
            char c = s[j];
            bool expo = hex ? c is 'p' or 'P' : c is 'e' or 'E';
            if (expo)
            {
                j++;
                if (j < s.Length && (s[j] == '+' || s[j] == '-')) j++;
            }
            else if (IsHex(c) || c == '.') j++;
            else break;
        }
        return j;
    }

    private static bool IsWellFormedNumber(string s)
    {
        int i = 0;
        if (s.Length >= 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X'))
        {
            i = 2;
            int digits = 0;
            while (i < s.Length && IsHex(s[i])) { i++; digits++; }
            if (i < s.Length && s[i] == '.')
            {
                i++;
                while (i < s.Length && IsHex(s[i])) { i++; digits++; }
            }
            if (digits == 0) return false;
            if (i < s.Length && (s[i] == 'p' || s[i] == 'P'))
            {
                i++;
                if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
                int e = 0;
                while (i < s.Length && IsDigit(s[i])) { i++; e++; }
                if (e == 0) return false;
            }
            return i == s.Length;
        }
        int d = 0;
        while (i < s.Length && IsDigit(s[i])) { i++; d++; }
        if (i < s.Length && s[i] == '.')
        {
            i++;
            while (i < s.Length && IsDigit(s[i])) { i++; d++; }
        }
        if (d == 0) return false;
        if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
        {
            i++;
            if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
            int e = 0;
            while (i < s.Length && IsDigit(s[i])) { i++; e++; }
            if (e == 0) return false;
        }
        return i == s.Length;
    }

    /// <summary>
    /// Scans a quoted string from its opening quote. Returns the end and whether it closed. An
    /// unclosed string ends before the newline or end of text that stopped it, as the prefix
    /// lua_ast.py measures for its error message.
    /// </summary>
    private static (int End, bool Closed) ScanQuoted(string s, int pos)
    {
        char q = s[pos];
        int j = pos + 1;
        while (j < s.Length)
        {
            char c = s[j];
            if (c == q) return (j + 1, true);
            if (c == '\n' || c == '\r') return (j, false);
            if (c == '\\')
            {
                if (j + 1 >= s.Length) return (j, false);
                char e = s[j + 1];
                if (e == 'z')
                {
                    j += 2;
                    while (j < s.Length && IsBlank(s[j])) j++;
                    continue;
                }
                if (e == '\r' || e == '\n')
                {
                    j += 2;
                    if (j < s.Length && (s[j] == '\n' || s[j] == '\r') && s[j] != e) j++;
                    continue;
                }
                j += 2;
                continue;
            }
            j++;
        }
        return (j, false);
    }

    /// <summary>
    /// An escape error, worded as luac words it. The near text runs through the character luac
    /// was looking at, which for an unfinished string can be the newline after the escape, so
    /// it is cut from the rest of the source and not from the string alone (where lua_ast.py
    /// cuts it, one character short). That last character is not consumed yet, so a newline
    /// there does not count toward the line.
    /// </summary>
    private static (int, string) EscapeError(string rest, (int Index, string Message) bad, int line)
    {
        var part = rest[..Math.Min(rest.Length, bad.Index + 1)];
        return (line + NewlineCount(part, 0, Math.Max(0, part.Length - 1)), bad.Message + " near " + NearText(part));
    }

    /// <summary>
    /// Decodes the escapes of a quoted string body. Error indices follow lua_ast.py, so the
    /// "near" text of an escape error matches luac.
    /// </summary>
    internal static (string? Value, (int Index, string Message)? Error) Unescape(string body)
    {
        var sb = new StringBuilder(body.Length);
        int n = body.Length, pos = 0;
        while (true)
        {
            int i = body.IndexOf('\\', pos);
            if (i < 0 || i + 1 >= n) break;
            sb.Append(body, pos, i - pos);
            char c = body[i + 1];
            int j = i + 2;
            switch (c)
            {
                case 'a': sb.Append('\a'); break;
                case 'b': sb.Append('\b'); break;
                case 'f': sb.Append('\f'); break;
                case 'n': sb.Append('\n'); break;
                case 'r': sb.Append('\r'); break;
                case 't': sb.Append('\t'); break;
                case 'v': sb.Append('\v'); break;
                case '\\': sb.Append('\\'); break;
                case '"': sb.Append('"'); break;
                case '\'': sb.Append('\''); break;
                case '\n':
                case '\r':
                    sb.Append('\n');
                    if (j < n && (body[j] == '\n' || body[j] == '\r') && body[j] != c) j++;
                    break;
                case 'x':
                    for (int k = i + 2; k <= i + 3; k++)
                        if (k >= n || !IsHex(body[k])) return (null, (k + 1, "hexadecimal digit expected"));
                    sb.Append((char)Convert.ToInt32(body.Substring(i + 2, 2), 16));
                    j = i + 4;
                    break;
                case 'z':
                    while (j < n && IsBlank(body[j])) j++;
                    break;
                case >= '0' and <= '9':
                {
                    j = i + 1;
                    int v = 0;
                    while (j < i + 4 && j < n && IsDigit(body[j])) { v = v * 10 + body[j] - '0'; j++; }
                    if (v > 255) return (null, (j + 1, "decimal escape too large"));
                    sb.Append((char)v);
                    break;
                }
                case 'u':
                {
                    if (j >= n || body[j] != '{') return (null, (j + 1, "missing '{'"));
                    j++;
                    if (j >= n || !IsHex(body[j])) return (null, (j + 1, "hexadecimal digit expected"));
                    long v = 0;
                    while (j < n && IsHex(body[j]))
                    {
                        v = v * 16 + Convert.ToInt32(body[j].ToString(), 16);
                        if (v > 0x10FFFF) return (null, (j + 1, "UTF-8 value too large"));
                        j++;
                    }
                    if (j >= n || body[j] != '}') return (null, (j + 1, "missing '}'"));
                    AppendUtf8(sb, (int)v);
                    j++;
                    break;
                }
                default:
                    return (null, (j, "invalid escape sequence"));
            }
            pos = j;
        }
        sb.Append(body, pos, n - pos);
        return (sb.ToString(), null);
    }

    /// <summary>Appends the UTF-8 bytes of a code point, one char per byte, as luaO_utf8esc does.</summary>
    private static void AppendUtf8(StringBuilder sb, int x)
    {
        if (x < 0x80) { sb.Append((char)x); return; }
        var outBytes = new List<int>();
        int mfb = 0x3f;
        while (true)
        {
            outBytes.Add(0x80 | (x & 0x3f));
            x >>= 6;
            mfb >>= 1;
            if (x <= mfb) break;
        }
        outBytes.Add(((~mfb << 1) | x) & 0xff);
        for (int k = outBytes.Count - 1; k >= 0; k--) sb.Append((char)outBytes[k]);
    }

    /// <summary>The value of a long string, without its first newline, newlines normalised to \n.</summary>
    internal static string LongValue(string text)
    {
        int level = text.IndexOf('[', 1) - 1;
        var body = text[(level + 2)..(text.Length - level - 2)];
        if (body.Length > 0 && (body[0] == '\n' || body[0] == '\r'))
        {
            int skip = 1;
            if (body.Length > 1 && (body[1] == '\n' || body[1] == '\r') && body[1] != body[0]) skip = 2;
            body = body[skip..];
        }
        if (!body.Contains('\r')) return body;
        var sb = new StringBuilder(body.Length);
        for (int i = 0; i < body.Length; i++)
        {
            char c = body[i];
            if (c == '\r' || c == '\n')
            {
                sb.Append('\n');
                if (i + 1 < body.Length && (body[i + 1] == '\n' || body[i + 1] == '\r') && body[i + 1] != c) i++;
            }
            else sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>The value of a numeral, a long (integer) or a double (float), as Lua 5.3 reads it.</summary>
    internal static object NumberValue(string s)
    {
        if (s.Length >= 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X'))
        {
            if (s.Contains('.') || s.Contains('p') || s.Contains('P')) return HexFloat(s[2..]);
            ulong v = 0;
            foreach (char c in s[2..]) v = unchecked(v * 16 + (ulong)Convert.ToInt32(c.ToString(), 16));
            return unchecked((long)v);
        }
        if (s.All(IsDigit))
        {
            var digits = s.TrimStart('0');
            if (digits.Length == 0) return 0L;
            if (digits.Length <= 19 && ulong.TryParse(digits, out var u) && u < 1UL << 63) return (long)u;
        }
        return double.Parse(s, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture);
    }

    private static double HexFloat(string body)
    {
        int p = body.IndexOfAny(new[] { 'p', 'P' });
        string mant = p < 0 ? body : body[..p];
        int exp = p < 0 ? 0 : int.Parse(body[(p + 1)..], System.Globalization.CultureInfo.InvariantCulture);
        double v = 0;
        int dot = mant.IndexOf('.');
        string whole = dot < 0 ? mant : mant[..dot], frac = dot < 0 ? "" : mant[(dot + 1)..];
        foreach (char c in whole) v = v * 16 + Convert.ToInt32(c.ToString(), 16);
        double scale = 1.0 / 16;
        foreach (char c in frac) { v += Convert.ToInt32(c.ToString(), 16) * scale; scale /= 16; }
        return v * Math.Pow(2, exp);
    }
}
