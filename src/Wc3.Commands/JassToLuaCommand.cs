// src/Wc3.Commands/JassToLuaCommand.cs
using System.Text.RegularExpressions;
using Wc3.Commands.Lua;
using Wc3.Model;
using War3Net.Build.Info;

namespace Wc3.Commands;

/// <summary>
/// What converting a map's script to Lua did. <paramref name="UsedGameHeaders"/> is false when
/// common.j and Blizzard.j could not be read from the game, in which case calls into them were
/// typed as unknown and every such call carries a warning.
/// </summary>
public sealed record JassToLuaSummary(
    string ScriptFile,
    int Functions,
    int ExsLiterals,
    int LuaChunks,
    int BytesBefore,
    int BytesAfter,
    bool UsedGameHeaders,
    IReadOnlyList<string> Warnings);

/// <summary>
/// Converts a JASS map to a Lua map in memory. The caller saves the document to a new path.
/// </summary>
public static class JassToLuaCommand
{
    public const string StubText = "// This map runs war3map.lua. The JASS it came from was converted by wc3ctl script to-lua.\n";

    /// <summary>
    /// Converts the script of <paramref name="doc"/> to war3map.lua and sets the w3i script
    /// language to Lua, so the game runs the Lua. The old war3map.j is replaced by a one line
    /// comment, because the archive model cannot remove an entry and the World Editor would
    /// otherwise still show the JASS.
    /// </summary>
    /// <remarks>
    /// The headers come from the installed game when <paramref name="commonJ"/> and
    /// <paramref name="blizzardJ"/> are not given. Without them the conversion still runs,
    /// since the map's own declarations type most expressions, but a call into a native is untyped
    /// and an integer operation on its result loses the 32 bit wrap.
    /// </remarks>
    public static JassToLuaSummary Convert(MapDocument doc, string? commonJ = null, string? blizzardJ = null, string? gameDirOverride = null)
    {
        var (file, source) = ScriptCommand.Read(doc);
        if (ScriptCommand.IsLua(file))
            throw new InvalidOperationException($"{file} is already Lua, nothing to convert.");

        var infoEntry = doc.GetFile("war3map.w3i");
        if (infoEntry?.Model is not MapInfo info)
            throw new InvalidOperationException("war3map.w3i is missing or could not be parsed, the script language cannot be set.");
        var warnings = new List<string>();
        if (info.FormatVersion < MapInfoFormatVersion.v28)
        {
            // v28 is the first w3i with a script language field, and it adds only that and the
            // game version, so a classic map moves up without any other field changing meaning.
            warnings.Add($"war3map.w3i was format {info.FormatVersion}, raised to v28 to carry the Lua flag. The map now needs game 1.31 or later.");
            info.FormatVersion = MapInfoFormatVersion.v28;
            info.GameVersion ??= new Version(1, 31, 1, 12164);
        }

        bool usedHeaders = commonJ is not null && blizzardJ is not null;
        if (!usedHeaders && TryReadHeaders(gameDirOverride, out var c, out var b))
        {
            (commonJ, blizzardJ, usedHeaders) = (c, b, true);
        }

        JassToLuaResult r;
        try
        {
            r = JassToLua.Transpile(commonJ ?? "", blizzardJ ?? "", NormalizeLineEnds(source));
        }
        catch (JassToLuaError e)
        {
            throw new InvalidOperationException($"{file} could not be converted. {e.Message}");
        }

        if (!usedHeaders)
            warnings.Add("common.j and Blizzard.j were not found in the game install, calls into them are untyped.");
        int customText = CustomTextTriggers(doc);
        if (customText > 0)
            warnings.Add($"war3map.wtg holds {customText} custom text trigger(s). They are JASS, and the World Editor "
                         + "would regenerate the script from them, so edit the converted map's script, not its triggers.");
        warnings.AddRange(r.Warnings);

        string lua = JassToLua.BuildScript(r);
        var luaBytes = ScriptCommand.ScriptEncoding.GetBytes(lua);
        string luaName = file.Contains('\\') ? @"scripts\war3map.lua" : "war3map.lua";
        doc.AddOrReplaceRawFile(luaName, luaBytes);
        doc.AddOrReplaceRawFile(file, ScriptCommand.ScriptEncoding.GetBytes(StubText));

        info.ScriptLanguage = ScriptLanguage.Lua;
        var w3i = doc.AddOrReplaceRawFile("war3map.w3i", MapInfoCommand.Serialize(info));
        w3i.Model = info;

        return new JassToLuaSummary(
            file, r.FuncNames.Count, r.ExsLiterals.Count, r.LuaChunks.Count,
            ScriptCommand.ScriptEncoding.GetByteCount(source), luaBytes.Length, usedHeaders, warnings);
    }

    /// <summary>
    /// Every bare CR as LF. The converter, like the Python it was ported from, ends a line only
    /// at LF and reads CR as a space, and 16 of 106 distinct scripts in one map library end their
    /// lines with a bare CR, which then read as one line and were refused. A CR LF pair is left
    /// alone, since the converter already reads it as a line end and a string literal holding one
    /// must keep it (two of those 106 scripts do).
    /// </summary>
    public static string NormalizeLineEnds(string source) => BareCr.Replace(source, "\n");

    private static readonly Regex BareCr = new("\r(?!\n)", RegexOptions.Compiled);

    private static bool TryReadHeaders(string? gameDirOverride, out string commonJ, out string blizzardJ)
    {
        commonJ = blizzardJ = "";
        try
        {
            if (!GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _) || ctx is null) return false;
            if (!PjassGate.TryReadHeader(ctx, "common.j", out var cb) || !PjassGate.TryReadHeader(ctx, "Blizzard.j", out var bb))
                return false;
            commonJ = ScriptCommand.ScriptEncoding.GetString(cb);
            blizzardJ = ScriptCommand.ScriptEncoding.GetString(bb);
            return true;
        }
        catch (Exception)
        {
            // No install, or CASC refused to open. The conversion runs without the headers.
            return false;
        }
    }

    private static int CustomTextTriggers(MapDocument doc)
    {
        try
        {
            return TriggerReadCommand.GetTriggers(doc).Triggers.Count(t => t.IsCustomText);
        }
        catch (Exception)
        {
            // An unreadable wtg cannot regenerate anything either.
            return 0;
        }
    }
}
