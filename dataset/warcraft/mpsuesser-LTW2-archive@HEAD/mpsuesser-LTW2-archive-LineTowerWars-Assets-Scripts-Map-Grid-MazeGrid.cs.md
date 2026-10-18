---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/Grid/MazeGrid.cs"
ref: HEAD
---

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGrid : MonoBehaviour
{
    [SerializeField] private GameObject Content;

    private Dictionary<int, MazeGridCell> CellsByID { get; set; }
    
    public MazeGridRow[] AllRows { get; set; }
    public MazeGridCell[] AllCells { get; set; }

    public void Init(int laneID) {
        AllRows = GetComponentsInChildren<MazeGridRow>(true);
        foreach (MazeGridRow row in AllRows) {
            row.Init(laneID);
        }
        
        Array.Sort(AllRows, (a, b) => b.MaxZ.CompareTo(a.MaxZ));
        
        CellsByID = new Dictionary<int, MazeGridCell>();
        AllCells = GetComponentsInChildren<MazeGridCell>(true);
        foreach (MazeGridCell cell in AllCells) {
            CellsByID[cell.ID] = cell;
        }
        
        Hide();
    }

    public void Show() {
        Content.SetActive(true);
    }

    public void Hide() {
        Content.SetActive(false);
    }

    public MazeGridCell[] GetCellsByID(int[] cellIDs) {
        MazeGridCell[] cells = new MazeGridCell[cellIDs.Length];
        for (int i = 0; i < cellIDs.Length; i++) {
            if (!CellsByID.ContainsKey(cellIDs[i])) {
                throw new NotFoundException($"Invalid cell ID provided: {cellIDs[i].ToString()}");
            }

            cells[i] = CellsByID[cellIDs[i]];
        }
        return cells;
    }

    public HashSet<MazeGridCell> GetCellsByID(HashSet<int> cellIDs) {
        HashSet<MazeGridCell> cells = new HashSet<MazeGridCell>();
        foreach (int cellID in cellIDs) {
            if (!CellsByID.ContainsKey(cellID)) {
                throw new NotFoundException($"Invalid cell ID provided: {cellID.ToString()}");
            }

            cells.Add(CellsByID[cellID]);
        }
        return cells;
    }
}