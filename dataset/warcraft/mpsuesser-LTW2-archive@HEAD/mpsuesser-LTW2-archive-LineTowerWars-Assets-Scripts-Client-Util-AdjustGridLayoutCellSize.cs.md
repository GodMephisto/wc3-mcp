---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Util/AdjustGridLayoutCellSize.cs"
ref: HEAD
---

﻿using System;
using UnityEngine;
using UnityEngine.UI;
 
[ExecuteInEditMode]
[RequireComponent(typeof(GridLayoutGroup))]
public class AdjustGridLayoutCellSize : MonoBehaviour
{
    private new RectTransform transform;
    private GridLayoutGroup grid;

    private void Awake() {
        Init();
    }

    private void Init() {
        transform = (RectTransform)base.transform;
        grid = GetComponent<GridLayoutGroup>();
    }
 
    // Start is called before the first frame update
    private void Start()
    {
        UpdateCellSize();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateCellSize();
    }
 
#if UNITY_EDITOR
    [ExecuteAlways]
    private void Update()
    {
        UpdateCellSize();
    }
#endif

    private void OnValidate()
    {
        transform = (RectTransform)base.transform;
        grid = GetComponent<GridLayoutGroup>();
        UpdateCellSize();
    }

    private void UpdateCellSize() {
        if (transform == null) {
            Init();
        }
        
        int childCount = transform.childCount;

        // Assumes x and y spacing are equal so just gets x spacing
        float spacing = grid.spacing.x;

        float horizontalContentSize = transform.rect.width - grid.padding.left - grid.padding.right;
        float verticalContentSize = transform.rect.height - grid.padding.top - grid.padding.bottom;

        float largerAxisSize = Mathf.Max(horizontalContentSize, verticalContentSize);
        float smallerAxisSize = Mathf.Min(horizontalContentSize, verticalContentSize);

        int rows = 0;
        float totalSpacing, sizePerCell, totalContentSizeAccountingForSpacing;
        do {
            rows++;
            totalSpacing = ((childCount - 1 - rows - 1) * spacing);
            sizePerCell = (smallerAxisSize - (spacing * (rows - 1))) / rows;
            totalContentSizeAccountingForSpacing = sizePerCell * childCount + totalSpacing;
        } while (totalContentSizeAccountingForSpacing / rows > largerAxisSize);

        grid.cellSize = new Vector2(
            sizePerCell,
            sizePerCell
        );
    }
}