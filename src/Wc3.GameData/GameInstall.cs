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
    /// The build an install carries, such as <c>1.30.1.10211</c> or <c>3.0.0.24268</c>, read
    /// from its <c>.build.info</c>. Every CASC-era install has that file (1.30 onwards), whereas
    /// the executable moved between eras, so this is the one place the version is always found.
    /// Null when the file is missing or names no version.
    /// </summary>
    public static string? ReadBuildVersion(string installDir)
    {
        var path = Path.Combine(installDir, ".build.info");
        if (!File.Exists(path)) path = Path.Combine(installDir, "build.info");
        if (!File.Exists(path)) return null;
        try { return ParseBuildInfoVersion(File.ReadAllText(path)); }
        catch (IOException) { return null; }
    }

    /// <summary>
    /// Reads the Version column of a <c>.build.info</c>. The first line names the columns as
    /// <c>Name!TYPE:size</c> separated by pipes, each later line is one product row. The row
    /// whose Active column is 1 wins, otherwise the first row that names a version.
    /// </summary>
    public static string? ParseBuildInfoVersion(string text)
    {
        var lines = text.Split('\n').Select(l => l.TrimEnd('\r')).Where(l => l.Length > 0).ToList();
        if (lines.Count < 2) return null;
        var header = lines[0].Split('|').Select(h => h.Split('!')[0].Trim()).ToList();
        int version = header.FindIndex(h => h.Equals("Version", StringComparison.OrdinalIgnoreCase));
        int active = header.FindIndex(h => h.Equals("Active", StringComparison.OrdinalIgnoreCase));
        if (version < 0) return null;
        string? first = null;
        foreach (var row in lines.Skip(1).Select(l => l.Split('|')))
        {
            if (row.Length <= version || row[version].Trim().Length == 0) continue;
            var v = row[version].Trim();
            if (active >= 0 && row.Length > active && row[active].Trim() == "1") return v;
            first ??= v;
        }
        return first;
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
