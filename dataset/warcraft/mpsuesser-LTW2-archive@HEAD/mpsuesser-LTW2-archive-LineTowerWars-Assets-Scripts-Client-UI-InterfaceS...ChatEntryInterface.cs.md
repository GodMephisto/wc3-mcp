---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/ChatEntryInterface/ChatEntryInterface.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ChatEntryInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        true;
    
    public override string Title => "Chat Entry";
    public override bool IsOverlay => true;

    [SerializeField] private TMP_InputField chatInputField;
    
    private EventSystem eventSystem;
    
    private void Awake() {
        eventSystem = EventSystem.current;
        chatInputField.characterLimit = ChatConstants.MaxCharsPerMessage;
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Return, KeyCode.Escape,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Return => EnterPressed(),
            KeyCode.Escape => EscapePressed(),
            _ => false
        };
    }

    protected override void OnShow() {
        SelectInputField();
    }

    protected override void OnHide() {
        chatInputField.text = "";
    }

    private void SelectInputField() {
        chatInputField.OnPointerClick(new PointerEventData(eventSystem));
        eventSystem.SetSelectedGameObject(chatInputField.gameObject, new BaseEventData(eventSystem));
    }

    private bool EnterPressed() {
        string rawInput = chatInputField.text;
        string trimmedInput = rawInput.Trim();
        if (trimmedInput != "") {
            ClientSend.SendChatMessage(trimmedInput);
        }

        return Close();
    }

    private bool EscapePressed() => Close();
}