---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/EventFeed/EventFeed.cs"
ref: HEAD
---

﻿using UnityEngine;
using System.Collections.Generic;

public class EventFeed : MonoBehaviour {
    [SerializeField] private EFSlot[] Slots;

    private void Awake() {
        EventBus.OnNewEventFeedEvent += ProcessNewEvent;
    }

    private void OnDestroy() {
        EventBus.OnNewEventFeedEvent -= ProcessNewEvent;
    }

    private void ProcessNewEvent(EFEvent efEvent) {
        if (!PassesFilter(efEvent)) {
            return;
        }

        // Since in most cases it will be full, let's start from the back
        // Little more complicated here but a bit more efficient in general
        int availableSlotIndex = -1;
        for (int i = Slots.Length - 1; i >= 0; i--) {
            if (!Slots[i].IsEmpty) {
                break;
            }
            
            availableSlotIndex = i;
        }

        // If no slots are available, we need to move all events up by one to make room
        if (availableSlotIndex == -1) {
            MoveAllSlotContentsUpByOne();
            availableSlotIndex = Slots.Length - 1;
        }
        
        // A slot was either already available or we've made the last slot available by now
        Slots[availableSlotIndex].LoadEvent(efEvent);
    }

    protected virtual bool PassesFilter(EFEvent efEvent) {
        return true;
    }

    private void MoveAllSlotContentsUpByOne() {
        for (int i = 0; i < Slots.Length - 1; i++) {
            Slots[i].LoadEvent(Slots[i+1].LoadedEvent);
        }

        Slots[Slots.Length - 1].Clear();
    }
}