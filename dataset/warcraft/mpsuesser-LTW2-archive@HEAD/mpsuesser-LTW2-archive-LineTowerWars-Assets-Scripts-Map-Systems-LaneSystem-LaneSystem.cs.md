---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/Systems/LaneSystem/LaneSystem.cs"
ref: HEAD
---

﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class LaneSystem : SingletonBehaviour<LaneSystem> {
    [SerializeField] private Transform lanesParent;

    public int MaxLaneCount { get; private set; }
    public Lane[] Lanes { get; private set; }
    public Dictionary<Lane, int> LaneIDByLane { get; private set; }

    private const float HorizontalDistanceBetweenLanes = 400f;
    private const float VerticalDistanceBetweenLanes = 600f;
    private const int LanesPerRow = 3;

    public readonly int MaxActiveUnits = 100;

    private void Awake() {
        InitializeSingleton(this);

        Lanes = new Lane[] { };
    }

    public void InitializeLanes(int numLanes) {
        LTWLogger.Log($"Generating {numLanes} lanes...");
        MaxLaneCount = numLanes;
        Lanes = new Lane[MaxLaneCount];

        GenerateLanes();
    }

    private void GenerateLanes() {
        LaneIDByLane = new Dictionary<Lane, int>();
        for (int i = 0; i < MaxLaneCount; i++) {
            Vector3 location = new Vector3(HorizontalDistanceBetweenLanes * (i % LanesPerRow), 0f, -VerticalDistanceBetweenLanes * (i / LanesPerRow));
            Lane lane = Lane.Create(i, location, lanesParent, GameConstants.StartingLives, GameConstants.StartingIncome, GameConstants.StartingGold, new HashSet<ElementalTechType>(), ElementalTechConstants.InitialTechCost);
            Lanes[i] = lane;
            
            LaneIDByLane[lane] = i;
            LaneEventBus.LaneCreated(lane);
        }
    }
}