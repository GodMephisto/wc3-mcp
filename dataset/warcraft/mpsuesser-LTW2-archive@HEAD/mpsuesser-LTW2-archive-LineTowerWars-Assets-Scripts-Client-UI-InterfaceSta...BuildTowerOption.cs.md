---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/BuildMenu/BuildTowerOption.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BuildTowerOption : HoverTooltip {
    public event Action<TowerType> OnTowerBuildPressed;

    [SerializeField] public KeyCode Hotkey;
    [SerializeField] private TowerType TowerTypeToBuild;

    private Image Img { get; set; }
    private Button Btn { get; set; }
    private TMP_Text HotkeyText { get; set; }

    protected override void Awake() {
        Img = GetComponentInChildren<Image>(true);
        Btn = GetComponentInChildren<Button>(true);
        HotkeyText = GetComponentInChildren<TMP_Text>(true);

        Btn.onClick.AddListener(ButtonClicked);
        HotkeyText.SetText(ClientUtil.GetKeyCodeStringRepresentation(Hotkey));

        base.Awake();
    }

    protected override void Start() {
        Img.sprite = ClientResources.Singleton.GetSpriteForTowerType(TowerTypeToBuild);

        base.Start();
    }

    public void ButtonClicked() {
        OnTowerBuildPressed?.Invoke(TowerTypeToBuild);
    }

    protected override Tooltip GetTooltipContent() {
        return new BuildTowerTooltip(TowerTypeToBuild);
    }
}
