---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Minimap/Minimap.cs"
ref: HEAD
---

﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour, IPointerDownHandler {
    [SerializeField] private CameraController mainCameraController;
    [SerializeField] private MinimapCameraController minimapCameraController;
    [SerializeField] private LineRenderer lineRenderer;
    
    private RectTransform ScreenRect { get; set; }

    private void Awake() {
        ScreenRect = GetComponent<RectTransform>();

        EventBus.OnCameraMovement += AdjustCameraOutline;
    }

    private void OnDestroy() {
        EventBus.OnCameraMovement -= AdjustCameraOutline;
    }

    private void AdjustCameraOutline(CameraController cc) {
        Vector3[] worldCorners = cc.GetWorldCornersForHeight(2f);

        lineRenderer.positionCount = 4;
        lineRenderer.SetPositions(worldCorners);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Vector3 worldPressPoint = GetWorldPressPoint(eventData.position);
        Debug.DrawLine(worldPressPoint, minimapCameraController.transform.position, Color.blue, 4f);

        // eventData.button can be checked for equality ot e.g. PointerEventData.InputButton.Right
        mainCameraController.FocusOn(worldPressPoint);
    }

    // Gets world press point from 2D click point on minimap image
    private Vector3 GetWorldPressPoint(Vector2 pressPoint) {
        Camera mmCamera = minimapCameraController.C;

        Vector3[] cameraViewportCorners = minimapCameraController.GetWorldCornersForHeight(2f);

        for (int i = 0; i < 4; i++) {
            Debug.DrawLine(mmCamera.transform.position, cameraViewportCorners[i], Color.red, 4.0f);
        }

        Vector3[] imageWorldCorners = new Vector3[4];
        ScreenRect.GetWorldCorners(imageWorldCorners);
        
        // Subtract the offset in world position of the image
        pressPoint.x -= imageWorldCorners[0].x;
        pressPoint.y -= imageWorldCorners[0].y;

        // For reference, let's also subtract the same offset from the top right coords, which will act as our max
        Vector2 maxImageLocation = new Vector2(
            imageWorldCorners[2].x - imageWorldCorners[0].x,
            imageWorldCorners[2].y - imageWorldCorners[0].y
        );

        // Get the proportion x and y of clicked, so min 0 and max 1 for both values
        Vector2 scaledPressPoint = new Vector2(
            pressPoint.x / maxImageLocation.x,
            pressPoint.y / maxImageLocation.y
        );

        LTWLogger.Log("Scaled press point: " + scaledPressPoint);

        float minX = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float minZ = Mathf.Infinity;
        float maxZ = -Mathf.Infinity;

        for (int i = 0; i < 4; i++) {
            minX = Math.Min(minX, cameraViewportCorners[i].x);
            maxX = Math.Max(maxX, cameraViewportCorners[i].x);
            minZ = Math.Min(minZ, cameraViewportCorners[i].z);
            maxZ = Math.Max(maxZ, cameraViewportCorners[i].z);
        }
        
        LTWLogger.Log($"Min X: {minX}");
        LTWLogger.Log($"Max X: {maxX}");
        LTWLogger.Log($"Min Z: {minZ}");
        LTWLogger.Log($"Max Z: {maxZ}");

        Vector2 viewportSize = new Vector2(
            maxX - minX,
            maxZ - minZ
        );

        Vector3 worldPressPoint = 
            Quaternion.Euler(0, Settings.CameraRotation.Value, 0) 
            * new Vector3(
                (scaledPressPoint.x * viewportSize.x) + minX,
                0,
                (scaledPressPoint.y * viewportSize.y) + minZ
            );

        return worldPressPoint;
    }

    public bool IsClickInMinimap(Vector2 clickPos) {
        Vector3[] imageWorldCorners = new Vector3[4];
        ScreenRect.GetWorldCorners(imageWorldCorners);
        /* [0]: bottom left     (2.7, 2.7, 0.0)
         * [1]: top left        (2.7, 109.3, 0.0)
         * [2]: top right       (109.3, 109.3, 0.0)
         * [3]: bottom right    (109.3, 2.7, 0.0)
         */

        return (
            clickPos.x > imageWorldCorners[0].x &&
            clickPos.x < imageWorldCorners[2].x &&
            clickPos.y > imageWorldCorners[0].y &&
            clickPos.y < imageWorldCorners[2].y
        );
    }
}