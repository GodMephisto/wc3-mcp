// src/Wc3.Commands/War3LogFaults.cs
using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>One engine fault, its kind, the asset or frame it names, and how many times it appeared.</summary>
public sealed record War3LogFault(string Kind, string Path, int Count, string Detail);

/// <summary>
/// Reads the faults Warcraft III writes to its own log, War3Log.txt. The engine logs no load list,
/// a map cannot see what the loading screen read, but it does write the assets it could not use,
/// and those are the errors a static inventory cannot see. The known shapes are a texture it
/// replaced with a solid colour, a model it could not create, a UI frame defined twice, and a
/// frame string declared twice. Everything else that reads as an error or a failure is kept as a
/// generic engine error so nothing the engine complained about is dropped.
/// </summary>
public static class War3LogFaults
{
    private static readonly (Regex Pattern, string Kind)[] Known =
    {
        (new Regex(@"Solid texture substituted\s*-\s*(.+?)\s*$", RegexOptions.IgnoreCase), "SolidTextureSubstituted"),
        (new Regex(@"model creation failed\s*-\s*(.*?)\s*$", RegexOptions.IgnoreCase), "ModelCreationFailed"),
        (new Regex(@"Error,\s*frame\s*""(.+?)""\s*already encountered", RegexOptions.IgnoreCase), "DuplicateFrame"),
        (new Regex(@"Error,\s*string\s+(\S+)\s+already exists", RegexOptions.IgnoreCase), "DuplicateFrameString"),
    };

    private static readonly Regex Timestamped = new(@"^\s*\d+/\d+\s+\d+:\d+:\d+\.\d+\s+(.*)$", RegexOptions.Compiled);
    private static readonly Regex LooksLikeFault = new(@"\bError,|failed|Substituted|missing", RegexOptions.IgnoreCase);

    /// <summary>The default log beside a documents folder, Documents\Warcraft III\Logs\War3Log.txt.</summary>
    public static string DefaultPath(string documentsDir) =>
        Path.Combine(documentsDir, "Warcraft III", "Logs", "War3Log.txt");

    /// <summary>
    /// Every fault in the log, grouped by kind and the asset or frame it names, most frequent
    /// first. Returns an empty list when the file is absent, so a missing log is not an error.
    /// </summary>
    public static IReadOnlyList<War3LogFault> Read(string logPath)
    {
        if (string.IsNullOrWhiteSpace(logPath) || !File.Exists(logPath))
            return Array.Empty<War3LogFault>();

        var counts = new Dictionary<(string Kind, string Path), (int Count, string Detail)>();
        foreach (var raw in File.ReadLines(logPath))
        {
            var match = Timestamped.Match(raw);
            var line = (match.Success ? match.Groups[1].Value : raw).Trim();
            if (line.Length == 0) continue;

            bool matched = false;
            foreach (var (pattern, kind) in Known)
            {
                var m = pattern.Match(line);
                if (!m.Success) continue;
                var path = m.Groups[1].Value.Trim();
                Add(counts, kind, path, line);
                matched = true;
                break;
            }
            if (!matched && LooksLikeFault.IsMatch(line))
                Add(counts, "EngineError", line, line);
        }

        return counts
            .Select(kv => new War3LogFault(kv.Key.Kind, kv.Key.Path, kv.Value.Count, kv.Value.Detail))
            .OrderByDescending(f => f.Count)
            .ThenBy(f => f.Kind, StringComparer.Ordinal)
            .ThenBy(f => f.Path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void Add(
        Dictionary<(string Kind, string Path), (int Count, string Detail)> counts,
        string kind, string path, string detail)
    {
        var key = (kind, path);
        counts[key] = counts.TryGetValue(key, out var existing)
            ? (existing.Count + 1, existing.Detail)
            : (1, detail);
    }
}
