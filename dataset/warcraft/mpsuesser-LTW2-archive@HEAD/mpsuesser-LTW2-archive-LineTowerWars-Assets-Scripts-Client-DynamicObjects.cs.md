---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/DynamicObjects.cs"
ref: HEAD
---

﻿using UnityEngine;

public class DynamicObjects : SingletonBehaviour<DynamicObjects> {
    [SerializeField] public Transform Creeps;
    [SerializeField] public Transform Towers;
    [SerializeField] public Transform Builders;
    [SerializeField] public Transform BuilderActionProjections;
    [SerializeField] public Transform AttackEffects;
    [SerializeField] public Transform MiscEffects;
    [SerializeField] public Transform BountyText;
    
    [SerializeField] public Transform PooledCreeps;
    [SerializeField] public Transform PooledTowers;

    private Transform[] ParentObjects;
    
    private void Awake() {
        InitializeSingleton(this);

        ParentObjects = new[] {
            Creeps,
            Towers,
            Builders,
            BuilderActionProjections,
            AttackEffects,
            MiscEffects,
            BountyText,
            PooledCreeps,
            PooledTowers
        };

        EventBus.OnGameSceneUnloaded += CleanUpObjects;
    }

    private void CleanUpObjects() {
        foreach (Transform parent in ParentObjects) {
            foreach (Transform child in parent) {
                Destroy(child.gameObject);
            }
        }
    }
}