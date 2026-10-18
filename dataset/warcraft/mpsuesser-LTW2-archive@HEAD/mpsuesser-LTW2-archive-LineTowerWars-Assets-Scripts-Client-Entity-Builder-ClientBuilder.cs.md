---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Builder/ClientBuilder.cs"
ref: HEAD
---

﻿using UnityEngine;

public class ClientBuilder : ClientEntity {
    public static ClientBuilder Create(
        int entityID,
        Lane lane,
        Vector3 location,
        Quaternion rotation
    ) {
        ClientBuilder builder = (ClientBuilder) CreateEntity(
            ClientPrefabs.Singleton.pfBuilder,
            DynamicObjects.Singleton.Builders,
            entityID,
            1,
            1, 
            1,
            1,
            lane,
            location,
            rotation
        );

        return builder;
    }
    
    private Animator Anim { get; set; }
    private static readonly int BuildAnimationTrigger =
        Animator.StringToHash("TriggerBuild");
    private static readonly int AttackAnimationTrigger =
        Animator.StringToHash("TriggerAttack");
    private static readonly int StartRunningAnimationTrigger =
        Animator.StringToHash("TriggerStartRunning");
    private static readonly int StopRunningAnimationTrigger =
        Animator.StringToHash("TriggerStopRunning");

    /*private const int MovementCheckOverNumFrames = 100;
    private const float MovementDistanceOverFramesThreshold = .5f;
    
    private int NumFramesSinceConfirmedMoving { get; set; }
    private bool IsMoving { get; set; }
    private Vector3 MovingSnapshotLocation { get; set; }*/
    
    protected override void Awake() {
        Anim = GetComponentInChildren<Animator>();

        /*NumFramesSinceConfirmedMoving = 0;
        IsMoving = false;*/

        base.Awake();
    }

    // This might all be unnecessary with the addition of the isMoving flag passed
    // down from the server in the SyncBuilderPosition message.
    /*private void LateUpdate() {
        if (!IsMoving) return;
        if (
            Vector3.Distance(
                transform.position,
                MovingSnapshotLocation
            ) > MovementDistanceOverFramesThreshold
        ) {
            SetMovingFlag();
            return;
        }
        
        if (++NumFramesSinceConfirmedMoving >= MovementCheckOverNumFrames) {
            UnsetMovingFlag();
        }
    }*/

    public override void HandleAttackEvent(AttackEventData eventData) {
        TriggerAttackAnimation();
    }

    private void TriggerStartRunningAnimation() {
        Anim.ResetTrigger(StopRunningAnimationTrigger);
        Anim.SetTrigger(StartRunningAnimationTrigger);
    }

    private void TriggerStopRunningAnimation() {
        Anim.SetTrigger(StopRunningAnimationTrigger);
    }

    public void TriggerBuildAnimation() {
        Anim.SetTrigger(BuildAnimationTrigger);
        UnsetMovingFlag();
    }

    public void TriggerAttackAnimation() {
        Anim.SetTrigger(AttackAnimationTrigger);
        UnsetMovingFlag();
    }

    public void SetMovingFlag() {
        // if (!IsMoving) {
            TriggerStartRunningAnimation();
        /*}
        
        IsMoving = true;
        NumFramesSinceConfirmedMoving = 0;
        MovingSnapshotLocation = transform.position;*/
    }

    public void UnsetMovingFlag() {
        // if (IsMoving) {
            TriggerStopRunningAnimation();
        /*}
        
        IsMoving = false;
        NumFramesSinceConfirmedMoving = 0;*/
    }
}