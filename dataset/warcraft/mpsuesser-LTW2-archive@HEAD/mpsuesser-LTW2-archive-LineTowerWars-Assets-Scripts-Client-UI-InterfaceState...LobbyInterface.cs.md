---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/Lobby/LobbyInterface.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    [SerializeField] private Button ReadyDownButton;
    [SerializeField] private Button ReadyUpButton;
    [SerializeField] private Button SettingsMenuButton;
    [SerializeField] private Button LeaveButton;

    private void Awake() {
        ReadyUpButton.onClick.AddListener(ReadyUpButtonPressed);
        ReadyDownButton.onClick.AddListener(ReadyDownButtonPressed);
        SettingsMenuButton.onClick.AddListener(SettingsButtonPressed);
        LeaveButton.onClick.AddListener(LeaveButtonPressed);

        EventBus.OnPlayersInfoUpdated += UpdateReadyButton;
    }

    private void OnDestroy() {
        EventBus.OnPlayersInfoUpdated -= UpdateReadyButton;
    }

    private void Start() {
        EventBus.LobbyOpened();
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Escape, KeyCode.Return,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Escape => LeavePressed(),
            KeyCode.Return => EnterPressed(),
            _ => false
        };
    }

    private void UpdateReadyButton() {
        PlayerInfo myPlayer;
        try {
            myPlayer = LobbySystem.Singleton.GetPlayerByClientID(
                ClientNetworkManager.Singleton.Client.Id
            );
        }
        catch (NotFoundException e) {
            LTWLogger.LogError($"Error when trying to update the ready button on the lobby screen: {e.Message}");
            return;
        }

        bool isReady = myPlayer.State == ClientGameStateType.LobbyReady;
        ReadyUpButton.gameObject.SetActive(!isReady);
        ReadyDownButton.gameObject.SetActive(isReady);
    }

    private static void ReadyUpButtonPressed() {
        EventBus.LobbyReadyUpPressed();
    }

    private static void ReadyDownButtonPressed() {
        EventBus.LobbyReadyDownPressed();
    }

    private static void SettingsButtonPressed() {
        LobbyInterfaceStateSystem.Singleton.TransitionToInterface(
            LobbyInterfaceStateSystem.LobbyInterfaceType.Settings
        );
    }

    private static void LeaveButtonPressed() => LeavePressed();
    private static bool LeavePressed() {
        ClientNetworkManager.Singleton.Disconnect();
        return true;
    }

    private static bool EnterPressed() {
        LobbyInterfaceStateSystem.Singleton.TransitionToInterface(
            LobbyInterfaceStateSystem.LobbyInterfaceType.ChatEntry
        );
        return true;
    }
}
