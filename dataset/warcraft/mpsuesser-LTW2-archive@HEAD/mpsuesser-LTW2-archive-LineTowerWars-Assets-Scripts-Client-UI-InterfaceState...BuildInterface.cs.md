---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/BuildPlacement/BuildInterface.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;

public class BuildInterface : InterfaceState, IHandleKeyDownInput
{
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    private TowerType SelectedTowerToBuild { get; set; }
    
    public HashSet<KeyCode> KeyDownSubscriptions { get; private set; }

    private HashSet<KeyCode> BuildHotkeys { get; set; }

    protected override void Initialize() {
        // To block the fallback...
        BuildHotkeys = new HashSet<KeyCode>() {
            Settings.BuildArcherHotkey.Value,
            Settings.BuildCutterHotkey.Value,
            Settings.BuildElementalCoreHotkey.Value,
            Settings.BuildTechnologyDiscHotkey.Value,
        };
        
        KeyDownSubscriptions = new HashSet<KeyCode>(BuildHotkeys) {
            KeyCode.Escape,
            KeyCode.Mouse0,
            KeyCode.Mouse1,
        };
    }
    
    public bool HandleInputKeyDown(KeyCode kc) {
        if (BuildHotkeys.Contains(kc)) {
            return true;
        }
        
        return kc switch {
            KeyCode.Escape => EscapePressed(),
            KeyCode.Mouse0 => Mouse0Pressed(),
            KeyCode.Mouse1 => Mouse1Pressed(),
            _ => false,
        };
    }

    protected override void IngestTransitionData(InterfaceTransitionData transitionData) {
        if (!(transitionData is BuildInterfaceTransitionData buildData)) {
            LTWLogger.LogError("The transition data passed into BuildInterface was incorrectly typed");
            return;
        }

        SelectedTowerToBuild = buildData.Type;
    }

    protected override void OnShow() {
        ClientSideGridSystem.Singleton.Enable();
    }

    protected override void OnHide() {
        ClientSideGridSystem.Singleton.Disable();
    }

    private bool Mouse0Pressed() {
        MazeGridCell[] selectedCells = ClientSideGridSystem.Singleton.CurrentHoverCells;
        if (selectedCells.Length != 4) {
            return false;
        }

        foreach (MazeGridCell cell in selectedCells) {
            if (cell.Occupied) {
                return false;
            }
        }

        bool isQueuedAction = Input.GetKey(KeyCode.LeftShift);

        ClientSend.RequestBuildTower(
            SelectedTowerToBuild,
            ClientLaneTracker.Singleton.MyLane,
            ClientSideGridSystem.Singleton.CurrentHoverCells,
            isQueuedAction
        );
        
        // There are problems here because the request can be denied for any number of
        // reasons. It seemed a waste of work to implement individualized messages for
        // "request to build tower" accepted when a client-server Request abstraction is
        // planned, so we'll just use this leaky workaround for now.
        //
        // TODO: Replace this once the client-server Request abstraction is implemented.
        if (!isQueuedAction) {
            TowerBuildProjection.ClearAll();
        }
        
        VisualEffectManager.CreateBuildProjection(
            SelectedTowerToBuild,
            ClientSideGridSystem.Singleton.CurrentHoverCells
        );

        return Close();
    }

    private bool Mouse1Pressed() => Close();
    private bool EscapePressed() => Close();
}
