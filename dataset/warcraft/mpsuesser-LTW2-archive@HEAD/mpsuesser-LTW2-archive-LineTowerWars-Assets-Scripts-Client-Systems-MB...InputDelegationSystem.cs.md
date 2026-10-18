---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InputDelegationSystem/InputDelegationSystem.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDelegationSystem : SingletonBehaviour<InputDelegationSystem>
{
    private HashSet<KeyCode> HandledKeyDown { get; set; }
    private HashSet<KeyCode> HandledKeyUp { get; set; }
    private HashSet<KeyCode> HandledKeyContinuous { get; set; }
    
    private void Awake() {
        InitializeSingleton(this);

        HandledKeyDown = new HashSet<KeyCode>();
        HandledKeyUp = new HashSet<KeyCode>();
        HandledKeyContinuous = new HashSet<KeyCode>();
    }

    private static int frame = 0;
    private void Update() {
        ++frame;
        HandledKeyDown.Clear();
        HandledKeyUp.Clear();
        HandledKeyContinuous.Clear();
        
        Stack<InterfaceState> activeInterfaces = GetActiveInterfaces();
        
        // This is a workaround for the case where the activeInterfaces stack
        // is updated mid-enumeration because some interface was closed. There
        // is definitely potential for fuckiness here so this deserves a
        // TODO: Implement a safer way to iterate only over the currently active
        // interface states, taking into account mid-enumeration closures.
        
        // Some more fuckery: a Stack<> copy constructor reverses the order, so
        // fuck it we're copying it twice
        Stack<InterfaceState> activeInterfacesReverseCopy =
            new Stack<InterfaceState>(activeInterfaces);
        Stack<InterfaceState> activeInterfacesCopy =
            new Stack<InterfaceState>(activeInterfacesReverseCopy);
        while (activeInterfacesCopy.Count > 0) {
            InterfaceState state = activeInterfacesCopy.Pop();
            if (state == null) {
                LTWLogger.Log("Return b/c null interface");
                return;
            }
            if (Input.GetKeyDown(KeyCode.K)) {
                LTWLogger.Log($"Active interface type being looked at: {state.gameObject.name}");
            }
            
            if (state is IHandleKeyDownInput keyDownSubscribedState) {
                foreach (
                    KeyCode keyDownSubscription
                    in keyDownSubscribedState.KeyDownSubscriptions
                ) {
                    if (
                        !HandledKeyDown.Contains(keyDownSubscription)
                        && Input.GetKeyDown(keyDownSubscription)
                        && keyDownSubscribedState.HandleInputKeyDown(
                            keyDownSubscription
                        )
                    ) {
                        if (keyDownSubscription == KeyCode.Escape) {
                            LTWLogger.Log($"Escape handled! Frame: {frame}");
                        }
                        HandledKeyDown.Add(keyDownSubscription);
                    }
                }
            }

            if (state is IHandleKeyUpInput keyUpSubscribedState) {
                foreach (
                    KeyCode keyUpSubscription
                    in keyUpSubscribedState.KeyUpSubscriptions
                ) {
                    if (
                        !HandledKeyUp.Contains(keyUpSubscription)
                        && Input.GetKeyUp(keyUpSubscription)
                        && keyUpSubscribedState.HandleInputKeyUp(
                            keyUpSubscription
                        )
                    ) {
                        HandledKeyUp.Add(keyUpSubscription);
                    }
                }
            }

            if (state is IHandleKeyContinuousInput keyContinuousSubscribedState) {
                foreach (
                    KeyCode keyContinuousSubscription
                    in keyContinuousSubscribedState.KeyContinuousSubscriptions
                ) {
                    if (
                        !HandledKeyContinuous.Contains(keyContinuousSubscription)
                        && Input.GetKey(keyContinuousSubscription)
                        && keyContinuousSubscribedState.HandleInputKeyContinuous(
                            keyContinuousSubscription
                        )
                    ) {
                        HandledKeyContinuous.Add(keyContinuousSubscription);
                    }
                }
            }

            if (state.ShouldShortCircuitBeforeCheckingBaseInterfaces) {
                break;
            }
        }
    }

    private Stack<InterfaceState> GetActiveInterfaces() {
        // TODO: Pretty ugly, could be cleaned up
        if (InGameInterfaceStateSystem.Singleton?.ActiveInterface != null) {
            return InGameInterfaceStateSystem.Singleton.InterfaceStack;
        } else if (MainMenuInterfaceStateSystem.Singleton?.ActiveInterface != null) {
            return MainMenuInterfaceStateSystem.Singleton.InterfaceStack;
        } else if (LobbyInterfaceStateSystem.Singleton?.ActiveInterface != null) {
            return LobbyInterfaceStateSystem.Singleton.InterfaceStack;
        }

        return new Stack<InterfaceState>();
    }
}
