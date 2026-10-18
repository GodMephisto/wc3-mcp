---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/SettingsMenu/ClampedSliderNumericalInput.cs"
ref: HEAD
---

﻿using System;
using System.Globalization;
using UnityEngine;
using TMPro;
using Michsky.UI.ModernUIPack;

public class ClampedSliderNumericalInput : MonoBehaviour {
    public event Action<int> OnIntValueChanged;
    public event Action<float> OnFloatValueChanged;
    
    [SerializeField] private SliderManager slider;
    [SerializeField] private TMP_InputField inputField;

    // Disgusting, hacky, this class should be abstracted to be type-generic
    private bool _isInt = true;
    
    private void Awake() {
        slider.mainSlider.onValueChanged.AddListener(HandleSliderValueUpdated);
        inputField.onValueChanged.AddListener(HandleInputFieldValueUpdated);
    }

    public void SetFloatUsage() {
        _isInt = false;
    }

    public void SetValue(int val, bool silent = false) {
        UpdateDisplayValue(val);
        
        if (!silent) {
            OnIntValueChanged?.Invoke(val);
        }
    }

    public void SetValue(float val, bool silent = false) {
        if (_isInt) {
            SetValue((int) Math.Round(val), silent);
            return;
        }
        
        UpdateDisplayValue(val);
        
        if (!silent) {
            OnFloatValueChanged?.Invoke(val);
        }
    }

    public void SetBounds(int min, int max) {
        slider.mainSlider.minValue = min;
        slider.mainSlider.maxValue = max;
        slider.minValue = min;
        slider.maxValue = max;
        slider.UpdateUI();
    }
    
    public void SetBounds(float min, float max) {
        slider.mainSlider.minValue = min;
        slider.mainSlider.maxValue = max;
        slider.minValue = min;
        slider.maxValue = max;
        slider.UpdateUI();
    }

    private void UpdateDisplayValue(int val) {
        slider.mainSlider.value = val;
        slider.UpdateUI();
        
        inputField.text = val.ToString();
    }
    
    private void UpdateDisplayValue(float val) {
        slider.mainSlider.value = val;
        slider.UpdateUI();
        
        inputField.text = val.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private void HandleSliderValueUpdated(float val) {
        SetValue(val);
    }

    private void HandleInputFieldValueUpdated(string val) {
        if (int.TryParse(val, out int iParsedVal)) {
            SetValue(
                Mathf.Clamp(
                    iParsedVal,
                    slider.mainSlider.minValue,
                    slider.mainSlider.maxValue
                )
            );
        } else if (
            float.TryParse(
                val,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out float fParsedVal
            )
        ) {
            SetValue(
                Mathf.Clamp(
                    fParsedVal,
                    slider.mainSlider.minValue,
                    slider.mainSlider.maxValue
                )
            );
        }
    }
}