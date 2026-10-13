// src/Wc3.Commands/LuaGlobalsCommand.cs
using Wc3.Commands.Lua;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One global a Lua map defines. <paramref name="ReadLines"/> are the lines that read it.
/// <paramref name="Framework"/> marks the two WC3 entry points, main and config, which the runtime
/// calls rather than the script.</summary>
public sealed record LuaGlobal(
    string Name, int FirstWriteLine, int Writes, int Reads, IReadOnlyList<int> ReadLines, bool Framework);

/// <summary>The globals a Lua map defines, where each is read, and how many are never read.</summary>
public sealed record LuaGlobalsResult(
    bool Ok, string Message, int Declared, int NeverRead, IReadOnlyList<LuaGlobal> Globals);

/// <summary>
/// Lists the globals a Lua map defines, how often each is written and read and at which lines, and
/// reports any global that is written but never read.
///
/// A Lua global is any name assigned where no enclosing block declares it with <c>local</c>, so the
/// walk keeps a scope stack (function parameters, <c>local</c>, and the loop variables of a
/// <c>for</c>) and treats a name outside every scope as a global. A bare <c>function name()</c> is
/// a global write, and so is <c>name = function()</c>. A function reference such as
/// <c>CreateUnit(...)</c> is a read of the global <c>CreateUnit</c>, which the runtime supplies
/// rather than the map, so a name that is only ever read is not reported. That is the point, only a
/// name the map itself defines is listed.
///
/// The two entry points main and config are marked framework. The game calls them, nothing in the
/// script does, so counting them as written and never read would be noise on every map.
/// </summary>
public static class LuaGlobalsCommand
{
    private sealed class Acc
    {
        public int Writes;
        public int FirstWriteLine = int.MaxValue;
        public readonly List<int> ReadLines = new();
    }

    public static LuaGlobalsResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var (file, source, isLua) = LuaScriptReader.Read(doc);
        if (!isLua)
            return new(false, $"{file} is a JASS script, not Lua.", 0, 0, Array.Empty<LuaGlobal>());
        var chunk = LuaScriptReader.TryParse(source, out var error);
        if (chunk is null)
            return new(false, $"{file} does not parse, line {error!.Line}. {error.Detail}", 0, 0, Array.Empty<LuaGlobal>());
        return Analyze(chunk);
    }

    internal static LuaGlobalsResult Analyze(LuaChunk chunk)
    {
        var acc = new Dictionary<string, Acc>(StringComparer.Ordinal);
        new Walker(acc).Walk(chunk.Body);

        var globals = acc
            .Where(kv => kv.Value.Writes > 0)
            .Select(kv => new LuaGlobal(
                kv.Key, kv.Value.FirstWriteLine, kv.Value.Writes, kv.Value.ReadLines.Count,
                kv.Value.ReadLines.OrderBy(l => l).ToList(), IsFramework(kv.Key)))
            .OrderBy(g => g.FirstWriteLine)
            .ThenBy(g => g.Name, StringComparer.Ordinal)
            .ToList();
        int neverRead = globals.Count(g => !g.Framework && g.Reads == 0);
        string message = $"{globals.Count} global(s) defined, {neverRead} written and never read";
        return new LuaGlobalsResult(true, message, globals.Count, neverRead, globals);
    }

    private static bool IsFramework(string name) =>
        name is "main" or "config";

    private static bool IsLuaKeyword(string name) => LuaLexer.Keywords.Contains(name);

    private sealed class Walker(Dictionary<string, Acc> acc)
    {
        // One top-level scope, so a chunk-level `local` has somewhere to go. Without it the first
        // top-level local indexed an empty list and threw ArgumentOutOfRangeException on every Lua
        // map whose script declares a local at file scope, which is nearly all of them.
        private readonly List<HashSet<string>> _scopes =
            new() { new HashSet<string>(StringComparer.Ordinal) };

        private bool IsLocal(string name)
        {
            for (int i = _scopes.Count - 1; i >= 0; i--)
                if (_scopes[i].Contains(name)) return true;
            return false;
        }

        private void Write(string name, int line)
        {
            if (IsLuaKeyword(name)) return;
            if (!acc.TryGetValue(name, out var a)) acc[name] = a = new Acc();
            a.Writes++;
            if (line < a.FirstWriteLine) a.FirstWriteLine = line;
        }

        private void Read(string name, int line)
        {
            if (IsLuaKeyword(name)) return;
            if (!acc.TryGetValue(name, out var a)) acc[name] = a = new Acc();
            a.ReadLines.Add(line);
        }

        private void Scoped(Action body)
        {
            _scopes.Add(new HashSet<string>(StringComparer.Ordinal));
            body();
            _scopes.RemoveAt(_scopes.Count - 1);
        }

        public void Walk(LuaBlock block)
        {
            foreach (var s in block) WalkStmt(s);
        }

        private void WalkStmt(LuaStmt s)
        {
            switch (s)
            {
                case LocalStmt l:
                    foreach (var v in l.Values) WalkExpr(v);
                    foreach (var n in l.Names) _scopes[^1].Add(n);
                    break;
                case AssignStmt a:
                    foreach (var v in a.Values) WalkExpr(v);
                    foreach (var t in a.Targets) WalkTarget(t);
                    break;
                case CallStmt c:
                    WalkExpr(c.Call);
                    break;
                case FunctionStmt f:
                    DeclareFunction(f);
                    Scoped(() =>
                    {
                        foreach (var p in f.Params) _scopes[^1].Add(p);
                        if (f.Name.Contains(':')) _scopes[^1].Add("self");
                        Walk(f.Body);
                    });
                    break;
                case IfStmt i:
                    foreach (var (cond, body) in i.Branches)
                    {
                        if (cond is not null) WalkExpr(cond);
                        Scoped(() => Walk(body));
                    }
                    break;
                case WhileStmt w:
                    WalkExpr(w.Cond);
                    Scoped(() => Walk(w.Body));
                    break;
                case NumericForStmt n:
                    WalkExpr(n.Start);
                    WalkExpr(n.Stop);
                    if (n.Step is not null) WalkExpr(n.Step);
                    Scoped(() =>
                    {
                        _scopes[^1].Add(n.Var);
                        Walk(n.Body);
                    });
                    break;
                case GenericForStmt g:
                    foreach (var e in g.Exprs) WalkExpr(e);
                    Scoped(() =>
                    {
                        foreach (var n in g.Names) _scopes[^1].Add(n);
                        Walk(g.Body);
                    });
                    break;
                case RepeatStmt r:
                    Scoped(() =>
                    {
                        Walk(r.Body);
                        WalkExpr(r.Cond);
                    });
                    break;
                case DoStmt d:
                    Scoped(() => Walk(d.Body));
                    break;
                case ReturnStmt r:
                    foreach (var v in r.Values) WalkExpr(v);
                    break;
            }
        }

        private void DeclareFunction(FunctionStmt f)
        {
            int dot = f.Name.IndexOfAny(new[] { '.', ':' });
            if (f.IsLocal) return;
            if (dot < 0) Write(f.Name, f.Line);
            else Read(f.Name[..dot], f.Line);
        }

        /// <summary>A target of an assignment. A bare name is a global write when it is not local.
        /// An indexed target reads its base and key, as evaluating the target does.</summary>
        private void WalkTarget(LuaExpr t)
        {
            switch (t)
            {
                case NameExpr n:
                    if (!IsLocal(n.Name)) Write(n.Name, n.Line);
                    break;
                case IndexExpr ix:
                    WalkExpr(ix.Base);
                    WalkExpr(ix.Key);
                    break;
                default:
                    WalkExpr(t);
                    break;
            }
        }

        private void WalkExpr(LuaExpr e)
        {
            switch (e)
            {
                case NameExpr n:
                    if (!IsLocal(n.Name)) Read(n.Name, n.Line);
                    break;
                case IndexExpr ix:
                    WalkExpr(ix.Base);
                    WalkExpr(ix.Key);
                    break;
                case CallExpr c:
                    WalkExpr(c.Func);
                    foreach (var a in c.Args) WalkExpr(a);
                    break;
                case FunctionExpr fe:
                    Scoped(() =>
                    {
                        foreach (var p in fe.Params) _scopes[^1].Add(p);
                        Walk(fe.Body);
                    });
                    break;
                case UnaryExpr u:
                    WalkExpr(u.Operand);
                    break;
                case BinaryExpr b:
                    WalkExpr(b.Left);
                    WalkExpr(b.Right);
                    break;
                case ParenExpr p:
                    WalkExpr(p.Inner);
                    break;
                case TableExpr t:
                    foreach (var (k, v) in t.Fields)
                    {
                        if (k is LuaExpr ke) WalkExpr(ke);
                        WalkExpr(v);
                    }
                    break;
            }
        }
    }
}
