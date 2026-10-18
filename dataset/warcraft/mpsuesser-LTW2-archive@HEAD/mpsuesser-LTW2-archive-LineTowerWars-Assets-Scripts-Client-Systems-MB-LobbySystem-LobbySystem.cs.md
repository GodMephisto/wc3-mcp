---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/LobbySystem/LobbySystem.cs"
ref: HEAD
---

using System.Collections.Generic;

public class LobbySystem : SingletonBehaviour<LobbySystem>
{
    public Dictionary<int, PlayerInfo> PlayersByClientID { get; private set; }
    private Dictionary<int, PlayerInfo> PlayersBySlot { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        PlayersByClientID = new Dictionary<int, PlayerInfo>();
        PlayersBySlot = new Dictionary<int, PlayerInfo>();
    }

    private void Start() {
        EventBus.OnFullPlayersSync += SyncPlayerList;
    }

    private void OnDestroy() {
        EventBus.OnFullPlayersSync -= SyncPlayerList;
    }

    private void SyncPlayerList(Dictionary<int, PlayerInfo> serverPlayerList) {
        PlayersByClientID.Clear();
        PlayersBySlot.Clear();

        foreach (PlayerInfo serverPlayer in serverPlayerList.Values) {
            // TODO: The copy constructor is probably unnecessary
            PlayersByClientID.Add(serverPlayer.ClientID, new PlayerInfo(serverPlayer));
            PlayersBySlot.Add(serverPlayer.Slot, new PlayerInfo(serverPlayer));
        }

        // TODO: Either only invoke this if a value has been changed or make it more granular
        EventBus.PlayersInfoUpdated();
    }

    public bool TryGetPlayerInSlot(int slotNum, out PlayerInfo player) {
        if (!PlayersBySlot.ContainsKey(slotNum)) {
            player = null;
            return false;
        }
        
        player = PlayersBySlot[slotNum];
        return true;
    }

    public PlayerInfo GetPlayerByClientID(int clientID) {
        if (!PlayersByClientID.ContainsKey(clientID)) {
            throw new NotFoundException($"Could not find player in lobby system with client ID {clientID}");
        }

        return PlayersByClientID[clientID];
    }
}
