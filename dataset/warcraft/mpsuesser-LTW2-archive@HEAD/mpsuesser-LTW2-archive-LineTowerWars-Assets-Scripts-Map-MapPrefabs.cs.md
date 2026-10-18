---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/MapPrefabs.cs"
ref: HEAD
---

﻿using UnityEngine;

public class MapPrefabs : SingletonBehaviour<MapPrefabs>
{
    [SerializeField] public Lane pfLane;

    private void Awake() {
        InitializeSingleton(this);
    }
}