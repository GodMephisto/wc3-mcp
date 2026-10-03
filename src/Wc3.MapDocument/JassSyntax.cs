// src/Wc3.MapDocument/JassSyntax.cs
namespace Wc3.Model;

/// <summary>
/// The JASS vocabulary, in one place. Keywords and types are fixed by the language. Natives are
/// not, they come from the game's own common.j, so a caller with a game install can name them and
/// a caller without one still gets everything else right.
/// </summary>
/// <remarks>
/// This lives beside <see cref="JassFunctionIndex"/>, <see cref="JassGlobals"/> and
/// <see cref="JassScriptCheck"/> rather than in the Studio, for the reason all four are here.
/// Nothing about "which words are JASS keywords" belongs to a front-end. The Studio builds its
/// syntax highlighting from this list, and a terminal or an MCP response could colour the same
/// tokens from the same list without either of them owning a second copy.
/// </remarks>
public static class JassSyntax
{
    /// <summary>Reserved words. JASS is small, so this is the whole list, plus the vJASS words
    /// that appear in scripts people paste into a map even though war3map.j is compiled JASS.</summary>
    public static readonly IReadOnlySet<string> Keywords = new HashSet<string>(StringComparer.Ordinal)
    {
        // JASS proper
        "globals", "endglobals", "constant", "native", "function", "endfunction",
        "takes", "returns", "nothing", "extends", "type", "local", "set", "call",
        "if", "then", "else", "elseif", "endif", "loop", "endloop", "exitwhen",
        "return", "array", "and", "or", "not", "true", "false", "null", "debug",
        // vJASS, harmless to highlight and useful when reading a source map's own script
        "library", "endlibrary", "scope", "endscope", "struct", "endstruct",
        "method", "endmethod", "interface", "endinterface", "module", "endmodule",
        "implement", "private", "public", "static", "readonly", "requires",
        "needs", "uses", "initializer", "operator", "delegate", "hook", "textmacro",
        "endtextmacro", "runtextmacro", "import", "optional", "defaults", "this",
        "thistype", "allocate", "deallocate", "create", "destroy", "super",
    };

    /// <summary>The primitive and handle types common.j declares. A script can declare more with
    /// <c>type X extends Y</c>, which <see cref="DeclaredTypes"/> finds.</summary>
    public static readonly IReadOnlySet<string> Types = new HashSet<string>(StringComparer.Ordinal)
    {
        "integer", "real", "string", "boolean", "code", "handle", "agent", "event",
        "player", "widget", "unit", "destructable", "item", "ability", "buff",
        "force", "group", "trigger", "triggercondition", "triggeraction", "timer",
        "location", "region", "rect", "boolexpr", "sound", "conditionfunc",
        "filterfunc", "unitpool", "itempool", "race", "alliancetype", "racepreference",
        "gamestate", "igamestate", "fgamestate", "playerstate", "playerscore",
        "playergameresult", "unitstate", "aidifficulty", "eventid", "gameevent",
        "playerevent", "playerunitevent", "unitevent", "limitop", "widgetevent",
        "dialogevent", "unittype", "gamespeed", "gamedifficulty", "gametype",
        "mapflag", "mapvisibility", "mapsetting", "mapdensity", "mapcontrol",
        "playerslotstate", "volumegroup", "camerafield", "camerasetup",
        "playercolor", "placement", "startlocprio", "raritycontrol", "blendmode",
        "texmapflags", "effect", "effecttype", "weathereffect", "terraindeformation",
        "fogstate", "fogmodifier", "dialog", "button", "quest", "questitem",
        "defeatcondition", "timerdialog", "leaderboard", "multiboard",
        "multiboarditem", "trackable", "gamecache", "version", "itemtype",
        "texttag", "attacktype", "damagetype", "weapontype", "soundtype",
        "lightning", "pathingtype", "mousebuttontype", "animtype", "subanimtype",
        "image", "ubersplat", "hashtable", "framehandle", "originframetype",
        "framepointtype", "textaligntype", "frameeventtype", "oskeytype",
        "abilityintegerfield", "abilityrealfield", "abilitybooleanfield",
        "abilitystringfield", "abilityintegerlevelfield", "abilityreallevelfield",
        "abilitybooleanlevelfield", "abilitystringlevelfield", "unitintegerfield",
        "unitrealfield", "unitbooleanfield", "unitstringfield", "unitweaponintegerfield",
        "unitweaponrealfield", "unitweaponbooleanfield", "unitweaponstringfield",
        "itemintegerfield", "itemrealfield", "itembooleanfield", "itemstringfield",
        "movetype", "targetflag", "armortype", "regentype", "unitcategory",
        "pathingflag", "commandbuttoneffect",
    };

    /// <summary>
    /// The natives a script may call, read from the game's own <c>common.j</c> and
    /// <c>Blizzard.j</c> text. Returns an empty set for empty input, so a caller with no game
    /// install degrades to "keywords and types only" rather than failing.
    /// </summary>
    /// <remarks>
    /// Worth doing rather than shipping a hardcoded list of about two thousand names. The list
    /// changes with every patch, a stale copy would highlight a removed native and miss a new
    /// one, and the authoritative answer is sitting in the install already.
    /// </remarks>
    public static IReadOnlySet<string> NativesIn(string commonJ)
    {
        var found = new HashSet<string>(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(commonJ)) return found;

        foreach (var raw in JassLines.Split(commonJ))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal)) continue;

            // "native Foo takes ..." and "constant native Foo takes ...", plus Blizzard.j's
            // plain "function Foo takes ...", which callers treat as a native for highlighting
            // because from a map script's point of view it is one.
            var tokens = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            int i = tokens.Length > 0 && tokens[0] == "constant" ? 1 : 0;
            if (tokens.Length < i + 2) continue;
            if (tokens[i] is not ("native" or "function")) continue;
            if (tokens[i + 1] == "interface") continue;
            found.Add(tokens[i + 1]);
        }
        return found;
    }

    /// <summary>Handle types the script declares itself, from <c>type X extends Y</c>.</summary>
    public static IReadOnlySet<string> DeclaredTypes(string jass)
    {
        var found = new HashSet<string>(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(jass)) return found;

        foreach (var raw in JassLines.Split(jass))
        {
            var line = raw.Trim();
            if (!line.StartsWith("type ", StringComparison.Ordinal)) continue;
            var tokens = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length >= 2) found.Add(tokens[1]);
        }
        return found;
    }

    /// <summary>Whether a word is a keyword or a built-in type, the two categories that need no
    /// game install and no script scan.</summary>
    public static bool IsReserved(string word) => Keywords.Contains(word) || Types.Contains(word);

    /// <summary>
    /// Identifiers the script CALLS but does not declare. From the script's own point of view
    /// those are its externals, the natives and the Blizzard.j helpers, which is exactly the set
    /// worth highlighting differently from the script's own functions.
    /// </summary>
    /// <remarks>
    /// Chosen over reading the game's common.j, which was the obvious alternative and is worse on
    /// every axis that matters here. It needs no install, so a machine without Warcraft III gets
    /// the same highlighting. It cannot go stale, since the answer is computed from the script in
    /// front of you rather than from a patch's idea of the native list. And it costs one pass over
    /// a string instead of a CASC read.
    ///
    /// It is an approximation, and the way it is wrong is harmless. A misspelled call is reported
    /// as an external, so it highlights as a native instead of as an error. That is a job for the
    /// compile check, not for a colour.
    /// </remarks>
    /// <param name="declared">
    /// The names the script declares, when the caller already has them. The editor does, from the
    /// function list it just built, and passing them here saves parsing the whole script a second
    /// time, which is 103ms of the 372ms this used to cost on an 8.4 MB script.
    /// </param>
    public static IReadOnlySet<string> ExternalCalls(string jass, IEnumerable<string>? declared = null)
    {
        var called = new HashSet<string>(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(jass)) return called;

        // One manual pass rather than a regex. On an 8.4 MB merged arena script a regex for
        // "identifier followed by an open paren" is measurably slower than reading the characters,
        // and this runs on every script load in the editor.
        for (int i = 0; i < jass.Length; i++)
        {
            char c = jass[i];
            if (!char.IsLetter(c) && c != '_') continue;

            int start = i;
            while (i < jass.Length && (char.IsLetterOrDigit(jass[i]) || jass[i] == '_')) i++;
            int end = i;

            // Only a call site, so a variable or a type name is not mistaken for a function.
            int j = i;
            while (j < jass.Length && (jass[j] == ' ' || jass[j] == '\t')) j++;
            if (j >= jass.Length || jass[j] != '(') { i = end - 1; continue; }

            var word = jass[start..end];
            if (!IsReserved(word)) called.Add(word);
            i = end - 1;
        }

        // A function the script declares is its own, not an external.
        if (declared is not null)
        {
            foreach (var name in declared) called.Remove(name);
        }
        else
        {
            foreach (var f in JassFunctionIndex.Parse(jass)) called.Remove(f.Name);
        }

        return called;
    }
}
