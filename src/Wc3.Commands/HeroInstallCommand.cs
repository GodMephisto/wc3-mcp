// src/Wc3.Commands/HeroInstallCommand.cs
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record InstallResult(
    bool Ok,
    string Message,
    IReadOnlyDictionary<string, string> RawcodeRemap,
    int ObjectsCreated,
    int FieldsApplied,
    int AssetsWritten,
    int AssetsSkippedIdentical,
    IReadOnlyList<string> Collisions,
    IReadOnlyList<string> UnmetRequirements,
    IReadOnlyList<string> NextSteps,
    string? RegisteredWith,
    IReadOnlyList<string> Assumptions,
    /// <summary>
    /// One line per Wurst class the export stubbed, saying how many of its calls were BOUND to the
    /// target's own functions and how many are still no-op stubs, with the reason. Reported rather
    /// than summarised away, because a stubbed call is a hero that half works and the difference is
    /// invisible in the finished map.
    /// </summary>
    IReadOnlyList<string> ScriptBindings);

/// <summary>
/// Installs a <see cref="HeroDefinition"/> into any map, the <c>install</c> verb of the format.
///
/// Three rules here exist because their absence caused real, expensive failures:
///
/// ASSETS ARE NEVER CLOBBERED. The porter silently overwrote six of a target map's own textures
/// and a model with same-named files from the source, changing how unrelated heroes rendered. An
/// asset whose hash matches is skipped as already present; one that differs is REFUSED and
/// reported, never overwritten.
///
/// RAWCODES ARE REMAPPED, NOT ASSUMED. The target may already use the definition's codes, so each
/// object is created from its base and the resulting code recorded. The script is rewritten
/// through that map, so nothing depends on a code surviving.
///
/// REQUIREMENTS ARE CHECKED BEFORE WRITING. A hero can have perfect objects, assets and script and
/// still not exist to the player, because a map builds its roster from its own script. This looks
/// for the target's registration call and reports it as an unmet requirement rather than producing
/// a map that silently lacks the hero. That exact failure cost a full day.
/// </summary>
public static class HeroInstallCommand
{
    public static InstallResult Run(string definitionDirectory, MapDocument target,
        bool force = false, string? gameDir = null, string? role = null,
        bool keepSourceStats = false)
    {
        ArgumentNullException.ThrowIfNull(target);
        var jsonPath = Path.Combine(definitionDirectory, "hero.json");
        if (!File.Exists(jsonPath))
            return Fail($"no hero.json in '{definitionDirectory}'");

        HeroDefinition? def;
        try { def = JsonSerializer.Deserialize<HeroDefinition>(File.ReadAllText(jsonPath)); }
        catch (JsonException ex) { return Fail($"hero.json could not be parsed: {ex.Message}"); }
        if (def is null) return Fail("hero.json could not be parsed");
        if (def.SchemaVersion > HeroDefinition.CurrentSchemaVersion)
            return Fail($"hero.json is schema v{def.SchemaVersion}, this build understands "
                        + $"v{HeroDefinition.CurrentSchemaVersion}");

        // 1. Assets first, because a refused collision should abort before anything is created.
        var collisions = new List<string>();
        var pending = new List<(string path, byte[] bytes)>();
        int skipped = 0;
        foreach (var a in def.Assets)
        {
            var disk = Path.Combine(definitionDirectory, "assets",
                a.Path.Replace('\\', Path.DirectorySeparatorChar));
            if (!File.Exists(disk)) { collisions.Add($"{a.Path}: missing from the definition folder"); continue; }
            var bytes = File.ReadAllBytes(disk);

            if (target.GetFile(a.Path) is { } existing)
            {
                var have = Convert.ToHexString(SHA256.HashData(existing.CurrentBytes))
                    .ToLowerInvariant();
                if (have == a.Sha256) { skipped++; continue; }   // already exactly this file
                collisions.Add($"{a.Path}: the target has a DIFFERENT file at this path");
                continue;
            }
            pending.Add((a.Path, bytes));
        }
        if (collisions.Count > 0 && !force)
            return new(false,
                $"{collisions.Count} asset collision(s); the target's own files would be overwritten. "
                + "Re-run with --force only if you are certain, or rename the definition's assets.",
                new Dictionary<string, string>(), 0, 0, 0, skipped, collisions,
                Array.Empty<string>(), Array.Empty<string>(), null, Array.Empty<string>(),
                Array.Empty<string>());

        // 2. Objects. Created from their base, so the target's existing codes are never disturbed.
        var remap = new Dictionary<string, string>(StringComparer.Ordinal);
        var plan = new List<(ObjectKind Kind, string Code, DefinitionObject Object)>();
        int created = 0, fields = 0;
        foreach (var o in def.Objects)
        {
            if (o.BaseRawcode is null) continue;
            if (!Enum.TryParse<ObjectKind>(o.Kind, ignoreCase: true, out var kind)) continue;
            var made = ObjectNewCommand.Execute(target, kind, o.BaseRawcode);
            if (!made.Ok || made.NewRawcode is null) continue;
            remap[o.Rawcode] = made.NewRawcode;
            created++;
            plan.Add((kind, made.NewRawcode, o));
        }

        // Fields are applied in a SECOND pass, after every object exists and the remap is complete.
        // A field value can name another carried rawcode, and a hero's ability list is exactly
        // that: uhab = "A1QZ,A1R1,...". Writing it verbatim pointed the installed hero at the
        // SOURCE map's ability codes, which do not exist in the target, so she arrived with no
        // abilities at all. Applying fields inside the creation loop could not have worked either,
        // since an object referenced later in the list has not been created yet.
        foreach (var (kind, code, o) in plan)
            foreach (var f in o.Fields)
                if (ObjectSetCommand.Execute(target, kind, code, f.Code, RemapRawcodes(f.Value, remap)).Ok)
                    fields++;

        // 3. Assets, now that nothing has been refused. Each one also gets a war3map.imp entry:
        // an archive file the import table does not declare is a real inconsistency, and 'lint'
        // reports it, so writing the bytes without the entry only moves the bug.
        foreach (var (path, bytes) in pending)
            FileEditCommand.AddOrReplace(target, path, bytes);
        if (pending.Count > 0) AddImportEntries(target, pending.Select(p => p.path));

        // 4. Script, rewritten through the remap so no carried code depends on an original code.
        var bindings = new List<string>();
        var stubUnmet = new List<string>();
        var castDispatchWired = new List<string>();
        if (def.ScriptFile is not null)
        {
            var scriptDisk = Path.Combine(definitionDirectory, def.ScriptFile);
            if (File.Exists(scriptDisk))
            {
                // Latin-1 both ways. A definition's script.j can carry bytes that are not valid
                // UTF-8 (they came out of a map whose script is not UTF-8 either), and a UTF-8
                // round-trip turns each into '?', which pjass rejects as "Unrecognized character".
                var body = File.ReadAllText(scriptDisk, ScriptBytes);
                foreach (var (from, to) in remap)
                    body = body.Replace($"'{from}'", $"'{to}'", StringComparison.Ordinal);
                var existing = target.GetFile("war3map.j");
                var head = existing is null ? "" :
                    ScriptBytes.GetString(existing.CurrentBytes);

                // Bind before the globals go in, so a bound class's calls point at the target's own
                // implementation rather than at a private no-op the definition brought with it.
                body = BindStubs(body, head, def, bindings, stubUnmet);

                // Globals must land INSIDE the target's own globals block; appended after it they
                // are a syntax error, and the carried functions that read them will not compile.
                head = InsertGlobals(head, def.Globals, remap);

                // The closure brings the source map's cast dispatcher but not the trigger that
                // drove it, because a trigger is a runtime handle and not a function. Measured on
                // GGGA: the carried dispatcher was declared, referenced nowhere, and every one of
                // the hero's spell-start functions was unreachable. Wire it, gated to this hero.
                string installedForDispatch = remap.TryGetValue(def.Id, out var dispatchCode) ? dispatchCode : def.Id;
                var wirings = CarriedCastDispatch.Wire(head, body, installedForDispatch);
                if (wirings.Count > 0)
                {
                    body += string.Concat(wirings.Select(w => w.GeneratedScript));
                    head = CarriedCastDispatch.InsertIntoMain(head, wirings.Select(w => w.InitCall));
                    foreach (var w in wirings)
                        castDispatchWired.Add(w.ConditionFunction);
                }
                // Latin-1 out, not FileEditCommand.WriteText, which is UTF-8. The head was DECODED
                // as Latin-1, so re-encoding it as UTF-8 turns each of the target's 51,779
                // non-ASCII bytes into two, silently mangling every localised string and author
                // name in the map. Measured on WOS2 before this line was fixed: the installed
                // war3map.j carried 103,572 non-ASCII bytes where the original had 51,779. The
                // read was switched to Latin-1 earlier, the write was not, and half a fix here is
                // still a corrupted map.
                // Through ScriptCommand.Write, so it lands on the entry it was read from. The
                // hardcoded root name left the 13 of 34 measured maps that keep their script at
                // scripts\war3map.j holding two disagreeing scripts.
                ScriptCommand.Write(target, ScriptBytes.GetBytes(
                    head + "\n\n// ==== wc3ctl hero: " + def.Name + " (" + def.Id + ") ====\n" + body));
            }
        }

        // 5. Requirements. Reported, never silently ignored.
        var unmet = new List<string>(stubUnmet);
        var next = new List<string>();
        var contract = ContractCommand.Run(target);
        // Call count and a near-1.0 rawcode ratio are NOT enough to identify a hero roster. On a
        // map with no roster at all they matched SetItemRelatives(integer itemId, ...) - an
        // item-relation table - and inserted a bogus call. The signature is the discriminator: a
        // hero roster names its first parameter for a unit (GGGA: RPB_AddHero(integer unitCode,
        // ...)), an item table names it for an item. Refusing to guess is the correct outcome when
        // no candidate looks unit-shaped; a wrong automatic edit is far worse than a manual step.
        var roster = contract.Registries.FirstOrDefault(r =>
            r.CallCount >= 8
            && r.RegisteredRawcodes.Count >= r.CallCount * 0.8
            && FirstParameterLooksLikeAUnit(r.Signature));
        string installedRoot = remap.TryGetValue(def.Id, out var rootCode) ? rootCode : def.Id;

        string? registeredWith = null;
        var assumptions = new List<string>();
        if (roster is null && contract.Templates.Count > 0)
        {
            // Block-style roster. The target registers each hero with several statements rather
            // than one call, so copy a REAL existing entry and substitute. Inventing the shape is
            // what fails here: the block carries a counter, a hidden preview dummy and one
            // UnitAddAbility per spell, and omitting any of those half-registers the hero.
            var tmpl = contract.Templates[0];
            var block = InstantiateTemplate(tmpl, installedRoot, def, remap, assumptions);
            if (block.Count > 0 && AppendAfterLastEntry(target, tmpl, block))
            {
                registeredWith = tmpl.ArrayName + "[" + tmpl.Entries + "] block, "
                    + block.Count + " statement(s) copied from the target's own convention";
                next.Add("registered automatically into " + tmpl.ArrayName
                         + " as entry " + tmpl.Entries);
                // The display name lives in a separate if/elseif chain, not in the roster block,
                // so it needs its own edit. Same principle as the roster itself: copy the branch
                // the map already uses for another hero and substitute, rather than invent a shape.
                if (AddDisplayNameBranch(target, tmpl, def, assumptions) is { } named)
                    next.Add("display name wired: " + named);
                else
                    next.Add("NOT automated: the display name. This map sets it in a separate "
                             + "if/elseif chain; add a branch for this hero by hand.");
            }
            else
                unmet.Add("roster-registration: could not instantiate the " + tmpl.ArrayName
                          + " block; add an entry by hand using the template from 'wc3ctl contract'.");
        }
        else if (roster is null)
            unmet.Add("roster-registration: no hero roster call was detected in the target. If it "
                      + "has one, the hero must be added to it by hand or it will not be selectable.");
        else
        {
            // Automatic registration. The argument shape is learned from how the target already
            // registers every other hero, so this follows the map's own convention instead of a
            // guess at it. Anything inferred is reported, never hidden.
            var call = BuildRegistrationCall(roster, installedRoot, def, assumptions, role);
            if (InsertAfterLastRegistration(target, roster.Function, call))
            {
                registeredWith = call.Trim();
                next.Add($"registered automatically: {registeredWith}");
            }
            else
                unmet.Add($"roster-registration: could not place the call automatically; add "
                          + $"{call.Trim()} inside the target's registration function by hand.");
        }

        // 6. The target's stat convention, applied last so it overrides the definition's own values.
        // A hero carries the stat MODEL of the map she came from, not just her tuning. Shadow
        // Nanaya arrived in GGGA with strength 55, agility 72, intelligence 50 and a 100 point
        // hit-point pool, on a map where all 167 of its heroes pin those three attributes at 0 and
        // state a flat pool instead (its Stalkers cluster on 3600). She was installed correctly and
        // still unplayable. The numbers are measured from the target rather than written down,
        // because they are GGGA's and would be wrong on the next map.
        if (roster is not null && !keepSourceStats
            && HeroStatConvention.Derive(target, roster, role) is { } convention)
        {
            int applied = 0;
            foreach (var f in convention.Fields)
            {
                var set = ObjectSetCommand.Execute(target, ObjectKind.Unit, installedRoot, f.Code, f.Value);
                if (set.Ok)
                {
                    applied++;
                    assumptions.Add("stat convention: " + f.Evidence);
                }
                else
                    // Never drop this quietly. A field the target states and this hero does not
                    // is exactly the difference that makes her unplayable, so a refusal to write
                    // it has to be as visible as a success.
                    unmet.Add($"stat convention: could not set {f.Code}={f.Value} on "
                              + $"'{installedRoot}' ({set.Message}). Set it by hand.");
            }
            if (applied > 0)
                next.Add($"stats brought onto the target's convention, {applied} field(s) measured "
                         + $"from {convention.SampleDescription}. Pass --keep-source-stats to keep "
                         + "the values the hero was exported with.");
        }
        else if (roster is not null && keepSourceStats)
            next.Add("NOT applied: the target's hero stat convention (--keep-source-stats). The "
                     + "hero keeps the source map's attributes and pools, which is only right when "
                     + "both maps model heroes the same way.");

        foreach (var fn in castDispatchWired)
            next.Add($"cast dispatch wired: the port carried '{fn}' and nothing drove it, so the "
                     + "hero's spells were unreachable. A trigger now runs it on spell effect, gated "
                     + $"to '{installedRoot}' so no other unit's cast can enter carried code.");

        if (contract.SpellDispatchers.Count == 0 && castDispatchWired.Count == 0)
            unmet.Add("spell-dispatch: no cast dispatcher detected; the hero's abilities may exist "
                      + "but do nothing.");

        // 7. The per-hero kit ladder, the last thing a ported hero is missing and the only one no
        // check used to mention. Measured in game on GGGA: this hero passed every pick-screen gate
        // and CreateUnit produced her unit, and WS_FinalizeWorkingSourceHero still returned false
        // because it has a hand-written branch per hero and none for her, so she arrived with no
        // caster global, no spell trigger registrations and no kit. Named here rather than
        // generated, because a branch copied from a neighbour would bind THAT hero's triggers to
        // this unit, which is the same trap BindStubs refuses on a name match.
        foreach (var chain in contract.HeroDispatchChains
                     .Where(c => !c.Rawcodes.Contains(installedRoot, StringComparer.Ordinal)))
            unmet.Add($"hero-kit branch: '{chain.Function}' gives each hero its kit in a branch on "
                      + $"the hero's rawcode ({chain.Branches} of them) and has none for "
                      + $"'{installedRoot}'. Add one by hand. It cannot be generated: every branch "
                      + "wires that hero's OWN triggers and caster global, so a copied one would "
                      + "point this unit at another character's kit.");

        // 8. Every OTHER per-hero table. The kit ladder above is one of many, and reporting only it
        // understated the work twelvefold on GGGA. Measured in the running game: the spawn path is
        // not what fails, an installed hero reaches the world alive, visible and selected, and is
        // simply absent from the tables that give her damage routing, death handling, portraits and
        // ability hints. Named, never generated, for the same reason as the kit branch.
        var targetScriptEntry = target.GetFile("war3map.j") ?? target.GetFile("scripts\\war3map.j");
        if (roster is not null && targetScriptEntry is not null)
        {
            // Latin-1, never UTF-8. A war3map.j is a byte stream and real maps carry bytes that are
            // not valid UTF-8.
            var targetScript = System.Text.Encoding.Latin1.GetString(
                targetScriptEntry.CurrentBytes);
            var alreadyNamed = contract.HeroDispatchChains.Select(c => c.Function).ToHashSet(StringComparer.Ordinal);
            foreach (var sys in HeroSystemAudit.Run(targetScript, roster.RegisteredRawcodes, installedRoot)
                         .Where(s => !alreadyNamed.Contains(s.Function)))
                // A ladder that ends in a default already answers for a hero with no branch,
                // so "add one by hand" would be wrong. GGGA's skin-origin table returns 0 for
                // anything it does not list, and a newly installed hero is not a skin of
                // anything, so absence there is the correct state. Say which case this is
                // rather than issuing the same instruction for both.
                unmet.Add($"per-hero table: '{sys.Function}' (war3map.j line {sys.Line}) hand-lists "
                          + $"{sys.HeroCount} of the target's own heroes and has no entry for "
                          + $"'{installedRoot}'."
                          + (sys.HasFallback
                              ? $" It ends in a DEFAULT and lists {sys.CoveragePercent}% of the "
                                + "roster, so absence is very likely correct. Check what the default "
                                + "means for this hero before adding anything."
                              : $" It has no default and lists {sys.CoveragePercent}% of the roster, "
                                + $"so a lookup for '{installedRoot}' yields nothing. Whether that "
                                + "matters depends on the reader, and at this coverage most of the "
                                + "roster is absent too. Read the call site before adding an entry."));
        }

        return new(true,
            $"installed {def.Name} as '{installedRoot}'",
            remap, created, fields, pending.Count, skipped,
            collisions, unmet, next, registeredWith, assumptions, bindings);
    }

    /// <summary>
    /// Replaces a no-op stub with a call to the TARGET's own implementation wherever that is safe,
    /// and reports every stub it had to keep.
    /// </summary>
    /// <remarks>
    /// Export stops the closure at any Wurst class the hero does not belong to, then stubs the calls
    /// so the script still compiles. Those stubs were namespaced like everything else, so the hero
    /// called <c>hH0DA__dispatch_HashMap_get</c>, an empty function returning 0, even on a target
    /// that implements HashMap perfectly well. Namespacing is right for the hero's OWN code (it
    /// cannot then collide with anything) and wrong for a call into shared infrastructure, which
    /// must reach the shared thing. Binding is un-namespacing one symbol so the reference resolves
    /// to the target's declaration, and deleting the stub so it does not shadow it.
    ///
    /// A bind is only performed when all four hold, because a wrong bind is worse than a no-op:
    /// <list type="number">
    /// <item>Not a peer. A target's own <c>AlucardSpells</c> is its Alucard, not a service.</item>
    /// <item>The definition did not carry the class's private instance tables. If it did, the ids
    /// the hero issues index HER arrays and the target's functions index the target's, so the bind
    /// would dispatch on an id the target never issued, with no compile error to catch it.</item>
    /// <item>The target declares the function with a matching parameter and return shape. A
    /// signature mismatch is a compile failure, and a war3map.j that will not compile means a
    /// hosted map with no player slots.</item>
    /// <item>EVERY called function of that class passes, so a class's calls are never split between
    /// the target's implementation and a private stub, which would be two half states of one
    /// object.</item>
    /// </list>
    /// </remarks>
    private static string BindStubs(string body, string targetScript, HeroDefinition def,
        List<string> bindings, List<string> unmet)
    {
        var stubs = def.Stubs;
        if (stubs is null || stubs.Count == 0) return body;

        // The target's own declarations, by name, with the signature to compare against.
        var declared = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var fn in JassFunctionIndex.Parse(targetScript))
            declared[fn.Name] = fn.Signature;

        // Peers are collapsed to one line. There were 21 of them against 7 infrastructure classes on
        // Shadow Nanaya, all with the same verdict for the same reason, and 21 identical lines bury
        // the 7 that a person can act on. The shared-spell-dispatcher requirement already states the
        // consequence, so the count is what is missing from this report, not the list.
        var peerClasses = stubs.Where(s => s.Peer)
            .Select(s => s.WurstClass).Distinct(StringComparer.Ordinal).ToList();
        if (peerClasses.Count > 0)
            bindings.Add($"{peerClasses.Count} peer class(es), {stubs.Count(s => s.Peer)} call(s) "
                         + "left as no-op STUBS (another character's kit, never bound even on a "
                         + "name match, see shared-spell-dispatcher)");

        // Judge a whole class at once, then act.
        var bind = new Dictionary<string, string>(StringComparer.Ordinal);   // carried name -> target name
        foreach (var group in stubs.Where(s => !s.Peer).GroupBy(s => s.WurstClass, StringComparer.Ordinal)
                                   .OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            var members = group.ToList();
            string? refusal = null;
            if (members.Any(s => s.ClassStateCarried))
                refusal = "this definition carries the class's own instance tables, so the target's "
                          + "functions would be handed ids it never issued";
            else
            {
                var absent = members.Where(s => !declared.ContainsKey(s.Function)).ToList();
                if (absent.Count > 0)
                    refusal = $"the target declares {members.Count - absent.Count} of "
                              + $"{members.Count} of its functions";
                else
                {
                    var mismatched = members
                        .Where(s => TypeShape(declared[s.Function]) != TypeShape(s.Signature))
                        .ToList();
                    if (mismatched.Count > 0)
                        refusal = $"{mismatched.Count} of {members.Count} function(s) have a "
                                  + $"different signature in the target (e.g. {mismatched[0].Function})";
                }
            }

            if (refusal is null)
            {
                foreach (var s in members) bind[s.CarriedName] = s.Function;
                bindings.Add($"{group.Key}: {members.Count} call(s) BOUND to the target's own functions");
            }
            else
            {
                bindings.Add($"{group.Key}: {members.Count} call(s) left as no-op STUBS ({refusal})");
                // An infrastructure stub is a hero that half works, which must not pass quietly. A
                // peer stub is expected and already covered by shared-spell-dispatcher, so it is
                // reported above rather than raised as an unmet requirement per character.
                unmet.Add($"wurst-class '{group.Key}': {members.Count} call(s) still resolve to "
                          + $"no-op stubs ({refusal}). Whatever the hero's code does through "
                          + "this class does nothing at runtime.");
            }
        }

        if (bind.Count == 0) return body;

        // Delete each bound stub's body, so its declaration cannot shadow the target's. Indexing
        // the script rather than trusting the emitted layout means a hand-edited definition still
        // works, and a stub whose declaration has moved is simply not found and not renamed.
        var lines = body.Split('\n');
        var drop = new bool[lines.Length];
        var removed = new HashSet<string>(StringComparer.Ordinal);
        foreach (var fn in JassFunctionIndex.Parse(body))
        {
            if (!bind.ContainsKey(fn.Name)) continue;
            for (int i = fn.StartLine - 1; i <= fn.EndLine - 1 && i < lines.Length; i++) drop[i] = true;
            removed.Add(fn.Name);
        }

        // Refuse to rename a reference whose stub was not found. Renaming it anyway would leave two
        // declarations of the same name, one of them the no-op, and the no-op would win or the
        // script would not compile. Never leave a reference without exactly one thing to resolve to.
        foreach (var name in bind.Keys.Where(k => !removed.Contains(k)).ToList())
        {
            bind.Remove(name);
            unmet.Add($"script: '{name}' was recorded as a stub but no such function is in "
                      + "script.j, so its calls were left pointing at the definition's own name. "
                      + "Re-export this hero.");
        }
        if (bind.Count == 0) return body;

        var kept = new StringBuilder();
        for (int i = 0; i < lines.Length; i++)
            if (!drop[i]) kept.Append(lines[i]).Append('\n');

        // Same rewriter export used, run backwards. One implementation of the rule means the two
        // directions cannot drift, and it already handles the escape-sequence guard and the
        // longest-name-first ordering that a naive replace gets wrong.
        return HeroExportCommand.ApplyNamespace(kept.ToString(), bind);
    }

    /// <summary>
    /// A declaration reduced to the only thing that decides whether a call compiles: its parameter
    /// types and return type. Parameter NAMES differ freely between two maps that compiled the same
    /// class, so comparing whole declaration lines would refuse every safe bind.
    /// </summary>
    private static string TypeShape(string declaration)
    {
        var takes = declaration.IndexOf(" takes ", StringComparison.Ordinal);
        var returns = declaration.LastIndexOf(" returns ", StringComparison.Ordinal);
        if (takes < 0 || returns <= takes) return "(unparsed)" + declaration;

        var parameters = declaration[(takes + 7)..returns].Trim();
        var shape = parameters.Equals("nothing", StringComparison.Ordinal)
            ? ""
            : string.Join(",", parameters.Split(',').Select(p =>
                p.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "?"));
        return shape + "->" + declaration[(returns + 9)..].Trim();
    }

    /// <summary>
    /// Builds the registration call by copying the target's OWN convention: same function, same
    /// argument count, our rawcode first. A string argument that looks like an asset path is
    /// filled from the definition's icon; every other argument reuses the value an existing
    /// registration used, so the call is always well-formed even on a map we have never seen.
    /// </summary>
    /// <summary>A short quoted word that is not a file path, which is what a role looks like.</summary>
    private static bool IsRoleArgument(string literal) =>
        literal.StartsWith("\"", StringComparison.Ordinal) && !LooksLikeAssetArgument(literal)
        && literal.Trim('"').Length is > 0 and <= 24 && !literal.Contains('.');

    private static string BuildRegistrationCall(RosterRegistry roster, string rawcode,
        HeroDefinition def, List<string> assumptions, string? role)
    {
        var args = SplitArguments(roster.ExampleCall);
        if (args.Count == 0) return $"    call {roster.Function}('{rawcode}')";

        var built = new List<string> { $"'{rawcode}'" };
        // Prefer the hero's OWN interface icon, the uico field, over whatever happens to be first
        // in the asset list. Picking the first icon-shaped asset handed the roster an ABILITY icon
        // (BTNHeroDarkiShiki_E, her E spell) as the hero's portrait, which is wrong and visible.
        var root = def.Objects.FirstOrDefault(o => o.Origin == "root");
        string? icon = root?.Fields
                .FirstOrDefault(f => f.Code is "uico" or "ussi")?.Value
            ?? def.Assets.FirstOrDefault(a =>
                a.Category.Equals("icon", StringComparison.OrdinalIgnoreCase))?.Path;
        // The field omits the extension; match it to a carried asset so the emitted path is real.
        if (icon is not null && !icon.Contains('.'))
            icon = def.Assets.FirstOrDefault(a =>
                a.Path.StartsWith(icon, StringComparison.OrdinalIgnoreCase))?.Path ?? icon;

        for (int i = 1; i < args.Count; i++)
        {
            var sample = args[i].Trim();
            // A caller-supplied role wins over copying a neighbour's. Without it the hero inherits
            // whatever the sampled registration said, so Shadow Nanaya registered as "Bruiser" and
            // appeared under the wrong tab in the picker. Nothing in the source map states a role,
            // so this cannot be derived and has to be told to us.
            if (role is not null && IsRoleArgument(sample))
            {
                built.Add("\"" + role + "\"");
                assumptions.Add($"argument {i + 1} (role) set from --role to \"{role}\"");
                continue;
            }
            if (icon is not null && LooksLikeAssetArgument(sample))
            {
                built.Add("\"" + icon.Replace("\\", "\\\\") + "\"");
                assumptions.Add($"argument {i + 1} (portrait) set to the definition's icon: {icon}");
                icon = null;
            }
            else
            {
                built.Add(sample);
                assumptions.Add($"argument {i + 1} copied from an existing registration: {sample}");
            }
        }
        return $"    call {roster.Function}({string.Join(" , ", built)})";
    }

    private static bool LooksLikeAssetArgument(string literal) =>
        literal.StartsWith("\"", StringComparison.Ordinal)
        && (literal.Contains(".blp", StringComparison.OrdinalIgnoreCase)
            || literal.Contains(".tga", StringComparison.OrdinalIgnoreCase)
            || literal.Contains(".dds", StringComparison.OrdinalIgnoreCase));

    /// <summary>Top-level arguments of one call, so nested parens and commas in strings survive.</summary>
    private static List<string> SplitArguments(string callLine)
    {
        int open = callLine.IndexOf('(');
        int close = callLine.LastIndexOf(')');
        if (open < 0 || close <= open) return new List<string>();
        var inner = callLine[(open + 1)..close];
        var parts = new List<string>();
        int depth = 0, start = 0; bool inString = false;
        for (int i = 0; i < inner.Length; i++)
        {
            char c = inner[i];
            if (c == '\"') inString = !inString;
            else if (!inString && c == '(') depth++;
            else if (!inString && c == ')') depth--;
            else if (!inString && c == ',' && depth == 0)
            { parts.Add(inner[start..i]); start = i + 1; }
        }
        parts.Add(inner[start..]);
        return parts;
    }

    /// <summary>
    /// Places the call immediately after the LAST existing registration, so it lands inside the
    /// same function and after whatever setup those calls depend on.
    /// </summary>
    private static bool InsertAfterLastRegistration(MapDocument target, string function, string call)
    {
        var entry = target.GetFile("war3map.j");
        if (entry is null) return false;
        var text = ScriptBytes.GetString(entry.CurrentBytes);
        var lines = text.Split('\n').ToList();
        int last = -1;
        for (int i = 0; i < lines.Count; i++)
            if (lines[i].Contains("call " + function + "(", StringComparison.Ordinal)) last = i;
        if (last < 0) return false;
        lines.Insert(last + 1, call);
        FileEditCommand.AddOrReplace(target, entry.FileName!, ScriptBytes.GetBytes(string.Join("\n", lines)));
        return true;
    }

    /// <summary>
    /// Appends paths to <c>war3map.imp</c>, bumping its declared count. The table is a version and
    /// a count followed by (flag byte, NUL-terminated path) records; the flag is copied from the
    /// map's own first entry rather than guessed.
    /// </summary>
    private static void AddImportEntries(MapDocument target, IEnumerable<string> paths)
    {
        var entry = target.GetFile("war3map.imp");
        var existing = entry is null ? Array.Empty<byte>() : (entry.CurrentBytes);
        byte flag = existing.Length > 8 ? existing[8] : (byte)0x0D;

        uint version = 1, count = 0;
        var body = new List<byte>();
        if (existing.Length >= 8)
        {
            version = BitConverter.ToUInt32(existing, 0);
            count = BitConverter.ToUInt32(existing, 4);
            body.AddRange(existing.Skip(8));
        }

        foreach (var p in paths)
        {
            body.Add(flag);
            body.AddRange(Encoding.UTF8.GetBytes(p));
            body.Add(0);
            count++;
        }

        var rebuilt = new List<byte>();
        rebuilt.AddRange(BitConverter.GetBytes(version));
        rebuilt.AddRange(BitConverter.GetBytes(count));
        rebuilt.AddRange(body);
        FileEditCommand.AddOrReplace(target, "war3map.imp", rebuilt.ToArray());
    }

    /// <summary>
    /// Splices the definition's globals in just before the target's <c>endglobals</c>, rewriting
    /// rawcode literals through the install remap so a carried declaration cannot reference a code
    /// that was renamed. A name the target already declares is skipped rather than duplicated.
    /// </summary>
    private static string InsertGlobals(string script, IReadOnlyList<string> globals,
        IReadOnlyDictionary<string, string> remap)
    {
        if (globals.Count == 0) return script;
        var lines = script.Split('\n').ToList();
        int end = lines.FindIndex(l => l.Trim() == "endglobals");
        if (end < 0) return script;

        var already = new HashSet<string>(StringComparer.Ordinal);
        var name = new System.Text.RegularExpressions.Regex(
            @"^\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_]*\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)");
        for (int i = 0; i < end; i++)
        {
            var m = name.Match(lines[i]);
            if (m.Success) already.Add(m.Groups[1].Value);
        }

        var add = new List<string>();
        foreach (var g in globals)
        {
            var m = name.Match(g);
            if (m.Success && already.Contains(m.Groups[1].Value)) continue;
            var line = g;
            foreach (var (from, to) in remap)
                line = line.Replace($"'{from}'", $"'{to}'", StringComparison.Ordinal);
            add.Add(line);
        }
        if (add.Count == 0) return script;

        lines.InsertRange(end, add);
        return string.Join("\n", lines);
    }

    /// <summary>
    /// True when the registration's FIRST parameter names a unit rather than an item, ability or
    /// anything else. Deliberately conservative: an unrecognised shape returns false, so install
    /// reports the requirement as unmet instead of writing a call it cannot justify.
    /// </summary>
    private static bool FirstParameterLooksLikeAUnit(string signature)
    {
        var first = signature.Split(',')[0];
        var name = first.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        if (name is null) return false;
        if (name.Contains("item", StringComparison.OrdinalIgnoreCase)) return false;
        return name.Contains("unit", StringComparison.OrdinalIgnoreCase)
            || name.Contains("hero", StringComparison.OrdinalIgnoreCase)
            || name.Contains("char", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Copies a real roster entry, substituting this hero's rawcode for the sample's and her
    /// abilities for the sample's ability ids. Everything else is preserved verbatim so the new
    /// entry matches the map's own convention exactly, including the preview dummy and the counter.
    /// </summary>
    private static List<string> InstantiateTemplate(RosterTemplate tmpl, string rawcode,
        HeroDefinition def, IReadOnlyDictionary<string, string> remap, List<string> assumptions)
    {
        // What "set Arr[n]=X" assigns in the sample, i.e. the sample hero's id.
        var slot = new Regex(@"^\s*set\s+" + Regex.Escape(tmpl.ArrayName)
                             + @"\s*\[[^\]]*\]\s*=\s*([A-Za-z_][A-Za-z0-9_]*)");
        string? sampleHeroId = null;
        foreach (var l in tmpl.TemplateLines)
        {
            var m = slot.Match(l);
            if (m.Success) { sampleHeroId = m.Groups[1].Value; break; }
        }
        if (sampleHeroId is null) return new List<string>();

        var addAbility = new Regex(@"UnitAddAbility\s*\([^,]*,\s*([A-Za-z_][A-Za-z0-9_]*)\s*\)");
        int sampleAbilityCount = tmpl.TemplateLines.Count(l => addAbility.IsMatch(l));

        // Ours: the abilities the definition carries, in declaration order, after install's remap.
        var ours = def.Objects
            .Where(o => o.Kind.Equals("ability", StringComparison.OrdinalIgnoreCase))
            .Select(o => remap.TryGetValue(o.Rawcode, out var to) ? to : o.Rawcode)
            .ToList();

        var outLines = new List<string>();
        int abilityIndex = 0;
        foreach (var raw in tmpl.TemplateLines)
        {
            var line = raw;

            // This hero goes after every existing entry.
            line = Regex.Replace(line, @"^(\s*set\s+n\s*=\s*)-?\d+", "${1}" + tmpl.Entries);
            line = line.Replace(sampleHeroId, "'" + rawcode + "'", StringComparison.Ordinal);
            // The sample carried a trailing "// Tomioka"; keeping it mislabels this hero.
            int cmt = line.IndexOf("//", StringComparison.Ordinal);
            if (cmt > 0) line = line[..cmt].TrimEnd();

            var am = addAbility.Match(line);
            if (am.Success)
            {
                // Fewer abilities than the sample: drop the surplus line rather than emit a
                // reference to an ability this hero does not have.
                if (abilityIndex >= ours.Count) continue;
                line = line.Replace(am.Groups[1].Value, "'" + ours[abilityIndex] + "'",
                    StringComparison.Ordinal);
                abilityIndex++;
            }
            outLines.Add(line);
        }

        assumptions.Add("roster block copied from " + tmpl.ArrayName + " entry at line " + tmpl.StartLine);
        assumptions.Add(abilityIndex + " of the sample's " + sampleAbilityCount
                        + " ability slot(s) filled from " + ours.Count
                        + " carried ability object(s), in declaration order");
        return outLines;
    }

    /// <summary>
    /// Appends after the LAST existing roster entry, not after the sampled one. Inserting at the
    /// sample's end put the new entry BEFORE the rest of the roster and produced a duplicate
    /// 'set n=' line, because the template is one entry out of many.
    /// </summary>
    private static bool AppendAfterLastEntry(MapDocument target, RosterTemplate tmpl, List<string> block)
    {
        var entry = target.GetFile("war3map.j");
        if (entry is null) return false;
        var text = ScriptBytes.GetString(entry.CurrentBytes);
        var lines = text.Split('\n').ToList();
        var slot = new Regex(@"^\s*set\s+" + Regex.Escape(tmpl.ArrayName) + @"\s*\[");
        int last = -1;
        for (int i = 0; i < lines.Count; i++) if (slot.IsMatch(lines[i])) last = i;
        if (last < 0) return false;
        // Past the trailing statements of that entry (its dummy, ShowUnit, ability adds).
        int after = last + 1;
        while (after < lines.Count)
        {
            var t = lines[after].TrimStart();
            if (t.StartsWith("call UnitAddAbility", StringComparison.Ordinal)
                || t.StartsWith("call ShowUnit", StringComparison.Ordinal)
                || t.StartsWith("set " + tmpl.ArrayName + "_Dummy", StringComparison.Ordinal)) after++;
            else break;
        }
        lines.InsertRange(after, block);
        FileEditCommand.AddOrReplace(target, entry.FileName!, ScriptBytes.GetBytes(string.Join("\n", lines)));
        return true;
    }

    /// <summary>Inserts lines directly after a 1-based line number in war3map.j.</summary>
    private static bool AppendAfterLine(MapDocument target, int oneBasedLine, List<string> block)
    {
        var entry = target.GetFile("war3map.j");
        if (entry is null) return false;
        var text = ScriptBytes.GetString(entry.CurrentBytes);
        var lines = text.Split('\n').ToList();
        if (oneBasedLine < 1 || oneBasedLine > lines.Count) return false;
        lines.InsertRange(oneBasedLine, block);
        FileEditCommand.AddOrReplace(target, entry.FileName!, ScriptBytes.GetBytes(string.Join("\n", lines)));
        return true;
    }

    /// <summary>
    /// Byte-preserving codec for script text. A map's war3map.j is NOT necessarily UTF-8: WOS2
    /// carries bytes that are not valid UTF-8, and decoding then re-encoding them as UTF-8 replaces
    /// each with '?', which pjass reports as "Unrecognized character ? (ASCII 63)" and which
    /// silently corrupts author names and localised strings. Latin-1 maps every byte 0..255 to the
    /// same code point and back, so a read/modify/write round-trip is lossless whatever the real
    /// encoding was.
    /// </summary>
    private static readonly Encoding ScriptBytes = Encoding.Latin1;

    /// <summary>
    /// Adds this hero to the target's display-name chain, the separate if/elseif ladder that maps a
    /// roster slot to the text shown in the pick UI.
    /// </summary>
    /// <remarks>
    /// A roster entry alone is not enough on a map built this way: the hero occupies a slot but the
    /// picker shows nothing for her. The chain is found by looking for a string array assigned
    /// literal names many times over, then the LAST branch of it is copied and substituted, so the
    /// new branch matches whatever comparison variable and formatting the map already uses. Copying
    /// a real branch rather than composing one is the same rule the roster block follows, and it is
    /// why this works on a map the tool has never seen.
    /// Returns the emitted assignment, or null when no such chain exists (plenty of maps have none).
    /// </remarks>
    private static string? AddDisplayNameBranch(MapDocument target, RosterTemplate tmpl,
        HeroDefinition def, List<string> assumptions)
    {
        var entry = target.GetFile("war3map.j");
        if (entry is null) return null;
        var lines = ScriptBytes.GetString(entry.CurrentBytes).Split('\n').ToList();

        // A string array assigned literal names repeatedly is the display chain.
        var assign = new Regex(@"^(\s*)set\s+([A-Za-z_][A-Za-z0-9_]*)\s*\[([^\]]*)\]\s*=\s*""([^""]{2,60})""\s*$");
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var l in lines)
            if (assign.Match(l) is { Success: true } m)
                counts[m.Groups[2].Value] = counts.GetValueOrDefault(m.Groups[2].Value) + 1;
        var chain = counts.Where(kv => kv.Value >= 8)
            .OrderByDescending(kv => kv.Value).Select(kv => kv.Key).FirstOrDefault();
        if (chain is null) return null;

        int last = -1;
        for (int i = 0; i < lines.Count; i++)
        {
            var m = assign.Match(lines[i]);
            if (m.Success && m.Groups[2].Value == chain) last = i;
        }
        if (last < 0) return null;

        // The condition guarding that last branch, so the new one is worded the same way.
        int cond = -1;
        for (int i = last; i >= 0 && i > last - 6; i--)
            if (Regex.IsMatch(lines[i], @"^\s*(else)?if\b.*\bthen\s*$")) { cond = i; break; }
        if (cond < 0) return null;

        var sample = assign.Match(lines[last]);
        var indent = sample.Groups[1].Value;
        var slotExpr = sample.Groups[3].Value;              // e.g. "pid", copied verbatim
        var name = def.Name ?? def.Id;

        // Reword the sampled condition to test THIS hero's roster slot. The comparison variable is
        // whatever the map already compares against, so only the array and index change.
        var condLine = Regex.Replace(lines[cond].TrimEnd('\r'),
            @"([A-Za-z_][A-Za-z0-9_]*)\s*\[[^\]]*\]",
            $"{tmpl.ArrayName}[{tmpl.Entries}]");
        if (!condLine.TrimStart().StartsWith("elseif", StringComparison.Ordinal))
            condLine = Regex.Replace(condLine, @"^(\s*)if\b", "${1}elseif");

        var setLine = $"{indent}set {chain}[{slotExpr}]=\"{name}\"";

        // Insert after the WHOLE sampled branch, not straight after its assignment. A branch can
        // carry more than the one line, WOS2 follows each name with a MakeSoundLocal for that
        // hero's pick voice line, and splicing in between stole the previous hero's sound into
        // this hero's branch and left that hero with none.
        int insertAt = last + 1;
        while (insertAt < lines.Count)
        {
            var t = lines[insertAt].TrimStart();
            if (t.StartsWith("elseif", StringComparison.Ordinal)
                || t.StartsWith("else", StringComparison.Ordinal)
                || t.StartsWith("endif", StringComparison.Ordinal)) break;
            insertAt++;
        }
        lines.InsertRange(insertAt, new[] { condLine, setLine });
        FileEditCommand.AddOrReplace(target, entry.FileName!,
            ScriptBytes.GetBytes(string.Join("\n", lines)));

        assumptions.Add($"display-name branch copied from the last '{chain}' branch at line {cond + 1}");
        return setLine.Trim();
    }

    /// <summary>
    /// Rewrites any carried rawcode appearing in a field value through the install remap.
    /// </summary>
    /// <remarks>
    /// Object fields reference other objects by rawcode, and a comma separated list is the common
    /// shape (a hero's uhab and uabi list her abilities that way). Install has to rename objects
    /// because the target's codes are its own, so a value copied verbatim points at codes that do
    /// not exist there. The symptom is silent: the hero installs cleanly, the script compiles, and
    /// she simply has no abilities in game.
    /// Only whole four character tokens are replaced, so a path or a piece of prose that happens to
    /// contain four matching characters is left alone.
    /// </remarks>
    private static string RemapRawcodes(string value, IReadOnlyDictionary<string, string> remap)
    {
        if (remap.Count == 0 || string.IsNullOrEmpty(value)) return value;
        if (value.Contains('\\') || value.Contains('/')) return value;   // an asset path, never a code list

        var parts = value.Split(',');
        bool changed = false;
        for (int i = 0; i < parts.Length; i++)
        {
            var token = parts[i].Trim();
            if (token.Length == 4 && remap.TryGetValue(token, out var to))
            {
                parts[i] = parts[i].Replace(token, to, StringComparison.Ordinal);
                changed = true;
            }
        }
        return changed ? string.Join(",", parts) : value;
    }

    private static InstallResult Fail(string message) =>
        new(false, message, new Dictionary<string, string>(), 0, 0, 0, 0,
            Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), null,
            Array.Empty<string>(), Array.Empty<string>());
}
