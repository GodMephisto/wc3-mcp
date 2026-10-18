---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/MessageDisplay/MessageDisplayer.cs"
ref: HEAD
---

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessageDisplayer : MonoBehaviour {
    [SerializeField] private RectTransform messagesParent;

    private bool needsRefreshing = false;

    // TODO: Would be nice to get rid of this and have this displayer
    // dynamically check if a message won't fit in the messagesParent
    // to determine if room needs to be made.
    private const int MaxActiveMessages = 2;

    private Queue<ScreenMessage> activeMessages;
    
    private void Awake() {
        activeMessages = new Queue<ScreenMessage>();
        
        EventBus.OnScreenMessageDisplayRequest += DisplayNewMessage;
    }

    private void OnDestroy() {
        EventBus.OnScreenMessageDisplayRequest -= DisplayNewMessage;
    }

    private void DisplayNewMessage(string message, bool isNegativeMessage) {
        ScreenMessage sm = ScreenMessage.Create(
            message,
            isNegativeMessage,
            messagesParent.transform
        );
        
        sm.OnDestroyed += HandleMessageDestroyed;
        activeMessages.Enqueue(sm);

        if (activeMessages.Count > MaxActiveMessages) {
            MakeRoomForNewMessage();
        }
        
        needsRefreshing = true;
    }

    private void MakeRoomForNewMessage() {
        // The OnDestroy handler below will dequeue the message from our queue
        Destroy(activeMessages.Peek().gameObject);
    }

    private void HandleMessageDestroyed() {
        activeMessages.Dequeue();
        
        needsRefreshing = true;
    }

    private void LateUpdate() {
        if (!needsRefreshing) {
            return;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(messagesParent);
        needsRefreshing = false;
    }
}