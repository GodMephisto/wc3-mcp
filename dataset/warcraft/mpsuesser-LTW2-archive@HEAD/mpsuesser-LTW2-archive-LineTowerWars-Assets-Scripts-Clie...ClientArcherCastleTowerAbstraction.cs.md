---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/ModelHelpers/ClientArcherCastleTowerAbstraction.cs"
ref: HEAD
---

﻿public abstract class ClientArcherCastleTowerAbstraction : ClientTower
{
    private TowerWeaponFrame WeaponFrame { get; set; }

    protected override void Awake() {
        base.Awake();

        WeaponFrame = GetComponentInChildren<TowerWeaponFrame>();
    }

    public override void HandleAttackEvent(AttackEventData eventData) {
        ClientEntity target = null;
        foreach (int targetEntityID in eventData.TargetEntityIDs) {
            try {
                target = ClientEntityStorageSystem.Singleton.GetEntityByID(targetEntityID);
                break;
            }
            catch (EntityNotFoundException) { }
        }

        if (target == null) {
            return;
        }
        
        WeaponFrame.RotateTowardEntityOverTime(target, InitialAnimationDelay);
    }
}