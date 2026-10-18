---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/TowerAttackEffect/TowerAttackEffect.cs"
ref: HEAD
---

using UnityEngine;

[CreateAssetMenu(fileName = "TowerName", menuName = "ScriptableObjects/TowerAttackEffect", order = 1)]
public class TowerAttackEffect : ScriptableObject {
    [SerializeField] public TowerType SourceTowerType;

    [Space(5)]
    [SerializeField] public bool ProjectileEffect;
    [SerializeField] public bool EffectOnSourceAtLaunch;
    [SerializeField] public bool EffectOnTargetAtImpact;
    // ... can be expanded for many different specific kinds of effects, to be handled in the effect handler

    [Space(5)]
    [SerializeField] public ProjectileEffect ProjectileEffectPrefab;
}