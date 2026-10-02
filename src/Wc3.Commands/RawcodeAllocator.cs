// src/Wc3.Commands/RawcodeAllocator.cs
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Fresh-rawcode allocation against a map's used-id set. Shared by PortCommand
/// (collision remap) and ObjectNewCommand (create-from-base). Callers add the
/// allocated code to the set themselves when allocating repeatedly.
/// </summary>
internal static class RawcodeAllocator
{
    /// <summary>Every object rawcode the map defines across all seven kinds,
    /// war3map.* and war3mapSkin.* layers both.</summary>
    internal static HashSet<int> UsedRawcodes(MapDocument doc)
    {
        var used = new HashSet<int>();
        foreach (var kind in ObjectKinds.All)
            foreach (var e in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                used.Add(e.Id);
        return used;
    }

    internal static int Allocate(string original, HashSet<int> used)
    {
        // Preserve the leading category char (editor convention), vary the last three
        // over [0-9A-Za-z] until a free code is found; fall back to varying all four.
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        char c0 = original.Length == 4 ? original[0] : 'X';
        foreach (var a in alphabet)
            foreach (var b in alphabet)
                foreach (var d in alphabet)
                {
                    int code = new string(new[] { c0, a, b, d }).FromRawcode();
                    if (!used.Contains(code)) return code;
                }
        // Exhausted (astronomically unlikely) — vary the first char too.
        foreach (var w in alphabet)
            foreach (var a in alphabet)
                foreach (var b in alphabet)
                    foreach (var d in alphabet)
                    {
                        int code = new string(new[] { w, a, b, d }).FromRawcode();
                        if (!used.Contains(code)) return code;
                    }
        throw new InvalidOperationException("rawcode space exhausted");
    }
}
