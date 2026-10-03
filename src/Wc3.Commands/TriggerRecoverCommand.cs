// src/Wc3.Commands/TriggerRecoverCommand.cs
using War3Net.Build;
using War3Net.Build.Script;
using War3Net.CodeAnalysis.Decompilers;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Rebuilds a browsable GUI trigger tree for a map that has none, by decompiling the
/// compiled script the game actually runs.
/// </summary>
/// <remarks>
/// <para>This exists in exactly one direction, and the direction is the whole point.</para>
///
/// <para>Regenerating <c>war3map.j</c> FROM a trigger tree is destructive. HiveWE does that
/// on every save, and its own maintainers record that it "silently clobbered the real
/// compiled code" for maps built by external tooling, which is why they had to add a bypass
/// path. wc3ctl refuses to do it, which is why an authored trigger here is inert.</para>
///
/// <para>Going the other way is additive. The script stays authoritative and untouched, and
/// a tree is built to describe it. Measured across 23 maps, the decompiler is useless where
/// a tree already exists, it reported success on RATankD_516.2 while producing 1 trigger
/// against that map's real 626, at 0% name recall. Where no tree exists it recovers a full
/// one, 996 triggers on Angel-samurai-Z-v332A with every one of them named and carrying at
/// least one event, condition or action.</para>
///
/// <para>So this REFUSES any map that already carries a tree. That refusal is the feature,
/// not a limitation, and it is pinned by a test.</para>
/// </remarks>
public static class TriggerRecoverCommand
{
    public sealed record RecoverResult(
        bool Ok,
        string Message,
        int Triggers = 0,
        int Categories = 0,
        int Variables = 0,
        int Functions = 0);

    /// <summary>
    /// Recovers a trigger tree from the map's compiled script and writes it as war3map.wtg.
    /// Refuses when the map already has a tree, because the decompiler cannot be trusted to
    /// reproduce one that exists.
    /// </summary>
    public static RecoverResult Recover(MapDocument doc)
    {
        if (doc.GetFile(TriggerCommand.FileName) is not null)
            return new(false,
                $"this map already has a {TriggerCommand.FileName}, and recovery would replace a "
                + "real trigger tree with a decompiled guess. Measured across the map library the "
                + "decompiler produced 1 trigger for a map carrying 626, at 0% name recall, so "
                + "this refuses rather than destroying work. Recovery is only for maps with no tree.");

        Map map;
        try
        {
            // Nothing is dirty on the recovery path, so this reproduces the original archive
            // bytes rather than rebuilding a different one.
            using var stream = new MemoryStream(doc.SaveToBytes());
            map = Map.Open(stream, MapFiles.All);
        }
        catch (Exception ex)
        {
            return new(false, $"could not open the archive for decompilation ({ex.GetType().Name}: {ex.Message})");
        }

        if (string.IsNullOrEmpty(map.Script))
            return new(false, "this map carries no compiled script, so there is nothing to recover a tree from");

        MapTriggers? tree;
        try
        {
            // A sub-version is requested deliberately. Under the sub-version rule a GUI trigger
            // holds NO war3map.wct slot, so a tree of purely GUI triggers needs no wct at all,
            // and War3Net offers no wct writer. Without this the pairing rule would demand one
            // empty slot per trigger and the map would be written inconsistent. See WctPairing.
            var decompiler = new JassScriptDecompiler(map, TriggerData.Default);
            if (!decompiler.TryDecompileMapTriggers(
                    MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4, out tree) || tree is null)
                return new(false,
                    "the decompiler could not read a trigger tree out of this script. That is a "
                    + "clean refusal rather than a wrong answer, and it usually means the script "
                    + "was hand written, optimised or obfuscated rather than emitted by the "
                    + "World Editor.");
        }
        catch (Exception ex)
        {
            return new(false, $"decompilation threw ({ex.GetType().Name}: {ex.Message})");
        }

        var defs = tree.TriggerItems.OfType<TriggerDefinition>().ToList();

        // A custom-text trigger DOES need a wct slot even under the sub-version rule, and
        // War3Net cannot write a wct. Rather than emit a tree whose bodies are unreachable,
        // refuse and say so.
        int customText = defs.Count(WctPairing.IsCustomText);
        if (customText > 0)
            return new(false,
                $"the recovered tree holds {customText} custom-text trigger(s), whose code bodies "
                + $"live in {TriggerCommand.CustomTextFileName}. That file has no writer here, so "
                + "writing the tree alone would leave those bodies unreachable. Refusing rather "
                + "than emitting an inconsistent pair.");

        if (defs.Count == 0)
            return new(false, "the decompiler returned an empty tree, so there is nothing worth writing");

        var entry = doc.AddOrReplaceRawFile(TriggerCommand.FileName, TriggerCommand.Serialize(tree));
        entry.Model = tree;

        int cats = tree.TriggerItems.OfType<TriggerCategoryDefinition>().Count();
        int funcs = defs.Sum(d => d.Functions?.Count ?? 0);

        return new(true,
            $"recovered {defs.Count} trigger(s) in {cats} categor(ies) with {tree.Variables.Count} "
            + $"variable(s) and {funcs} function(s) from the compiled script. The script itself was "
            + "not modified. Note that this tree DESCRIBES the script, it does not drive it, so "
            + "editing it here changes what the World Editor shows and not what the game runs.",
            defs.Count, cats, tree.Variables.Count, funcs);
    }
}
