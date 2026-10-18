---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/ChatInterface/ChatWindow.cs"
ref: HEAD
---

﻿using UnityEngine;

public class ChatWindow : MonoBehaviour {
    [SerializeField] private ChatEntryInterface entryInterface;
    [SerializeField] private ChatMessagesWindow messagesWindow;
    
    private void Awake() {
        entryInterface.OnOpened += HandleEntryInterfaceOpened;
        entryInterface.OnClosed += HandleEntryInterfaceClosed;
    }

    private void HandleEntryInterfaceOpened(InterfaceState entryState) {
        messagesWindow.SetMessageEntryOpen();
    }

    private void HandleEntryInterfaceClosed(InterfaceState entryState) {
        messagesWindow.SetMessageEntryClosed();
    }
}