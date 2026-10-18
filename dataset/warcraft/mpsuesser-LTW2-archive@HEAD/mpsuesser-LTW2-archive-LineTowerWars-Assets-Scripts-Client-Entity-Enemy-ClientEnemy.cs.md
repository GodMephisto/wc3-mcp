---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Enemy/ClientEnemy.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClientEnemy : ClientEntity
{
    public static ClientEnemy Create(
        int entityID,
        EnemyType type,
        int hp,
        int mp,
        Lane lane,
        Vector3 location,
        Quaternion rotation
    ) {
        ClientEnemy pfEnemy = ClientPrefabs.Enemies[type];
        int maxHP = EnemyConstants.HP[type];
        int maxMP = EnemyConstants.MP[type];
        
        ClientEnemy e = (ClientEnemy) ClientEntity.CreateEntity(
            pfEnemy,
            DynamicObjects.Singleton.Creeps,
            entityID,
            hp,
            maxHP, 
            mp,
            maxMP,
            lane,
            location,
            rotation
        );

        // Do any enemy-specific things

        return e;
    }

    public abstract EnemyType Type { get; }
    
    private Animator Anim { get; set; }
    private static readonly int AttackAnimationTrigger =
        Animator.StringToHash("TriggerAttack");
    private static readonly int BackToRunningTrigger =
        Animator.StringToHash("TriggerBackToRunning");

    protected override void Awake() {
        Anim = GetComponentInChildren<Animator>();
        
        base.Awake();
    }

    public override void HandleAttackEvent(AttackEventData eventData) {
        Anim.SetTrigger(AttackAnimationTrigger);
    }
}
