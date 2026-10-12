// tests/Wc3.Tests/ScriptTestSchema.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Builds a throwaway schema folder declaring a set of natives and functions, so the target-aware
/// script commands (api-usage, modernize) can be exercised hermetically with no game install.
/// </summary>
internal static class ScriptTestSchema
{
    internal static string Save(string dir, string build,
        IEnumerable<string> natives, IEnumerable<string>? functions = null)
    {
        var jass = new JassApi(
            Array.Empty<JassTypeDecl>(),
            natives.Select(n => new JassNativeDecl(n, Array.Empty<JassParameter>(), "nothing", false)).ToList(),
            Array.Empty<JassGlobalDecl>(),
            (functions ?? Array.Empty<string>())
                .Select(f => new JassFunctionDecl(f, Array.Empty<JassParameter>(), "nothing", false)).ToList());

        var schema = new GameSchema(1, build, build,
            Array.Empty<SchemaInput>(), new[] { "script" }, null, (SchemaTriggers?)null,
            new Dictionary<string, JassApi>(StringComparer.Ordinal)
            {
                ["scripts/common.j"] = jass,
            });

        Directory.CreateDirectory(dir);
        GameSchemaCommand.Save(schema, dir);
        return dir;
    }
}
