// src/Wc3.Commands/VersionSetCommand.cs
using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>Outcome of writing the product version. Path is the file written, Version the value set.</summary>
public sealed record VersionSetResult(bool Ok, string Message, string? Path, string? Version);

/// <summary>
/// Sets the one product version in Directory.Build.props, so a release bump is a single command.
/// The version must be three dot-separated numbers. A malformed version is refused without touching
/// the file, and the file must carry exactly one Version element or the command refuses rather than
/// guess which one to change.
/// </summary>
public static partial class VersionSetCommand
{
    public const string VersionPattern = @"^\d+\.\d+\.\d+$";

    [GeneratedRegex(VersionPattern)]
    private static partial Regex VersionShape();

    [GeneratedRegex(@"<Version>([^<]*)</Version>")]
    private static partial Regex VersionElement();

    /// <summary>True when the text is exactly three dot-separated numbers, e.g. 0.2.2.</summary>
    public static bool IsValid(string? version) => version is not null && VersionShape().IsMatch(version);

    /// <summary>Write the version into the build properties file, refusing a malformed value.</summary>
    public static VersionSetResult Execute(string buildPropsPath, string version)
    {
        if (!IsValid(version))
            return new VersionSetResult(false, $"'{version}' is not a version of the form x.y.z", buildPropsPath, null);
        if (!File.Exists(buildPropsPath))
            return new VersionSetResult(false, $"no file at {buildPropsPath}", buildPropsPath, null);

        string text = File.ReadAllText(buildPropsPath);
        var matches = VersionElement().Matches(text);
        if (matches.Count != 1)
            return new VersionSetResult(false,
                $"expected exactly one Version element in {buildPropsPath}, found {matches.Count}", buildPropsPath, null);

        string updated = VersionElement().Replace(text, $"<Version>{version}</Version>", 1);
        if (string.Equals(updated, text, StringComparison.Ordinal))
            return new VersionSetResult(true, $"version is already {version}", buildPropsPath, version);

        File.WriteAllText(buildPropsPath, updated);
        return new VersionSetResult(true, $"set version {version} in {buildPropsPath}", buildPropsPath, version);
    }
}
