// src/Wc3.Commands/GameBuildName.cs
// Parses Warcraft III build names into a structured form for correct
// version-aware ordering.  Handles bare builds (1.30.4.11274), prefixed
// builds (TFT-v1.31.1.12173), beta builds (Beta-ROC-v1.00,
// Beta-TFT-v315), test builds (…-w3t, …-w3b), and locale variants
// (-ru, -zhtw).

using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>
/// A parsed Warcraft III build name, supporting correct version-aware
/// ordering across all known build variants (bare, TFT, ROC,
/// Reforged, Beta, test, locale).
/// </summary>
public sealed record GameBuildName(
    string Raw,
    Version Version,
    string Line,          // full name after version extraction (prefix, locale, test tag)
    bool IsTest,          // has -w3t or -w3b suffix
    bool IsBeta,          // starts with Beta-
    string? Locale,
    string? LetterSuffix, // "a" in 1.24a, null for bare
    string? Prefix)       // "TFT", "ROC", "Reforged", "Beta-ROC", "Beta-TFT"
{

    /// <summary>Parse a raw build-name string into a structured, comparable form.</summary>
    /// <param name="raw">A build folder name (e.g. "TFT-v1.31.1.12173", "Beta-ROC-v1.00", "1.30.4.11274").</param>
    /// <returns>A parsed GameBuildName ready for comparison.</returns>
    public static GameBuildName Parse(string raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        var s = raw;
        var isBeta = false;
        var prefix = (string?)null;

        // Detect Beta- prefix (before any "v").
        string betaPrefix = "Beta-";
        if (s.Length >= betaPrefix.Length && s.StartsWith(betaPrefix, StringComparison.Ordinal))
        {
            isBeta = true;
            s = s[betaPrefix.Length..];

            // Extract category: "ROC" or "TFT" from e.g. "ROC-v1.00" or "TFT-v1.07".
            int dash = s.IndexOf('-');
            if (dash > 0)
            {
                prefix = s[..dash]; // "ROC" or "TFT"
            }
        }
        else
        {
            // Non-beta: extract prefix (TFT, ROC, Reforged) before the first '-v'.
            string marker = "-v";
            int vDash = s.IndexOf(marker, StringComparison.Ordinal);
            if (vDash > 0)
            {
                prefix = s[..vDash];
                s = s[(vDash + marker.Length)..]; // strip "TFT-v" -> "1.31.1.12173"
            }
        }

        // Extract version: first dotted number sequence from remaining string.
        string versionPart = s;
        Match vm = Regex.Match(s, @"\d+(?:\.\d+)*");
        string? letterSuffix = null;
        if (vm.Success)
        {
            versionPart = vm.Value;
            // Check for letter suffix on the LAST segment (e.g. "1.24a").
            string restAfter = s[vm.Length..];
            if (restAfter.Length == 1 && char.IsLetter(restAfter[0]))
            {
                letterSuffix = restAfter.ToLowerInvariant();
            }
        }

        // Parse into Version.
        Version version;
        try
        {
            version = new Version(versionPart);
        }
        catch
        {
            version = new Version(0, 0);
        }

        // Build "Line" = what remains after the version number (for ordering hints).
        int verEnd = vm.Success ? vm.Length : 0;
        string line = verEnd < s.Length ? s[verEnd..] : string.Empty;

        // Detect test builds: "-w3t", "-w3b", or "-w3-" followed by a hash.
        bool isTest = line.IndexOf("-w3t", StringComparison.Ordinal) >= 0
                   || line.IndexOf("-w3b", StringComparison.Ordinal) >= 0
                   || line.IndexOf("-w3-", StringComparison.Ordinal) >= 0;

        // Detect locale: "-ru", "-zhtw" etc. at the end.
        string? locale = null;
        foreach (var loc in new[] { "-zhtw", "-ru" })
        {
            if (line.EndsWith(loc, StringComparison.Ordinal))
            {
                locale = loc.Substring(1); // strip leading dash
                break;
            }
        }

        return new GameBuildName(raw, version, line, isTest, isBeta, locale, letterSuffix, prefix);
    }

    /// <summary>
    /// Compare two build names by version-aware ordering rules.
    /// Returns -1, 0, or +1.
    ///
    /// Ordering:
    /// 1. Beta-ROC versions come before ROC-v1.00.
    /// 2. Beta-TFT versions slot between ROC-v1.06 and TFT-v1.07.
    /// 3. Main releases (TFT, ROC, Reforged) sort by version.
    /// 4. Within the same version, release sorts before test builds.
    /// 5. Same version + same type, locale sorts by locale ordinal.
    /// 6. Bare versions (3.0.0.24268) sort by version after Reforged/standard.
    /// </summary>
    public static int Compare(GameBuildName a, GameBuildName b)
    {
        if (a is null) throw new ArgumentNullException(nameof(a));
        if (b is null) throw new ArgumentNullException(nameof(b));

        int cmp = CompareCategory(a, b);
        if (cmp != 0) return cmp;

        cmp = a.Version.CompareTo(b.Version);
        if (cmp != 0) return cmp;

        // Same version. Release before test build.
        cmp = a.IsTest.CompareTo(b.IsTest);
        if (cmp != 0) return cmp;

        // Compare letter suffixes (a, b, c). Null means bare, which sorts before letters.
        if (a.LetterSuffix != b.LetterSuffix)
        {
            if (a.LetterSuffix is null) return -1;
            if (b.LetterSuffix is null) return 1;
            cmp = string.Compare(a.LetterSuffix, b.LetterSuffix, StringComparison.Ordinal);
            if (cmp != 0) return cmp;
        }

        // Compare locale (ordinal).
        if (a.Locale != b.Locale)
        {
            if (a.Locale is null) return 1;  // no locale sorts after locale'd (English first)
            if (b.Locale is null) return -1;
            return string.Compare(a.Locale, b.Locale, StringComparison.Ordinal);
        }

        // Compare remaining line (test hash like -w3-abc123, -w3b-ab0c8ee).
        int lineCmp = string.Compare(a.Line, b.Line, StringComparison.Ordinal);
        if (lineCmp != 0) return lineCmp;

        // Same version, same type, same locale, same line:
        // bare (no prefix) sorts after named (has prefix).
        if (a.Prefix != b.Prefix)
        {
            if (a.Prefix is null) return 1;
            if (b.Prefix is null) return -1;
        }
        return 0;
    }

    /// <summary>
    /// Determine the "category" or slot of a build for inter-category ordering.
    /// Categories (ascending): Beta-ROC, ROC, Beta-TFT, TFT, Reforged, Bare.
    /// </summary>
    private static int CompareCategory(GameBuildName a, GameBuildName b)
    {
        int catA = GetCategory(a);
        int catB = GetCategory(b);
        int cmp = catA.CompareTo(catB);
        if (cmp != 0) return cmp;

        return 0;
    }

    /// <summary>
    /// Return a category ordinal for inter-category ordering.
    /// 0 = Beta-ROC, 1 = ROC, 2 = Beta-TFT, 3 = TFT, 4 = Reforged, 5 = Bare.
    /// Unknown prefixes fall into the closest matching category.
    /// For bare (non-beta) names, determine the category from the version:
    /// below 1.07 is ROC, 1.07 through 1.31 is TFT, 1.32 and above is Reforged.
    /// Bare names always sort after named builds (add 1 to their category tier).
    /// </summary>
    private static int GetCategory(GameBuildName b)
    {
        if (b.IsBeta)
        {
            return b.Prefix?.ToLowerInvariant() switch
            {
                "roc" => 0,
                "tft" => 2,
                _ => 5,
            };
        }
        // Named prefix: use the prefix directly.
        if (b.Prefix is not null)
        {
            return b.Prefix.ToLowerInvariant() switch
            {
                "roc" => 1,
                "tft" => 3,
                "reforged" => 4,
                _ => 5,
            };
        }
        // Bare name: determine category tier from version.
        // Below 1.07 → ROC (1). 1.07 through 1.31 → TFT (3). 1.32+ → Reforged (4).
        // Then use the version to sort within the tier.
        // Bare names sort after named builds of the same version (no prefix > has prefix).
        Version v = b.Version;
        if (v.Major < 1 || (v.Major == 1 && v.Minor < 7))
        {
            return 1;  // ROC era
        }
        if (v.Major == 1 && v.Minor < 32)
        {
            return 3;  // TFT era: 1.07 through 1.31.x
        }
        return 4;  // Reforged era: 1.32+, 2.x, 3.x, ...
    }
}
