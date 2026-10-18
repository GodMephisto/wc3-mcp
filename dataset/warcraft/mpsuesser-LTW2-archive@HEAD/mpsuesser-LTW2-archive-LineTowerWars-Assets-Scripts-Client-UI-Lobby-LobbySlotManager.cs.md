---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Lobby/LobbySlotManager.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySlotManager : MonoBehaviour {
    [SerializeField] private LobbySlot[] Slots;

    private void Awake() {
        for (int i = 0; i < Slots.Length; i++) {
            Slots[i].SetNumber(i);
        }
        
        EventBus.OnPlayersInfoUpdated += UpdateSlots;
    }

    private void OnDestroy() {
        EventBus.OnPlayersInfoUpdated -= UpdateSlots;
    }

    private void UpdateSlots() {
        for (int i = 0; i < Slots.Length; i++) {
            if (LobbySystem.Singleton.TryGetPlayerInSlot(i, out PlayerInfo player)) {
                Slots[i].LoadPlayer(player);
            }
            else {
                Slots[i].ClearPlayer();
            }
        }
    }
}
