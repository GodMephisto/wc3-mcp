---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/EventFeed/EFEventName.cs"
ref: HEAD
---

﻿using System;
using UnityEngine;
using TMPro;

public class EFEventName : MonoBehaviour {
    private TMP_Text nameText;

    private void Awake() {
        nameText = GetComponentInChildren<TMP_Text>();
    }

    public void LoadNameForLane(Lane lane) {
        Color laneColor = Color.white;
        try {
            laneColor = ClientResources.Singleton.GetColorForLane(lane.ID);
        }
        catch (IndexOutOfRangeException e) {
            LTWLogger.Log($"Could not get lane color for {lane.ID}... {e.Message}");
        }
        
        string playerInLane = "";
        if (LobbySystem.Singleton.TryGetPlayerInSlot(lane.ID, out PlayerInfo player)) {
            playerInLane = player.Username;
        }
        
        nameText.SetText(playerInLane);
        nameText.color = laneColor;
    }
}