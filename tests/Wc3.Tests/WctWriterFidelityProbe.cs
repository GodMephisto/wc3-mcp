// tests/Wc3.Tests/WctWriterFidelityProbe.cs
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// A probe, deliberately not a guard. It answers the one question that gates trigger removal.
///
/// TriggerWriteTests pins that war3map.wct has NO model writer in the save dispatch, because
/// War3Net's writer measured 549,168 bytes in and 549,170 out, a 2 byte gain on an untouched
/// file. That is the right call for the automatic path, where an untouched map must not grow.
///
/// It says nothing about a DELIBERATE edit. Removing a trigger has to remove that trigger's
/// code slot too, because TriggerReadCommand pairs wct bodies to wtg TriggerDefinitions
/// POSITIONALLY, so leaving the slot list alone silently reassigns every later body. So the
/// question is not "is the writer byte-faithful", it is "is the writer CONTENT-faithful".
///
/// Two very different answers hide behind the same 2 bytes. Benign framing (a length prefix or
/// a terminator written a hair differently) means an edit path may use the writer, and the cost
/// is 2 bytes on a file the user asked to change. Content drift (a truncated body, a lost
/// header) means removal needs a hand-written serializer instead.
///
/// This reports which, and names the first differing byte offset so the answer is checkable.
/// </summary>
public class WctWriterFidelityProbe
{
    private readonly ITestOutputHelper _out;
    public WctWriterFidelityProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    [Trait("Category", "Corpus")]
    public void Is_the_wct_writer_content_faithful_even_though_it_is_not_byte_faithful()
    {
        string path = CorpusMap.PathOrEmpty;
        if (!File.Exists(path)) { _out.WriteLine("corpus map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var entry = doc.GetFile("war3map.wct");
        if (entry?.Model is not MapCustomTextTriggers wct)
        { _out.WriteLine("this map has no parsed war3map.wct, skipped"); return; }

        byte[] before = entry.CurrentBytes;

        byte[] after;
        using (var ms = new MemoryStream())
        {
            using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
                w.Write(wct);
            after = ms.ToArray();
        }

        _out.WriteLine($"in  {before.Length:N0} bytes");
        _out.WriteLine($"out {after.Length:N0} bytes  (delta {after.Length - before.Length:+#;-#;0})");

        int firstDiff = -1;
        for (int i = 0; i < Math.Min(before.Length, after.Length); i++)
            if (before[i] != after[i]) { firstDiff = i; break; }

        if (firstDiff < 0 && before.Length == after.Length)
            _out.WriteLine("bytes are IDENTICAL, so the pin may be measuring a different map");
        else if (firstDiff < 0)
            _out.WriteLine($"identical up to {Math.Min(before.Length, after.Length):N0}, "
                         + "the difference is purely trailing bytes");
        else
        {
            _out.WriteLine($"first differing byte at offset {firstDiff:N0} "
                         + $"(0x{firstDiff:X}): {before[firstDiff]} -> {after[firstDiff]}");
            int lo = Math.Max(0, firstDiff - 24), hi = Math.Min(before.Length, firstDiff + 24);
            _out.WriteLine("  in : " + Hex(before, lo, hi));
            _out.WriteLine("  out: " + Hex(after, lo, Math.Min(after.Length, hi)));
            _out.WriteLine("  in  as text : " + Safe(before, lo, hi));
            _out.WriteLine("  out as text : " + Safe(after, lo, Math.Min(after.Length, hi)));
        }

        // The length prefix at the divergence dropped by 50 while the whole file grew by 2, so
        // at least two things changed. Count every differing byte and compare the line-ending
        // populations, because a CRLF-to-LF normalization would lose exactly one byte per line
        // break and would be invisible to a model-to-model comparison (both sides read back the
        // same way). That is content drift even though our own round trip looks clean.
        int diffCount = 0, lastDiff = -1;
        for (int i = 0; i < Math.Min(before.Length, after.Length); i++)
            if (before[i] != after[i]) { diffCount++; lastDiff = i; }
        _out.WriteLine($"differing bytes: {diffCount:N0}, last at {lastDiff:N0} "
                     + $"of {Math.Min(before.Length, after.Length):N0}");

        _out.WriteLine($"raw  CRLF pairs: {CountPairs(before, 13, 10):N0}, "
                     + $"bare CR: {CountBare(before, 13, 10):N0}, "
                     + $"bare LF: {CountBareLf(before):N0}");
        _out.WriteLine($"out  CRLF pairs: {CountPairs(after, 13, 10):N0}, "
                     + $"bare CR: {CountBare(after, 13, 10):N0}, "
                     + $"bare LF: {CountBareLf(after):N0}");

        string model = wct.GlobalCustomScriptCode?.Code ?? string.Empty;
        if (wct.CustomTextTriggers.Count > 0)
            model += wct.CustomTextTriggers[0].Code ?? string.Empty;
        int mCrLf = 0, mBareCr = 0, mBareLf = 0;
        for (int i = 0; i < model.Length; i++)
        {
            char c = model[i];
            if (c == (char)13 && i + 1 < model.Length && model[i + 1] == (char)10)
            { mCrLf++; i++; }
            else if (c == (char)13) mBareCr++;
            else if (c == (char)10) mBareLf++;
        }
        _out.WriteLine($"model CRLF: {mCrLf:N0}, bare CR: {mBareCr:N0}, bare LF: {mBareLf:N0}");

        // Now the question that actually matters. Re-read the written bytes through the same
        // parser the map load uses, and compare CONTENT slot by slot.
        MapCustomTextTriggers reread;
        try
        {
            using var rms = new MemoryStream(after);
            using var r = new BinaryReader(rms);
            reread = r.ReadMapCustomTextTriggers();
        }
        catch (Exception ex)
        {
            _out.WriteLine($"REREAD FAILED: {ex.GetType().Name}: {ex.Message}");
            _out.WriteLine("VERDICT: the writer emits something the reader rejects. "
                         + "Removal needs a hand-written serializer.");
            return;
        }

        _out.WriteLine($"slots in {wct.CustomTextTriggers.Count}, out {reread.CustomTextTriggers.Count}");

        bool headerSame = (wct.GlobalCustomScriptComment ?? string.Empty)
                       == (reread.GlobalCustomScriptComment ?? string.Empty);
        bool globalSame = (wct.GlobalCustomScriptCode?.Code ?? string.Empty)
                       == (reread.GlobalCustomScriptCode?.Code ?? string.Empty);
        _out.WriteLine($"global comment preserved: {headerSame}");
        _out.WriteLine($"global custom script preserved: {globalSame}");

        int drifted = 0;
        int n = Math.Min(wct.CustomTextTriggers.Count, reread.CustomTextTriggers.Count);
        for (int i = 0; i < n; i++)
        {
            string a = wct.CustomTextTriggers[i].Code ?? string.Empty;
            string b = reread.CustomTextTriggers[i].Code ?? string.Empty;
            if (a == b) continue;
            drifted++;
            if (drifted <= 3)
                _out.WriteLine($"  slot {i} differs: {a.Length} chars -> {b.Length} chars");
        }

        bool countSame = wct.CustomTextTriggers.Count == reread.CustomTextTriggers.Count;
        bool contentFaithful = countSame && headerSame && globalSame && drifted == 0;

        _out.WriteLine(contentFaithful
            ? "VERDICT: CONTENT-FAITHFUL. Every slot, the global script and the comment survive. "
            + "An edit path may use this writer, paying the byte delta only on a file the user "
            + "asked to change."
            : $"VERDICT: CONTENT DRIFT ({drifted} slot(s) changed, count same: {countSame}). "
            + "Removal must not use this writer.");
    }

    private static string Hex(byte[] b, int lo, int hi)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = lo; i < hi; i++) sb.Append(b[i].ToString("X2")).Append(' ');
        return sb.ToString();
    }

    private static string Safe(byte[] b, int lo, int hi)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = lo; i < hi; i++)
            sb.Append(b[i] >= 32 && b[i] < 127 ? (char)b[i] : '.');
        return sb.ToString();
    }

    private static int CountPairs(byte[] b, byte first, byte second)
    {
        int n = 0;
        for (int i = 0; i + 1 < b.Length; i++) if (b[i] == first && b[i + 1] == second) n++;
        return n;
    }

    private static int CountBare(byte[] b, byte c, byte notFollowedBy)
    {
        int n = 0;
        for (int i = 0; i < b.Length; i++)
            if (b[i] == c && (i + 1 >= b.Length || b[i + 1] != notFollowedBy)) n++;
        return n;
    }

    private static int CountBareLf(byte[] b)
    {
        int n = 0;
        for (int i = 0; i < b.Length; i++)
            if (b[i] == 10 && (i == 0 || b[i - 1] != 13)) n++;
        return n;
    }
}
