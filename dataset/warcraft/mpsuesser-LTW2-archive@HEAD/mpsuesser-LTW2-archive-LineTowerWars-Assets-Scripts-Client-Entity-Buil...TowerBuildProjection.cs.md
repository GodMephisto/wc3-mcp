---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Builder/TowerBuildProjection.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;

public class TowerBuildProjection : MonoBehaviour {
    private static Queue<TowerBuildProjection> ActiveProjections
        = new Queue<TowerBuildProjection>();
    
    public static TowerBuildProjection Create(
        TowerType type,
        Vector3 location
    ) {
        TowerBuildProjection projection = Instantiate(
            ClientPrefabs.Singleton.pfTowerBuildProjection,
            location,
            Quaternion.identity,
            DynamicObjects.Singleton.BuilderActionProjections
        );
        
        projection.LoadModelForTowerType(type);
        ActiveProjections.Enqueue(projection);

        return projection;
    }

    private void LoadModelForTowerType(TowerType type) {
        LTWLogger.Log("TODO: Load model for tower type...");
    }

    public static void ClearAll() {
        while (ActiveProjections.Count > 0) {
            Destroy(ActiveProjections.Dequeue().gameObject);
        }
    }

    public static void ClearSingle() {
        if (ActiveProjections.Count == 0) {
            return;
        }
        
        Destroy(ActiveProjections.Dequeue().gameObject);
    }
}