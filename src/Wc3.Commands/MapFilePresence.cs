// src/Wc3.Commands/MapFilePresence.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Why a map's list of something came back empty.</summary>
public enum MapFileState
{
    /// <summary>The archive has no entry under that name. On a protected map that can also mean
    /// the entry is there and its name was stripped, which is why the wording says archive
    /// rather than map.</summary>
    Absent,

    /// <summary>The entry exists but did not parse, so its contents are unknown. The loader
    /// records the reason as a diagnostic rather than throwing, which is what makes this state
    /// so easy to mistake for genuine emptiness.</summary>
    Unreadable,

    /// <summary>The entry exists, parsed, and really does hold nothing.</summary>
    Empty,
}

/// <summary>
/// Distinguishes "this map has none of these" from "we could not find out".
///
/// Three catalog panels told the user a specific thing that none of them could know. The cameras
/// panel said "Its war3map.w3c holds a version and a count of zero, so there is nothing to list",
/// and the regions panel said the same of war3map.w3r, when the command layer returns an empty
/// list for an absent file and for an unparseable one too. MapDocument records a parse failure as
/// a diagnostic and leaves Model null rather than throwing, so a corrupt file and an empty file
/// are indistinguishable to a caller that only looks at the count.
///
/// The result is a panel that confidently explains a file it never read. A user with a damaged
/// map is told their map is fine. That is worse than saying nothing, because it stops the
/// investigation.
///
/// The player panel already had the honest version, "war3map.w3i missing or unparseable", and
/// this generalises it so the wording lives in one place rather than being reinvented per panel.
/// </summary>
public static class MapFilePresence
{
    /// <summary>Which of the three situations produced an empty list.</summary>
    public static MapFileState Of(MapDocument doc, string fileName)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = doc.GetFile(fileName);
        if (entry is null) return MapFileState.Absent;
        return entry.Model is null ? MapFileState.Unreadable : MapFileState.Empty;
    }

    /// <summary>
    /// A sentence explaining an empty list, accurate for whichever of the three cases holds.
    /// </summary>
    /// <param name="whatItHolds">Plural noun for the contents, for example "cameras".</param>
    /// <param name="normallyEmpty">Text appended only when the file really is present and empty,
    /// for the cases where empty is unremarkable and the user should not go looking for a
    /// problem.</param>
    public static string Describe(
        MapDocument doc, string fileName, string whatItHolds, string? normallyEmpty = null)
    {
        switch (Of(doc, fileName))
        {
            case MapFileState.Absent:
                return $"This archive has no {fileName}, so it defines no {whatItHolds}. "
                     + "A protected map can also reach this state by having the entry with its "
                     + "name stripped.";

            case MapFileState.Unreadable:
                var why = doc.Diagnostics
                    .FirstOrDefault(d => string.Equals(d.FileName, fileName,
                        StringComparison.OrdinalIgnoreCase))?.Message;
                return $"{fileName} is present but could not be read, so its {whatItHolds} are "
                     + "unknown rather than absent."
                     + (string.IsNullOrWhiteSpace(why) ? string.Empty : $" The loader said: {why}");

            default:
                return $"{fileName} was read and defines no {whatItHolds}."
                     + (string.IsNullOrWhiteSpace(normallyEmpty) ? string.Empty : " " + normallyEmpty);
        }
    }
}
