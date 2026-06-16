//=======================================================================
// SerializedSwiftBiDictionary.cs
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
    /// Unity-serializable two-way key/value entry used by <see cref="SerializedSwiftBiDictionary{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft">A Unity-serializable left key type.</typeparam>
    /// <typeparam name="TRight">A Unity-serializable right key type.</typeparam>
    [Serializable]
    public struct SerializedSwiftBiDictionaryEntry<TLeft, TRight>
    {
        [SerializeField]
        private TLeft _left;

        [SerializeField]
        private TRight _right;

        /// <summary>
        /// Initializes a serialized bi-dictionary entry.
        /// </summary>
        public SerializedSwiftBiDictionaryEntry(TLeft left, TRight right)
        {
            _left = left;
            _right = right;
        }

        /// <summary>
        /// Gets or sets the left key.
        /// </summary>
        public TLeft Left
        {
            get => _left;
            set => _left = value;
        }

        /// <summary>
        /// Gets or sets the right key.
        /// </summary>
        public TRight Right
        {
            get => _right;
            set => _right = value;
        }
    }

    /// <summary>
    /// Unity-serializable adapter for persisted two-way lookup data consumed as a <see cref="SwiftBiDictionary{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft">A Unity-serializable left key type.</typeparam>
    /// <typeparam name="TRight">A Unity-serializable right key type.</typeparam>
    [Serializable]
    public sealed class SerializedSwiftBiDictionary<TLeft, TRight> : ISerializationCallbackReceiver
        where TLeft : notnull
        where TRight : notnull
    {
        [SerializeField]
        private SerializedSwiftBiDictionaryEntry<TLeft, TRight>[] _items =
            Array.Empty<SerializedSwiftBiDictionaryEntry<TLeft, TRight>>();

        [NonSerialized]
        private SwiftBiDictionary<TLeft, TRight> _runtime;

        /// <summary>
        /// Gets the runtime SwiftCollections bi-dictionary rebuilt from Unity serialized data.
        /// </summary>
        public SwiftBiDictionary<TLeft, TRight> Runtime
        {
            get
            {
                EnsureRuntime();
                return _runtime;
            }
        }

        /// <summary>
        /// Gets the number of two-way mappings.
        /// </summary>
        public int Count => Runtime.Count;

        /// <summary>
        /// Gets or sets a right value by left key and updates the serialized backing entries.
        /// </summary>
        public TRight this[TLeft left]
        {
            get => Runtime[left];
            set
            {
                Runtime[left] = value;
                SyncSerializedItemsFromRuntime();
            }
        }

        /// <summary>
        /// Replaces the serialized contents with a copy of the supplied entries.
        /// </summary>
        public void SetItems(params SerializedSwiftBiDictionaryEntry<TLeft, TRight>[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = CopyItems(items);
            RebuildRuntime();
        }

        /// <summary>
        /// Adds a two-way mapping and updates the serialized backing entries.
        /// </summary>
        public bool Add(TLeft left, TRight right)
        {
            bool added = Runtime.Add(left, right);
            if (added)
                SyncSerializedItemsFromRuntime();

            return added;
        }

        /// <summary>
        /// Attempts to get the right value associated with the left key.
        /// </summary>
        public bool TryGetValue(TLeft left, out TRight right)
        {
            return Runtime.TryGetValue(left, out right);
        }

        /// <summary>
        /// Attempts to get the left key associated with the right value.
        /// </summary>
        public bool TryGetKey(TRight right, out TLeft left)
        {
            return Runtime.TryGetKey(right, out left);
        }

        /// <summary>
        /// Returns whether the bi-dictionary contains the left key.
        /// </summary>
        public bool ContainsKey(TLeft left)
        {
            return Runtime.ContainsKey(left);
        }

        /// <summary>
        /// Returns whether the bi-dictionary contains the right key.
        /// </summary>
        public bool ContainsValue(TRight right)
        {
            return Runtime.ContainsValue(right);
        }

        /// <summary>
        /// Removes all mappings from the runtime bi-dictionary and serialized backing array.
        /// </summary>
        public void Clear()
        {
            Runtime.Clear();
            _items = Array.Empty<SerializedSwiftBiDictionaryEntry<TLeft, TRight>>();
        }

        /// <summary>
        /// Returns a copy of the serialized entries.
        /// </summary>
        public SerializedSwiftBiDictionaryEntry<TLeft, TRight>[] ToArray()
        {
            SyncSerializedItemsFromRuntime();
            return CopyItems(_items);
        }

        /// <summary>
        /// Returns a copy as a standalone <see cref="SwiftBiDictionary{TLeft, TRight}"/>.
        /// </summary>
        public SwiftBiDictionary<TLeft, TRight> ToSwiftBiDictionary()
        {
            SyncSerializedItemsFromRuntime();
            return CreateBiDictionary(_items);
        }

        /// <summary>
        /// Returns an enumerator for the runtime bi-dictionary.
        /// </summary>
        public SwiftDictionary<TLeft, TRight>.SwiftDictionaryEnumerator GetEnumerator()
        {
            return Runtime.GetEnumerator();
        }

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            if (_runtime != null)
                SyncSerializedItemsFromRuntime();
            else if (_items == null)
                _items = Array.Empty<SerializedSwiftBiDictionaryEntry<TLeft, TRight>>();
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            if (_items == null)
                _items = Array.Empty<SerializedSwiftBiDictionaryEntry<TLeft, TRight>>();

            RebuildRuntime();
        }

        private void EnsureRuntime()
        {
            if (_runtime == null)
                RebuildRuntime();
        }

        private void RebuildRuntime()
        {
            _runtime = CreateBiDictionary(_items ?? Array.Empty<SerializedSwiftBiDictionaryEntry<TLeft, TRight>>());
        }

        private void SyncSerializedItemsFromRuntime()
        {
            if (_runtime == null || _runtime.Count == 0)
            {
                _items = Array.Empty<SerializedSwiftBiDictionaryEntry<TLeft, TRight>>();
                return;
            }

            var entries = new SerializedSwiftBiDictionaryEntry<TLeft, TRight>[_runtime.Count];
            int index = 0;

            foreach (KeyValuePair<TLeft, TRight> pair in _runtime)
                entries[index++] = new SerializedSwiftBiDictionaryEntry<TLeft, TRight>(pair.Key, pair.Value);

            _items = entries;
        }

        private static SwiftBiDictionary<TLeft, TRight> CreateBiDictionary(
            SerializedSwiftBiDictionaryEntry<TLeft, TRight>[] items)
        {
            var dictionary = new SwiftBiDictionary<TLeft, TRight>();

            for (int i = 0; i < items.Length; i++)
            {
                SerializedSwiftBiDictionaryEntry<TLeft, TRight> entry = items[i];
                if (dictionary.ContainsKey(entry.Left))
                    throw new InvalidOperationException($"Duplicate left key '{entry.Left}' at serialized bi-dictionary index {i}.");
                if (dictionary.ContainsValue(entry.Right))
                    throw new InvalidOperationException($"Duplicate right key '{entry.Right}' at serialized bi-dictionary index {i}.");

                dictionary.Add(entry.Left, entry.Right);
            }

            return dictionary;
        }

        private static SerializedSwiftBiDictionaryEntry<TLeft, TRight>[] CopyItems(
            SerializedSwiftBiDictionaryEntry<TLeft, TRight>[] items)
        {
            if (items.Length == 0)
                return Array.Empty<SerializedSwiftBiDictionaryEntry<TLeft, TRight>>();

            var copy = new SerializedSwiftBiDictionaryEntry<TLeft, TRight>[items.Length];
            Array.Copy(items, copy, items.Length);
            return copy;
        }
    }
}
