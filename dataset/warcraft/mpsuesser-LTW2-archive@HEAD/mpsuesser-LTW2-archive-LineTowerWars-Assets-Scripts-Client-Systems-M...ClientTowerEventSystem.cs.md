---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/ClientTowerEventSystem/ClientTowerEventSystem.cs"
ref: HEAD
---

using UnityEngine;

public class ClientTowerEventSystem : MonoBehaviour
{
    private void Start() {
        EventBus.OnTowerSpawnPre += SpawnTower;
        EventBus.OnTowerRotationSync += SyncTowerRotation;

        EventBus.OnTowerSaleStarted += HandleTowerSaleStarted;
        EventBus.OnTowerUpgradeStarted += HandleTowerUpgradeStarted;

        EventBus.OnEntityStatusSync += SyncTowerStatus;
        
        EventBus.OnEntityDeath += DestroyTowerAfterDeath;
        EventBus.OnEntityDespawn += DestroyTowerAfterDespawn;
        EventBus.OnTowerSaleFinished += DestroyTowerAfterSale;
        EventBus.OnTowerUpgradeFinished += DestroyTowerAfterUpgrade;
    }

    private void OnDestroy() {
        EventBus.OnTowerSpawnPre -= SpawnTower;
        EventBus.OnTowerRotationSync -= SyncTowerRotation;

        EventBus.OnTowerSaleStarted -= HandleTowerSaleStarted;
        EventBus.OnTowerUpgradeStarted -= HandleTowerUpgradeStarted;

        EventBus.OnEntityStatusSync -= SyncTowerStatus;
        
        EventBus.OnEntityDeath -= DestroyTowerAfterDeath;
        EventBus.OnEntityDespawn -= DestroyTowerAfterDespawn;
        EventBus.OnTowerSaleFinished -= DestroyTowerAfterSale;
        EventBus.OnTowerUpgradeFinished -= DestroyTowerAfterUpgrade;
    }

    private void SpawnTower(
        TowerType type,
        Lane lane,
        int entityID,
        Vector3 location,
        int hp,
        int mp
    ) {
        location.y = 1;
        ClientTower t = ClientTower.Create(
            type,
            lane,
            entityID,
            location,
            hp,
            mp
        );

        EventBus.TowerSpawnPost(t);
    }

    private static void SyncTowerRotation(ClientTower tower, Quaternion rotation) {
        tower.transform.rotation = rotation;
    }

    private static void SyncTowerStatus(ClientEntity entity, int hp, int mp) {
        if (!(entity is ClientTower tower)) {
            return;
        }

        tower.SetHP(hp);
        tower.SetMP(mp);
    }

    private static void HandleTowerSaleStarted(ClientTower t) {
        t.TriggerSellAnimation();
    }

    private static void HandleTowerUpgradeStarted(ClientTower t, TowerType upgradingTo, double time) {
        t.TriggerUpgradeAnimation(upgradingTo);
    }

    private static void DestroyTowerAfterDeath(ClientEntity entity) {
        if (!(entity is ClientTower tower)) {
            return;
        }

        Destroy(tower.gameObject, 0.1f);
    }
    
    private static void DestroyTowerAfterDespawn(ClientEntity entity) {
        if (!(entity is ClientTower tower)) {
            return;
        }

        Destroy(tower.gameObject, 0.1f);
    }

    private static void DestroyTowerAfterSale(ClientTower t) {
        Destroy(t.gameObject, 0.1f);
    }

    private static void DestroyTowerAfterUpgrade(ClientTower oldTower, int newTowerEntityID) {
        Destroy(oldTower.gameObject, 0.1f);
    }
}
