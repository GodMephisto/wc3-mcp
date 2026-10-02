// src/Wc3.Commands/Editing/IMapEdit.cs
using Wc3.Model;

namespace Wc3.Commands.Editing;

/// <summary>
/// A single reversible mutation of an in-memory <see cref="MapDocument"/>.
/// <see cref="Apply"/> and <see cref="Revert"/> must be exact inverses so an
/// <see cref="EditHistory"/> can walk the stack in either direction without
/// drift. Edits touch only the in-memory model — persistence stays an explicit
/// Save, so undo/redo never has to reconcile against disk.
/// </summary>
public interface IMapEdit
{
    /// <summary>Short human-readable label, e.g. "Place doodad LTlt".</summary>
    string Describe { get; }

    /// <summary>Applies the edit. Also used to re-apply on redo, so it must be idempotent
    /// with respect to its own captured state (a second Apply reproduces the first).</summary>
    void Apply(MapDocument doc);

    /// <summary>Reverts the edit — the exact inverse of <see cref="Apply"/>.</summary>
    void Revert(MapDocument doc);
}
