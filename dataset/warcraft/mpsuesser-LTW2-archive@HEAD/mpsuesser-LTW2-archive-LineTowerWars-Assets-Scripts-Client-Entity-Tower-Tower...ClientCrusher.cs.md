---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Tower/Towers/CutterBase/ClientCrusher.cs"
ref: HEAD
---

﻿public class ClientCrusher : ClientTower
{
    public override TowerType Type => TowerType.Crusher;

    protected override void Awake() {
        PointInAttackAnimationOfAttack = 0.52f;

        base.Awake();
    }
}