---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Enemy/Enemies/Tier1/ClientSheep.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSheep : ClientEnemy
{
    public override EnemyType Type => EnemyType.Sheep;
}
