// src/Wc3.Commands/ObjectTargetFieldsCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One build the object editor can target, with whether it carries object data.</summary>
public sealed record ObjectTargetBuild(string Build, bool HasObjectData, string Era);

/// <summary>
/// One field of an object, annotated with what a target build's schema says about it. Known is
/// false when the build defines no such field, which is the compat signal for the object editor.
/// </summary>
public sealed record ObjectTargetField(
    string Code, string Name, bool Known, string Type, string? MinValue, string? MaxValue, bool CanBeEmpty)
{
    /// <summary>Type, bounds and blank policy as one compact line for a bounds hint.</summary>
    public string Hint
    {
        get
        {
            string type = Type.Length == 0 ? "type unknown" : Type;
            string bounds = MinValue is null && MaxValue is null
                ? "any value"
                : $"{MinValue ?? "*"}..{MaxValue ?? "*"}";
            string blank = CanBeEmpty ? "blank allowed" : "blank not allowed";
            return $"{type}  {bounds}  {blank}";
        }
    }
}

/// <summary>
/// A target build's view of one object, or the reason the build has none. HasObjectData false with
/// a Message means the build defines no object data, so a front end must say so rather than showing
/// the installed build's fields, which would misrepresent what the target supports.
/// </summary>
public sealed record ObjectTargetFormResult(
    string Build, bool HasObjectData, string Message, IReadOnlyList<ObjectTargetField> Fields);

/// <summary>
/// Lists the builds the object editor can target and reads one object's fields the way a chosen
/// build's schema defines them. Everything comes from the schema folders, so it works with no game
/// install, and the same data backs the selector, the missing-data message and the bounds hint.
/// </summary>
public static class ObjectTargetFieldsCommand
{
    /// <summary>Every build the schemas index declares, oldest first, with its object-data flag.</summary>
    public static IReadOnlyList<ObjectTargetBuild> Builds(string? schemasDir = null) =>
        TargetBuildSettings.List(schemasDir)
            .Select(b => new ObjectTargetBuild(
                b.Build,
                b.Sections.Contains("objects"),
                GamedataSupport.Era(b.Build)))
            .ToList();

    /// <summary>Whether a build carries an objects section, so a form can be shown for it. Answered
    /// from the cached build list rather than by parsing the build's whole schema again, which is
    /// what the Studio's object panel asks on every ShowMap.</summary>
    public static bool HasObjectData(string build, string? schemasDir = null) =>
        TargetBuildSettings.List(schemasDir)
            .Any(b => b.Build == build && b.Sections.Contains("objects"));

    /// <summary>
    /// The build's view of one object's fields, or a message when the build has no object data.
    /// </summary>
    public static ObjectTargetFormResult Execute(
        MapDocument doc, ObjectKind kind, string rawcode, string build,
        string? schemasDir = null, string? gameDir = null)
    {
        if (string.IsNullOrEmpty(build))
            return new("", false, "No target build is available.", Array.Empty<ObjectTargetField>());

        var schema = TargetBuildSettings.LoadSchema(build, schemasDir);
        if (schema.Objects is not { Count: > 0 })
            return new(schema.Build, false,
                $"Build {schema.Build} defines no object data, so the object editor cannot "
                + "show fields for it. Pick a build that carries object data.",
                Array.Empty<ObjectTargetField>());

        var byCode = FieldsByCode(schema, kind);
        if (byCode.Count == 0)
            return new(schema.Build, false,
                $"Build {schema.Build} defines no {kind.ToString().ToLowerInvariant()} object data.",
                Array.Empty<ObjectTargetField>());

        var fields = new List<ObjectTargetField>();
        foreach (var f in ObjectGetCommand.Execute(doc, kind, rawcode, gameDir).Fields)
        {
            var bare = BareCode(f.Code);
            byCode.TryGetValue(bare, out var sf);
            fields.Add(new ObjectTargetField(
                f.Code,
                sf is null ? f.Name : DisplayNameOf(sf),
                sf is not null,
                sf?.Type ?? "",
                Blank(sf?.MinValue),
                Blank(sf?.MaxValue),
                sf?.CanBeEmpty ?? true));
        }
        return new(schema.Build, true, "", fields);
    }

    /// <summary>
    /// One field code's definition in a build's schema, for a bounds hint on the field under edit,
    /// or null when the build has no object data or does not define the code.
    /// </summary>
    public static ObjectTargetField? Field(
        string build, ObjectKind kind, string code, string? schemasDir = null)
    {
        if (string.IsNullOrEmpty(build)) return null;
        var schema = TargetBuildSettings.LoadSchema(build, schemasDir);
        var byCode = FieldsByCode(schema, kind);
        return byCode.TryGetValue(BareCode(code), out var sf)
            ? new ObjectTargetField(BareCode(code), DisplayNameOf(sf), true,
                sf.Type, Blank(sf.MinValue), Blank(sf.MaxValue), sf.CanBeEmpty)
            : null;
    }

    /// <summary>The codes a build defines for one kind, from every metadata file of that kind.</summary>
    private static Dictionary<string, SchemaField> FieldsByCode(GameSchema schema, ObjectKind kind)
    {
        var dict = new Dictionary<string, SchemaField>(StringComparer.Ordinal);
        if (schema.Objects is null) return dict;
        string wanted = kind.ToString();
        foreach (var (slk, fields) in schema.Objects)
        {
            if (!string.Equals(TargetCompatCommand.SchemaKindFromPath(slk), wanted,
                    StringComparison.OrdinalIgnoreCase))
                continue;
            foreach (var f in fields) dict[f.Code] = f;
        }
        return dict;
    }

    private static string BareCode(string code)
    {
        int colon = code.IndexOf(':');
        return colon > 0 ? code[..colon] : code;
    }

    private static string DisplayNameOf(SchemaField field) =>
        string.IsNullOrEmpty(field.DisplayName) ? field.Code : field.DisplayName;

    private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;
}
