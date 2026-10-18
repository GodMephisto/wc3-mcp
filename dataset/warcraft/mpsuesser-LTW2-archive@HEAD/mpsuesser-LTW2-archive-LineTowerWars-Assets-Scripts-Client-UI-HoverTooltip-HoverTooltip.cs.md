---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/HoverTooltip.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    protected abstract Tooltip GetTooltipContent();

    private TooltipInterface TTI { get; set; }

    private Tooltip ActiveTooltip { get; set; }

    protected virtual void Awake() {
        ActiveTooltip = null;
    }

    protected virtual void Start() {
        TTI = TooltipInterface.Singleton;
    }

    public void OnPointerEnter(PointerEventData data) {
        ActiveTooltip = GetTooltipContent();
        TTI.Show(ActiveTooltip);
    }

    public void OnPointerExit(PointerEventData data) {
        TTI.HideIfActive(ActiveTooltip);
        ActiveTooltip = null;
    }

    protected virtual void OnDisable() {
        Hide();
    }

    protected virtual void OnDestroy() {
        Hide();
    }

    private void Hide() {
        if (ActiveTooltip != null) {
            TTI.HideIfActive(ActiveTooltip);
        }
    }
}