---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Buffs/BuffDisplay.cs"
ref: HEAD
---

﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffDisplay : HoverTooltip {
    public static BuffDisplay Create(BuffState bs, Transform parent) {
        BuffDisplay bd = Instantiate(
            ClientPrefabs.Singleton.pfBuffDisplay,
            parent
        );

        bd.LoadState(bs);

        return bd;
    }
    
    [SerializeField] private Image positiveImage;
    [SerializeField] private Image negativeImage;
    [SerializeField] private TMP_Text stacksText;
    
    private bool IsActive { get; set; }
    
    private BuffDescriptor ActiveBuffDescriptor { get; set; }
    private BuffState ActiveBuffState { get; set; }

    protected override void Awake() {
        IsActive = false;

        base.Awake();
    }

    private void LoadState(BuffState bs) {
        try {
            ActiveBuffDescriptor = BuffDescriptors.Singleton.GetBuffDescriptorByType(bs.Type);
        }
        catch (BuffDescriptorNotFoundException e) {
            LTWLogger.Log(e.Message);
            return;
        }

        ActiveBuffState = bs;
        
        IsActive = true;

        PopulateDisplay();
    }

    public void Clear() {
        ActiveBuffDescriptor = null;
        ActiveBuffState = null;
        
        IsActive = false;

        ClearDisplay();
    }

    private void PopulateDisplay() {
        positiveImage.sprite = ActiveBuffDescriptor.Image;
        negativeImage.sprite = ActiveBuffDescriptor.Image;
        
        stacksText.SetText(ActiveBuffState.Stacks > 1 ? ActiveBuffState.Stacks.ToString() : "");
    }

    private void ClearDisplay() {
        positiveImage.sprite = null;
        positiveImage.sprite = null;
        
        stacksText.SetText("");
    }

    private void Update() {
        if (!IsActive || !ActiveBuffState.IsDurationBased) {
            return;
        }
        
        positiveImage.fillAmount =
            ActiveBuffState.DurationRemaining / ActiveBuffState.FullDuration;
    }
    
    protected override Tooltip GetTooltipContent() {
        return new BuffTooltip(ActiveBuffDescriptor);
    }
}