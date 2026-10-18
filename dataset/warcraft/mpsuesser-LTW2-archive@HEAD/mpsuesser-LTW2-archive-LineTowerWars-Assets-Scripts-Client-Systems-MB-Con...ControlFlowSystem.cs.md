---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/ControlFlowSystem/ControlFlowSystem.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFlowSystem : SingletonBehaviour<ControlFlowSystem>
{
    private ServerGameStateType ServerState { get; set; }
    private ClientGameStateType ClientState { get; set; }

    private void Awake() {
        InitializeSingleton(this);
        
        LTWLogger.Log("ControlFlowSystem awake!");
        
        EventBus.OnServerGameStateUpdated += UpdateServerState;

        EventBus.OnJoinLobbyPressed += InitiateConnectionToLobby;
        EventBus.OnConnectedToLobby += HandleEstablishedConnection;
        EventBus.OnLobbyOpened += TransitionToLobbyState;
        EventBus.OnLobbyReadyUpPressed += SetLobbyReadyUp;
        EventBus.OnLobbyReadyDownPressed += SetLobbyReadyDown;
        EventBus.OnStagingCompleted += SetStagingReadiness;
        EventBus.OnReturnToMainMenuPressed += ReturnToMainMenu;
        EventBus.OnDisconnectedFromLobby += HandleDisconnection;
    }

    private void OnDestroy() {
        EventBus.OnServerGameStateUpdated -= UpdateServerState;

        EventBus.OnJoinLobbyPressed -= InitiateConnectionToLobby;
        EventBus.OnConnectedToLobby -= HandleEstablishedConnection;
        EventBus.OnLobbyOpened -= TransitionToLobbyState;
        EventBus.OnLobbyReadyUpPressed -= SetLobbyReadyUp;
        EventBus.OnLobbyReadyDownPressed -= SetLobbyReadyDown;
        EventBus.OnStagingCompleted -= SetStagingReadiness;
        EventBus.OnReturnToMainMenuPressed -= ReturnToMainMenu;
        EventBus.OnDisconnectedFromLobby -= HandleDisconnection;
    }

    private static void InitiateConnectionToLobby() {
        ClientNetworkManager.Singleton.Connect();
    }

    private void HandleEstablishedConnection() {
        ClientSend.PresentTableStakes(
            LoginSystem.Singleton.ActiveUsername,
            LoginSystem.Singleton.ActivePlayfabID,
            LoginSystem.Singleton.ActiveAuthToken,
            ClientState
        );
        
        SceneSystem.Singleton.LoadLobby();
    }

    private static void HandleDisconnection() {
        SceneSystem.Singleton.CleanupFromLobby();
        SceneSystem.Singleton.LoadMainMenu();
    }

    // TODO: Maybe move these both into their own state systems
    private void UpdateServerState(ServerGameStateType newState) {
        if (ServerState == newState) {
            return;
        }

        ServerState = newState;
        switch (newState) {
            case ServerGameStateType.Lobby:
                break;

            case ServerGameStateType.Staging:
                EventBus.StagingStarted();
                UpdateClientState(ClientGameStateType.Staging);
                break;

            case ServerGameStateType.Active:
                EventBus.GameStarted();
                UpdateClientState(ClientGameStateType.InGame);
                break;

            case ServerGameStateType.Completed:
                EventBus.GameCompleted();
                UpdateClientState(ClientGameStateType.PostGame);
                break;

            default:
                break;
        }
    }

    public void UpdateClientState(ClientGameStateType newState) {
        if (ClientState == newState) {
            return;
        }

        ClientState = newState;
        switch (newState) {
            default:
                break;
        }
        
        ClientSend.UpdateClientState(newState);
    }

    private void TransitionToLobbyState() {
        UpdateClientState(ClientGameStateType.Lobby);
    }

    private void SetLobbyReadyUp() {
        LTWLogger.Log($"Going to set the lobby ready up... current state is {ClientState}");
        if (ClientState != ClientGameStateType.Lobby) {
            return;
        }

        UpdateClientState(ClientGameStateType.LobbyReady);
    }

    private void SetLobbyReadyDown() {
        LTWLogger.Log($"Going to set the lobby ready down... current state is {ClientState}");
        if (ClientState != ClientGameStateType.LobbyReady) {
            return;
        }

        UpdateClientState(ClientGameStateType.Lobby);

    }

    private void SetStagingReadiness() {
        UpdateClientState(ClientGameStateType.StagingReady);
    }

    private void ReturnToMainMenu() {
        // TODO: Update
        SceneSystem.Singleton.LoadMainMenu();

        UpdateClientState(ClientGameStateType.MainMenu);
    }
}
