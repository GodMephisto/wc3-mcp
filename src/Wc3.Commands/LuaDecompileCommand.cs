// src/Wc3.Commands/LuaDecompileCommand.cs
using Wc3.Commands.Lua;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One Lua entry that held bytecode and now holds source.</summary>
public sealed record LuaDecompiledFile(string FileName, string OpcodeTable, int Functions, int BytesBefore, int BytesAfter);

/// <summary>
/// What a decompile did. <paramref name="Files"/> is empty when the map holds no Lua bytecode,
/// and then nothing changed.
/// </summary>
public sealed record LuaDecompileResult(IReadOnlyList<LuaDecompiledFile> Files, IReadOnlyList<string> Failed);

/// <summary>
/// Turns every Lua entry of a map that is precompiled Lua 5.3 bytecode back into source, in memory.
/// Maps built with YDWE ship war3map.lua or the modules it requires this way, and a map in that
/// form cannot be read, searched or edited until it is source again.
/// </summary>
public static class LuaDecompileCommand
{
    /// <summary>The line a translated entry ends with, which runs the chunk with the global table as _ENV.</summary>
    public const string RunLine = "return F[0]({{_ENV}})(...)\n";

    public static LuaDecompileResult Decompile(MapDocument doc)
    {
        var done = new List<LuaDecompiledFile>();
        var failed = new List<string>();
        foreach (var entry in doc.Files.Where(f => f.FileName is not null
                                                   && f.FileName.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
                                       .GroupBy(f => f.FileName!, StringComparer.OrdinalIgnoreCase)
                                       .Select(g => g.First())
                                       .ToList())
        {
            var bytes = entry.CurrentBytes;
            if (!LuaBytecode.IsLua53Chunk(bytes)) continue;
            string name = entry.FileName!;
            string text;
            try
            {
                text = ToRunnableSource(bytes, name);
            }
            catch (Exception e) when (e is LuaBytecodeError or LuaTranslateError)
            {
                failed.Add($"{name}: {e.Message}");
                continue;
            }
            // The translation is checked as luac would check it, so a result the game could not
            // load is never written into the map.
            if (LuaParser.Check(text) is { } err)
            {
                failed.Add($"{name}: the translation does not parse, line {err.Line}. {err.Detail}");
                continue;
            }
            var output = ScriptCommand.ScriptEncoding.GetBytes(text);
            doc.AddOrReplaceRawFile(name, output);
            var (main, _) = LuaBytecode.Load(bytes);
            done.Add(new LuaDecompiledFile(name, Lua53Translate.OpcodeTable(main) ?? "", main.Walk().Count(), bytes.Length, output.Length));
        }
        return new LuaDecompileResult(done, failed);
    }

    /// <summary>
    /// The translation with its last line changed from returning a function to running it, so the
    /// entry runs when the game loads it or a require reaches it, as the bytecode did.
    /// </summary>
    public static string ToRunnableSource(byte[] chunk, string name)
    {
        string text = Lua53Translate.Translate(chunk, name);
        return text[..^Lua53Translate.ReturnLine.Length] + RunLine;
    }
}
