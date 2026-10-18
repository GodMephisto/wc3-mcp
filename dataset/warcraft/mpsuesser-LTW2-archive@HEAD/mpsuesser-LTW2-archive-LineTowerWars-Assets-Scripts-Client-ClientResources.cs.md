---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/ClientResources.cs"
ref: HEAD
---

using System;
using UnityEngine;
using System.Collections.Generic;

public class ClientResources : SingletonBehaviour<ClientResources> {
    private Dictionary<TowerType, Sprite> TowerImages { get; set; }
    private Dictionary<EnemyType, Sprite> EnemyImages { get; set; }
    private Dictionary<ElementalTechType, Sprite> TechImages { get; set; }
    private Dictionary<TowerType, TowerAttackEffect> TowerAttackEffects { get; set; }

    [SerializeField] private Material[] LaneColorMaterials;
    private Color[] LaneColors { get; set; }
    
    private const string ClientResourcesPath = "Client";

    private void Awake() {
        InitializeSingleton(this);

        LoadTowerImages();
        LoadEnemyImages();
        LoadTechImages();
        LoadTowerAttackEffects();
        LoadLaneColors();
    }

    private void LoadTowerImages() {
        TowerImages = new Dictionary<TowerType, Sprite>();
        TowerImageResource[] imageResources = Resources.LoadAll<TowerImageResource>($"{ClientResourcesPath}/TowerImages");
        foreach (TowerImageResource imageResource in imageResources) {
            if (imageResource.Image != null) {
                TowerImages.Add(imageResource.Tower, imageResource.Image);
            }
        }
    }

    private void LoadEnemyImages() {
        EnemyImages = new Dictionary<EnemyType, Sprite>();
        EnemyImageResource[] imageResources = Resources.LoadAll<EnemyImageResource>($"{ClientResourcesPath}/EnemyImages");
        foreach (EnemyImageResource imageResource in imageResources) {
            if (imageResource.Image != null) {
                EnemyImages.Add(imageResource.Enemy, imageResource.Image);
            }
        }
    }

    private void LoadTechImages() {
        TechImages = new Dictionary<ElementalTechType, Sprite>();
        TechImageResource[] imageResources = Resources.LoadAll<TechImageResource>($"{ClientResourcesPath}/TechImages");
        foreach (TechImageResource imageResource in imageResources) {
            if (imageResource.Image != null) {
                TechImages.Add(imageResource.TechType, imageResource.Image);
            }
        }
    }

    private void LoadTowerAttackEffects() {
        TowerAttackEffects = new Dictionary<TowerType, TowerAttackEffect>();
        TowerAttackEffect[] attackEffects = Resources.LoadAll<TowerAttackEffect>($"{ClientResourcesPath}/TowerAttackEffects");
        foreach (TowerAttackEffect attackEffect in attackEffects) {
            TowerAttackEffects[attackEffect.SourceTowerType] = attackEffect;
        }
    }

    private void LoadLaneColors() {
        LaneColors = new Color[LaneColorMaterials.Length];
        for (int i = 0; i < LaneColorMaterials.Length; i++) {
            LaneColors[i] = LaneColorMaterials[i].color;
        }
    }

    public Sprite GetSpriteForTowerType(TowerType type)
    {
        return TowerImages.ContainsKey(type) ? TowerImages[type] : null;
    }

    public Sprite GetSpriteForEnemyType(EnemyType type)
    {
        return EnemyImages.ContainsKey(type) ? EnemyImages[type] : null;
    }

    public Sprite GetSpriteForTechType(ElementalTechType type) {
        return TechImages.ContainsKey(type) ? TechImages[type] : null;
    }

    public TowerAttackEffect GetAttackEffectForTowerType(TowerType type) {
        if (!TowerAttackEffects.ContainsKey(type)) {
            throw new ResourceNotFoundException();
        }

        return TowerAttackEffects[type];
    }

    public Color GetColorForLane(int laneNum) {
        if (laneNum >= LaneColors.Length) {
            throw new IndexOutOfRangeException();
        }

        return LaneColors[laneNum];
    }

    public Material GetColorMaterialForLane(int laneNum) {
        if (laneNum >= LaneColorMaterials.Length) {
            throw new IndexOutOfRangeException();
        }

        return LaneColorMaterials[laneNum];
    }
}