---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Tower/Towers/CutterBase/ClientWrecker.cs"
ref: HEAD
---

﻿public class ClientWrecker : ClientTower
{
    public override TowerType Type => TowerType.Wrecker;
    
    protected override void Awake() {
        PointInAttackAnimationOfAttack = 0.52f;

        base.Awake();
    }
}