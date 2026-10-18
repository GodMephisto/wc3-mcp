---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoveringEntityCanvas/HoveringEntityCanvas.cs"
ref: HEAD
---

using UnityEngine;

public class HoveringEntityCanvas : MonoBehaviour
{
    private Transform EntityCanvasTransform { get; set; }
    private Transform CameraTransform { get; set; }
    
    private float CameraYRotation { get; set; }

    private void Awake() {
        EntityCanvasTransform = GetComponent<Canvas>().transform;

        CameraTransform = CameraController.Singleton.transform;
    }

    private void Update() {
        AdjustCanvasVisibility();
    }

    private void AdjustCanvasVisibility() {
        Vector3 direction = (EntityCanvasTransform.position - CameraTransform.position).normalized;
        Vector3 lookRotation = Quaternion.LookRotation(direction).eulerAngles;
        EntityCanvasTransform.rotation = Quaternion.Euler(
            lookRotation.x,
            CameraTransform.rotation.eulerAngles.y,
            0
        );
    }
}
