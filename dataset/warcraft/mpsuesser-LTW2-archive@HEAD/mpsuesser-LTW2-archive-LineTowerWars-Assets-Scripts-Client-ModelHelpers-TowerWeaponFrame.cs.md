---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/ModelHelpers/TowerWeaponFrame.cs"
ref: HEAD
---

﻿using UnityEngine;

public class TowerWeaponFrame : MonoBehaviour {
    private ClientEntity rotationTarget;
    private Quaternion rotationStartValue;
    private float rotationStartTime;
    private float rotationEndTime;
    private bool isRotating;
    private bool isReturning;

    private const float TimeUntilReset = 1f;
    private const float ResetDuration = 2f;

    private void Awake() {
        isRotating = false;
        isReturning = false;
    }
    
    public void RotateTowardEntityOverTime(ClientEntity entity, float overTime) {
        rotationTarget = entity;
        rotationStartValue = transform.rotation;
        rotationStartTime = Time.time;
        rotationEndTime = rotationStartTime + overTime;
        isRotating = true;
        isReturning = false;
    }

    private void Update() {
        if (!isRotating) {
            if (isReturning) {
                float returnFinishTime = rotationEndTime + TimeUntilReset + ResetDuration;
                if (Time.time > returnFinishTime) {
                    transform.rotation = Quaternion.identity;
                    isReturning = false;
                }
                else {
                    float timeOfReturnCompleted = Time.time - (rotationEndTime + TimeUntilReset);
                    transform.rotation = Quaternion.Lerp(
                        rotationStartValue,
                        Quaternion.identity,
                        timeOfReturnCompleted / ResetDuration
                    );
                }
            }
            return;
        }
        
        if (rotationTarget == null || Time.time > rotationEndTime) {
            isRotating = false;
            isReturning = true;
            rotationStartValue = transform.rotation;
            return;
        }

        float totalTime = rotationEndTime - rotationStartTime;
        float completedTime = Time.time - rotationStartTime;

        Vector3 dir = (rotationTarget.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Quaternion lerpedRotation = Quaternion.Lerp(
            rotationStartValue,
            lookRotation,
            completedTime / totalTime
        );
        
        transform.rotation = Quaternion.Euler(
            0,
            lerpedRotation.eulerAngles.y,
            0
        );
    }
}