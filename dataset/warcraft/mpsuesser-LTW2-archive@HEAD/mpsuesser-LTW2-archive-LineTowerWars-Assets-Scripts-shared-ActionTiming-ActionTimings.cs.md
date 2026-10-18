---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/ActionTiming/ActionTimings.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTimings : SingletonBehaviour<ActionTimings> {
    private Dictionary<TowerType, ActionTiming> TowerActionTimings { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        LoadTowerActionTimings();
    }

    private void LoadTowerActionTimings() {
        TowerActionTimings = new Dictionary<TowerType, ActionTiming>();
        ActionTiming[] timings = Resources.LoadAll<ActionTiming>("shared/ActionTimings");
        foreach (ActionTiming timing in timings) {
            TowerActionTimings[timing.SourceTowerType] = timing;
        }
    }

    public ActionTiming GetActionTimingForTowerType(TowerType towerType) {
        if (!TowerActionTimings.ContainsKey(towerType)) {
            throw new ResourceNotFoundException();
        }

        return TowerActionTimings[towerType];
    }
}
