---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Data/ChatMessage.cs"
ref: HEAD
---

﻿public class ChatMessage {
    public int ID { get; }
    public PlayerInfo Sender { get; }
    public string Content { get; }

    public ChatMessage(int id, PlayerInfo sender, string content) {
        ID = id;
        Sender = sender;
        Content = content;
    }
}