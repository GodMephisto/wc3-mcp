---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/BuilderEventSystem/BuilderEventSystem.cs"
ref: HEAD
---

﻿using UnityEngine;

public class BuilderEventSystem : SingletonBehaviour<BuilderEventSystem> 
{
    private void Awake() {
        InitializeSingleton(this);
        
        EventBus.OnBuilderMovementSync += SyncBuilderMovement;
        EventBus.OnBuilderSpawnPre += SpawnBuilder;
        EventBus.OnEntityDespawn += DestroyBuilderAfterDespawn;
        EventBus.OnTowerSpawnPre += TriggerBuilderBuildAnimation;
        EventBus.OnTowerSpawnPre += ClearSingleTowerProjection;
    }

    private void OnDestroy() {
        EventBus.OnBuilderMovementSync -= SyncBuilderMovement;
        EventBus.OnBuilderSpawnPre -= SpawnBuilder;
        EventBus.OnEntityDespawn -= DestroyBuilderAfterDespawn;
        EventBus.OnTowerSpawnPre -= TriggerBuilderBuildAnimation;
        EventBus.OnTowerSpawnPre -= ClearSingleTowerProjection;
    }
    
    private static void SyncBuilderMovement(
        ClientBuilder builder,
        Vector3 position,
        Quaternion rotation,
        bool isMoving
    ) {
        builder.transform.position = position;
        builder.transform.rotation = rotation;

        if (isMoving) {
            builder.SetMovingFlag();
        }
        else {
            builder.UnsetMovingFlag();
        }
    }

    private void SpawnBuilder(
        int entityID,
        Lane lane,
        Vector3 location,
        Quaternion rotation
    ) {
        ClientBuilder builder = ClientBuilder.Create(
            entityID,
            lane,
            location,
            rotation
        );

        EventBus.BuilderSpawnPost(builder);
    }

    private static void DestroyBuilderAfterDespawn(ClientEntity entity) {
        if (!(entity is ClientBuilder builder)) {
            return;
        }

        Destroy(builder.gameObject);
    }

    private static void TriggerBuilderBuildAnimation(
        TowerType _type,
        Lane lane,
        int _towerEntityID,
        Vector3 _towerLocation,
        int _hp,
        int _mp
    ) {
        try {
            ClientBuilder builder = ClientEntityStorageSystem.Singleton.GetBuilderByLaneID(lane.ID);
            builder.TriggerBuildAnimation();
        }
        catch (NotFoundException e) {
            LTWLogger.LogError(e.Message);
        }
    }

    private static void ClearSingleTowerProjection(
        TowerType _type,
        Lane lane,
        int _towerEntityID,
        Vector3 _towerLocation,
        int _hp,
        int _mp
    ) {
        TowerBuildProjection.ClearSingle();
    }
}
