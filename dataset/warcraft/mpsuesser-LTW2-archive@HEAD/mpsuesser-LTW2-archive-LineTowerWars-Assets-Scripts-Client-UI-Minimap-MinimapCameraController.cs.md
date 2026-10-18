---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Minimap/MinimapCameraController.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour {
    public Camera C { get; private set; }
    
    private float minX = Mathf.Infinity;
    private float maxX = -Mathf.Infinity;
    private float minZ = Mathf.Infinity;
    private float maxZ = -Mathf.Infinity;

    private void Awake() {
        C = GetComponent<Camera>();

        Settings.CameraRotation.Updated += UpdateCameraRotation;
        LaneEventBus.OnLaneCreated += UpdateZoomToAccountForLane;
    }

    private void OnDestroy() {
        Settings.CameraRotation.Updated -= UpdateCameraRotation;
        LaneEventBus.OnLaneCreated -= UpdateZoomToAccountForLane;
    }

    private void Start() {
        UpdateCameraRotation(Settings.CameraRotation.Value);
        
        for (int i = 0; i < LaneSystem.Singleton.Lanes.Length; i++) {
            UpdateZoomToAccountForLane(LaneSystem.Singleton.Lanes[i]);
        }
    }

    private void UpdateZoomToAccountForLane(Lane createdLane) {
        foreach (MazeGridRow row in createdLane.Grid.AllRows) {
            minX = Math.Min(minX, row.MinX);
            maxX = Math.Max(maxX, row.MaxX);
            minZ = Math.Min(minZ, row.MinZ);
            maxZ = Math.Max(maxZ, row.MaxZ);
        }

        Vector3[] allLanesCollectiveBounds = new Vector3[4] {
            new Vector3(minX, 0, minZ),
            new Vector3(minX, 0, maxZ),
            new Vector3(maxX, 0, minZ),
            new Vector3(maxX, 0, maxZ),
        };

        transform.position = new Vector3(
            (minX + maxX) / 2,
            transform.position.y,
            (minZ + maxZ) / 2
        );
        
        LTWLogger.Log("Updating zoom to account for new lane...");
        for (int i = 0; i < 4; i++) {
            LTWLogger.Log($"Lane collective bound: x={allLanesCollectiveBounds[i].x}, y={allLanesCollectiveBounds[i].y}, z={allLanesCollectiveBounds[i].z}");
        };
        
        while (!ArePointsContainedWithinPointsOnXZAxis(allLanesCollectiveBounds, GetWorldCornersForHeight(2f), 50)) {
            LTWLogger.Log("Zooming!");
            transform.position += (Vector3.up * 50);
        }
        
        EventBus.MinimapWorldCornersUpdated(GetWorldCornersForHeight(2f));
    }

    private static bool ArePointsContainedWithinPointsOnXZAxis(IEnumerable<Vector3> contained, IEnumerable<Vector3> containers, int buffer = 0) {
        float minContainedX = Mathf.Infinity;
        float maxContainedX = -Mathf.Infinity;
        float minContainedZ = Mathf.Infinity;
        float maxContainedZ = -Mathf.Infinity;
        foreach (Vector3 containedPoint in contained) {
            minContainedX = Math.Min(minContainedX, containedPoint.x);
            maxContainedX = Math.Max(maxContainedX, containedPoint.x);
            minContainedZ = Math.Min(minContainedZ, containedPoint.z);
            maxContainedZ = Math.Max(maxContainedZ, containedPoint.z);
        }
        
        float minContainersX = Mathf.Infinity;
        float maxContainersX = -Mathf.Infinity;
        float minContainersZ = Mathf.Infinity;
        float maxContainersZ = -Mathf.Infinity;
        foreach (Vector3 containerPoint in containers) {
            minContainersX = Math.Min(minContainersX, containerPoint.x);
            maxContainersX = Math.Max(maxContainersX, containerPoint.x);
            minContainersZ = Math.Min(minContainersZ, containerPoint.z);
            maxContainersZ = Math.Max(maxContainersZ, containerPoint.z);
        }

        minContainedX -= buffer;
        minContainedZ -= buffer;
        maxContainedX += buffer;
        maxContainedZ += buffer;

        return minContainedX > minContainersX
               && maxContainedX < maxContainersX
               && minContainedZ > minContainersZ
               && maxContainedZ < maxContainersZ;
    }

    private void UpdateCameraRotation(int rotation) {
        Quaternion curRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(
            curRotation.eulerAngles.x,
            rotation,
            curRotation.eulerAngles.z
        );
    }

    public Vector3[] GetWorldCornersForHeight(float height) {
        Vector3[] worldCorners = new Vector3[4];
        Ray[] worldCornerRays = new Ray[4];
        worldCornerRays[0] = C.ViewportPointToRay(new Vector3(0, 0));
        worldCornerRays[1] = C.ViewportPointToRay(new Vector3(0, 1));
        worldCornerRays[2] = C.ViewportPointToRay(new Vector3(1, 1));
        worldCornerRays[3] = C.ViewportPointToRay(new Vector3(1, 0));

        Vector3 refPoint = new Vector3(0, height, 0);
        Plane heightPlane = new Plane(Vector3.up, refPoint);

        for (int i = 0; i < 4; i++) {
            if (heightPlane.Raycast(worldCornerRays[i], out float enter)) {
                worldCorners[i] = worldCornerRays[i].GetPoint(enter);
            }
        }
        
        return worldCorners;
    }
}
