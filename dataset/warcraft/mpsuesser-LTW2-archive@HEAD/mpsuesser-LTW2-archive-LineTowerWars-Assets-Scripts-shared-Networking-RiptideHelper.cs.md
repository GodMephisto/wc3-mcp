---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Networking/RiptideHelper.cs"
ref: HEAD
---

﻿using RiptideNetworking;

public static class RiptideHelper {
    public static Message CreateReliableMessage(RiptideMessageIDs.ServerToClient messageID) {
        return Message.Create(MessageSendMode.reliable, (ushort) messageID);
    }

    public static Message CreateReliableMessage(RiptideMessageIDs.ClientToServer messageID) {
        return Message.Create(MessageSendMode.reliable, (ushort) messageID);
    }
    
    // ---

    public static Message CreateUnreliableMessage(RiptideMessageIDs.ServerToClient messageID) {
        return Message.Create(MessageSendMode.unreliable, (ushort) messageID);
    }

    public static Message CreateUnreliableMessage(RiptideMessageIDs.ClientToServer messageID) {
        return Message.Create(MessageSendMode.unreliable, (ushort) messageID);
    }
}