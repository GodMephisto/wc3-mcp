---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Camera/CameraController.cs"
ref: HEAD
---

using System;
using UnityEngine;

public class CameraController : SingletonBehaviour<CameraController>
{
    [SerializeField] private LayerMask AimPointMask;

    public Camera C { get; private set; }

    private Lane FocusedLane { get; set; }
    private int LaneIndex { get; set; }
    
    private bool doMovement = true;

    // Panning
    private float MinXBound { get; set; }
    private float MaxXBound { get; set; }
    private float MinZBound { get; set; }
    private float MaxZBound { get; set; }
    private int PanBorderThickness { get; set; }

    // To notify subscribers of camera update
    private Vector3 prevPosition;
    private Quaternion prevRotation;
    private const float MinDiffToTriggerEvent = 0.1f;

    public Vector3 GetAimPoint() {
        Ray ray = C.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, AimPointMask)) {
            throw new NotFoundException("Could not get an aim point");
        }

        return hit.point;
    }

    private const float DefaultDistanceIfFOVIsBeyondParallel = 1000f;
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
            else {
                Vector3 defaultedPoint = worldCornerRays[i].GetPoint(DefaultDistanceIfFOVIsBeyondParallel);
                defaultedPoint.y = height;
                worldCorners[i] = defaultedPoint;
            }
        }
        
        return worldCorners;
    }

    private void Awake() {
        InitializeSingleton(this);

        C = GetComponent<Camera>();

        prevPosition = transform.position;
        prevRotation = transform.rotation;

        MinXBound = Mathf.Infinity;
        MinZBound = Mathf.Infinity;
        MaxXBound = -Mathf.Infinity;
        MaxZBound = -Mathf.Infinity;

        EventBus.OnMyLaneUpdated += SetFocusedLane;
        EventBus.OnSetActiveInterfaceState += UpdateMovementLockByActiveInterface;
        EventBus.OnMinimapWorldCornersUpdated += UpdateBounds;

        Settings.CameraFieldOfViewAngle.Updated += UpdateCameraFOV;
        Settings.CameraHeight.Updated += UpdateCameraHeight;
        Settings.CameraRotation.Updated += UpdateCameraRotation;
        Settings.CameraPanBorderThickness.Updated += UpdateCameraPanBorderThickness;
        
        UpdateCameraPanBorderThickness(Settings.CameraPanBorderThickness.Value);
    }

    private void OnDestroy() {
        EventBus.OnMyLaneUpdated -= SetFocusedLane;
        EventBus.OnSetActiveInterfaceState -= UpdateMovementLockByActiveInterface;
        EventBus.OnMinimapWorldCornersUpdated -= UpdateBounds;
        
        Settings.CameraFieldOfViewAngle.Updated -= UpdateCameraFOV;
        Settings.CameraHeight.Updated -= UpdateCameraHeight;
        Settings.CameraRotation.Updated -= UpdateCameraRotation;
        Settings.CameraPanBorderThickness.Updated -= UpdateCameraPanBorderThickness;
    }

    private void UpdateCameraPanBorderThickness(int thickness) {
        PanBorderThickness = thickness;
    }

    private void UpdateBounds(Vector3[] worldCorners) {
        foreach (Vector3 corner in worldCorners) {
            MinXBound = Math.Min(MinXBound, corner.x);
            MinZBound = Math.Min(MinZBound, corner.z);
            MaxXBound = Math.Max(MaxXBound, corner.x);
            MaxZBound = Math.Max(MaxZBound, corner.z);
        }
    }

    private void UpdateMovementLockByActiveInterface(InterfaceState activeInterface) {
        if (InGameInterfaceStateSystem.IsCameraMovementLockedState(activeInterface)) {
            LockMovement();
        }
        else {
            UnlockMovement();
        }
    }

    private void Start() {
        if (FocusedLane == null && ClientLaneTracker.Singleton?.MyLane != null) {
            SetFocusedLane(ClientLaneTracker.Singleton.MyLane);
        }
    }

    private void Update() {
        if (!doMovement) {
            return;
        }

        // Testing...
        /* if (Input.GetKeyDown(KeyCode.X)) {
            FocusedLane = LaneSystem.Singleton.Lanes[++LaneIndex % LaneSystem.Singleton.MaxLaneCount];
            FocusOn(FocusedLane.transform.position);
            return;
        } */

        // Up
        if (Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - PanBorderThickness) {
            transform.Translate(
                Quaternion.Euler(0, Settings.CameraRotation.Value, 0)
                            * Vector3.forward
                            * Settings.CameraScrollSpeed.Value
                            * Time.deltaTime, 
                Space.World
            );
        }

        // Down
        if (Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= PanBorderThickness) {
            transform.Translate(
                Quaternion.Euler(0, Settings.CameraRotation.Value, 0)
                * Vector3.back
                * Settings.CameraScrollSpeed.Value
                * Time.deltaTime, 
                Space.World
            );
        }

        // Left
        if (Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= PanBorderThickness) {
            transform.Translate(
                Quaternion.Euler(0, Settings.CameraRotation.Value, 0)
                * Vector3.left
                * Settings.CameraScrollSpeed.Value
                * Time.deltaTime,
                Space.World
            );
        }

        // Right
        if (Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - PanBorderThickness) {
            transform.Translate(
                Quaternion.Euler(0, Settings.CameraRotation.Value, 0)
                * Vector3.right
                * Settings.CameraScrollSpeed.Value
                * Time.deltaTime, 
                Space.World
            );
        }

        // Clamp the zoom within our bounds
        if (float.IsPositiveInfinity(MinXBound)) { // we haven't set any bounds yet
            return;
        }
        
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, MinXBound, MaxXBound);
        pos.z = Mathf.Clamp(pos.z, MinZBound, MaxZBound);
        transform.position = pos;
    }

    private void LateUpdate() {
        float positionalDiff = Vector3.Distance(prevPosition, transform.position);
        float rotationalDiff = Quaternion.Angle(prevRotation, transform.rotation);
        if (positionalDiff > MinDiffToTriggerEvent || rotationalDiff > MinDiffToTriggerEvent) {
            EventBus.CameraMovement(this);
            prevPosition = transform.position;
            prevRotation = transform.rotation;
        }
    }

    private void SetFocusedLane(Lane lane) {
        LTWLogger.Log("Setting focused lane!");
        FocusedLane = lane;
        LaneIndex = LaneSystem.Singleton.LaneIDByLane[FocusedLane];
        FocusOn(FocusedLane.transform.position);
    }

    private void LockMovement() {
        doMovement = false;
    }

    private void UnlockMovement() {
        doMovement = true;
    }

    public void FocusOn(Vector3 location) {
        Vector3 cameraPosition = location;
        cameraPosition.y = Settings.CameraHeight.Value;

        // If it is 90, just leave the Z value at the location since we're looking straight down.
        if (Settings.CameraFieldOfViewAngle.Value != 90) {
            #region Drawing
            /*     
             *                        Camera
             *                         /|
             *                        / | <- angle for calculation = 90 - Settings.instance.CameraAngle
             *                       /  | SOH CAH TOA --- tan(x) = opp/hyp
             *                      /   |
             *                     /    | heightOffset
             *                    /     | (hypotenuse)
             *                   /      |
             *                  /       |
             *                 /        |
             * focusing on -> /_________|
             *                   (opp)
             *       Z distance of camera from focus
             * 
             */
            #endregion

            float distanceFromLocation = Settings.CameraHeight.Value *
                                         Mathf.Tan(Mathf.Deg2Rad * (90f - Settings.CameraFieldOfViewAngle.Value));
            Vector3 adjustmentPreRotation = new Vector3(0, 0, distanceFromLocation);
            Vector3 adjustmentPostRotation =
                Quaternion.Euler(0, Settings.CameraRotation.Value, 0) * adjustmentPreRotation;
            cameraPosition -= adjustmentPostRotation;
        }

        transform.rotation = Quaternion.Euler(
            Settings.CameraFieldOfViewAngle.Value, 
            Settings.CameraRotation.Value,
            0f
        );
        
        transform.position = cameraPosition;
    }

    private void UpdateCameraFOV(int fovAngle) {
        Quaternion curRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(
            Settings.CameraFieldOfViewAngle.Value,
            curRotation.eulerAngles.y,
            curRotation.eulerAngles.z
        );
    }

    private void UpdateCameraHeight(int height) {
        Vector3 curPos = transform.position;
        transform.position = new Vector3(
            curPos.x,
            height,
            curPos.z
        );
    }

    private void UpdateCameraRotation(int rotation) {
        Quaternion curRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(
            curRotation.eulerAngles.x,
            Settings.CameraRotation.Value,
            curRotation.eulerAngles.z
        );
    }
}
