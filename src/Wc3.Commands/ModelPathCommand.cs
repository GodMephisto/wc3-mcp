// src/Wc3.Commands/ModelPathCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One model path the map asks the engine to load that resolves to no file. <see cref="Source"/>
/// is "script" or the object data file that holds it. <see cref="Suggestion"/> is set only when
/// exactly one file in the archive is plainly what the author meant.
/// </summary>
public sealed record MissingModel(
    string Source,
    ObjectKind? Kind,
    string? Rawcode,
    string? Field,
    string Path,
    int Uses,
    bool Placeholder,
    string? Suggestion);

public sealed record ModelPathScan(
    int Examined,
    int ResolvedByMap,
    int ResolvedByGame,
    int BaseGameUnchecked,
    IReadOnlyList<MissingModel> Missing,
    IReadOnlyList<string> Diagnostics);

public sealed record ModelPathRepairResult(
    ModelPathScan Scan,
    IReadOnlyList<string> Changes,
    IReadOnlyList<MissingModel> LeftAlone);

/// <summary>
/// Model paths a map references that the engine cannot load, and the repair for the ones with an
/// unambiguous answer.
///
/// The engine does not stop on a missing model. It logs "model creation failed", draws nothing,
/// and tries again the next time the same effect, unit or destructable is created, so a bad path
/// inside a spell that runs on a timer costs a failed lookup every tick. Measured on Anime WOS2
/// 0.32d on 2026-09-26, the game logged 4,754 such failures in a 12 minute match, and 4,754 of
/// them were one destructable, B017, whose model field reads ".mdl .mdl". A spell removes and
/// recreates it on every tick because a destructable cannot be moved.
///
/// Three outcomes per path. It resolves inside the archive. It resolves in the installed game.
/// Or it resolves nowhere, and then it is either a placeholder (no file name at all, the common
/// idiom for "no model") or a real name that is simply wrong. Only the last two are reported.
///
/// Lookups follow the engine. Case does not matter, a forward slash is a backslash, and a
/// request for .mdl is served by the .mdx of the same name, which is how Reforged ships them.
/// </summary>
public static class ModelPathCommand
{
    /// <summary>Where the repair points a placeholder, and the file it adds for that.</summary>
    public const string EmptyModelPath = @"war3mapImported\wc3ctl_empty.mdx";
    private const string EmptyModelReference = @"war3mapImported\wc3ctl_empty.mdl";

    // Fields that hold a model path even when the value carries no extension, which ability art
    // commonly does. Any OTHER field is still examined when its value ends in .mdl or .mdx.
    private static readonly HashSet<string> ModelFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "umdl", "ua1m", "ua2m", "uspa",           // unit model, attack missiles, special art
        "atat", "acat", "aeat", "asat", "amat",   // ability target, caster, effect, special, missile art
        "ftat", "fsat", "feft",                   // buff target, special, effect art
        "ifil", "bfil", "dfil",                   // item, destructable, doodad model
    };

    // A JASS string literal, with \" and \\ escapes.
    private static readonly Regex JassString = new(@"""((?:[^""\\]|\\.)*)""", RegexOptions.Compiled);
    /// <summary>
    /// A literal is a path only when it stands alone. One joined to anything by + is a fragment of
    /// a path built at runtime, such as ".mdx" after I2S(i), and is neither reported nor rewritten.
    /// </summary>
    private static bool Standalone(string text, Match literal)
    {
        int before = literal.Index - 1;
        while (before >= 0 && text[before] is ' ' or '\t') before--;
        int after = literal.Index + literal.Length;
        while (after < text.Length && text[after] is ' ' or '\t') after++;
        return !(before >= 0 && text[before] == '+') && !(after < text.Length && text[after] == '+');
    }

    private static readonly Regex ModelExtension = new(@"\.(mdl|mdx)\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ExtensionAnywhere = new(@"\.(mdl|mdx)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Prefix changes that turned up between a script's path and the file actually shipped, on
    // WOS2 0.32d. The import tool there prefixed files with wos_ and some with wos_JY-, while
    // the script kept the old names.
    private static readonly string[] Prefixes = { "wos_", "jy-", "wos_jy-" };

    public static ModelPathScan Scan(MapDocument doc, string? gameDirOverride = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var diagnostics = new List<string>();
        GameData.GameDataContext? game = null;
        if (GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diag)) game = ctx;
        else diagnostics.Add($"base-game model paths not checked, game data unavailable ({diag})");

        var gameCache = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        var archiveModels = ArchiveModelsByStem(doc);
        int examined = 0, byMap = 0, byGame = 0, unchecked_ = 0;
        var missing = new List<MissingModel>();

        void Consider(string source, ObjectKind? kind, string? rawcode, string? field, string path, int uses)
        {
            examined += uses;
            if (IsPlaceholder(path))
            {
                missing.Add(new(source, kind, rawcode, field, path, uses, true, null));
                return;
            }
            if (Candidates(path).Any(doc.HasFileByName)) { byMap += uses; return; }
            if (!LooksImported(path))
            {
                if (game is null) { unchecked_ += uses; return; }
                if (!gameCache.TryGetValue(path, out bool inGame))
                    gameCache[path] = inGame = Candidates(path).Any(c =>
                        game.TryReadFile(c, out _) || game.TryReadFile("war3.w3mod:_hd.w3mod:" + c, out _));
                if (inGame) { byGame += uses; return; }
            }
            missing.Add(new(source, kind, rawcode, field, path, uses, false, Suggest(path, archiveModels)));
        }

        // The script, one entry per distinct literal, counting every occurrence.
        if (ScriptFile(doc) is { } script)
        {
            foreach (var g in JassString.Matches(script.Text).Where(m => Standalone(script.Text, m))
                         .Select(m => m.Groups[1].Value)
                         .Where(s => ModelExtension.IsMatch(s)).GroupBy(s => s, StringComparer.Ordinal))
                Consider("script", null, null, null, Unescape(g.Key), g.Count());
        }
        else diagnostics.Add("no war3map.j or war3map.lua found, the script was not examined");

        // Object data, the effective value after the skin layer overlays the map layer.
        foreach (var kind in ObjectKinds.All)
        {
            var info = ObjectKinds.Info(kind);
            foreach (var e in ObjectKinds.MergedEntries(doc, info))
            {
                string holder = e.Mods.Any(m => IsSkinMod(doc, info, e.Id, m.Key)) ? info.SkinFile : info.MapFile;
                foreach (var (key, value) in ObjectKinds.ModsToDict(e.Mods))
                {
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    string code = key.Split(':')[0];
                    bool modelField = ModelFields.Contains(code);
                    foreach (var part in value.Split(','))
                    {
                        // "_" is how an SLK spells an empty value, it names no file.
                        if (part.Length == 0 || part.Trim() == "_") continue;
                        if (!modelField && !ModelExtension.IsMatch(part)) continue;
                        if (modelField && part.Trim().Length == 0) continue;
                        Consider(holder, kind, e.Id.ToRawcode(), key, part, 1);
                    }
                }
            }
        }

        diagnostics.Add($"examined {examined} model reference(s), {byMap} resolved by the archive, "
            + $"{byGame} by the installed game"
            + (unchecked_ > 0 ? $", {unchecked_} base-game path(s) not checked" : ""));
        var ordered = missing.OrderByDescending(m => m.Placeholder).ThenByDescending(m => m.Uses)
            .ThenBy(m => m.Path, StringComparer.OrdinalIgnoreCase).ToList();
        return new ModelPathScan(examined, byMap, byGame, unchecked_, ordered, diagnostics);
    }

    /// <summary>
    /// Rewrites every missing path that has exactly one intended target, and, when
    /// <paramref name="emptyModel"/> is set, points every placeholder at a bundled model with no
    /// geometry so it still draws nothing but no longer fails to load. Paths with no single
    /// answer are left alone and returned, because guessing at another author's effect would
    /// replace one wrong model with a different wrong one.
    /// </summary>
    public static ModelPathRepairResult Repair(MapDocument doc, string? gameDirOverride = null, bool emptyModel = true)
    {
        var scan = Scan(doc, gameDirOverride);
        var changes = new List<string>();
        var left = new List<MissingModel>();
        string? scriptText = null;
        var script = ScriptFile(doc);
        bool emptyAdded = false;

        foreach (var m in scan.Missing)
        {
            string? target = m.Placeholder ? (emptyModel ? EmptyModelReference : null) : m.Suggestion;
            if (target is null) { left.Add(m); continue; }

            if (m.Placeholder && !emptyAdded)
            {
                FileEditCommand.AddOrReplace(doc, EmptyModelPath, EmptyModel());
                emptyAdded = true;
                changes.Add($"added {EmptyModelPath}, {EmptyModel().Length} bytes, a model with no geometry");
            }

            if (m.Source == "script")
            {
                if (script is null) { left.Add(m); continue; }
                scriptText ??= script.Text;
                string from = Escape(m.Path), to = "\"" + Escape(target) + "\"";
                string current = scriptText;
                scriptText = JassString.Replace(current, x =>
                    x.Groups[1].Value == from && Standalone(current, x) ? to : x.Value);
                changes.Add($"script, {m.Uses} use(s) of '{m.Path}' now '{target}'");
            }
            else
            {
                var merged = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(m.Kind!.Value))
                    .First(e => e.Id.ToRawcode() == m.Rawcode);
                string current = ObjectKinds.ModsToDict(merged.Mods)[m.Field!];
                string value = string.Join(",", current.Split(',').Select(p => p == m.Path ? target : p));
                var r = ObjectSetCommand.Execute(doc, m.Kind.Value, m.Rawcode!, m.Field!, value);
                if (!r.Ok) { left.Add(m); continue; }
                changes.Add($"{m.Kind.Value.ToString().ToLowerInvariant()} {m.Rawcode} {m.Field}, "
                    + $"'{m.Path}' now '{target}', {r.Message}");
            }
        }

        if (script is not null && scriptText is not null && scriptText != script.Text)
            doc.TryReplaceFileByName(script.Name, Encoding.Latin1.GetBytes(scriptText));
        return new ModelPathRepairResult(scan, changes, left);
    }

    /// <summary>
    /// Blizzard's own empty model, byte for byte. The game ships it as
    /// <c>doodads\cinematic\empty\empty.mdx</c> (536 bytes, in the _de.w3mod layer of build
    /// 24268), a VERS of 1800, a MODL named "Empty" with zero extents, and one SEQS entry,
    /// "Stand" over 0 to 300. No geometry, bones or emitters, so it loads and draws nothing.
    /// It is rebuilt here rather than referenced by path because that layer is not one the
    /// SD client is guaranteed to search. A GameData test compares the two byte for byte.
    /// </summary>
    public static byte[] EmptyModel()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(Encoding.ASCII.GetBytes("MDLX"));
        w.Write(Encoding.ASCII.GetBytes("VERS")); w.Write(4u); w.Write(1800u);
        w.Write(Encoding.ASCII.GetBytes("MODL")); w.Write(372u);
        w.Write(Fixed("Empty", 80));
        w.Write(new byte[260]);                   // animation file name
        for (int i = 0; i < 7; i++) w.Write(0f);  // bounds radius, minimum, maximum
        w.Write(150u);                            // blend time
        w.Write(Encoding.ASCII.GetBytes("SEQS")); w.Write(132u);
        w.Write(Fixed("Stand", 80));
        w.Write(0u); w.Write(300u);               // interval
        w.Write(0f);                              // move speed
        w.Write(0u);                              // flags, looping
        w.Write(0f);                              // rarity
        w.Write(0u);                              // sync point
        for (int i = 0; i < 7; i++) w.Write(0f);  // extent
        w.Flush();
        return ms.ToArray();

        static byte[] Fixed(string s, int n)
        {
            var b = new byte[n];
            Encoding.ASCII.GetBytes(s).CopyTo(b, 0);
            return b;
        }
    }

    // ---- helpers -----------------------------------------------------------------

    /// <summary>A path with no file name left once every .mdl and .mdx is removed.</summary>
    internal static bool IsPlaceholder(string path)
    {
        string file = path.Replace('/', '\\').Split('\\').Last();
        return ExtensionAnywhere.Replace(file, "").Trim().Length == 0;
    }

    private static bool LooksImported(string path) =>
        path.Contains("war3mapimported", StringComparison.OrdinalIgnoreCase)
        || !path.Replace('/', '\\').Contains('\\');

    /// <summary>The names the engine would try for a path, .mdl and .mdx both.</summary>
    internal static IEnumerable<string> Candidates(string path)
    {
        string p = path.Trim().Replace('/', '\\');
        string stem = ModelExtension.Replace(p, "");
        yield return p;
        yield return stem + ".mdx";
        yield return stem + ".mdl";
    }

    private static string Stem(string path)
    {
        string file = path.Trim().Replace('/', '\\').Split('\\').Last();
        return ModelExtension.Replace(file, "").Trim().ToLowerInvariant();
    }

    private static Dictionary<string, List<string>> ArchiveModelsByStem(MapDocument doc)
    {
        var by = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in doc.Files)
        {
            string? name = f.FileName ?? f.RecoveredFileName;
            if (name is null || !ModelExtension.IsMatch(name)) continue;
            string s = Stem(name);
            if (!by.TryGetValue(s, out var list)) by[s] = list = new List<string>();
            list.Add(name);
        }
        return by;
    }

    /// <summary>
    /// The one archive model a broken path plainly meant, or null. The file name is compared
    /// with the folder dropped, which undoes a doubled war3mapImported folder or a prefix glued
    /// onto the folder, and with a wos_ / JY- prefix added or removed. More than one hit is
    /// ambiguous and returns null.
    /// </summary>
    private static string? Suggest(string path, Dictionary<string, List<string>> archive)
    {
        string s = Stem(path);
        var variants = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { s };
        foreach (var pre in Prefixes)
        {
            variants.Add(pre + s);
            if (s.StartsWith(pre, StringComparison.OrdinalIgnoreCase)) variants.Add(s[pre.Length..]);
        }
        foreach (var v in variants.ToList())
            if (v.StartsWith("wos_", StringComparison.OrdinalIgnoreCase)) variants.Add("wos_jy-" + v[4..]);
        var hits = variants.SelectMany(v => archive.GetValueOrDefault(v) ?? new List<string>())
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (hits.Count != 1) return null;
        // Keep the author's .mdl or .mdx spelling, the engine serves either from the same file.
        string ext = ModelExtension.Match(path) is { Success: true } m ? m.Value.Trim() : "";
        return ext.Length > 0 ? ModelExtension.Replace(hits[0], ext) : ModelExtension.Replace(hits[0], "");
    }

    private static bool IsSkinMod(MapDocument doc, ObjectKindInfo info, int id, string key) =>
        doc.GetFile(info.SkinFile)?.Model is { } skin
        && ObjectDataWriter.AccessFor(skin) is { } access
        && access.FindGroup(id) is { } group
        && access.FindMod(group, key.Split(':')[0].FromRawcode(),
               key.Contains(':') && int.TryParse(key.Split(':')[1], out var n) ? n : 0) is not null;

    private sealed record Script(string Name, string Text);

    private static Script? ScriptFile(MapDocument doc)
    {
        foreach (var name in new[] { "war3map.j", "war3map.lua", @"scripts\war3map.j", @"scripts\war3map.lua" })
            if (doc.TryReadFileByName(name, out var bytes) && bytes.Length > 0)
                return new Script(name, Encoding.Latin1.GetString(bytes));
        return null;
    }

    private static string Unescape(string literal) => literal.Replace(@"\\", @"\").Replace("\\\"", "\"");
    private static string Escape(string path) => path.Replace(@"\", @"\\").Replace("\"", "\\\"");
}
