---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/TooltipInterface.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipInterface : SingletonBehaviour<TooltipInterface>
{
    [SerializeField] private StandardTooltipContents StandardContents;
    [SerializeField] private StandardTwoBodiesTooltipContents StandardTwoBodiesContents;
    [SerializeField] private UnitTooltipContents UnitContents;
    [SerializeField] private TowerTooltipContents TowerContents;
    [SerializeField] private BuildTowerTooltipContents BuildTowerContents;
    [SerializeField] private SendUnitTooltipContents SendUnitContents;
    [SerializeField] private ResearchElementTooltipContents ResearchElementContents;
    [SerializeField] private BuffTooltipContents BuffContents;
    
    private HashSet<GameObject> AllContents { get; set; }

    private Tooltip ActiveTooltip { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        AllContents = new HashSet<GameObject>() {
            StandardContents.gameObject,
            StandardTwoBodiesContents.gameObject,
            UnitContents.gameObject,
            TowerContents.gameObject,
            BuildTowerContents.gameObject,
            SendUnitContents.gameObject,
            ResearchElementContents.gameObject,
            BuffContents.gameObject,
        };
    }

    private void Start() {
        HideAll();
    }

    public void Show(Tooltip tooltip)
    {
        switch (tooltip)
        {
            case StandardTooltip st:
                StandardContents.Load(st);
                SetContentActive(StandardContents.gameObject);
                break;
            case StandardTwoBodiesTooltip stbt:
                StandardTwoBodiesContents.Load(stbt);
                SetContentActive(StandardTwoBodiesContents.gameObject);
                break;
            case UnitTooltip ut:
                UnitContents.Load(ut);
                SetContentActive(UnitContents.gameObject);
                break;
            case TowerTooltip tt:
                TowerContents.Load(tt);
                SetContentActive(TowerContents.gameObject);
                break;
            case BuildTowerTooltip btt:
                BuildTowerContents.Load(btt);
                SetContentActive(BuildTowerContents.gameObject);
                break;
            case SendUnitTooltip sut:
                SendUnitContents.Load(sut);
                SetContentActive(SendUnitContents.gameObject);
                break;
            case ResearchElementTooltip ret:
                ResearchElementContents.Load(ret);
                SetContentActive(ResearchElementContents.gameObject);
                break;
            case BuffTooltip bt:
                BuffContents.Load(bt);
                SetContentActive(BuffContents.gameObject);
                break;
        }

        ActiveTooltip = tooltip;
    }

    private void SetContentActive(GameObject objectToSetActive) {
        foreach (GameObject obj in AllContents) {
            if (obj == objectToSetActive) {
                obj.SetActive(true);
            } else {
                obj.SetActive(false);
            }
        }
    }

    private void HideAll() {
        foreach (GameObject obj in AllContents) {
            obj.SetActive(false);
        }

        ActiveTooltip = null;
    }

    public void HideIfActive(Tooltip tooltip) {
        if (IsActive(tooltip)) {
            HideAll();
        }
    }

    private bool IsActive(Tooltip tooltip) {
        return ActiveTooltip == tooltip;
    }
}
