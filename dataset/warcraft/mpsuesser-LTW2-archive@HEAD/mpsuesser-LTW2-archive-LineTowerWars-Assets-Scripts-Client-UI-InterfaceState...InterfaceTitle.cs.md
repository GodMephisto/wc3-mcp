---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/InterfaceTitle.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

class InterfaceTitle : MonoBehaviour {
    private TMP_Text titleText;

    private void Awake() {
        titleText = GetComponent<TMP_Text>();
    }
    
    private void Start() {
        EventBus.OnSetActiveInterfaceState += UpdateTitle;
    }

    private void OnDestroy() {
        EventBus.OnSetActiveInterfaceState -= UpdateTitle;
    }

    private void UpdateTitle(InterfaceState state) {
        titleText.SetText(state.Title);
    }
}
