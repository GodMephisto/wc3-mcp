---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/Grid/MazeGridCell.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGridCell : MonoBehaviour
{
    public int ID { get; private set; }
    public bool Occupied { get; private set; }
    private bool Hovering { get; set; }

    [SerializeField] private Material OccupiedMaterial;
    [SerializeField] private Material AvailableMaterial;
    [SerializeField] private Material UnavailableMaterial;
    [SerializeField] private Material SelectedMaterial;

    private MeshRenderer _renderer = null;

    public MeshRenderer Renderer {
        get {
            if (_renderer == null) {
                _renderer = GetComponent<MeshRenderer>();
            }

            return _renderer;
        }
    }

    public HashSet<MazeGridCell> Neighbors { get; set; }
    private static float NeighborSearchRadius => 3f; // should be limited to 4, no diagonal neighbors

    public void Init(int laneID) {
        SetID(laneID);
        Occupied = false;
    }

    public const int GridCellLaneIDOffset = 10000;
    private void SetID(int laneID) {
        string myName = transform.name;
        string parentName = transform.parent.name;
        int parentChildrenCount = transform.parent.childCount;
        int myNum = SharedUtil.ParseNumberFromString(myName);
        int parentNum = SharedUtil.ParseNumberFromString(parentName);
        ID = parentNum * parentChildrenCount + myNum + (laneID * GridCellLaneIDOffset);
    }

    private void Start() {
        FindNeighbors();
    }

    private void FindNeighbors() {
        Neighbors = new HashSet<MazeGridCell>();

        Collider[] neighborColliders = Physics.OverlapSphere(transform.position, NeighborSearchRadius, 1 << gameObject.layer);
        foreach (Collider neighborCollider in neighborColliders) {
            MazeGridCell cell = neighborCollider.gameObject.GetComponent<MazeGridCell>();
            if (cell != this) {
                Neighbors.Add(cell);
            }
        }
    }

    public MazeGridCell[] GetThreeNeighborsClosestToPoint(Vector3 point) {
        SortedList<float, MazeGridCell> rankedDistances = new SortedList<float, MazeGridCell>();
        foreach (MazeGridCell neighbor in Neighbors) {
            float dist = Vector3.Distance(point, neighbor.transform.position);
            rankedDistances.Add(dist, neighbor);
        }


        MazeGridCell[] closestNeighbors = new MazeGridCell[3];
        for (int i = 0; i < Math.Min(3, rankedDistances.Count); i++) {
            closestNeighbors[i] = rankedDistances.Values[i];
        }

        // See if closest neighbor has any neighbors in common with the second closest neighbor
        HashSet<MazeGridCell> commonNeighbors = new HashSet<MazeGridCell>(closestNeighbors[0].Neighbors);
        commonNeighbors.IntersectWith(closestNeighbors[1].Neighbors);
        commonNeighbors.Remove(this);
        if (commonNeighbors.Count == 1) {
            foreach (MazeGridCell cell in commonNeighbors) {
                closestNeighbors[2] = cell;
            }

            return closestNeighbors;
        }

        if (rankedDistances.Count < 3) {
            LTWLogger.LogError("There is an issue with the 4-cell neighbor algorithm for a corner cell with less than 3 neighbors!");
            return closestNeighbors;
        }

        // If not, then check for neighbors in common between first and third closest
        commonNeighbors = new HashSet<MazeGridCell>(closestNeighbors[0].Neighbors);
        commonNeighbors.IntersectWith(closestNeighbors[2].Neighbors);
        commonNeighbors.Remove(this);
        if (commonNeighbors.Count != 1) {
            LTWLogger.LogError("There is an issue with the 4-cell neighbor algorithm!");
        } else {
            foreach (MazeGridCell cell in commonNeighbors) {
                closestNeighbors[1] = cell;
            }
        }

        return closestNeighbors;
    }

    public void SetOccupied() {
        Occupied = true;
        UpdateIndicatorMaterial();
    }

    public void UnsetOccupied() {
        Occupied = false;
        UpdateIndicatorMaterial();
    }

    public void SetHover() {
        Hovering = true;
        UpdateIndicatorMaterial();
    }

    public void UnsetHover() {
        Hovering = false;
        UpdateIndicatorMaterial();
    }

    private void UpdateIndicatorMaterial() {
        if (Hovering) {
            if (Occupied) {
                SetMaterial(UnavailableMaterial);
            } else {
                SetMaterial(SelectedMaterial);
            }
        } else {
            if (Occupied) {
                SetMaterial(OccupiedMaterial);
            } else {
                SetMaterial(AvailableMaterial);
            }
        }
    }

    private void SetMaterial(Material m) {
        Renderer.material = m;
    }
}
