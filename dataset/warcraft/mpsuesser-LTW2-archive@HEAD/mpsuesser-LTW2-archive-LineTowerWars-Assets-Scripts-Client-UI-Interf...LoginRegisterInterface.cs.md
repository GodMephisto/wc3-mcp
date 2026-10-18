---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/MainMenu/LoginRegisterInterface.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRegisterInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    public override string Title => "Login / Register";
    
    [SerializeField] private Button LogInButton;
    [SerializeField] private Button CreateAccountButton;
    [SerializeField] private Button GenericLoginButton;
    [SerializeField] private Button BackButton;
    
    private void Awake() {
        LogInButton.onClick.AddListener(LogInButtonPressed);
        CreateAccountButton.onClick.AddListener(CreateAccountButtonPressed);
        GenericLoginButton.onClick.AddListener(GenericLoginButtonPressed);
        BackButton.onClick.AddListener(BackButtonPressed);

        OnOpened += Opened;
        OnClosed += Closed;
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

    private void Opened(InterfaceState state) {
        EventBus.OnLoginSuccess += HandleLoginSuccess;
    }

    private void Closed(InterfaceState state) {
        EventBus.OnLoginSuccess -= HandleLoginSuccess;
    }

    private static void LogInButtonPressed() {
        MainMenuInterfaceStateSystem.Singleton.TransitionToInterface(
            MainMenuInterfaceStateSystem.MainMenuInterfaceType.Login
        );
    }

    private static void CreateAccountButtonPressed() {
        MainMenuInterfaceStateSystem.Singleton.TransitionToInterface(
            MainMenuInterfaceStateSystem.MainMenuInterfaceType.CreateAccount
        );
    }

    private static void GenericLoginButtonPressed() {
        LoginSystem.Singleton.AttemptGenericLogin();
        
        // TODO: Show loading icon until event hook comes back
    }

    private void HandleLoginSuccess() => Close();
    private void BackButtonPressed() => BackPressed();
    private bool BackPressed() => Close();
}