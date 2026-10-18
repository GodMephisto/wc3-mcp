---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InterfaceStateSystem/LobbyInterfaceStateSystem.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;

public class LobbyInterfaceStateSystem : InterfaceStateSystem<LobbyInterfaceStateSystem> {
    [SerializeField] private LobbyInterface baseLobbyInterface;
    [SerializeField] private SettingsInterface settingsInterface;
    [SerializeField] private ChatEntryInterface chatEntryInterface;

    public enum LobbyInterfaceType {
        BaseLobby,
        Settings,
        ChatEntry,
    }

    private Dictionary<LobbyInterfaceType, InterfaceState> Interfaces { get; set; }

    protected override void Awake() {
        InitializeSingleton(this);

        Interfaces = new Dictionary<LobbyInterfaceType, InterfaceState>() {
            { LobbyInterfaceType.BaseLobby, baseLobbyInterface },
            { LobbyInterfaceType.Settings, settingsInterface },
            { LobbyInterfaceType.ChatEntry, chatEntryInterface },
        };
        
        base.Awake();
    }

    protected override void TransitionToBaseInterface() {
        TransitionToInterface(Interfaces[LobbyInterfaceType.BaseLobby]);
    }

    public void TransitionToInterface(LobbyInterfaceType lobbyInterfaceType) {
        base.TransitionToInterface(Interfaces[lobbyInterfaceType]);
    }

    public void TransitionToInterfaceWithData(LobbyInterfaceType lobbyInterfaceType, InterfaceTransitionData transitionData) {
        base.TransitionToInterfaceWithData(Interfaces[lobbyInterfaceType], transitionData);
    }

    private void OpenChatEntryInterface() {
        TransitionToInterface(LobbyInterfaceType.ChatEntry);
    }
}
