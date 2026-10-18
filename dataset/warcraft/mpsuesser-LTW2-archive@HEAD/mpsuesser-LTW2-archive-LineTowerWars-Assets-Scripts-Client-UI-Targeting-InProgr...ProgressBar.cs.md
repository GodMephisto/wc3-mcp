---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/InProgress/ProgressBar.cs"
ref: HEAD
---

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image Progress;
    [SerializeField] private TMP_Text ProgressText;

    public void UpdateProgress(double currentValue, double maxValue) {
        Progress.fillAmount = (float)(currentValue / maxValue);

        if (ProgressText != null) {
            ProgressText.SetText($"{String.Format("{0:0.0}", currentValue)} / {String.Format("{0:0.0}", maxValue)}");
        }
    }

    public void UpdateProgress(int currentValue, int maxValue) {
        Progress.fillAmount = (float)currentValue / (float)maxValue;
        
        if (ProgressText != null) {
            ProgressText.SetText($"{currentValue} / {maxValue}");
        }
    }

    public void UpdateColorByFillAmount() {
        Progress.color = ClientUtil.GetHealthBarColor(Progress.fillAmount);
    }
}
