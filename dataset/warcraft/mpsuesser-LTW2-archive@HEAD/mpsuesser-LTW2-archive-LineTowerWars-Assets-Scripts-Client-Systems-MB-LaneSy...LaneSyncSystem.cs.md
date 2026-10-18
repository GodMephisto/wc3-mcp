---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/LaneSyncSystem/LaneSyncSystem.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

public class LaneSyncSystem : SingletonBehaviour<LaneSyncSystem> {
    private void Awake() {
        InitializeSingleton(this);
    }

    private void Start() {
        EventBus.OnLaneGoldSync += SyncGold;
        EventBus.OnLaneLivesSync += SyncLives;
        EventBus.OnLaneIncomeSync += SyncIncome;
        EventBus.OnLaneActiveUnitsSync += SyncActiveUnits;
        EventBus.OnLaneTechCostSync += SyncTechCost;
        EventBus.OnLaneTechsSync += SyncTechs;
    }
    
    private void OnDestroy() {
        EventBus.OnLaneGoldSync -= SyncGold;
        EventBus.OnLaneLivesSync -= SyncLives;
        EventBus.OnLaneIncomeSync -= SyncIncome;
        EventBus.OnLaneActiveUnitsSync -= SyncActiveUnits;
        EventBus.OnLaneTechCostSync -= SyncTechCost;
        EventBus.OnLaneTechsSync -= SyncTechs;
    }

    private static void SyncGold(Lane lane, int gold) {
        lane.SetGold(gold);
    }

    private static void SyncLives(Lane lane, int lives) {
        lane.SetLives(lives);
    }

    private static void SyncIncome(Lane lane, int income) {
        lane.SetIncome(income);
    }

    private static void SyncActiveUnits(Lane lane, int activeUnits) {
        lane.SetActiveUnits(activeUnits);
    }

    private static void SyncTechCost(Lane lane, int techCost) {
        lane.SetTechCost(techCost);
    }

    private static void SyncTechs(Lane lane, HashSet<ElementalTechType> techs) {
        lane.SetTechs(techs);
    }
}