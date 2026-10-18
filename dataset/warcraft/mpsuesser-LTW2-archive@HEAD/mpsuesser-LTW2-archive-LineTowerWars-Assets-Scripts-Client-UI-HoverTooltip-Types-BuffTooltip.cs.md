---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Types/BuffTooltip.cs"
ref: HEAD
---

﻿using UnityEngine;

public class BuffTooltip : Tooltip {
    public string Name { get; }
    public string Description { get; }
    public Sprite BuffImageSprite { get; }

    public BuffTooltip(BuffDescriptor bd) {
        LTWLogger.Log($"BD type => {bd.Type}");
        Name = EffectDescriptors.Buff[bd.Type].Name;
        Description = EffectDescriptors.Buff[bd.Type].Description;
        LTWLogger.Log($"Loaded! Name => {Name}... Description => {Description}");
        BuffImageSprite = bd.Image;
    }

    public BuffTooltip(BuffType buffType) {
        Name = EffectDescriptors.Buff[buffType].Name;
        Description = EffectDescriptors.Buff[buffType].Description;
        // TODO: Get BuffDescriptor and load BuffImageSprite
    }
}