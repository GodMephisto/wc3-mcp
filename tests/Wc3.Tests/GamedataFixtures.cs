// tests/Wc3.Tests/GamedataFixtures.cs
// Builds temporary schema folders for the gamedata command tests using GameSchemaCommand.Save, so
// no test depends on the untracked data/schemas directory.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

internal static class GamedataFixtures
{
    /// <summary>One object field with the given code, type and bounds.</summary>
    internal static SchemaField Field(
        string code, string type, string min, string max, bool canEmpty, string? display = null) =>
        new(code, "units/unitmetadata.slk", "Column", type, "Key" + code, display ?? ("Name " + code),
            "Category", 0, 0, 0, "", "", min, max, canEmpty, false, true, false, false);

    /// <summary>
    /// Saves one schema folder named after the build. Objects are written when slk is given, and
    /// script when any of the identifier lists is given, so a build can be objects only, script only,
    /// or both.
    /// </summary>
    internal static string Save(
        string root, string build,
        string? slk = null, SchemaField[]? fields = null,
        string[]? natives = null, string[]? types = null,
        string[]? globals = null, string[]? functions = null)
    {
        var dir = Path.Combine(root, build);
        Directory.CreateDirectory(dir);

        IReadOnlyDictionary<string, IReadOnlyList<SchemaField>>? objects = null;
        if (slk is not null)
            objects = new Dictionary<string, IReadOnlyList<SchemaField>>(StringComparer.Ordinal)
            {
                [slk] = fields ?? Array.Empty<SchemaField>(),
            };

        IReadOnlyDictionary<string, JassApi>? script = null;
        if (natives is not null || types is not null || globals is not null || functions is not null)
            script = new Dictionary<string, JassApi>(StringComparer.Ordinal)
            {
                ["scripts/common.j"] = new JassApi(
                    (types ?? Array.Empty<string>()).Select(t => new JassTypeDecl(t, "handle")).ToList(),
                    (natives ?? Array.Empty<string>()).Select(n => new JassNativeDecl(
                        n, Array.Empty<JassParameter>(), "boolean", false)).ToList(),
                    (globals ?? Array.Empty<string>()).Select(g => new JassGlobalDecl(
                        "integer", g, false, false, null)).ToList(),
                    (functions ?? Array.Empty<string>()).Select(f => new JassFunctionDecl(
                        f, Array.Empty<JassParameter>(), "nothing", false)).ToList()),
            };

        GameSchemaCommand.Save(new GameSchema(1, build, build,
            Array.Empty<SchemaInput>(), Array.Empty<string>(), objects, null, script), dir);
        return dir;
    }
}
