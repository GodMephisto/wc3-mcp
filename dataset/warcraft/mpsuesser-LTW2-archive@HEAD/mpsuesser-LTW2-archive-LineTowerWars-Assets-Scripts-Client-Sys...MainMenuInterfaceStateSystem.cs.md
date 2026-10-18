---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InterfaceStateSystem/MainMenuInterfaceStateSystem.cs"
ref: HEAD
---

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuInterfaceStateSystem : InterfaceStateSystem<MainMenuInterfaceStateSystem> {
    [SerializeField] private MainMenuInterface mainMenuBaseInterface;
    [SerializeField] private LoginRegisterInterface loginRegisterInterface;
    [SerializeField] private LoginInterface loginInterface;
    [SerializeField] private CreateAccountInterface createAccountInterface;
    [SerializeField] private MatchmakingInterface matchmakingInterface;
    [SerializeField] private CreateLobbyInterface createLobbyInterface;
    [SerializeField] private ViewLobbiesInterface viewLobbiesInterface;
    [SerializeField] private SettingsInterface settingsInterface;

    public enum MainMenuInterfaceType {
        BaseMenu = 0,
        LoginRegister,
        Login,
        CreateAccount,
        Matchmaking,
        CreateLobby,
        ViewLobbies,
        Settings,
    }

    private Dictionary<MainMenuInterfaceType, InterfaceState> Interfaces { get; set; }

    protected override void Awake() {
        InitializeSingleton(this);

        Interfaces = new Dictionary<MainMenuInterfaceType, InterfaceState>() {
            { MainMenuInterfaceType.BaseMenu, mainMenuBaseInterface },
            { MainMenuInterfaceType.LoginRegister, loginRegisterInterface },
            { MainMenuInterfaceType.Login, loginInterface },
            { MainMenuInterfaceType.CreateAccount, createAccountInterface },
            { MainMenuInterfaceType.Matchmaking, matchmakingInterface },
            { MainMenuInterfaceType.CreateLobby, createLobbyInterface },
            { MainMenuInterfaceType.ViewLobbies, viewLobbiesInterface },
            { MainMenuInterfaceType.Settings, settingsInterface },
        };
        
        base.Awake();
    }

    protected override void TransitionToBaseInterface() {
        TransitionToInterface(Interfaces[MainMenuInterfaceType.BaseMenu]);
    }

    public void TransitionToInterface(MainMenuInterfaceType mainMenuInterfaceType) {
        base.TransitionToInterface(Interfaces[mainMenuInterfaceType]);
    }

    public void TransitionToInterfaceWithData(MainMenuInterfaceType mainMenuInterfaceType, InterfaceTransitionData transitionData) {
        base.TransitionToInterfaceWithData(Interfaces[mainMenuInterfaceType], transitionData);
    }
}
