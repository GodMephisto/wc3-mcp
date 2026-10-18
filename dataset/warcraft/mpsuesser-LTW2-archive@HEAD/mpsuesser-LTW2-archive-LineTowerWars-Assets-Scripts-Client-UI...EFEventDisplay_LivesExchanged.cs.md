---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/EventFeed/Displays/EFEventDisplay_LivesExchanged.cs"
ref: HEAD
---

﻿using TMPro;
using UnityEngine;

public class EFEventDisplay_LivesExchanged : EFEventDisplay {
    public static EFEventDisplay Create(
        EFEvent_LivesExchanged leEvent,
        Transform parent
    ) {
        EFEventDisplay_LivesExchanged eventDisplay = Instantiate(
            ClientPrefabs.Singleton.pfEventDisplayLivesExchanged,
            parent
        );

        eventDisplay.LoadDisplayFromEvent(leEvent);

        return eventDisplay;
    }

    [SerializeField] private EFEventName gainingLaneName;
    [SerializeField] private EFEventName losingLaneName;
    [SerializeField] private TMP_Text livesCountText;
    
    private EFEvent_LivesExchanged LoadedEvent { get; set; }

    private void OnDestroy() {
        LoadedEvent.OnUpdated -= ProcessLoadedEventUpdated;
    }

    private void LoadDisplayFromEvent(EFEvent_LivesExchanged leEvent) {
        LoadedEvent = leEvent;
        
        gainingLaneName.LoadNameForLane(leEvent.GainingLane);
        losingLaneName.LoadNameForLane(leEvent.LosingLane);
        SetLivesCount(leEvent.Amount);
        
        LoadedEvent.OnUpdated += ProcessLoadedEventUpdated;
    }

    private void ProcessLoadedEventUpdated(EFEvent efEvent) {
        SetLivesCount(LoadedEvent.Amount);
    }

    private void SetLivesCount(int amount) {
        livesCountText.SetText($"x{amount}");

        Lane myLane = ClientLaneTracker.Singleton.MyLane;
        if (myLane == null) {
            return;
        }

        if (myLane == LoadedEvent.LosingLane) {
            FlashNegative();
        }
        else if (myLane == LoadedEvent.GainingLane) {
            FlashPositive();
        }
    }
}