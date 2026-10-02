// src/Wc3.Modeling/ModelPoser.cs
using System.Numerics;

namespace Wc3.Modeling;

/// <summary>
/// Evaluates a model's skeleton at one frame and bakes the result into new geoset
/// vertex/normal arrays (CPU skinning). Deliberately approximate where the format
/// allows it: linear interpolation stands in for hermite/bezier tracks, and classic
/// matrix-group skinning is a rigid average over the group's bones (classic MDX
/// stores no per-vertex weights). Every unexpected condition — missing bones,
/// parent cycles, zero weights — degrades toward bind pose rather than throwing.
/// </summary>
internal static class ModelPoser
{
    private const int MaxParentDepth = 512; // cycle guard; real skeletons are < 100 deep

    /// <summary>
    /// Poses <paramref name="model"/> at the first frame of the chosen sequence.
    /// Returns the model unchanged when nothing can be posed.
    /// </summary>
    public static Model3D Pose(Model3D model, string? sequenceName)
    {
        var skeleton = model.Skeleton;
        if (skeleton is null || skeleton.Nodes.Count == 0 || skeleton.Sequences.Count == 0)
            return model;

        var sequence = PickSequence(skeleton.Sequences, sequenceName);
        if (sequence is null)
            return model;
        int frame = sequence.IntervalStart; // first frame = the canonical rest pose of the sequence

        var nodesById = new Dictionary<int, ModelNode>(skeleton.Nodes.Count);
        foreach (var node in skeleton.Nodes)
            nodesById.TryAdd(node.ObjectId, node);
        var worlds = new Dictionary<int, Matrix4x4>(skeleton.Nodes.Count);

        var geosets = new List<Geoset>(model.Geosets.Count);
        bool anyPosed = false;
        foreach (var geoset in model.Geosets)
        {
            var posed = PoseGeoset(geoset, nodesById, worlds, sequence, frame);
            anyPosed |= !ReferenceEquals(posed, geoset);
            geosets.Add(posed);
        }
        if (!anyPosed)
            return model;

        // The output's geometry is already posed; dropping the skeleton makes the
        // result stable (posing a posed model is a no-op, never a double-apply).
        return new Model3D(geosets, model.Textures);
    }

    /// <summary>Exact name match first, then any prefixed variant (e.g. "Stand - 2"), then the first sequence.</summary>
    private static ModelSequence? PickSequence(IReadOnlyList<ModelSequence> sequences, string? name)
    {
        name = string.IsNullOrWhiteSpace(name) ? "Stand" : name;
        return sequences.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? sequences.FirstOrDefault(s => s.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
            ?? sequences.FirstOrDefault();
    }

    private static Geoset PoseGeoset(
        Geoset geoset,
        Dictionary<int, ModelNode> nodesById,
        Dictionary<int, Matrix4x4> worlds,
        ModelSequence sequence,
        int frame)
    {
        int vertexCount = geoset.Vertices.Length / 3;
        if (vertexCount == 0
            || geoset.SkinBones is not { } bones || geoset.SkinWeights is not { } weights
            || bones.Length != vertexCount * 4 || weights.Length != vertexCount * 4)
            return geoset; // no usable skinning data — bind pose

        var vertices = new float[geoset.Vertices.Length];
        var normals = new float[geoset.Normals.Length];
        bool hasNormals = geoset.Normals.Length == geoset.Vertices.Length;

        for (int i = 0; i < vertexCount; i++)
        {
            var bind = new Vector3(
                geoset.Vertices[i * 3], geoset.Vertices[i * 3 + 1], geoset.Vertices[i * 3 + 2]);
            var bindNormal = hasNormals
                ? new Vector3(geoset.Normals[i * 3], geoset.Normals[i * 3 + 1], geoset.Normals[i * 3 + 2])
                : Vector3.Zero;

            Vector3 position = Vector3.Zero, normal = Vector3.Zero;
            float weightSum = 0f;
            for (int slot = 0; slot < 4; slot++)
            {
                float w = weights[i * 4 + slot];
                if (w <= 0f) continue;
                var world = WorldOf(bones[i * 4 + slot], nodesById, worlds, sequence, frame, 0);
                position += w * Vector3.Transform(bind, world);
                normal += w * Vector3.TransformNormal(bindNormal, world);
                weightSum += w;
            }

            if (weightSum <= 1e-6f)
            {
                position = bind;   // weightless vertex stays in bind pose
                normal = bindNormal;
            }
            else
            {
                position /= weightSum;
                normal = normal.LengthSquared() > 1e-12f ? Vector3.Normalize(normal) : bindNormal;
            }

            vertices[i * 3] = position.X;
            vertices[i * 3 + 1] = position.Y;
            vertices[i * 3 + 2] = position.Z;
            if (hasNormals)
            {
                normals[i * 3] = normal.X;
                normals[i * 3 + 1] = normal.Y;
                normals[i * 3 + 2] = normal.Z;
            }
        }

        return geoset with { Vertices = vertices, Normals = hasNormals ? normals : geoset.Normals };
    }

    /// <summary>
    /// The node's world matrix at the frame: local pivot transform composed up the
    /// parent chain (memoized). Unknown node ids and over-deep chains (cycles)
    /// resolve to identity, which leaves affected vertices in bind pose.
    /// </summary>
    private static Matrix4x4 WorldOf(
        int objectId,
        Dictionary<int, ModelNode> nodesById,
        Dictionary<int, Matrix4x4> worlds,
        ModelSequence sequence,
        int frame,
        int depth)
    {
        if (worlds.TryGetValue(objectId, out var cached))
            return cached;
        if (depth > MaxParentDepth || !nodesById.TryGetValue(objectId, out var node))
            return Matrix4x4.Identity;

        var local = LocalOf(node, sequence, frame);
        var world = node.ParentId >= 0 && node.ParentId != objectId
            ? local * WorldOf(node.ParentId, nodesById, worlds, sequence, frame, depth + 1)
            : local;
        worlds[objectId] = world;
        return world;
    }

    /// <summary>
    /// Local transform: rotate/scale about the node's pivot, then translate.
    /// Row-vector composition (System.Numerics), so the leftmost factor applies first:
    /// v' = v · T(-pivot) · S · R · T(pivot + translation).
    /// </summary>
    private static Matrix4x4 LocalOf(ModelNode node, ModelSequence sequence, int frame)
    {
        if (node.Translation is null && node.Rotation is null && node.Scaling is null)
            return Matrix4x4.Identity; // untracked node: pure pass-through regardless of pivot

        var translation = Sample(node.Translation, sequence, frame, Vector3.Zero, Vector3.Lerp);
        var rotation = Sample(node.Rotation, sequence, frame, Quaternion.Identity, Quaternion.Slerp);
        var scale = Sample(node.Scaling, sequence, frame, Vector3.One, Vector3.Lerp);

        return Matrix4x4.CreateTranslation(-node.Pivot)
             * Matrix4x4.CreateScale(scale)
             * Matrix4x4.CreateFromQuaternion(rotation)
             * Matrix4x4.CreateTranslation(node.Pivot + translation);
    }

    /// <summary>
    /// Samples a track at a frame, restricted to keys inside the sequence interval
    /// (MDX keys live on a global timeline; other sequences' keys must not bleed in).
    /// Clamps to the nearest in-interval key at the edges and interpolates linearly
    /// between the bracketing pair — a deliberate simplification of hermite/bezier.
    /// Global-sequence tracks (independent looping timelines) sample their start.
    /// </summary>
    private static T Sample<T>(
        AnimTrack<T>? track, ModelSequence sequence, int frame, T fallback, Func<T, T, float, T> lerp)
        where T : struct
    {
        if (track is null || track.Keys.Count == 0)
            return fallback;

        int start = sequence.IntervalStart, end = sequence.IntervalEnd, target = frame;
        if (track.GlobalSequenceId >= 0)
        {
            start = 0;
            end = int.MaxValue;
            target = 0;
        }

        AnimKey<T>? before = null, after = null;
        foreach (var key in track.Keys)
        {
            if (key.Frame < start || key.Frame > end) continue;
            if (key.Frame <= target && (before is null || key.Frame > before.Frame)) before = key;
            if (key.Frame >= target && (after is null || key.Frame < after.Frame)) after = key;
        }

        if (before is null && after is null) return fallback;
        if (before is null) return after!.Value;
        if (after is null || after.Frame == before.Frame) return before.Value;

        float t = (target - before.Frame) / (float)(after.Frame - before.Frame);
        return lerp(before.Value, after.Value, t);
    }
}
