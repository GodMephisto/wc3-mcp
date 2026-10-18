---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/ElementalTechMenu/UpgradeMenuInterface.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    public HashSet<KeyCode> KeyDownSubscriptions { get; private set; }
    
    [SerializeField] private Button BackButton;
    [SerializeField] private ElementalTechOption[] TechOptions;

    private void Awake() {
        KeyDownSubscriptions = new HashSet<KeyCode>() {
            KeyCode.Escape
        };
        
        for (int i = 0; i < TechOptions.Length; i++) {
            TechOptions[i].OnPushed += TechOptionPushed;
            // TEMP
            // TODO: Implement hotkeys through settings
            TechOptions[i].SetHotkey(KeyCode.Minus);
        }
        
        BackButton.onClick.AddListener(BackButtonPressed);
    }

    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Escape => BackPressed(),
            _ => false
        };
    }

    private void Start() {
        ClientLaneTracker.Singleton.SubscribeToOnTechUpdated(TechUpdated);

        if (ClientLaneTracker.Singleton.MyLane != null) {
            TechUpdated(ClientLaneTracker.Singleton.MyLane);
        }
    }

    private static void TechOptionPushed(ElementalTechType techType) {
        AttemptToUpgradeElement(techType);
    }

    private static void AttemptToUpgradeElement(ElementalTechType upgradeType) {
        Lane lane = ClientLaneTracker.Singleton.MyLane;
        if (!ClientElementalTechSystem.Singleton.LaneFulfillsRequirementsForPurchase(lane, upgradeType)) {
            LTWLogger.Log($"Lane did not fulfill requirements for purchase!");
            LTWLogger.Log("TODO: Play a sound.");
            return;
        }

        ClientSend.RequestPurchaseElementTech(lane, upgradeType);
    }

    private void TechUpdated(Lane lane) {
        foreach (ElementalTechOption techOption in TechOptions) {
            techOption.UpdateStateByLane(lane);
        }
    }

    private void BackButtonPressed() => BackPressed();
    private bool BackPressed() => Close();
}
