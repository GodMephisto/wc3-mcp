---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/EventFeedMiddlewareSystem/EFEvent.cs"
ref: HEAD
---

﻿using System;
using UnityEngine;

public class EFEvent {
    public event Action<EFEvent> OnUpdated;
    
    public EFEventType Type { get; }
    public float LastUpdatedTime { get; set; }
    
    public EFEvent(EFEventType type) {
        Type = type;
        LastUpdatedTime = Time.time;
    }

    protected void Updated() {
        LastUpdatedTime = Time.time;
        
        OnUpdated?.Invoke(this);
    }
}