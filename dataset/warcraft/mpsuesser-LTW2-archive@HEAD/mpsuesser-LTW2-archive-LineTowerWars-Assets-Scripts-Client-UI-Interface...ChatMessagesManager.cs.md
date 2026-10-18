---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/ChatInterface/ChatMessagesManager.cs"
ref: HEAD
---

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(LayoutGroup))]
public class ChatMessagesManager : MonoBehaviour {
    private List<ChatMessageText> ActiveMessages{ get; set; }
    
    private RectTransform Rect { get; set; }

    private void Awake() {
        Rect = GetComponent<RectTransform>();
        
        ActiveMessages = new List<ChatMessageText>();
    }

    private void Start() {
        EventBus.OnChatMessageReceived += AddNewMessage;
    }

    private void OnDestroy() {
        EventBus.OnChatMessageReceived -= AddNewMessage;
    }

    private void AddNewMessage(ChatMessage chatMessage) {
        ChatMessageText text = ChatMessageText.Create(chatMessage, transform);
        ActiveMessages.Add(text);
        LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
    }
}