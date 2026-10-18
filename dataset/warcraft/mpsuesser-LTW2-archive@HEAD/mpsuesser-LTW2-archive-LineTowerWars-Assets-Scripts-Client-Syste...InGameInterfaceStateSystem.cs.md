---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InterfaceStateSystem/InGameInterfaceStateSystem.cs"
ref: HEAD
---

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InGameInterfaceStateSystem : InterfaceStateSystem<InGameInterfaceStateSystem> {
    [SerializeField] private BaseMenuInterface baseMenuInterface;
    [SerializeField] private TargetSelectedInterface targetSelectedInterface;
    [SerializeField] private BuildMenuInterface buildMenuInterface;
    [SerializeField] private BuildInterface buildInterface;
    [SerializeField] private SendMenuInterface sendMenuInterface;
    [SerializeField] private UpgradeMenuInterface upgradeMenuInterface;
    [SerializeField] private OptionsMenuInterface optionsMenuInterface;
    [SerializeField] private SettingsInterface settingsInterface;
    [SerializeField] private ChatEntryInterface chatEntryInterface;
    [SerializeField] private QuitConfirmationInterface quitConfirmationInterface;
    [SerializeField] private PostgameInterface postgameInterface;
    
    public enum InGameInterfaceType {
        BaseMenu = 0,
        TargetSelected,
        BuildMenu,
        Build,
        SendMenu,
        UpgradeMenu,
        OptionsMenu,
        Settings,
        ChatEntry,
        QuitConfirmation,
        Postgame,
    }

    private Dictionary<InGameInterfaceType, InterfaceState> Interfaces { get; set; }

    private static HashSet<InGameInterfaceType> PostgamePersistentInterfaceTypes
        = new HashSet<InGameInterfaceType>() {
            InGameInterfaceType.OptionsMenu,
            InGameInterfaceType.Settings,
            InGameInterfaceType.ChatEntry,
            InGameInterfaceType.QuitConfirmation
        };

    protected override void Awake() {
        InitializeSingleton(this);

        Interfaces = new Dictionary<InGameInterfaceType, InterfaceState>() {
            { InGameInterfaceType.TargetSelected, targetSelectedInterface },
            { InGameInterfaceType.BaseMenu, baseMenuInterface },
            { InGameInterfaceType.BuildMenu, buildMenuInterface },
            { InGameInterfaceType.Build, buildInterface },
            { InGameInterfaceType.SendMenu, sendMenuInterface },
            { InGameInterfaceType.UpgradeMenu, upgradeMenuInterface },
            { InGameInterfaceType.OptionsMenu, optionsMenuInterface },
            { InGameInterfaceType.Settings, settingsInterface },
            { InGameInterfaceType.ChatEntry, chatEntryInterface },
            { InGameInterfaceType.QuitConfirmation, quitConfirmationInterface },
            { InGameInterfaceType.Postgame, postgameInterface },
        };

        EventBus.OnGameCompleted += HandleEndOfGame;
        
        base.Awake();
    }

    private void OnDestroy() {
        EventBus.OnGameCompleted -= HandleEndOfGame;
    }

    private void HandleEndOfGame() {
        TransitionToPostgameInterface();
    }

    private void TransitionToPostgameInterface() {
        Stack<InterfaceState> prioritizedOnTransition = new Stack<InterfaceState>();

        while (InterfaceStack.Count > 0) {
            InterfaceState cur = InterfaceStack.Peek();
            InGameInterfaceType type = Interfaces.FirstOrDefault(kvp => kvp.Value == cur).Key;
            if (PostgamePersistentInterfaceTypes.Contains(type)) {
                prioritizedOnTransition.Push(InterfaceStack.Pop());
            }
            else {
                cur.Close();
            }
        }
        
        TransitionToInterface(InGameInterfaceType.Postgame);

        while (prioritizedOnTransition.Count > 0) {
            InterfaceStack.Push(prioritizedOnTransition.Pop());
        }
    }

    protected override void TransitionToBaseInterface() {
        TransitionToInterface(Interfaces[InGameInterfaceType.BaseMenu]);
    }

    public void TransitionToInterface(InGameInterfaceType inGameInterfaceType) {
        base.TransitionToInterface(Interfaces[inGameInterfaceType]);
    }

    public void TransitionToInterfaceWithData(InGameInterfaceType inGameInterfaceType, InterfaceTransitionData transitionData) {
        base.TransitionToInterfaceWithData(Interfaces[inGameInterfaceType], transitionData);
    }
    
    protected override void PreActiveInterfaceOpened(
        InterfaceState state
    ) {
        EnsureMutuallyExclusiveInterfacesAreClearedBeforeOpening(state);
    }

    private void EnsureMutuallyExclusiveInterfacesAreClearedBeforeOpening(
        InterfaceState state
    ) {
        if (
            !(
                state is TargetSelectedInterface
                || state is SendMenuInterface
                || state is UpgradeMenuInterface
            )
        ) {
            return;
        }

        while (
            ActiveInterface != null
            && (
                IsTargetSelectedInterfaceInActiveStack()
                || IsSendMenuInterfaceInActiveStack()
                || IsUpgradeMenuInterfaceInActiveStack()
            )
        ) {
            ActiveInterface.Close();
        }
    }

    protected override void PostActiveInterfaceClosed(InterfaceState state) {
        if (ActiveInterface == null) {
            return;
        }
        
        // Carryover mechanic from WC3
        if (
            Settings.RemainOnBuildMenuAfterBuilding.Value == false
            && state is BuildInterface &&
            ActiveInterface is BuildMenuInterface)
        {
            ActiveInterface.Close();
        }
        else
        {
            ActiveInterface.Show(); // the new active interface, since we've popped the closed one
        }

        base.PostActiveInterfaceClosed(state);
    }

    public bool IsTargetSelectedInterfaceTheActiveInterface() {
        return ActiveInterface != null &&
               ActiveInterface is TargetSelectedInterface;
    }

    public bool IsTargetSelectedInterfaceInActiveStack() {
        return ActiveStackContainsInterfaceOfType<TargetSelectedInterface>();
    }

    private bool IsSendMenuInterfaceInActiveStack() {
        return ActiveStackContainsInterfaceOfType<SendMenuInterface>();
    }

    private bool IsUpgradeMenuInterfaceInActiveStack() {
        return ActiveStackContainsInterfaceOfType<UpgradeMenuInterface>();
    }

    public static bool IsCameraMovementLockedState(InterfaceState state) {
        return (
            state is SettingsInterface ||
            state is OptionsMenuInterface ||
            state is QuitConfirmationInterface
        );
    }
}
