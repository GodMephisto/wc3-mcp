---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Options/OptionsContent.cs"
ref: HEAD
---

using UnityEngine;

public class OptionsContent : MonoBehaviour {
    [SerializeField] private TowerBaseAbilities baseAbilities;
    [SerializeField] private TowerUpgradeOptions upgradeOptions;

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void LoadAndShowForEntity(ClientEntity e) {
        baseAbilities.LoadForEntity(e);
        upgradeOptions.LoadForEntity(e);

        gameObject.SetActive(true);
    }
}
