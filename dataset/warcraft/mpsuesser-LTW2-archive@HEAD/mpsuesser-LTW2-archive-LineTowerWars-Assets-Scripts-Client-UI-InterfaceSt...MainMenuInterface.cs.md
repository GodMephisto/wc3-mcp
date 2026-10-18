---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/MainMenu/MainMenuInterface.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuInterface : InterfaceState {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    public override string Title => "Line Tower Wars 2";
    
    [SerializeField] private Button LogInRegisterButton;
    [SerializeField] private Button LogOutButton;
    [SerializeField] private Button JoinLobbyButton;
    [SerializeField] private Button CreateLobbyButton;
    [SerializeField] private Button ViewLobbiesButton;
    [SerializeField] private Button MatchmakingButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button QuitButton;
    
    private void Awake() {
        LogInRegisterButton.onClick.AddListener(LogInRegisterPressed);
        LogOutButton.onClick.AddListener(LogOutPressed);
        JoinLobbyButton.onClick.AddListener(JoinLobbyPressed);
        CreateLobbyButton.onClick.AddListener(CreateLobbyPressed);
        ViewLobbiesButton.onClick.AddListener(ViewLobbiesPressed);
        MatchmakingButton.onClick.AddListener(MatchmakingPressed);
        SettingsButton.onClick.AddListener(SettingsPressed);
        QuitButton.onClick.AddListener(QuitPressed);
    }

    private void Start() {
        EventBus.OnLoginSuccess += ReflectLoggedInStatus;
        EventBus.OnLoggedOut += ReflectLoggedOutStatus;

        if (LoginSystem.Singleton.IsLoggedIn) {
            ReflectLoggedInStatus();
        } else {
            ReflectLoggedOutStatus();
        }
    }

    private void OnDestroy() {
        EventBus.OnLoginSuccess -= ReflectLoggedInStatus;
        EventBus.OnLoggedOut -= ReflectLoggedOutStatus;
    }

    private void ReflectLoggedInStatus() {
        LogInRegisterButton.gameObject.SetActive(false);
        LogOutButton.gameObject.SetActive(true);
        
        JoinLobbyButton.interactable = true;
        CreateLobbyButton.interactable = true;
        ViewLobbiesButton.interactable = true;
        MatchmakingButton.interactable = true;
    }

    private void ReflectLoggedOutStatus() {
        LogInRegisterButton.gameObject.SetActive(true);
        LogOutButton.gameObject.SetActive(false);
        
        JoinLobbyButton.interactable = false;
        CreateLobbyButton.interactable = false;
        ViewLobbiesButton.interactable = false;
        MatchmakingButton.interactable = false;
    }

    private static void LogInRegisterPressed() {
        MainMenuInterfaceStateSystem.Singleton.TransitionToInterface(
            MainMenuInterfaceStateSystem.MainMenuInterfaceType.LoginRegister
        );
    }

    private static void LogOutPressed() {
        LoginSystem.Singleton.AttemptLogout();
    }

    private static void JoinLobbyPressed() {
        LTWLogger.Log("Join lobby pressed!");
        EventBus.JoinLobbyPressed();
    }

    private static void CreateLobbyPressed() {
        MainMenuInterfaceStateSystem.Singleton.TransitionToInterface(
            MainMenuInterfaceStateSystem.MainMenuInterfaceType.CreateLobby
        );
    }

    private static void ViewLobbiesPressed() {
        MainMenuInterfaceStateSystem.Singleton.TransitionToInterface(
            MainMenuInterfaceStateSystem.MainMenuInterfaceType.ViewLobbies
        );
    }
    
    private static void MatchmakingPressed() {
        MainMenuInterfaceStateSystem.Singleton.TransitionToInterface(
            MainMenuInterfaceStateSystem.MainMenuInterfaceType.Matchmaking
        );
    }
    
    private static void SettingsPressed() {
        MainMenuInterfaceStateSystem.Singleton.TransitionToInterface(
            MainMenuInterfaceStateSystem.MainMenuInterfaceType.Settings
        );
    }
    
    private static void QuitPressed() {
        Application.Quit();
    }
}