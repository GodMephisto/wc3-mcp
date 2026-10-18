---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Contents/StandardTooltipContents.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

public class StandardTooltipContents : MonoBehaviour
{
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Subtitle;
    [SerializeField] private TMP_Text CornerNote;
    [SerializeField] private TMP_Text Body;
    [SerializeField] private TMP_Text Footer;

    public void Load(StandardTooltip tooltip) {
        Title.SetText(tooltip.Title);
        Subtitle.SetText(tooltip.Subtitle);
        CornerNote.SetText(tooltip.CornerNote);
        Body.SetText(tooltip.Body);
        Footer.SetText(tooltip.Footer);
    }
}
