---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/MainMenu/CreateAccountInterface.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class CreateAccountInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    public override string Title => "Account Creation";
    
    [SerializeField] private TMP_InputField UsernameInput;
    [SerializeField] private TMP_InputField EmailInput;
    [SerializeField] private TMP_InputField PasswordInput;
    [SerializeField] private TMP_InputField PasswordConfirmationInput;
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
        EmailInput.text = "";
        PasswordInput.text = "";
        PasswordConfirmationInput.text = "";
        
        EventBus.OnAccountCreationSuccess += HandleAccountCreationSuccess;
        EventBus.OnAccountCreationFailed += HandleAccountCreationFailure;
    }

    private void Closed(InterfaceState state) {
        EventBus.OnAccountCreationSuccess -= HandleAccountCreationSuccess;
        EventBus.OnAccountCreationFailed -= HandleAccountCreationFailure;
    }

    private void HandleAccountCreationSuccess() {
        // TODO: Clear loading icon

        Close();
    }

    private void HandleAccountCreationFailure(string failureReason) {
        // TODO: Clear loading icon
        
        DisplayErrorText("Hmm...", failureReason);
    }

    private void SubmitButtonPressed() => SubmitPressed();
    private bool SubmitPressed() {
        string username = UsernameInput.text;
        string email = EmailInput.text;
        string password = PasswordInput.text;
        string passwordConfirmation = PasswordConfirmationInput.text;

        if (password != passwordConfirmation) {
            DisplayErrorText("Hmm...","The given passwords do not match.");
            return true;
        }
        
        // TODO: Show loading icon
        
        LoginSystem.Singleton.AttemptAccountCreation(username, email, password);
        return true;
    }

    private void DisplayErrorText(string title, string text) {
        ErrorNotification.title = title;
        ErrorNotification.description = text;
        ErrorNotification.UpdateUI();
        ErrorNotification.OpenNotification();
    }

    private void BackButtonPressed() => BackPressed();
    private bool BackPressed() => Close();
}