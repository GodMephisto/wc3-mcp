---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/MapEnvironment.cs"
ref: HEAD
---

﻿public class MapEnvironment : SingletonBehaviour<MapEnvironment> {
    private void Awake() {
        InitializeSingleton(this);

        Disable();
    }

    public void Enable() {
        gameObject.SetActive(true);
    }
    
    public void Disable() {
        gameObject.SetActive(false);
    }
}