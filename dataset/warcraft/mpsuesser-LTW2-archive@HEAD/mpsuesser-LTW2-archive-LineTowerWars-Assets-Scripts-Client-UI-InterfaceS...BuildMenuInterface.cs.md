---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/BuildMenu/BuildMenuInterface.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;

    [SerializeField] private BuildTowerOption archerBuildOption;
    [SerializeField] private BuildTowerOption cutterBuildOption;
    [SerializeField] private BuildTowerOption elementalCoreBuildOption;
    [SerializeField] private BuildTowerOption technologyDiscBuildOption;
    
    [SerializeField] private Button BackButton;

    private Dictionary<KeyCode, BuildTowerOption> towerOptionKeyMap;
    
    public HashSet<KeyCode> KeyDownSubscriptions { get; private set; }
    
    protected override void Initialize() {
        if (Initialized) return;
        
        KeyDownSubscriptions = new HashSet<KeyCode>() {
            KeyCode.Escape,
            Settings.BuildArcherHotkey.Value,
            Settings.BuildCutterHotkey.Value,
            Settings.BuildElementalCoreHotkey.Value,
            Settings.BuildTechnologyDiscHotkey.Value,
            
            /**
             * This is to block the fallback. If this wasn't here, the build
             * hotkey would get caught by the interface below this interface,
             * which results in a second BuildMenuInterface being opened on top
             * of the one we currently have open.
             */
            Settings.BuildHotkey.Value,
        };

        towerOptionKeyMap = new Dictionary<KeyCode, BuildTowerOption>() {
            {
                Settings.BuildArcherHotkey.Value, archerBuildOption
            }, {
                Settings.BuildCutterHotkey.Value, cutterBuildOption
            }, {
                Settings.BuildElementalCoreHotkey.Value,
                elementalCoreBuildOption
            }, {
                Settings.BuildTechnologyDiscHotkey.Value,
                technologyDiscBuildOption
            },
        };
        
        archerBuildOption.OnTowerBuildPressed += BuildTower;
        cutterBuildOption.OnTowerBuildPressed += BuildTower;
        elementalCoreBuildOption.OnTowerBuildPressed += BuildTower;
        technologyDiscBuildOption.OnTowerBuildPressed += BuildTower;
        
        BackButton.onClick.AddListener(CancelButtonPressed);
        
        base.Initialize();
    }
    
    public bool HandleInputKeyDown(KeyCode kc) {
        if (kc == KeyCode.Escape) {
            return CancelPressed();
        }

        if (kc == Settings.BuildHotkey.Value) {
            return true;
        }

        if (towerOptionKeyMap.ContainsKey(kc)) {
            towerOptionKeyMap[kc].ButtonClicked();
            return true;
        }

        return false;
    }

    private static void BuildTower(TowerType type) {
        EngageBuildInterface(type);
    }

    private static void EngageBuildInterface(TowerType towerType) {
        if (TowerConstants.BuildCost[towerType] > ClientLaneTracker.Singleton.MyLane.Gold) {
            // TODO: Play sound

            return;
        }

        BuildInterfaceTransitionData transitionData =
            new BuildInterfaceTransitionData(towerType);
        
        InGameInterfaceStateSystem.Singleton.TransitionToInterfaceWithData(
            InGameInterfaceStateSystem.InGameInterfaceType.Build,
            transitionData
        );
    }

    private void CancelButtonPressed() {
        CancelPressed();
    }
    
    private bool CancelPressed() {
        Close();
        return true;
    }
}
