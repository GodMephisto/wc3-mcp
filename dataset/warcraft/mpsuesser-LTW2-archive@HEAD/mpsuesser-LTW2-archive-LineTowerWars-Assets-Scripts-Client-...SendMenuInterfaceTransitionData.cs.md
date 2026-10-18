---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/TransitionData/SendMenuInterfaceTransitionData.cs"
ref: HEAD
---

﻿public class SendMenuInterfaceTransitionData : InterfaceTransitionData {
    public int TavernIndex { get; }

    public SendMenuInterfaceTransitionData(int tavernIndex) {
        TavernIndex = tavernIndex;
    }
}