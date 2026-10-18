---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/SettingsMenu/SettingsInterfaceContentGameplay.cs"
ref: HEAD
---

﻿using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class SettingsInterfaceContentGameplay : SettingsInterfaceContent {
    [SerializeField] private Toggle remainOnBuildMenuAfterBuildingInput;
    
    [SerializeField] private ClampedSliderNumericalInput continuousCreepSendHotkeyActivationInput;
    private const float ActivationTimeMinBound = 0.1f;
    private const float ActivationTimeMaxBound = 2f;

    [SerializeField] private ClampedSliderNumericalInput continuousCreepSendHotkeyFrequencyInput;
    private const int SendFrequencyMinBound = 1;
    private const int SendFrequencyMaxBound = 50;
    
    protected override void LoadSettings() {
        remainOnBuildMenuAfterBuildingInput.isOn = Settings.RemainOnBuildMenuAfterBuilding.Value;
        remainOnBuildMenuAfterBuildingInput.onValueChanged.AddListener(HandleRemainOnBuildMenuValue);
        
        continuousCreepSendHotkeyActivationInput.SetFloatUsage();
        continuousCreepSendHotkeyActivationInput.SetBounds(ActivationTimeMinBound, ActivationTimeMaxBound);
        continuousCreepSendHotkeyActivationInput.SetValue(Settings.ContinuousCreepSendHotkeyActivationTime.Value, true);
        continuousCreepSendHotkeyActivationInput.OnFloatValueChanged += HandleContinuousCreepSendActivationTimeValue;

        continuousCreepSendHotkeyFrequencyInput.SetBounds(SendFrequencyMinBound, SendFrequencyMaxBound);
        continuousCreepSendHotkeyFrequencyInput.SetValue(Settings.ContinuousCreepSendHotkeyFrequency.Value, true);
        continuousCreepSendHotkeyFrequencyInput.OnIntValueChanged += HandleContinuousCreepSendFrequencyValue;
    }

    private static void HandleRemainOnBuildMenuValue(bool toggleValue) {
        Settings.RemainOnBuildMenuAfterBuilding.Save(toggleValue);
    }

    private static void HandleContinuousCreepSendFrequencyValue(int frequencyValue) {
        Settings.ContinuousCreepSendHotkeyFrequency.Save(frequencyValue);
    }

    private static void HandleContinuousCreepSendActivationTimeValue(float activationTimeValue) {
        Settings.ContinuousCreepSendHotkeyActivationTime.Save(activationTimeValue);
    }
}