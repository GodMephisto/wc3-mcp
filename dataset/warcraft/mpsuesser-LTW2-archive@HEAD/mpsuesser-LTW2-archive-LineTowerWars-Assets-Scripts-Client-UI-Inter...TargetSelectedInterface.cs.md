---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/TargetSelected/TargetSelectedInterface.cs"
ref: HEAD
---

﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectedInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    private static readonly HashSet<KeyCode> baseKeyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Escape,
        };
    
    public HashSet<KeyCode> KeyDownSubscriptions { get; set; }
    private Dictionary<KeyCode, TowerUpgrade> ActiveTowerUpgradeActions { get;
        set;
    }

    protected override void Initialize() {
        if (Initialized) return;
        
        KeyDownSubscriptions = new HashSet<KeyCode>();
        ActiveTowerUpgradeActions = new Dictionary<KeyCode, TowerUpgrade>();
        ResetLoadedActions();

        EventBus.OnUpgradePressed += HandleUpgradeAction;
        EventBus.OnSellPressed += HandleSellAction;
        EventBus.OnOpenBuildMenuPressed += HandleOpenBuildMenuAction;
        
        base.Initialize();
    }

    private void OnDestroy() {
        EventBus.OnUpgradePressed -= HandleUpgradeAction;
        EventBus.OnSellPressed -= HandleSellAction;
        EventBus.OnOpenBuildMenuPressed -= HandleOpenBuildMenuAction;
    }

    protected override void OnShow() {
        EventBus.OnTargetsUpdated += HandleTargetsUpdated;
    }

    protected override void OnHide() {
        EventBus.OnTargetsUpdated -= HandleTargetsUpdated;
    }

    private void HandleTargetsUpdated(List<ClientEntity> targets) {
        LoadTargets(targets);
    }

    public bool HandleInputKeyDown(KeyCode kc) {
        if (kc == KeyCode.Escape) {
            return BackPressed();
        }

        if (kc == Settings.AttackHotkey.Value) {
            HandleAttackAction();
            return true;
        }

        if (kc == Settings.MoveHotkey.Value) {
            HandleMoveAction();
            return true;
        }

        if (kc == Settings.BuildHotkey.Value) {
            HandleOpenBuildMenuAction();
            return true;
        }

        if (kc == Settings.SellHotkey.Value) {
            HandleSellAction();
            return true;
        }

        if (
            ActiveTowerUpgradeActions.TryGetValue(
                kc,
                out TowerUpgrade upgrade
            )
        ) {
            HandleUpgradeAction(upgrade);
            return true;
        }
        
        return false;
    }

    private bool BackPressed() => Close();

    // TODO: This is also a hacky workaround to TargetSystem's naive clearing
    // of targets if the **active** interface is not TargetSelected. When
    // TargetSelected is the one being loaded, the flow goes...
    //   set targets
    //   -> set target event triggers TargetSelectedInterface load
    //   -> close all mutually exclusive interfaces in the stack
    //     -> which triggers TargetSystem to clear targets
    //   -> load TargetSelected as the active interface
    protected override void IngestTransitionData(InterfaceTransitionData transitionData) {
        if (!(transitionData is TargetSelectedInterfaceTransitionData targetSelectedData)) {
            LTWLogger.LogError("The transition data passed into TargetSelectedInterface was incorrectly typed");
            return;
        }

        LoadTargets(targetSelectedData.Targets);
    }

    private void LoadTargets(
        List<ClientEntity> targets
    ) {
        TargetSystem.Singleton.SelectEntities(targets);
        if (targets.Count > 0) {
            PopulateActionsForTarget(targets[0]);
        }
    }

    private void PopulateActionsForTarget(ClientEntity target) {
        LTWLogger.Log($"Populating actions for target: {target.gameObject.name}");
        ResetLoadedActions();
        
        switch (target) {
            case ClientTower tower:
                LTWLogger.Log("Tower...");
                if (!(tower is ClientTechnologyDisc)) {
                    LoadAttackAction();
                }
                LoadSellAction();
                LoadUpgradeActions(tower);
                
                break;
            
            case ClientEnemy creep:
                LTWLogger.Log("Creep...");
                if (creep.ActiveLane == ClientLaneTracker.Singleton.MyLane &&
                    TraitConstants.EnemyTraitMap[creep.Type]
                        .Contains(TraitType.Attacker)) {
                    LoadMoveAction();
                    LoadAttackAction();
                }
                break;
            
            case ClientBuilder builder:
                LTWLogger.Log("Builder...");
                LoadMoveAction();
                LoadAttackAction();
                LoadOpenBuildMenuAction();
                break;
        }
    }

    private void ResetLoadedActions() {
        LTWLogger.Log("Resetting loaded actions!");
        KeyDownSubscriptions.Clear();
        foreach (KeyCode kc in baseKeyDownSubscriptions) {
            KeyDownSubscriptions.Add(kc);
        }

        ActiveTowerUpgradeActions.Clear();
    }

    private void LoadMoveAction() {
        LTWLogger.Log($"Move hotkey: {Settings.MoveHotkey.Value}");
        KeyDownSubscriptions.Add(Settings.MoveHotkey.Value);
    }

    private void LoadAttackAction() {
        KeyDownSubscriptions.Add(Settings.AttackHotkey.Value);
    }

    private void LoadSellAction() {
        KeyDownSubscriptions.Add(Settings.SellHotkey.Value);
        // TODO: Create button and set onListen
    }

    private void LoadUpgradeActions(ClientTower tower) {
        List<TowerUpgrade> availableUpgrades =
            TowerUpgrades.Singleton.GetAvailableUpgradesForTowerType(tower.Type);

        for (int i = 0; i < availableUpgrades.Count; i++) {
            ActiveTowerUpgradeActions.Add(
                Settings.UpgradeHotkeys[i].Value,
                availableUpgrades[i]
            );
            KeyDownSubscriptions.Add(Settings.UpgradeHotkeys[i].Value);
            // TODO: Create button and set onListen
        }
    }

    private void LoadOpenBuildMenuAction() {
        KeyDownSubscriptions.Add(Settings.BuildHotkey.Value);
        // TODO: Create button and set onListen
    }

    private static void HandleMoveAction() {
        LTWLogger.Log("Handling move action...");
        Vector3 aimPoint = CameraController.Singleton.GetAimPoint();
        bool isQueuedAction = Input.GetKey(KeyCode.LeftShift);
        
        // TODO: Replace this once the client-server Request abstraction is implemented.
        if (!isQueuedAction) {
            TowerBuildProjection.ClearAll();
        }
        
        ClientSend.RequestEntitiesMove(
            TargetSystem.Singleton.Targets,
            aimPoint,
            isQueuedAction
        );
        VisualEffectManager.CreateEntityMovementCommandIndicator(aimPoint);
    }

    private static void HandleAttackAction() {
        bool isQueuedAction = Input.GetKey(KeyCode.LeftShift);
        
        // TODO: Replace this once the client-server Request abstraction is implemented.
        if (!isQueuedAction) {
            TowerBuildProjection.ClearAll();
        }

        HashSet<ClientEntity> hoverUnits = TargetSystem.Singleton.HoverUnits;
        if (hoverUnits.Count > 0) {
            ClientSend.RequestEntitiesAttackTarget(
                TargetSystem.Singleton.Targets,
                ClientUtil.GetOnlyValueFromHashSet(
                    new HashSet<ClientEntity>(hoverUnits)
                ),
                isQueuedAction
            );
        }
        else {
            Vector3 aimPoint = CameraController.Singleton.GetAimPoint();
            ClientSend.RequestEntitiesAttackLocation(
                TargetSystem.Singleton.Targets,
                aimPoint,
                isQueuedAction
            );
            VisualEffectManager.CreateEntityAttackCommandIndicator(aimPoint);
        }
    }

    private static void HandleSellAction() {
        List<ClientEntity> targets = TargetSystem.Singleton.Targets;
        HashSet<ClientTower> towers = new HashSet<ClientTower>();
        foreach (ClientEntity e in targets) {
            if (!(e is ClientTower t)) {
                continue;
            }

            towers.Add(t);
        }
        
        ClientSend.RequestTowerSale(towers);
    }

    private static void HandleUpgradeAction(TowerUpgrade upgrade) {
        if (TargetSystem.Singleton.Targets.Count == 0) {
            LTWLogger.LogError("Trying to handle upgrade action when there are no active targets! Should not happen!");
            return;
        }
        
        ClientEntity mainTarget = TargetSystem.Singleton.Targets[0];
        // This is all a bit contrived... trying to get the arguments right because it does seem right to pass a HS<ClientTower> to the ClientSend function
        HashSet<ClientEntity> matchingTypeTargets = 
            TargetSystem.Singleton.GetTargetsOfMatchingType(mainTarget);
        HashSet<ClientTower> filteredTowerTargets = new HashSet<ClientTower>();
        foreach (ClientEntity e in matchingTypeTargets) {
            if (e is ClientTower t) {
                filteredTowerTargets.Add(t);
            }
        }
        ClientSend.RequestTowerUpgrade(filteredTowerTargets, upgrade);
    }

    private static void HandleOpenBuildMenuAction() {
        InGameInterfaceStateSystem.Singleton.TransitionToInterface(
            InGameInterfaceStateSystem.InGameInterfaceType.BuildMenu
        );
    }
}