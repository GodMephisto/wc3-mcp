---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Types/ClientGameStateType.cs"
ref: HEAD
---

public enum ClientGameStateType {
    None = 0,
    MainMenu,
    Lobby,
    LobbyReady,
    Staging,
    StagingReady,
    InGame,
    PostGame,
    Disconnected,
}