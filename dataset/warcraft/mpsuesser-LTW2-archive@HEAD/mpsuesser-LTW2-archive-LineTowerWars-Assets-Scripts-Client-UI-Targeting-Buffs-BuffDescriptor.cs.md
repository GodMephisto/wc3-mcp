---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Buffs/BuffDescriptor.cs"
ref: HEAD
---

﻿using UnityEngine;

[CreateAssetMenu(fileName = "BuffDescriptor", menuName = "ScriptableObjects/BuffDescriptor", order = 1)]
public class BuffDescriptor : ScriptableObject {
    [SerializeField] public BuffType Type;

    [SerializeField] public Sprite Image;
}