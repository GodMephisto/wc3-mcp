---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Options/TargetOptions.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;

public class TargetOptions : MonoBehaviour
{
    [SerializeField] private OptionsContent OptionsContent;
    [SerializeField] private InProgressContent InProgressContent;

    private void Start() {
        InProgressContent.ProgressCanceled += InProgressContentCanceled;
    }

    public void ClearAndHide() {
        OptionsContent.Hide();
        InProgressContent.Hide();
    }

    // TODO: This function is kind of gross, it should be refactored
    public void ShowForTargets(List<ClientEntity> targets) {
        ClientEntity target = null;
        double lowestRemainingDuration = Mathf.Infinity;
        bool targetIsInProgress = false;
        foreach (ClientEntity e in targets) {
            if (target == null) {
                target = e;
            }
            
            // only towers do things that can be in progress (sell, upgrade)
            if (!(e is ClientTower t)) {
                continue;
            }

            if (
                ClientTowerUpgradeSystem.Singleton.IsTowerInProgress(
                    t,
                    out double duration
                )
                && duration < lowestRemainingDuration
            ) {
                lowestRemainingDuration = duration;
                target = t;
                targetIsInProgress = true;
            }
        }

        if (target == null || target.ActiveLane != ClientLaneTracker.Singleton.MyLane) {
            ClearAndHide();
            return;
        }

        if (targetIsInProgress) {
            OptionsContent.Hide();
            InProgressContent.LoadAndShowForEntity(target);
        } else {
            InProgressContent.Hide();
            OptionsContent.LoadAndShowForEntity(target);
        }
    }

    private void InProgressContentCanceled() {
        ShowForTargets(TargetSystem.Singleton.Targets);
    }
}
