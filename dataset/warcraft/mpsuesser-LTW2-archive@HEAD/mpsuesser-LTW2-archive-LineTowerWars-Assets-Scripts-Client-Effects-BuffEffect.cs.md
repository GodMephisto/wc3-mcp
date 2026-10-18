---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Effects/BuffEffect.cs"
ref: HEAD
---

﻿using UnityEngine;

[CreateAssetMenu(fileName = "BuffEffect", menuName = "ScriptableObjects/BuffEffect", order = 1)]
public class BuffEffect : ScriptableObject {
    [SerializeField] public BuffType Type;
    [SerializeField] public EntityEffect EffectPrefab;

    public EntityEffect ApplyTo(ClientEntity entity) {
        EntityEffect effect = Instantiate(EffectPrefab, entity.transform);

        return effect;
    }
}