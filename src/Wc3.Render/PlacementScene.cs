// src/Wc3.Render/PlacementScene.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Modeling;

namespace Wc3.Render;

/// <summary>One textured draw range inside a <see cref="PlacementMesh"/>: the indices
/// [<see cref="IndexOffset"/>, +<see cref="IndexCount"/>) sample
/// <c>Textures[TextureSlot]</c>; slot -1 means untextured (flat lit color).
/// <paramref name="FilterMode"/> is the source geoset's material blend mode so the
/// viewport can blend glows/cutouts correctly. <paramref name="IsTeamColor"/> marks a
/// section whose geoset samples an undecoded <c>ReplaceableId</c> (team-color) texture
/// inside an otherwise-textured model — the shader tints it by the owning player's
/// color instead of drawing it gray (its vertex color is white so the tint lands pure).</summary>
public sealed record PlacementMeshSection(
    int IndexOffset,
    int IndexCount,
    int TextureSlot,
    FilterMode FilterMode = FilterMode.None,
    bool IsTeamColor = false);

/// <summary>
/// One unique renderable mesh (a placement type's model, or the fallback box) in model
/// space. Vertices are interleaved <c>position(3) · normal(3) · uv(2) · color(3)</c>;
/// indices are a triangle list split into per-texture <see cref="Sections"/>, with the
/// decoded texture images the sections reference in <see cref="Textures"/>. The same
/// mesh is drawn once per placement with a per-instance world transform, so a map with
/// hundreds of Footmen uploads the Footman (geometry and textures) exactly once.
/// <paramref name="BoundsMin"/>/<paramref name="BoundsMax"/> is the LOCAL-space
/// axis-aligned bounding box of the vertex positions (both zero for an empty mesh) —
/// transform by an instance's <see cref="PlacementInstance.World"/> to ray-pick it.
/// </summary>
public sealed record PlacementMesh(
    string Key,
    float[] Vertices,
    uint[] Indices,
    IReadOnlyList<PlacementMeshSection> Sections,
    IReadOnlyList<TextureImage> Textures,
    Vector3 BoundsMin = default,
    Vector3 BoundsMax = default)
{
    /// <summary>Floats per vertex: px,py,pz, nx,ny,nz, u,v, r,g,b.</summary>
    public const int Stride = 11;

    public int VertexCount => Vertices.Length / Stride;
}

/// <summary>One placed widget: which mesh to draw and its world transform
/// (scale · Z-rotation · translation, System.Numerics row-vector convention).
/// <paramref name="OwnerId"/> is the owning player for units (-1 for doodads, which
/// have no owner) and feeds team-color tinting. <paramref name="CreationNumber"/> is
/// the widget's stable identity for pick and edit round-trips, from
/// <c>war3mapUnits.doo</c> for units and <c>war3map.doo</c> for doodads. The two
/// creation-number spaces can overlap, so <paramref name="IsUnit"/> disambiguates
/// which file (and which instance editor) the number belongs to.</summary>
public sealed record PlacementInstance(
    string MeshKey,
    Matrix4x4 World,
    int OwnerId = -1,
    int? CreationNumber = null,
    bool IsUnit = false);

/// <summary>Everything the GL viewport needs to draw a map's placements: the unique
/// meshes plus one instance per placed unit/doodad referencing its mesh by key.</summary>
public sealed record PlacementSceneData(
    IReadOnlyList<PlacementMesh> Meshes,
    IReadOnlyList<PlacementInstance> Instances,
    int UnitCount,
    int DoodadCount);

/// <summary>A placement type's resolved render model: posed geometry plus its decoded
/// textures keyed by the model's texture index (the shape
/// <c>RenderModelCommand.Prepare(...).Textures</c> returns). Geosets whose texture is
/// absent from the dictionary (ReplaceableId/team color, missing file) draw untextured.</summary>
public sealed record PlacementModel(Model3D Model, IReadOnlyDictionary<int, TextureImage> Textures);

/// <summary>
/// Builds a <see cref="PlacementSceneData"/> from a parsed map document: units from
/// <c>war3mapUnits.doo</c>, doodads/destructables from <c>war3map.doo</c>. Each unique
/// type resolves its real model once (via the caller's resolver) with per-geoset UV
/// texturing; types without a resolvable model share an upright unit-sized box mesh in
/// the kind's color. Every instance stands on the terrain (Z sampled from the heightmap)
/// at its (x, y), rotated by its facing and scaled per the widget data.
/// </summary>
public static class PlacementScene
{
    /// <summary>Fallback-box color for units (light blue).</summary>
    public static readonly Vector3 UnitColor = new(0.30f, 0.62f, 1.00f);

    /// <summary>Fallback-box color for doodads (orange).</summary>
    public static readonly Vector3 DoodadColor = new(1.00f, 0.62f, 0.15f);

    /// <summary>Flat lit color for geosets without a resolvable texture — the same
    /// neutral gray the CPU <see cref="ModelRenderer"/> uses (190/255).</summary>
    public static readonly Vector3 UntexturedColor = new(0.745f, 0.745f, 0.745f);

    public const string UnitBoxKey = "box:unit";
    public const string DoodadBoxKey = "box:doodad";

    /// <summary>WC3 start-location marker: stored in <c>war3mapUnits.doo</c> as a unit but
    /// carries no renderable model (the World Editor shows a special flag, in-game it is
    /// invisible). Skipped entirely so it never renders as a placeholder box.</summary>
    public const string StartLocationRawcode = "sloc";

    // Fallback box: roughly a unit's footprint and stature (before placement scale).
    private const float BoxHalf = 28f;
    private const float BoxHeight = 112f;

    /// <summary>Boxes only (no model resolution) — units blue, doodads orange.</summary>
    public static PlacementSceneData Build(Wc3.Model.MapDocument doc)
        => Build(doc, resolveModel: null);

    /// <summary>
    /// Builds the scene. <paramref name="resolveModel"/> maps a type (rawcode, isUnit)
    /// to its render model — return null for "no model, use the box". It is invoked at
    /// most once per unique type per call, and the resulting mesh is shared by every
    /// placement of that type.
    /// </summary>
    public static PlacementSceneData Build(
        Wc3.Model.MapDocument doc,
        Func<string, bool, PlacementModel?>? resolveModel)
    {
        var units = (doc.GetFile("war3mapUnits.doo")?.Model as MapUnits)?.Units;
        var doodads = (doc.GetFile("war3map.doo")?.Model as MapDoodads)?.Doodads;
        int unitCount = units?.Count ?? 0;
        int doodadCount = doodads?.Count ?? 0;

        var heights = TerrainHeightField.TryCreate(doc);
        var meshes = new List<PlacementMesh>();
        var meshKeyByType = new Dictionary<(string Rawcode, bool IsUnit), string?>();
        bool unitBoxAdded = false, doodadBoxAdded = false;
        var instances = new List<PlacementInstance>(unitCount + doodadCount);

        // The mesh key for a type, or null to SKIP the placement entirely (no box). When a
        // real resolver is supplied, a type it can't turn into drawable geometry is skipped:
        // either no model at all (pathing blocker, ability doodad) or a mesh-less model whose
        // visual is pure particles/light (e.g. a bubble geyser) that we don't render anyway.
        // Only the no-resolver debug path (Build(doc)) still boxes, so placements are visible
        // in tests and quick inspection.
        string? MeshKeyFor(string rawcode, bool isUnit)
        {
            var type = (rawcode, isUnit);
            if (meshKeyByType.TryGetValue(type, out var known))
                return known;

            string? key;
            var model = resolveModel?.Invoke(rawcode, isUnit);
            if (model is not null && IsTeamColorEffect(model))
                model = null; // glow/effect plane (all ReplaceableId, e.g. GeneralHeroGlow) —
                              // renders as an opaque white/gray box without additive blend, so skip
            var mesh = model is null ? null : BuildModelMesh($"{(isUnit ? "unit" : "doodad")}:{rawcode}", model);
            if (mesh is not null)
            {
                meshes.Add(mesh);
                key = mesh.Key;
            }
            else if (resolveModel is not null)
            {
                key = null; // resolver consulted, no drawable geometry -> skip, don't box
            }
            else if (isUnit)
            {
                if (!unitBoxAdded) { meshes.Add(BuildBoxMesh(UnitBoxKey, UnitColor)); unitBoxAdded = true; }
                key = UnitBoxKey;
            }
            else
            {
                if (!doodadBoxAdded) { meshes.Add(BuildBoxMesh(DoodadBoxKey, DoodadColor)); doodadBoxAdded = true; }
                key = DoodadBoxKey;
            }
            meshKeyByType[type] = key;
            return key;
        }

        if (units is not null)
            foreach (var u in units)
            {
                var rawcode = u.TypeId.ToRawcode();
                if (string.Equals(rawcode, StartLocationRawcode, StringComparison.OrdinalIgnoreCase))
                    continue; // model-less editor marker — don't box it
                if (MeshKeyFor(rawcode, isUnit: true) is { } key)
                    instances.Add(new PlacementInstance(
                        key, WorldOf(u.Position, u.Rotation, u.Scale, heights),
                        u.OwnerId, u.CreationNumber, IsUnit: true));
            }
        if (doodads is not null)
            foreach (var d in doodads)
                if (MeshKeyFor(d.TypeId.ToRawcode(), isUnit: false) is { } key)
                    instances.Add(new PlacementInstance(
                        key, WorldOf(d.Position, d.Rotation, d.Scale, heights),
                        CreationNumber: d.CreationNumber));

        return new PlacementSceneData(meshes, instances, unitCount, doodadCount);
    }

    /// <summary>
    /// True when a model's whole surface is team-color/effect texturing (every geoset either
    /// references a <c>ReplaceableId</c> texture or has none) with no real diffuse texture
    /// anywhere — a glow/effect plane such as <c>GeneralHeroGlow</c>. We don't decode
    /// replaceable textures or additive blending, so such a model would render as an opaque
    /// white/gray quad ("white box"); it is skipped instead. A model with even one resolved
    /// texture (a normal unit/doodad, including one with team-colored trim) is NOT an effect
    /// and renders normally, its untextured geosets shaded neutral gray.
    /// </summary>
    public static bool IsTeamColorEffect(PlacementModel pm)
    {
        bool anyResolved = false, anyReplaceable = false;
        foreach (var g in pm.Model.Geosets)
        {
            if (g.TextureId >= 0 && pm.Textures.ContainsKey(g.TextureId))
                anyResolved = true;
            else if (g.TextureId >= 0 && g.TextureId < pm.Model.Textures.Count
                     && pm.Model.Textures[g.TextureId]
                         .StartsWith("ReplaceableId", StringComparison.OrdinalIgnoreCase))
                anyReplaceable = true;
        }
        return !anyResolved && anyReplaceable;
    }

    /// <summary>World transform for one placement: scale, face (Z-rotation, radians as
    /// the .doo format stores), then translate to (x, y, terrain height). Without terrain
    /// the widget's stored Z is used.</summary>
    private static Matrix4x4 WorldOf(Vector3 pos, float rotation, Vector3 scale, TerrainHeightField? heights)
    {
        float z = heights?.Sample(pos.X, pos.Y) ?? pos.Z;
        return Matrix4x4.CreateScale(SafeScale(scale.X), SafeScale(scale.Y), SafeScale(scale.Z))
             * Matrix4x4.CreateRotationZ(rotation)
             * Matrix4x4.CreateTranslation(pos.X, pos.Y, z);
    }

    /// <summary>Widget scale components of 0 mean "unset" in some tools; treat as 1.</summary>
    private static float SafeScale(float s) => MathF.Abs(s) < 1e-3f ? 1f : s;

    /// <summary>
    /// Flattens the model's geosets into one indexed mesh: positions/normals/UVs pass
    /// through (bind/posed model space, feet on the local Z=0 plane), and each geoset
    /// becomes a section bound to its texture (deduplicated across geosets). Geosets
    /// whose texture is unavailable get slot -1 and a neutral flat color. Returns null
    /// when the model has no valid triangles.
    /// </summary>
    public static PlacementMesh? BuildModelMesh(string key, PlacementModel pm)
    {
        var geosets = pm.Model.Geosets;
        int vertTotal = 0;
        foreach (var g in geosets)
            vertTotal += g.Vertices.Length / 3;
        if (vertTotal == 0)
            return null;

        var verts = new float[vertTotal * PlacementMesh.Stride];
        var indices = new List<uint>();
        var sections = new List<PlacementMeshSection>();
        var textures = new List<TextureImage>();
        var slotByTextureId = new Dictionary<int, int>();
        int vo = 0;      // write offset into verts
        uint baseV = 0;  // index offset of the current geoset's first vertex

        foreach (var g in geosets)
        {
            int slot = -1;
            if (g.TextureId >= 0 && pm.Textures.TryGetValue(g.TextureId, out var tex))
            {
                if (!slotByTextureId.TryGetValue(g.TextureId, out slot))
                {
                    slot = textures.Count;
                    textures.Add(tex);
                    slotByTextureId[g.TextureId] = slot;
                }
            }
            // Undecoded ReplaceableId texture inside an otherwise-textured model: a
            // team-colored section (trim, banner) the shader tints by the owner's color.
            bool isTeamColor = slot < 0
                && g.TextureId >= 0
                && g.TextureId < pm.Model.Textures.Count
                && pm.Model.Textures[g.TextureId]
                    .StartsWith("ReplaceableId", StringComparison.OrdinalIgnoreCase);

            // Textured sections ignore the color channel (white); team-color sections are
            // white so the owner tint lands pure; other untextured sections shade gray.
            var color = slot >= 0 || isTeamColor ? Vector3.One : UntexturedColor;

            int n = g.Vertices.Length / 3;
            for (int i = 0; i < n; i++)
            {
                int o = i * 3, uo = i * 2;
                verts[vo++] = g.Vertices[o];
                verts[vo++] = g.Vertices[o + 1];
                verts[vo++] = g.Vertices[o + 2];
                if (o + 2 < g.Normals.Length)
                {
                    verts[vo++] = g.Normals[o];
                    verts[vo++] = g.Normals[o + 1];
                    verts[vo++] = g.Normals[o + 2];
                }
                else
                {
                    verts[vo++] = 0f; verts[vo++] = 0f; verts[vo++] = 1f;
                }
                if (uo + 1 < g.Uvs.Length)
                {
                    verts[vo++] = g.Uvs[uo];
                    verts[vo++] = g.Uvs[uo + 1];
                }
                else
                {
                    verts[vo++] = 0f; verts[vo++] = 0f;
                }
                verts[vo++] = color.X; verts[vo++] = color.Y; verts[vo++] = color.Z;
            }

            int sectionStart = indices.Count;
            int count = g.Indices.Length - g.Indices.Length % 3;
            for (int t = 0; t + 2 < count; t += 3)
            {
                int a = g.Indices[t], b = g.Indices[t + 1], c = g.Indices[t + 2];
                if ((uint)a >= (uint)n || (uint)b >= (uint)n || (uint)c >= (uint)n)
                    continue; // malformed triangle — skip it whole
                indices.Add(baseV + (uint)a);
                indices.Add(baseV + (uint)b);
                indices.Add(baseV + (uint)c);
            }
            if (indices.Count > sectionStart)
                sections.Add(new PlacementMeshSection(
                    sectionStart, indices.Count - sectionStart, slot, g.FilterMode, isTeamColor));
            baseV += (uint)n;
        }

        if (indices.Count == 0)
            return null;
        var (bmin, bmax) = ComputeBounds(verts);
        return new PlacementMesh(key, verts, indices.ToArray(), sections, textures, bmin, bmax);
    }

    /// <summary>Local-space AABB of the interleaved vertex positions (stride offsets
    /// 0..2). Both corners zero when there are no vertices.</summary>
    private static (Vector3 Min, Vector3 Max) ComputeBounds(float[] verts)
    {
        if (verts.Length < 3)
            return (Vector3.Zero, Vector3.Zero);
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        for (int i = 0; i + 2 < verts.Length; i += PlacementMesh.Stride)
        {
            var p = new Vector3(verts[i], verts[i + 1], verts[i + 2]);
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
        }
        return (min, max);
    }

    /// <summary>Upright unit-sized box with its base on the local Z=0 plane — the shared
    /// fallback mesh for types whose model cannot be resolved. Flat per-face normals
    /// (24 verts / 36 indices), untextured single section, kind-colored.</summary>
    public static PlacementMesh BuildBoxMesh(string key, Vector3 color)
    {
        var verts = new List<float>(24 * PlacementMesh.Stride);
        var indices = new List<uint>(36);

        void Face(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal)
        {
            uint baseV = (uint)(verts.Count / PlacementMesh.Stride);
            foreach (var p in new[] { a, b, c, d })
            {
                verts.Add(p.X); verts.Add(p.Y); verts.Add(p.Z);
                verts.Add(normal.X); verts.Add(normal.Y); verts.Add(normal.Z);
                verts.Add(0f); verts.Add(0f); // untextured — UV unused
                verts.Add(color.X); verts.Add(color.Y); verts.Add(color.Z);
            }
            indices.Add(baseV); indices.Add(baseV + 1); indices.Add(baseV + 2);
            indices.Add(baseV); indices.Add(baseV + 2); indices.Add(baseV + 3);
        }

        const float h = BoxHalf;
        const float top = BoxHeight;
        var p000 = new Vector3(-h, -h, 0f);
        var p100 = new Vector3(h, -h, 0f);
        var p110 = new Vector3(h, h, 0f);
        var p010 = new Vector3(-h, h, 0f);
        var p001 = new Vector3(-h, -h, top);
        var p101 = new Vector3(h, -h, top);
        var p111 = new Vector3(h, h, top);
        var p011 = new Vector3(-h, h, top);

        Face(p000, p100, p101, p001, new Vector3(0, -1, 0)); // south
        Face(p100, p110, p111, p101, new Vector3(1, 0, 0));  // east
        Face(p110, p010, p011, p111, new Vector3(0, 1, 0));  // north
        Face(p010, p000, p001, p011, new Vector3(-1, 0, 0)); // west
        Face(p001, p101, p111, p011, new Vector3(0, 0, 1));  // top
        Face(p000, p010, p110, p100, new Vector3(0, 0, -1)); // bottom

        var vertArray = verts.ToArray();
        var (bmin, bmax) = ComputeBounds(vertArray);
        return new PlacementMesh(
            key, vertArray, indices.ToArray(),
            new[] { new PlacementMeshSection(0, indices.Count, -1) },
            Array.Empty<TextureImage>(), bmin, bmax);
    }
}
