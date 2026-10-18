---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Visuals/SelectionSpriteIndicator.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSpriteIndicator : MonoBehaviour
{
    [SerializeField] private Transform Indicator;
    private ClientEntity Parent { get; set; }

    private static float Offset = .05f;

    private void Awake() {
        Parent = null;
    }

    public void SetParent(ClientEntity parent) {
        Parent = parent;
    }

    private void LateUpdate() {
        if (Parent == null) {
            return;
        }

        Indicator.position = new Vector3(
            Parent.transform.position.x,
            ClientUtil.GetGround(Parent.transform.position).y + Offset,
            Parent.transform.position.z
        );
    }
}
