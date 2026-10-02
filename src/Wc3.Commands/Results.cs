using Wc3.Model;

namespace Wc3.Commands;

public sealed record FileEntryInfo(string? Name, int SizeBytes, bool Known, bool Parsed);
public sealed record FileListResult(IReadOnlyList<FileEntryInfo> Files);
public sealed record MapInfoResult(
    string Name, string Author, int Players, int? Width, int? Height, IReadOnlyList<string> Diagnostics);
public sealed record SearchHit(string FileName, string Context);
public sealed record SearchResult(IReadOnlyList<SearchHit> Hits);
public sealed record DiffEntry(string Name, string Change);
public sealed record DiffResult(IReadOnlyList<DiffEntry> Entries);
public sealed record RoundtripResult(
    bool Faithful, IReadOnlyList<string> Mismatches, IReadOnlyList<string> ExcludedNotes);
public sealed record MergedField(string Code, string Name, string Value, string Source) // Source: "base" | "map"
{
    private readonly string? _display;
    /// <summary><see cref="Value"/> with TRIGSTR_ references resolved against war3map.wts,
    /// for DISPLAY only. The raw <see cref="Value"/> is what editing / write-back use, so the
    /// string-table reference is never clobbered. Defaults to Value when not set.</summary>
    public string Display { get => _display ?? Value; init => _display = value; }
}
public sealed record MergedObjectResult(
    string Rawcode, bool Found, string? BaseRawcode, string? Name,
    IReadOnlyList<MergedField> Fields, IReadOnlyList<string> Diagnostics);
public sealed record ObjectListItem(string Rawcode, string? BaseRawcode, string? Name);
public sealed record ObjectListResult(IReadOnlyList<ObjectListItem> Items);
// Doodad "palette": the placeable doodad catalog = base-game set (GameData) unioned with
// the map's own object-data (war3map.w3d ⊕ war3mapSkin.w3d). Source: "base" (stock, from
// the install), "map-custom" (a New* doodad defined by the map), or "map-modified" (a
// base doodad the map edits in place). BaseRawcode = the base a map entry derives from
// (null for pure base-catalog rows). Name is null when unresolvable (no install / WESTRING).
// IconPath = the entry's icon art (units: 'uico' delta, else the base skin profile's Art);
// null when the kind has no icon (doodads) or nothing resolves. Decode via PaletteCommand.IconPng.
public sealed record PaletteEntry(
    string Rawcode, string? Name, string Source, string? BaseRawcode, string? IconPath = null);
public sealed record DoodadPaletteResult(
    bool Ok, string Message, IReadOnlyList<PaletteEntry> Entries, IReadOnlyList<string> Diagnostics);
// Same shape as DoodadPaletteResult, over the unit catalog (ObjectKind.Unit) instead.
public sealed record UnitPaletteResult(
    bool Ok, string Message, IReadOnlyList<PaletteEntry> Entries, IReadOnlyList<string> Diagnostics);
// Same shape over the item catalog (ObjectKind.Item). Items place into war3mapUnits.doo
// via PlacementCommand.PlaceItem.
public sealed record ItemPaletteResult(
    bool Ok, string Message, IReadOnlyList<PaletteEntry> Entries, IReadOnlyList<string> Diagnostics);
// Same shape over the destructable catalog (ObjectKind.Destructable). Destructables place
// through PlacementCommand.PlaceDoodad because war3map.doo holds doodads and destructables
// alike (the TypeId decides which catalog resolves it).
public sealed record DestructablePaletteResult(
    bool Ok, string Message, IReadOnlyList<PaletteEntry> Entries, IReadOnlyList<string> Diagnostics);
public sealed record ExtractedItem(string? Name, int BlockIndex, byte[] Bytes);
public sealed record ExtractResult(IReadOnlyList<ExtractedItem> Items);

// A rawcode that had to be reassigned because it collided with the target map.
public sealed record RawcodeRemap(ObjectKind Kind, string From, string To);
public sealed record PortedObject(ObjectKind Kind, string Rawcode, string? Name, bool ModifiesStandard);
// Outcome of the best-effort JASS script closure append (null when no script was ported).
public sealed record ScriptPortInfo(
    int Functions, int Globals, int Renamed, bool InitHooked, IReadOnlyList<string> Notes);
public sealed record PortResult(
    string RootRawcode,
    string? RootPortedTo,          // the root's rawcode in the target (== RootRawcode if no collision)
    string? RootName,
    IReadOnlyList<RawcodeRemap> Remaps,
    IReadOnlyList<PortedObject> Objects,
    IReadOnlyList<string> CopiedFiles,
    IReadOnlyList<string> SkippedFiles,
    int InlinedStrings,
    IReadOnlyList<string> Warnings,
    IReadOnlyList<string> Diagnostics,
    ScriptPortInfo? Script = null);
// Combined outcome of porting several units into one target: per-unit results in input
// order (their Script is always null — the batch splices the merged closure once), the
// single merged script summary, and batch-level warnings.
public sealed record BatchPortResult(
    IReadOnlyList<PortResult> Units,
    ScriptPortInfo? Script,
    IReadOnlyList<string> Warnings);
public sealed record BundleNode(string Rawcode, ObjectKind Kind, string? Name, bool CustomToMap);
public sealed record BundleFile(string Path, string Category, bool PresentInMap); // Category: model|texture|icon|sound|other
public sealed record BundleEdge(string From, string To, string Via); // From/To = rawcode or file path; Via = field code
// Reason: "references '<rawcode>'[, ...]" (call-graph seed) or "called by <FunctionName>" (first discoverer).
public sealed record BundleFunction(string Name, int StartLine, int EndLine, string Reason);
// Despite the name, this is a generic object bundle: the root may be ANY ObjectKind
// (BundleCommand.ResolveObject). "Unit" is kept for API/JSON compatibility this pass.
public sealed record UnitBundle(string RootRawcode, string? RootName,
    IReadOnlyList<BundleNode> Objects, IReadOnlyList<BundleFile> Files,
    IReadOnlyList<string> Strings, IReadOnlyList<BundleEdge> Edges, IReadOnlyList<string> Diagnostics,
    IReadOnlyList<BundleFunction> Functions);

// Output of the `validate` command. Severity reuses the loader's DiagnosticSeverity
// so JSON renders "Error"/"Warning"/"Info". Category is a short machine tag
// (loader | missing-file | empty-file | map-info). Valid == no Error-severity issues.
public sealed record ValidationIssue(DiagnosticSeverity Severity, string Category, string FileName, string Message);
public sealed record ValidateResult(
    bool Valid, int Errors, int Warnings, IReadOnlyList<ValidationIssue> Issues);
