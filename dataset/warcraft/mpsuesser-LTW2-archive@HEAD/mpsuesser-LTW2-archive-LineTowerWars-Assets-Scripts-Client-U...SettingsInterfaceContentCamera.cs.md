---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/SettingsMenu/SettingsInterfaceContentCamera.cs"
ref: HEAD
---

﻿using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;

public class SettingsInterfaceContentCamera : SettingsInterfaceContent {
    [SerializeField] private HorizontalSelector yRotationSelectorInput;

    [SerializeField] private ClampedSliderNumericalInput xRotationInput;
    private const int XRotationMinBound = 10;
    private const int XRotationMaxBound = 90;
    
    [SerializeField] private ClampedSliderNumericalInput heightInput;
    private const int HeightMinBound = 30;
    private const int HeightMaxBound = 500;
    
    [SerializeField] private ClampedSliderNumericalInput scrollSpeedInput;
    private const int ScrollSpeedMinBound = 10;
    private const int ScrollSpeedMaxBound = 400;

    [SerializeField] private ClampedSliderNumericalInput panBorderThicknessInput;
    private const int PanBorderThicknessMinBound = 1;
    private const int PanBorderThicknessMaxBound = 30;
    
    protected override void LoadSettings() {
        yRotationSelectorInput.index = Settings.CameraRotation.Value / 90;
        yRotationSelectorInput.UpdateUI();
        // Hack, weird issue where the text is off 500 units to the right at the start
        yRotationSelectorInput.ForwardClick();
        yRotationSelectorInput.PreviousClick();
        yRotationSelectorInput.onValueChanged.AddListener(HandleYRotationInputValue);
        
        xRotationInput.SetBounds(XRotationMinBound, XRotationMaxBound);
        xRotationInput.SetValue(Settings.CameraFieldOfViewAngle.Value, true);
        xRotationInput.OnIntValueChanged += HandleXRotationInputValue;
        
        heightInput.SetBounds(HeightMinBound, HeightMaxBound);
        heightInput.SetValue(Settings.CameraHeight.Value, true);
        heightInput.OnIntValueChanged += HandleHeightInputValue;
        
        scrollSpeedInput.SetBounds(ScrollSpeedMinBound, ScrollSpeedMaxBound);
        scrollSpeedInput.SetValue(Settings.CameraScrollSpeed.Value, true);
        scrollSpeedInput.OnIntValueChanged += HandleScrollSpeedInputValue;
        
        panBorderThicknessInput.SetBounds(PanBorderThicknessMinBound, PanBorderThicknessMaxBound);
        panBorderThicknessInput.SetValue(Settings.CameraPanBorderThickness.Value, true);
        panBorderThicknessInput.OnIntValueChanged += HandlePanBorderThicknessInputValue;
    }

    private static void HandleYRotationInputValue(int dropdownSelectionVal) {
        Settings.CameraRotation.Save(90 * dropdownSelectionVal);
    }

    private static void HandleXRotationInputValue(int val) {
        Settings.CameraFieldOfViewAngle.Save(val);
    }

    private static void HandleHeightInputValue(int val) {
        Settings.CameraHeight.Save(val);
    }

    private static void HandleScrollSpeedInputValue(int val) {
        Settings.CameraScrollSpeed.Save(val);
    }

    private static void HandlePanBorderThicknessInputValue(int val) {
        Settings.CameraPanBorderThickness.Save(val);
    }
}