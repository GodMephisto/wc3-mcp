// src/Wc3.Commands/PlayerSetColorCommand.cs
using System.Text.RegularExpressions;
using War3Net.Build.Info;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Sets one player's team color.
///
/// war3map.w3i carries no per-player color at any format version. The World Editor player
/// record holds only id, controller, race, fixed-start flag, name, start position and the four
/// priority masks, and this codebase has no war3mapSkin.w3i companion either. The color a map
/// actually shows is applied by the script at map init, the
/// <c>SetPlayerColor( Player(n), ConvertPlayerColor(c) )</c> call the World Editor emits in
/// InitCustomPlayerSlots. This command rewrites the color id in that one call, in war3map.j or
/// war3map.lua, and leaves every other script byte untouched.
///
/// The first matching call for the player is the one edited, which is the lobby-init call in a
/// World-Editor-generated script. A later gameplay "change color" action is deliberately left
/// alone. When the script has no such call the command refuses rather than inventing one.</summary>
public static class PlayerSetColorCommand
{
    public static PlayerForceEditResult Execute(MapDocument doc, int playerId, string color)
    {
        if (!PlayerColors.TryParseId(color, out var colorId))
            return new(false,
                $"unknown color '{color}', expected one of {string.Join(", ", PlayerColors.ColorNames)}");

        if (doc.GetFile(MapInfoCommand.FileName)?.Model is not MapInfo info)
            return new(false, $"map has no parseable {MapInfoCommand.FileName}");
        if (PlayerForceCommand.FindPlayer(info, playerId) is null)
            return new(false, $"map has no player with id {playerId}");

        string scriptFile;
        string source;
        try
        {
            (scriptFile, source) = ScriptCommand.Read(doc);
        }
        catch (FileNotFoundException e)
        {
            return new(false, e.Message);
        }

        var pattern = new Regex(
            $@"(SetPlayerColor\s*\(\s*Player\s*\(\s*{playerId}\s*\)\s*,\s*ConvertPlayerColor\s*\(\s*)\d+(\s*\))",
            RegexOptions.CultureInvariant);
        if (!pattern.IsMatch(source))
            return new(false,
                $"{scriptFile} has no SetPlayerColor call for player {playerId} to edit");

        var edited = pattern.Replace(
            source, m => m.Groups[1].Value + colorId.ToString() + m.Groups[2].Value, 1);
        ScriptCommand.Write(doc, edited);
        return new(true,
            $"player {playerId} color set to {color.ToLowerInvariant()} (id {colorId}) in {scriptFile}");
    }
}
