using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace KendaAi.BlackboardSystem
{
    [Serializable]
    public class Blackboard
    {
        private Dictionary<string, BlackboardKey> keyRegistry = new();
        private Dictionary<BlackboardKey, object> entries = new();

        public Dictionary<BlackboardKey, object> Entries => entries;

        public List<Action> PassedActions { get; } = new();

        public void AddAction(Action action)
        {
            // Preconditions.CheckNotNull(action);
            PassedActions.Add(action);
        }
        public void ClearActions() => PassedActions.Clear();

        [Button]
        public void Debug()
        {
            foreach (var entry in entries)
            {
                var entryType = entry.Value.GetType();

                if (!entryType.IsGenericType ||
                    entryType.GetGenericTypeDefinition() != typeof(BlackboardEntry<>)) continue;
                var valueProperty = entryType.GetProperty("Value");
                if (valueProperty == null) continue;
                var value = valueProperty.GetValue(entry.Value);
                UnityEngine.Debug.Log($"Key: {entry.Key}, Value: {value}");
            }
        }

        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if (entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }       
        public bool TryGetValue<T>(string keyName, out T value)
        {
            var key = GetOrRegisterKey(keyName);
            if (entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BlackboardKey key, T value)
        {
            entries[key] = new BlackboardEntry<T>(key, value);
        }       
        
        public void SetValue<T>(string keyName, T value)
        {
            var key = GetOrRegisterKey(keyName);
            entries[key] = new BlackboardEntry<T>(key, value);
        }

        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            // Preconditions.CheckNotNull(keyName);

            if (keyRegistry.TryGetValue(keyName, out var key)) return key;
            key = new BlackboardKey(keyName);
            keyRegistry[keyName] = key;

            return key;
        }

        public bool ContainsKey(BlackboardKey key) => entries.ContainsKey(key);

        public void Remove(BlackboardKey key) => entries.Remove(key);
    }
}