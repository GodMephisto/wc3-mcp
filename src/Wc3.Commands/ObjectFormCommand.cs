// src/Wc3.Commands/ObjectFormCommand.cs
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One editable field, with everything a front end needs to render and validate it.</summary>
public sealed record FormField(
    string Code,
    string Name,
    string Value,
    string Display,
    string Source,
    string Type,
    string? MinValue,
    string? MaxValue,
    bool ForceNonNegative,
    bool CanBeEmpty,
    bool MultiLine,
    ObjectLayer Layer,
    bool LayerIsAuthoritative)
{
    /// <summary>Whether a candidate value satisfies the metadata's own constraints. The message
    /// is null when it passes.</summary>
    public string? Validate(string candidate)
    {
        if (string.IsNullOrEmpty(candidate))
            return CanBeEmpty ? null : $"{Name} cannot be empty";

        if (!double.TryParse(candidate, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var n))
            return null;   // not numeric, so the numeric bounds do not apply

        if (ForceNonNegative && n < 0)
            return $"{Name} cannot be negative";
        if (MinValue is not null && double.TryParse(MinValue,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var min) && n < min)
            return $"{Name} is below the minimum of {MinValue}";
        if (MaxValue is not null && double.TryParse(MaxValue,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var max) && n > max)
            return $"{Name} is above the maximum of {MaxValue}";
        return null;
    }
}

/// <summary>A titled group of fields, the unit the World Editor shows as one collapsible section.</summary>
public sealed record FormGroup(string Key, string Title, IReadOnlyList<FormField> Fields);

/// <summary>An object rendered as a form rather than as a flat field dump.</summary>
public sealed record ObjectForm(
    string Rawcode,
    string? Name,
    string? BaseRawcode,
    ObjectKind Kind,
    IReadOnlyList<FormGroup> Groups,
    int FieldCount,
    int HiddenFieldCount,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Turns an object's merged fields into the grouped, ordered, validated form the World Editor
/// shows, using the metadata the game itself ships.
///
/// Without this a client can only render one flat list. A single unit resolves to 169 fields on a
/// real map and the kind defines 273, in no meaningful order, with no indication of which are
/// art and which are combat, which accept a blank, what the legal range is, or which of them even
/// apply to the object in hand. That is a data dump, not an editor, and it is unusable for the
/// job people actually open an object editor to do.
///
/// Every input here is a column of `unitmetadata.slk` and its siblings, read out of the retail
/// install. Nothing is invented and nothing is hardcoded per field, so the form tracks whatever
/// the installed patch says rather than a table that goes stale.
///
/// This lives in the command layer rather than in a panel because the house has three front ends
/// and a layout that lives in the GUI cannot be reached from the CLI or from MCP.
/// </summary>
public static class ObjectFormCommand
{
    // The category codes the metadata uses, in READING order.
    //
    // The order is ours and the names are not. The game ships the names in WorldEditData.txt's
    // ObjectEditorCategories section, localized, and those are authoritative, so they are read
    // from there and these strings are only the fallback for a machine with no install. The file
    // lists the categories alphabetically by key, which is not a reading order, so the sequence
    // below stays hardcoded on purpose: Art first because it is what someone looks at first, and
    // Editor last because it is metadata about the object rather than the object.
    private static readonly (string Key, string Title)[] CategoryOrder =
    {
        ("art", "Art"),
        ("combat", "Combat"),
        ("data", "Data"),
        ("abil", "Abilities"),
        ("move", "Movement"),
        ("path", "Pathing"),
        ("sound", "Sound"),
        ("stats", "Stats"),
        ("tech", "Techtree"),
        ("text", "Text"),
        ("editor", "Editor"),
    };

    /// <summary>
    /// The display name for a category code, preferring the game's own localized name over the
    /// English fallback above. A patch that renames a category, or an install in another language,
    /// is then followed rather than contradicted.
    /// </summary>
    private static string CategoryTitle(GameDataContext? ctx, string key, string fallback)
    {
        if (ctx is not null
            && ctx.EditorCatalogs.TryGet("ObjectEditorCategories", out var entries))
        {
            var hit = entries.FirstOrDefault(e =>
                string.Equals(e.Key, key, StringComparison.OrdinalIgnoreCase));
            if (hit is not null && hit.Label.Length > 0) return hit.Label;
        }
        return fallback;
    }

    public static ObjectForm Execute(
        MapDocument doc, ObjectKind kind, string rawcode, string? gameDirOverride)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var merged = ObjectGetCommand.Execute(doc, kind, rawcode, gameDirOverride);
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _);
        return Build(doc, kind, rawcode, merged, ctx);
    }

    internal static ObjectForm Build(
        MapDocument doc, ObjectKind kind, string rawcode,
        MergedObjectResult merged, GameDataContext? ctx)
    {
        var diagnostics = new List<string>(merged.Diagnostics);
        var meta = ObjectKinds.FieldMeta(ctx, kind);
        if (meta is null)
            diagnostics.Add("no game data, so fields cannot be grouped, ordered or bounds-checked. "
                          + "Point --game-dir at a Warcraft III install for the full form.");

        // The measured split is the fallback for which layer a field lands in, and the only answer
        // available with no install present. See SkinFieldPartition.
        //
        // LAZY on purpose. Learning it counts every field of every object of the kind across both
        // layers, which on a real map is 2247 units times about 170 fields times two, and with a
        // game install present the authoritative netsafe column answers every field so the result
        // is never read. Computing it eagerly cost 50ms on every object selection to produce a
        // value nobody used, which is most of why this pane was four times slower than the field
        // dump it replaced.
        var partition = new Lazy<SkinFieldPartition>(() => SkinFieldPartition.Learn(doc, kind));
        var byCode = ObjectKinds.ModsToDict(
            merged.Fields.Select(f => KeyValuePair.Create(f.Code, f.Value)).ToList());
        var (isHero, isBuilding, isItem) = Shape(kind, byCode, rawcode);

        int hidden = 0;
        var groups = new Dictionary<string, List<(string Sort, FormField Field)>>(StringComparer.OrdinalIgnoreCase);

        // The form is built from the fields that APPLY to this object, not from the fields that
        // happen to hold a value.
        //
        // This was the wrong way round and it cost a lot of the form's usefulness. Iterating the
        // merged values means a field the object inherits without overriding, and that the base
        // data has no row for, never appears at all. Measured on a hero unit, the metadata says
        // 223 of its 273 fields apply and the form listed 160. Art showed 19 of 51 and Techtree 3
        // of 18. The Abilities group existed with 3 of its 5 rows, which reads as "this hero has
        // almost no ability fields" when the truth is they were simply unset.
        //
        // An editor has to offer an unset field, because setting it is the entire point. So the
        // metadata supplies the row list and the merged values fill in what exists, and a field
        // with no value anywhere is shown empty with source "unset".
        var valueOf = new Dictionary<string, MergedField>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in merged.Fields) valueOf[f.Code] = f;

        var rows = new List<MergedField>();
        if (meta is not null)
        {
            foreach (var fm in meta.Fields)
            {
                // Two filters, and both matter. AppliesTo answers whether the field applies to
                // an object of this SHAPE, a hero or a building or an item. AppliesToObject
                // answers whether it applies to THIS object, which for an ability is the
                // difference between about 70 fields and 769, because 708 of the 777 ability
                // fields belong to one named ability type rather than to abilities in general.
                if (!fm.AppliesTo(isHero, isBuilding, isItem)
                    || !fm.AppliesToObject(rawcode, merged.BaseRawcode)) { hidden++; continue; }

                // A per-level field surfaces once per level it actually carries, and unset at
                // level 1 otherwise, so a leveled kind is not flattened to a single row.
                var levelled = valueOf.Keys
                    .Where(k => k.Length > fm.Code.Length
                                && k.StartsWith(fm.Code + ":", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (levelled.Count > 0)
                {
                    foreach (var k in levelled) rows.Add(valueOf[k]);
                    continue;
                }
                rows.Add(valueOf.TryGetValue(fm.Code, out var have)
                    ? have
                    : new MergedField(fm.Code, DisplayNameOf(ctx, fm), "", "unset"));
            }

            // Anything the map defines that the metadata does not know about. Never drop a value
            // the map actually holds just because the installed patch has no row for its code.
            var known = new HashSet<string>(meta.Fields.Select(f => f.Code), StringComparer.OrdinalIgnoreCase);
            foreach (var f in merged.Fields)
            {
                var bareCode = f.Code.Contains(':') ? f.Code[..f.Code.IndexOf(':')] : f.Code;
                if (!known.Contains(bareCode)) rows.Add(f);
            }
        }
        else
        {
            // No game data, so the applicable set is unknowable and the values are all there is.
            rows.AddRange(merged.Fields);
        }

        foreach (var f in rows)
        {
            var bare = f.Code.Contains(':') ? f.Code[..f.Code.IndexOf(':')] : f.Code;
            ObjectFieldMeta? fm = null;
            if (meta is not null && meta.TryGet(bare, out var found)) fm = found;

            bool authoritative = fm is not null && fm.NetSafe.Length > 0;
            var layer = authoritative
                ? (fm!.IsSkinField ? ObjectLayer.Skin : ObjectLayer.Map)
                : partition.Value.LayerFor(bare);

            // Deliberately NOT resolving the option set here. A form is a LISTING, and the legal
            // values only matter for the one field someone selects to edit, which a front end asks
            // for separately through ObjectFieldOptionsCommand. Resolving them for every field cost
            // a base-data scan per field, 168 of them on one unit, which was most of why this pane
            // was four times slower than the flat field dump it replaced, to produce a value no
            // caller ever read.
            var fieldType = fm?.Type ?? "";

            var field = new FormField(
                Code: f.Code,
                Name: f.Name,
                Value: f.Value,
                Display: f.Display,
                Source: f.Source,
                Type: fieldType,
                MinValue: Blank(fm?.MinValue),
                MaxValue: Blank(fm?.MaxValue),
                ForceNonNegative: fm?.ForceNonNegative ?? false,
                // Absent metadata must not invent a constraint, so a blank stays legal.
                CanBeEmpty: fm?.CanBeEmpty ?? true,
                MultiLine: fm is not null && fm.StringExt > 0,
                Layer: layer,
                LayerIsAuthoritative: authoritative);

            var key = fm is null || fm.Category.Length == 0 ? "other" : fm.Category.ToLowerInvariant();
            // Sort key first, then the display name, so a group without sort keys is still ordered.
            var sort = (fm?.Sort ?? "") + "" + f.Name;
            if (!groups.TryGetValue(key, out var list)) groups[key] = list = new();
            list.Add((sort, field));
        }

        var ordered = new List<FormGroup>();
        foreach (var (key, title) in CategoryOrder)
            if (groups.TryGetValue(key, out var list))
            {
                ordered.Add(new FormGroup(key, CategoryTitle(ctx, key, title), Sorted(list)));
                groups.Remove(key);
            }
        // Anything the metadata did not categorise, or that arrived with no metadata at all.
        foreach (var key in groups.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
            ordered.Add(new FormGroup(key,
                key == "other" ? "Other" : CategoryTitle(ctx, key, Title(key)),
                Sorted(groups[key])));

        return new ObjectForm(rawcode, merged.Name, merged.BaseRawcode, kind, ordered,
            ordered.Sum(g => g.Fields.Count), hidden, diagnostics);
    }

    private static IReadOnlyList<FormField> Sorted(List<(string Sort, FormField Field)> list) =>
        list.OrderBy(x => x.Sort, StringComparer.OrdinalIgnoreCase).Select(x => x.Field).ToList();

    private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;

    private static string Title(string key) =>
        key.Length == 0 ? key : char.ToUpperInvariant(key[0]) + key[1..];

    /// <summary>
    /// Which of the four applicability flags to read, derived from the object itself.
    /// </summary>
    /// <remarks>
    /// The metadata states applicability per shape but the object does not carry a "shape" field,
    /// so it is inferred. A building says so in <c>ubld</c>. A hero is recognised by carrying the
    /// hero-only fields, a primary attribute or a hero ability list, which is more reliable than
    /// the uppercase-first-letter convention because a custom object need not follow it.
    /// </remarks>
    private static (bool Hero, bool Building, bool Item) Shape(
        ObjectKind kind, IReadOnlyDictionary<string, string> fields, string rawcode)
    {
        if (kind == ObjectKind.Item) return (false, false, true);

        bool building = fields.TryGetValue("ubld", out var b)
            && b.Trim() is "1" or "true" or "True";

        bool hero = (fields.TryGetValue("upra", out var pra) && pra.Trim().Length > 0)
                 || (fields.TryGetValue("uhab", out var hab) && hab.Trim().Length > 0)
                 // Last resort, and the convention every base-game hero follows.
                 || (rawcode.Length == 4 && char.IsUpper(rawcode[0]));

        return (hero && !building, building, false);
    }

    /// <summary>
    /// The label for a field the object does not currently hold, which therefore has no merged
    /// row to take a name from. Resolves the metadata's WESTRING display key, falling back to the
    /// field code, because a row labelled with a raw WESTRING is worse than one labelled with its
    /// code.
    /// </summary>
    private static string DisplayNameOf(GameDataContext? ctx, ObjectFieldMeta fm)
    {
        if (fm.DisplayName.Length == 0) return fm.Code;
        if (ctx is not null && ctx.Strings.TryGet(fm.DisplayName, out var resolved)
            && resolved.Length > 0)
            return resolved;
        return fm.DisplayName.StartsWith("WESTRING", StringComparison.OrdinalIgnoreCase)
            ? fm.Code
            : fm.DisplayName;
    }
}
