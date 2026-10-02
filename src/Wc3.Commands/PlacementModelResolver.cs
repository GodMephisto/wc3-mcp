// src/Wc3.Commands/PlacementModelResolver.cs
using System;
using System.Linq;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Resolves the on-disk model path for a placed widget type (unit / doodad / destructable)
/// so a viewport can draw the real model instead of a placeholder box. Lives in the command
/// layer — not the GUI — so the CLI, MCP, and Studio resolve identically and the logic is
/// hermetically testable (Principle #3: reuse Wc3.Commands, never reimplement in the UI).
///
/// Resolution order for a type:
///   1. the type's own model field (units <c>umdl</c>; destructables <c>bfil</c>; doodads <c>dfil</c>),
///   2. its Reforged skin-profile model (<see cref="RenderModelCommand.BaseModelPath"/>),
///   3. failing both, the SAME resolution on the base object it derives from — a *custom*
///      doodad/destructable that never overrides its art (e.g. GGGA's D00A→ARrk, D00V→LPcr)
///      inherits the base model rather than degrading to a placeholder box.
/// Returns null only when the whole chain yields nothing (a genuinely model-less type).
/// </summary>
public static class PlacementModelResolver
{
    public static string? ResolveModelPath(
        MapDocument doc, string rawcode, bool isUnit, string? gameDir)
        => ResolveModelPath(doc, rawcode, isUnit, gameDir, depth: 0);

    private static string? ResolveModelPath(
        MapDocument doc, string rawcode, bool isUnit, string? gameDir, int depth)
    {
        var kinds = isUnit
            ? new[] { (ObjectKind.Unit, "umdl") }
            : new[] { (ObjectKind.Destructable, "bfil"), (ObjectKind.Doodad, "dfil") };

        string? baseRawcode = null;
        foreach (var (kind, fieldCode) in kinds)
        {
            try
            {
                var merged = ObjectGetCommand.Execute(doc, kind, rawcode, gameDir);
                if (merged.Found)
                {
                    var field = merged.Fields.FirstOrDefault(f =>
                        string.Equals(f.Code, fieldCode, StringComparison.OrdinalIgnoreCase));
                    // Variation-heavy fields can list several paths comma-separated; use the first.
                    if (!string.IsNullOrWhiteSpace(field?.Value))
                        return field!.Value.Split(',')[0].Trim();
                    // No own art here — remember a differing base to inherit from if nothing resolves.
                    if (baseRawcode is null && merged.BaseRawcode is { Length: > 0 } b
                        && !string.Equals(b, rawcode, StringComparison.OrdinalIgnoreCase))
                        baseRawcode = b;
                }
            }
            catch
            {
                // fall through to the skin profile, then the next kind
            }
            if (RenderModelCommand.BaseModelPath(kind, rawcode, gameDir) is { Length: > 0 } skinPath)
                return skinPath;
        }

        // Custom type with no art of its own: inherit the base object's model. One hop is
        // usually enough; base objects report themselves as their own base (which the guard
        // above rejects), and the depth cap is a backstop against any pathological chain.
        if (baseRawcode is not null && depth < 4)
            return ResolveModelPath(doc, baseRawcode, isUnit, gameDir, depth + 1);

        return null;
    }
}
