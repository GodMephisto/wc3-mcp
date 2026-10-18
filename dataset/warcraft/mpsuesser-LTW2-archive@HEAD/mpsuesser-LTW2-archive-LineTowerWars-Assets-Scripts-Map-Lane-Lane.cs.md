---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/Lane/Lane.cs"
ref: HEAD
---

using UnityEngine;
using System;
using System.Collections.Generic;

public class Lane : MonoBehaviour
{
    public static Lane Create(int laneID, Vector3 location, Transform parent, int lives, int income, int gold, HashSet<ElementalTechType> techs, int techCost) {
        Lane lane = Instantiate(MapPrefabs.Singleton.pfLane, location, Quaternion.identity, parent);

        lane.SetID(laneID);
        lane.SetLives(lives);
        lane.SetIncome(income);
        lane.SetGold(gold);
        lane.SetTechs(techs);
        lane.SetTechCost(techCost);

        return lane;
    }

    [SerializeField] public LaneSpawnArea SpawnArea;
    [SerializeField] public LaneEndzone Endzone;
    [SerializeField] public MazeGrid Grid;

    public CreepStock Stock { get; private set; }

    public int ID { get; private set; }
    public int Lives { get; private set; }
    public event Action<Lane> OnLivesUpdated;

    public int Income { get; private set; }
    public event Action<Lane> OnIncomeUpdated;

    public int Gold { get; private set; }
    public event Action<Lane> OnGoldUpdated;

    public int ActiveUnits { get; private set; }
    public event Action<Lane> OnActiveUnitsUpdated;

    public HashSet<ElementalTechType> Techs { get; private set; }
    public event Action<Lane> OnTechUpdated;

    public int TechCost { get; private set; }
    public event Action<Lane> OnTechCostUpdated;

    public bool IsActive => Lives > 0;
    
    private void Awake() {
        Stock = new CreepStock();
    }

    private void SetID(int laneID) {
        ID = laneID;
        Grid.Init(ID);
    }

    public void SetLives(int lives) {
        Lives = lives;
        OnLivesUpdated?.Invoke(this);
        LaneEventBus.LaneLivesUpdated(this);
    }
    public void AddLives(int amt) => SetLives(Lives + amt);
    public void DeductLives(int amt) => SetLives(Math.Max(0, Lives - amt));

    public void SetIncome(int income) {
        Income = income;
        OnIncomeUpdated?.Invoke(this);
        LaneEventBus.LaneIncomeUpdated(this);
    }
    public void AddIncome(int amt) => SetIncome(Income + amt);
    public void DeductIncome(int amt) => SetIncome(Math.Max(0, Income - amt));

    public void SetGold(int gold) {
        Gold = gold;
        OnGoldUpdated?.Invoke(this);
        LaneEventBus.LaneGoldUpdated(this);
    }
    public void AddGold(int amt) => SetGold(Gold + amt);
    public void DeductGold(int amt) => SetGold(Math.Max(0, Gold - amt));

    public void SetActiveUnits(int unitCount) {
        ActiveUnits = unitCount;
        OnActiveUnitsUpdated?.Invoke(this);
        LaneEventBus.LaneActiveUnitsUpdated(this);
    }
    public void AddActiveUnits(int amt) => SetActiveUnits(ActiveUnits + amt);
    public void DeductActiveUnits(int amt) => SetActiveUnits(Math.Max(0, ActiveUnits - amt));

    public void SetTechs(HashSet<ElementalTechType> techs) {
        Techs = techs;
        OnTechUpdated?.Invoke(this);
        LaneEventBus.LaneTechsUpdated(this);
    }
    public void AddTech(ElementalTechType tech) {
        Techs.Add(tech);
        SetTechs(Techs);
    }
    public bool HasTech(ElementalTechType tech) => Techs.Contains(tech);

    public void SetTechCost(int techCost) {
        TechCost = techCost;
        OnTechCostUpdated?.Invoke(this);
        LaneEventBus.LaneTechCostUpdated(this);
    }
}
