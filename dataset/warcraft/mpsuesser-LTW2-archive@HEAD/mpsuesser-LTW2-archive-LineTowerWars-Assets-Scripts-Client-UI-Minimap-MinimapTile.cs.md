---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Minimap/MinimapTile.cs"
ref: HEAD
---

using UnityEngine;

public class MinimapTile : MonoBehaviour
{
    private ClientTower Tower { get; set; }
    private MeshRenderer MR { get; set; }

    private void Awake() {
        Tower = GetComponentInParent<ClientTower>();
        MR = GetComponent<MeshRenderer>();

        Tower.OnSetupComplete += UpdateTileColorBasedOnLane;
    }

    private void OnDestroy() {
        Tower.OnSetupComplete -= UpdateTileColorBasedOnLane;
    }

    private void UpdateTileColorBasedOnLane(ClientEntity e) {
        Material laneColorMaterial = ClientResources.Singleton.GetColorMaterialForLane(Tower.ActiveLane.ID);

        MR.material = laneColorMaterial;
    }
}
