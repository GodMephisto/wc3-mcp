---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Networking/MessageExtensions.cs"
ref: HEAD
---

﻿
// This file is provided under The MIT License as part of RiptideNetworking.
// Copyright (c) 2021 Tom Weiland
// For additional information please see the included LICENSE.md file or view it on GitHub: https://github.com/tom-weiland/RiptideNetworking/blob/main/LICENSE.md

using UnityEngine;
using RiptideNetworking;

public static class MessageExtensions
{
    #region Vector2
    /// <summary>Adds a <see cref="Vector2"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector2"/> to add.</param>
    /// <returns>The message that the <see cref="Vector2"/> was added to.</returns>
    public static Message Add(this Message message, Vector2 value)
    {
        message.Add(value.x);
        message.Add(value.y);
        return message;
    }
    
    /// <summary>Adds a <see cref="Vector2"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector2"/> to add.</param>
    /// <returns>The message that the <see cref="Vector2"/> was added to.</returns>
    public static Message AddVector2(this Message message, Vector2 value)
    {
        message.Add(value.x);
        message.Add(value.y);
        return message;
    }

    /// <summary>Retrieves a <see cref="Vector2"/> from the message.</summary>
    /// <returns>The <see cref="Vector2"/> that was retrieved.</returns>
    public static Vector2 GetVector2(this Message message)
    {
        return new Vector2(message.GetFloat(), message.GetFloat());
    }
    #endregion

    #region Vector3
    /// <summary>Adds a <see cref="Vector3"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector3"/> to add.</param>
    /// <returns>The message that the <see cref="Vector3"/> was added to.</returns>
    public static Message Add(this Message message, Vector3 value)
    {
        message.Add(value.x);
        message.Add(value.y);
        message.Add(value.z);
        return message;
    }
    
    /// <summary>Adds a <see cref="Vector3"/> to the message.</summary>
    /// <param name="value">The <see cref="Vector3"/> to add.</param>
    /// <returns>The message that the <see cref="Vector3"/> was added to.</returns>
    public static Message AddVector3(this Message message, Vector3 value)
    {
        message.Add(value.x);
        message.Add(value.y);
        message.Add(value.z);
        return message;
    }

    /// <summary>Retrieves a <see cref="Vector3"/> from the message.</summary>
    /// <returns>The <see cref="Vector3"/> that was retrieved.</returns>
    public static Vector3 GetVector3(this Message message)
    {
        return new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion

    #region Quaternion
    /// <summary>Adds a <see cref="Quaternion"/> to the message.</summary>
    /// <param name="value">The <see cref="Quaternion"/> to add.</param>
    /// <returns>The message that the <see cref="Quaternion"/> was added to.</returns>
    public static Message Add(this Message message, Quaternion value)
    {
        message.Add(value.x);
        message.Add(value.y);
        message.Add(value.z);
        message.Add(value.w);
        return message;
    }
    
    /// <summary>Adds a <see cref="Quaternion"/> to the message.</summary>
    /// <param name="value">The <see cref="Quaternion"/> to add.</param>
    /// <returns>The message that the <see cref="Quaternion"/> was added to.</returns>
    public static Message AddQuaternion(this Message message, Quaternion value)
    {
        message.Add(value.x);
        message.Add(value.y);
        message.Add(value.z);
        message.Add(value.w);
        return message;
    }

    /// <summary>Retrieves a <see cref="Quaternion"/> from the message.</summary>
    /// <returns>The <see cref="Quaternion"/> that was retrieved.</returns>
    public static Quaternion GetQuaternion(this Message message)
    {
        return new Quaternion(message.GetFloat(), message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion
    
    #region int
    /// <summary>Adds an <see cref="int"/> to the message.</summary>
    /// <param name="value">The <see cref="int"/> to add.</param>
    /// <returns>The message that the <see cref="int"/> was added to.</returns>
    public static Message AddInt(this Message message, int value)
    {
        message.Add(value);
        return message;
    }
    #endregion
    
    
    #region float
    /// <summary>Adds a <see cref="float"/> to the message.</summary>
    /// <param name="value">The <see cref="float"/> to add.</param>
    /// <returns>The message that the <see cref="float"/> was added to.</returns>
    public static Message AddFloat(this Message message, float value)
    {
        message.Add(value);
        return message;
    }
    #endregion
    
    
    #region double
    /// <summary>Adds a <see cref="double"/> to the message.</summary>
    /// <param name="value">The <see cref="double"/> to add.</param>
    /// <returns>The message that the <see cref="double"/> was added to.</returns>
    public static Message AddDouble(this Message message, double value)
    {
        message.Add(value);
        return message;
    }
    #endregion
    
    
    #region string
    /// <summary>Adds a <see cref="string"/> to the message.</summary>
    /// <param name="value">The <see cref="string"/> to add.</param>
    /// <returns>The message that the <see cref="string"/> was added to.</returns>
    public static Message AddString(this Message message, string value)
    {
        message.Add(value);
        return message;
    }
    #endregion
    
    
    #region ushort
    /// <summary>Adds a <see cref="ushort"/> to the message.</summary>
    /// <param name="value">The <see cref="ushort"/> to add.</param>
    /// <returns>The message that the <see cref="ushort"/> was added to.</returns>
    public static Message AddUShort(this Message message, ushort value)
    {
        message.Add(value);
        return message;
    }
    #endregion
    
    #region long
    /// <summary>Adds a <see cref="long"/> to the message.</summary>
    /// <param name="value">The <see cref="long"/> to add.</param>
    /// <returns>The message that the <see cref="long"/> was added to.</returns>
    public static Message AddLong(this Message message, long value)
    {
        message.Add(value);
        return message;
    }
    #endregion
    
    #region bool
    /// <summary>Adds a <see cref="bool"/> to the message.</summary>
    /// <param name="value">The <see cref="bool"/> to add.</param>
    /// <returns>The message that the <see cref="bool"/> was added to.</returns>
    public static Message AddBool(this Message message, bool value)
    {
        message.Add(value);
        return message;
    }
    #endregion
    
    
    
    
    // LTW-specific types
    #region PlayerInfo
    
    public static Message AddPlayerInfo(this Message message, PlayerInfo value) {
        message.AddInt(value.ClientID);
        message.AddString(value.Username);
        message.AddString(value.PlayfabID);
        message.AddInt(value.Slot);
        message.AddInt((int) value.State);

        return message;
    }
    
    public static PlayerInfo GetPlayerInfo(this Message message) {
        return new PlayerInfo(
            message.GetInt(),
            message.GetString(),
            message.GetString(),
            message.GetInt(),
            (ClientGameStateType) message.GetInt()
        );
    }
    
    #endregion
    
    #region ChatMessage

    public static Message AddChatMessage(this Message message, ChatMessage value) {
        message.AddInt(value.ID);
        message.AddPlayerInfo(value.Sender);
        message.AddString(value.Content);

        return message;
    }

    public static ChatMessage GetChatMessage(this Message message) {
        return new ChatMessage(
            message.GetInt(),
            message.GetPlayerInfo(),
            message.GetString()
        );
    }
    
    #endregion
    
    #region BuffTransitData

    public static Message AddBuffTransitData(this Message message, BuffTransitData buffData) {
        message.AddInt(buffData.ID);
        message.AddInt((int) buffData.Type);
        message.AddInt(buffData.Stacks);
        message.AddBool(buffData.IsDurationBased);
        message.AddDouble(buffData.FullDuration);
        message.AddDouble(buffData.RemainingDuration);

        return message;
    }

    public static BuffTransitData GetBuffTransitData(this Message message) {
        return new BuffTransitData(
            message.GetInt(),
            (BuffType)message.GetInt(),
            message.GetInt(),
            message.GetBool(),
            message.GetDouble(),
            message.GetDouble()
        );
    }
    
    #endregion
}
