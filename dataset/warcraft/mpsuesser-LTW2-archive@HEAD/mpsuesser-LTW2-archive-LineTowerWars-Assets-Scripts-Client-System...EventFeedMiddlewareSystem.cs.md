---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/EventFeedMiddlewareSystem/EventFeedMiddlewareSystem.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;

public class EventFeedMiddlewareSystem : SingletonBehaviour<EventFeedMiddlewareSystem> {
    
    // TODO: Replace with a setting
    private const float TimeBeforeEventBecomesStale = 5;

    // We will store all events, sorted by LastUpdatedTime, in this linked list
    // When an event is updated, we will move it to the back of the list
    // We will check the first stored event (least recently updated) each frame for staleness
    // If an event has exceeded the staleness timer, we remove it from the list
    private List<EFEvent> StoredEvents { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        StoredEvents = new List<EFEvent>();

        EventBus.OnLivesExchanged += IngestLivesExchanged;
    }

    private void OnDestroy() {
        EventBus.OnLivesExchanged += IngestLivesExchanged;
    }

    private void Update() {
        if (StoredEvents.Count == 0) {
            return;
        }

        if (StoredEvents[0].LastUpdatedTime < Time.time - TimeBeforeEventBecomesStale) {
            DeregisterStaleEvent(StoredEvents[0]);
            StoredEvents.RemoveAt(0);
        }
    }

    private void IngestLivesExchanged(
        Lane losingLane,
        Lane gainingLane,
        int amount,
        EnemyType enemyType
    ) {
        foreach (EFEvent storedEvent in StoredEvents) {
            if (
                storedEvent is EFEvent_LivesExchanged leEvent
                && leEvent.LosingLane == losingLane
                && leEvent.GainingLane == gainingLane
            ) {
                leEvent.SetAmount(leEvent.Amount + amount);
                return;
            }
        }
        
        // If there is no stored (un-stale) event, we create a new one
        EFEvent efEvent = new EFEvent_LivesExchanged(losingLane, gainingLane, amount);
        RegisterNewEvent(efEvent);
    }

    private void RegisterNewEvent(EFEvent efEvent) {
        StoredEvents.Add(efEvent);
        efEvent.OnUpdated += UpdateStaleTimeForEvent;

        EventBus.NewEventFeedEvent(efEvent);
    }

    private void DeregisterStaleEvent(EFEvent efEvent) {
        efEvent.OnUpdated -= UpdateStaleTimeForEvent;
    }

    private void UpdateStaleTimeForEvent(EFEvent efEvent) {
        if (StoredEvents.Remove(efEvent)) {
            StoredEvents.Add(efEvent);
        }
    }
}