---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/BaseMenu/Buttons/ResearchButton.cs"
ref: HEAD
---

public class ResearchButton : ButtonWithTooltip
{
    protected override Tooltip GetTooltipContent()
    {
        return new StandardTwoBodiesTooltip(
            "Research Elemental Technology",
            ClientUtil.GetKeyCodeStringRepresentation(
                Settings.ResearchHotkey.Value
            ),
            "View a selection of elemental technologies you can purchase.",
            "By researching new elements, you open different upgrade paths from the base Elemental Core tower."
        );
    }
}