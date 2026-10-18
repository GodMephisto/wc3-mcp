---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Buffs/BuffState.cs"
ref: HEAD
---

﻿using System;
using UnityEngine;

public class BuffState {
    public event Action<BuffState> OnUpdated;
    
    public int ID { get; private set; }
    public BuffType Type { get; private set; }
    public int Stacks { get; private set; }
    public bool IsDurationBased { get; private set; }

    public float FullDuration { get; private set; }
    public float DurationRemaining => Mathf.Max(
        durationRemainingAtMessageReceived
            - (Time.time - timeOfMessageReceived)
    );
    private float timeOfMessageReceived;
    private float durationRemainingAtMessageReceived;

    public BuffState(BuffTransitData transitData) {
        LTWLogger.Log("BuffState created");
        CopyFromTransitData(transitData);
    }

    public void UpdateState(BuffTransitData transitData) {
        CopyFromTransitData(transitData);

        OnUpdated?.Invoke(this);
    }

    private void CopyFromTransitData(BuffTransitData transitData) {
        ID = transitData.ID;
        Type = transitData.Type;
        Stacks = transitData.Stacks;
        IsDurationBased = transitData.IsDurationBased;
        FullDuration = (float)transitData.FullDuration;

        timeOfMessageReceived = Time.time;
        durationRemainingAtMessageReceived = (float) transitData.RemainingDuration;
    }
}