// src/Wc3.Commands/Lua/LuaApiNames.cs
namespace Wc3.Commands.Lua;

/// <summary>
/// The mapping between the spelling of a name in a Lua map and the JASS spelling the target
/// build declares.
///
/// Warcraft III's Lua runtime exposes the WC3 natives and Blizzard.j functions under their JASS
/// names, so the mapping is the identity for the natives this command cares about. The one
/// systematic difference is a trailing underscore. When a JASS-to-Lua conversion emits a name that
/// is a Lua keyword, it appends an underscore (see <c>JassToLua.Name</c>, which writes
/// <c>end_</c> for <c>end</c> and so on), so a Lua identifier that is a Lua keyword plus a trailing
/// underscore reads back as the JASS name with that underscore stripped. Natives are never Lua
/// keywords, but a map's own function can be, and applying the same rule keeps this command and the
/// converter agreeing about a name.
///
/// Two other real differences are handled by not treating them as natives at all. A four character
/// code written in JASS as <c>'hfoo'</c> is written in Lua as <c>FourCC("hfoo")</c>, so a call to
/// <c>FourCC</c> is skipped rather than looked up. And Blizzard.j helper functions such as
/// <c>GetLastCreatedUnit</c> are not loaded into the Lua runtime, so a Lua map calling one is a
/// genuine missing-name finding that the target schema reports on its own.
/// </summary>
internal static class LuaApiNames
{
    /// <summary>Names that are Lua standard library or runtime helpers rather than WC3 natives,
    /// skipped by the api check so they are never reported as a missing native.</summary>
    internal static readonly HashSet<string> NonNative = new(StringComparer.Ordinal)
    {
        "FourCC", "print", "tostring", "tonumber", "type", "pairs", "ipairs", "next", "select",
        "require", "pcall", "xpcall", "error", "assert", "setmetatable", "getmetatable", "rawget",
        "rawset", "rawequal", "rawlen", "unpack", "collectgarbage",
    };

    /// <summary>The JASS spelling a Lua identifier names, by the rule in the type comment.</summary>
    internal static string ToJassSpelling(string luaName)
    {
        if (luaName.Length > 1 && luaName[^1] == '_' && LuaLexer.Keywords.Contains(luaName[..^1]))
            return luaName[..^1];
        return luaName;
    }

    /// <summary>True when the Lua spelling differs from the JASS spelling this command applies.</summary>
    internal static bool Differs(string luaName) => ToJassSpelling(luaName) != luaName;

    /// <summary>One line describing the mapping, carried in the api result so the answer says how
    /// it read the names.</summary>
    internal const string Description =
        "Lua exposes the WC3 natives under their JASS names, so the mapping is the identity, except a "
        + "trailing underscore on a Lua keyword name is stripped to get the JASS spelling.";
}
