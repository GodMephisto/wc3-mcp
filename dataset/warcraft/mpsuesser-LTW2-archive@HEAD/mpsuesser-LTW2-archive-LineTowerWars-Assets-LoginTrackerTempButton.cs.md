---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/LoginTrackerTempButton.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

public class LoginTrackerTempButton : MonoBehaviour {
    private TMP_Text Text { get; set; }
    
    private void Awake() {
        Text = GetComponentInChildren<TMP_Text>(true);
        
        EventBus.OnLoginSuccess += UpdateText;
        EventBus.OnLoggedOut += UpdateText;
    }

    private void Start() {
        UpdateText();
    }

    private void OnDestroy() {
        EventBus.OnLoginSuccess -= UpdateText;
        EventBus.OnLoggedOut -= UpdateText;
    }

    private void UpdateText() {
        if (LoginSystem.Singleton.IsLoggedIn) {
            Text.SetText($"Log out as {LoginSystem.Singleton.ActiveUsername}");
        }
        else {
            Text.SetText("-");
        }
    }
}
