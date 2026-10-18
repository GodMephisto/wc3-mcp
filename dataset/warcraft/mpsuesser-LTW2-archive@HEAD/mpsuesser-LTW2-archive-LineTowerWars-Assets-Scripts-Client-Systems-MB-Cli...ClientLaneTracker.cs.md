---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/ClientLaneTracker/ClientLaneTracker.cs"
ref: HEAD
---

﻿using System;
using System.Collections.Generic;

public class ClientLaneTracker : SingletonBehaviour<ClientLaneTracker>
{
    
    public Lane MyLane { get; set; }

    private HashSet<Action<Lane>> OnLivesUpdatedActionSubscriptions { get; set; }
    private HashSet<Action<Lane>> OnIncomeUpdatedActionSubscriptions { get; set; }
    private HashSet<Action<Lane>> OnGoldUpdatedActionSubscriptions { get; set; }
    private HashSet<Action<Lane>> OnActiveUnitsUpdatedActionSubscriptions { get; set; }
    private HashSet<Action<Lane>> OnTechUpdatedActionSubscriptions { get; set; }
    private HashSet<Action<Lane>> OnTechCostUpdatedActionSubscriptions { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        OnLivesUpdatedActionSubscriptions = new HashSet<Action<Lane>>();
        OnIncomeUpdatedActionSubscriptions = new HashSet<Action<Lane>>();
        OnGoldUpdatedActionSubscriptions = new HashSet<Action<Lane>>();
        OnActiveUnitsUpdatedActionSubscriptions = new HashSet<Action<Lane>>();
        OnTechUpdatedActionSubscriptions = new HashSet<Action<Lane>>();
        OnTechCostUpdatedActionSubscriptions = new HashSet<Action<Lane>>();

        EventBus.OnMyLaneUpdated += SetMyLane;
    }

    private void OnDestroy() {
        EventBus.OnMyLaneUpdated -= SetMyLane;
    }

    private void SetMyLane(Lane newLane) {
        foreach (Action<Lane> subscription in OnLivesUpdatedActionSubscriptions) {
            if (MyLane != null) {
                MyLane.OnLivesUpdated -= subscription;
            }

            newLane.OnLivesUpdated += subscription;
            subscription.Invoke(newLane);
        }

        foreach (Action<Lane> subscription in OnIncomeUpdatedActionSubscriptions) {
            if (MyLane != null) {
                MyLane.OnIncomeUpdated -= subscription;
            }

            newLane.OnIncomeUpdated += subscription;
            subscription.Invoke(newLane);
        }

        foreach (Action<Lane> subscription in OnGoldUpdatedActionSubscriptions) {
            if (MyLane != null) {
                MyLane.OnGoldUpdated -= subscription;
            }

            newLane.OnGoldUpdated += subscription;
            subscription.Invoke(newLane);
        }

        foreach (Action<Lane> subscription in OnActiveUnitsUpdatedActionSubscriptions) {
            if (MyLane != null) {
                MyLane.OnActiveUnitsUpdated -= subscription;
            }

            newLane.OnActiveUnitsUpdated += subscription;
            subscription.Invoke(newLane);
        }

        foreach (Action<Lane> subscription in OnTechUpdatedActionSubscriptions) {
            if (MyLane != null) {
                MyLane.OnTechUpdated -= subscription;
            }

            newLane.OnTechUpdated += subscription;
            subscription.Invoke(newLane);
        }

        foreach (Action<Lane> subscription in OnTechCostUpdatedActionSubscriptions) {
            if (MyLane != null) {
                MyLane.OnTechCostUpdated -= subscription;
            }

            newLane.OnTechCostUpdated += subscription;
            subscription.Invoke(newLane);
        }

        MyLane = newLane;
    }

    public void SubscribeToOnLivesUpdated(Action<Lane> subscription) {
        OnLivesUpdatedActionSubscriptions.Add(subscription);

        if (MyLane != null) {
            MyLane.OnLivesUpdated += subscription;
        }
    }

    public void SubscribeToOnIncomeUpdated(Action<Lane> subscription) {
        OnIncomeUpdatedActionSubscriptions.Add(subscription);

        if (MyLane != null) {
            MyLane.OnIncomeUpdated += subscription;
        }
    }

    public void SubscribeToOnGoldUpdated(Action<Lane> subscription) {
        OnGoldUpdatedActionSubscriptions.Add(subscription);

        if (MyLane != null) {
            MyLane.OnGoldUpdated += subscription;
        }
    }

    public void SubscribeToOnActiveUnitsUpdated(Action<Lane> subscription) {
        OnActiveUnitsUpdatedActionSubscriptions.Add(subscription);

        if (MyLane != null) {
            MyLane.OnActiveUnitsUpdated += subscription;
        }
    }

    public void SubscribeToOnTechUpdated(Action<Lane> subscription) {
        OnTechUpdatedActionSubscriptions.Add(subscription);

        if (MyLane != null) {
            MyLane.OnTechUpdated += subscription;
        }
    }

    public void SubscribeToOnTechCostUpdated(Action<Lane> subscription) {
        OnTechCostUpdatedActionSubscriptions.Add(subscription);

        if (MyLane != null) {
            MyLane.OnTechCostUpdated += subscription;
        }
    }
}