//=======================================================================
// SerializedSwiftSparseMap.cs
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
    /// Unity-serializable integer key/value entry used by <see cref="SerializedSwiftSparseMap{T}"/>.
    /// </summary>
    /// <typeparam name="T">A Unity-serializable value type.</typeparam>
    [Serializable]
    public struct SerializedSwiftSparseMapEntry<T>
    {
        [SerializeField]
        private int _key;

        [SerializeField]
        private T _value;

        /// <summary>
        /// Initializes a serialized sparse map entry.
        /// </summary>
        public SerializedSwiftSparseMapEntry(int key, T value)
        {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Gets or sets the integer key.
        /// </summary>
        public int Key
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        /// Gets or sets the entry value.
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }
    }

    /// <summary>
    /// Unity-serializable adapter for persisted compact integer-ID values consumed as a <see cref="SwiftSparseMap{T}"/>.
    /// </summary>
    /// <typeparam name="T">A Unity-serializable value type.</typeparam>
    [Serializable]
    public sealed class SerializedSwiftSparseMap<T> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private SerializedSwiftSparseMapEntry<T>[] _items = Array.Empty<SerializedSwiftSparseMapEntry<T>>();

        [NonSerialized]
        private SwiftSparseMap<T> _runtime;

        /// <summary>
        /// Gets the runtime SwiftCollections sparse map rebuilt from Unity serialized data.
        /// </summary>
        public SwiftSparseMap<T> Runtime
        {
            get
            {
                EnsureRuntime();
                return _runtime;
            }
        }

        /// <summary>
        /// Gets the number of mapped IDs.
        /// </summary>
        public int Count => Runtime.Count;

        /// <summary>
        /// Gets or sets a value by integer key and updates the serialized backing entries.
        /// </summary>
        public T this[int key]
        {
            get
            {
                ValidateKey(key);
                return Runtime[key];
            }
            set
            {
                ValidateKey(key);
                Runtime[key] = value;
                SyncSerializedItemsFromRuntime();
            }
        }

        /// <summary>
        /// Replaces the serialized contents with a copy of the supplied entries.
        /// </summary>
        public void SetItems(params SerializedSwiftSparseMapEntry<T>[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = CopyItems(items);
            RebuildRuntime();
        }

        /// <summary>
        /// Adds a key/value pair to the runtime sparse map and updates the serialized backing entries.
        /// </summary>
        public bool Add(int key, T value)
        {
            ValidateKey(key);

            bool added = Runtime.TryAdd(key, value);
            if (added)
                SyncSerializedItemsFromRuntime();

            return added;
        }

        /// <summary>
        /// Attempts to get the value associated with the key.
        /// </summary>
        public bool TryGetValue(int key, out T value)
        {
            ValidateKey(key);
            return Runtime.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns whether the sparse map contains the key.
        /// </summary>
        public bool ContainsKey(int key)
        {
            ValidateKey(key);
            return Runtime.ContainsKey(key);
        }

        /// <summary>
        /// Removes a key from the runtime sparse map and updates the serialized backing entries.
        /// </summary>
        public bool Remove(int key)
        {
            ValidateKey(key);

            bool removed = Runtime.Remove(key);
            if (removed)
                SyncSerializedItemsFromRuntime();

            return removed;
        }

        /// <summary>
        /// Removes all entries from the runtime sparse map and serialized backing array.
        /// </summary>
        public void Clear()
        {
            Runtime.Clear();
            _items = Array.Empty<SerializedSwiftSparseMapEntry<T>>();
        }

        /// <summary>
        /// Returns a copy of the serialized entries.
        /// </summary>
        public SerializedSwiftSparseMapEntry<T>[] ToArray()
        {
            SyncSerializedItemsFromRuntime();
            return CopyItems(_items);
        }

        /// <summary>
        /// Returns a copy as a standalone <see cref="SwiftSparseMap{T}"/>.
        /// </summary>
        public SwiftSparseMap<T> ToSwiftSparseMap()
        {
            SyncSerializedItemsFromRuntime();
            return CreateSparseMap(_items);
        }

        /// <summary>
        /// Returns an enumerator for the runtime sparse map.
        /// </summary>
        public SwiftSparseMap<T>.SwiftSparseMapEnumerator GetEnumerator()
        {
            return Runtime.GetEnumerator();
        }

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            if (_runtime != null)
                SyncSerializedItemsFromRuntime();
            else if (_items == null)
                _items = Array.Empty<SerializedSwiftSparseMapEntry<T>>();
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            if (_items == null)
                _items = Array.Empty<SerializedSwiftSparseMapEntry<T>>();

            RebuildRuntime();
        }

        private void EnsureRuntime()
        {
            if (_runtime == null)
                RebuildRuntime();
        }

        private void RebuildRuntime()
        {
            _runtime = CreateSparseMap(_items ?? Array.Empty<SerializedSwiftSparseMapEntry<T>>());
        }

        private void SyncSerializedItemsFromRuntime()
        {
            if (_runtime == null || _runtime.Count == 0)
            {
                _items = Array.Empty<SerializedSwiftSparseMapEntry<T>>();
                return;
            }

            var entries = new SerializedSwiftSparseMapEntry<T>[_runtime.Count];
            int index = 0;

            foreach (KeyValuePair<int, T> pair in _runtime)
                entries[index++] = new SerializedSwiftSparseMapEntry<T>(pair.Key, pair.Value);

            _items = entries;
        }

        private static SwiftSparseMap<T> CreateSparseMap(SerializedSwiftSparseMapEntry<T>[] items)
        {
            var map = new SwiftSparseMap<T>(GetSparseCapacity(items), items.Length);

            for (int i = 0; i < items.Length; i++)
            {
                SerializedSwiftSparseMapEntry<T> entry = items[i];
                ValidateKey(entry.Key);

                if (!map.TryAdd(entry.Key, entry.Value))
                    throw new InvalidOperationException($"Duplicate key '{entry.Key}' at serialized sparse map index {i}.");
            }

            return map;
        }

        private static int GetSparseCapacity(SerializedSwiftSparseMapEntry<T>[] items)
        {
            int max = -1;
            for (int i = 0; i < items.Length; i++)
            {
                ValidateKey(items[i].Key);
                if (items[i].Key > max)
                    max = items[i].Key;
            }

            return max < 0 ? 0 : max + 1;
        }

        private static void ValidateKey(int key)
        {
            if (key < 0)
                throw new ArgumentOutOfRangeException(nameof(key), key, "Sparse map keys cannot be negative.");
        }

        private static SerializedSwiftSparseMapEntry<T>[] CopyItems(SerializedSwiftSparseMapEntry<T>[] items)
        {
            if (items.Length == 0)
                return Array.Empty<SerializedSwiftSparseMapEntry<T>>();

            var copy = new SerializedSwiftSparseMapEntry<T>[items.Length];
            Array.Copy(items, copy, items.Length);
            return copy;
        }
    }
}
