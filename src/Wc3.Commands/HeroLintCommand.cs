// src/Wc3.Commands/HeroLintCommand.cs
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record HeroLintResult(
    string Directory,
    string? Id,
    IReadOnlyList<LintCheck> Checks)
{
    public int Errors => Checks.Count(c => c.Severity == LintSeverity.Error);
    public int Warnings => Checks.Count(c => c.Severity == LintSeverity.Warning);
    public bool Ok => Errors == 0;
}

/// <summary>
/// Validates a <see cref="HeroDefinition"/> before it is installed anywhere.
///
/// A definition is the artifact people keep, share and edit by hand, so it is worth catching a
/// problem at authoring time rather than after it has been written into someone's map. Install
/// mutates a target; lint does not, so this is the cheap check that should always run first.
///
/// The hash check is the important one. Assets carry a SHA-256 precisely so an install can tell
/// "the target already has exactly this file" from "a DIFFERENT file lives at this path" and
/// refuse the second case. If a definition's stored hash no longer matches its own bytes, that
/// distinction silently stops working and the guard against clobbering a target's assets is gone.
/// </summary>
public static class HeroLintCommand
{
    public static HeroLintResult Run(string directory)
    {
        var checks = new List<LintCheck>();
        var jsonPath = Path.Combine(directory, "hero.json");
        if (!File.Exists(jsonPath))
            return new(directory, null, new[]
            {
                new LintCheck("definition-present", LintSeverity.Error,
                    "no hero.json in this folder", Array.Empty<string>()),
            });

        HeroDefinition? def;
        try { def = JsonSerializer.Deserialize<HeroDefinition>(File.ReadAllText(jsonPath)); }
        catch (JsonException ex)
        {
            return new(directory, null, new[]
            {
                new LintCheck("definition-parses", LintSeverity.Error,
                    $"hero.json could not be parsed: {ex.Message}", Array.Empty<string>()),
            });
        }
        if (def is null)
            return new(directory, null, new[]
            {
                new LintCheck("definition-parses", LintSeverity.Error,
                    "hero.json deserialized to nothing", Array.Empty<string>()),
            });

        checks.Add(def.SchemaVersion <= HeroDefinition.CurrentSchemaVersion
            ? new("schema-version", LintSeverity.Ok,
                $"schema v{def.SchemaVersion}", Array.Empty<string>())
            : new("schema-version", LintSeverity.Error,
                $"schema v{def.SchemaVersion} is newer than this build understands "
                + $"(v{HeroDefinition.CurrentSchemaVersion})", Array.Empty<string>()));

        checks.Add(RootPresent(def));
        checks.Add(ObjectsInstallable(def));
        checks.Add(AssetsIntact(def, directory));
        checks.Add(ScriptPresent(def, directory));
        checks.Add(StubsMatchTheScript(def, directory));
        checks.Add(FieldsReferenceCarriedObjects(def));

        return new HeroLintResult(directory, def.Id, checks);
    }

    private static LintCheck RootPresent(HeroDefinition def)
    {
        var roots = def.Objects.Where(o => o.Origin == "root").ToList();
        if (roots.Count == 1 && roots[0].Rawcode == def.Id)
            return new("root-object", LintSeverity.Ok,
                $"root {def.Id} \"{def.Name}\" present", Array.Empty<string>());
        return new("root-object", LintSeverity.Error,
            roots.Count == 0 ? $"no object is marked root, so nothing identifies {def.Id}"
                             : $"{roots.Count} objects claim to be the root",
            roots.Select(r => r.Rawcode).ToList());
    }

    /// <summary>
    /// Install creates each object from its base rawcode, so an object without one cannot be
    /// installed at all. Catching that here beats discovering it halfway through writing a map.
    /// </summary>
    private static LintCheck ObjectsInstallable(HeroDefinition def)
    {
        var bad = def.Objects.Where(o => string.IsNullOrWhiteSpace(o.BaseRawcode))
            .Select(o => $"{o.Rawcode} ({o.Kind}) has no base rawcode").ToList();
        return bad.Count == 0
            ? new("objects-installable", LintSeverity.Ok,
                $"all {def.Objects.Count} object(s) have a base to derive from", Array.Empty<string>())
            : new("objects-installable", LintSeverity.Error,
                $"{bad.Count} object(s) cannot be created by install", Cap(bad));
    }

    private static LintCheck AssetsIntact(HeroDefinition def, string directory)
    {
        var problems = new List<string>();
        foreach (var a in def.Assets)
        {
            var disk = Path.Combine(directory, "assets",
                a.Path.Replace('\\', Path.DirectorySeparatorChar));
            if (!File.Exists(disk)) { problems.Add($"{a.Path}: file missing from assets/"); continue; }
            var bytes = File.ReadAllBytes(disk);
            if (bytes.Length != a.SizeBytes)
                problems.Add($"{a.Path}: {bytes.Length} bytes on disk, {a.SizeBytes} recorded");
            var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            if (!hash.Equals(a.Sha256, StringComparison.OrdinalIgnoreCase))
                problems.Add($"{a.Path}: hash does not match the recorded one");
        }
        return problems.Count == 0
            ? new("assets-intact", LintSeverity.Ok,
                $"all {def.Assets.Count} asset(s) present and hash-matched", Array.Empty<string>())
            : new("assets-intact", LintSeverity.Error,
                $"{problems.Count} asset problem(s); install's collision guard depends on these hashes",
                Cap(problems));
    }

    private static LintCheck ScriptPresent(HeroDefinition def, string directory)
    {
        if (def.ScriptFile is null)
            return new("script", LintSeverity.Warning,
                "no script carried, so this hero's abilities will have no behaviour",
                Array.Empty<string>());
        var path = Path.Combine(directory, def.ScriptFile);
        if (!File.Exists(path))
            return new("script", LintSeverity.Error,
                $"hero.json names '{def.ScriptFile}' but it is not in the folder", Array.Empty<string>());

        // Latin-1, matching how export wrote it. Decoding a script that is really UTF-8 gives code
        // points above U+00FF and decoding one that is neither gives U+FFFD, and both make this
        // check answer a question about text the definition does not actually contain.
        var text = File.ReadAllText(path, Encoding.Latin1);
        var missing = def.ScriptEntryPoints
            .Where(fn => !text.Contains("function " + fn, StringComparison.Ordinal))
            .ToList();
        return missing.Count == 0
            ? new("script", LintSeverity.Ok,
                $"{def.ScriptEntryPoints.Count} declared function(s) all present", Array.Empty<string>())
            : new("script", LintSeverity.Error,
                $"{missing.Count} declared entry point(s) are not in the script", Cap(missing));
    }

    /// <summary>
    /// Every recorded stub must really be in script.j under the name recorded for it, because that
    /// record is what install uses to swap a no-op for the target's own implementation.
    /// </summary>
    /// <remarks>
    /// If the record and the script disagree, install cannot delete the stub it is about to rename
    /// references away from. Install refuses that bind rather than risk two declarations of one
    /// name, so a stale record silently costs a hero its infrastructure. Catching it here, where
    /// nothing has been written to anyone's map yet, is the cheap version of that discovery.
    ///
    /// The tally is worth printing even when everything checks out: a stub is a placeholder, and
    /// how many of them can bind is the difference between a hero that works on a given target and
    /// one that loads and then does nothing.
    /// </remarks>
    private static LintCheck StubsMatchTheScript(HeroDefinition def, string directory)
    {
        var stubs = def.Stubs;
        if (stubs is null || stubs.Count == 0)
            return new("script-stubs", LintSeverity.Ok,
                "no stubs recorded, so every called function is carried or was exported by an "
                + "older build", Array.Empty<string>());

        if (def.ScriptFile is null || !File.Exists(Path.Combine(directory, def.ScriptFile)))
            return new("script-stubs", LintSeverity.Error,
                $"{stubs.Count} stub(s) are recorded but there is no script to hold them",
                Array.Empty<string>());

        // Latin-1, matching how export wrote it and how install reads it. Decoding a script as
        // UTF-8 when it is not replaces bytes with U+FFFD and can hide a name behind a mangled one.
        var text = File.ReadAllText(Path.Combine(directory, def.ScriptFile), Encoding.Latin1);
        var declared = JassFunctionIndex.Parse(text).Select(f => f.Name).ToHashSet(StringComparer.Ordinal);
        var missing = stubs.Where(s => !declared.Contains(s.CarriedName))
            .Select(s => $"{s.CarriedName} ({s.WurstClass}) is recorded as a stub but not declared")
            .ToList();
        if (missing.Count > 0)
            return new("script-stubs", LintSeverity.Error,
                $"{missing.Count} of {stubs.Count} recorded stub(s) are not in the script, so install "
                + "cannot bind them to a target's own implementation", Cap(missing));

        int peer = stubs.Count(s => s.Peer);
        int stateBound = stubs.Count(s => !s.Peer && s.ClassStateCarried);
        int bindable = stubs.Count - peer - stateBound;
        var perClass = stubs.GroupBy(s => s.WurstClass, StringComparer.Ordinal)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => $"{g.Key}: {g.Count()} stub(s), "
                         + (g.Any(s => s.Peer) ? "peer, always stubbed"
                            : g.Any(s => s.ClassStateCarried)
                              ? "instance tables carried here, CANNOT bind"
                              : "bindable if the target declares them"))
            .ToList();
        return new("script-stubs", LintSeverity.Warning,
            $"{stubs.Count} call(s) are no-op stubs, not implementations. {bindable} bindable to a "
            + $"target that declares them, {peer} peer (always stubbed), {stateBound} unbindable "
            + "because this definition carries the class's own instance tables", Cap(perClass));
    }

    /// <summary>
    /// A field naming a rawcode this definition does not carry will still install, but the object
    /// it points at will not exist in the target, so the ability or unit silently does nothing.
    /// Reported as a warning because a base-game rawcode is a legitimate reference.
    /// </summary>
    private static LintCheck FieldsReferenceCarriedObjects(HeroDefinition def)
    {
        var carried = def.Objects.Select(o => o.Rawcode).ToHashSet(StringComparer.Ordinal);
        var dangling = new List<string>();
        foreach (var o in def.Objects)
            foreach (var f in o.Fields)
                foreach (var token in f.Value.Split(',', StringSplitOptions.TrimEntries))
                    if (token.Length == 4 && token.All(char.IsLetterOrDigit)
                        && char.IsUpper(token[0]) && !carried.Contains(token))
                        dangling.Add($"{o.Rawcode}.{f.Code} -> {token}");

        return dangling.Count == 0
            ? new("field-references", LintSeverity.Ok,
                "no field points at an object this definition does not carry", Array.Empty<string>())
            : new("field-references", LintSeverity.Warning,
                $"{dangling.Count} field value(s) name a rawcode not carried here; a base-game code "
                + "is fine, a custom one will be missing in the target", Cap(dangling.Distinct().ToList()));
    }

    private static IReadOnlyList<string> Cap(List<string> items) =>
        items.Count <= 20 ? items : items.Take(20).Append($"... and {items.Count - 20} more").ToList();
}
