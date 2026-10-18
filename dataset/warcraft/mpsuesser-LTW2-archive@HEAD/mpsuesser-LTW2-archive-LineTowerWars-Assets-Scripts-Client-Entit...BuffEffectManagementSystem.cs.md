---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Systems/BuffEffectManagementSystem.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;

public class BuffEffectManagementSystem {
    private ClientEntity E { get; }
    
    private Dictionary<BuffType, EntityEffect> ActiveBuffEffectsByType { get; }
    
    public BuffEffectManagementSystem(
        ClientEntity e,
        BuffStateTrackingSystem trackingSystem
    ) {
        E = e;

        trackingSystem.OnBuffOfTypeApplied += ApplyEffectForBuffType;
        trackingSystem.OnBuffOfTypeRemoved += RemoveEffectForBuffType;

        ActiveBuffEffectsByType = new Dictionary<BuffType, EntityEffect>();
    }

    private void ApplyEffectForBuffType(BuffType buffType) {
        if (!BuffEffects.Singleton.TryGetBuffEffectByType(buffType, out BuffEffect be)) {
            return;
        }

        if (ActiveBuffEffectsByType.ContainsKey(buffType)) {
            LTWLogger.Log($"An effect for {buffType} already exists according to the BEMS - this should not happen.");
        }

        EntityEffect effect = be.ApplyTo(E);
        ActiveBuffEffectsByType[buffType] = effect;
    }

    private void RemoveEffectForBuffType(BuffType buffType) {
        if (!ActiveBuffEffectsByType.TryGetValue(buffType, out EntityEffect effect)) {
            return;
        }

        Object.Destroy(effect.gameObject);
    }
}