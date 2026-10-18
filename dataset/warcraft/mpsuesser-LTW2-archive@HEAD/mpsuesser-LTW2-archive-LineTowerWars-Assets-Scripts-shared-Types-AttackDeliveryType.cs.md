---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Types/AttackDeliveryType.cs"
ref: HEAD
---

﻿using UnityEngine;

public enum AttackDeliveryType {
    None = 0,
    Instant,
    ProjectileFromSource,
    ProjectileFromAboveTarget
}

public abstract class AttackDeliveryModifiers
{
    public abstract AttackDeliveryType Type { get; }

    public double InitialAnimationDelay { get; set; }
}

public abstract class ProjectileAttackDelivery : AttackDeliveryModifiers {
    public double ProjectileSpeed { get; set; }
    public bool TrackTarget { get; set; }
    public Vector3 ProjectileInitialOffset { get; set; }
    public double MaxDistance { get; set; }
    public double MaxSeconds { get; set; }
}

public class InstantAttackDelivery : AttackDeliveryModifiers
{
    public override AttackDeliveryType Type => AttackDeliveryType.Instant;
}

public class ProjectileFromSourceAttackDelivery : ProjectileAttackDelivery
{
    public override AttackDeliveryType Type => AttackDeliveryType.ProjectileFromSource;

}

public class ProjectileFromAboveTargetAttackDelivery : ProjectileAttackDelivery
{
    public override AttackDeliveryType Type => AttackDeliveryType.ProjectileFromAboveTarget;

}

public class NoAttackDelivery : AttackDeliveryModifiers
{
    public override AttackDeliveryType Type => AttackDeliveryType.None;
}