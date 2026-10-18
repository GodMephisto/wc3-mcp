---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/ClientEntity.cs"
ref: HEAD
---

using System;
using UnityEngine;

public abstract class ClientEntity : MonoBehaviour
{
    protected static ClientEntity CreateEntity(
        ClientEntity pfEntity,
        Transform parent,
        int entityID,
        int hp,
        int maxHP,
        int mp,
        int maxMP,
        Lane lane,
        Vector3 location,
        Quaternion rotation
    ) {
        ClientEntity e = Instantiate(
            pfEntity,
            location,
            rotation,
            parent
        );
        
        e.SetID(entityID);
        e.SetLane(lane);
        e.SetHP(hp);
        e.SetMaxHP(maxHP);
        e.SetMP(mp);
        e.SetMaxMP(maxMP);
        e.SetupComplete();
        
        return e;
    }

    public event Action<ClientEntity> OnSetupComplete;
    public event Action<ClientEntity> OnStatusUpdated;
    public event Action<ClientEntity> OnDestroyed;
    
    public int ID { get; private set; }
    public Lane ActiveLane { get; private set; }

    public int MaxHP { get; private set; }
    public int MaxMP { get; private set; }
    public int HP { get; private set; }
    public int MP { get; private set; }
    
    // Systems
    public BuffStateTrackingSystem Buffs { get; private set; }
    private BuffEffectManagementSystem BuffEffects { get; set; }

    protected virtual void Awake() {
        Buffs = new BuffStateTrackingSystem(this);
        BuffEffects = new BuffEffectManagementSystem(this, Buffs);
    }

    public void SetLane(Lane lane) {
        ActiveLane = lane;
    }

    public void SetHP(int hp) {
        HP = hp;
        OnStatusUpdated?.Invoke(this);
    }

    public void SetMP(int mp) {
        MP = mp;
        OnStatusUpdated?.Invoke(this);
    }

    private void SetID(int entityID) {
        ID = entityID;
    }

    private void SetMaxHP(int maxHP) {
        MaxHP = maxHP;
    }

    private void SetMaxMP(int maxMP) {
        MaxMP = maxMP;
    }

    private void SetupComplete() {
        OnSetupComplete?.Invoke(this);
    }

    protected virtual void OnDestroy() {
        OnDestroyed?.Invoke(this);
    }

    public virtual void HandleAttackEvent(AttackEventData eventData) { }
}
