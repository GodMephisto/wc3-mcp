// src/Wc3.Commands/ObjectNewCommand.cs
using War3Net.Common.Extensions;
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
    /// war3map.* file — the new object inherits everything from its base, exactly
    /// like a just-created World Editor custom object. The base rawcode is not
    /// validated against the game data (any 4-char code is accepted, so customs and
    /// base-game objects both work as bases without needing a WC3 install).
    /// </summary>
    public static ObjectNewResult Execute(MapDocument doc, ObjectKind kind, string baseRawcode)
    {
        string kindName = kind.ToString().ToLowerInvariant();
        if (baseRawcode.Length != 4)
            return new(false, $"invalid base rawcode '{baseRawcode}' — expected 4 characters");

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

        return new(true, $"created {kindName} {fresh.ToRawcode()} (base {baseRawcode})", fresh.ToRawcode());
    }
}
