---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Networking/ClientSend.cs"
ref: HEAD
---

﻿using System.Linq;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;
using static ClientNetworkManager;

// All functions here are intended to eventually be split up into ClientSend -> ServerReceive
public class ClientSend {
    public static void PresentTableStakes(
        string username,
        string playfabID,
        string playfabSessionToken,
        ClientGameStateType clientState
    ) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.PresentTableStakes
        );

        message.AddString(username);
        message.AddString(playfabID);
        message.AddString(playfabSessionToken);
        message.AddInt((int) clientState);

        Send(message);
    }

    public static void SendChatMessage(string chatMessage) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.SendChatMessage
        );

        message.AddString(chatMessage);

        Send(message);
    }

    public static void RequestNewLobbySlot(int lobbySlot) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestNewLobbySlot
        );

        message.AddInt(lobbySlot);

        Send(message);
    }

    public static void UpdateClientState(ClientGameStateType clientState) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.UpdateClientState
        );

        message.AddInt((int) clientState);

        Send(message);
    }

    public static void RequestSendCreep(EnemyType type, Lane lane) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestSendCreep
        );

        message.AddInt((int) type);
        message.AddInt(lane.ID);

        Send(message);
    }

    public static void RequestBuildTower(
        TowerType type,
        Lane lane,
        MazeGridCell[] cells,
        bool isQueuedAction
    ) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestBuildTower
        );

        message.AddInt((int) type);
        message.AddInt(lane.ID);
        message.AddInt(cells.Length);
        foreach (MazeGridCell cell in cells) {
            message.AddInt(cell.ID);
        }

        message.AddBool(isQueuedAction);

        Send(message);
    }

    public static void RequestPurchaseElementTech(Lane lane, ElementalTechType techType) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestPurchaseElementTech
        );

        message.AddInt((int) techType);
        message.AddInt(lane.ID);

        Send(message);
    }

    public static void RequestTowerUpgrade(HashSet<ClientTower> towers, TowerUpgrade upgrade) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestTowerUpgrade
        );

        message.AddInt((int) upgrade.SourceTowerType);
        message.AddInt((int) upgrade.TargetTowerType);
        message.AddInt(towers.Count);
        foreach (ClientTower tower in towers) {
            message.AddInt(tower.ID);
        }

        Send(message);
    }

    public static void RequestTowerUpgradeCancellation(HashSet<ClientTower> towers) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestTowerUpgradeCancellation
        );

        message.AddInt(towers.Count);
        foreach (ClientTower tower in towers) {
            message.AddInt(tower.ID);
        }

        Send(message);
    }

    public static void RequestTowerSale(HashSet<ClientTower> towers) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestTowerSale
        );

        message.AddInt(towers.Count);
        foreach (ClientTower tower in towers) {
            message.AddInt(tower.ID);
        }

        Send(message);
    }

    public static void RequestTowerSaleCancellation(HashSet<ClientTower> towers) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestTowerSaleCancellation
        );

        message.AddInt(towers.Count);
        foreach (ClientTower tower in towers) {
            message.AddInt(tower.ID);
        }

        Send(message);
    }

    public static void RequestEntitiesMove(
        List<ClientEntity> entities,
        Vector3 destination,
        bool isQueuedAction
    ) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestEntityMove
        );

        message.AddInt(entities.Count);
        foreach (ClientEntity entity in entities) {
            message.AddInt(entity.ID);
        }

        message.AddVector3(destination);
        message.AddBool(isQueuedAction);

        Send(message);
    }

    public static void RequestEntitiesAttackTarget(
        List<ClientEntity> attackingEntities,
        ClientEntity target,
        bool isQueuedAction
    ) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestEntityAttackTarget
        );

        message.AddInt(attackingEntities.Count);
        foreach (ClientEntity entity in attackingEntities) {
            message.AddInt(entity.ID);
        }

        message.AddInt(target.ID);
        message.AddBool(isQueuedAction);

        Send(message);
    }

    public static void RequestEntitiesAttackLocation(
        List<ClientEntity> attackingEntities,
        Vector3 location,
        bool isQueuedAction
    ) {
        Message message = RiptideHelper.CreateReliableMessage(
            RiptideMessageIDs.ClientToServer.RequestEntityAttackLocation
        );

        message.AddInt(attackingEntities.Count);
        foreach (ClientEntity entity in attackingEntities) {
            message.AddInt(entity.ID);
        }

        message.AddVector3(location);
        message.AddBool(isQueuedAction);

        Send(message);
    }

    private static HashSet<int> ConvertTowerSetToIDs(HashSet<ClientTower> towers) {
        HashSet<int> towerEntityIDs = new HashSet<int>();
        foreach (ClientTower tower in towers) {
            towerEntityIDs.Add(tower.ID);
        }

        return towerEntityIDs;
    }

    private static void Send(Message message) {
        bool connected = ClientNetworkManager.Singleton?.Client?.IsConnected ?? false;
        if (connected) {
            SingletonBehaviour<ClientNetworkManager>.Singleton.Client.Send(message);
        }
    }
}