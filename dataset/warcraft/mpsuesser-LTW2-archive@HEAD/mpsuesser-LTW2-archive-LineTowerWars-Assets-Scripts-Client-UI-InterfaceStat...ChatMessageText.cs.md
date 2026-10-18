---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/ChatInterface/ChatMessageText.cs"
ref: HEAD
---

﻿using System;
using UnityEngine;
using TMPro;

public class ChatMessageText : MonoBehaviour {
    public static ChatMessageText Create(ChatMessage message, Transform parent) {
        ChatMessageText text = Instantiate(ClientPrefabs.Singleton.pfChatMessageText, parent);
        text.SetMessage(message);
        return text;
    }

    public ChatMessage ActiveMessage { get; private set; }
    public TextMeshProUGUI ActiveMessageText { get; private set; }
    
    private const int MinFontSize = 4;
    private const int MaxFontSize = 60;

    private void Awake() {
        ActiveMessage = null;
        ActiveMessageText = GetComponentInChildren<TextMeshProUGUI>(true);

        UpdateFontSize(Settings.ChatFontSize.Value);
        Settings.ChatFontSize.Updated += UpdateFontSize;
    }

    private void OnDestroy() {
        Settings.ChatFontSize.Updated -= UpdateFontSize;
    }

    private void UpdateFontSize(int fontSize) {
        ActiveMessageText.fontSize = Mathf.Clamp(fontSize, MinFontSize, MaxFontSize);
    }

    private void SetMessage(ChatMessage message) {
        ActiveMessage = message;

        Color laneColor;
        try {
            laneColor = ClientResources.Singleton.GetColorForLane(message.Sender.Slot);
        }
        catch (IndexOutOfRangeException) {
            laneColor = Color.white;
        }
        
        ActiveMessageText.SetText($"{ActiveMessage.Sender.Username.AddColor(laneColor)}: {ActiveMessage.Content}");
    }
}