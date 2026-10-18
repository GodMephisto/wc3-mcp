---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Constants/Constants.cs"
ref: HEAD
---

using UnityEngine;

public class Constants : SingletonBehaviour<Constants>
{
    private void Awake() {
        InitializeSingleton(this);

        PullConstants();
    }

    private void PullConstants() {
        // TODO: Combine all different constants files under this singleton umbrella, non-static since will be pulled from PlayFab
        // TODO: Move constants values to PlayFab and pull them from there
    }
}
