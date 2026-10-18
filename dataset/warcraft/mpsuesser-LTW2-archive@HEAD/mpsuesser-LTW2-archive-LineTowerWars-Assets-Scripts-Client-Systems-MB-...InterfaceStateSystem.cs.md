---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/InterfaceStateSystem/InterfaceStateSystem.cs"
ref: HEAD
---

using System.Collections.Generic;

public abstract class InterfaceStateSystem<T> : SingletonBehaviour<T> {
    protected abstract void TransitionToBaseInterface();
    
    public InterfaceState ActiveInterface => InterfaceStack.Count > 0 ? InterfaceStack.Peek() : null;
    public Stack<InterfaceState> InterfaceStack { get; private set; }
    private HashSet<InterfaceState> ClosedBehind { get; set; }

    protected virtual void Awake() {
        InterfaceStack = new Stack<InterfaceState>();
        ClosedBehind = new HashSet<InterfaceState>();
    }

    private void Start() {
        TransitionToBaseInterface();
    }

    protected void TransitionToInterface(InterfaceState interfaceState) {
        PreActiveInterfaceOpened(interfaceState);
        
        if (ActiveInterface != null && !interfaceState.IsOverlay) {
            ActiveInterface.Hide();
        }

        InterfaceStack.Push(interfaceState);
        ActiveInterface.OnClosed += ActiveInterfaceClosed;
        ActiveInterface.Open();

        EventBus.SetActiveInterfaceState(ActiveInterface);
    }

    protected void TransitionToInterfaceWithData(InterfaceState interfaceState, InterfaceTransitionData transitionData) {
        PreActiveInterfaceOpened(interfaceState);
        
        if (ActiveInterface != null) {
            ActiveInterface.Hide();
        }

        InterfaceStack.Push(interfaceState);
        ActiveInterface.OnClosed += ActiveInterfaceClosed;
        ActiveInterface.OpenWithData(transitionData);

        EventBus.SetActiveInterfaceState(ActiveInterface);
    }

    private void ActiveInterfaceClosed(InterfaceState state) {
        state.OnClosed -= ActiveInterfaceClosed;

        if (InterfaceStack.Peek() == state) {
            InterfaceStack.Pop();

            while (InterfaceStack.Count > 0 && ClosedBehind.Contains(InterfaceStack.Peek())) {
                ClosedBehind.Remove(InterfaceStack.Pop());
            }
        } else {
            ClosedBehind.Add(state);
        }

        PostActiveInterfaceClosed(state);

        EventBus.SetActiveInterfaceState(ActiveInterface);
    }

    protected virtual void PreActiveInterfaceOpened(
        InterfaceState state
    ) { }

    protected virtual void PostActiveInterfaceClosed(InterfaceState state) {
        if (ActiveInterface == null) {
            LTWLogger.Log("There was no base interface to fall back to after closing the previously active interface! This shouldn't happen.");
            return;
        }
        
        ActiveInterface.Show();
    }
    
    protected bool ActiveStackContainsInterfaceOfType<TI>() where TI : InterfaceState {
        foreach (InterfaceState state in InterfaceStack) {
            if (state is TI) {
                return true;
            }
        }

        return false;
    }
}
