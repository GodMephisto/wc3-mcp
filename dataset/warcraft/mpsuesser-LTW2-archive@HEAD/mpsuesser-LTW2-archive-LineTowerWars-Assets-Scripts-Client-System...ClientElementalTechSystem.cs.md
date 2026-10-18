---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/ClientElementalTechSystem/ClientElementalTechSystem.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;

public class ClientElementalTechSystem : SingletonBehaviour<ClientElementalTechSystem>
{
    private void Awake() {
        InitializeSingleton(this);
    }

    public bool LaneFulfillsRequirementsForPurchase(Lane lane, ElementalTechType upgradeType) {
        return lane.Gold >= lane.TechCost
            && (
            !ElementalTech.Prerequisite.ContainsKey(upgradeType)
            || lane.HasTech(ElementalTech.Prerequisite[upgradeType])
        );
    }
}
