// src/Wc3.Commands/MpqNameRecoverCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One archive block that had no name and a dictionary line resolved it.</summary>
public sealed record MpqRecoveredName(int BlockIndex, string Name, int SizeBytes);

public sealed record MpqNameRecoverResult(
    string DictionaryPath,
    int CandidatesTried,
    int UnnamedBefore,
    int Recovered,
    IReadOnlyList<MpqRecoveredName> Names);

/// <summary>
/// Resolves a caller-supplied dictionary against the archive's hash table and reports the
/// names that hit a block which had no name before.
///
/// This is the narrow, direct probe of <see cref="NameRecoveryCommand"/>. That command harvests
/// candidates from the map's own data and script and folds in listfiles, which is the right tool
/// for deprotecting a map in one pass. This one is the question underneath it, take exactly the
/// names I give you and tell me which of them the archive answers to. An MPQ locates a file by
/// the hash of its name, so a hit here proves the archive holds that name even when it ships no
/// listfile, and a miss is honest about a dictionary line the archive does not carry.
/// </summary>
public static class MpqNameRecoverCommand
{
    public static MpqNameRecoverResult Execute(MapDocument doc, string dictionaryPath)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(dictionaryPath);

        var candidates = ReadDictionary(dictionaryPath);
        int unnamedBefore = doc.Files.Count(f => f.FileName is null);

        var hits = doc.RecoverNames(candidates);
        var sizes = doc.UnnamedBlocks();
        var names = hits
            .OrderBy(kv => kv.Key)
            .Select(kv => new MpqRecoveredName(kv.Key, kv.Value, sizes.GetValueOrDefault(kv.Key)))
            .ToList();

        return new MpqNameRecoverResult(dictionaryPath, candidates.Count, unnamedBefore,
            names.Count, names);
    }

    /// <summary>One candidate name per line. Blank lines and surrounding space are dropped.</summary>
    private static IReadOnlyList<string> ReadDictionary(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"dictionary not found: {path}", path);
        var names = new List<string>();
        foreach (var line in File.ReadLines(path))
        {
            var trimmed = line.Trim();
            if (trimmed.Length > 0) names.Add(trimmed);
        }
        return names;
    }
}
