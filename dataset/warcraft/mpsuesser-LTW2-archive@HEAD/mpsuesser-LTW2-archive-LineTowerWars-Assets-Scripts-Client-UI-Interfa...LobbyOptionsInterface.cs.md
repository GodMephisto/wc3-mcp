---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/Lobby/LobbyOptionsInterface.cs"
ref: HEAD
---

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyOptionsInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        true;
    
    [SerializeField] private Button ReturnButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button QuitButton;

    private void Awake() {
        ReturnButton.onClick.AddListener(ReturnToLobbyButtonPressed);
        SettingsButton.onClick.AddListener(SettingsPressed);
        QuitButton.onClick.AddListener(QuitPressed);
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Escape,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Escape => ReturnToLobbyPressed(),
            _ => false
        };
    }

    private void ReturnToLobbyButtonPressed() => ReturnToLobbyPressed();
    private bool ReturnToLobbyPressed() => Close();

    private void SettingsPressed() {
        LobbyInterfaceStateSystem.Singleton.TransitionToInterface(LobbyInterfaceStateSystem.LobbyInterfaceType.Settings);
    }

    private void QuitPressed() {
    }
}
