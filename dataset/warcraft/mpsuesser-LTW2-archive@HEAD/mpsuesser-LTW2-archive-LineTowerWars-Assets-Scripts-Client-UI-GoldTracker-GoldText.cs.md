---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/GoldTracker/GoldText.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

public class GoldText : MonoBehaviour
{
    private int CurrentValue { get; set; }

    private TMP_Text Text { get; set; }

    // Animation
    [SerializeField] private Color ObtainedGoldAnimationColor;
    private Color OriginalColor { get; set; }
    private bool animationActive;
    private float animationTimeRemaining;
    private static readonly float TotalAnimationTime = 1f;


    private void Awake() {
        CurrentValue = -1;

        Text = GetComponent<TMP_Text>();
        OriginalColor = Text.color;
    }

    public void SetValue(int val) {
        if (val > CurrentValue && CurrentValue != -1) {
            PlayObtainedAnimation();
        }

        CurrentValue = val;

        Text.SetText(val.ToString());
    }

    private void PlayObtainedAnimation() {
        animationTimeRemaining = TotalAnimationTime;
        animationActive = true;
    }

    private void Update() {
        if (animationActive) {
            animationTimeRemaining -= Time.deltaTime;
            if (animationTimeRemaining < 0f) {
                animationActive = false;
                Text.color = OriginalColor;
            } else {
                Text.color = Color.Lerp(ObtainedGoldAnimationColor, OriginalColor, (TotalAnimationTime - animationTimeRemaining) / TotalAnimationTime);
            }
        }
    }
}
