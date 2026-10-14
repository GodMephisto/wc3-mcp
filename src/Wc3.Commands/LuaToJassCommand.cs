// src/Wc3.Commands/LuaToJassCommand.cs
using System.Globalization;
using System.Text;
using Wc3.Commands.Lua;
using Wc3.Model;
using War3Net.Build.Info;

namespace Wc3.Commands;

/// <summary>A construct the Lua to JASS conversion could not turn back into JASS.</summary>
public sealed record LuaToJassUnconverted(string Construct, int Line, string Detail);

/// <summary>What a Lua to JASS conversion produced. <paramref name="Jass"/> is the whole war3map.j
/// text, which may hold only the confident part of the map's script.</summary>
public sealed record LuaToJassResult(
    string ScriptFile,
    bool Ok,
    int FunctionsConverted,
    int GlobalsConverted,
    int FunctionsSkipped,
    string Jass,
    IReadOnlyList<LuaToJassUnconverted> Unconverted,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Converts a Lua map back to JASS, best effort. The JASS to Lua conversion is not reversible in
/// general, because Lua carries types it does not spell and the runtime helpers hide the JASS rules,
/// so this converts only the constructs that map back without guessing, and reports every construct
/// it left rather than emitting JASS that would not compile or would run differently.
///
/// What is exact, the JASS to Lua runtime helpers are inverted. Their call shape is fixed by the
/// converter, so <c>__iadd(a, b)</c> reads back as <c>a + b</c>, <c>__idiv</c> as <c>a / b</c>,
/// <c>__cat</c> as <c>a + b</c>, and so on, and a literal keeps its spelling.
///
/// What is best effort. A function becomes JASS only when its body converts whole, because JASS has
/// no untyped local and no partially right function. A function with parameters, a value returning
/// return, a closure, a table, a method call, a generic for, a repeat, a goto or a label is reported
/// and left out, not approximated. A global is emitted only when its value is a literal, since JASS
/// needs the type and only a literal gives one. A call to a name the map does not declare is emitted
/// and named in the diagnostics, so the reader can check it against the target build.
/// </summary>
public static class LuaToJassCommand
{
    /// <summary>The JASS spelling of the helper names the JASS to Lua prelude introduces. The value
    /// is the JASS operator that helper stands for, or null when it is a plain function.</summary>
    private static readonly Dictionary<string, string> HelperOperators = new(StringComparer.Ordinal)
    {
        ["__iadd"] = "+", ["__isub"] = "-", ["__imul"] = "*", ["__idiv"] = "/", ["__div"] = "/",
        // The strict-real helpers stand for the same operators. The rounding they add is what the
        // JASS runtime does anyway, so the operator is the exact JASS spelling.
        ["__fadd"] = "+", ["__fsub"] = "-", ["__fmul"] = "*", ["__fdiv"] = "/",
    };

    /// <summary>
    /// The native a rewritten hashtable helper names back, or null when the name is not one of
    /// them. The JASS to Lua direction puts the stored type in the helper's suffix, so SaveInteger
    /// and SaveReal come back as themselves rather than as a guess, and every handle kind keeps
    /// its own name.
    /// </summary>
    private static string? HashtableCall(string name, List<string> args)
    {
        if (name == "__hs_flush_child" && args.Count == 2)
            return $"FlushChildHashtable({args[0]}, {args[1]})";
        if (name == "__hs_flush" && args.Count == 1)
            return $"FlushParentHashtable({args[0]})";

        (string Verb, int Arity)? shape =
            name.StartsWith("__hs_save_", StringComparison.Ordinal) ? ("Save", 4) :
            name.StartsWith("__hs_load_", StringComparison.Ordinal) ? ("Load", 3) :
            name.StartsWith("__hs_has_", StringComparison.Ordinal) ? ("HaveSaved", 3) :
            name.StartsWith("__hs_remove_", StringComparison.Ordinal) ? ("RemoveSaved", 3) :
            null;
        if (shape is not { } s || args.Count < s.Arity) return null;

        string type = name[(name.LastIndexOf('_') + 1)..];
        string? nativeSuffix = JassToLua.HashtableStore
            .FirstOrDefault(kv => kv.Value == type).Key;
        if (nativeSuffix is null) return null;
        // A load drops its default, which the JASS native does not take.
        return $"{s.Verb}{nativeSuffix}({string.Join(", ", args.Take(s.Arity))})";
    }

    /// <summary>
    /// The locals in a function body that hold a hashtable. The JASS to Lua direction turns
    /// CreateHashtable into an empty table, so the reverse has to recognise it again, and it only
    /// does when the local is declared as an empty table and every reference to its name is the
    /// first argument of a __hs_ helper. A real Lua table used as an array is left alone.
    /// </summary>
    private static HashSet<string> HashtableLocals(LuaBlock body)
    {
        var declared = new List<string>();
        var refs = new Dictionary<string, int>(StringComparer.Ordinal);
        var handleRefs = new Dictionary<string, int>(StringComparer.Ordinal);
        Collect(body, declared, refs, handleRefs);

        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var name in declared)
            if (refs.GetValueOrDefault(name) > 0 && refs.GetValueOrDefault(name) == handleRefs.GetValueOrDefault(name))
                result.Add(name);
        return result;
    }

    private static void Collect(LuaBlock block, List<string> declared,
        Dictionary<string, int> refs, Dictionary<string, int> handleRefs)
    {
        foreach (var s in block) Collect(s, declared, refs, handleRefs);
    }

    private static void Collect(LuaStmt s, List<string> declared,
        Dictionary<string, int> refs, Dictionary<string, int> handleRefs)
    {
        switch (s)
        {
            case LocalStmt l:
                if (l.Names.Count == 1 && l.Values.Count == 1 && l.Values[0] is TableExpr { Fields.Count: 0 })
                    declared.Add(l.Names[0]);
                foreach (var v in l.Values) Collect(v, declared, refs, handleRefs);
                break;
            case AssignStmt a:
                foreach (var t in a.Targets) Collect(t, declared, refs, handleRefs);
                foreach (var v in a.Values) Collect(v, declared, refs, handleRefs);
                break;
            case CallStmt c: Collect(c.Call, declared, refs, handleRefs); break;
            case FunctionStmt f: Collect(f.Body, declared, refs, handleRefs); break;
            case IfStmt i:
                foreach (var (cond, blk) in i.Branches)
                {
                    if (cond is not null) Collect(cond, declared, refs, handleRefs);
                    Collect(blk, declared, refs, handleRefs);
                }
                break;
            case WhileStmt w:
                Collect(w.Cond, declared, refs, handleRefs);
                Collect(w.Body, declared, refs, handleRefs);
                break;
            case NumericForStmt nf:
                Collect(nf.Start, declared, refs, handleRefs);
                Collect(nf.Stop, declared, refs, handleRefs);
                if (nf.Step is not null) Collect(nf.Step, declared, refs, handleRefs);
                Collect(nf.Body, declared, refs, handleRefs);
                break;
            case GenericForStmt gf:
                foreach (var e in gf.Exprs) Collect(e, declared, refs, handleRefs);
                Collect(gf.Body, declared, refs, handleRefs);
                break;
            case RepeatStmt r:
                Collect(r.Body, declared, refs, handleRefs);
                Collect(r.Cond, declared, refs, handleRefs);
                break;
            case DoStmt d: Collect(d.Body, declared, refs, handleRefs); break;
            case ReturnStmt r: foreach (var v in r.Values) Collect(v, declared, refs, handleRefs); break;
            default: break;   // break, goto and label carry no reference
        }
    }

    private static void Collect(LuaExpr e, List<string> declared,
        Dictionary<string, int> refs, Dictionary<string, int> handleRefs)
    {
        switch (e)
        {
            case NameExpr n:
                refs[n.Name] = refs.GetValueOrDefault(n.Name) + 1;
                break;
            case CallExpr c:
                // A __hs_ helper's first argument is a handle position, not an ordinary use, so it
                // counts in both tallies. Every other use counts only as a plain reference, which
                // is what makes a name that also appears elsewhere fail the check.
                if (LuaScriptReader.CalleeName(c.Func) is { } callee
                    && callee.StartsWith("__hs_", StringComparison.Ordinal)
                    && c.Args.Count > 0 && c.Args[0] is NameExpr h)
                {
                    refs[h.Name] = refs.GetValueOrDefault(h.Name) + 1;
                    handleRefs[h.Name] = handleRefs.GetValueOrDefault(h.Name) + 1;
                    for (int i = 1; i < c.Args.Count; i++) Collect(c.Args[i], declared, refs, handleRefs);
                    return;
                }
                Collect(c.Func, declared, refs, handleRefs);
                foreach (var a in c.Args) Collect(a, declared, refs, handleRefs);
                break;
            case IndexExpr ix:
                Collect(ix.Base, declared, refs, handleRefs);
                Collect(ix.Key, declared, refs, handleRefs);
                break;
            case FunctionExpr fe: Collect(fe.Body, declared, refs, handleRefs); break;
            case UnaryExpr u: Collect(u.Operand, declared, refs, handleRefs); break;
            case BinaryExpr b:
                Collect(b.Left, declared, refs, handleRefs);
                Collect(b.Right, declared, refs, handleRefs);
                break;
            case ParenExpr p: Collect(p.Inner, declared, refs, handleRefs); break;
            case TableExpr t:
                foreach (var (k, v) in t.Fields)
                {
                    if (k is LuaExpr ke) Collect(ke, declared, refs, handleRefs);
                    Collect(v, declared, refs, handleRefs);
                }
                break;
            default: break;   // literals
        }
    }

    public static LuaToJassResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var (file, source, isLua) = LuaScriptReader.Read(doc);
        if (!isLua)
            return new(file, false, 0, 0, 0, "", Array.Empty<LuaToJassUnconverted>(),
                new[] { $"{file} is a JASS script, nothing to convert." });
        var chunk = LuaScriptReader.TryParse(source, out var error);
        if (chunk is null)
            return new(file, false, 0, 0, 0, "", Array.Empty<LuaToJassUnconverted>(),
                new[] { $"{file} does not parse, line {error!.Line}. {error.Detail}" });

        var translator = new Translator(chunk);
        translator.Run();
        string jass = translator.Text;

        // One entry stub keeps the map hostable and is what the game calls, so the request says the
        // conversion is partial when neither is present.
        var diagnostics = translator.Diagnostics;
        if (!translator.SawMain && !translator.SawConfig)
            diagnostics.Add("the converted JASS has no main or config, both of which the game calls");

        string jassName = file.Contains('\\') ? @"scripts\war3map.j" : "war3map.j";
        doc.AddOrReplaceRawFile(jassName, ScriptCommand.ScriptEncoding.GetBytes(jass));
        if (doc.GetFile("war3map.w3i")?.Model is MapInfo info)
        {
            info.ScriptLanguage = ScriptLanguage.Jass;
            var w3i = doc.AddOrReplaceRawFile("war3map.w3i", MapInfoCommand.Serialize(info));
            w3i.Model = info;
        }

        return new LuaToJassResult(
            file, true, translator.FunctionsConverted, translator.GlobalsConverted, translator.FunctionsSkipped,
            jass, translator.Unconverted, diagnostics);
    }

    private sealed class Translator(LuaChunk chunk)
    {
        private readonly StringBuilder _sb = new();
        private readonly Dictionary<string, int> _funcArity = new(StringComparer.Ordinal);
        private readonly HashSet<string> _globals = new(StringComparer.Ordinal);
        private readonly Dictionary<string, string> _globalTypes = new(StringComparer.Ordinal);
        /// <summary>The locals of the function being converted that hold a rewritten hashtable, so
        /// its empty-table declaration is emitted as CreateHashtable. Set per function.</summary>
        private HashSet<string> _hashtableLocals = new(StringComparer.Ordinal);

        public string Text = "";
        public int FunctionsConverted;
        public int GlobalsConverted;
        public int FunctionsSkipped;
        public bool SawMain;
        public bool SawConfig;
        public readonly List<LuaToJassUnconverted> Unconverted = new();
        public readonly List<string> Diagnostics = new();

        public void Run()
        {
            CollectDeclarations();
            var globals = new StringBuilder();
            CollectGlobals(globals);
            var body = new StringBuilder();
            foreach (var s in chunk.Body)
                ConvertTopLevel(s, body);

            _sb.Append("globals\n").Append(globals);
            if (globals.Length > 0) _sb.Append('\n');
            _sb.Append("endglobals\n\n").Append(body);
            Text = _sb.ToString();
        }

        private void Report(string construct, int line, string detail) =>
            Unconverted.Add(new LuaToJassUnconverted(construct, line, detail));

        private void CollectDeclarations()
        {
            foreach (var s in chunk.Body)
            {
                if (s is FunctionStmt f)
                    _funcArity[f.Name] = f.Params.Count;
                else if (s is AssignStmt { Targets.Count: 1, Values.Count: 1 } a && a.Values[0] is FunctionExpr fe
                         && a.Targets[0] is NameExpr n1)
                    _funcArity[n1.Name] = fe.Params.Count;
                else if (s is LocalStmt { Names.Count: 1, Values.Count: 1 } l && l.Values[0] is FunctionExpr lfe)
                    _funcArity[l.Names[0]] = lfe.Params.Count;
            }
        }

        private void CollectGlobals(StringBuilder globals)
        {
            foreach (var s in chunk.Body)
            {
                string? name = null;
                LuaExpr? value = null;
                if (s is AssignStmt { Targets.Count: 1, Values.Count: 1 } a && a.Targets[0] is NameExpr an)
                {
                    name = an.Name; value = a.Values[0];
                }
                else if (s is LocalStmt { Names.Count: 1, Values.Count: 1 } l)
                {
                    name = l.Names[0]; value = l.Values[0];
                }
                if (name is null || value is null) continue;
                if (_funcArity.ContainsKey(name)) continue;   // a function, emitted as a function
                string? type = LiteralType(value);
                if (type is null)
                {
                    Report("global", s.Line, $"'{name}' has no literal initializer, so its JASS type is unknown");
                    continue;
                }
                string literal = Literal(value)!;
                _globals.Add(name);
                _globalTypes[name] = type;
                globals.Append(type).Append(' ').Append(name).Append(" = ").Append(literal).Append('\n');
                GlobalsConverted++;
            }
        }

        private void ConvertTopLevel(LuaStmt s, StringBuilder body)
        {
            switch (s)
            {
                case FunctionStmt f:
                    EmitFunction(f.Name, f.Params, f.IsVararg, false, f.Body, f.Line, body);
                    break;
                case AssignStmt { Targets.Count: 1, Values.Count: 1 } a when a.Values[0] is FunctionExpr fe
                    && a.Targets[0] is NameExpr n:
                    EmitFunction(n.Name, fe.Params, fe.IsVararg, false, fe.Body, s.Line, body);
                    break;
                case LocalStmt { Names.Count: 1, Values.Count: 1 } l when l.Values[0] is FunctionExpr le:
                    EmitFunction(l.Names[0], le.Params, le.IsVararg, true, le.Body, s.Line, body);
                    break;
                case AssignStmt or LocalStmt:
                    break;   // a global, already in the globals block
                default:
                    Report("statement", s.Line, "a top level statement that is not a declaration");
                    break;
            }
        }

        private void EmitFunction(string name, List<string> pars, bool vararg, bool local, LuaBlock fnBody, int line, StringBuilder body)
        {
            if (name is "main") SawMain = true;
            if (name is "config") SawConfig = true;
            if (pars.Count > 0 || vararg)
            {
                FunctionsSkipped++;
                Report("function", line, $"'{name}' has parameters or is variadic, which Lua does not type, so develop a JASS header by hand");
                return;
            }
            if (name.Contains('.') || name.Contains(':'))
            {
                FunctionsSkipped++;
                Report("function", line, $"'{name}' is a method or a field, which JASS has no form for");
                return;
            }
            var locals = new HashSet<string>(StringComparer.Ordinal);
            _hashtableLocals = HashtableLocals(fnBody);
            var converted = new StringBuilder();
            if (!ConvertBlock(fnBody, locals, converted, 1))
            {
                FunctionsSkipped++;
                return;
            }
            body.Append("// @lua ").Append(line).Append('\n');
            body.Append("function ").Append(name).Append(" takes nothing returns nothing\n");
            body.Append(converted);
            body.Append("endfunction\n\n");
            FunctionsConverted++;
        }

        /// <summary>Converts a block, returning false when a statement could not be converted. On a
        /// false return nothing from the block should be kept, so the whole function is dropped.</summary>
        private bool ConvertBlock(LuaBlock block, HashSet<string> locals, StringBuilder output, int depth)
        {
            string ind = new(' ', 4 * depth);
            int mark = output.Length;
            foreach (var s in block)
                if (!ConvertStmt(s, locals, output, depth))
                {
                    output.Length = mark;
                    return false;
                }
            return true;
        }

        private bool ConvertStmt(LuaStmt s, HashSet<string> locals, StringBuilder output, int depth)
        {
            string ind = new(' ', 4 * depth);
            switch (s)
            {
                case CallStmt c:
                {
                    var expr = Expr(c.Call, locals);
                    if (expr is null) { Report("call", s.Line, "this call is not a plain native or map function call"); return false; }
                    output.Append(ind).Append("call ").Append(expr).Append('\n');
                    return true;
                }
                case LocalStmt l:
                    if (l.Names.Count != 1 || l.Values.Count != 1) { Report("local", s.Line, "a local with more than one name or value"); return false; }
                    // A rewritten hashtable comes back as the CreateHashtable call it started as,
                    // which is the only way the round trip closes for a hashtable script.
                    if (l.Values[0] is TableExpr { Fields.Count: 0 } && _hashtableLocals.Contains(l.Names[0]))
                    {
                        locals.Add(l.Names[0]);
                        output.Append(ind).Append("local hashtable ").Append(l.Names[0])
                            .Append(" = CreateHashtable()\n");
                        return true;
                    }
                    var type = InferType(l.Values[0]);
                    if (type is null) { Report("local", s.Line, $"local '{l.Names[0]}' has no literal initializer, so its JASS type is unknown"); return false; }
                    var localValue = Expr(l.Values[0], locals);
                    if (localValue is null) { Report("local", s.Line, $"local '{l.Names[0]}' has an initializer that is not convertible"); return false; }
                    locals.Add(l.Names[0]);
                    output.Append(ind).Append("local ").Append(type).Append(' ').Append(l.Names[0])
                        .Append(" = ").Append(localValue).Append('\n');
                    return true;
                case AssignStmt a:
                {
                    if (a.Targets.Count != 1 || a.Values.Count != 1 || a.Targets[0] is not NameExpr t)
                    { Report("assignment", s.Line, "an assignment to anything but one plain name"); return false; }
                    if (!locals.Contains(t.Name) && !_globals.Contains(t.Name))
                    { Report("assignment", s.Line, $"an assignment to '{t.Name}', which is neither a local nor a converted global"); return false; }
                    var value = Expr(a.Values[0], locals);
                    if (value is null) { Report("assignment", s.Line, "the assigned expression is not convertible"); return false; }
                    output.Append(ind).Append("set ").Append(t.Name).Append(" = ").Append(value).Append('\n');
                    return true;
                }
                case IfStmt i:
                {
                    bool first = true;
                    foreach (var (cond, blk) in i.Branches)
                    {
                        if (cond is not null)
                        {
                            var c = Expr(cond, locals);
                            if (c is null) { Report("condition", s.Line, "an if condition is not convertible"); return false; }
                            output.Append(ind).Append(first ? "if " : "elseif ").Append(c).Append(" then\n");
                        }
                        else output.Append(ind).Append("else\n");
                        first = false;
                        if (!ConvertBlock(blk, new HashSet<string>(locals, StringComparer.Ordinal), output, depth + 1)) return false;
                    }
                    output.Append(ind).Append("endif\n");
                    return true;
                }
                case WhileStmt w:
                {
                    if (Expr(w.Cond, locals) is not { } c) { Report("while", s.Line, "a while condition is not convertible"); return false; }
                    output.Append(ind).Append("loop\n");
                    output.Append(ind).Append("    exitwhen not ").Append(c).Append('\n');
                    if (!ConvertBlock(w.Body, new HashSet<string>(locals, StringComparer.Ordinal), output, depth + 1)) return false;
                    output.Append(ind).Append("endloop\n");
                    return true;
                }
                case DoStmt d:
                    return ConvertBlock(d.Body, locals, output, depth);
                case ReturnStmt r:
                    if (r.Values.Count != 0) { Report("return", s.Line, "a return with a value needs a JASS return type, which Lua does not spell"); return false; }
                    output.Append(ind).Append("return\n");
                    return true;
                default:
                    Report("statement", s.Line, $"a {s.GetType().Name.Replace("Stmt", "").ToLowerInvariant()} statement with no JASS form");
                    return false;
            }
        }

        /// <summary>The JASS text for a Lua expression, or null when it does not convert. A call to a
        /// name the map does not declare is emitted and named in the diagnostics.</summary>
        private string? Expr(LuaExpr e, HashSet<string> locals)
        {
            switch (e)
            {
                case LiteralExpr lit:
                    return Literal(lit);
                case NameExpr n:
                    return n.Name;
                case ParenExpr p:
                    return Expr(p.Inner, locals) is { } inner ? "(" + inner + ")" : null;
                case UnaryExpr u:
                    return Expr(u.Operand, locals) is { } operand ? $"({u.Op} {operand})" : null;
                case BinaryExpr b:
                {
                    if (Expr(b.Left, locals) is not { } left || Expr(b.Right, locals) is not { } right) return null;
                    string op = b.Op == "~=" ? "!=" : b.Op == ".." ? "+" : b.Op;
                    return $"({left} {op} {right})";
                }
                case CallExpr c:
                    return Call(c, locals);
                default:
                    return null;
            }
        }

        private string? Call(CallExpr c, HashSet<string> locals)
        {
            if (c.Method is not null || LuaScriptReader.CalleeName(c.Func) is not { } name) return null;
            if (locals.Contains(name)) return null;   // a closure call, which JASS cannot express this way
            var args = new List<string>();
            foreach (var a in c.Args)
            {
                var text = Expr(a, locals);
                if (text is null) return null;
                args.Add(text);
            }
            if (HelperOperators.TryGetValue(name, out var jassOp) && args.Count == 2)
                return $"({args[0]} {jassOp} {args[1]})";
            if (name == "__cat" && args.Count >= 2)
                return "(" + string.Join(" + ", args) + ")";
            if (name.StartsWith("__hs_", StringComparison.Ordinal))
                return HashtableCall(name, args);
            if (name is "__exs" or "__luachunk")
            {
                Diagnostics.Add($"a call to {name} at line {c.Line} needs the runtime tables it is indexed by, which were not converted");
                return null;
            }
            if (!_funcArity.TryGetValue(name, out int arity))
                Diagnostics.Add($"called '{name}' at line {c.Line} is not declared by this map, so the target build must declare it");
            else if (arity != args.Count)
            {
                Diagnostics.Add($"called '{name}' at line {c.Line} passes {args.Count} argument(s) but the map declares {arity}");
                return null;
            }
            return $"{name}({string.Join(", ", args)})";
        }

        private static string? LiteralType(LuaExpr e) => e switch
        {
            LiteralExpr { Kind: "boolean" } => "boolean",
            LiteralExpr { Kind: "string" } => "string",
            LiteralExpr { Kind: "number", Value: long } => "integer",
            LiteralExpr { Kind: "number" } => "real",
            _ => null,
        };

        /// <summary>The JASS type of a value whose type is certain. A literal gives its own type,
        /// and the runtime helpers have fixed return types, string for __cat and integer for the
        /// wrapping arithmetic. Anything else is unknown, so a local of it is left alone.</summary>
        private static string? InferType(LuaExpr e)
        {
            if (LiteralType(e) is { } t) return t;
            if (e is CallExpr c && LuaScriptReader.CalleeName(c.Func) is { } name)
                return name switch
                {
                    "__cat" => "string",
                    "__iadd" or "__isub" or "__imul" or "__idiv" => "integer",
                    "__div" => "real",
                    "__fadd" or "__fsub" or "__fmul" or "__fdiv" => "real",
                    // A typed hashtable load returns the type in its suffix, and a has returns a
                    // boolean, so a local assigned from either keeps its JASS type.
                    _ when name.StartsWith("__hs_load_", StringComparison.Ordinal) =>
                        name[(name.LastIndexOf('_') + 1)..],
                    _ when name.StartsWith("__hs_has_", StringComparison.Ordinal) => "boolean",
                    _ => null,
                };
            return null;
        }

        private static string? Literal(LuaExpr e)
        {
            if (e is not LiteralExpr lit) return null;
            return lit.Kind switch
            {
                "boolean" => (bool)lit.Value! ? "true" : "false",
                "string" => JassString((string)lit.Value!),
                "number" => NumberText(lit),
                _ => null,
            };
        }

        private static string NumberText(LiteralExpr lit)
        {
            if (lit.Value is long l) return l.ToString(CultureInfo.InvariantCulture);
            if (lit.Value is double d) return d.ToString("0.0###############", CultureInfo.InvariantCulture);
            return lit.Text ?? "0";
        }

        /// <summary>A JASS string literal, one char per byte, escaped as the game reads it.</summary>
        private static string JassString(string s)
        {
            var sb = new StringBuilder(s.Length + 2);
            sb.Append('"');
            foreach (char ch in s)
            {
                if (ch == '"') sb.Append("\\\"");
                else if (ch == '\\') sb.Append("\\\\");
                else if (ch == '\n') sb.Append("\\n");
                else if (ch == '\r') sb.Append("\\r");
                else if (ch == '\t') sb.Append("\\t");
                else sb.Append(ch);
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
