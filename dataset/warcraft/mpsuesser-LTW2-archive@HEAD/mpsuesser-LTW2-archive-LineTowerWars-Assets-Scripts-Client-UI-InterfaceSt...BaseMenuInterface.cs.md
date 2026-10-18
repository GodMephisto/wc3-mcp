---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/BaseMenu/BaseMenuInterface.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class BaseMenuInterface : InterfaceState,
                                 IHandleKeyDownInput,
                                 IHandleKeyUpInput,
                                 IHandleKeyContinuousInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        false;

    [SerializeField] private Button selectBuilderButton;
    [SerializeField] private Button openTavern1Button;
    [SerializeField] private Button openTavern2Button;
    [SerializeField] private Button openTavern3Button;
    [SerializeField] private Button openTavern4Button;
    [SerializeField] private ResearchButton researchButton;
    // TODO: Add tavern buttons
    
    private Dictionary<KeyCode, KeyPressSubscriptionHandler> SubscriptionHandlersByKey {
        get;
        set;
    }
    public HashSet<KeyCode> KeyDownSubscriptions { get; private set; }
    public HashSet<KeyCode> KeyUpSubscriptions =>
        TargetSystem.Singleton.KeyUpSubscriptions;
    public HashSet<KeyCode> KeyContinuousSubscriptions =>
        TargetSystem.Singleton.KeyContinuousSubscriptions;
    
    private void Awake() {
        SubscriptionHandlersByKey = new Dictionary<KeyCode, KeyPressSubscriptionHandler>() {
            { KeyCode.Return, EnterPressed },
            { KeyCode.Escape, EscapePressed },
            { Settings.SelectBuilderHotkey.Value, SelectBuilderPressed },
            { Settings.ResearchHotkey.Value, ResearchPressed },
            { Settings.OpenTavern1Hotkey.Value, OpenTavern1Pressed },
            { Settings.OpenTavern2Hotkey.Value, OpenTavern2Pressed },
            { Settings.OpenTavern3Hotkey.Value, OpenTavern3Pressed },
            { Settings.OpenTavern4Hotkey.Value, OpenTavern4Pressed },
            { Settings.CameraJumpToNextLaneHotkey.Value, CameraJumpToNextLanePressed },
            { Settings.CameraJumpToOwnLaneHotkey.Value, CameraJumpToOwnLanePressed },
            { Settings.CameraJumpToPreviousLaneHotkey.Value, CameraJumpToPreviousLanePressed },
        };

        KeyDownSubscriptions =
            new HashSet<KeyCode>(SubscriptionHandlersByKey.Keys);
        
        EventBus.OnTargetsUpdated += HandleTargetsUpdated;
    }

    private void OnDestroy() {
        EventBus.OnTargetsUpdated -= HandleTargetsUpdated;
    }

    private void Start() {
        selectBuilderButton.onClick.AddListener(SelectBuilderButtonPressed);
        openTavern1Button.onClick.AddListener(OpenTavern1ButtonPressed);
        openTavern2Button.onClick.AddListener(OpenTavern2ButtonPressed);
        openTavern3Button.onClick.AddListener(OpenTavern3ButtonPressed);
        openTavern4Button.onClick.AddListener(OpenTavern4ButtonPressed);
        researchButton.Btn.onClick.AddListener(ResearchButtonPressed);
        
        foreach (KeyCode kc in TargetSystem.Singleton.KeyDownSubscriptions) {
            KeyDownSubscriptions.Add(kc);
        }
    }

    private void HandleTargetsUpdated(
        List<ClientEntity> targets
    ) {
        // closing an active one is handled within the TargetSelected interface

        if (
            targets.Count > 0
            && !InGameInterfaceStateSystem.Singleton
                .IsTargetSelectedInterfaceTheActiveInterface()
        ) {
            InGameInterfaceStateSystem.Singleton.TransitionToInterfaceWithData(
                InGameInterfaceStateSystem.InGameInterfaceType.TargetSelected,
                new TargetSelectedInterfaceTransitionData(targets)
            );
        }
    }
    
    public bool HandleInputKeyDown(KeyCode kc) {
        if (
            SubscriptionHandlersByKey.TryGetValue(
                kc,
                out KeyPressSubscriptionHandler handler
            )
        ) {
            return handler();
        }

        if (TargetSystem.Singleton.KeyDownSubscriptions.Contains(kc)) {
            return TargetSystem.Singleton.HandleInputKeyDown(kc);
        }

        return false;
    }

    public bool HandleInputKeyUp(
        KeyCode kc
    ) {
        return TargetSystem.Singleton.HandleInputKeyUp(kc);
    }

    public bool HandleInputKeyContinuous(
        KeyCode kc
    ) {
        return TargetSystem.Singleton.HandleInputKeyContinuous(kc);
    }

    private static void SelectBuilderButtonPressed() => SelectBuilderPressed();
    private static void OpenTavern1ButtonPressed() => OpenTavern1Pressed();
    private static void OpenTavern2ButtonPressed() => OpenTavern2Pressed();
    private static void OpenTavern3ButtonPressed() => OpenTavern3Pressed();
    private static void OpenTavern4ButtonPressed() => OpenTavern4Pressed();
    private static void ResearchButtonPressed() => ResearchPressed();

    private static bool ResearchPressed() {
        InGameInterfaceStateSystem.Singleton.TransitionToInterface(
            InGameInterfaceStateSystem.InGameInterfaceType.UpgradeMenu
        );
        return true;
    }

    private static bool EscapePressed() {
        InGameInterfaceStateSystem.Singleton.TransitionToInterface(
            InGameInterfaceStateSystem.InGameInterfaceType.OptionsMenu
        );
        return true;
    }

    private static bool EnterPressed() {
        InGameInterfaceStateSystem.Singleton.TransitionToInterface(
            InGameInterfaceStateSystem.InGameInterfaceType.ChatEntry
        );
        return true;
    }

    private static bool SelectBuilderPressed() {
        Lane lane = ClientLaneTracker.Singleton.MyLane;
        ClientBuilder builder =
            ClientEntityStorageSystem.Singleton.GetBuilderByLaneID(lane.ID);
        TargetSystem.Singleton.SelectEntities(new List<ClientEntity>{builder});

        return true;
    }

    private static bool CameraJumpToNextLanePressed() {
        LTWLogger.Log("TODO: Implement jump to next lane");

        return true;
    }

    private static bool CameraJumpToOwnLanePressed() {
        LTWLogger.Log("TODO: Implement jump to own lane");

        return true;
    }

    private static bool CameraJumpToPreviousLanePressed() {
        LTWLogger.Log("TODO: Implement jump to previous lane");

        return true;
    }

    private static bool OpenTavern1Pressed() {
        return OpenTavernWithIndex(0);
    }

    private static bool OpenTavern2Pressed() {
        return OpenTavernWithIndex(1);
    }

    private static bool OpenTavern3Pressed() {
        return OpenTavernWithIndex(2);
    }

    private static bool OpenTavern4Pressed() {
        return OpenTavernWithIndex(3);
    }

    private static bool OpenTavernWithIndex(int tavernIndex) {
        SendMenuInterfaceTransitionData transitionData =
            new SendMenuInterfaceTransitionData(tavernIndex);
        
        InGameInterfaceStateSystem.Singleton.TransitionToInterfaceWithData(
            InGameInterfaceStateSystem.InGameInterfaceType.SendMenu,
            transitionData
        );

        return true;
    }
}
