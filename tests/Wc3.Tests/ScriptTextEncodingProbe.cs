// tests/Wc3.Tests/ScriptTextEncodingProbe.cs
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Chases down a contradiction that WctWriterFidelityProbe surfaced and could not explain.
///
/// Writing a real map's war3map.wct back through War3Net's writer changes 475,175 of 549,168
/// bytes, 86 percent of the file, while every string in the model compares EQUAL before and
/// after, and the CRLF population is identical on both sides. Those two facts cannot both be
/// innocent. A model-to-model comparison is blind to exactly one failure, a decode that is
/// LOSSY but STABLE: bytes the decoder cannot interpret become U+FFFD, re-encoding U+FFFD
/// produces EF BF BD, and decoding that gives U+FFFD again. The round trip looks clean forever
/// while the original bytes are gone.
///
/// The map in question is a deprotected Russian map (its own header says xgm.ru), so its script
/// very likely carries Windows-1251 Cyrillic, which is not valid UTF-8.
///
/// This matters well beyond the wct. The same writer family serializes war3map.wtg, and THAT
/// one is already shipped and already used by the four trigger edits. Trigger names are text.
/// So this probe measures both files, and the wtg answer decides whether a shipped feature is
/// quietly damaging maps.
/// </summary>
public class ScriptTextEncodingProbe
{
    private readonly ITestOutputHelper _out;
    public ScriptTextEncodingProbe(ITestOutputHelper output) => _out = output;

    private const char Replacement = '�';

    [Fact]
    [Trait("Category", "Corpus")]
    public void Does_the_decode_lose_bytes_it_cannot_interpret()
    {
        string path = CorpusMap.PathOrEmpty;
        if (!File.Exists(path)) { _out.WriteLine("corpus map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        _out.WriteLine($"map: {Path.GetFileName(path)}");

        // ---- war3map.wct ----
        if (doc.GetFile("war3map.wct")?.Model is MapCustomTextTriggers wct)
        {
            string text = (wct.GlobalCustomScriptCode?.Code ?? string.Empty)
                        + string.Concat(wct.CustomTextTriggers.Select(t => t.Code ?? string.Empty));
            int bad = text.Count(c => c == Replacement);
            _out.WriteLine($"wct: {text.Length:N0} chars, {bad:N0} U+FFFD replacement char(s)");
            if (bad > 0)
            {
                int at = text.IndexOf(Replacement);
                int lo = Math.Max(0, at - 40);
                _out.WriteLine($"  first at char {at:N0}, context: "
                             + text.Substring(lo, Math.Min(80, text.Length - lo))
                                   .Replace("\r", " ").Replace("\n", " "));
            }
        }
        else _out.WriteLine("wct: no parsed model on this map");

        // ---- war3map.wtg, the one that is already shipped ----
        var wtgEntry = doc.GetFile("war3map.wtg");
        if (wtgEntry?.Model is not MapTriggers wtg)
        { _out.WriteLine("wtg: no parsed model on this map"); return; }

        int nameChars = 0, nameBad = 0;
        foreach (var item in wtg.TriggerItems)
        {
            string n = item.Name ?? string.Empty;
            nameChars += n.Length;
            nameBad += n.Count(c => c == Replacement);
        }
        int descBad = wtg.TriggerItems.OfType<TriggerDefinition>()
            .Sum(t => (t.Description ?? string.Empty).Count(c => c == Replacement));
        _out.WriteLine($"wtg: {wtg.TriggerItems.Count} item(s), {nameChars:N0} name char(s), "
                     + $"{nameBad:N0} U+FFFD in names, {descBad:N0} in descriptions");

        // The decisive wtg measurement: serialize an UNTOUCHED model and compare bytes. This is
        // the exact path TriggerCommand.Persist uses, so any loss here is loss the four shipped
        // trigger edits already cause.
        byte[] before = wtgEntry.CurrentBytes;
        byte[] after = TriggerCommand.Serialize(wtg);
        int diff = 0, firstDiff = -1;
        for (int i = 0; i < Math.Min(before.Length, after.Length); i++)
            if (before[i] != after[i]) { diff++; if (firstDiff < 0) firstDiff = i; }

        _out.WriteLine($"wtg round trip: {before.Length:N0} -> {after.Length:N0} bytes "
                     + $"(delta {after.Length - before.Length:+#;-#;0}), {diff:N0} differing byte(s)"
                     + (firstDiff < 0 ? "" : $", first at {firstDiff:N0}"));

        _out.WriteLine(before.Length == after.Length && diff == 0
            ? "VERDICT (wtg): byte-identical on an untouched model. The shipped trigger edits are "
            + "safe, and any wtg text on this map survives the writer."
            : "VERDICT (wtg): the writer does NOT reproduce an untouched wtg. Every shipped "
            + "trigger edit rewrites bytes it was not asked to change, so this needs fixing "
            + "before add and remove are built on top of it.");
    }
}
