---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/Static/EventBus/EventBus.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    // Server-inbound game loop events
    public static event Action<ServerGameStateType> OnServerGameStateUpdated;
    public static event Action<Dictionary<int, PlayerInfo>> OnFullPlayersSync;
    public static event Action OnPlayersInfoUpdated;

    // Server state change events
    public static event Action OnStagingStarted;
    public static event Action OnGameStarted;
    public static event Action OnGameCompleted;

    // Client-side login events
    public static event Action OnLoginSuccess;
    public static event Action<string> OnLoginFailed;
    public static event Action OnLoggedOut;
    public static event Action OnAccountCreationSuccess;
    public static event Action<string> OnAccountCreationFailed;

    // Client-side game loop events
    public static event Action OnJoinLobbyPressed;
    public static event Action OnConnectedToLobby;
    public static event Action OnLobbyOpened;
    public static event Action OnLobbyReadyUpPressed;
    public static event Action OnLobbyReadyDownPressed;
    public static event Action OnStagingCompleted;
    public static event Action OnReturnToMainMenuPressed;
    public static event Action OnDisconnectedFromLobby;
    public static event Action OnGameSceneUnloaded;
    
    // Client-side UI events
    public static event Action<InterfaceState> OnSetActiveInterfaceState;
    public static event Action OnSettingsUpdated;
    public static event Action<int, int> OnResolutionUpdated;
    public static event Action<CameraController> OnCameraMovement;
    public static event Action<Vector3[]> OnMinimapWorldCornersUpdated;
    public static event Action<string, bool> OnScreenMessageDisplayRequest;
    public static event Action<EFEvent> OnNewEventFeedEvent;
    
    // Client-side target events
    public static event Action<List<ClientEntity>> OnTargetsUpdated;
    public static event Action<ClientEntity> OnSingleTargetSelected;
    public static event Action<TowerUpgrade> OnUpgradePressed;
    public static event Action OnSellPressed;
    public static event Action OnOpenBuildMenuPressed;

    // Enemy events
    public static event Action<int, EnemyType, int, int, Lane, Vector3, Quaternion> OnEnemySpawnPre;
    public static event Action<ClientEnemy> OnEnemySpawnPost;
    public static event Action<ClientEnemy, Lane> OnEnemyLaneUpdate;
    public static event Action<ClientEnemy, Vector3, Quaternion> OnEnemyMovementSync;

    // Tower events
    public static event Action<TowerType, Lane, int, Vector3, int, int> OnTowerSpawnPre;
    public static event Action<ClientTower> OnTowerSpawnPost;
    public static event Action<ClientTower, Quaternion> OnTowerRotationSync;
    public static event Action<ClientTower> OnTowerSaleStarted;
    public static event Action<ClientTower> OnTowerSaleCanceled;
    public static event Action<ClientTower> OnTowerSaleFinished;
    public static event Action<ClientTower, TowerType, double> OnTowerUpgradeStarted;
    public static event Action<ClientTower> OnTowerUpgradeCanceled;
    public static event Action<ClientTower, int> OnTowerUpgradeFinished;

    // Builder events
    public static event Action<int, Lane, Vector3, Quaternion> OnBuilderSpawnPre;
    public static event Action<ClientBuilder> OnBuilderSpawnPost;
    public static event Action<ClientBuilder, Vector3, Quaternion, bool> OnBuilderMovementSync;
    // TODO: Attack => convert OnTowerAttack to OnEntityAttack

    // Entity events
    public static event Action<ClientEntity> OnEntityDeath;
    public static event Action<ClientEntity> OnEntityDespawn;
    public static event Action<ClientEntity, int, int> OnEntityStatusSync;
    public static event Action<ClientEntity, AttackEventData> OnEntityAttack;
    
    // Buff events
    public static event Action<BuffTransitData, int> OnBuffAppliedToEntity;
    public static event Action<BuffTransitData> OnBuffUpdated;
    public static event Action<int, int> OnBuffRemovedFromEntity;

    // Lane events
    public static event Action<Lane> OnMyLaneUpdated;
    public static event Action<Lane, int> OnLaneGoldSync;
    public static event Action<Lane, int> OnLaneIncomeSync;
    public static event Action<Lane, int> OnLaneLivesSync;
    public static event Action<Lane, int> OnLaneActiveUnitsSync;
    public static event Action<Lane, int> OnLaneTechCostSync;
    public static event Action<Lane, HashSet<ElementalTechType>> OnLaneTechsSync;
    public static event Action<Lane> OnLaneCleanedUp;
    public static event Action<HashSet<MazeGridCell>, bool> OnGridCellsOccupancyUpdated;
    public static event Action<Lane, Lane, int, EnemyType> OnLivesExchanged;
    
    // Creep stock events
    public static event Action<EnemyType, int, float> OnCreepStockUpdated;
    
    // Income events
    public static event Action<float> OnIncomeDelivery;
    
    // Chat events
    public static event Action<ChatMessage> OnChatMessageReceived;
    

    public static void ServerGameStateUpdated(ServerGameStateType newState) {
        OnServerGameStateUpdated?.Invoke(newState);
    }

    public static void GridCellsOccupancyUpdated(int laneID, HashSet<int> cellIDs, bool occupied) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];
            HashSet<MazeGridCell> cells = lane.Grid.GetCellsByID(cellIDs);

            OnGridCellsOccupancyUpdated?.Invoke(cells, occupied);
        }
        catch (IndexOutOfRangeException) {
            LTWLogger.LogError(($"Could not get lane with ID {laneID}!"));
        }
        catch (NotFoundException) {
            LTWLogger.LogError($"Could not find cell!");
        }
    }

    public static void FullPlayersSync(Dictionary<int, PlayerInfo> playersByClientID) {
        OnFullPlayersSync?.Invoke(playersByClientID);
    }

    public static void PlayersInfoUpdated() {
        OnPlayersInfoUpdated?.Invoke();
    }

    public static void SettingsUpdated() {
        OnSettingsUpdated?.Invoke();
    }

    public static void ResolutionUpdated(int x, int y) {
        OnResolutionUpdated?.Invoke(x, y);
    }

    public static void CameraMovement(CameraController cc) {
        OnCameraMovement?.Invoke(cc);
    }

    public static void MinimapWorldCornersUpdated(Vector3[] corners) {
        OnMinimapWorldCornersUpdated?.Invoke(corners);
    }

    public static void ScreenMessageDisplayRequest(string message, bool isNegativeMessage) {
        OnScreenMessageDisplayRequest?.Invoke(message, isNegativeMessage);
    }

    public static void NewEventFeedEvent(EFEvent efEvent) {
        OnNewEventFeedEvent?.Invoke(efEvent);
    }

    // Server state change events
    public static void StagingStarted() {
        OnStagingStarted?.Invoke();
    }

    public static void GameStarted() {
        OnGameStarted?.Invoke();
    }

    public static void GameCompleted() {
        OnGameCompleted?.Invoke();
    }

    // Client-side
    public static void LoginSuccessful() {
        OnLoginSuccess?.Invoke();
    }

    public static void LoginFailed(string failureReason) {
        OnLoginFailed?.Invoke(failureReason);
    }

    public static void LoggedOut() {
        OnLoggedOut?.Invoke();
    }

    public static void AccountCreationSuccessful() {
        OnAccountCreationSuccess?.Invoke();
    }

    public static void AccountCreationFailed(string failureReason) {
        OnAccountCreationFailed?.Invoke(failureReason);
    }
    
    public static void JoinLobbyPressed() {
        OnJoinLobbyPressed?.Invoke();
    }

    public static void ConnectedToLobby() {
        OnConnectedToLobby?.Invoke();
    }
    
    public static void LobbyOpened() {
        OnLobbyOpened?.Invoke();
    }

    public static void LobbyReadyUpPressed() {
        OnLobbyReadyUpPressed?.Invoke();
    }

    public static void LobbyReadyDownPressed() {
        OnLobbyReadyDownPressed?.Invoke();
    }

    public static void StagingCompleted() {
        OnStagingCompleted?.Invoke();
    }

    public static void ReturnToMainMenuPressed() {
        OnReturnToMainMenuPressed?.Invoke();
    }

    public static void DisconnectedFromLobby() {
        OnDisconnectedFromLobby?.Invoke();
    }

    public static void GameSceneUnloaded() {
        OnGameSceneUnloaded?.Invoke();
    }
    // ----

    public static void TargetsUpdated(List<ClientEntity> newTargets) {
        OnTargetsUpdated?.Invoke(newTargets);
    }

    public static void SingleTargetSelected(ClientEntity entity) {
        OnSingleTargetSelected?.Invoke(entity);
    }
    
    public static void UpgradePressed(
        TowerUpgrade upgrade
    ) {
        OnUpgradePressed?.Invoke(upgrade);
    }

    public static void SellPressed() {
        OnSellPressed?.Invoke();
    }

    public static void OpenBuildMenuPressed() {
        OnOpenBuildMenuPressed?.Invoke();
    }

    public static void EnemySpawnPre(
        int enemyEntityID,
        EnemyType type,
        int hp,
        int mp,
        int laneID,
        Vector3 spawnPosition,
        Quaternion spawnRotation
    ) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnEnemySpawnPre?.Invoke(
                enemyEntityID,
                type,
                hp,
                mp,
                lane,
                spawnPosition,
                spawnRotation
            );
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Enemy with entity ID {enemyEntityID} and type {type} was supposed be spawned in lane with ID {laneID}, but there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void EnemySpawnPost(ClientEnemy enemy) {
        OnEnemySpawnPost?.Invoke(enemy);
    }

    public static void EnemyLaneUpdate(int enemyEntityID, int newLaneID) {
        try {
            ClientEnemy enemy = ClientEntityStorageSystem.Singleton.GetEnemyByEntityID(enemyEntityID);
            Lane lane = LaneSystem.Singleton.Lanes[newLaneID];

            OnEnemyLaneUpdate?.Invoke(enemy, lane);
        }
        catch (NotFoundException) {}
        catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Enemy with entity ID {enemyEntityID} was supposed to have updated lane to lane with ID {newLaneID}, but there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void EnemyMovementSync(int enemyEntityID, Vector3 position, Quaternion rotation) {
        try {
            ClientEnemy enemy = ClientEntityStorageSystem.Singleton.GetEnemyByEntityID(enemyEntityID);

            OnEnemyMovementSync?.Invoke(enemy, position, rotation);
        } catch (NotFoundException) { }
    }

    public static void EntityStatusSync(
        int entityID,
        int hp,
        int mp
    ) {
        try {
            ClientEntity entity = ClientEntityStorageSystem.Singleton.GetEntityByID(entityID);

            OnEntityStatusSync?.Invoke(entity, hp, mp);
        } catch (NotFoundException) { }
    }

    public static void TowerSpawnPre(
        TowerType type,
        int laneID,
        int towerEntityID,
        Vector3 spawnPosition,
        int hp,
        int mp
    ) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnTowerSpawnPre?.Invoke(
                type,
                lane,
                towerEntityID,
                spawnPosition,
                hp,
                mp
            );
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Tower with entity ID {towerEntityID} and type {type} was supposed be spawned in lane with ID {laneID}, but there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void TowerSpawnPost(ClientTower tower) {
        OnTowerSpawnPost?.Invoke(tower);
    }

    public static void TowerRotationSync(int towerEntityID, Quaternion rotation) {
        try {
            ClientTower tower = ClientEntityStorageSystem.Singleton.GetTowerByEntityID(towerEntityID);

            OnTowerRotationSync?.Invoke(tower, rotation);
        } catch (NotFoundException) { }
    }

    public static void TowerSaleStarted(int towerEntityID) {
        try {
            ClientTower tower = ClientEntityStorageSystem.Singleton.GetTowerByEntityID(towerEntityID);

            OnTowerSaleStarted?.Invoke(tower);
        } catch (NotFoundException) { }
    }

    public static void TowerSaleCanceled(int towerEntityID) {
        try {
            ClientTower tower = ClientEntityStorageSystem.Singleton.GetTowerByEntityID(towerEntityID);

            OnTowerSaleCanceled?.Invoke(tower);
        } catch (NotFoundException) { }
    }

    public static void TowerSaleFinished(int towerEntityID) {
        try {
            ClientTower tower = ClientEntityStorageSystem.Singleton.GetTowerByEntityID(towerEntityID);

            OnTowerSaleFinished?.Invoke(tower);
        } catch (NotFoundException) { }
    }

    public static void TowerUpgradeStarted(int towerEntityID, TowerType targetType, double upgradeDuration) {
        try {
            ClientTower tower = ClientEntityStorageSystem.Singleton.GetTowerByEntityID(towerEntityID);

            OnTowerUpgradeStarted?.Invoke(tower, targetType, upgradeDuration);
        } catch (NotFoundException) { }
    }

    public static void TowerUpgradeCanceled(int towerEntityID) {
        try {
            ClientTower tower = ClientEntityStorageSystem.Singleton.GetTowerByEntityID(towerEntityID);

            OnTowerUpgradeCanceled?.Invoke(tower);
        } catch (NotFoundException) { }
    }

    public static void TowerUpgradeFinished(int oldTowerEntityID, int newTowerEntityID) {
        try {
            ClientTower tower = ClientEntityStorageSystem.Singleton.GetTowerByEntityID(oldTowerEntityID);

            OnTowerUpgradeFinished?.Invoke(tower, newTowerEntityID);
        } catch (NotFoundException) { }
    }

    public static void EntityAttacked(int entityID, AttackEventData eventData) {
        try {
            ClientEntity entity = ClientEntityStorageSystem.Singleton.GetEntityByID(
                entityID
            );

            OnEntityAttack?.Invoke(entity, eventData);
        } catch (NotFoundException) { }
    }

    public static void BuilderSpawnPre(
        int entityID,
        int laneID,
        Vector3 position,
        Quaternion rotation
    ) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];
            
            OnBuilderSpawnPre?.Invoke(
                entityID,
                lane,
                position,
                rotation
            );
        }
        catch (IndexOutOfRangeException) {
            LTWLogger.LogError(
                $"Builder with entity ID {entityID} was supposed to be spawned in "
                + $"lane with ID {laneID}, but there are only {LaneSystem.Singleton.Lanes.Length} lanes!"
            );
        }
    }

    public static void BuilderSpawnPost(ClientBuilder builder) {
        OnBuilderSpawnPost?.Invoke(builder);
    }

    public static void BuilderMovementSync(
        int builderEntityID,
        Vector3 position,
        Quaternion rotation,
        bool isMoving
    ) {
        try {
            ClientBuilder builder =
                ClientEntityStorageSystem.Singleton.GetBuilderByEntityID(builderEntityID);
            
            OnBuilderMovementSync?.Invoke(builder, position, rotation, isMoving);
        }
        catch (EntityNotFoundException) { }
    }

    public static void EntityDeath(int entityID) {
        try {
            ClientEntity entity = ClientEntityStorageSystem.Singleton.GetEntityByID(entityID);

            OnEntityDeath?.Invoke(entity);
        } catch (NotFoundException) { }
    }

    public static void EntityDespawn(int entityID) {
        try {
            ClientEntity entity = ClientEntityStorageSystem.Singleton.GetEntityByID(entityID);

            OnEntityDespawn?.Invoke(entity);
        } catch (NotFoundException) { }
    }

    public static void BuffAppliedToEntity(BuffTransitData buffData, int entityID) {
        OnBuffAppliedToEntity?.Invoke(buffData, entityID);
    }

    public static void BuffUpdated(BuffTransitData buffData) {
        OnBuffUpdated?.Invoke(buffData);
    }

    public static void BuffRemovedFromEntity(int buffID, int entityID) {
        OnBuffRemovedFromEntity?.Invoke(buffID, entityID);
    }

    public static void MyLaneUpdated(int laneID) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            LTWLogger.Log($"Invoking OnMyLaneUpdated... lane not null = {lane != null}");
            OnMyLaneUpdated?.Invoke(lane);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LaneGoldSync(int laneID, int gold) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnLaneGoldSync?.Invoke(lane, gold);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LaneIncomeSync(int laneID, int income) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnLaneIncomeSync?.Invoke(lane, income);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LaneLivesSync(int laneID, int lives) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnLaneLivesSync?.Invoke(lane, lives);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LaneActiveUnitsSync(int laneID, int activeUnitCount) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnLaneActiveUnitsSync?.Invoke(lane, activeUnitCount);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LaneTechCostSync(int laneID, int techCost) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnLaneTechCostSync?.Invoke(lane, techCost);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LaneTechsSync(int laneID, HashSet<ElementalTechType> techs) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnLaneTechsSync?.Invoke(lane, techs);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LaneCleanedUp(int laneID) {
        try {
            Lane lane = LaneSystem.Singleton.Lanes[laneID];

            OnLaneCleanedUp?.Invoke(lane);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID {laneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void LivesExchanged(
        int losingLaneID,
        int gainingLaneID,
        int amount,
        EnemyType enemyType
    ) {
        try {
            Lane losingLane = LaneSystem.Singleton.Lanes[losingLaneID];
            Lane gainingLane = LaneSystem.Singleton.Lanes[gainingLaneID];
            
            OnLivesExchanged?.Invoke(losingLane, gainingLane, amount, enemyType);
        } catch (IndexOutOfRangeException) {
            LTWLogger.LogError($"Lane with ID either {losingLaneID} or {gainingLaneID} was invalid... there are only {LaneSystem.Singleton.Lanes.Length} lanes!");
        }
    }

    public static void CreepStockUpdated(
        EnemyType creepType,
        int stockAmount,
        float timeSinceLastIncrement
    ) {
        OnCreepStockUpdated?.Invoke(creepType, stockAmount, timeSinceLastIncrement);
    }

    public static void IncomeDelivered(float timeToNextDelivery) {
        OnIncomeDelivery?.Invoke(timeToNextDelivery);
    }

    public static void ChatMessageReceived(ChatMessage chatMessage) {
        OnChatMessageReceived?.Invoke(chatMessage);
    }

    public static void SetActiveInterfaceState(InterfaceState state) {
        OnSetActiveInterfaceState?.Invoke(state);
    }
}
