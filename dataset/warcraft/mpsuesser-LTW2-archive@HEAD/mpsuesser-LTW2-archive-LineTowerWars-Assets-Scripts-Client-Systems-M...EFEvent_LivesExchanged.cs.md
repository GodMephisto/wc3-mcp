---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/EventFeedMiddlewareSystem/EFEvent_LivesExchanged.cs"
ref: HEAD
---

﻿public class EFEvent_LivesExchanged : EFEvent {
    public Lane LosingLane { get; }
    public Lane GainingLane { get; }
    public int Amount { get; private set; }
    
    public EFEvent_LivesExchanged(
        Lane losingLane,
        Lane gainingLane,
        int amount
    ) : base(EFEventType.LivesExchanged) {
        LosingLane = losingLane;
        GainingLane = gainingLane;
        Amount = amount;
    }

    public void SetAmount(int amount) {
        Amount = amount;

        Updated();
    }
}