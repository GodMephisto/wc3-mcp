---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Contents/StandardTwoBodiesTooltipContents.cs"
ref: HEAD
---

﻿using UnityEngine;
using TMPro;

public class StandardTwoBodiesTooltipContents : MonoBehaviour
{
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text CornerNote;
    [SerializeField] private TMP_Text Body;
    [SerializeField] private TMP_Text SecondBody;

    public void Load(StandardTwoBodiesTooltip tooltip) {
        Title.SetText(tooltip.Title);
        CornerNote.SetText(tooltip.CornerNote);
        Body.SetText(tooltip.Body);
        SecondBody.SetText(tooltip.SecondBody);
    }
}