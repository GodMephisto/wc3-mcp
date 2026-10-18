---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/MazeGrid/GridRowNumber.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

public class GridRowNumber : MonoBehaviour {
    public static GridRowNumber Create(int num, Vector3 pos, Transform parent) {
        GridRowNumber rowNumber =
            Instantiate(ClientPrefabs.Singleton.pfGridRowNumber, pos, Quaternion.identity, parent);
        rowNumber.SetNumber(num);
        
        return rowNumber;
    }

    private TMP_Text NumberText { get; set; }

    private void Awake() {
        NumberText = GetComponentInChildren<TMP_Text>(true);

        Settings.CameraRotation.Updated += RotateText;
        RotateText(Settings.CameraRotation.Value);
    }

    private void OnDestroy() {
        Settings.CameraRotation.Updated -= RotateText;
    }

    private void RotateText(int rotation) {
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void SetNumber(int num) {
        NumberText.SetText(num.ToString());
    }
}
