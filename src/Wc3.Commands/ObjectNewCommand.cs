// src/Wc3.Commands/ObjectNewCommand.cs
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

public static class ObjectNewCommand
{
    public sealed record ObjectNewResult(bool Ok, string Message, string? NewRawcode = null);

    /// <summary>
    /// Creates a custom object of <paramref name="kind"/> derived from a base rawcode:
    /// a fresh unused rawcode is allocated (scanning every kind in both object-data
    /// layers, keeping the base's leading category char) and an empty modification
    /// group (OldId = base, NewId = fresh, no field mods) is appended to the kind's
    /// war3map.* file, so the new object inherits everything from its base, exactly
    /// like a just-created World Editor custom object.
    ///
    /// The base rawcode is not rejected for being unknown, because a base-game code is a
    /// legitimate base and confirming one needs a Warcraft III install. When neither the map nor
    /// the game data knows the code, the result says so, since the new object then inherits
    /// nothing and would not work in-game.
    /// </summary>
    public static ObjectNewResult Execute(
        MapDocument doc, ObjectKind kind, string baseRawcode, string? gameDirOverride = null)
    {
        string kindName = kind.ToString().ToLowerInvariant();
        if (baseRawcode.Length != 4)
            return new(false, $"invalid base rawcode '{baseRawcode}', expected 4 characters");

        var info = ObjectKinds.Info(kind);
        var model = ObjectDataWriter.GetOrCreateMapModel(doc, kind);
        var access = model is null ? null : ObjectDataWriter.AccessFor(model);
        if (access is null)
            return new(false, $"map has no parseable {info.MapFile} ({kindName} object data)");

        var used = RawcodeAllocator.UsedRawcodes(doc);
        used.Add(baseRawcode.FromRawcode()); // never hand back the base itself
        int fresh = RawcodeAllocator.Allocate(baseRawcode, used);
        access.CreateGroup(baseRawcode.FromRawcode(), fresh);
        doc.AddOrReplaceModelFile(info.MapFile, model!);

        string note = BaseIsKnown(doc, kind, baseRawcode, gameDirOverride)
            ? string.Empty
            : $" Note, base {baseRawcode} is not a {kindName} this map defines and no game data "
              + "confirmed it, so the new object carries no fields and would not work in-game.";

        return new(true, $"created {kindName} {fresh.ToRawcode()} (base {baseRawcode}){note}",
            fresh.ToRawcode());
    }

    /// <summary>True when the base rawcode is an object this map defines, or one the game data
    /// knows. False means the new object inherits nothing, which the caller is told.</summary>
    private static bool BaseIsKnown(
        MapDocument doc, ObjectKind kind, string baseRawcode, string? gameDirOverride)
    {
        try
        {
            if (ObjectListCommand.Execute(doc, kind, gameDirOverride).Items
                .Any(i => i.Rawcode.Equals(baseRawcode, StringComparison.OrdinalIgnoreCase)))
                return true;
        }
        catch { /* an unreadable object list is not proof of absence, fall through to game data */ }

        return GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _)
            && ObjectKinds.TryGetBaseFields(ctx, kind, baseRawcode, out _);
    }
}
