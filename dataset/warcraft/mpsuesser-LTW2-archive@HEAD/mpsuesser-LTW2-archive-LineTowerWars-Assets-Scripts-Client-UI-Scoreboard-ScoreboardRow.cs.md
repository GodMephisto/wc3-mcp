---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Scoreboard/ScoreboardRow.cs"
ref: HEAD
---

using System;
using UnityEngine;
using TMPro;

public class ScoreboardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text incomeText;
    [SerializeField] private TMP_Text livesText;

    private Lane TrackingLane { get; set; }
    private Color TrackingLaneColor { get; set; }

    public static ScoreboardRow Create(Lane lane, Transform parent) {
        ScoreboardRow row = Instantiate(ClientPrefabs.Singleton.pfScoreboardRow, parent);
        row.SetLane(lane);
        return row;
    }

    public void SetLane(Lane lane) {
        if (TrackingLane != null) {
            TrackingLane.OnIncomeUpdated -= UpdateIncomeText;
            TrackingLane.OnLivesUpdated -= UpdateLivesText;
        }

        TrackingLane = lane;
        TrackingLaneColor = GetLaneColor();
        
        UpdatePlayerNameText();
        UpdateIncomeText(lane);
        UpdateLivesText(lane);

        TrackingLane.OnIncomeUpdated += UpdateIncomeText;
        TrackingLane.OnLivesUpdated += UpdateLivesText;
    }

    private Color GetLaneColor() {
        try {
            return ClientResources.Singleton.GetColorForLane(TrackingLane.ID);
        }
        catch (IndexOutOfRangeException) {
            return Color.white;
        }
    }

    private void UpdatePlayerNameText() {
        string playerInLane = "";
        if (LobbySystem.Singleton.TryGetPlayerInSlot(TrackingLane.ID, out PlayerInfo player)) {
            playerInLane = player.Username;
        }
        
        playerNameText.SetText(playerInLane);
        playerNameText.color = TrackingLaneColor;
    }

    private void UpdateIncomeText(Lane lane) {
        incomeText.SetText(TrackingLane.Income.ToString());
        incomeText.color = TrackingLaneColor;
    }

    private void UpdateLivesText(Lane lane) {
        livesText.SetText(TrackingLane.Lives.ToString());
        livesText.color = TrackingLaneColor;
    }
}
