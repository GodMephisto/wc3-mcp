---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/GoldTracker/GoldTracker.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

public class GoldTracker : MonoBehaviour
{
    [SerializeField] private GoldText goldText;
    [SerializeField] private IncomeTimer incomeTimer;
    [SerializeField] private ActiveUnitsText activeUnits;

    private void Start() {
        if (ClientLaneTracker.Singleton.MyLane != null) {
            UpdateGoldText(ClientLaneTracker.Singleton.MyLane);
        }

        ClientLaneTracker.Singleton.SubscribeToOnGoldUpdated(UpdateGoldText);
        ClientLaneTracker.Singleton.SubscribeToOnActiveUnitsUpdated(UpdateActiveUnitsText);
        EventBus.OnIncomeDelivery += UpdateIncomeTimer;
    }

    private void UpdateGoldText(Lane lane) {
        goldText.SetValue(lane.Gold);
    }

    private void UpdateActiveUnitsText(Lane lane) {
        activeUnits.SetValue(lane.ActiveUnits);
    }

    private void UpdateIncomeTimer(float timeToNextIncome) {
        incomeTimer.SetValue(timeToNextIncome);
    }
}
