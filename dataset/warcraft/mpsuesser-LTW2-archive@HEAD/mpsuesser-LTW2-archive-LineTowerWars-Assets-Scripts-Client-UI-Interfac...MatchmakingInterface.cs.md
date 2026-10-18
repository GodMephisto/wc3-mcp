---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/MainMenu/MatchmakingInterface.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    public override string Title => "Matchmaking";
    
    [SerializeField] private Button BackButton;
    
    private void Awake() {
        BackButton.onClick.AddListener(BackButtonPressed);
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Escape,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Escape => BackPressed(),
            _ => false
        };
    }

    private void BackButtonPressed() => BackPressed();
    private bool BackPressed() => Close();
}