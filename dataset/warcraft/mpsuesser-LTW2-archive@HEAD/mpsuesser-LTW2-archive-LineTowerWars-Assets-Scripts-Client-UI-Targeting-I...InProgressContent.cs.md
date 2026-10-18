---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/InProgress/InProgressContent.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InProgressContent : MonoBehaviour
{
    private enum ProgressType {
        Upgrade,
        Sell
    }

    public event Action ProgressCanceled;

    [SerializeField] private TMP_Text DescriptionText;
    [SerializeField] private ProgressBar ProgBar;
    [SerializeField] private Button CancelButton;

    private ClientTower ActiveTower { get; set; }
    private ProgressType ActiveProgressType { get; set; }

    private void Awake() {
        CancelButton.onClick.AddListener(CancelClicked);
    }

    private void Update() {
        if (ActiveTower == null) {
            return;
        }

        if ((ActiveProgressType == ProgressType.Upgrade && !ClientTowerUpgradeSystem.Singleton.IsTowerUpgrading(ActiveTower)) || (ActiveProgressType == ProgressType.Sell && !ClientTowerUpgradeSystem.Singleton.IsTowerSelling(ActiveTower))) {
            ProgressCanceled?.Invoke();
            return;
        }

        LoadAndShowForEntity(ActiveTower);
    }

    public void Hide() {
        ActiveTower = null;
        gameObject.SetActive(false);
    }

    public void LoadAndShowForEntity(ClientEntity e) {
        if (!(e is ClientTower t)) {
            return;
        }
        
        ClientTowerUpgradeSystem.ActionProgress AP;
        if (ClientTowerUpgradeSystem.Singleton.IsTowerUpgrading(t)) {
            AP = ClientTowerUpgradeSystem.Singleton.GetUpgradeActionProgressForTower(t);
            DescriptionText.SetText("Upgrading...");
            ActiveProgressType = ProgressType.Upgrade;
        } else {
            AP = ClientTowerUpgradeSystem.Singleton.GetSellActionProgressForTower(t);
            DescriptionText.SetText("Selling...");
            ActiveProgressType = ProgressType.Sell;
        }

        ActiveTower = t;
        ProgBar.UpdateProgress(AP.ElapsedTime, AP.FullDuration);

        gameObject.SetActive(true);
    }

    private void CancelClicked() {
        if (ActiveTower == null) {
            return;
        }

        HashSet<ClientTower> towersToCancel = new HashSet<ClientTower>();
        foreach (ClientEntity e in TargetSystem.Singleton.Targets) {
            if (!(e is ClientTower t)) {
                continue;
            }

            if ((ActiveProgressType == ProgressType.Upgrade && ClientTowerUpgradeSystem.Singleton.IsTowerUpgrading(t)) || (ActiveProgressType == ProgressType.Sell && ClientTowerUpgradeSystem.Singleton.IsTowerSelling(t))) {
                towersToCancel.Add(t);
            }
        }

        if (towersToCancel.Count == 0) {
            return;
        }

        if (ActiveProgressType == ProgressType.Upgrade) {
            ClientSend.RequestTowerUpgradeCancellation(towersToCancel);
        } else {
            ClientSend.RequestTowerSaleCancellation(towersToCancel);
        }
    }
}
