// src/Wc3.Commands/FileAssociationCommand.cs
using Microsoft.Win32;

namespace Wc3.Commands;

/// <summary>The outcome of a file association change, with a human readable message.</summary>
public sealed record FileAssociationResult(bool Ok, string Message);

/// <summary>
/// Adds and removes the Windows Explorer right-click entry that opens a map in the Studio. Every
/// entry lives under the current user's classes, so no admin rights are needed and nothing is
/// added to the machine-wide file association.
///
/// The registration is guarded by <see cref="OperatingSystem.IsWindows"/>, so on any other system
/// every method reports that associations are Windows only and does nothing. Only paths and the
/// registry are touched here, so this stays in Wc3.Commands with no reference to the Studio.
/// </summary>
public static class FileAssociationCommand
{
    /// <summary>The verb and label Explorer shows.</summary>
    public const string Label = "Edit in wc3ctl Studio";

    /// <summary>The map extensions the entry is registered for.</summary>
    public static readonly IReadOnlyList<string> Extensions = new[] { ".w3x", ".w3m" };

    private const string VerbName = "wc3ctlStudio";

    /// <summary>True when file associations can be changed on this system.</summary>
    public static bool IsSupported => OperatingSystem.IsWindows();

    private static string VerbKey(string extension) =>
        $@"Software\Classes\SystemFileAssociations\{extension}\shell\{VerbName}";

    /// <summary>
    /// Registers the Explorer context menu for both map extensions, pointing the command at the
    /// given Studio executable. The executable is stored as the literal path, so a moved Studio
    /// leaves a stale entry that a fresh register or an unregister repairs.
    /// </summary>
    public static FileAssociationResult Register(string studioExePath)
    {
        if (!OperatingSystem.IsWindows())
            return new FileAssociationResult(false, "File associations are Windows only.");
        if (string.IsNullOrWhiteSpace(studioExePath))
            return new FileAssociationResult(false, "No Studio executable was given.");

        // A trailing separator would make the closing quote part of the path once Windows parses
        // the command line, so trim it before quoting.
        string exe = studioExePath.TrimEnd('\\', '/');
        string command = $"\"{exe}\" \"%1\"";
        try
        {
            foreach (var extension in Extensions)
            {
                using var verb = Registry.CurrentUser.CreateSubKey(VerbKey(extension), writable: true)
                    ?? throw new InvalidOperationException($"could not create {VerbKey(extension)}");
                verb.SetValue(null, Label);
                using var cmd = verb.CreateSubKey("command", writable: true)
                    ?? throw new InvalidOperationException("could not create the command subkey");
                cmd.SetValue(null, command);
            }
            return new FileAssociationResult(true, "Added the Explorer right-click entry for .w3x and .w3m.");
        }
        catch (Exception ex)
        {
            return new FileAssociationResult(false, $"Could not write the file association, {ex.Message}");
        }
    }

    /// <summary>Removes the Explorer context menu entry for both map extensions.</summary>
    public static FileAssociationResult Unregister()
    {
        if (!OperatingSystem.IsWindows())
            return new FileAssociationResult(false, "File associations are Windows only.");
        try
        {
            foreach (var extension in Extensions)
                Registry.CurrentUser.DeleteSubKeyTree(VerbKey(extension), throwOnMissingSubKey: false);
            return new FileAssociationResult(true, "Removed the Explorer right-click entry for .w3x and .w3m.");
        }
        catch (Exception ex)
        {
            return new FileAssociationResult(false, $"Could not remove the file association, {ex.Message}");
        }
    }

    /// <summary>True when both extensions carry a registered command.</summary>
    public static bool IsRegistered()
    {
        if (!OperatingSystem.IsWindows()) return false;
        foreach (var extension in Extensions)
        {
            using var cmd = Registry.CurrentUser.OpenSubKey(VerbKey(extension) + @"\command");
            if (cmd?.GetValue(null) is not string value || value.Length == 0) return false;
        }
        return true;
    }
}
