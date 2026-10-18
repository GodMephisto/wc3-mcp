---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/SendMenu/SendMenuInterface.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class SendMenuInterface : InterfaceState,
                                 IHandleKeyDownInput,
                                 IHandleKeyContinuousInput {
    
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;
    
    [SerializeField] private SendMenuTavern[] allTaverns;
    
    [SerializeField] private Button backButton;
    
    private SendMenuTavern ActiveTavern { get; set; }
    
    public HashSet<KeyCode> KeyDownSubscriptions { get; private set; }
    public HashSet<KeyCode> KeyContinuousSubscriptions { get; private set; }
    private Dictionary<KeyCode, KeyPressSubscriptionHandler> ActiveTavernHotkeys {
        get;
        set;
    }
    
    private KeyCode MostRecentKeyCodeHandled { get; set; }
    private float TimeOfMostRecentKeyCodeFirstHandling { get; set; }
    private float TimeOfMostRecentKeyCodeLastSendRequest { get; set; }
    
    private void Awake() {
        KeyDownSubscriptions = new HashSet<KeyCode>() {
            KeyCode.Escape
        };
        KeyContinuousSubscriptions = new HashSet<KeyCode>();
        foreach (
            Setting<KeyCode> creepSendHotkeySetting
            in Settings.SendCreepHotkeys
        ) {
            KeyDownSubscriptions.Add(creepSendHotkeySetting.Value);
            KeyContinuousSubscriptions.Add(creepSendHotkeySetting.Value);
        }

        MostRecentKeyCodeHandled = KeyCode.None;
        TimeOfMostRecentKeyCodeFirstHandling = Time.time;
        TimeOfMostRecentKeyCodeLastSendRequest = Time.time;
        
        backButton.onClick.AddListener(BackButtonPressed);

        SendUnitOption.OnSendUnitPressed += SendUnit;
    }

    private void OnDestroy() {
        SendUnitOption.OnSendUnitPressed -= SendUnit;
    }
    
    public bool HandleInputKeyDown(KeyCode kc) {
        if (kc == KeyCode.Escape) {
            return BackPressed();
        }

        if (
            !ActiveTavernHotkeys.TryGetValue(
                kc,
                out KeyPressSubscriptionHandler handler
            )
        ) {
            return false;
        }

        if (!handler()) {
            return false;
        }

        MostRecentKeyCodeHandled = kc;
        TimeOfMostRecentKeyCodeFirstHandling = Time.time;
        TimeOfMostRecentKeyCodeLastSendRequest = Time.time;
        return true;
    }

    public bool HandleInputKeyContinuous(
        KeyCode kc
    ) {
        if (kc != MostRecentKeyCodeHandled) {
            return true;
        }

        if (
            !ActiveTavernHotkeys.TryGetValue(
                kc,
                out KeyPressSubscriptionHandler handler
            )
        ) {
            return false;
        }

        float timeSinceFirstHandling =
            Time.time - TimeOfMostRecentKeyCodeFirstHandling;
        if (
            timeSinceFirstHandling <
            Settings.ContinuousCreepSendHotkeyActivationTime.Value
        ) {
            return true;
        }

        float timeSinceLastSend =
            Time.time - TimeOfMostRecentKeyCodeLastSendRequest;
        if (
            timeSinceLastSend <
            Settings.ContinuousCreepSendHotkeyFrequency.Value
        ) {
            return true;
        }

        if (!handler()) {
            return true;
        }

        TimeOfMostRecentKeyCodeLastSendRequest = Time.time;
        return true;
    }
    
    protected override void IngestTransitionData(InterfaceTransitionData transitionData) {
        if (!(transitionData is SendMenuInterfaceTransitionData buildData)) {
            LTWLogger.LogError("The transition data passed into SendMenuInterface was incorrectly typed");
            return;
        }

        LTWLogger.Log($"Activating new tavern: index={buildData.TavernIndex} ");
        ActivateNewTavern(buildData.TavernIndex);
    }

    public void ActivateNewTavern(int tavernIndex) {
        if (tavernIndex >= allTaverns.Length) {
            LTWLogger.LogError($"Could not open tavern with index {tavernIndex}!");
            return;
        }

        if (ActiveTavern != null) {
            ActiveTavern.gameObject.SetActive(false);
        }
        
        ActiveTavern = allTaverns[tavernIndex];
        SetHotkeys(ActiveTavern);
        ActiveTavern.gameObject.SetActive(true);
    }

    private void SetHotkeys(SendMenuTavern tavern) {
        if (tavern == null) {
            return;
        }

        LTWLogger.Log("Clearing active tavern hotkeys!");
        ActiveTavernHotkeys ??= new Dictionary<KeyCode, KeyPressSubscriptionHandler>();
        ActiveTavernHotkeys.Clear();

        SendUnitOption[] creepSendOptions = tavern.CreepSendOptions;
        for (
            int i = 0;
            i < Math.Min(creepSendOptions.Length, Settings.SendCreepHotkeys.Count);
            i++
        ) {
            KeyCode hotkey = Settings.SendCreepHotkeys[i].Value;
            LTWLogger.Log($"Setting hotkey: {hotkey}");
            creepSendOptions[i].SetHotkey(hotkey);
            ActiveTavernHotkeys.Add(hotkey, creepSendOptions[i].SendClicked);
        }
    }

    private void SendUnit(EnemyType type) {
        if (
            EnemyConstants.GoldCost[type] > ClientLaneTracker.Singleton.MyLane.Gold
            || ClientLaneTracker.Singleton.MyLane.Stock.GetStockForCreep(type) < 1
        ) {
            // TODO: Play sound

            return;
        }

        ClientSend.RequestSendCreep(type, ClientLaneTracker.Singleton.MyLane);
    }

    private void BackButtonPressed() => BackPressed();
    private bool BackPressed() => Close();

    protected override void OnHide() {
        LTWLogger.Log("Send menu hide");
    }
}
