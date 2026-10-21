// src/Wc3.Commands/ModelExtFixCommand.cs
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One rewritten field value.</summary>
public sealed record ModelExtFixEdit(
    string Kind, string Rawcode, string? Name, string Field, string Before, string After);

/// <summary>
/// The remap result. <see cref="ReferencesChanged"/> counts individual model references,
/// <see cref="FieldsChanged"/> the object fields that held them, and
/// <see cref="NoTwinPaths"/> the references left alone because no .mdx twin exists.
/// </summary>
public sealed record ModelExtFixResult(
    bool Ok,
    string Message,
    int ObjectsScanned,
    int FieldsChanged,
    int ReferencesChanged,
    IReadOnlyList<ModelExtFixEdit> Edits,
    IReadOnlyList<string> NoTwinPaths,
    IReadOnlyList<string> FailedWrites,
    int Redirected);

/// <summary>
/// Rewrite object-data model paths that name a .mdl the archive does not hold, when the
/// identical stem exists as .mdx. The archive resolves paths exactly, so a .mdl reference is
/// a model the engine can never load. Rewriting it can only turn a missing model into the
/// file that is actually there, so it cannot regress a working reference. Base-game paths are
/// untouched, since those resolve through CASC and are not this archive's to fix.
/// </summary>
public static class ModelExtFixCommand
{
    public static ModelExtFixResult Execute(MapDocument doc, bool apply, string? gameDirOverride = null, string? redirectMissing = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _);

        var edits = new List<ModelExtFixEdit>();
        var noTwin = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        var failed = new List<string>();
        int objects = 0, fields = 0, refs = 0, redirected = 0;

        foreach (var kind in ObjectKinds.All)
        {
            foreach (var obj in ObjectScan.Kind(doc, ctx, kind))
            {
                objects++;
                foreach (var (code, value) in obj.DeltaFields)
                {
                    if (value is null || value.IndexOf(".mdl", StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    var parts = value.Split(',');
                    bool changed = false;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string reference = parts[i].Trim();
                        if (!reference.EndsWith(".mdl", StringComparison.OrdinalIgnoreCase)) continue;
                        if (IsBaseGamePath(reference))
                            continue; // base-game path, resolved through CASC
                        if (doc.HasFileByName(reference)) continue; // the .mdl really exists

                        string? resolved = null;
                        string twin = reference[..^4] + ".mdx";
                        if (doc.HasFileByName(twin))
                        {
                            resolved = twin;
                        }
                        else
                        {
                            string collapsed = CollapseDuplicatePrefix(reference);
                            if (!string.Equals(collapsed, reference, StringComparison.Ordinal))
                            {
                                string collapsedTwin = collapsed[..^4] + ".mdx";
                                if (doc.HasFileByName(collapsedTwin)) resolved = collapsedTwin;
                                else if (doc.HasFileByName(collapsed)) resolved = collapsed;
                            }
                        }
                        if (resolved is null && redirectMissing is not null
                            && doc.HasFileByName(redirectMissing))
                        {
                            resolved = redirectMissing;
                            redirected++;
                        }
                        if (resolved is null)
                        {
                            noTwin.Add(reference);
                            continue;
                        }
                        parts[i] = resolved;
                        changed = true;
                        refs++;
                    }

                    if (!changed) continue;
                    string after = string.Join(",", parts);
                    if (apply)
                    {
                        var write = ObjectSetCommand.Execute(doc, kind, obj.Rawcode, code, after);
                        if (!write.Ok)
                        {
                            failed.Add($"{kind} {obj.Rawcode} {code}: {write.Message}");
                            continue;
                        }
                    }
                    fields++;
                    edits.Add(new ModelExtFixEdit(kind.ToString(), obj.Rawcode, obj.Name, code, value, after));
                }
            }
        }

        string verb = apply ? "rewrote" : "would rewrite";
        string message = $"{verb} {refs} model reference(s) across {fields} field(s) in {objects} object(s)"
            + (noTwin.Count > 0 ? $", {noTwin.Count} reference(s) have no .mdx twin and were left alone" : "");
        if (failed.Count > 0)
            message += $", {failed.Count} field(s) could not be written";
        if (redirected > 0)
            message += $", {redirected} unresolved reference(s) redirected to {redirectMissing}";
        return new ModelExtFixResult(true, message, objects, fields, refs, edits, noTwin.ToList(), failed, redirected);
    }

    /// <summary>Collapse a doubled <c>war3mapImported\war3mapImported\</c> prefix, which some
    /// authoring pipelines emit. The doubled path resolves to nothing, the collapsed one often
    /// names a file that is there, so collapsing can only turn a missing model into a real one.</summary>
    private static string CollapseDuplicatePrefix(string path)
    {
        const string prefix = "war3mapimported";
        if (path.Length < prefix.Length * 2 + 2) return path;
        char sep = path[prefix.Length];
        if (sep.ToString() != "\\" && sep.ToString() != "/") return path;
        string doubled = prefix + sep + prefix + sep;
        return path.StartsWith(doubled, StringComparison.OrdinalIgnoreCase)
            ? path.Substring(prefix.Length + 1)
            : path;
    }

    /// <summary>True for the roots the install resolves through CASC rather than the archive, so a
    /// path under them is not this archive's to repair. A bare filename or a doubled war3mapimported
    /// prefix is not one of these, which is how the universal dummy and caster dummy get caught.</summary>
    private static bool IsBaseGamePath(string path)
    {
        char sep = (char)92;
        string p = path.Replace((char)47, sep).TrimStart(sep);
        string[] roots =
        {
            "units" + sep, "abilities" + sep, "doodads" + sep, "destructables" + sep,
            "sharedmodels" + sep, "replaceabletextures" + sep, "sound" + sep, "ui" + sep,
            "environment" + sep
        };
        foreach (string root in roots)
        {
            if (p.StartsWith(root, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}
