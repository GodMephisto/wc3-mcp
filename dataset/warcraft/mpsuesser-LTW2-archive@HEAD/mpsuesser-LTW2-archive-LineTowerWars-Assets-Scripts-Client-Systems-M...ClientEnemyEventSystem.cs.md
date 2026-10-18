---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/ClientEnemyEventSystem/ClientEnemyEventSystem.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientEnemyEventSystem : SingletonBehaviour<ClientEnemyEventSystem> 
{
    private void Awake() {
        InitializeSingleton(this);
        
        EventBus.OnEnemyLaneUpdate += UpdateEnemyLane;
        EventBus.OnEnemyMovementSync += SyncEnemyMovement;
        EventBus.OnEnemySpawnPre += SpawnEnemy;
        EventBus.OnEntityStatusSync += SyncEnemyStatus;
        EventBus.OnEntityDeath += DestroyEnemyAfterDeath;
        EventBus.OnEntityDespawn += DestroyEnemyAfterDespawn;
    }

    private void OnDestroy() {
        EventBus.OnEnemyLaneUpdate -= UpdateEnemyLane;
        EventBus.OnEnemyMovementSync -= SyncEnemyMovement;
        EventBus.OnEnemySpawnPre -= SpawnEnemy;
        EventBus.OnEntityStatusSync -= SyncEnemyStatus;
        EventBus.OnEntityDeath -= DestroyEnemyAfterDeath;
        EventBus.OnEntityDespawn -= DestroyEnemyAfterDespawn;
    }

    private static void UpdateEnemyLane(ClientEnemy enemy, Lane lane) {
        enemy.SetLane(lane);
    }

    private static void SyncEnemyMovement(ClientEnemy enemy, Vector3 position, Quaternion rotation) {
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
    }

    private static void SyncEnemyStatus(ClientEntity entity, int hp, int mp) {
        if (!(entity is ClientEnemy enemy)) {
            return;
        }

        enemy.SetHP(hp);
        enemy.SetMP(mp);
    }

    private void SpawnEnemy(
        int entityID,
        EnemyType type,
        int hp,
        int mp,
        Lane lane,
        Vector3 location,
        Quaternion rotation
    ) {
        ClientEnemy e = ClientEnemy.Create(
            entityID,
            type,
            hp,
            mp,
            lane,
            location,
            rotation
        );
        
        EventBus.EnemySpawnPost(e);
    }

    private static HashSet<TraitType> DeathPactTraitTypes = new HashSet<TraitType>() {
        TraitType.DeathPact1,
        TraitType.DeathPact2,
        TraitType.DeathPact3,
    };
    private static bool EnemyHasDeathPactTrait(ClientEnemy enemy) {
        HashSet<TraitType> deathPactTraitTypes = new HashSet<TraitType>(DeathPactTraitTypes);
        deathPactTraitTypes.IntersectWith(TraitConstants.EnemyTraitMap[enemy.Type]);
        return deathPactTraitTypes.Count > 0;
    }
    
    private static void DestroyEnemyAfterDeath(ClientEntity entity) {
        if (!(entity is ClientEnemy enemy)) {
            return;
        }

        // TODO: Make the death pact check more flexible to include other death pacts in future
        if (
            enemy.ActiveLane == ClientLaneTracker.Singleton.MyLane
            && EnemyConstants.KillReward[enemy.Type] > 0
            && !EnemyHasDeathPactTrait(enemy)
        ) {
            FloatingBountyText.Create(EnemyConstants.KillReward[enemy.Type], enemy.transform.position);
        }

        Destroy(enemy.gameObject);
    }

    private static void DestroyEnemyAfterDespawn(ClientEntity entity) {
        if (!(entity is ClientEnemy enemy)) {
            return;
        }

        Destroy(enemy.gameObject);
    }
}
