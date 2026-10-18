---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/Systems/EventBus/LaneEventBus.cs"
ref: HEAD
---

﻿using System;
using System.Collections.Generic;

public static class LaneEventBus {
    public static event Action<Lane> OnLaneCreated;
    public static event Action<Lane> OnLaneLivesUpdated;
    public static event Action<Lane> OnLaneIncomeUpdated;
    public static event Action<Lane> OnLaneGoldUpdated;
    public static event Action<Lane> OnLaneActiveUnitsUpdated;
    public static event Action<Lane> OnLaneTechsUpdated;
    public static event Action<Lane> OnLaneTechCostUpdated;

    public static void LaneCreated(Lane lane) {
        OnLaneCreated?.Invoke(lane);
    }
    
    public static void LaneLivesUpdated(Lane lane) {
        OnLaneLivesUpdated?.Invoke(lane);
    }

    public static void LaneIncomeUpdated(Lane lane) {
        OnLaneIncomeUpdated?.Invoke(lane);
    }

    public static void LaneGoldUpdated(Lane lane) {
        OnLaneGoldUpdated?.Invoke(lane);
    }

    public static void LaneActiveUnitsUpdated(Lane lane) {
        OnLaneActiveUnitsUpdated?.Invoke(lane);
    }

    public static void LaneTechsUpdated(Lane lane) {
        OnLaneTechsUpdated?.Invoke(lane);
    }

    public static void LaneTechCostUpdated(Lane lane) {
        OnLaneTechCostUpdated?.Invoke(lane);
    }
}