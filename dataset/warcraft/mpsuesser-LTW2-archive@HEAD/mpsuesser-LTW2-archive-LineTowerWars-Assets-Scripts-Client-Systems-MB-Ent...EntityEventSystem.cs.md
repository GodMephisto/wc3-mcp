---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/EntityEventSystem/EntityEventSystem.cs"
ref: HEAD
---

﻿using UnityEngine;

public class EntityEventSystem : MonoBehaviour
{
    private void Start() {
        EventBus.OnEntityAttack += HandleEntityAttack;
    }

    private void OnDestroy() {
        EventBus.OnEntityAttack -= HandleEntityAttack;
    }
    
    private static void HandleEntityAttack(
        ClientEntity entity,
        AttackEventData eventData
    ) {
        entity.HandleAttackEvent(eventData);
    }
}
