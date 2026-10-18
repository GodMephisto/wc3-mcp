---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/GoldTracker/IncomeTimer.cs"
ref: HEAD
---

using System;
using UnityEngine;
using TMPro;

public class IncomeTimer : MonoBehaviour
{
    private TMP_Text IncomeText;

    private float timerValue;

    private void Awake() {
        IncomeText = GetComponent<TMP_Text>();

        ClearTimer();
        SetValue(GameConstants.IncomeInterval);
    }

    private void Update() {
        if (timerValue < 0f) {
            return;
        }

        timerValue -= Time.deltaTime;
        if (timerValue < 0f) {
            ClearTimer();
        } else {
            IncomeText.SetText(Math.Ceiling(timerValue).ToString());
        }
    }

    public void SetValue(float val) {
        timerValue = val;
    }

    private void ClearTimer() {
        timerValue = -1f;
        IncomeText.SetText("-");
    }
}
