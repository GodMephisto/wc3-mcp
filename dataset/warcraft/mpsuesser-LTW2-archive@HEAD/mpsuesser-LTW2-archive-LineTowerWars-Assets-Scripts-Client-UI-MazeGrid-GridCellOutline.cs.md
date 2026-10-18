---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/MazeGrid/GridCellOutline.cs"
ref: HEAD
---

using UnityEngine;

public class GridCellOutline : MonoBehaviour
{
    public static GridCellOutline Create(Vector3 pos, Transform parent) {
        GridCellOutline outline = Instantiate(ClientPrefabs.Singleton.pfGridCellOutline, pos, Quaternion.identity, parent);

        return outline;
    }
}
