---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/CreepStockSyncSystem/CreepStockSyncSystem.cs"
ref: HEAD
---

﻿public class CreepStockSyncSystem : SingletonBehaviour<CreepStockSyncSystem> {
    private void Awake() {
        InitializeSingleton(this);

        EventBus.OnCreepStockUpdated += HandleUpdatedCreepStock;
    }

    private void OnDestroy() {
        EventBus.OnCreepStockUpdated -= HandleUpdatedCreepStock;
    }

    private static void HandleUpdatedCreepStock(
        EnemyType creepType,
        int stockAmount,
        float timeSinceLastIncrement
    ) {
        Lane lane = ClientLaneTracker.Singleton.MyLane;
        lane.Stock.SetStockForCreep(
            creepType,
            stockAmount,
            timeSinceLastIncrement
        );
    }
}