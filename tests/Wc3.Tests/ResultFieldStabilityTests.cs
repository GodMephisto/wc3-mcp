// tests/Wc3.Tests/ResultFieldStabilityTests.cs
// CMP-04. A result field may be added inside a major version and never removed without a decision.
// This test reads the published result shape of every tool, the return type the SDK derives the
// output schema from (RES-02), records every field name per tool in a checked in file, and fails
// when a name disappears unless the same file lists that rename. An addition is written back, so
// the record stays current and a removal is the only thing that can fail. A rename is then a
// decision a reviewer reads in the diff, not an accident nobody noticed.
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol.Server;
using Wc3.Mcp;

namespace Wc3.Tests;

public sealed class ResultFieldStabilityTests
{
    /// <summary>The checked in record of every result field name, relative to the repository root.
    /// Its renames list is the only place a disappeared name may be explained.</summary>
    private const string RecordPath = "tests/Wc3.Tests/Fixtures/result-fields.json";

    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    [Fact]
    public void A_result_field_changes_only_additively()
    {
        var current = CurrentFields();
        string path = Path.Combine(RepoRoot(), RecordPath.Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(path), RecordPath + " is missing, so no test pins the result fields");

        var record = JsonSerializer.Deserialize<FieldRecord>(File.ReadAllText(path), Json);
        Assert.NotNull(record);
        Assert.NotNull(record!.Tools);
        Assert.NotEmpty(record.Tools);

        var renames = record.Renames ?? Array.Empty<Rename>();
        var added = new List<string>();
        var dropped = new List<string>();
        var removed = new List<string>();
        var merged = new SortedDictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var (tool, fields) in current)
        {
            string[] before = record.Tools.TryGetValue(tool, out var recorded) && recorded is not null
                ? recorded
                : Array.Empty<string>();
            var now = fields.ToHashSet(StringComparer.Ordinal);

            foreach (string name in fields)
                if (!before.Contains(name, StringComparer.Ordinal))
                    added.Add(tool + "." + name);

            foreach (string name in before)
            {
                if (now.Contains(name)) continue;
                if (Renamed(renames, tool, name)) dropped.Add(tool + "." + name);
                else removed.Add(tool + "." + name);
            }

            merged[tool] = fields;
        }

        foreach (string tool in record.Tools.Keys)
            if (!current.ContainsKey(tool))
            {
                if (Renamed(renames, tool, "*")) dropped.Add(tool);
                else removed.Add(tool + " (the whole tool)");
            }

        // Keep the record current. An addition is accepted, and so is a disappearance the renames
        // list already explains, which is what lets a rename land as one reviewed line in the diff.
        if ((added.Count > 0 || dropped.Count > 0) && removed.Count == 0)
        {
            string text = JsonSerializer.Serialize(
                new { note = record.Note, renames = record.Renames, tools = merged }, Json);
            File.WriteAllText(path, text.Replace("\r\n", "\n"), new UTF8Encoding(false));
        }

        Assert.True(removed.Count == 0,
            "result fields that disappeared without an entry in the renames list of " + RecordPath + ". "
            + string.Join(", ", removed)
            + ". Keep the old name as an alias for this version, or add a renames entry naming the tool, "
            + "the old name, the new name and the release, so the rename is a decision rather than an accident.");
    }

    /// <summary>True when the record explains the disappeared name, either as a field rename on that
    /// tool or, for the whole tool, as a wildcard entry.</summary>
    private static bool Renamed(IReadOnlyList<Rename> renames, string tool, string from) =>
        renames.Any(r =>
            string.Equals(r.Tool, tool, StringComparison.Ordinal)
            && string.Equals(r.From, from, StringComparison.Ordinal));

    /// <summary>Every tool the surface serves, with the field names a caller reads in its result.</summary>
    private static SortedDictionary<string, string[]> CurrentFields()
    {
        var byTool = new SortedDictionary<string, string[]>(StringComparer.Ordinal);

        foreach (Type host in new[] { typeof(Wc3Tools), typeof(Wc3Context) })
        {
            foreach (MethodInfo method in host.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                McpServerToolAttribute? attribute = method.GetCustomAttribute<McpServerToolAttribute>();
                if (attribute is null) continue;
                string name = string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name;
                byTool[name] = FieldsOf(method.ReturnType);
            }
        }

        Assert.True(byTool.Count > 200,
            "the surface holds " + byTool.Count + " tools, which is too few to be the whole surface");

        // The names this test read are the names the server actually serves, so a tool added without
        // a method here, or a method the server does not bind, fails rather than leaving a gap.
        var served = Wc3McpServer.CreateTools()
            .Select(t => t.ProtocolTool.Name)
            .ToHashSet(StringComparer.Ordinal);
        var reflected = byTool.Keys.ToHashSet(StringComparer.Ordinal);
        Assert.True(served.SetEquals(reflected),
            "the served tools and the result types disagree. Only served, "
            + string.Join(", ", served.Except(reflected).OrderBy(n => n, StringComparer.Ordinal))
            + ". Only typed, "
            + string.Join(", ", reflected.Except(served).OrderBy(n => n, StringComparer.Ordinal)));

        return byTool;
    }

    /// <summary>The field names of one result. A result that is an array of rows names the row's
    /// fields, since that is what a caller reads. A scalar result, an object typed result and a
    /// keyed dictionary name no fields here, because the call decides those names.</summary>
    private static string[] FieldsOf(Type type, int depth = 0)
    {
        type = Unwrap(type);
        if (depth > 2 || type == typeof(object) || type == typeof(JsonElement) || type == typeof(JsonDocument)
            || type.IsEnum || IsScalar(type))
            return Array.Empty<string>();

        if (typeof(IDictionary).IsAssignableFrom(type))
            return Array.Empty<string>();

        if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
        {
            Type? element = ElementType(type);
            return element is null ? Array.Empty<string>() : FieldsOf(element, depth + 1);
        }

        var names = new List<string>();
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length > 0) continue;
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() is not null) continue;
            names.Add(JsonName(property));
        }

        return names.Distinct(StringComparer.Ordinal).OrderBy(n => n, StringComparer.Ordinal).ToArray();
    }

    /// <summary>The name the serializer gives the property, which is the name a caller reads.</summary>
    private static string JsonName(PropertyInfo property) =>
        property.GetCustomAttribute<JsonPropertyNameAttribute>() is { } named
            ? named.Name
            : Wc3McpServer.JsonOptions.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;

    private static bool IsScalar(Type type) =>
        type == typeof(string) || type == typeof(bool) || type == typeof(char)
        || type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort)
        || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
        || type == typeof(float) || type == typeof(double) || type == typeof(decimal)
        || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(DateTimeOffset)
        || type == typeof(TimeSpan);

    /// <summary>Removes a nullable wrapper and a task wrapper, since neither changes the JSON.</summary>
    private static Type Unwrap(Type type)
    {
        while (true)
        {
            Type? nullable = Nullable.GetUnderlyingType(type);
            if (nullable is not null) { type = nullable; continue; }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                type = type.GetGenericArguments()[0];
                continue;
            }
            return type;
        }
    }

    /// <summary>The element type of a collection, or null when it holds no one named type.</summary>
    private static Type? ElementType(Type type)
    {
        if (type.IsArray) return type.GetElementType();
        foreach (Type contract in type.GetInterfaces())
            if (contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return contract.GetGenericArguments()[0];
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            ? type.GetGenericArguments()[0]
            : null;
    }

    /// <summary>The repository root, found by walking up from the test binary to the solution.</summary>
    private static string RepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (dir is { Length: > 0 })
        {
            if (File.Exists(Path.Combine(dir, "Wc3.sln"))) return dir;
            var parent = Directory.GetParent(dir);
            if (parent is null) break;
            dir = parent.FullName;
        }
        throw new InvalidOperationException("could not locate the repository root from " + AppContext.BaseDirectory);
    }

    /// <summary>The checked in record. The note is prose for a reader. The renames list is the only
    /// place a disappeared name may be explained, and the tools map is every field name per tool.</summary>
    public sealed record FieldRecord(
        string? Note,
        IReadOnlyList<Rename>? Renames,
        Dictionary<string, string[]> Tools);

    /// <summary>One rename decision. From is the field that disappeared and To is what replaced it,
    /// or a wildcard From when the whole tool was renamed.</summary>
    public sealed record Rename(string Tool, string From, string? To, string? Release);
}
