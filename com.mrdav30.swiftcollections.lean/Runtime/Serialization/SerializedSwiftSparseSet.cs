//=======================================================================
// SerializedSwiftSparseSet.cs
//=======================================================================
// MIT License, Copyright (c) 2024-present David Oravsky (mrdav30)
// See LICENSE file in the project root for full license information.
//=======================================================================

using System;
using UnityEngine;

namespace SwiftCollections.Unity
{
    /// <summary>
    /// Unity-serializable adapter for persisted compact integer-ID membership consumed as a <see cref="SwiftSparseSet"/>.
    /// </summary>
    [Serializable]
    public sealed class SerializedSwiftSparseSet : ISerializationCallbackReceiver
    {
        [SerializeField]
        private int[] _items = Array.Empty<int>();

        [NonSerialized]
        private SwiftSparseSet _runtime;

        /// <summary>
        /// Gets the live runtime sparse set rebuilt from Unity serialized data.
        /// </summary>
        /// <remarks>Direct runtime mutations are copied into serialized backing data before Unity serialization.</remarks>
        public SwiftSparseSet Runtime
        {
            get
            {
                EnsureRuntime();
                return _runtime;
            }
        }

        /// <summary>
        /// Gets the number of stored IDs.
        /// </summary>
        public int Count => Runtime.Count;

        /// <summary>
        /// Replaces the serialized contents with a copy of the supplied IDs.
        /// </summary>
        /// <param name="items">The unique, non-negative IDs to persist.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">An ID is negative.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="items" /> contains duplicate IDs.</exception>
        public void SetItems(params int[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = CopyItems(items);
            RebuildRuntime();
        }

        /// <summary>
        /// Adds an ID to the runtime set and updates the serialized backing array.
        /// </summary>
        /// <param name="item">The non-negative ID to add.</param>
        /// <returns><c>true</c> when the ID was added; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item" /> is negative.</exception>
        public bool Add(int item)
        {
            ValidateId(item);

            bool added = Runtime.Add(item);
            if (added)
                SyncSerializedItemsFromRuntime();

            return added;
        }

        /// <summary>
        /// Removes an ID from the runtime set and updates the serialized backing array.
        /// </summary>
        /// <param name="item">The non-negative ID to remove.</param>
        /// <returns><c>true</c> when the ID was removed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item" /> is negative.</exception>
        public bool Remove(int item)
        {
            ValidateId(item);

            bool removed = Runtime.Remove(item);
            if (removed)
                SyncSerializedItemsFromRuntime();

            return removed;
        }

        /// <summary>
        /// Returns whether the set contains the specified ID.
        /// </summary>
        /// <param name="item">The non-negative ID to find.</param>
        /// <returns><c>true</c> when the ID exists; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item" /> is negative.</exception>
        public bool Contains(int item)
        {
            ValidateId(item);
            return Runtime.Contains(item);
        }

        /// <summary>
        /// Removes all IDs from the runtime set and serialized backing array.
        /// </summary>
        public void Clear()
        {
            Runtime.Clear();
            _items = Array.Empty<int>();
        }

        /// <summary>
        /// Returns a copy of the serialized IDs.
        /// </summary>
        /// <returns>A new array containing the current IDs.</returns>
        public int[] ToArray()
        {
            SyncSerializedItemsFromRuntime();
            return CopyItems(_items);
        }

        /// <summary>
        /// Returns a copy as a standalone <see cref="SwiftSparseSet"/>.
        /// </summary>
        /// <returns>A new sparse set containing the current IDs.</returns>
        public SwiftSparseSet ToSwiftSparseSet()
        {
            SyncSerializedItemsFromRuntime();
            return CreateSparseSet(_items);
        }

        /// <summary>
        /// Returns an enumerator for the runtime sparse set.
        /// </summary>
        /// <returns>A SwiftCollections sparse set enumerator.</returns>
        public SwiftSparseSet.SwiftSparseSetEnumerator GetEnumerator()
        {
            return Runtime.GetEnumerator();
        }

        /// <summary>
        /// Copies live runtime contents into Unity's serialized backing data.
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (_runtime != null)
                SyncSerializedItemsFromRuntime();
            else if (_items == null)
                _items = Array.Empty<int>();
        }

        /// <summary>
        /// Rebuilds runtime contents after Unity deserialization.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">A serialized ID is negative.</exception>
        /// <exception cref="InvalidOperationException">Serialized IDs contain duplicates.</exception>
        public void OnAfterDeserialize()
        {
            if (_items == null)
                _items = Array.Empty<int>();

            RebuildRuntime();
        }

        private void EnsureRuntime()
        {
            if (_runtime == null)
                RebuildRuntime();
        }

        private void RebuildRuntime()
        {
            _runtime = CreateSparseSet(_items ?? Array.Empty<int>());
        }

        private void SyncSerializedItemsFromRuntime()
        {
            _items = _runtime == null || _runtime.Count == 0
                ? Array.Empty<int>()
                : _runtime.State.Items;
        }

        private static SwiftSparseSet CreateSparseSet(int[] items)
        {
            var set = new SwiftSparseSet(GetSparseCapacity(items), items.Length);

            for (int i = 0; i < items.Length; i++)
            {
                int item = items[i];
                ValidateId(item);

                if (!set.Add(item))
                    throw new InvalidOperationException($"Duplicate ID '{item}' at serialized sparse set index {i}.");
            }

            return set;
        }

        private static int GetSparseCapacity(int[] items)
        {
            int max = -1;
            for (int i = 0; i < items.Length; i++)
            {
                ValidateId(items[i]);
                if (items[i] > max)
                    max = items[i];
            }

            return max < 0 ? 0 : max + 1;
        }

        private static void ValidateId(int item)
        {
            if (item < 0)
                throw new ArgumentOutOfRangeException(nameof(item), item, "Sparse set IDs cannot be negative.");
        }

        private static int[] CopyItems(int[] items)
        {
            if (items.Length == 0)
                return Array.Empty<int>();

            var copy = new int[items.Length];
            Array.Copy(items, copy, items.Length);
            return copy;
        }
    }
}
