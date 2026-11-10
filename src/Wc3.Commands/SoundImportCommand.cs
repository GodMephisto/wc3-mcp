// src/Wc3.Commands/SoundImportCommand.cs
using System.Globalization;
using System.Text;
using System.Text.Json;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One resolved clip of an imported event. <see cref="FilePath"/> is the map path the
/// catalog entry points at, always under <c>war3mapImported\</c>.</summary>
public sealed record SoundImportClip(string SoundName, string FilePath, bool Created);

/// <summary>One event that resolved to at least one clip. <see cref="SoundNames"/> is in clip
/// order, so the generated dispatcher rotates them the way the event listed them.</summary>
public sealed record SoundEventImport(
    string EventName,
    string FunctionName,
    IReadOnlyList<string> Clips,
    IReadOnlyList<string> SoundNames,
    float Cooldown,
    int MaxVoices,
    int Priority);

/// <summary>An event that was not imported, with the reason a caller can show a user.</summary>
public sealed record SoundImportRefusal(string EventName, string Reason);

/// <summary>
/// Everything a sound event import did. <see cref="Events"/> are the events that resolved,
/// <see cref="CatalogEntries"/> the war3map.w3s definitions written (one per clip),
/// <see cref="MissingClips"/> the clips found in neither the clip folder nor the map, and
/// <see cref="Jass"/> the dispatcher text for a caller to append to war3map.j. This command
/// never edits the script, it returns the text.
/// </summary>
public sealed record SoundImportResult(
    bool Ok,
    string Message,
    IReadOnlyList<SoundEventImport> Events,
    IReadOnlyList<SoundImportClip> CatalogEntries,
    IReadOnlyList<string> MissingClips,
    IReadOnlyList<SoundImportRefusal> Refused,
    string Jass);

/// <summary>
/// Imports sound events from a JSON document into a map. For each event it writes one sound
/// catalog entry per clip into war3map.w3s (through <see cref="SoundCommand.Add"/>, so the
/// byte-faithful War3Net writer is the only serializer) and generates a JASS dispatcher that
/// keeps a per-event timer, a per-event cooldown, a maximum voice count and a priority.
///
/// The clip folder is the imports folder on disk, not the map. A clip therefore has two legal
/// homes, a file under <paramref name="clipsDirectory"/> waiting to be imported, and an entry
/// the map already holds at <c>war3mapImported\&lt;clip&gt;</c>. A clip in neither is reported in
/// <see cref="SoundImportResult.MissingClips"/>.
///
/// An event is imported whole or not at all. One missing clip refuses the event, because a
/// dispatcher that can select a sound which does not exist is worse than no dispatcher.
///
/// The generated JASS is plain JASS, not vJass. It declares every global it assigns, so
/// <see cref="JassScriptCheck.IsCompilable"/> accepts it as a fragment.
/// </summary>
public static class SoundImportCommand
{
    /// <summary>The map folder every generated catalog entry points into.</summary>
    public const string ImportPrefix = @"war3mapImported\";

    /// <summary>Seconds a voice slot stays claimed after a play starts. This is the only hold
    /// the generator can know without decoding the clip, and it is emitted as a named constant
    /// in the generated block so a map author can tune it.</summary>
    private const double VoiceHoldSeconds = 0.8;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    /// <summary>JSON shape of one event, exactly as the document spells it.</summary>
    private sealed class EventJson
    {
        public List<string>? Clips { get; set; }
        public double? Cooldown { get; set; }
        public int? MaxVoices { get; set; }
        public int? Priority { get; set; }
    }

    /// <summary>One event that resolved, with its catalog writes, kept internally so the JASS
    /// generator and the public result are built from the same decisions.</summary>
    private sealed record ResolvedEvent(
        string Name, string FunctionName, float Cooldown, int MaxVoices, int Priority,
        IReadOnlyList<string> Clips, IReadOnlyList<SoundImportClip> Catalog);

    /// <summary>
    /// Resolves <paramref name="eventsJson"/> against <paramref name="clipsDirectory"/>, writes the
    /// sound catalog entries, and returns the dispatcher text. Nothing is saved here, the catalog
    /// is marked dirty on <paramref name="doc"/> and the front-end owns the write.
    /// </summary>
    public static SoundImportResult Import(MapDocument doc, string eventsJson, string clipsDirectory)
    {
        ArgumentNullException.ThrowIfNull(doc);

        Dictionary<string, EventJson?>? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<Dictionary<string, EventJson?>>(eventsJson ?? "", JsonOptions);
        }
        catch (JsonException ex)
        {
            return new(false, $"events JSON could not be parsed, {ex.Message}",
                Array.Empty<SoundEventImport>(), Array.Empty<SoundImportClip>(),
                Array.Empty<string>(), Array.Empty<SoundImportRefusal>(), string.Empty);
        }

        var events = new List<SoundEventImport>();
        var catalog = new List<SoundImportClip>();
        var missing = new List<string>();
        var refused = new List<SoundImportRefusal>();
        var takenFunctions = new HashSet<string>(StringComparer.Ordinal);
        var existing = SoundCommand.List(doc)
            .ToDictionary(s => s.Name, s => s.FilePath, StringComparer.OrdinalIgnoreCase);
        var takenNames = new HashSet<string>(existing.Keys, StringComparer.OrdinalIgnoreCase);
        var resolved = new List<ResolvedEvent>();

        // Sorted so two runs over one document generate the same text, event order in JSON is not
        // preserved by Dictionary in any contract.
        foreach (var (name, spec) in (parsed ?? new()).OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                refused.Add(new(name ?? "", "the event name is blank"));
                continue;
            }
            if (spec is null)
            {
                refused.Add(new(name, "the event has no definition"));
                continue;
            }

            var clips = (spec.Clips ?? new List<string>())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (clips.Count == 0)
            {
                refused.Add(new(name, "the event lists no clips"));
                continue;
            }

            float cooldown = (float)(spec.Cooldown ?? 0);
            int maxVoices = spec.MaxVoices ?? 1;
            int priority = spec.Priority ?? 0;
            if (cooldown < 0 || float.IsNaN(cooldown) || float.IsInfinity(cooldown))
            {
                refused.Add(new(name, $"cooldown {cooldown.ToString(CultureInfo.InvariantCulture)} is not a non-negative number of seconds"));
                continue;
            }
            if (maxVoices < 1 || maxVoices > 32)
            {
                refused.Add(new(name, $"maxVoices {maxVoices} is outside the supported range 1 to 32"));
                continue;
            }
            if (priority < 0)
            {
                refused.Add(new(name, $"priority {priority} is negative"));
                continue;
            }

            var unresolved = new List<string>();
            foreach (var clip in clips)
            {
                if (!ClipResolves(doc, clipsDirectory, clip))
                {
                    unresolved.Add(clip);
                    if (!missing.Contains(clip, StringComparer.OrdinalIgnoreCase)) missing.Add(clip);
                }
            }
            if (unresolved.Count > 0)
            {
                refused.Add(new(name, $"clips not found, {string.Join(", ", unresolved)}"));
                continue;
            }

            var writes = new List<SoundImportClip>();
            var created = new List<string>();
            string? failure = null;
            foreach (var clip in clips)
            {
                var filePath = ImportPathFor(clip);
                // A re-run over the same document must not stack a second entry for a clip the
                // catalog already defines at the same path.
                var reused = existing.FirstOrDefault(
                    p => string.Equals(ImportPathFor(p.Value), filePath, StringComparison.OrdinalIgnoreCase));

                string soundName;
                bool isNew = false;
                if (reused.Key is not null)
                {
                    soundName = reused.Key;
                }
                else
                {
                    soundName = AllocateSoundName(clip, takenNames);
                    var add = SoundCommand.Add(doc, soundName, filePath);
                    if (!add.Ok)
                    {
                        takenNames.Remove(soundName);
                        failure = add.Message;
                        break;
                    }
                    existing[soundName] = filePath;
                    created.Add(soundName);
                    isNew = true;
                }
                writes.Add(new SoundImportClip(soundName, filePath, isNew));
            }
            if (failure is not null || writes.Count != clips.Count)
            {
                // Whole or nothing, so an entry already written for this event is taken back out.
                foreach (var sound in created) SoundCommand.Remove(doc, sound);
                refused.Add(new(name, failure ?? "the event resolved fewer clips than it lists"));
                continue;
            }

            var functionName = AllocateFunctionName(name, takenFunctions);
            resolved.Add(new ResolvedEvent(name, functionName, cooldown, maxVoices, priority, clips, writes));
        }

        foreach (var e in resolved)
        {
            catalog.AddRange(e.Catalog);
            events.Add(new SoundEventImport(
                e.Name, e.FunctionName, e.Clips, e.Catalog.Select(c => c.SoundName).ToList(),
                e.Cooldown, e.MaxVoices, e.Priority));
        }

        string jass = resolved.Count == 0 ? string.Empty : BuildJass(resolved);
        bool ok = events.Count > 0 && missing.Count == 0 && refused.Count == 0;
        return new(ok, BuildMessage(events.Count, catalog.Count, missing, refused, parsed is null),
            events, catalog, missing, refused, jass);
    }

    /// <summary>The map path a clip lands at, canonical <c>war3mapImported\</c> prefix, backslashes,
    /// and no doubled folder when the JSON already spells one.</summary>
    public static string ImportPathFor(string clip)
    {
        var relative = (clip ?? "").Replace('/', '\\').TrimStart('\\');
        while (relative.StartsWith(ImportPrefix, StringComparison.OrdinalIgnoreCase))
            relative = relative[ImportPrefix.Length..];
        return ImportPrefix + relative;
    }

    /// <summary>
    /// True when the clip is somewhere the map can play it from, a file under
    /// <paramref name="clipsDirectory"/> or an entry the map already holds at its import path.
    /// A clip that escapes the folder with a parent segment never resolves.
    /// </summary>
    private static bool ClipResolves(MapDocument doc, string clipsDirectory, string clip)
    {
        var relative = ImportPathFor(clip)[ImportPrefix.Length..];
        if (relative.Length == 0) return false;
        if (relative.Split('\\').Any(s => s == "..")) return false;

        if (!string.IsNullOrWhiteSpace(clipsDirectory))
        {
            var onDisk = Path.Combine(clipsDirectory, relative.Replace('\\', Path.DirectorySeparatorChar));
            if (File.Exists(onDisk)) return true;
        }
        return doc.GetFile(ImportPrefix + relative) is not null;
    }

    /// <summary>A catalog name no other entry uses. <see cref="SoundCommand.SoundNameForFile"/> is
    /// the base, then a numeric suffix, because two clips in different folders routinely share a
    /// bare file name.</summary>
    private static string AllocateSoundName(string clip, HashSet<string> taken)
    {
        var baseName = SoundCommand.SoundNameForFile(clip);
        if (taken.Add(baseName)) return baseName;
        for (int i = 2; ; i++)
        {
            var candidate = $"{baseName}_{i}";
            if (taken.Add(candidate)) return candidate;
        }
    }

    private static string AllocateFunctionName(string eventName, HashSet<string> taken)
    {
        var baseName = "SND_Event_" + Identifier(eventName) + "_Play";
        if (taken.Add(baseName)) return baseName;
        for (int i = 2; ; i++)
        {
            var candidate = $"SND_Event_{Identifier(eventName)}_{i}_Play";
            if (taken.Add(candidate)) return candidate;
        }
    }

    /// <summary>A JASS identifier derived from an event name. Dots and spaces become underscores,
    /// and a leading digit is prefixed so the name is always legal.</summary>
    private static string Identifier(string name)
    {
        var sb = new StringBuilder(name.Length);
        foreach (var c in name)
            sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
        var ident = sb.ToString();
        if (ident.Length == 0) return "event";
        return char.IsDigit(ident[0]) ? "e_" + ident : ident;
    }

    private static string BuildMessage(
        int events, int catalog, IReadOnlyList<string> missing, IReadOnlyList<SoundImportRefusal> refused,
        bool unparsed)
    {
        if (unparsed) return "no events were read from the JSON document";
        if (events == 0 && refused.Count == 0) return "the JSON document defines no events";
        var message = $"imported {events} event(s), wrote {catalog} sound catalog entr{(catalog == 1 ? "y" : "ies")}";
        if (refused.Count > 0) message += $", refused {refused.Count} event(s)";
        if (missing.Count > 0) message += $", {missing.Count} clip(s) missing";
        return message;
    }

    /// <summary>
    /// The dispatcher text. <c>SND_EventPlay</c> is the one place voice accounting happens, every
    /// per-event function is a thin wrapper naming that event's index and cooldown, and
    /// <c>SND_Init</c> creates the sounds and timers. Slot ownership is recorded on each play and
    /// read back from the slot timers, so no timer callback and no hashtable is needed to expire
    /// a voice.
    /// </summary>
    private static string BuildJass(IReadOnlyList<ResolvedEvent> events)
    {
        int slots = events.Sum(e => e.MaxVoices);
        int clipIndex = 0;
        var sb = new StringBuilder();

        sb.AppendLine("// ---- sound events, generated by wc3ctl sound import ----");
        sb.AppendLine("// Append this block to war3map.j and call SND_Init once, from config() or main().");
        sb.AppendLine("// Play an event with SND_Event_<name>_Play, which returns true when a voice started.");
        sb.AppendLine("// SND_VOICE_HOLD is how long a voice stays claimed, raise it for long clips.");
        sb.AppendLine();
        sb.AppendLine("globals");
        sb.AppendLine($"    constant real SND_VOICE_HOLD = {Real(VoiceHoldSeconds)}");
        sb.AppendLine($"    constant integer SND_VOICE_SLOTS = {slots.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine();
        sb.AppendLine("    timer array SND_EVENT_TIMER");
        sb.AppendLine("    integer array SND_EVENT_VOICES");
        sb.AppendLine("    integer array SND_EVENT_PRIORITY");
        sb.AppendLine("    integer array SND_EVENT_FIRST_CLIP");
        sb.AppendLine("    integer array SND_EVENT_CLIP_COUNT");
        sb.AppendLine("    integer array SND_EVENT_CURSOR");
        sb.AppendLine();
        sb.AppendLine("    sound array SND_CLIP");
        sb.AppendLine("    timer array SND_SLOT_TIMER");
        sb.AppendLine("    integer array SND_SLOT_OWNER");
        sb.AppendLine("    integer array SND_SLOT_PRIORITY");
        sb.AppendLine("endglobals");
        sb.AppendLine();

        sb.AppendLine("function SND_EventPlay takes integer ev, real cooldown returns boolean");
        sb.AppendLine("    local integer active = 0");
        sb.AppendLine("    local integer slot = -1");
        sb.AppendLine("    local integer victim = -1");
        sb.AppendLine("    local integer victimPriority = 32767");
        sb.AppendLine("    local integer i = 0");
        sb.AppendLine("    local real remaining = 0.00");
        sb.AppendLine("    local sound snd = null");
        sb.AppendLine();
        sb.AppendLine("    if (SND_EVENT_TIMER[ev] == null) then");
        sb.AppendLine("        set SND_EVENT_TIMER[ev] = CreateTimer()");
        sb.AppendLine("    endif");
        sb.AppendLine("    if (TimerGetRemaining(SND_EVENT_TIMER[ev]) > 0.00) then");
        sb.AppendLine("        return false");
        sb.AppendLine("    endif");
        sb.AppendLine();
        sb.AppendLine("    set i = 0");
        sb.AppendLine("    loop");
        sb.AppendLine("        exitwhen i >= SND_VOICE_SLOTS");
        sb.AppendLine("        set remaining = TimerGetRemaining(SND_SLOT_TIMER[i])");
        sb.AppendLine("        if (remaining > 0.00) then");
        sb.AppendLine("            if (SND_SLOT_OWNER[i] == ev) then");
        sb.AppendLine("                set active = active + 1");
        sb.AppendLine("            endif");
        sb.AppendLine("            if (SND_SLOT_PRIORITY[i] < victimPriority) then");
        sb.AppendLine("                set victimPriority = SND_SLOT_PRIORITY[i]");
        sb.AppendLine("                set victim = i");
        sb.AppendLine("            endif");
        sb.AppendLine("        else");
        sb.AppendLine("            if (slot < 0) then");
        sb.AppendLine("                set slot = i");
        sb.AppendLine("            endif");
        sb.AppendLine("        endif");
        sb.AppendLine("        set i = i + 1");
        sb.AppendLine("    endloop");
        sb.AppendLine();
        sb.AppendLine("    if (active >= SND_EVENT_VOICES[ev]) then");
        sb.AppendLine("        return false");
        sb.AppendLine("    endif");
        sb.AppendLine("    if (slot < 0) then");
        sb.AppendLine("        if (victim < 0) then");
        sb.AppendLine("            return false");
        sb.AppendLine("        endif");
        sb.AppendLine("        if (victimPriority >= SND_EVENT_PRIORITY[ev]) then");
        sb.AppendLine("            return false");
        sb.AppendLine("        endif");
        sb.AppendLine("        set slot = victim");
        sb.AppendLine("    endif");
        sb.AppendLine();
        sb.AppendLine("    set snd = SND_CLIP[SND_EVENT_FIRST_CLIP[ev] + SND_EVENT_CURSOR[ev]]");
        sb.AppendLine("    set SND_EVENT_CURSOR[ev] = ModuloInteger(SND_EVENT_CURSOR[ev] + 1, SND_EVENT_CLIP_COUNT[ev])");
        sb.AppendLine("    call StartSound(snd)");
        sb.AppendLine("    call TimerStart(SND_SLOT_TIMER[slot], SND_VOICE_HOLD, false, null)");
        sb.AppendLine("    set SND_SLOT_OWNER[slot] = ev");
        sb.AppendLine("    set SND_SLOT_PRIORITY[slot] = SND_EVENT_PRIORITY[ev]");
        sb.AppendLine("    call TimerStart(SND_EVENT_TIMER[ev], cooldown, false, null)");
        sb.AppendLine("    return true");
        sb.AppendLine("endfunction");
        sb.AppendLine();

        sb.AppendLine("function SND_Init takes nothing returns nothing");
        sb.AppendLine("    local integer i = 0");
        sb.AppendLine();
        for (int e = 0; e < events.Count; e++)
        {
            var ev = events[e];
            sb.AppendLine($"    // {ev.Name}, {ev.Catalog.Count} clip(s), cooldown {Real(ev.Cooldown)}, {ev.MaxVoices} voice(s), priority {ev.Priority.ToString(CultureInfo.InvariantCulture)}");
            foreach (var clip in ev.Catalog)
                sb.AppendLine($"    set SND_CLIP[{clipIndex++}] = CreateSound(\"{JassPath(clip.FilePath)}\", false, false, false, 10, 10, \"DefaultEAXON\")");
            sb.AppendLine($"    set SND_EVENT_VOICES[{e}] = {ev.MaxVoices.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"    set SND_EVENT_PRIORITY[{e}] = {ev.Priority.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"    set SND_EVENT_FIRST_CLIP[{e}] = {FirstClip(events, e).ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"    set SND_EVENT_CLIP_COUNT[{e}] = {ev.Catalog.Count.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"    set SND_EVENT_TIMER[{e}] = CreateTimer()");
            sb.AppendLine();
        }
        sb.AppendLine("    set i = 0");
        sb.AppendLine("    loop");
        sb.AppendLine("        exitwhen i >= SND_VOICE_SLOTS");
        sb.AppendLine("        set SND_SLOT_TIMER[i] = CreateTimer()");
        sb.AppendLine("        set i = i + 1");
        sb.AppendLine("    endloop");
        sb.AppendLine("endfunction");
        sb.AppendLine();

        for (int e = 0; e < events.Count; e++)
        {
            var ev = events[e];
            sb.AppendLine($"// {ev.Name}, {ev.Catalog.Count} clip(s), {ev.MaxVoices} voice(s), priority {ev.Priority.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"function {ev.FunctionName} takes nothing returns boolean");
            sb.AppendLine($"    return SND_EventPlay({e.ToString(CultureInfo.InvariantCulture)}, {Real(ev.Cooldown)})");
            sb.AppendLine("endfunction");
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd() + "\n";
    }

    private static int FirstClip(IReadOnlyList<ResolvedEvent> events, int index)
    {
        int first = 0;
        for (int i = 0; i < index; i++) first += events[i].Catalog.Count;
        return first;
    }

    /// <summary>A JASS real literal. Always carries a decimal point, an integer literal in a real
    /// position is a compile error under pjass.</summary>
    private static string Real(double value)
    {
        var text = value.ToString("0.###", CultureInfo.InvariantCulture);
        return text.Contains('.') ? text : text + ".00";
    }

    /// <summary>A map path as the body of a JASS string literal. Backslashes are doubled because
    /// the game reads a backslash as an escape, and pjass rejects a bare one as an invalid escape
    /// sequence. Every other JASS path literal in this repo is written the same way, for example
    /// <c>AddSpecialEffect("war3mapImported\\x.mdx")</c>.</summary>
    private static string JassPath(string path) => path.Replace("\\", "\\\\");
}
