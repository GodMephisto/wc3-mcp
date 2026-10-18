---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/CreepStock/CreepStock.cs"
ref: HEAD
---

﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CreepStock {
    private Dictionary<EnemyType, int> CurrentStock { get; }
    private Dictionary<EnemyType, float> MostRecentIncrementTime { get; }

    public const int InitialDelayNotYetFinished = -1;
    
    public CreepStock() {
        CurrentStock = new Dictionary<EnemyType, int>();
        MostRecentIncrementTime = new Dictionary<EnemyType, float>();

        foreach (EnemyType type in EnemyConstants.FullyImplementedCreeps) {
            if (
                EnemyConstants.InitialStockDelay[type] == 0
                || EnemyConstants.IgnoreInitialDelayForTesting
            ) {
                CurrentStock[type] = EnemyConstants.InitialStockAmountAfterDelay[type];
            }
            else {
                CurrentStock[type] = InitialDelayNotYetFinished;
            }
            
            MostRecentIncrementTime[type] = Time.time;
        }
    }

    public void SetStockForCreep(EnemyType creepType, int stock, bool isIncrement) {
        CurrentStock[creepType] = stock;

        if (isIncrement) {
            MostRecentIncrementTime[creepType] = Time.time;
        }
    }

    public void SetStockForCreep(
        EnemyType creepType,
        int stock,
        float timeSinceLastIncrement
    ) {
        CurrentStock[creepType] = stock;
        MostRecentIncrementTime[creepType] = Time.time - timeSinceLastIncrement;
    }

    public int GetStockForCreep(EnemyType creepType) {
        return CurrentStock[creepType];
    }

    public float GetMostRecentIncrementTimeForCreep(EnemyType creepType) {
        return MostRecentIncrementTime[creepType];
    }
}