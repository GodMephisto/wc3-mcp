---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/EventFeed/PersonalEventFeed.cs"
ref: HEAD
---

﻿public class PersonalEventFeed : EventFeed {
    protected override bool PassesFilter(EFEvent efEvent) {
        switch (efEvent) {
            case EFEvent_LivesExchanged leEvent:
                Lane lane = ClientLaneTracker.Singleton.MyLane;
                
                return
                    lane != null
                    && (
                        lane == leEvent.LosingLane
                        || lane == leEvent.GainingLane
                    );
            
            default:
                return false;
        }
    }
}