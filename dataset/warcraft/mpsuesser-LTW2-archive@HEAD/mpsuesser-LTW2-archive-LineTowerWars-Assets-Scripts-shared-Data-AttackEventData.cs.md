---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Data/AttackEventData.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using RiptideNetworking;

public class AttackEventData : EventData<AttackEventData> {
    public List<int> TargetEntityIDs { get; }
    public double InitialSnapshotDamage { get; }
    public DamageType DmgType { get; }

    public AttackEventData(
        List<int> targetEntityIDs,
        double initialSnapshotDamage,
        DamageType damageType
    ) {
        TargetEntityIDs = new List<int>(targetEntityIDs);
        InitialSnapshotDamage = initialSnapshotDamage;
        DmgType = damageType;
    }

    public AttackEventData(ref Message message) {
        TargetEntityIDs = new List<int>();
        int targetCount = message.GetInt();
        for (int i = 0; i < targetCount; i++) {
            TargetEntityIDs.Add(message.GetInt());
        }

        InitialSnapshotDamage = message.GetDouble();
        DmgType = (DamageType) message.GetInt();
    }

    public override void WriteDataToMessage(ref Message message) {
        message.AddInt(TargetEntityIDs.Count);
        foreach (int entityID in TargetEntityIDs) {
            message.AddInt(entityID);
        }

        message.AddDouble(InitialSnapshotDamage);
        message.AddInt((int) DmgType);
    }
}