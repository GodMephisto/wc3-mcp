// tests/Wc3.Tests/CorpusSweep.cs
namespace Wc3.Tests;

/// <summary>
/// Bounds a test that walks a whole library of maps.
///
/// Two Corpus tests enumerate every <c>.w3x</c> and <c>.w3m</c> under the maps folder recursively
/// and load each one. On this machine that is thirty-odd maps including several over 240 MB, plus
/// delivered copies in nested folders, so the sweep reads gigabytes of MPQ and pushes the Corpus
/// category past a ten minute timeout while spiking memory.
///
/// A sweep is still the right shape for those tests, since the point is breadth. What was missing
/// is a bound. So the candidate list is capped, smallest first, and <see cref="Describe"/> reports
/// what was left out.
///
/// Reporting the cap is not decoration. A test that silently samples reads as though it covered
/// everything, which is the same class of mistake as a test that silently skips.
/// <c>WC3CTL_CORPUS_SWEEP_ALL=1</c> lifts the cap for a deliberate full run.
/// </summary>
internal static class CorpusSweep
{
    private const string AllVar = "WC3CTL_CORPUS_SWEEP_ALL";

    /// <summary>How many maps a bounded sweep reads. Enough for breadth across formats and
    /// protection states, small enough to finish in seconds rather than minutes.</summary>
    public const int MaxMaps = 8;

    /// <summary>Skip anything larger than this in a bounded sweep. The big maps are the slow ones
    /// and they exercise the same code paths as the small ones.</summary>
    private const long MaxBytes = 40L * 1024 * 1024;

    public static bool SweepAll =>
        Environment.GetEnvironmentVariable(AllVar) is { Length: > 0 } v && v != "0";

    private static int _lastConsidered;
    private static int _lastTaken;

    /// <summary>
    /// Bounds a candidate list. Smallest first, because size correlates with load time and not
    /// with how much of the format a map exercises.
    /// </summary>
    public static List<string> Bound(IEnumerable<string> candidates)
    {
        var all = candidates.ToList();
        _lastConsidered = all.Count;

        if (SweepAll)
        {
            _lastTaken = all.Count;
            return all;
        }

        var taken = all
            .Select(p => new FileInfo(p))
            .Where(f => f.Exists && f.Length <= MaxBytes)
            .OrderBy(f => f.Length)
            .Take(MaxMaps)
            .Select(f => f.FullName)
            .ToList();

        _lastTaken = taken.Count;
        return taken;
    }

    /// <summary>What the last bounded sweep actually read, and what it did not.</summary>
    public static string Describe() =>
        SweepAll
            ? $"swept all {_lastConsidered} map(s), cap lifted by {AllVar}"
            : $"swept {_lastTaken} of {_lastConsidered} map(s). Capped at {MaxMaps} and "
              + $"{MaxBytes / (1024 * 1024)} MB to keep the suite fast. "
              + $"Set {AllVar}=1 for the full library.";
}
