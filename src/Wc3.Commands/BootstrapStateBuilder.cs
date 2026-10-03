// src/Wc3.Commands/BootstrapStateBuilder.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Finds every carried global a ported script reads but assigns nowhere in the merged output, and
/// builds a small function that constructs the ones that can be constructed safely, by type.
///
/// Why this exists: every headline bug found while porting Anime WOS2's Asta had the exact same
/// shape, a global the carried code reads and nothing ever assigns, sitting at its type default
/// with no error and no warning. <c>gg_rct_Base</c> and <c>gg_rct_Cage</c> (CreateRegions, the
/// function that would set them, was never carried, the whole map's region list is not this
/// hero's dependency) read as bounds (0,0) to (0,0) on a null rect, so a hero standing at the
/// origin was permanently "inside the base" and a cast gate blocked every spell. GearTimer03,
/// GearTimer05 and GearTimer10 (created only inside a hand-written Init reached solely through
/// <c>call ExecuteFunc("Init")</c>, a STRING literal, invisible to identifier scanning) stayed
/// null, so every timed part of a spell silently did nothing while its instant part worked. See
/// <see cref="ScriptPorter"/> for where this is spliced in (opt-in, behind --bootstrap-state).
///
/// Only handle types with a universal, argument-free constructor are ever built here: timer,
/// group, hashtable, trigger, rect, force. A rect is built EMPTY (<c>Rect(0., 0., 0., 0.)</c>),
/// never a guess at the source's real region, because for a GATE an empty rect can never falsely
/// block (the worst case is a handler that used the rect to pick a location now reading (0,0), not
/// a cast that silently never fires, and reconstructing the source's real region from its own
/// unrelated placement data would be guesswork this tool does not do). <c>unit</c>, <c>item</c>,
/// <c>destructable</c>, <c>effect</c>, <c>code</c>, <c>framehandle</c>, an array of any type, and
/// any type this does not recognize are deliberately left alone and reported instead by the
/// caller, inventing one of those is worse than leaving it null.
/// </summary>
internal static class BootstrapStateBuilder
{
    /// <summary>One global the ported script reads but never assigns. <see cref="Constructor"/> is
    /// the native call that safely builds a fresh instance of <see cref="Type"/>, or null when this
    /// type (or an array of it) has no universal, argument-free constructor and must be left alone.
    /// </summary>
    public sealed record Candidate(string Name, string Type, bool IsArray, string? Constructor);

    /// <summary>The only types with a construction that is safe for ANY declaration of that type,
    /// needs no arguments, and cannot make a gate more permissive than doing nothing at all (a rect
    /// is the borderline case, see the class doc for why empty is the right default there).</summary>
    private static readonly IReadOnlyDictionary<string, string> SafeConstructors =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["timer"] = "CreateTimer()",
            ["group"] = "CreateGroup()",
            ["hashtable"] = "InitHashtable()",
            ["trigger"] = "CreateTrigger()",
            ["rect"] = "Rect(0., 0., 0., 0.)",
            ["force"] = "CreateForce()",
        };

    /// <summary>
    /// Every candidate among <paramref name="declaredByFinalName"/> (each carried global's ORIGINAL
    /// declaration text, type and initializer shape are unaffected by renaming, keyed by its FINAL,
    /// post-collision-rename name) that <paramref name="mergedBodyText"/> (every function this port
    /// is about to splice in, already renamed and rawcode-remapped) never assigns.
    /// <see cref="JassGlobals.Assigned"/> decides "never assigns" the exact same way the runtime
    /// readiness check does (a "set NAME=" anywhere in the text, or a non-default value on the
    /// declaration itself), so the two can never disagree about what counts as an assignment.
    /// Returned in name order, for a deterministic, diffable bootstrap function.
    /// </summary>
    public static IReadOnlyList<Candidate> FindUnassigned(
        IReadOnlyDictionary<string, string> declaredByFinalName, string mergedBodyText)
    {
        var assigned = JassGlobals.Assigned(declaredByFinalName, mergedBodyText);
        var result = new List<Candidate>();
        foreach (var (name, decl) in declaredByFinalName)
        {
            if (assigned.Contains(name)) continue;
            if (JassGlobals.TypeOf(decl) is not { } described) continue;
            var (type, isArray) = described;
            string? ctor = !isArray && SafeConstructors.TryGetValue(type, out var c) ? c : null;
            result.Add(new Candidate(name, type, isArray, ctor));
        }
        return result.OrderBy(c => c.Name, StringComparer.Ordinal).ToList();
    }

    /// <summary>The bootstrap function's raw text: one "set NAME = ctor()" per constructible
    /// candidate (<see cref="Candidate.Constructor"/> non-null), in the order given. Null when
    /// there is nothing to construct, every candidate needs a type this cannot safely build, so the
    /// caller has nothing to splice in.</summary>
    public static string? BuildRawText(string functionName, IReadOnlyList<Candidate> candidates)
    {
        var constructible = candidates.Where(c => c.Constructor is not null).ToList();
        if (constructible.Count == 0) return null;

        var sb = new StringBuilder();
        sb.Append("function ").Append(functionName).Append(" takes nothing returns nothing\n");
        foreach (var c in constructible)
            sb.Append("    set ").Append(c.Name).Append(" = ").Append(c.Constructor).Append('\n');
        sb.Append("endfunction\n");
        return sb.ToString();
    }
}
