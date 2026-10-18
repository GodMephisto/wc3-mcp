---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Lobby/LobbySlot.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LobbySlot : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private Sprite readiedImage;
    [SerializeField] private Sprite notReadiedImage;

    [Space(5)]
    [SerializeField] private TMP_Text UsernameText;
    [SerializeField] private Image ReadinessIndicator;
    [SerializeField] private Image HostIndicator;
    
    private int SlotNumber { get; set; }

    public void LoadPlayer(PlayerInfo playerInfo) {
        UsernameText.SetText(playerInfo.Username);
        SetReadiness(playerInfo.State == ClientGameStateType.LobbyReady);
        SetHost(playerInfo);
    }

    public void SetNumber(int num) {
        SlotNumber = num;
    }

    public void ClearPlayer() {
        UsernameText.SetText("");
        
        ReadinessIndicator.color = new Color(0, 0, 0, 0);
        ReadinessIndicator.sprite = null;
    }
    
    private void SetReadiness(bool isReady) {
        ReadinessIndicator.color = Color.white;
        ReadinessIndicator.sprite = isReady ? readiedImage : notReadiedImage;
    }

    private void SetHost(PlayerInfo playerInfo) {
        LTWLogger.Log("TODO: Set host indicator either not showing or showing based on if player is host");
    }
    
    public void OnPointerClick(PointerEventData pointerEventData) {
        LTWLogger.Log($"Slot {SlotNumber} with name {gameObject.name} left clicked!");
        ClientSend.RequestNewLobbySlot(SlotNumber);
    }
}
