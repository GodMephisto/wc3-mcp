---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/MainMenu/LoginInterface.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class LoginInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    public override string Title => "Login";
    
    [SerializeField] private TMP_InputField UsernameInput;
    [SerializeField] private TMP_InputField PasswordInput;
    [SerializeField] private Button SubmitButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private NotificationManager ErrorNotification;
    
    private void Awake() {
        SubmitButton.onClick.AddListener(SubmitButtonPressed);
        BackButton.onClick.AddListener(BackButtonPressed);

        OnOpened += Opened;
        OnClosed += Closed;
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Return, KeyCode.Escape,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Return => SubmitPressed(),
            KeyCode.Escape => BackPressed(),
            _ => false
        };
    }

    private void Opened(InterfaceState state) {
        UsernameInput.text = "";
        PasswordInput.text = "";
        
        EventBus.OnLoginSuccess += HandleLoginSuccess;
        EventBus.OnLoginFailed += HandleLoginFailure;
    }

    private void Closed(InterfaceState state) {
        EventBus.OnLoginSuccess -= HandleLoginSuccess;
        EventBus.OnLoginFailed -= HandleLoginFailure;
    }

    private void SubmitButtonPressed() => SubmitPressed();
    private bool SubmitPressed() {
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        
        // TODO: Show loading icon
        
        LoginSystem.Singleton.AttemptAccountLogin(username, password);
        return true;
    }

    private void BackButtonPressed() => BackPressed();
    private bool BackPressed() => Close();

    private void HandleLoginSuccess() {
        // TODO: Clear loading icon

        Close();
    }

    private void HandleLoginFailure(string failureReason) {
        // TODO: Clear loading icon
        
        DisplayErrorText("Hmm...", failureReason);
    }

    private void DisplayErrorText(string title, string text) {
        ErrorNotification.title = title;
        ErrorNotification.description = text;
        ErrorNotification.UpdateUI();
        ErrorNotification.OpenNotification();
    }
}