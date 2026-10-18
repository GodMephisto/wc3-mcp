---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Effects/VisualEffect.cs"
ref: HEAD
---

﻿using UnityEngine;

public class VisualEffect : MonoBehaviour {
    [SerializeField] private float duration;

    private void Start() {
        Destroy(gameObject, duration);
    }
}