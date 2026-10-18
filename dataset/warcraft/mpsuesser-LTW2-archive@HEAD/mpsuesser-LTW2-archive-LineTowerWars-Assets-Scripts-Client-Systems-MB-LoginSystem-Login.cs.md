---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/LoginSystem/Login.cs"
ref: HEAD
---

﻿public class Login {
    public string PlayfabID { get; }
    public string Username { get; }
    public string AuthToken { get; }

    public Login(string playfabID, string username, string authToken) {
        PlayfabID = playfabID;
        Username = username;
        AuthToken = authToken;
    }
}
