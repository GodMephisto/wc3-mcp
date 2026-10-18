---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/ActionTiming/Base/ActionTiming.cs"
ref: HEAD
---

using UnityEngine;

public abstract class ActionTiming : ScriptableObject {
    [SerializeField] public TowerType SourceTowerType;

    [Space(5)]
    [SerializeField] public double InitialAnimationDelay;
}