// src/Wc3.Commands/LintCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

public enum LintSeverity { Ok, Warning, Error }

/// <summary>One lint rule's outcome. <see cref="Detail"/> lines name the offending items.</summary>
public sealed record LintCheck(
    string Name,
    LintSeverity Severity,
    string Summary,
    IReadOnlyList<string> Detail);

public sealed record LintResult(IReadOnlyList<LintCheck> Checks)
{
    public int Errors => Checks.Count(c => c.Severity == LintSeverity.Error);
    public int Warnings => Checks.Count(c => c.Severity == LintSeverity.Warning);
    public bool Ok => Errors == 0;
}

/// <summary>
/// Pre-flight checks on a map, aimed squarely at the failures that a green build, a passing
/// test and a clean <c>validate</c> all missed while a user was repeatedly unable to host or
/// load the map. Each rule here exists because its absence cost real debugging time:
/// <list type="bullet">
/// <item>A script that could not compile passed <c>validate</c>, because pjass's
/// uninitialized-variable errors were classified as advisory. The user's only symptom was
/// being kicked from their own lobby.</item>
/// <item>Assets were added to an archive without a matching <c>war3map.imp</c> entry.</item>
/// <item>A one-hero port copied hundreds of files and thousands of functions belonging to
/// other heroes, and nothing objected.</item>
/// <item>A port silently overwrote the TARGET's own same-named assets.</item>
/// </list>
/// This cannot predict Warcraft III's loader. It catches the classes we have actually been
/// burned by, before a human spends minutes loading a 250 MB map to find out.
/// </summary>
public static class LintCommand
{
    /// <summary>
    /// Extensions Warcraft III actually loads from a map archive. A carried path outside this
    /// set is reported, because the porter's closure walk picks paths out of script string
    /// literals and object fields and has no notion of whether the game can use them.
    /// </summary>
    private static readonly HashSet<string> LoadableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".blp", ".tga", ".dds", ".jpg", ".jpeg",          // textures
        ".mdx", ".mdl",                                   // models
        ".wav", ".mp3", ".ogg", ".flac",                  // audio
        ".slk", ".txt", ".ai", ".wpm", ".shd", ".w3e",    // data the editor/game reads
        ".doo", ".w3i", ".w3u", ".w3t", ".w3a", ".w3b",
        ".w3d", ".w3q", ".w3h", ".w3c", ".w3r", ".w3s",
        ".wts", ".wtg", ".wct", ".imp", ".j", ".lua",
        ".fdf", ".toc", ".mmp", ".pld", ".json",   // Reforged ships conversation.json
    };

    private static readonly System.Text.RegularExpressions.Regex ScriptStringLiteral =
        new("\"([^\"\\r\\n]{3,260})\"", System.Text.RegularExpressions.RegexOptions.Compiled);

    public static LintResult Run(MapDocument doc, MapDocument? original = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var checks = new List<LintCheck>
        {
            ScriptCompiles(doc),
            ImportTableConsistent(doc),
            AssetReferencesResolve(doc),
            CarriedFileTypesLoadable(doc),
            TriggerBodiesPairUp(doc),
        };
        if (original is not null)
        {
            checks.Add(NoFilesLost(doc, original));
            checks.Add(NoTargetAssetsClobbered(doc, original));
        }
        return new LintResult(checks);
    }

    /// <summary>
    /// war3map.wct holds one code body per trigger, found by POSITION, with no name or id
    /// anchoring it to the trigger it belongs to. So if the count on either side drifts, every
    /// body after the drift belongs to the wrong trigger, and the map still loads, still runs,
    /// and still looks correct in a tree view. There is no symptom until someone reads a script
    /// and finds someone else's.
    ///
    /// The expected count depends on the wtg sub-version, and both branches were measured across
    /// the whole map library rather than assumed (see <see cref="WctPairing"/>). Every readable
    /// map there agrees with its own rule, which is what makes a disagreement worth reporting
    /// rather than a false alarm waiting to happen.
    ///
    /// A Warning rather than an Error: a mismatch makes the trigger panel untrustworthy and makes
    /// removing a trigger unsafe, but it does not stop the map loading, and a map that arrived
    /// this way is not made worse by being opened.
    /// </summary>
    private static LintCheck TriggerBodiesPairUp(MapDocument doc)
    {
        var wtg = doc.GetFile(TriggerCommand.FileName)?.Model
            as War3Net.Build.Script.MapTriggers;
        if (wtg is null)
            return new("trigger-bodies", LintSeverity.Ok,
                "map has no readable war3map.wtg", Array.Empty<string>());

        var wct = doc.GetFile(TriggerCommand.CustomTextFileName)?.Model
            as War3Net.Build.Script.MapCustomTextTriggers;
        if (wct is null)
            return new("trigger-bodies", LintSeverity.Ok,
                "map has no war3map.wct, so no code bodies to pair", Array.Empty<string>());

        int expected = WctPairing.ExpectedSlotCount(wtg);
        int actual = wct.CustomTextTriggers.Count;
        bool guiHoldsSlot = WctPairing.GuiTriggersHoldASlot(wtg);
        string rule = guiHoldsSlot
            ? "no wtg sub-version, so every trigger owns a slot (an empty one when it is GUI)"
            : "wtg sub-version present, so only custom-text triggers own a slot";

        if (expected == actual)
            return new("trigger-bodies", LintSeverity.Ok,
                $"{actual} war3map.wct code slot(s) pair up with the trigger tree",
                new[] { rule });

        var detail = new List<string>
        {
            rule,
            $"war3map.wct holds {actual} slot(s), the trigger tree accounts for {expected}",
        };

        // Name the first trigger whose body is in doubt, since that is what makes the report
        // actionable rather than a number.
        var slots = WctPairing.SlotIndices(wtg);
        var firstAffected = slots
            .Where(kv => kv.Value >= Math.Min(expected, actual))
            .OrderBy(kv => kv.Value)
            .Select(kv => kv.Key)
            .FirstOrDefault();
        if (firstAffected is not null)
            detail.Add($"from '{firstAffected.Name}' onward, a body may belong to another trigger");
        detail.Add("removing a trigger is refused while this holds, and the trigger panel's "
                 + "script bodies cannot be trusted");

        return new("trigger-bodies", LintSeverity.Warning,
            $"war3map.wct and war3map.wtg disagree by {Math.Abs(expected - actual)} slot(s)",
            detail);
    }

    /// <summary>
    /// The check that matters most. An un-compilable <c>war3map.j</c> means <c>config()</c>
    /// never runs, so a hosted map shows no player slots and the host is kicked from their own
    /// lobby. Everything pjass calls an error is an error here, bar a missing-but-real native.
    /// </summary>
    private static LintCheck ScriptCompiles(MapDocument doc)
    {
        var script = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (script is null)
            return new("script-compiles", LintSeverity.Ok, "map has no JASS script", Array.Empty<string>());
        // Latin-1, NOT UTF-8. PjassGate writes the string back out with Latin1.GetBytes, and
        // Latin-1 cannot encode anything above U+00FF, so decoding as UTF-8 first turned every
        // such character into '?' and pjass reported "Unrecognized character ? (ASCII 63)". That
        // made this check fail on UNTOUCHED maps whose scripts are not pure ASCII, and worse, it
        // passed on a map an earlier install had already corrupted into that range, so a green
        // result meant nothing. Latin-1 both ways round-trips every byte 0..255 unchanged.
        var pj = PjassGate.Check(System.Text.Encoding.Latin1.GetString(script.CurrentBytes));
        if (!pj.Ran)
            return new("script-compiles", LintSeverity.Warning,
                "pjass did not run, so the script was not compile-checked", new[] { pj.Note });
        if (!pj.Passed)
            return new("script-compiles", LintSeverity.Error,
                $"war3map.j does not compile ({pj.Errors.Count} error(s)). A hosted map will show no "
                + "player slots. If any error names a variable the porter marked "
                + "\"(initializer dropped)\", 'wc3ctl script repair' is the fix.",
                pj.Errors.ToList());
        return new("script-compiles", LintSeverity.Ok, "war3map.j compiles", pj.Warnings.ToList());
    }

    /// <summary>
    /// Every imported asset in the archive should be listed in <c>war3map.imp</c>, and every
    /// listed path should exist. Adding an archive entry without its import entry is easy to do
    /// and invisible to a content diff.
    /// </summary>
    /// <remarks>
    /// A WARNING, not an error, and the reasoning is worth keeping because the opposite mistake has
    /// been made in this file before. Measured across 34 maps in the user's Maps folder, 4 have an
    /// .imp listing files the archive does not contain, up to 1,138 of them, and all 4 are
    /// published playable maps. The absent entries are checked by hand and are genuinely absent,
    /// none is a prefix-spelling miss. What makes them harmless is that nothing references them,
    /// which is why asset-references passes on those maps, and the game loads an asset by path when
    /// something asks for it rather than by walking this table.
    ///
    /// So the table is stale bookkeeping, the normal residue of an optimizer stripping unused
    /// imports without rewriting it. Reporting it as an error made `lint` exit 1 on working maps,
    /// which is how a tool teaches people to ignore its exit code.
    ///
    /// The other direction is the one with teeth, and the message says so. An archive entry the
    /// table does NOT list survives in the game but is invisible to the World Editor's import
    /// manager, so the next World Editor save can drop it. That is data loss rather than a
    /// rendering fault, and it is still not something that stops the current map running, which is
    /// what an error-level finding claims.
    /// </remarks>
    private static LintCheck ImportTableConsistent(MapDocument doc)
    {
        if (doc.GetFile(ImportsCommand.ImpFileName) is null)
            return new("import-table", LintSeverity.Ok, "map has no war3map.imp", Array.Empty<string>());

        var listing = ImportsCommand.Execute(doc);
        var stale = listing.Entries.Where(e => e.InManifest && !e.InArchive).ToList();
        var unlisted = listing.Entries.Where(e => !e.InManifest && e.InArchive).ToList();

        if (stale.Count == 0 && unlisted.Count == 0)
            return new("import-table", LintSeverity.Ok,
                $"war3map.imp agrees with the archive ({listing.Entries.Count} entries)",
                Array.Empty<string>());

        var problems = new List<string>();
        foreach (var e in stale)
            problems.Add($"war3map.imp lists '{e.Path}', which is not in the archive");
        foreach (var e in unlisted)
            problems.Add($"'{e.Path}' is in the archive but not listed in war3map.imp");

        // Say which direction, and what each costs, rather than one count of "disagree".
        var parts = new List<string>();
        if (stale.Count > 0)
            parts.Add($"{stale.Count} listed path(s) are not in the archive (stale table entries, "
                    + "the map still runs, see asset-references for whether anything needs them)");
        if (unlisted.Count > 0)
            parts.Add($"{unlisted.Count} archive entry(ies) are not listed (they load in the game, "
                    + "but the World Editor cannot see them and may drop them on its next save)");

        return new("import-table", LintSeverity.Warning, string.Join("; ", parts), Capped(problems));
    }

    /// <summary>
    /// Asset paths named by the map's own script and object data must resolve to a real entry.
    /// Uses the same un-escaping and spelling expansion the loader effectively does, so a
    /// doubled backslash in a JASS literal is not mistaken for a missing file.
    /// </summary>
    private static LintCheck AssetReferencesResolve(MapDocument doc)
    {
        var script = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j")
            ?? doc.GetFile("war3map.lua") ?? doc.GetFile("scripts\\war3map.lua");
        if (script is null)
            return new("asset-references", LintSeverity.Ok, "no script to scan", Array.Empty<string>());

        var text = ScriptText.GetString(script.CurrentBytes);
        var imported = ImportedPathsOf(doc);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var missing = new List<string>();
        foreach (System.Text.RegularExpressions.Match m in ScriptStringLiteral.Matches(text))
        {
            // Unescape first: these maps write one real separator as a doubled backslash, and
            // comparing the raw literal against stored names is how a present file was once
            // reported absent (see AssetPathCandidates.Unescape).
            var raw = AssetPathCandidates.Unescape(m.Groups[1].Value);
            if (!AssetPathCandidates.LooksLikeAssetPath(raw) || !seen.Add(raw)) continue;
            if (AssetPathCandidates.Expand(raw).Any(c => doc.GetFile(c) is not null)) continue;
            // A path the map never claimed to import is a base-game asset the archive is not
            // expected to hold, and flagging those buries the ones that matter. Only report a
            // path the map itself says it imports.
            if (!imported.Contains(raw) && !raw.StartsWith("war3mapImported", StringComparison.OrdinalIgnoreCase))
                continue;
            missing.Add(raw);
        }
        return missing.Count == 0
            ? new("asset-references", LintSeverity.Ok, "every referenced asset path resolves", Array.Empty<string>())
            : new("asset-references", LintSeverity.Warning,
                $"{missing.Count} referenced asset path(s) do not resolve. A base-game path is fine, "
                + "an imported one that is absent renders as nothing.", Capped(missing));
    }

    /// <summary>
    /// Flags archive entries whose extension the game does not load. The porter's closure walk
    /// harvests paths from script literals, so it can carry things that are not assets at all.
    /// </summary>
    private static LintCheck CarriedFileTypesLoadable(MapDocument doc)
    {
        var odd = doc.Files
            .Select(f => f.FileName)
            .Where(n => n is not null && !n.StartsWith("(", StringComparison.Ordinal))
            .Select(n => n!)
            .Where(n => Path.GetExtension(n) is { Length: > 0 } ext && !LoadableExtensions.Contains(ext))
            .ToList();

        return odd.Count == 0
            ? new("file-types", LintSeverity.Ok, "every archive entry has a loadable extension", Array.Empty<string>())
            : new("file-types", LintSeverity.Warning,
                $"{odd.Count} entry(ies) have an extension the game does not load", Capped(odd));
    }

    /// <summary>A rebuild must never drop a file. Bookkeeping is excluded, it is regenerated or dropped by design.</summary>
    private static LintCheck NoFilesLost(MapDocument doc, MapDocument original)
    {
        var have = doc.Files.Where(f => f.FileName is not null)
            .Select(f => f.FileName!).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var lost = original.Files
            .Where(f => f.FileName is not null && !IsBookkeeping(f.FileName))
            .Select(f => f.FileName!)
            .Where(n => !have.Contains(n))
            .ToList();

        return lost.Count == 0
            ? new("no-files-lost", LintSeverity.Ok, "no file from the original is missing", Array.Empty<string>())
            : new("no-files-lost", LintSeverity.Error, $"{lost.Count} file(s) present in the original are gone", Capped(lost));
    }

    /// <summary>
    /// A port must not replace the TARGET's own assets with same-named ones from the source.
    /// Those files belong to the target's other content, so overwriting them changes how
    /// unrelated units and doodads look, or stops them loading at all.
    /// </summary>
    private static LintCheck NoTargetAssetsClobbered(MapDocument doc, MapDocument original)
    {
        // Grouped, not ToDictionary. An MPQ can hold two entries under one name, and one map in
        // the measured library of 34 holds 14 such pairs, which made this throw and took the whole
        // lint down. The first copy is the one a name lookup resolves to, so it is the one
        // compared, and that is what a clobber check is asking about.
        var before = original.Files
            .Where(f => f.FileName is not null && !IsBookkeeping(f.FileName) && !IsStructural(f.FileName))
            .GroupBy(f => f.FileName!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().CurrentBytes, StringComparer.OrdinalIgnoreCase);

        var clobbered = new List<string>();
        foreach (var f in doc.Files)
        {
            if (f.FileName is null || !before.TryGetValue(f.FileName, out var was)) continue;
            var now = f.CurrentBytes;
            if (!now.AsSpan().SequenceEqual(was)) clobbered.Add(f.FileName);
        }

        return clobbered.Count == 0
            ? new("no-assets-clobbered", LintSeverity.Ok, "no pre-existing asset was overwritten", Array.Empty<string>())
            : new("no-assets-clobbered", LintSeverity.Error,
                $"{clobbered.Count} of the target's own asset(s) were overwritten by same-named source files. "
                + "The target's other content renders with these.", Capped(clobbered));
    }

    /// <summary>
    /// Paths the map's own war3map.imp claims to import, in both the spelling the table uses and
    /// the archive spelling WorldEdit prepends, so a membership test hits either way.
    /// </summary>
    /// <remarks>
    /// This used to hand-roll the .imp parse by scanning NUL-delimited runs from offset 8, and it
    /// was wrong in a way that produced a lot of confident noise. The format is a flag byte
    /// followed by a NUL-terminated path, and the scan accumulated the FLAG BYTE into the path, so
    /// every path came out with a leading control character and none of them ever resolved.
    ///
    /// On FgoRD_1.11 that reported 1,124 problems, 562 "listed but not in the archive" and the same
    /// 562 as "in the archive but not listed", which is the tell, the same files counted twice from
    /// both directions. The canonical reader says the real number is ONE. ImportsCommand had always
    /// parsed it properly through War3Net's model and knew about the war3mapImported\ prefix, so
    /// the duplicate was not only wrong but unnecessary.
    /// </remarks>
    private static HashSet<string> ImportedPathsOf(MapDocument doc)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in ImportsCommand.Execute(doc).Entries)
        {
            if (!e.InManifest) continue;
            set.Add(e.Path);
            if (!e.Path.StartsWith(ImportsCommand.DefaultImportPrefix, StringComparison.OrdinalIgnoreCase))
                set.Add(ImportsCommand.DefaultImportPrefix + e.Path);
        }
        return set;
    }

    private static bool IsBookkeeping(string name) =>
        name is "(listfile)" or "(attributes)" or "(signature)";

    /// <summary>Structural files legitimately change on an edit, unlike assets.</summary>
    private static bool IsStructural(string name) =>
        Path.GetExtension(name).ToLowerInvariant() is ".j" or ".lua" or ".imp" or ".wts"
            or ".w3i" or ".w3u" or ".w3t" or ".w3a" or ".w3b" or ".w3d" or ".w3q"
            or ".w3h" or ".w3c" or ".w3r" or ".w3s" or ".doo" or ".w3e" or ".wpm"
            or ".shd" or ".wtg" or ".wct" or ".mmp";

    private static IReadOnlyList<string> Capped(List<string> items) =>
        items.Count <= 25 ? items : items.Take(25).Append($"... and {items.Count - 25} more").ToList();
}
