---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Frame/MultipleTargetDisplay.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetDisplay : MonoBehaviour
{
    public void Load(List<ClientEntity> targets) {
        ClearChildren();
        foreach (ClientEntity target in targets) {
            EntityImage image = Instantiate(ClientPrefabs.Singleton.pfMultipleTargetEntityImage, transform);
            image.LoadImageForEntity(target);
        }
    }

    private void ClearChildren() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
