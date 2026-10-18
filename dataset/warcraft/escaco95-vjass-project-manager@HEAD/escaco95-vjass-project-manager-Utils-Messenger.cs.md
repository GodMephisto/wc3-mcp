---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/Messenger.cs"
ref: HEAD
---

﻿namespace vJassMainJBlueprint.Utils
{
    public static class Messenger
    {
        // Dictionary to store event handlers for different message types
        private static readonly Dictionary<Type, List<Action<object>>> _subscriptions = [];

        // Subscribe to a specific message type
        public static void Subscribe<TMessage>(Action<TMessage> action)
        {
            var messageType = typeof(TMessage);
            if (!_subscriptions.TryGetValue(messageType, out List<Action<object>>? value))
            {
                value = [];
                _subscriptions[messageType] = value;
            }

            value.Add(msg => action((TMessage)msg));
        }

        // Unsubscribe from a specific message type
        public static void Unsubscribe<TMessage>(Action<TMessage> action)
        {
            var messageType = typeof(TMessage);
            if (_subscriptions.TryGetValue(messageType, out List<Action<object>>? value))
            {
                value.RemoveAll(a => a.Equals(action));
            }
        }

        // Send a message to all subscribers of that message type
        public static void Send<TMessage>(TMessage message) where TMessage : notnull
        {
            var messageType = typeof(TMessage);
            if (_subscriptions.TryGetValue(messageType, out List<Action<object>>? value))
            {
                foreach (var action in value)
                {
                    action(message);
                }
            }
        }
    }
}
