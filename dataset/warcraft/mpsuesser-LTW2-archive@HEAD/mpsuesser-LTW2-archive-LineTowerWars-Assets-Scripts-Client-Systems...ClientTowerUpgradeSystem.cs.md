---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/TowerUpgradeSystem/ClientTowerUpgradeSystem.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientTowerUpgradeSystem : SingletonBehaviour<ClientTowerUpgradeSystem>
{
    public class ActionProgress {
        public double ElapsedTime { get; set; }
        public double FullDuration { get; set; }
        public double TimeRemaining => FullDuration - ElapsedTime;

        public ActionProgress(double elapsedTime, double fullDuration) {
            ElapsedTime = elapsedTime;
            FullDuration = fullDuration;
        }
    }

    private Dictionary<ClientTower, ActionProgress> RemainingUpgradeTimeForTower { get; set; }
    private Dictionary<ClientTower, ActionProgress> RemainingSellTimeForTower { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        RemainingUpgradeTimeForTower = new Dictionary<ClientTower, ActionProgress>();
        RemainingSellTimeForTower = new Dictionary<ClientTower, ActionProgress>();
        
        EventBus.OnTowerSaleStarted += TowerSaleStarted;
        EventBus.OnTowerSaleCanceled += TowerSaleCanceled;
        EventBus.OnTowerSaleFinished += TowerSaleFinished;

        EventBus.OnTowerUpgradeStarted += TowerUpgradeStarted;
        EventBus.OnTowerUpgradeCanceled += TowerUpgradeCanceled;
        EventBus.OnTowerUpgradeFinished += TowerUpgradeFinished;
    }

    private void OnDestroy() {
        EventBus.OnTowerSaleStarted -= TowerSaleStarted;
        EventBus.OnTowerSaleCanceled -= TowerSaleCanceled;
        EventBus.OnTowerSaleFinished -= TowerSaleFinished;

        EventBus.OnTowerUpgradeStarted -= TowerUpgradeStarted;
        EventBus.OnTowerUpgradeCanceled -= TowerUpgradeCanceled;
        EventBus.OnTowerUpgradeFinished -= TowerUpgradeFinished;
    }

    private void Update() {
        if (RemainingUpgradeTimeForTower.Count > 0) {
            foreach (ClientTower t in RemainingUpgradeTimeForTower.Keys) {
                if (RemainingUpgradeTimeForTower[t].TimeRemaining > 0) {
                    RemainingUpgradeTimeForTower[t].ElapsedTime += Time.deltaTime;
                } 
            }
        }

        if (RemainingSellTimeForTower.Count > 0) {
            foreach (ClientTower t in RemainingSellTimeForTower.Keys) {
                if (RemainingSellTimeForTower[t].TimeRemaining > 0) {
                    RemainingSellTimeForTower[t].ElapsedTime += Time.deltaTime;
                }
            }
        }
    }

    private void TowerSaleStarted(ClientTower t) {
        RemainingSellTimeForTower.Add(t, new ActionProgress(0, TowerConstants.SellDuration));
        CallForTargetRefresh(t);
    }

    private void TowerSaleCanceled(ClientTower t) {
        RemainingSellTimeForTower.Remove(t);
        CallForTargetRefresh(t);
    }

    private void TowerSaleFinished(ClientTower t) {
        RemainingSellTimeForTower.Remove(t);
        CallForTargetRefresh(t);
    }

    private void TowerUpgradeStarted(ClientTower t, TowerType upgradedType, double upgradeTime) {
        RemainingUpgradeTimeForTower.Add(t, new ActionProgress(0, upgradeTime));
        CallForTargetRefresh(t);
    }

    private void TowerUpgradeCanceled(ClientTower t) {
        RemainingUpgradeTimeForTower.Remove(t);
        CallForTargetRefresh(t);
    }

    private void TowerUpgradeFinished(ClientTower oldTower, int newTowerEntityID) {
        RemainingUpgradeTimeForTower.Remove(oldTower);
        CallForTargetRefresh(oldTower);
    }

    private static void CallForTargetRefresh(ClientEntity e) {
        TargetSystem.Singleton.RefreshEntity(e);
    }

    public bool IsTowerInProgress(ClientTower t, out double remainingTime) {
        remainingTime = 10000;

        if (RemainingUpgradeTimeForTower.ContainsKey(t)) {
            remainingTime = RemainingUpgradeTimeForTower[t].TimeRemaining;
        }

        if (RemainingSellTimeForTower.ContainsKey(t)) {
            remainingTime = Math.Min(remainingTime, RemainingSellTimeForTower[t].TimeRemaining);
        }

        return (remainingTime < 10000);
    }
    public bool IsTowerUpgrading(ClientTower t) => RemainingUpgradeTimeForTower.ContainsKey(t);
    public bool IsTowerSelling(ClientTower t) => RemainingSellTimeForTower.ContainsKey(t);
    public ActionProgress GetUpgradeActionProgressForTower(ClientTower t) => RemainingUpgradeTimeForTower[t];
    public ActionProgress GetSellActionProgressForTower(ClientTower t) => RemainingSellTimeForTower[t];
}
