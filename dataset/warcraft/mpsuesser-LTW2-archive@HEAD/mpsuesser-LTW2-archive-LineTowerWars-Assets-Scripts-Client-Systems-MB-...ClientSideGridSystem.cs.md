---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/ClientSideGridSystem/ClientSideGridSystem.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/* The full grid will be of size 8 x 25 towers.
 * Each tower occupies 4 cells.
 * The grid should be 16 x 50 cells large.
 */

public class ClientSideGridSystem : SingletonBehaviour<ClientSideGridSystem>
{
    [SerializeField] private LayerMask GridCellLayerMask;
    [SerializeField] private Transform gridOutlineParent;
    [SerializeField] private Transform rowNumbersParent;
    
    private static MazeGrid MyGrid => ClientLaneTracker.Singleton.MyLane.Grid;

    private const float HoverSensitivityRadius = 2f;

    public MazeGridCell[] CurrentHoverCells => prevHover.ToArray();
    private HashSet<MazeGridCell> prevHover;

    private HashSet<GridCellOutline> CellOutlines { get; set; }
    private HashSet<GridRowNumber> RowNumbers { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        prevHover = new HashSet<MazeGridCell>();
        
        EventBus.OnGridCellsOccupancyUpdated += UpdateCellsOccupancyStatus;
        EventBus.OnMyLaneUpdated += SetupSystemForLane;
    }

    public void OnDestroy() {
        EventBus.OnGridCellsOccupancyUpdated -= UpdateCellsOccupancyStatus;
        EventBus.OnMyLaneUpdated -= SetupSystemForLane;
    }

    private void SetupSystemForLane(Lane lane) {
        ResetCellOutlines();
        ResetRowNumbers();
        
        CreateCellOutlinesForLane(lane);
        CreateRowNumbersForLane(lane);
        
        Disable();
    }

    private void ResetCellOutlines() {
        if (CellOutlines != null) {
            foreach (GridCellOutline outline in CellOutlines) {
                Destroy(outline.gameObject);
            }

            CellOutlines.Clear();
        }
        else {
            CellOutlines = new HashSet<GridCellOutline>();
        }
    }

    private void CreateCellOutlinesForLane(Lane lane) {
        int cellsCount = lane.Grid.AllCells.Length;
        int rowsCount = lane.Grid.AllRows.Length;
        int colsCount = cellsCount / rowsCount;
        int r = 0;
        int c = 0;
        while (c < colsCount - 1 && r < rowsCount - 1) {
            HashSet<int> cellGroupingIDs = new HashSet<int>() {
                (r) * colsCount + (c)
                    + (lane.ID * MazeGridCell.GridCellLaneIDOffset),
                (r+1) * colsCount + (c)
                    + (lane.ID * MazeGridCell.GridCellLaneIDOffset),
                (r) * colsCount + (c+1)
                    + (lane.ID * MazeGridCell.GridCellLaneIDOffset),
                (r+1) * colsCount + (c+1)
                    + (lane.ID * MazeGridCell.GridCellLaneIDOffset),
            };
            
            HashSet<MazeGridCell> cellGrouping = lane.Grid.GetCellsByID(cellGroupingIDs);
            Vector3 centerPoint = ClientUtil.GetCenterPoint(cellGrouping.Select(cell => cell.transform.position).ToArray());
            GridCellOutline outline = GridCellOutline.Create(centerPoint, gridOutlineParent);
            CellOutlines.Add(outline);
            
            c += 2;
            if (c >= colsCount - 1) {
                c = 0;
                r += 2;
            }
        }
    }

    private void ResetRowNumbers() {
        if (RowNumbers != null) {
            foreach (GridRowNumber rowNumber in RowNumbers) {
                Destroy(rowNumber.gameObject);
            }

            RowNumbers.Clear();
        }
        else {
            RowNumbers = new HashSet<GridRowNumber>();
        }
    }

    private const float RowNumberOffsetDistanceFromEdge = 7f;
    private void CreateRowNumbersForLane(Lane lane) {
        MazeGridRow[] rows = lane.Grid.AllRows;
        int rowsCount = rows.Length;
        for (int i = 0; i < rowsCount - 1; i += 2) {
            MazeGridRow top = rows[i];
            MazeGridRow bottom = rows[i + 1];
            
            Vector3 pos = new Vector3(
                top.MinX - RowNumberOffsetDistanceFromEdge,
                0.1f,
                (top.MinZ + bottom.MaxZ) / 2
            );
            
            GridRowNumber rowNumber = GridRowNumber.Create(
                (i / 2) + 1,
                pos,
                rowNumbersParent
            );
            RowNumbers.Add(rowNumber);
        }
    }

    public void Enable() {
        gameObject.SetActive(true);
        MyGrid.Show();
        ShowGridLines();
        ClearHoverIndicators();
    }

    public void Disable() {
        gameObject.SetActive(false);
        ClearHoverIndicators();
        MyGrid.Hide();
        HideGridLines();
    }

    private void ShowGridLines() {
        gridOutlineParent.gameObject.SetActive(true);
        rowNumbersParent.gameObject.SetActive(true);
    }

    private void HideGridLines() {
        gridOutlineParent.gameObject.SetActive(false);
        rowNumbersParent.gameObject.SetActive(false);
    }


    private void Update() {
        try {
            Vector3 aimPoint = CameraController.Singleton.GetAimPoint();

            MazeGridCell closestCell = GetClosestCellToPoint(aimPoint);
            HashSet<MazeGridCell> hovering = new HashSet<MazeGridCell> { closestCell };
            hovering.UnionWith(closestCell.GetThreeNeighborsClosestToPoint(aimPoint));

            foreach (MazeGridCell cell in hovering) {
                cell.SetHover();
            }

            prevHover.ExceptWith(hovering);
            foreach (MazeGridCell cell in prevHover) {
                cell.UnsetHover();
            }

            prevHover = hovering;
        } catch (NotFoundException) {
            ClearHoverIndicators();
            return;
        } catch (Exception e) {
            Debug.Log(e);
            return;
        }
    }

    private MazeGridCell GetClosestCellToPoint(Vector3 point) {
        HashSet<MazeGridCell> hovering = new HashSet<MazeGridCell>();
        Collider[] nearbyColliders = Physics.OverlapSphere(
            point,
            HoverSensitivityRadius, 
            GridCellLayerMask
        );

        MazeGridCell closest = null;
        float closestDist = Mathf.Infinity;
        foreach (Collider nearbyCollider in nearbyColliders) {
            MazeGridCell cell = nearbyCollider.gameObject.GetComponent<MazeGridCell>();
            if (cell == null) {
                continue;
            }
            
            float dist = Vector3.Distance(point, nearbyCollider.transform.position);
            if (closest == null || dist < closestDist) {
                closest = cell;
                closestDist = dist;
            }
        }
        
        if (closest == null) {
            throw new NotFoundException($"No grid cells were found around point {point}");
        }

        return closest;
    }

    private static void UpdateCellsOccupancyStatus(HashSet<MazeGridCell> cells, bool occupied) {
        foreach (MazeGridCell cell in cells) {
            if (occupied) cell.SetOccupied();
            else cell.UnsetOccupied();
        }
    }

    private void ClearHoverIndicators() {
        foreach (MazeGridCell cell in prevHover) {
            cell.UnsetHover();
        }
        prevHover.Clear();
    }
}
