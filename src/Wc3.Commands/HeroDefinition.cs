// src/Wc3.Commands/HeroDefinition.cs
namespace Wc3.Commands;

/// <summary>
/// One field the source map set on an object. Only map-level fields are carried: a base-game
/// value is not the hero's data, it is the engine's, and re-stating it would make the definition
/// brittle across patches.
/// </summary>
public sealed record DefinitionField(string Code, string Value);

/// <summary>
/// One object the hero needs, with its fields STATED rather than inferred. Carrying the fields is
/// what makes a definition self-contained: without them an install would still need the original
/// map, and the whole point of the format is that it does not.
/// </summary>
public sealed record DefinitionObject(
    string Rawcode,
    string Kind,
    string? Name,
    string? BaseRawcode,
    bool CustomToMap,
    string Origin,
    IReadOnlyList<DefinitionField> Fields);

/// <summary>
/// One asset. <see cref="Sha256"/> makes an install idempotent and lets a collision be judged:
/// same hash means the target already has this exact file and nothing needs writing.
/// </summary>
public sealed record DefinitionAsset(
    string Path,
    string Category,
    long SizeBytes,
    string Sha256);

/// <summary>
/// Something the TARGET map must provide. This is the part the World Editor has no concept of and
/// the reason a hero with perfect object data can still not exist to the player: a map builds its
/// roster from its own script, so a unit absent from that call is simply not a hero there.
/// </summary>
public sealed record DefinitionRequirement(
    string Kind,
    string Detail,
    bool Satisfiable);

/// <summary>
/// One function the export did NOT carry but the carried code still CALLS, together with the
/// no-op stub that was emitted in its place so the script would compile.
///
/// A stub is a placeholder, not an implementation, so recording it is what lets an install replace
/// it. Without this list the stubs were indistinguishable from carried code: they were namespaced
/// like everything else, so a call to the source map's HashMap became a call to a private empty
/// function that returns 0, and a target that already implements HashMap could never be reached.
/// Install reads these and BINDS the call to the target's own function where that is safe.
/// </summary>
/// <param name="Function">
/// The name the SOURCE map declares, un-namespaced. This is what a target must also declare for
/// the call to be bindable, since Wurst's output is named the same way in every map it compiles.
/// </param>
/// <param name="CarriedName">
/// The namespaced name the carried script calls today. Install needs it to find the stub body to
/// delete and the references to rewrite.
/// </param>
/// <param name="WurstClass">The class the closure stopped at, i.e. what to report per class.</param>
/// <param name="Signature">
/// The source's whole declaration line. Binding to a target function whose parameter types differ
/// would not compile, so the shapes are compared rather than assumed to match.
/// </param>
/// <param name="Peer">
/// True when this belongs to ANOTHER character's kit rather than to infrastructure. A peer is never
/// bound even if the target happens to declare the same name, because a target's AlucardSpells is
/// its own Alucard, not a service this hero may call into.
/// </param>
/// <param name="ClassStateCarried">
/// True when the definition also carries a PRIVATE copy of this class's instance tables (its
/// allocator counters and typeId array). Then the hero allocates instance ids in its own tables
/// while the target's functions index the target's tables, so binding would hand the target an id
/// it never issued. That is silent cross-table corruption, which is strictly worse than a no-op,
/// so such a class stays stubbed and is reported instead.
/// </param>
public sealed record DefinitionStub(
    string Function,
    string CarriedName,
    string WurstClass,
    string Signature,
    bool Peer,
    bool ClassStateCarried);

/// <summary>
/// A hero as a self-describing artifact, rather than something inferred out of a host map every
/// time it is needed.
///
/// The reason this exists: extracting a hero from a merged 200,000-line arena map is intractable,
/// and every failure in that direction came from INFERENCE - guessing which objects belong to it,
/// which functions are reachable, which calls can be trimmed. A definition replaces inference with
/// declaration. Stated once, reviewed by a human, then installed mechanically.
///
/// It also lets the importer be imperfect, which it never could be before. "Got 80% and flagged
/// the rest" is a good outcome for something a person reviews, and a broken map when it goes
/// straight into a game.
/// </summary>
public sealed record HeroDefinition(
    int SchemaVersion,
    string Id,
    string? Name,
    string SourceMap,
    string ExportedFrom,
    IReadOnlyList<DefinitionObject> Objects,
    IReadOnlyList<DefinitionAsset> Assets,
    IReadOnlyList<string> Strings,
    string? ScriptFile,
    IReadOnlyList<string> ScriptEntryPoints,
    /// <summary>
    /// Global declarations the carried functions read, verbatim. Without these the script
    /// does not compile: carrying functions alone produced 50 'undeclared variable' errors,
    /// which is the same fatal outcome as a missing callee, and an un-compilable war3map.j
    /// means a hosted map shows no player slots.
    /// </summary>
    IReadOnlyList<string> Globals,
    IReadOnlyList<DefinitionRequirement> Requires,
    IReadOnlyList<string> ReviewNotes,
    /// <summary>
    /// The no-op stubs the export emitted for called-but-not-carried functions, so an install can
    /// replace one with the target's own implementation instead of shipping the placeholder. Null
    /// or empty means "not recorded", which is how a definition exported by an older build reads,
    /// and it degrades to the old behaviour (every stub kept) rather than to a broken script.
    /// </summary>
    IReadOnlyList<DefinitionStub>? Stubs = null)
{
    /// <remarks>
    /// Deliberately NOT bumped for <see cref="Stubs"/>. The version exists so an older build can
    /// REFUSE a definition it would mis-handle, and this field is additive and optional: System.Text
    /// .Json ignores an unknown property, so an older build installs such a definition exactly as
    /// it does today, keeping every stub. Bumping would make older builds refuse a definition they
    /// can still install correctly, which is a regression dressed up as caution. Bump this only for
    /// a change that would be MIS-read, for example a renamed or re-typed existing field.
    /// </remarks>
    public const int CurrentSchemaVersion = 1;
}
