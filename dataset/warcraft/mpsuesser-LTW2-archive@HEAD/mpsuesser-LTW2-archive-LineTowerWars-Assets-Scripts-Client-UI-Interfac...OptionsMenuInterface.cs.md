---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/OptionsMenuInterface.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuInterface : InterfaceState, IHandleKeyDownInput
{
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        true;
    
    [SerializeField] private Button ReturnToGameButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button QuitButton;

    private void Awake() {
        ReturnToGameButton.onClick.AddListener(ReturnToGameButtonPressed);
        SettingsButton.onClick.AddListener(SettingsButtonPressed);
        QuitButton.onClick.AddListener(QuitButtonPressed);
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Q, KeyCode.Escape,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Q => QuitPressed(),
            KeyCode.Escape => ReturnToGamePressed(),
            _ => false
        };
    }

    private void ReturnToGameButtonPressed() => ReturnToGamePressed();
    private void SettingsButtonPressed() => SettingsPressed();
    private void QuitButtonPressed() => QuitPressed();

    private bool ReturnToGamePressed() => Close();

    private static bool SettingsPressed() {
        InGameInterfaceStateSystem.Singleton.TransitionToInterface(
            InGameInterfaceStateSystem.InGameInterfaceType.Settings
        );
        return true;
    }

    private static bool QuitPressed() {
        InGameInterfaceStateSystem.Singleton.TransitionToInterface(
            InGameInterfaceStateSystem.InGameInterfaceType.QuitConfirmation
        );
        return true;
    }

    protected override void OnShow() {
        LTWLogger.Log("Options menu show");
    }
}
