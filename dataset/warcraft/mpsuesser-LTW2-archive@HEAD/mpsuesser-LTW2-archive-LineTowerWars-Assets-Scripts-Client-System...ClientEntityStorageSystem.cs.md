---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/ClientEntityStorageSystem/ClientEntityStorageSystem.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;

public class ClientEntityStorageSystem : SingletonBehaviour<ClientEntityStorageSystem> {
    private Dictionary<int, ClientEnemy> EnemiesByID { get; set; }
    private Dictionary<int, ClientTower> TowersByID { get; set; }
    private Dictionary<int, ClientBuilder> BuildersByID { get; set; }
    private Dictionary<int, HashSet<ClientTower>> TowersByLaneID { get; set; }
    private Dictionary<int, HashSet<ClientEnemy>> CreepsByLaneID { get; set; }
    private Dictionary<int, ClientBuilder> BuilderByLaneID { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        EnemiesByID = new Dictionary<int, ClientEnemy>();
        TowersByID = new Dictionary<int, ClientTower>();
        BuildersByID = new Dictionary<int, ClientBuilder>();
        TowersByLaneID = new Dictionary<int, HashSet<ClientTower>>();
        CreepsByLaneID = new Dictionary<int, HashSet<ClientEnemy>>();
        BuilderByLaneID = new Dictionary<int, ClientBuilder>();

        EventBus.OnTowerSpawnPost += RegisterTower;
        EventBus.OnEnemySpawnPost += RegisterEnemy;
        EventBus.OnBuilderSpawnPost += RegisterBuilder;
        EventBus.OnEnemyLaneUpdate += UpdateEnemyLane;
    }

    private void OnDestroy() {
        EventBus.OnTowerSpawnPost -= RegisterTower;
        EventBus.OnEnemySpawnPost -= RegisterEnemy;
        EventBus.OnBuilderSpawnPost -= RegisterBuilder;
        EventBus.OnEnemyLaneUpdate -= UpdateEnemyLane;
    }

    private void RegisterEnemy(ClientEnemy e) {
        EnemiesByID.Add(e.ID, e);
        
        int laneID = e.ActiveLane.ID;
        if (!CreepsByLaneID.ContainsKey(laneID)) {
            CreepsByLaneID[laneID] = new HashSet<ClientEnemy>();
        }
        CreepsByLaneID[laneID].Add(e);

        RegisterEntity(e);
    }

    private void RegisterTower(ClientTower t) {
        TowersByID.Add(t.ID, t);

        int laneID = t.ActiveLane.ID;
        if (!TowersByLaneID.ContainsKey(laneID)) {
            TowersByLaneID[laneID] = new HashSet<ClientTower>();
        }
        TowersByLaneID[laneID].Add(t);

        RegisterEntity(t);
    }

    private void RegisterBuilder(ClientBuilder builder) {
        BuildersByID.Add(builder.ID, builder);

        int laneID = builder.ActiveLane.ID;
        if (BuilderByLaneID.ContainsKey(laneID)) {
            LTWLogger.LogError($"More than one builder has been registered in lane {laneID}!");
        }

        BuilderByLaneID[laneID] = builder;

        RegisterEntity(builder);
    }

    private void RegisterEntity(ClientEntity e) {
        e.OnDestroyed += OnEntityDestroyed;
    }

    private void UpdateEnemyLane(ClientEnemy e, Lane lane) {
        foreach (int laneID in CreepsByLaneID.Keys) {
            if (CreepsByLaneID[laneID].Contains(e)) {
                CreepsByLaneID[laneID].Remove(e);
            }
        }

        if (!CreepsByLaneID.ContainsKey(lane.ID)) {
            CreepsByLaneID[lane.ID] = new HashSet<ClientEnemy>();
        }

        CreepsByLaneID[lane.ID].Add(e);
    }

    private void OnEntityDestroyed(ClientEntity e) {
        switch (e) {
            case ClientTower t:
                TowersByID.Remove(t.ID);
                TowersByLaneID[t.ActiveLane.ID].Remove(t);
                break;
            case ClientEnemy c:
                EnemiesByID.Remove(c.ID);
                CreepsByLaneID[c.ActiveLane.ID].Remove(c);
                break;
            case ClientBuilder builder:
                BuildersByID.Remove(builder.ID);
                BuilderByLaneID.Remove(builder.ActiveLane.ID);
                break;
        }
    }

    public ClientEntity GetEntityByID(int entityID) {
        // TODO: Just keep an "EntitiesByID" dict
        if (EnemiesByID.ContainsKey(entityID)) {
            return EnemiesByID[entityID];
        } else if (TowersByID.ContainsKey(entityID)) {
            return TowersByID[entityID];
        } else if (BuildersByID.ContainsKey(entityID)) {
            return BuildersByID[entityID];
        }

        throw new EntityNotFoundException(entityID);
    }

    public ClientEnemy GetEnemyByEntityID(int entityID) {
        if (!EnemiesByID.ContainsKey(entityID)) {
            throw new EntityNotFoundException(entityID);
        }

        return EnemiesByID[entityID];
    }

    public ClientTower GetTowerByEntityID(int entityID) {
        if (!TowersByID.ContainsKey(entityID)) {
            throw new EntityNotFoundException(entityID);
        }

        return TowersByID[entityID];
    }

    public ClientBuilder GetBuilderByEntityID(int entityID) {
        if (!BuildersByID.ContainsKey(entityID)) {
            throw new EntityNotFoundException(entityID);
        }

        return BuildersByID[entityID];
    }

    public HashSet<ClientTower> GetTowersOfTypeByLaneID(TowerType towerType, int laneID) {
        HashSet<ClientTower> towers = new HashSet<ClientTower>();

        if (!TowersByLaneID.ContainsKey(laneID)) {
            throw new NotFoundException($"There were no towers to be searched in lane ID {laneID}");
        }

        foreach (ClientTower tower in TowersByLaneID[laneID]) {
            if (tower.Type == towerType) {
                towers.Add(tower);
            }
        }

        return towers;
    }

    public HashSet<ClientEnemy> GetCreepsOfTypeByLaneID(EnemyType creepType, int laneID) {
        HashSet<ClientEnemy> creeps = new HashSet<ClientEnemy>();
        
        if (!CreepsByLaneID.ContainsKey(laneID)) {
            throw new NotFoundException($"There were no creeps to be searched in lane ID {laneID}");
        }

        foreach (ClientEnemy creep in CreepsByLaneID[laneID]) {
            if (creep.Type == creepType) {
                creeps.Add(creep);
            }
        }

        return creeps;
    }

    public ClientBuilder GetBuilderByLaneID(int laneID) {
        if (!BuilderByLaneID.ContainsKey(laneID)) {
            throw new NotFoundException($"There was no active builder in lane {laneID}");
        }

        return BuilderByLaneID[laneID];
    }
}
