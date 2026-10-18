---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/EntityBuffSystem/EntityBuffSystem.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

public class EntityBuffSystem : SingletonBehaviour<EntityBuffSystem> {
    
    private Dictionary<int, BuffState> BuffStatesByID { get; set; }
    private Dictionary<int, List<BuffState>> BuffStatesPendingEntityCreation { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        BuffStatesByID = new Dictionary<int, BuffState>();
        BuffStatesPendingEntityCreation = new Dictionary<int, List<BuffState>>();

        EventBus.OnBuffAppliedToEntity += HandleBuffAppliedToEntity;
        EventBus.OnBuffUpdated += HandleBuffUpdated;
        EventBus.OnBuffRemovedFromEntity += HandleBuffRemovedFromEntity;

        EventBus.OnTowerSpawnPost += HandleEntitySpawnPost;
        EventBus.OnEnemySpawnPost += HandleEntitySpawnPost;
    }

    private void OnDestroy() {
        EventBus.OnBuffAppliedToEntity -= HandleBuffAppliedToEntity;
        EventBus.OnBuffUpdated -= HandleBuffUpdated;
        EventBus.OnBuffRemovedFromEntity -= HandleBuffRemovedFromEntity;
        
        EventBus.OnTowerSpawnPost -= HandleEntitySpawnPost;
        EventBus.OnEnemySpawnPost -= HandleEntitySpawnPost;
    }

    private void HandleBuffAppliedToEntity(BuffTransitData buffData, int entityID) {
        BuffState bs = new BuffState(buffData);
        BuffStatesByID[bs.ID] = bs;
        
        try {
            ClientEntity entity = ClientEntityStorageSystem.Singleton.GetEntityByID(entityID);
            entity.Buffs.Add(bs);
        }
        catch (EntityNotFoundException) {
            AddBuffStateToEntityPendingCreationQueue(entityID, bs);
        }
    }

    private void AddBuffStateToEntityPendingCreationQueue(int entityID, BuffState bs) {
        if (!BuffStatesPendingEntityCreation.ContainsKey(entityID)) {
            BuffStatesPendingEntityCreation[entityID] = new List<BuffState>();
        }

        BuffStatesPendingEntityCreation[entityID].Add(bs);
    }

    private void HandleBuffUpdated(BuffTransitData buffData) {
        if (!BuffStatesByID.ContainsKey(buffData.ID)) {
            LTWLogger.Log($"EntityBuffSystem did not have a record of buff with ID {buffData.ID} to update");
            return;
        }

        BuffStatesByID[buffData.ID].UpdateState(buffData);
    }

    private void HandleBuffRemovedFromEntity(int buffID, int entityID) {
        try {
            ClientEntity entity = ClientEntityStorageSystem.Singleton.GetEntityByID(entityID);
            entity.Buffs.RemoveBuffStateByBuffID(buffID);
        }
        catch (EntityNotFoundException e) {
            LTWLogger.Log(e.Message);
        }
        catch (BuffStateNotFoundException e) {
            LTWLogger.Log(e.Message);
        }
        
        BuffStatesByID.Remove(buffID);
    }

    private void HandleEntitySpawnPost(ClientEntity entity) {
        if (!BuffStatesPendingEntityCreation.ContainsKey(entity.ID)) {
            return;
        }

        foreach (BuffState bs in BuffStatesPendingEntityCreation[entity.ID]) {
            entity.Buffs.Add(bs);
        }

        BuffStatesPendingEntityCreation.Remove(entity.ID);
    }
}