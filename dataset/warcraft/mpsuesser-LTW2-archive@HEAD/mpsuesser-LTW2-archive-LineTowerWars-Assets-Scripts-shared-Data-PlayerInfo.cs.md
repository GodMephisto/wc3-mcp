---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Data/PlayerInfo.cs"
ref: HEAD
---

public class PlayerInfo {
    public int ClientID { get;}
    public string Username { get; }
    public string PlayfabID { get;}
    public int Slot { get; private set; }
    public ClientGameStateType State { get; private set; }

    public PlayerInfo(int clientID, string username, string playfabID, int slot, ClientGameStateType state = ClientGameStateType.Lobby) {
        ClientID = clientID;
        Username = username;
        PlayfabID = playfabID;
        Slot = slot;
        State = state;
    }

    public PlayerInfo(PlayerInfo copyPlayer) {
        ClientID = copyPlayer.ClientID;
        Username = copyPlayer.Username;
        PlayfabID = copyPlayer.PlayfabID;
        Slot = copyPlayer.Slot;
        State = copyPlayer.State;
    }

    public bool UpdateState(ClientGameStateType newState) {
        if (State == newState) {
            return false;
        }

        State = newState;
        return true;
    }

    public void UpdateSlot(int slot) {
        Slot = slot;
    }
}