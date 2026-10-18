---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InterfaceState.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

// Primarily intended for use by the input system
public abstract class InterfaceState : MonoBehaviour {
    protected delegate bool KeyPressSubscriptionHandler();
    
    public virtual string Title => "";
    public virtual bool IsOverlay => false;
    
    public bool IsActive => gameObject.activeSelf;
    protected bool Initialized = false;

    protected virtual void OnHide() { }
    protected virtual void OnShow() { }
    
    protected virtual void IngestTransitionData(
        InterfaceTransitionData transitionData
    ) { }

    public event Action<InterfaceState> OnClosed;
    public event Action<InterfaceState> OnOpened;

    public void Hide() {
        gameObject.SetActive(false);
        OnHide();
    }

    public void Show() {
        gameObject.SetActive(true);
        OnShow();
    }

    public bool Close() {
        LTWLogger.Log($"Closing: {this.GetType().Name}");
        Hide();

        OnClosed?.Invoke(this);

        return true;
    }

    public void Open() {
        LTWLogger.Log($"Opening: {this.GetType().Name}");
        Initialize();
        Show();

        OnOpened?.Invoke(this);
    }

    public void OpenWithData(InterfaceTransitionData transitionData) {
        Initialize();
        IngestTransitionData(transitionData);
        Open();
    }

    protected virtual void Initialize() {
        Initialized = true;
    }

    public abstract bool ShouldShortCircuitBeforeCheckingBaseInterfaces { get; }
}
