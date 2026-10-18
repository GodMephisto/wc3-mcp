---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/TransitionData/TargetSelectedInterfaceTransitionData.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

public class TargetSelectedInterfaceTransitionData : InterfaceTransitionData {
    public List<ClientEntity> Targets { get; }

    public TargetSelectedInterfaceTransitionData(List<ClientEntity> targets) {
        Targets = new List<ClientEntity>(targets);
    }
}
