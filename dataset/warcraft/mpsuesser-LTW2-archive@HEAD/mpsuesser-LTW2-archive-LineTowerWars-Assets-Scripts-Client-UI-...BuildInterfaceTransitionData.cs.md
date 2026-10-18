---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/TransitionData/BuildInterfaceTransitionData.cs"
ref: HEAD
---

﻿public class BuildInterfaceTransitionData : InterfaceTransitionData {
    public TowerType Type { get; }

    public BuildInterfaceTransitionData(TowerType type) {
        Type = type;
    }
}