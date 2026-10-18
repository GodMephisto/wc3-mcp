---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Settings/Settings.cs"
ref: HEAD
---

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting<T> {
    public event Action<T> Updated;
    public string PrefID { get; private set; }
    public T Value { get; private set; }

    public Setting(string prefID, T defaultValue) {
        PrefID = prefID;

        if (PlayerPrefs.HasKey(PrefID)) {
            Load(defaultValue);
        } else {
            Value = defaultValue;
        }
    }

    // Passing in the default value here only because default(T) is not of type string when T is a string, for some reason
    private void Load(T defaultValue) {
        switch (defaultValue) {
            case string _:
                Value = (T)(object)PlayerPrefs.GetString(PrefID);
                break;
            case int _:
                Value = (T)(object)PlayerPrefs.GetInt(PrefID);
                break;
            case float _:
                Value = (T)(object)PlayerPrefs.GetFloat(PrefID);
                break;
            case bool _:
                Value = (T)(object)(PlayerPrefs.GetInt(PrefID) == 1);
                break;
            case KeyCode _:
                Value = (T)(object)((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(PrefID)));
                break;
            default:
                LTWLogger.Log($"Setting with pref ID {PrefID} was not of a valid PlayerPref type!");
                Value = default(T);
                break;
        }

        Updated?.Invoke(Value);
    }

    public void Save(T value) {
        Value = value;

        switch (Value) {
            case string _:
                PlayerPrefs.SetString(PrefID, (string)(object)Value);
                break;
            case int _:
                PlayerPrefs.SetInt(PrefID, (int)(object)Value);
                break;
            case float _:
                PlayerPrefs.SetFloat(PrefID, (float)(object)Value);
                break;
            case bool _:
                PlayerPrefs.SetInt(PrefID, (bool)(object)Value ? 1 : 0);
                break;
            case KeyCode kc:
                PlayerPrefs.SetString(PrefID, (string)(object)kc.ToString());
                break;
            default:
                LTWLogger.Log($"Could not save setting with ID {PrefID} because it was not of a valid PlayerPref type!");
                break;
        }

        Updated?.Invoke(Value);
    }
}

public class Settings : SingletonBehaviour<Settings> {
    // Audio
    public static Setting<float> MasterVolume { get; private set; }
    public static Setting<float> GameVolume { get; private set; }
    public static Setting<float> AmbianceVolume { get; private set; }
    
    // Camera
    public static Setting<int> CameraRotation { get; private set; }
    public static Setting<int> CameraHeight { get; private set; }
    public static Setting<int> CameraFieldOfViewAngle { get; private set; }
    public static Setting<int> CameraScrollSpeed { get; private set; }
    public static Setting<int> CameraPanBorderThickness { get; private set; }

    // In-Game UI
    public static Setting<int> ChatFontSize { get; private set; }
    
    // Gameplay
    public static Setting<bool> RemainOnBuildMenuAfterBuilding { get; private set; }
    public static Setting<float> ContinuousCreepSendHotkeyActivationTime { get; private set; }
    public static Setting<int> ContinuousCreepSendHotkeyFrequency { get; private set; }

    // Hotkeys
    public static Setting<KeyCode> SelectBuilderHotkey { get; private set; }

    public static Setting<KeyCode> BuildArcherHotkey { get; private set; }
    public static Setting<KeyCode> BuildCutterHotkey { get; private set; }
    public static Setting<KeyCode> BuildElementalCoreHotkey { get; private set; }
    public static Setting<KeyCode> BuildTechnologyDiscHotkey { get; private set; }
    
    public static Setting<KeyCode> OpenTavern1Hotkey { get; private set; }
    public static Setting<KeyCode> OpenTavern2Hotkey { get; private set; }
    public static Setting<KeyCode> OpenTavern3Hotkey { get; private set; }
    public static Setting<KeyCode> OpenTavern4Hotkey { get; private set; }
    
    public static List<Setting<KeyCode>> SendCreepHotkeys { get; private set; }
    public static Setting<KeyCode> SendCreep1Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep2Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep3Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep4Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep5Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep6Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep7Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep8Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep9Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep10Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep11Hotkey { get; private set; }
    public static Setting<KeyCode> SendCreep12Hotkey { get; private set; }
    
    public static Setting<KeyCode> CameraJumpToNextLaneHotkey { get; private set; }
    public static Setting<KeyCode> CameraJumpToPreviousLaneHotkey { get; private set; }
    public static Setting<KeyCode> CameraJumpToOwnLaneHotkey { get; private set; }
    
    public static Setting<KeyCode> ResearchHotkey { get; private set; }
    public static Setting<KeyCode> BuildHotkey { get; private set; }
    public static Setting<KeyCode> MoveHotkey { get; private set; }
    public static Setting<KeyCode> AttackHotkey { get; private set; }
    public static Setting<KeyCode> SellHotkey { get; private set; }
    public static List<Setting<KeyCode>> UpgradeHotkeys { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey1 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey2 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey3 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey4 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey5 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey6 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey7 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey8 { get; private set; }
    public static Setting<KeyCode> UpgradeHotkey9 { get; private set; }
    
    // Resolution
    private int ResX { get; set; }
    private int ResY { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        ResX = Screen.width;
        ResY = Screen.height;

        LoadAllSettings();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void LoadAllSettings() {
        MasterVolume = new Setting<float>("masterVolume", .75f);
        GameVolume = new Setting<float>("gameVolume", .50f);
        AmbianceVolume = new Setting<float>("ambianceVolume", .75f);

        CameraRotation = new Setting<int>("cameraRotation", 90);
        CameraHeight = new Setting<int>("cameraHeight", 70);
        CameraFieldOfViewAngle = new Setting<int>("cameraFieldOfViewAngle", 60);
        CameraScrollSpeed = new Setting<int>("cameraScrollSpeed", 190);
        CameraPanBorderThickness = new Setting<int>("cameraPanBorderThickness", 10);
        
        ChatFontSize = new Setting<int>("chatFontSize", 20);

        RemainOnBuildMenuAfterBuilding = new Setting<bool>("remainOnBuildMenuAfterBuilding", false);
        ContinuousCreepSendHotkeyActivationTime = new Setting<float>("continuousCreepSendHotkeyActivationTime", 0.5f);
        ContinuousCreepSendHotkeyFrequency = new Setting<int>("continuousCreepSendHotkeyFrequency", 15);
        
        SelectBuilderHotkey = new Setting<KeyCode>("selectBuilderHotkey", KeyCode.Alpha1);
        
        BuildArcherHotkey = new Setting<KeyCode>("buildArcherHotkey", KeyCode.Q);
        BuildCutterHotkey = new Setting<KeyCode>("buildCutterHotkey", KeyCode.W);
        BuildElementalCoreHotkey = new Setting<KeyCode>("buildElementalCoreHotkey", KeyCode.E);
        BuildTechnologyDiscHotkey = new Setting<KeyCode>("buildTechnologyDiscHotkey", KeyCode.S);
        
        OpenTavern1Hotkey = new Setting<KeyCode>("openTavern1Hotkey", KeyCode.Alpha2);
        OpenTavern2Hotkey = new Setting<KeyCode>("openTavern2Hotkey", KeyCode.Alpha3);
        OpenTavern3Hotkey = new Setting<KeyCode>("openTavern3Hotkey", KeyCode.Alpha4);
        OpenTavern4Hotkey = new Setting<KeyCode>("openTavern4Hotkey", KeyCode.Alpha5);
        
        SendCreep1Hotkey = new Setting<KeyCode>("sendCreep1Hotkey", KeyCode.Q);
        SendCreep2Hotkey = new Setting<KeyCode>("sendCreep2Hotkey", KeyCode.W);
        SendCreep3Hotkey = new Setting<KeyCode>("sendCreep3Hotkey", KeyCode.E);
        SendCreep4Hotkey = new Setting<KeyCode>("sendCreep4Hotkey", KeyCode.R);
        SendCreep5Hotkey = new Setting<KeyCode>("sendCreep5Hotkey", KeyCode.A);
        SendCreep6Hotkey = new Setting<KeyCode>("sendCreep6Hotkey", KeyCode.S);
        SendCreep7Hotkey = new Setting<KeyCode>("sendCreep7Hotkey", KeyCode.D);
        SendCreep8Hotkey = new Setting<KeyCode>("sendCreep8Hotkey", KeyCode.F);
        SendCreep9Hotkey = new Setting<KeyCode>("sendCreep9Hotkey", KeyCode.Z);
        SendCreep10Hotkey = new Setting<KeyCode>("sendCreep10Hotkey", KeyCode.X);
        SendCreep11Hotkey = new Setting<KeyCode>("sendCreep11Hotkey", KeyCode.C);
        SendCreep12Hotkey = new Setting<KeyCode>("sendCreep12Hotkey", KeyCode.V);
        SendCreepHotkeys = new List<Setting<KeyCode>>() {
            SendCreep1Hotkey,
            SendCreep2Hotkey,
            SendCreep3Hotkey,
            SendCreep4Hotkey,
            SendCreep5Hotkey,
            SendCreep6Hotkey,
            SendCreep7Hotkey,
            SendCreep8Hotkey,
            SendCreep9Hotkey,
            SendCreep10Hotkey,
            SendCreep11Hotkey,
            SendCreep12Hotkey,
        };

        CameraJumpToNextLaneHotkey = new Setting<KeyCode>("cameraJumpToNextLaneHotkey", KeyCode.F1);
        CameraJumpToPreviousLaneHotkey = new Setting<KeyCode>("cameraJumpToPreviousLaneHotkey", KeyCode.F2);
        CameraJumpToOwnLaneHotkey = new Setting<KeyCode>("cameraJumpToOwnLaneHotkey", KeyCode.F3);
        
        ResearchHotkey = new Setting<KeyCode>("researchHotkey", KeyCode.T);
        BuildHotkey = new Setting<KeyCode>("buildHotkey", KeyCode.B);
        MoveHotkey = new Setting<KeyCode>("moveHotkey", KeyCode.Mouse1);
        AttackHotkey = new Setting<KeyCode>("attackHotkey", KeyCode.A);
        SellHotkey = new Setting<KeyCode>("sellHotkey", KeyCode.S);
        
        UpgradeHotkey1 = new Setting<KeyCode>("upgradeHotkey1", KeyCode.Q);
        UpgradeHotkey2 = new Setting<KeyCode>("upgradeHotkey2", KeyCode.W);
        UpgradeHotkey3 = new Setting<KeyCode>("upgradeHotkey3", KeyCode.E);
        UpgradeHotkey4 = new Setting<KeyCode>("upgradeHotkey4", KeyCode.R);
        UpgradeHotkey5 = new Setting<KeyCode>("upgradeHotkey5", KeyCode.Z);
        UpgradeHotkey6 = new Setting<KeyCode>("upgradeHotkey6", KeyCode.X);
        UpgradeHotkey7 = new Setting<KeyCode>("upgradeHotkey7", KeyCode.C);
        UpgradeHotkey8 = new Setting<KeyCode>("upgradeHotkey8", KeyCode.V);
        UpgradeHotkey9 = new Setting<KeyCode>("upgradeHotkey9", KeyCode.B);
        UpgradeHotkeys = new List<Setting<KeyCode>>() {
            UpgradeHotkey1,
            UpgradeHotkey2,
            UpgradeHotkey3,
            UpgradeHotkey4,
            UpgradeHotkey5,
            UpgradeHotkey6,
            UpgradeHotkey7,
            UpgradeHotkey8,
            UpgradeHotkey9,
        };

    }

    private void LateUpdate() {
        if (ResX == Screen.width && ResY == Screen.height) {
            return;
        }

        StartCoroutine(UpdateResolution());
    }

    private IEnumerator UpdateResolution() {
        yield return null;

        ResX = Screen.width;
        ResY = Screen.height;

        EventBus.ResolutionUpdated(ResX, ResY);
    }
}
