// src/Wc3.Modeling/Skeleton.cs
using System.Numerics;

namespace Wc3.Modeling;

/// <summary>
/// Skeletal animation data parsed from a model: named sequences (Stand, Walk, ...)
/// and the node hierarchy (bones + helpers) with their keyframe tracks. Optional —
/// models whose animation data is absent or unparseable simply have no skeleton
/// and render in bind pose.
/// </summary>
public sealed record ModelSkeleton(
    IReadOnlyList<ModelSequence> Sequences,
    IReadOnlyList<ModelNode> Nodes);

/// <summary>One animation sequence: name plus its [start, end] frame interval on the model's global timeline.</summary>
public sealed record ModelSequence(string Name, int IntervalStart, int IntervalEnd);

/// <summary>
/// One skeleton node (BONE or HELP). <paramref name="ObjectId"/> is the id other
/// nodes and vertex matrix-groups reference; <paramref name="ParentId"/> is -1 for
/// roots. <paramref name="Pivot"/> is the rotation/scale origin. Tracks are null
/// when the node has no keys of that kind (identity).
/// </summary>
public sealed record ModelNode(
    string Name,
    int ObjectId,
    int ParentId,
    Vector3 Pivot,
    AnimTrack<Vector3>? Translation,
    AnimTrack<Quaternion>? Rotation,
    AnimTrack<Vector3>? Scaling);

/// <summary>
/// A keyframe track. Keys are ordered by frame on the model's global timeline.
/// <paramref name="GlobalSequenceId"/> ≥ 0 means the track runs on an independent
/// looping timeline instead of the sequence interval.
/// </summary>
public sealed record AnimTrack<T>(IReadOnlyList<AnimKey<T>> Keys, int GlobalSequenceId) where T : struct;

/// <summary>One keyframe: frame number and value (Vector3 for translation/scale, Quaternion for rotation).</summary>
public sealed record AnimKey<T>(int Frame, T Value) where T : struct;
