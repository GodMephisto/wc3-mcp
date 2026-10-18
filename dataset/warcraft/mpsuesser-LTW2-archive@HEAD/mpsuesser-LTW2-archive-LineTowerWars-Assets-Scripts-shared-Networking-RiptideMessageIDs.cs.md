---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Networking/RiptideMessageIDs.cs"
ref: HEAD
---

﻿public static class RiptideMessageIDs {
    public enum ServerToClient : ushort {
        GameStateUpdated,
        PlayersUpdated,
        CreepSpawned,
        CreepLaneUpdated,
        CreepPositionUpdated,
        TowerCreated,
        TowerRotationUpdated,
        TowerUpgradeStarted,
        TowerUpgradeCancelled,
        TowerUpgradeFinished,
        TowerSaleStarted,
        TowerSaleFinished,
        TowerSaleCancelled,
        EntityAttacked,
        BuilderSpawned,
        BuilderPositionUpdated,
        EntityDied,
        EntityDespawned,
        EntityStatusUpdated,
        LaneGoldUpdated,
        LaneIncomeUpdated,
        LaneLivesUpdated,
        LaneActiveUnitsUpdated,
        LaneTechCostUpdated,
        LaneTechsUpdated,
        LaneCleanedUp,
        IncomeDelivered,
        GridCellsOccupancyUpdated,
        ChatMessageReceived,
        DisplayScreenMessage,
        BuffAppliedToEntity,
        BuffUpdated,
        BuffRemovedFromEntity,
        CreepStockUpdated,
        LivesExchanged,
    }

    public enum ClientToServer : ushort {
        PresentTableStakes,
        SendChatMessage,
        RequestNewLobbySlot,
        UpdateClientState,
        RequestBuildTower,
        RequestSendCreep,
        RequestPurchaseElementTech,
        RequestTowerUpgrade,
        RequestTowerUpgradeCancellation,
        RequestTowerSale,
        RequestTowerSaleCancellation,
        RequestEntityMove,
        RequestEntityAttackTarget,
        RequestEntityAttackLocation,
    }
}