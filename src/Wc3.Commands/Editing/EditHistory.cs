// src/Wc3.Commands/Editing/EditHistory.cs
using Wc3.Model;

namespace Wc3.Commands.Editing;

/// <summary>
/// Undo/redo journal for interactive map editing. Holds two stacks of
/// <see cref="IMapEdit"/>: the <i>done</i> stack (undoable) and the <i>undone</i>
/// stack (redoable). Applying a fresh edit clears the redo stack, matching the
/// linear-history convention every editor uses. The journal owns no document —
/// each call names the <see cref="MapDocument"/> to mutate — so one history can
/// be reset by pointing a session at a new map without reallocating.
/// </summary>
public sealed class EditHistory
{
    private readonly List<IMapEdit> _done = new();
    private readonly List<IMapEdit> _undone = new();

    public bool CanUndo => _done.Count > 0;
    public bool CanRedo => _undone.Count > 0;
    public int UndoDepth => _done.Count;
    public int RedoDepth => _undone.Count;

    /// <summary>Label of the edit the next <see cref="Undo"/> would revert (null if none).</summary>
    public string? NextUndoLabel => CanUndo ? _done[^1].Describe : null;

    /// <summary>Label of the edit the next <see cref="Redo"/> would re-apply (null if none).</summary>
    public string? NextRedoLabel => CanRedo ? _undone[^1].Describe : null;

    /// <summary>Raised after any mutation of the stacks, so UI can refresh enablement/labels.</summary>
    public event Action? Changed;

    /// <summary>Applies <paramref name="edit"/> to <paramref name="doc"/>, records it on the
    /// done stack, and clears the redo stack. Exceptions from Apply propagate without recording.</summary>
    public void Do(MapDocument doc, IMapEdit edit)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(edit);
        edit.Apply(doc);
        _done.Add(edit);
        _undone.Clear();
        Changed?.Invoke();
    }

    /// <summary>Reverts the most recent edit and moves it to the redo stack.
    /// Returns false (a no-op) when there is nothing to undo.</summary>
    public bool Undo(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (_done.Count == 0)
            return false;
        var edit = _done[^1];
        _done.RemoveAt(_done.Count - 1);
        edit.Revert(doc);
        _undone.Add(edit);
        Changed?.Invoke();
        return true;
    }

    /// <summary>Re-applies the most recently undone edit and moves it back to the done stack.
    /// Returns false (a no-op) when there is nothing to redo.</summary>
    public bool Redo(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (_undone.Count == 0)
            return false;
        var edit = _undone[^1];
        _undone.RemoveAt(_undone.Count - 1);
        edit.Apply(doc);
        _done.Add(edit);
        Changed?.Invoke();
        return true;
    }

    /// <summary>Drops all history (e.g. when a different map is loaded into the session).</summary>
    public void Clear()
    {
        if (_done.Count == 0 && _undone.Count == 0)
            return;
        _done.Clear();
        _undone.Clear();
        Changed?.Invoke();
    }
}
