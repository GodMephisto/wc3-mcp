---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Util/ClientUtil.cs"
ref: HEAD
---

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClientUtil : MonoBehaviour {
    // Gets the ground position below a location
    private const float MaxDistance = 10f;
    private static readonly int GroundLayerMask =
        LayerMaskConstants.EnvironmentLayerMask | LayerMaskConstants.LaneLayerMask;
    
    public static Vector3 GetGround(Vector3 origin) {
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, MaxDistance, GroundLayerMask);

        if (hits.Length == 0) {
            LTWLogger.Log("Could not get a ground point!");
            return origin;
        }

        RaycastHit currentClosest = hits[0];
        foreach (RaycastHit hit in hits) {
            if (currentClosest.distance > hit.distance) {
                currentClosest = hit;
            }
        }

        return currentClosest.point;
    }

    public static Collider GetColliderClosestToPoint(Collider[] colliders, Vector3 point) {
        Collider closest = null;
        bool foundOneInside = false;
        float closestDistance = Mathf.Infinity;
        foreach (Collider collider in colliders) {
            float distance = Vector3.Distance(collider.ClosestPoint(point), point);
            bool isInside = false;
            if (distance <= Mathf.Epsilon) {
                foundOneInside = true;
                isInside = true;
                distance = Vector3.Distance(collider.transform.position, point);
            }

            if (closest == null || (foundOneInside && isInside && distance < closestDistance) || (!foundOneInside && distance < closestDistance)) {
                closest = collider;
                closestDistance = distance;
            }
        }

        return closest;
    }

    public static Vector3 GetCenterPoint(Vector3[] points) {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        
        foreach (Vector3 point in points) {
            x += point.x;
            y += point.y;
            z += point.z;
        }

        int numPoints = points.Length;
        return new Vector3(
            x / numPoints,
            y / numPoints,
            z / numPoints
        );
    }

    private static readonly Dictionary<KeyCode, string> KeyCodeStringMappings = new Dictionary<KeyCode, string>() {
        { KeyCode.Alpha1, "1" },
        { KeyCode.Alpha2, "2" },
        { KeyCode.Alpha3, "3" },
        { KeyCode.Alpha4, "4" },
        { KeyCode.Alpha5, "5" },
        { KeyCode.Alpha6, "6" },
        { KeyCode.Alpha7, "7" },
        { KeyCode.Alpha8, "8" },
        { KeyCode.Alpha9, "9" },
        { KeyCode.Alpha0, "0" },
        { KeyCode.Keypad1, "K1" },
        { KeyCode.Keypad2, "K2" },
        { KeyCode.Keypad3, "K3" },
        { KeyCode.Keypad4, "K4" },
        { KeyCode.Keypad5, "K5" },
        { KeyCode.Keypad6, "K6" },
        { KeyCode.Keypad7, "K7" },
        { KeyCode.Keypad8, "K8" },
        { KeyCode.Keypad9, "K9" },
        { KeyCode.Keypad0, "K0" },
        { KeyCode.BackQuote, "`" },
        { KeyCode.Backslash, "\\" },
        { KeyCode.Minus, "-" },
        { KeyCode.Equals, "=" }
    };

    private static readonly Color fullGreen = new Color(0.3207f, 0.8490f, 0f, 255f);
    private static readonly Color fullYellow = new Color(0.4235f, 0.4185f, 0f, 255f);
    private static readonly Color fullRed = new Color(0.7f, 0.1432f, 0f, 255f);
    public static Color GetHealthBarColor(float healthRatio) {
        Color color;
        if (healthRatio > 0.5f) { // > 50% hp
            float lerpCoefficient = (healthRatio - 0.5f) / 0.5f;
            color = Color.Lerp(fullYellow, fullGreen, lerpCoefficient);
        } else {
            float lerpCoefficient = healthRatio / 0.5f;
            color = Color.Lerp(fullRed, fullYellow, lerpCoefficient);
        }

        return color;
    }

    public static string GetRandomString(string[] strings, System.Random rng) {
        if (strings.Length == 0) {
            return "";
        }

        return strings[rng.Next(strings.Length)];
    }

    public static string SecondsToString(float time) {
        int seconds = (int)Mathf.Round(time);
        int minutes = seconds / 60;
        seconds -= minutes * 60;

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public static Vector3 DirectionFromPointToPoint(Vector3 origin, Vector3 dest) {
        return (dest - origin).normalized;
    }

    public static string GetKeyCodeStringRepresentation(KeyCode kc) {
        if (KeyCodeStringMappings.ContainsKey(kc)) {
            return KeyCodeStringMappings[kc];
        }

        return kc.ToString();
    }

    public static T GetOnlyValueFromHashSet<T>(HashSet<T> set) {
        foreach (T val in set) {
            return val;
        }

        throw new Exception("There were no values in the provided HashSet");
    }
    
    // Duplicated from ServerUtil... TODO: Merge into a shared util
    public static Vector3 GetMidpointOfTransforms(Transform[] transforms) {
        float totalX = 0f;
        float totalY = 0f;
        float totalZ = 0f;

        foreach (Transform t in transforms) {
            Vector3 position = t.position;
            
            totalX += position.x;
            totalY += position.y;
            totalZ += position.z;
        }

        return new Vector3(
            totalX / transforms.Length,
            totalY / transforms.Length,
            totalZ / transforms.Length
        );
    }

    // From https://www.youtube.com/watch?v=JOABOQMurZo
    // ----
    // Since it is expensive to get Camera.main often...
    private static Camera _camera;
    public static Camera Camera {
        get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }
    
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    public static bool IsMousePointerOverUI() {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) {position = Input.mousePosition};
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    // Great for spawning 3D objects on the canvas, like for Lobby builder preview
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element) {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            element,
            element.position,
            Camera,
            out Vector3 result
        );
        return result;
    }
    // ----
}
