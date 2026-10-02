using Microsoft.Win32;
namespace Wc3.GameData;

public static class GameInstall
{
    public static string? Locate(string? overridePath = null)
    {
        // An explicit override is authoritative: a path that doesn't exist means
        // "not found", never a silent fallback to some other detected install.
        if (!string.IsNullOrWhiteSpace(overridePath))
            return Directory.Exists(overridePath) ? overridePath : null;

        if (OperatingSystem.IsWindows())
        {
            foreach (var (hive, sub, val) in new[]
            {
                (RegistryHive.CurrentUser, @"SOFTWARE\Blizzard Entertainment\Warcraft III", "InstallPath"),
                (RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Warcraft III", "InstallLocation"),
            })
            {
                try
                {
                    using var key = RegistryKey.OpenBaseKey(hive, RegistryView.Default).OpenSubKey(sub);
                    if (key?.GetValue(val) is string p && Directory.Exists(p)) return p;
                }
                catch { /* ignore, fall through */ }
            }
        }
        foreach (var p in new[] { @"C:\Warcraft III", @"C:\Program Files (x86)\Warcraft III", @"C:\Program Files\Warcraft III" })
            if (Directory.Exists(p)) return p;
        return null;
    }

    /// <summary>
    /// Finds the Warcraft III executable under an install root (located via
    /// <see cref="Locate"/>). Probes the Reforged layout (Warcraft III.exe under
    /// _retail_/x86_64 or x86_64) first, then the classic war3.exe in the root.
    /// Returns null when the install or a known executable can't be found.
    /// </summary>
    public static string? LocateExecutable(string? overridePath = null)
    {
        if (Locate(overridePath) is not { } root)
            return null;
        foreach (var rel in new[]
        {
            Path.Combine("_retail_", "x86_64", "Warcraft III.exe"),
            Path.Combine("x86_64", "Warcraft III.exe"),
            "Warcraft III.exe",
            "war3.exe",
        })
        {
            var full = Path.Combine(root, rel);
            if (File.Exists(full)) return full;
        }
        return null;
    }
}
