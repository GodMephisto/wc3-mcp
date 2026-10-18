---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Tower/Towers/CutterBase/ClientCutter.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCutter : ClientTower
{
    public override TowerType Type => TowerType.Cutter;
}
