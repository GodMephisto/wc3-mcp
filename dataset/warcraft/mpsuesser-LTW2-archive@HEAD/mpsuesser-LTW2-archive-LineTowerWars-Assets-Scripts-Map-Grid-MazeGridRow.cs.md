---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/Grid/MazeGridRow.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGridRow : MonoBehaviour {
    public MazeGridCell[] Cells { get; set; }
    
    public float MinX { get; private set; }
    public float MaxX { get; private set; }
    public float MinZ { get; private set; }
    public float MaxZ { get; private set; }

    public void Init(int laneID) {
        Cells = GetComponentsInChildren<MazeGridCell>(true);

        MinX = Mathf.Infinity;
        MaxX = -Mathf.Infinity;
        MinZ = Mathf.Infinity;
        MaxZ = -Mathf.Infinity;
        
        foreach (MazeGridCell cell in Cells) {
            cell.Init(laneID);
            
            MeshRenderer meshRenderer = cell.Renderer;
            Vector3 boundMin = meshRenderer.bounds.min;
            Vector3 boundMax = meshRenderer.bounds.max;

            MinX = Mathf.Min(MinX, boundMin.x);
            MaxX = Mathf.Max(MaxX, boundMax.x);
            MinZ = Mathf.Min(MinZ, boundMin.z);
            MaxZ = Mathf.Max(MaxZ, boundMax.z);
        }
    }
}