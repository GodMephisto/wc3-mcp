---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Networking/ClientReceive.cs"
ref: HEAD
---

using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

public static class ClientReceive {

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.GameStateUpdated)]
    private static void GameStateUpdated(Message message) {
        ServerGameStateType gameState = (ServerGameStateType) message.GetInt();
        
        EventBus.ServerGameStateUpdated(gameState);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.GridCellsOccupancyUpdated)]
    private static void GridCellsOccupancyUpdated(Message message) {
        int laneID = message.GetInt();
        int numCells = message.GetInt();
        HashSet<int> cellIDs = new HashSet<int>();
        for (int i = 0; i < numCells; i++) {
            cellIDs.Add(message.GetInt());
        }

        bool occupied = message.GetBool();

        EventBus.GridCellsOccupancyUpdated(laneID, cellIDs, occupied);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.PlayersUpdated)]
    private static void PlayersUpdated(Message message) {
        Dictionary<int, PlayerInfo> playersByClientID = new Dictionary<int, PlayerInfo>();
        
        int playerCount = message.GetInt();
        for (int i = 0; i < playerCount; i++) {
            PlayerInfo player = message.GetPlayerInfo();
            playersByClientID.Add(player.ClientID, player);
        }
        
        EventBus.FullPlayersSync(playersByClientID);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.CreepSpawned)]
    private static void CreepSpawned(Message message) {
        int entityID = message.GetInt();
        EnemyType type = (EnemyType) message.GetInt();
        int hp = message.GetInt();
        int mp = message.GetInt();
        int laneID = message.GetInt();
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();

        EventBus.EnemySpawnPre(
            entityID,
            type,
            hp,
            mp,
            laneID,
            position,
            rotation
        );
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.CreepLaneUpdated)]
    private static void CreepLaneUpdated(Message message) {
        int entityID = message.GetInt();
        int newLaneID = message.GetInt();

        EventBus.EnemyLaneUpdate(entityID, newLaneID);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.CreepPositionUpdated)]
    private static void CreepPositionUpdated(Message message) {
        int entityID = message.GetInt();
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();

        EventBus.EnemyMovementSync(entityID, position, rotation);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerCreated)]
    private static void TowerCreated(Message message) {
        int entityID = message.GetInt();
        TowerType type = (TowerType) message.GetInt();
        int laneID = message.GetInt();
        Vector3 position = message.GetVector3();
        int hp = message.GetInt();
        int mp = message.GetInt();

        EventBus.TowerSpawnPre(type, laneID, entityID, position, hp, mp);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerRotationUpdated)]
    private static void TowerRotationUpdated(Message message) {
        int entityID = message.GetInt();
        Quaternion rotation = message.GetQuaternion();
        
        EventBus.TowerRotationSync(entityID, rotation);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerUpgradeStarted)]
    private static void TowerUpgradeStarted(Message message) {
        int entityID = message.GetInt();
        TowerType targetTowerType = (TowerType) message.GetInt();
        double upgradeDuration = message.GetDouble();
        
        EventBus.TowerUpgradeStarted(entityID, targetTowerType, upgradeDuration);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerUpgradeCancelled)]
    private static void TowerUpgradeCancelled(Message message) {
        int entityID = message.GetInt();

        EventBus.TowerUpgradeCanceled(entityID);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerUpgradeFinished)]
    private static void TowerUpgradeFinished(Message message) {
        int oldEntityID = message.GetInt();
        int newEntityID = message.GetInt();

        EventBus.TowerUpgradeFinished(oldEntityID, newEntityID);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerSaleStarted)]
    private static void TowerSaleStarted(Message message) {
        int entityID = message.GetInt();

        EventBus.TowerSaleStarted(entityID);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerSaleCancelled)]
    private static void TowerSaleCancelled(Message message) {
        int entityID = message.GetInt();

        EventBus.TowerSaleCanceled(entityID);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.TowerSaleFinished)]
    private static void TowerSaleFinished(Message message) {
        int entityID = message.GetInt();

        EventBus.TowerSaleFinished(entityID);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.EntityAttacked)]
    private static void EntityAttacked(Message message) {
        int entityID = message.GetInt();
        AttackEventData eventData = new AttackEventData(ref message);
        
        EventBus.EntityAttacked(entityID, eventData);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.BuilderSpawned)]
    private static void BuilderSpawned(Message message) {
        int entityID = message.GetInt();
        int laneID = message.GetInt();
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        
        LTWLogger.Log("BS 1");
        EventBus.BuilderSpawnPre(entityID, laneID, position, rotation);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.BuilderPositionUpdated)]
    private static void BuilderPositionUpdated(Message message) {
        int entityID = message.GetInt();
        Vector3 position = message.GetVector3();
        Quaternion rotation = message.GetQuaternion();
        bool isMoving = message.GetBool();
        
        EventBus.BuilderMovementSync(entityID, position, rotation, isMoving);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.EntityDied)]
    private static void EntityDied(Message message) {
        int entityID = message.GetInt();
        
        EventBus.EntityDeath(entityID);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.EntityDespawned)]
    private static void EntityDespawned(Message message) {
        int entityID = message.GetInt();
        
        EventBus.EntityDespawn(entityID);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.EntityStatusUpdated)]
    private static void EntityStatusUpdated(Message message) {
        int entityID = message.GetInt();
        int hp = message.GetInt();
        int mp = message.GetInt();
        
        EventBus.EntityStatusSync(entityID, hp, mp);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LaneGoldUpdated)]
    private static void LaneGoldUpdated(Message message) {
        int laneID = message.GetInt();
        int gold = message.GetInt();
        
        EventBus.LaneGoldSync(laneID, gold);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LaneIncomeUpdated)]
    private static void LaneIncomeUpdated(Message message) {
        int laneID = message.GetInt();
        int income = message.GetInt();
        
        EventBus.LaneIncomeSync(laneID, income);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LaneLivesUpdated)]
    private static void LaneLivesUpdated(Message message) {
        int laneID = message.GetInt();
        int lives = message.GetInt();
        
        EventBus.LaneLivesSync(laneID, lives);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LaneActiveUnitsUpdated)]
    private static void LaneActiveUnitsUpdated(Message message) {
        int laneID = message.GetInt();
        int activeUnits = message.GetInt();
        
        EventBus.LaneActiveUnitsSync(laneID, activeUnits);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LaneTechCostUpdated)]
    private static void LaneTechCostUpdated(Message message) {
        int laneID = message.GetInt();
        int techCost = message.GetInt();
        
        EventBus.LaneTechCostSync(laneID, techCost);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LaneTechsUpdated)]
    private static void LaneTechsUpdated(Message message) {
        int laneID = message.GetInt();
        HashSet<ElementalTechType> techs = new HashSet<ElementalTechType>();
        int techsCount = message.GetInt();
        for (int i = 0; i < techsCount; i++) {
            techs.Add((ElementalTechType) message.GetInt());
        }
        
        EventBus.LaneTechsSync(laneID, techs);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LaneCleanedUp)]
    private static void LaneCleanedUp(Message message) {
        int laneID = message.GetInt();

        EventBus.LaneCleanedUp(laneID);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.IncomeDelivered)]
    private static void IncomeDelivered(Message message) {
        float timeToNextIncome = message.GetFloat();
        
        EventBus.IncomeDelivered(timeToNextIncome);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.ChatMessageReceived)]
    private static void ChatMessageReceived(Message message) {
        ChatMessage chatMessage = message.GetChatMessage();

        EventBus.ChatMessageReceived(chatMessage);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.DisplayScreenMessage)]
    private static void DisplayScreenMessage(Message message) {
        string screenMessageText = message.GetString();
        bool isNegativeMessage = message.GetBool();

        EventBus.ScreenMessageDisplayRequest(screenMessageText, isNegativeMessage);
    }

    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.BuffAppliedToEntity)]
    private static void BuffAppliedToEntity(Message message) {
        LTWLogger.Log("BuffAppliedToEntity called");
        int entityID = message.GetInt();
        BuffTransitData buffData = message.GetBuffTransitData();
        
        EventBus.BuffAppliedToEntity(buffData, entityID);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.BuffUpdated)]
    private static void BuffUpdated(Message message) {
        BuffTransitData buffData = message.GetBuffTransitData();
        
        EventBus.BuffUpdated(buffData);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.BuffRemovedFromEntity)]
    private static void BuffRemovedFromEntity(Message message) {
        int entityID = message.GetInt();
        int buffID = message.GetInt();
        
        EventBus.BuffRemovedFromEntity(buffID, entityID);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.CreepStockUpdated)]
    private static void CreepStockUpdated(Message message) {
        EnemyType creepType = (EnemyType) message.GetInt();
        int stockAmount = message.GetInt();
        float timeSinceLastIncrement = message.GetFloat();

        EventBus.CreepStockUpdated(creepType, stockAmount, timeSinceLastIncrement);
    }
    
    [MessageHandler((ushort) RiptideMessageIDs.ServerToClient.LivesExchanged)]
    private static void LivesExchanged(Message message) {
        int losingLaneID = message.GetInt();
        int gainingLaneID = message.GetInt();
        int amount = message.GetInt();
        EnemyType enemyType = (EnemyType) message.GetInt();
        
        EventBus.LivesExchanged(losingLaneID, gainingLaneID, amount, enemyType);
    }
}
