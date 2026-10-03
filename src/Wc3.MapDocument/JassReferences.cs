// src/Wc3.MapDocument/JassReferences.cs
namespace Wc3.Model;

/// <summary>
/// One place a name is used. <paramref name="Line"/> is 1-based.
/// <paramref name="InFunction"/> is the function whose body contains it, null at file scope
/// (a globals block, or a bare declaration between functions).
/// </summary>
public sealed record JassReference(int Line, string? InFunction, string Text, JassReferenceKind Kind);

/// <summary>What kind of use a reference is.</summary>
public enum JassReferenceKind
{
    /// <summary>The <c>function X takes ...</c> that declares it.</summary>
    Declaration,
    /// <summary>A call, <c>call X(...)</c> or <c>X(...)</c> inside an expression.</summary>
    Call,
    /// <summary>The name used without parentheses, which for a function means it is being passed
    /// as <c>code</c>, to Condition, Filter, TriggerAddAction and the like.</summary>
    CodeReference,
}

/// <summary>
/// Finds every use of a name in a script. "Who calls this" for a merged map script.
/// </summary>
/// <remarks>
/// The question worth answering on this project's real inputs. A merged arena script runs to 3,009
/// functions over 113,387 lines, and reading one means asking what reaches it far more often than
/// asking where it is declared. Go to definition was the easy half.
///
/// Three kinds, and the third is the one a plain text search gets wrong. JASS passes a function as
/// a <c>code</c> value by naming it without parentheses, so <c>Condition(function Foo)</c> and
/// <c>TriggerAddAction(t, function Foo)</c> are how most handlers are actually reached. A search
/// for "Foo(" misses every one of them, which on a trigger-driven map is most of the answer.
///
/// Comments are stripped before matching, through the same <see cref="JassComments"/> pass the
/// other analyses use, so a commented-out call is not reported as a live one. The TEXT returned is
/// the original line, comment and all, because that is what a reader wants to see.
/// </remarks>
public static class JassReferences
{
    /// <summary>
    /// Every use of <paramref name="name"/>, in line order. An empty or non-identifier name yields
    /// nothing rather than matching everything.
    /// </summary>
    public static IReadOnlyList<JassReference> Find(string jass, string name)
    {
        var found = new List<JassReference>();
        if (string.IsNullOrEmpty(jass) || !IsIdentifier(name)) return found;

        var lines = JassLines.Split(jass);
        var code = JassComments.Strip(lines);
        var functions = JassFunctionIndex.Parse(jass);

        // Line to owning function, built once. A linear scan per hit would be quadratic on a
        // script with three thousand functions.
        var owner = new string?[lines.Length + 1];
        foreach (var fn in functions)
            for (int line = fn.StartLine; line <= fn.EndLine && line < owner.Length; line++)
                owner[line] = fn.Name;

        for (int i = 0; i < code.Length; i++)
        {
            var text = code[i];
            if (text.Length == 0) continue;

            foreach (var at in Occurrences(text, name))
            {
                var kind = Classify(text, at, name.Length);
                int lineNumber = i + 1;
                found.Add(new JassReference(
                    lineNumber,
                    owner.Length > lineNumber ? owner[lineNumber] : null,
                    i < lines.Length ? lines[i].Trim() : text.Trim(),
                    kind));
            }
        }
        return found;
    }

    /// <summary>Uses that are not the declaration, which is what "find references" usually means.</summary>
    public static IReadOnlyList<JassReference> FindUses(string jass, string name) =>
        Find(jass, name).Where(r => r.Kind != JassReferenceKind.Declaration).ToList();

    /// <summary>
    /// Whole-word occurrences of <paramref name="name"/> in a line. Whole-word matters, or
    /// "Foo" reports every hit inside "FooBar" and "MyFoo".
    /// </summary>
    private static IEnumerable<int> Occurrences(string text, string name)
    {
        int from = 0;
        while (from <= text.Length - name.Length)
        {
            int at = text.IndexOf(name, from, StringComparison.Ordinal);
            if (at < 0) break;
            from = at + 1;

            bool leftClear = at == 0 || !IsWordChar(text[at - 1]);
            int after = at + name.Length;
            bool rightClear = after >= text.Length || !IsWordChar(text[after]);
            if (leftClear && rightClear) yield return at;
        }
    }

    private static JassReferenceKind Classify(string text, int at, int length)
    {
        // Preceded by "function" means either the declaration or a code reference. The difference
        // is whether the line STARTS the declaration, since "function Foo takes" opens a body and
        // "Condition(function Foo)" does not.
        var before = text[..at].TrimEnd();
        if (before.EndsWith("function", StringComparison.Ordinal)
            && (before.Length == "function".Length
                || !IsWordChar(before[^("function".Length + 1)])))
        {
            var head = text.TrimStart();
            bool declares = head.StartsWith("function ", StringComparison.Ordinal)
                            || head.StartsWith("constant function ", StringComparison.Ordinal);
            return declares ? JassReferenceKind.Declaration : JassReferenceKind.CodeReference;
        }

        // Followed by an open paren, allowing whitespace, is a call.
        int after = at + length;
        while (after < text.Length && (text[after] == ' ' || text[after] == '\t')) after++;
        if (after < text.Length && text[after] == '(') return JassReferenceKind.Call;

        return JassReferenceKind.CodeReference;
    }

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';

    private static bool IsIdentifier(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        if (!char.IsLetter(s[0]) && s[0] != '_') return false;
        foreach (var c in s)
            if (!IsWordChar(c)) return false;
        return true;
    }
}
