---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Scoreboard/Scoreboard.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private Transform rowsParent;

    private List<ScoreboardRow> Rows { get; set; }

    private void Start() {
        Rows = new List<ScoreboardRow>();
        foreach (Lane lane in LaneSystem.Singleton.Lanes) {
            Rows.Add(ScoreboardRow.Create(lane, rowsParent));
        }
    }
}
