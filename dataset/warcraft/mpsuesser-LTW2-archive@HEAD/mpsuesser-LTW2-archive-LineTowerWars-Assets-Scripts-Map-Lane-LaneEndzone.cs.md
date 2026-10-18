---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Map/Lane/LaneEndzone.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneEndzone : MonoBehaviour
{
    private Collider C { get; set; }

    private void Awake() {
        C = GetComponent<Collider>();
    }

    public Vector3 GetClosestDestinationTo(Vector3 location) {
        return C.ClosestPoint(location);
    }
}