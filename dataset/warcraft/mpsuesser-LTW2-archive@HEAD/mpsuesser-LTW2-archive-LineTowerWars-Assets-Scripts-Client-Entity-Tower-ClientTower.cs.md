---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Tower/ClientTower.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClientTower : ClientEntity
{
    public static ClientTower Create(
        TowerType type,
        Lane lane,
        int entityID,
        Vector3 location,
        int hp,
        int mp
    ) {
        ClientTower pfTower = ClientPrefabs.Towers[type];
        int maxHP = TowerConstants.HP[type];
        int maxMP = TowerConstants.MP[type];
        
        ClientTower t = (ClientTower) ClientEntity.CreateEntity(
            pfTower,
            DynamicObjects.Singleton.Towers,
            entityID,
            hp,
            maxHP,
            mp,
            maxMP,
            lane,
            location,
            Quaternion.Euler(0, Settings.CameraRotation.Value, 0)
        );

        // Do any tower-specific things

        return t;
    }

    // From 0 to 1
    protected float PointInAttackAnimationOfAttack = 1;

    [SerializeField] public Transform NullableProjectileReleasePoint;
    [SerializeField] private bool hasSpawnAnimation;
    [SerializeField] private bool hasSellAnimation;
    
    public abstract TowerType Type { get; }
    protected float InitialAnimationDelay => (float) TowerConstants.AttackDelivery[Type].InitialAnimationDelay;

    private Animator Anim { get; set; }
    private static readonly int AttackAnimationTrigger = Animator.StringToHash("StartAttack");

    protected override void Awake() {
        Anim = GetComponentInChildren<Animator>();

        if (Anim != null) {
            SetSpawnAnimationField();
            SetAttackAnimationSpeedMultiplier();
        }

        base.Awake();

        Settings.CameraRotation.Updated += CameraRotationChanged;
    }

    protected override void OnDestroy() {
        Settings.CameraRotation.Updated -= CameraRotationChanged;

        base.OnDestroy();
    }

    private void CameraRotationChanged(int rotation) {
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void SetSpawnAnimationField() {
        Anim.SetBool("HasSpawnAnimation", hasSpawnAnimation);
    }

    private void SetAttackAnimationSpeedMultiplier() {
        AnimationClip[] clips = Anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            // TODO: Hacky, consider rework
            if (
                !clip.name.ContainsCaseInsensitive("Shoot")
                && !clip.name.ContainsCaseInsensitive("Attack")
            ) {
                continue;
            }
            
            float speedMultiplier;
            if (InitialAnimationDelay == 0) {
                // TODO: Don't love this, would be nice to implement a better looping system
                speedMultiplier = (float) (clip.length / TowerConstants.Cooldown[Type]);
            }
            else {
                // Should get our length equal to initialAnimationDelay
                speedMultiplier =
                    clip.length
                    / (InitialAnimationDelay / PointInAttackAnimationOfAttack);
            }
            
            Anim.SetFloat("AttackAnimationSpeedMultiplier", speedMultiplier);
        }
    }

    public override void HandleAttackEvent(AttackEventData eventData) {
        if (Anim == null) {
            return;
        }
        
        Anim.SetTrigger(AttackAnimationTrigger);
    }

    public void TriggerSellAnimation() {
        if (Anim == null) {
            return;
        }
        
        // TODO: Create some effect
    }

    public void TriggerUpgradeAnimation(TowerType upgradingTo) {
        if (Anim == null) {
            return;
        }
        
        // TODO: Create some effect
    }
}
