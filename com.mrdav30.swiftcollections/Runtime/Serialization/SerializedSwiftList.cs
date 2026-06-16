//=======================================================================
// SerializedSwiftList.cs
//=======================================================================
// MIT License, Copyright (c) 2024-present David Oravsky (mrdav30)
// See LICENSE file in the project root for full license information.
//=======================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftCollections.Unity
{
    /// <summary>
    /// Unity-serializable adapter for persisted authoring data that is consumed as a <see cref="SwiftList{T}"/>.
    /// </summary>
    /// <remarks>
    /// Use this type for MonoBehaviour, ScriptableObject, prefab, or asset fields that must persist through
    /// Unity serialization. Runtime systems can read <see cref="Runtime"/> or call <see cref="ToSwiftList"/>.
    /// </remarks>
    /// <typeparam name="T">A Unity-serializable item type.</typeparam>
    [Serializable]
    public sealed class SerializedSwiftList<T> : IReadOnlyList<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private T[] _items = Array.Empty<T>();

        [NonSerialized]
        private SwiftList<T> _runtime;

        /// <summary>
        /// Gets the runtime SwiftCollections list rebuilt from Unity serialized data.
        /// </summary>
        public SwiftList<T> Runtime
        {
            get
            {
                EnsureRuntime();
                return _runtime;
            }
        }

        /// <inheritdoc />
        public int Count => Runtime.Count;

        /// <inheritdoc />
        public T this[int index] => Runtime[index];

        /// <summary>
        /// Replaces the serialized contents with a copy of the supplied items.
        /// </summary>
        /// <param name="items">Items to persist and expose through the runtime list.</param>
        public void SetItems(params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = CopyItems(items);
            RebuildRuntime();
        }

        /// <summary>
        /// Adds an item to the runtime list and updates the serialized backing array.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            Runtime.Add(item);
            SyncSerializedItemsFromRuntime();
        }

        /// <summary>
        /// Removes all items from the runtime list and serialized backing array.
        /// </summary>
        public void Clear()
        {
            Runtime.Clear();
            _items = Array.Empty<T>();
        }

        /// <summary>
        /// Removes the item at the specified index and updates the serialized backing array.
        /// </summary>
        /// <param name="index">Index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            Runtime.RemoveAt(index);
            SyncSerializedItemsFromRuntime();
        }

        /// <summary>
        /// Returns a copy of the serialized items.
        /// </summary>
        /// <returns>A new array containing the current items.</returns>
        public T[] ToArray()
        {
            return Runtime.ToArray();
        }

        /// <summary>
        /// Returns a copy as a standalone <see cref="SwiftList{T}"/>.
        /// </summary>
        /// <returns>A new SwiftList containing the current items.</returns>
        public SwiftList<T> ToSwiftList()
        {
            return new SwiftList<T>(ToArray());
        }

        /// <inheritdoc />
        public SwiftList<T>.SwiftListEnumerator GetEnumerator()
        {
            return Runtime.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            if (_runtime != null)
                SyncSerializedItemsFromRuntime();
            else if (_items == null)
                _items = Array.Empty<T>();
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            if (_items == null)
                _items = Array.Empty<T>();

            RebuildRuntime();
        }

        private void EnsureRuntime()
        {
            if (_runtime == null)
                RebuildRuntime();
        }

        private void RebuildRuntime()
        {
            _runtime = new SwiftList<T>(_items ?? Array.Empty<T>());
        }

        private void SyncSerializedItemsFromRuntime()
        {
            _items = _runtime == null
                ? Array.Empty<T>()
                : _runtime.ToArray();
        }

        private static T[] CopyItems(T[] items)
        {
            if (items.Length == 0)
                return Array.Empty<T>();

            var copy = new T[items.Length];
            Array.Copy(items, copy, items.Length);
            return copy;
        }
    }
}
