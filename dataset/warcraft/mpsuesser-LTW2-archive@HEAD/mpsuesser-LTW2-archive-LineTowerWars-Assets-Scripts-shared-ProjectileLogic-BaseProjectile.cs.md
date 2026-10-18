---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/ProjectileLogic/BaseProjectile.cs"
ref: HEAD
---

﻿using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour {
    public event Action<AttackEventData, GameObject> OnLanded;
    private float Speed { get; set; }
    private GameObject TrackTarget { get; set; }
    private Vector3 Destination {
        get {
            if (TrackTarget != null) {
                return TrackTarget.transform.position;
            } else {
                return _destination;
            }
        }
        set {
            _destination = value;
        }
    }
    private Vector3 _destination;
    private static float ReachedDestinationDistance = .1f;

    private float StartTime { get; set; }
    private float MaxTime { get; set; }
    private Vector3 StartPosition { get; set; }
    private float MaxDistance { get; set; }
    private AttackEventData EventData { get; set; }

    private void Awake() {
        Collider collider = GetComponentInChildren<Collider>(true);
        if (collider == null) {
            LTWLogger.Log($"Could not find collider in effect {gameObject.name}!");
        } else {
            collider.isTrigger = true;
        }

        StartTime = Time.time;
        StartPosition = transform.position;
        MaxTime = Mathf.Infinity;
        MaxDistance = Mathf.Infinity;
        EventData = null;
    }

    public void Load(ProjectileAttackDelivery delivery, GameObject target, AttackEventData eventData) {
        Speed = (float)delivery.ProjectileSpeed;
        MaxDistance = (float)delivery.MaxDistance;
        MaxTime = (float)delivery.MaxSeconds;
        if (delivery.TrackTarget) {
            TrackTarget = target;
        } else {
            Destination = target.transform.position;
        }

        EventData = eventData;
    }

    private void Update() {
        // For projectile pathing after the target has died
        if (TrackTarget != null) {
            Destination = TrackTarget.transform.position;
        }

        float distanceFromStart = (transform.position - StartPosition).magnitude;
        if (Time.time - StartTime > MaxTime || distanceFromStart > MaxDistance) {
            MaxedOut();
            return;
        }

        float distance = (Destination - transform.position).magnitude;
        if (distance < ReachedDestinationDistance) {
            ReachedDestination();
            return;
        }

        Vector3 direction = (Destination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        float step = (float)Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Destination, step);
    }

    private void OnTriggerEnter(Collider collider) {
        if (TrackTarget == null || collider.gameObject != TrackTarget) {
            return;
        }

        ReachedDestination();
    }

    protected virtual void ReachedDestination() {
        // Hacky, gets around an AttackSystem subscribed to this being destroyed while projectile is in midair throwing a runtime error. This event is only called once before the projectile is destroyed, so it's not like the problem will pile up and get worse over time. Not great but not really a big deal for now.
        try {
            OnLanded?.Invoke(EventData, TrackTarget);
        } catch (Exception) { }

        Destroy(gameObject);
    }

    protected virtual void MaxedOut() {
        Destroy(gameObject);
    }
}
