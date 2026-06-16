//=======================================================================
// SerializedSwiftDictionary.cs
//=======================================================================
// MIT License, Copyright (c) 2024-present David Oravsky (mrdav30)
// See LICENSE file in the project root for full license information.
//=======================================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftCollections.Unity
{
    /// <summary>
    /// Unity-serializable key/value entry used by <see cref="SerializedSwiftDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">A Unity-serializable key type.</typeparam>
    /// <typeparam name="TValue">A Unity-serializable value type.</typeparam>
    [Serializable]
    public struct SerializedSwiftDictionaryEntry<TKey, TValue>
    {
        [SerializeField]
        private TKey _key;

        [SerializeField]
        private TValue _value;

        /// <summary>
        /// Initializes a serialized dictionary entry.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        public SerializedSwiftDictionaryEntry(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Gets or sets the entry key.
        /// </summary>
        public TKey Key
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        /// Gets or sets the entry value.
        /// </summary>
        public TValue Value
        {
            get => _value;
            set => _value = value;
        }
    }

    /// <summary>
    /// Unity-serializable adapter for persisted authoring data that is consumed as a <see cref="SwiftDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <remarks>
    /// Unity does not serialize dictionary fields directly, so this adapter stores an ordered array of
    /// Unity-serializable entries and rebuilds a runtime SwiftDictionary from those entries.
    /// </remarks>
    /// <typeparam name="TKey">A Unity-serializable dictionary key type.</typeparam>
    /// <typeparam name="TValue">A Unity-serializable dictionary value type.</typeparam>
    [Serializable]
    public sealed class SerializedSwiftDictionary<TKey, TValue> : ISerializationCallbackReceiver
        where TKey : notnull
    {
        [SerializeField]
        private SerializedSwiftDictionaryEntry<TKey, TValue>[] _items = Array.Empty<SerializedSwiftDictionaryEntry<TKey, TValue>>();

        [NonSerialized]
        private SwiftDictionary<TKey, TValue> _runtime;

        /// <summary>
        /// Gets the runtime SwiftCollections dictionary rebuilt from Unity serialized data.
        /// </summary>
        public SwiftDictionary<TKey, TValue> Runtime
        {
            get
            {
                EnsureRuntime();
                return _runtime;
            }
        }

        /// <summary>
        /// Gets the number of entries in the runtime dictionary.
        /// </summary>
        public int Count => Runtime.Count;

        /// <summary>
        /// Gets or sets a value by key and updates the serialized backing entries.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <returns>The value associated with the key.</returns>
        public TValue this[TKey key]
        {
            get => Runtime[key];
            set
            {
                Runtime[key] = value;
                SyncSerializedItemsFromRuntime();
            }
        }

        /// <summary>
        /// Replaces the serialized contents with a copy of the supplied entries.
        /// </summary>
        /// <param name="items">Entries to persist and expose through the runtime dictionary.</param>
        public void SetItems(params SerializedSwiftDictionaryEntry<TKey, TValue>[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = CopyItems(items);
            RebuildRuntime();
        }

        /// <summary>
        /// Adds a key/value pair to the runtime dictionary and updates the serialized backing entries.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> when the key was added; otherwise <c>false</c>.</returns>
        public bool Add(TKey key, TValue value)
        {
            bool added = Runtime.Add(key, value);
            if (added)
                SyncSerializedItemsFromRuntime();

            return added;
        }

        /// <summary>
        /// Attempts to get the value associated with the key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The value if found; otherwise the default value.</param>
        /// <returns><c>true</c> if the key exists; otherwise <c>false</c>.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return Runtime.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns whether the dictionary contains the key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns><c>true</c> if the key exists; otherwise <c>false</c>.</returns>
        public bool ContainsKey(TKey key)
        {
            return Runtime.ContainsKey(key);
        }

        /// <summary>
        /// Removes all entries from the runtime dictionary and serialized backing array.
        /// </summary>
        public void Clear()
        {
            Runtime.Clear();
            _items = Array.Empty<SerializedSwiftDictionaryEntry<TKey, TValue>>();
        }

        /// <summary>
        /// Returns a copy of the serialized entries.
        /// </summary>
        /// <returns>A new array containing the current entries.</returns>
        public SerializedSwiftDictionaryEntry<TKey, TValue>[] ToArray()
        {
            SyncSerializedItemsFromRuntime();
            return CopyItems(_items);
        }

        /// <summary>
        /// Returns a copy as a standalone <see cref="SwiftDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A new SwiftDictionary containing the current entries.</returns>
        public SwiftDictionary<TKey, TValue> ToSwiftDictionary()
        {
            SyncSerializedItemsFromRuntime();
            return CreateDictionary(_items);
        }

        /// <summary>
        /// Returns an enumerator for the runtime dictionary.
        /// </summary>
        /// <returns>A SwiftDictionary enumerator.</returns>
        public SwiftDictionary<TKey, TValue>.SwiftDictionaryEnumerator GetEnumerator()
        {
            return Runtime.GetEnumerator();
        }

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            if (_runtime != null)
                SyncSerializedItemsFromRuntime();
            else if (_items == null)
                _items = Array.Empty<SerializedSwiftDictionaryEntry<TKey, TValue>>();
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            if (_items == null)
                _items = Array.Empty<SerializedSwiftDictionaryEntry<TKey, TValue>>();

            RebuildRuntime();
        }

        private void EnsureRuntime()
        {
            if (_runtime == null)
                RebuildRuntime();
        }

        private void RebuildRuntime()
        {
            _runtime = CreateDictionary(_items ?? Array.Empty<SerializedSwiftDictionaryEntry<TKey, TValue>>());
        }

        private void SyncSerializedItemsFromRuntime()
        {
            if (_runtime == null || _runtime.Count == 0)
            {
                _items = Array.Empty<SerializedSwiftDictionaryEntry<TKey, TValue>>();
                return;
            }

            var entries = new SerializedSwiftDictionaryEntry<TKey, TValue>[_runtime.Count];
            int index = 0;

            foreach (KeyValuePair<TKey, TValue> pair in _runtime)
                entries[index++] = new SerializedSwiftDictionaryEntry<TKey, TValue>(pair.Key, pair.Value);

            _items = entries;
        }

        private static SwiftDictionary<TKey, TValue> CreateDictionary(
            SerializedSwiftDictionaryEntry<TKey, TValue>[] items)
        {
            var dictionary = new SwiftDictionary<TKey, TValue>(items.Length);

            for (int i = 0; i < items.Length; i++)
            {
                SerializedSwiftDictionaryEntry<TKey, TValue> entry = items[i];
                if (!dictionary.Add(entry.Key, entry.Value))
                    throw new InvalidOperationException($"Duplicate key '{entry.Key}' at serialized dictionary index {i}.");
            }

            return dictionary;
        }

        private static SerializedSwiftDictionaryEntry<TKey, TValue>[] CopyItems(
            SerializedSwiftDictionaryEntry<TKey, TValue>[] items)
        {
            if (items.Length == 0)
                return Array.Empty<SerializedSwiftDictionaryEntry<TKey, TValue>>();

            var copy = new SerializedSwiftDictionaryEntry<TKey, TValue>[items.Length];
            Array.Copy(items, copy, items.Length);
            return copy;
        }
    }
}
