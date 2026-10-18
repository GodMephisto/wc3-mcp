---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/TowerAttackEffect/TowerAttackEffectSystem.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class TowerAttackEffectSystem : MonoBehaviour
{
    private void Start() {
        EventBus.OnEntityAttack += HandleAttackEffect;
    }

    private void OnDestroy() {
        EventBus.OnEntityAttack -= HandleAttackEffect;
    }

    private void HandleAttackEffect(ClientEntity entity, AttackEventData eventData) {
        if (!(entity is ClientTower tower)) {
            return;
        }
        
        AttackDeliveryModifiers delivery = TowerConstants.AttackDelivery[tower.Type];

        if (delivery.Type == AttackDeliveryType.Instant) {
            return;
        }

        StartCoroutine(EffectRoutine(tower, delivery, eventData));
    }

    private static IEnumerator EffectRoutine(ClientTower tower, AttackDeliveryModifiers delivery, AttackEventData attack) {
        if (delivery.InitialAnimationDelay > 0) {
            yield return new WaitForSeconds((float)delivery.InitialAnimationDelay);
        }

        if (tower == null) {
            yield break;
        }

        if (delivery is ProjectileAttackDelivery projectileDelivery) {
            CreateProjectileEffect(tower, projectileDelivery, attack);
        }
    }

    private static void CreateProjectileEffect(ClientTower tower, ProjectileAttackDelivery projectileDelivery, AttackEventData attack) {
        TowerAttackEffect effectData;
        try {
            effectData = ClientResources.Singleton.GetAttackEffectForTowerType(tower.Type);
        } catch (ResourceNotFoundException) {
            return;
        }

        if (!effectData.ProjectileEffect || effectData.ProjectileEffectPrefab == null) {
            return;
        }

        List<ClientEntity> targets = new List<ClientEntity>();
        foreach (int targetEntityID in attack.TargetEntityIDs) {
            try {
                ClientEntity target = ClientEntityStorageSystem.Singleton.GetEntityByID(targetEntityID);
                targets.Add(target);
            } catch (NotFoundException) { }
        }

        foreach (ClientEntity target in targets) {
            Vector3 projectileStartLocation =
                tower.NullableProjectileReleasePoint != null 
                    ? tower.NullableProjectileReleasePoint.position 
                    : tower.transform.position;

            projectileStartLocation += projectileDelivery.ProjectileInitialOffset;

            ProjectileEffect projectile = Instantiate(
                effectData.ProjectileEffectPrefab,
                projectileStartLocation,
                Quaternion.identity,
                DynamicObjects.Singleton.AttackEffects
            );
            projectile.Load(projectileDelivery, target.gameObject, attack);
        }
    }
}
