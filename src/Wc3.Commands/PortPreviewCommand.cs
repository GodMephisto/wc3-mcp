// src/Wc3.Commands/PortPreviewCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The dry run as its own command. It resolves a unit's bundle and reports exactly what porting
/// it into the target would add or change, writing nothing. Same code path as
/// <see cref="PortCommand.PortUnit"/> behind <see cref="PortCommand.PreviewPort"/>'s apply
/// switch, so the preview and the port cannot drift.
/// </summary>
public static class PortPreviewCommand
{
    public static PortResult Execute(
        MapDocument source, string rawcode, MapDocument target, string? gameDirOverride,
        bool includeScript = true, bool synthDispatch = false, bool bootstrapState = false)
    {
        var bundle = BundleCommand.ResolveUnit(source, rawcode, gameDirOverride);
        return PortCommand.PreviewPort(source, bundle, target, includeScript, synthDispatch, bootstrapState);
    }
}
