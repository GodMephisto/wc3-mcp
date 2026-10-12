// src/Wc3.Commands/ForceRenameCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Renames one force (team) in war3map.w3i, the World Editor's Scenario, Forces,
/// "Force Properties" name. Only the target force's name changes, and an unknown force index
/// is refused without touching the file.</summary>
public static class ForceRenameCommand
{
    public static PlayerForceEditResult Execute(MapDocument doc, int forceIndex, string name)
        => PlayerForceCommand.Edit(doc, info =>
        {
            if (PlayerForceCommand.CheckForceIndex(info, forceIndex) is { } failure)
                return failure;
            if (name is null)
                return new(false, "a force name is required");
            info.Forces![forceIndex].Name = name;
            return new(true, $"force {forceIndex} renamed to \"{name}\"");
        });
}
