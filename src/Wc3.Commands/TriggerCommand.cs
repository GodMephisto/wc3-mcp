// src/Wc3.Commands/TriggerCommand.cs
using Wc3.Model;
using War3Net.Build.Extensions;
using War3Net.Build.Script;

namespace Wc3.Commands;

/// <summary>One GUI-trigger-tree item, flattened for read-back and listing. Categories,
/// GUI/comment/script triggers and deleted stubs all share Id/ParentId/Type/Name; the
/// trigger-only flags (<see cref="IsEnabled"/> etc.) are null for non-trigger rows.</summary>
public sealed record TriggerItemFields(
    int Id, int ParentId, string Type, string Name,
    bool? IsEnabled, bool? IsInitiallyOn, bool? RunOnMapInit, bool? IsComment,
    int? FunctionCount);

/// <summary>One map-variable definition (the .wtg <c>Variables</c> list, which is distinct
/// from the TriggerItems tree).</summary>
public sealed record TriggerVariableFields(
    int Id, int ParentId, string Name, string Type,
    bool IsArray, int ArraySize, bool IsInitialized, string InitialValue);

/// <summary>Result of a mutating trigger op. <see cref="Count"/> is the trigger-item count
/// after the op (-1 when the op failed).</summary>
public sealed record TriggerOpResult(bool Ok, string Message, int Count = -1);

/// <summary>
/// Read/write access to the GUI-trigger file (war3map.wtg). The write side mutates the
/// already-parsed War3Net <see cref="MapTriggers"/> model and re-serializes the WHOLE file
/// via War3Net's writer (the inverse of the ReadMapTriggers parser wired in Parsers), so
/// untouched trigger items survive byte-for-byte. MapDocument.SerializeEntry has no
/// MapTriggers case, so the bytes go back through <see cref="MapDocument.AddOrReplaceRawFile"/>
/// (raw payloads are written verbatim on Save) and the parsed model is restored afterwards
/// so in-memory readers keep seeing the mutation. Edits here never add or remove items.
///
/// That last sentence used to end "so the model's TriggerItemCounts stays consistent with the
/// writer", which read as a LIMIT on what could ever be edited here. Measured, it is not one.
/// Adding a category to a real map's tree, serializing through this same path, saving and
/// reloading yields 3 items where there were 2, the added item survives by name, and
/// TriggerReadCommand sees the new category. War3Net's writer recomputes its own counts, so
/// adding and removing items is buildable on this writer as it stands. See
/// tests/Wc3.Tests/TriggerAddFeasibilityProbe.cs.
/// </summary>
public static class TriggerCommand
{
    public const string FileName = "war3map.wtg";
    /// <summary>The custom-text bodies. Read for the removal safety check, never written.</summary>
    public const string CustomTextFileName = "war3map.wct";

    /// <summary>Lists every trigger-tree item in document order.</summary>
    public static IReadOnlyList<TriggerItemFields> List(MapDocument doc) =>
        GetTriggers(doc).TriggerItems.Select(ToFields).ToList();

    /// <summary>Lists every map-variable definition in document order.</summary>
    public static IReadOnlyList<TriggerVariableFields> ListVariables(MapDocument doc) =>
        GetTriggers(doc).Variables.Select(ToVarFields).ToList();

    /// <summary>Renames the trigger item with the given id (any kind: category, trigger or
    /// deleted stub) and writes the re-serialized wtg back into the in-memory document.
    /// Rejects a blank name or an id that no item carries.</summary>
    public static TriggerOpResult Rename(MapDocument doc, int id, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return new TriggerOpResult(false, "Trigger name must not be blank.");

        var triggers = GetTriggers(doc);
        var item = triggers.TriggerItems.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return new TriggerOpResult(false, $"No trigger item with id {id}.");
        int sharing = triggers.TriggerItems.Count(i => i.Id == id);
        if (sharing > 1)
            return new TriggerOpResult(false,
                $"{sharing} trigger items share id {id}, so it does not identify one item. "
                + "This map numbers items per type rather than uniquely.");

        item.Name = newName;
        return Persist(doc, triggers, $"Renamed trigger item {id} to '{newName}'.");
    }

    /// <summary>Enables/disables the GUI or custom-text trigger with the given id. Rejects
    /// an id that is not a <see cref="TriggerDefinition"/> (categories and deleted stubs
    /// have no enabled flag).</summary>
    public static TriggerOpResult SetEnabled(MapDocument doc, int id, bool enabled) =>
        SetTriggerFlag(doc, id, t => t.IsEnabled = enabled,
            enabled ? $"Enabled trigger {id}." : $"Disabled trigger {id}.");

    /// <summary>Sets whether the trigger with the given id starts turned on at map load.</summary>
    public static TriggerOpResult SetInitiallyOn(MapDocument doc, int id, bool on) =>
        SetTriggerFlag(doc, id, t => t.IsInitiallyOn = on,
            $"Set trigger {id} initially-on = {on}.");

    /// <summary>Sets whether the trigger with the given id runs on map initialization.</summary>
    public static TriggerOpResult SetRunOnMapInit(MapDocument doc, int id, bool on) =>
        SetTriggerFlag(doc, id, t => t.RunOnMapInit = on,
            $"Set trigger {id} run-on-map-init = {on}.");

    /// <summary>
    /// What a GUI trigger edit does NOT do, said once so every caller can say it.
    ///
    /// Warcraft III runs war3map.j, the compiled script. war3map.wtg is the World Editor's
    /// SOURCE for that script, and nothing regenerates one from the other except the World
    /// Editor itself. Measured: adding a trigger with an event and an action to a real map left
    /// war3map.j byte-identical and never mentioning the trigger by name.
    ///
    /// So a trigger authored here is real, is visible in the World Editor, and does nothing in
    /// game until the map is opened and saved there. That is the exact failure this codebase
    /// keeps guarding against, something that looks correct and silently never runs, so it is
    /// stated in the result of every edit that creates one rather than left in documentation
    /// nobody reads at the moment it matters.
    /// </summary>
    public const string NotCompiledNote =
        " Note that this changes the World Editor's trigger source only. Warcraft III runs the "
        + "compiled war3map.j, which wc3ctl does not regenerate, so this trigger does nothing in "
        + "game until the map is opened and saved in the World Editor. That is a deliberate "
        + "refusal rather than a gap: regenerating the script from the tree overwrites whatever "
        + "compiled or hand-written code the map already carries, which is a known way to "
        + "destroy a map built by external tooling. The safe direction is the reverse, and "
        + "'trigger recover-from-script' takes it, rebuilding a tree from the script for maps "
        + "that have no tree at all.";

    /// <summary>What kind of trigger to create. Custom-text triggers are deliberately absent:
    /// their body lives in war3map.wct, which cannot be written (see <see cref="Remove"/>), so
    /// creating one would produce a trigger whose code can never be set.</summary>
    public enum NewTriggerKind { Gui, Comment }

    /// <summary>
    /// Appends a new category to the trigger tree.
    ///
    /// Always safe with respect to war3map.wct, because a category holds no code slot, so no
    /// existing pairing moves.
    /// </summary>
    public static TriggerOpResult AddCategory(MapDocument doc, string name, int parentId = -1)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new TriggerOpResult(false, "Category name must not be blank.");

        var triggers = GetTriggers(doc);
        if (parentId != -1 && !triggers.TriggerItems.Any(i => i.Id == parentId))
            return new TriggerOpResult(false, $"No trigger item with id {parentId} to parent to.");

        var cat = new TriggerCategoryDefinition
        {
            Id = NextId(triggers, TriggerItemType.Category),
            Name = name,
            ParentId = parentId,
            IsComment = false,
            IsExpanded = true,
        };
        triggers.TriggerItems.Add(cat);
        BumpCount(triggers, TriggerItemType.Category);
        return Persist(doc, triggers, $"Added category '{name}' with id {cat.Id}.");
    }

    /// <summary>
    /// Appends a new trigger to the tree, under <paramref name="parentId"/>.
    ///
    /// The new trigger is APPENDED rather than inserted next to its siblings, and that is
    /// deliberate. Document order is what pairs war3map.wct code bodies to trigger definitions, so
    /// inserting mid-list would renumber every later body's slot. Appending cannot: the new
    /// definition's slot index is one past the end, which reads as "no body", which is correct for
    /// a trigger that has none. The World Editor groups the tree by ParentId, so position in the
    /// file is not position in the UI.
    ///
    /// A new trigger is created ENABLED and initially-on, matching what the World Editor does. A
    /// default-constructed War3Net TriggerDefinition is neither, and a trigger born disabled is
    /// the kind of thing that looks fine and silently never runs.
    /// </summary>
    public static TriggerOpResult AddTrigger(
        MapDocument doc, string name, int parentId,
        NewTriggerKind kind = NewTriggerKind.Gui)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new TriggerOpResult(false, "Trigger name must not be blank.");

        var triggers = GetTriggers(doc);
        if (parentId != -1)
        {
            var parent = triggers.TriggerItems.FirstOrDefault(i => i.Id == parentId);
            if (parent is null)
                return new TriggerOpResult(false, $"No trigger item with id {parentId} to parent to.");
            if (parent is not TriggerCategoryDefinition)
                return new TriggerOpResult(false,
                    $"Trigger item {parentId} is a {parent.Type}, so it cannot hold triggers. "
                    + "Parent a trigger to a category.");
        }

        var type = kind == NewTriggerKind.Comment ? TriggerItemType.Comment : TriggerItemType.Gui;
        var td = new TriggerDefinition(type)
        {
            Id = NextId(triggers, type),
            Name = name,
            Description = string.Empty,
            ParentId = parentId,
            IsEnabled = true,
            IsInitiallyOn = true,
            RunOnMapInit = false,
            IsComment = kind == NewTriggerKind.Comment,
            IsCustomTextTrigger = false,
        };
        triggers.TriggerItems.Add(td);
        BumpCount(triggers, type);

        // On a map with no wtg sub-version a GUI definition owns an empty slot of its own, so the
        // slot list is now one short. Our reader treats a missing trailing slot as "no body",
        // which is right, and the alternative would be writing war3map.wct, which loses bytes it
        // cannot decode. Said out loud in the result rather than hidden.
        string note = WctPairing.GuiTriggersHoldASlot(triggers)
            ? " This map predates the trigger sub-version, where every trigger owns a "
              + "war3map.wct slot, so the new trigger has no slot and no code body."
            : string.Empty;

        return Persist(doc, triggers,
            $"Added {kind.ToString().ToLowerInvariant()} trigger '{name}' with id {td.Id}.{note}"
            + NotCompiledNote);
    }

    /// <summary>
    /// Removes a trigger item, and with <paramref name="recursive"/> its descendants too.
    ///
    /// Refuses rather than risks two specific corruptions, both of which would look fine on save
    /// and only surface later.
    ///
    /// First, orphaning. Removing a category that still holds children would leave those children
    /// pointing at an id nothing carries. Pass <paramref name="recursive"/> to remove the subtree.
    ///
    /// Second, and the reason this took measuring: war3map.wct pairs code bodies to definitions by
    /// POSITION (see <see cref="WctPairing"/>), and war3map.wct CANNOT be rewritten. Its decoder is
    /// lossy, measured on a real map as one byte of script text destroyed per undecodable
    /// character, invisible to a round trip because the replacement character re-encodes to
    /// itself. So the slot list is immovable, and removing a definition that holds a slot shifts
    /// every later definition onto its neighbour's code. That is refused unless it provably
    /// changes nothing: either the removed definition holds the last slot, or every slot from its
    /// own index onward is empty.
    /// </summary>
    public static TriggerOpResult Remove(MapDocument doc, int id, bool recursive = false)
    {
        var triggers = GetTriggers(doc);
        var item = triggers.TriggerItems.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return new TriggerOpResult(false, $"No trigger item with id {id}.");

        int sharing = triggers.TriggerItems.Count(i => i.Id == id);
        if (sharing > 1)
            return new TriggerOpResult(false,
                $"{sharing} trigger items share id {id}, so it does not identify one item. "
                + "This map numbers items per type rather than uniquely.");

        if (item.Type == TriggerItemType.RootCategory)
            return new TriggerOpResult(false,
                "The root category represents the map itself and cannot be removed.");

        // Gather the subtree. Parenting is by id, so walk it breadth-first over ids.
        var doomed = new List<TriggerItem> { item };
        if (recursive)
        {
            var frontier = new Queue<int>();
            frontier.Enqueue(id);
            while (frontier.Count > 0)
            {
                int parent = frontier.Dequeue();
                foreach (var child in triggers.TriggerItems.Where(i => i.ParentId == parent))
                {
                    if (doomed.Any(d => ReferenceEquals(d, child))) continue;
                    doomed.Add(child);
                    frontier.Enqueue(child.Id);
                }
            }
        }
        else
        {
            int children = triggers.TriggerItems.Count(i => i.ParentId == id);
            if (children > 0)
                return new TriggerOpResult(false,
                    $"'{item.Name}' still holds {children} item(s). Removing it would orphan them. "
                    + "Remove them first, or ask for a recursive removal.");
        }

        var refusal = WctRemovalRefusal(doc, triggers, doomed);
        if (refusal is not null) return new TriggerOpResult(false, refusal);

        foreach (var d in doomed)
        {
            triggers.TriggerItems.Remove(d);
            DropCount(triggers, d.Type);
        }

        string what = doomed.Count == 1
            ? $"'{item.Name}'"
            : $"'{item.Name}' and {doomed.Count - 1} descendant(s)";
        return Persist(doc, triggers, $"Removed {what}.");
    }

    /// <summary>
    /// Appends an event, condition or action to a GUI trigger.
    ///
    /// The parameters are built by <see cref="TriggerFunctionBuilder"/> against the World-Editor
    /// function table, and that is not a nicety. war3map.wtg stores a function's parameters but
    /// not how many there are, so the count is taken from the table on read. Writing the wrong
    /// number does not make a wrong trigger, it makes a file nobody can parse, this tool included.
    ///
    /// Refuses on a custom-text trigger, whose body is JASS in war3map.wct rather than a list of
    /// functions, and which this cannot edit at all because that file is never written.
    /// </summary>
    public static TriggerOpResult AddFunction(
        MapDocument doc, int id, TriggerFunctionType kind, string name,
        IReadOnlyList<string>? parameters = null)
    {
        var (triggers, td, refusal) = ResolveGuiTrigger(doc, id);
        if (refusal is not null) return new TriggerOpResult(false, refusal);

        var (fn, error) = TriggerFunctionBuilder.Build(kind, name, parameters);
        if (error is not null || fn is null) return new TriggerOpResult(false, error ?? "Unknown error.");

        td!.Functions.Add(fn);
        string shown = fn.Parameters.Count == 0
            ? string.Empty
            : " (" + string.Join(", ", fn.Parameters.Select(p =>
                p.Value.Length == 0 ? "<empty>" : p.Value)) + ")";
        return Persist(doc, triggers!,
            $"Added {kind.ToString().ToLowerInvariant()} '{name}'{shown} to '{td.Name}'."
            + NotCompiledNote);
    }

    /// <summary>
    /// Removes the function at <paramref name="index"/> from a GUI trigger, counting over the
    /// trigger's whole function list in the order <see cref="TriggerReadCommand"/> reports it.
    /// </summary>
    public static TriggerOpResult RemoveFunction(MapDocument doc, int id, int index)
    {
        var (triggers, td, refusal) = ResolveGuiTrigger(doc, id);
        if (refusal is not null) return new TriggerOpResult(false, refusal);

        // ResolveGuiTrigger hands back a trigger whenever it does not refuse. Binding it once
        // states that, where a null-forgiving operator on the first use only silences the first
        // use and leaves every later one warning.
        var trigger = td!;

        if (index < 0 || index >= trigger.Functions.Count)
            return new TriggerOpResult(false,
                trigger.Functions.Count == 0
                    ? $"'{trigger.Name}' has no events, conditions or actions to remove."
                    : $"'{trigger.Name}' has {trigger.Functions.Count} function(s), so index "
                      + $"{index} is out of range (0 to {trigger.Functions.Count - 1}).");

        var removed = trigger.Functions[index];
        trigger.Functions.RemoveAt(index);
        return Persist(doc, triggers!,
            $"Removed {removed.Type.ToString().ToLowerInvariant()} '{removed.Name}' from "
            + $"'{trigger.Name}'.");
    }

    /// <summary>Enables or disables one function within a trigger, the World Editor's per-line
    /// toggle rather than the whole-trigger one.</summary>
    public static TriggerOpResult SetFunctionEnabled(
        MapDocument doc, int id, int index, bool on)
    {
        var (triggers, td, refusal) = ResolveGuiTrigger(doc, id);
        if (refusal is not null) return new TriggerOpResult(false, refusal);
        var trigger = td!;   // see RemoveFunction, the tuple is non-null whenever refusal is
        if (index < 0 || index >= trigger.Functions.Count)
            return new TriggerOpResult(false,
                $"'{trigger.Name}' has {trigger.Functions.Count} function(s), so index {index} "
                + "is out of range.");

        trigger.Functions[index].IsEnabled = on;
        return Persist(doc, triggers!,
            $"{(on ? "Enabled" : "Disabled")} "
            + $"{trigger.Functions[index].Type.ToString().ToLowerInvariant()} "
            + $"'{trigger.Functions[index].Name}' in '{trigger.Name}'.");
    }

    /// <summary>Resolves an id to a GUI trigger, or explains why it is not one.</summary>
    private static (MapTriggers?, TriggerDefinition?, string?) ResolveGuiTrigger(
        MapDocument doc, int id)
    {
        var triggers = GetTriggers(doc);
        var item = triggers.TriggerItems.FirstOrDefault(i => i.Id == id);
        if (item is null) return (null, null, $"No trigger item with id {id}.");

        int sharing = triggers.TriggerItems.Count(i => i.Id == id);
        if (sharing > 1)
            return (null, null,
                $"{sharing} trigger items share id {id}, so it does not identify one item. "
                + "This map numbers items per type rather than uniquely.");

        if (item is not TriggerDefinition td)
            return (null, null, $"Trigger item {id} is a {item.Type}, not a trigger.");

        if (WctPairing.IsCustomText(td))
            return (null, null,
                $"'{td.Name}' is a custom-text trigger. Its body is JASS in "
                + $"{CustomTextFileName}, not a list of events and actions, and that file is never "
                + "written back. Edit the map script instead.");

        return (triggers, td, null);
    }

    /// <summary>Why this removal would move a war3map.wct code body onto the wrong trigger, or
    /// null when it provably would not.</summary>
    private static string? WctRemovalRefusal(
        MapDocument doc, MapTriggers triggers, IReadOnlyList<TriggerItem> doomed)
    {
        if (doc.GetFile(CustomTextFileName)?.Model is not MapCustomTextTriggers wct)
            return null;   // no bodies to move

        var slots = WctPairing.SlotIndices(triggers);
        var removedSlots = doomed.OfType<TriggerDefinition>()
            .Select(d => slots.TryGetValue(d, out int i) ? i : -1)
            .Where(i => i >= 0)
            .OrderBy(i => i)
            .ToList();
        if (removedSlots.Count == 0) return null;   // nothing being removed owns a slot

        int expected = WctPairing.ExpectedSlotCount(triggers);
        if (wct.CustomTextTriggers.Count != expected)
            return $"{CustomTextFileName} holds {wct.CustomTextTriggers.Count} code slot(s) where "
                 + $"the trigger tree accounts for {expected}. The two halves of this map already "
                 + "disagree, so removing a trigger that owns a slot could attach code to the "
                 + "wrong trigger. Refusing rather than guessing.";

        int first = removedSlots[0];
        var doomedSet = new HashSet<TriggerItem>(doomed);

        // Which surviving definitions would move onto a different slot, and what they would find
        // there. A definition holding slot j, with n removed slots below j, ends up reading slot
        // j - n. Naming the VICTIM is what makes the refusal actionable: the trigger being deleted
        // is not the one that ends up showing the wrong script.
        var victims = slots
            .Where(kv => kv.Value > first && !doomedSet.Contains(kv.Key))
            .OrderBy(kv => kv.Value)
            .ToList();
        if (victims.Count == 0) return null;   // nothing survives past the gap, so nothing shifts

        foreach (var (victim, slot) in victims.Select(kv => (kv.Key, kv.Value)))
        {
            int shifted = slot - removedSlots.Count(r => r < slot);
            if (shifted == slot) continue;                       // did not actually move
            string now = Body(wct, slot), then = Body(wct, shifted);
            if (now == then) continue;                           // moved onto identical content

            return $"Cannot remove this without moving code. '{victim.Name}' reads its script from "
                 + $"{CustomTextFileName} slot {slot}, and this removal would shift it to slot "
                 + $"{shifted}, so it would show "
                 + (then.Length == 0
                    ? "no script at all"
                    : $"{then.Trim().Length} character(s) belonging to another trigger")
                 + $". {CustomTextFileName} cannot be rewritten to match, because its decoder "
                 + "loses bytes it cannot interpret.";
        }
        return null;
    }

    private static string Body(MapCustomTextTriggers wct, int index) =>
        index >= 0 && index < wct.CustomTextTriggers.Count
            ? wct.CustomTextTriggers[index].Code?.TrimEnd('\0') ?? string.Empty
            : string.Empty;

    /// <summary>
    /// The next free id for a new item of the given type.
    ///
    /// Measured, not guessed. Sub-version maps namespace ids by item type in the high byte
    /// (0x02xxxxxx categories, 0x03xxxxxx triggers, 0x04xxxxxx comments), so ids repeat across
    /// types and 24 duplicates on one real map are normal rather than corruption. Allocating one
    /// past the global maximum would hand a category an id in the comment range. Allocating one
    /// past the maximum OF THE SAME TYPE keeps the new item inside its own namespace.
    ///
    /// Maps with no sub-version give every trigger id 0 and only number categories, so the same
    /// rule yields 1 for the first added trigger there, which is harmless and has the side benefit
    /// of making the added trigger uniquely addressable on a map where nothing else is.
    /// </summary>
    private static int NextId(MapTriggers triggers, TriggerItemType type)
    {
        var sameType = triggers.TriggerItems.Where(i => i.Type == type).Select(i => i.Id).ToList();
        if (sameType.Count > 0) return sameType.Max() + 1;

        int global = triggers.TriggerItems.Count == 0 ? 0 : triggers.TriggerItems.Max(i => i.Id);
        return global + 1;
    }

    /// <summary>
    /// Keeps MapTriggers.TriggerItemCounts in step with the item list.
    ///
    /// The writer EMITS this dictionary on sub-version maps, measured by corrupting one entry and
    /// watching the serialized bytes change at offset 12. It is a plain per-type tally, verified
    /// against the real tally on all 10 maps in the library that populate it. Maps that leave it
    /// empty are left empty, since the writer ignores it there and adding keys would change bytes
    /// for no reason.
    /// </summary>
    private static void BumpCount(MapTriggers triggers, TriggerItemType type) =>
        SyncCount(triggers, type);

    private static void DropCount(MapTriggers triggers, TriggerItemType type) =>
        SyncCount(triggers, type);

    private static void SyncCount(MapTriggers triggers, TriggerItemType type)
    {
        if (triggers.TriggerItemCounts.Count == 0) return;
        triggers.TriggerItemCounts[type] = triggers.TriggerItems.Count(i => i.Type == type);
    }

    private static TriggerOpResult SetTriggerFlag(
        MapDocument doc, int id, Action<TriggerDefinition> mutate, string message)
    {
        var triggers = GetTriggers(doc);
        var item = triggers.TriggerItems.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return new TriggerOpResult(false, $"No trigger item with id {id}.");
        // Ids are not unique. Sub-version maps number items per type, and maps without a
        // sub-version give EVERY trigger id 0 (measured: 606 triggers all id 0 on one real map).
        // Editing whichever one happens to come first is worse than refusing.
        int sharing = triggers.TriggerItems.Count(i => i.Id == id);
        if (sharing > 1)
            return new TriggerOpResult(false,
                $"{sharing} trigger items share id {id}, so it does not identify one item. "
                + "This map numbers items per type rather than uniquely.");
        if (item is not TriggerDefinition td)
            return new TriggerOpResult(false,
                $"Trigger item {id} is a {item.Type}, not an editable trigger.");

        mutate(td);
        return Persist(doc, triggers, message);
    }

    /// <summary>Serializes a MapTriggers with War3Net's writer (the exact inverse of the
    /// ReadMapTriggers parser wired in Parsers).</summary>
    public static byte[] Serialize(MapTriggers triggers)
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            writer.Write(triggers);
        return ms.ToArray();
    }

    private static TriggerOpResult Persist(MapDocument doc, MapTriggers triggers, string message)
    {
        var entry = doc.AddOrReplaceRawFile(FileName, Serialize(triggers));
        // AddOrReplaceRawFile drops the parsed model (raw payload wins on Save); restore it
        // so in-memory readers keep seeing the mutation. The override bytes were serialized
        // from this very model, so the two stay consistent.
        entry.Model = triggers;
        return new TriggerOpResult(true, message, triggers.TriggerItems.Count);
    }

    private static MapTriggers GetTriggers(MapDocument doc) =>
        doc.GetFile(FileName)?.Model as MapTriggers
        ?? throw new InvalidOperationException(
            $"{FileName} is missing or could not be parsed; triggers are not editable.");

    private static TriggerItemFields ToFields(TriggerItem i) => i is TriggerDefinition td
        ? new(i.Id, i.ParentId, i.Type.ToString(), i.Name ?? string.Empty,
              td.IsEnabled, td.IsInitiallyOn, td.RunOnMapInit, td.IsComment, td.Functions.Count)
        : new(i.Id, i.ParentId, i.Type.ToString(), i.Name ?? string.Empty,
              null, null, null, null, null);

    private static TriggerVariableFields ToVarFields(VariableDefinition v) => new(
        v.Id, v.ParentId, v.Name ?? string.Empty, v.Type ?? string.Empty,
        v.IsArray, v.ArraySize, v.IsInitialized, v.InitialValue ?? string.Empty);
}
