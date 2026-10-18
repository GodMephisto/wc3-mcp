---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Preload/PreloadSystem/PreloadSystem.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadSystem : MonoBehaviour
{
    private void Start() {
        Invoke(nameof(OpenMainMenu), .5f);
    }

    private void OpenMainMenu() {
        SceneSystem.Singleton.LoadMainMenu();
    }
}
