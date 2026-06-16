//=======================================================================
// SerializedSwiftArray2D.cs
//=======================================================================
// MIT License, Copyright (c) 2024-present David Oravsky (mrdav30)
// See LICENSE file in the project root for full license information.
//=======================================================================

using System;
using SwiftCollections.Dimensions;
using UnityEngine;

namespace SwiftCollections.Unity
{
    /// <summary>
    /// Unity-serializable adapter for persisted authoring data that is consumed as a <see cref="SwiftArray2D{T}"/>.
    /// </summary>
    /// <typeparam name="T">A Unity-serializable item type.</typeparam>
    [Serializable]
    public sealed class SerializedSwiftArray2D<T> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        [SerializeField]
        private T[] _items = Array.Empty<T>();

        [NonSerialized]
        private SwiftArray2D<T> _runtime;

        /// <summary>
        /// Gets the runtime SwiftCollections 2D array rebuilt from Unity serialized data.
        /// </summary>
        public SwiftArray2D<T> Runtime
        {
            get
            {
                EnsureRuntime();
                return _runtime;
            }
        }

        /// <summary>
        /// Gets the serialized width.
        /// </summary>
        public int Width => Runtime.Width;

        /// <summary>
        /// Gets the serialized height.
        /// </summary>
        public int Height => Runtime.Height;

        /// <summary>
        /// Gets the total serialized item count.
        /// </summary>
        public int Length => Runtime.Length;

        /// <summary>
        /// Gets or sets the item at the specified coordinates.
        /// </summary>
        public T this[int x, int y]
        {
            get => Runtime[x, y];
            set
            {
                Runtime[x, y] = value;
                SyncSerializedItemsFromRuntime();
            }
        }

        /// <summary>
        /// Replaces the serialized contents with the supplied dimensions and flat item data.
        /// </summary>
        /// <param name="width">The array width.</param>
        /// <param name="height">The array height.</param>
        /// <param name="items">Flat item data in SwiftArray2D index order.</param>
        public void SetItems(int width, int height, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            ValidateShape(width, height, items.Length, nameof(items));

            _width = width;
            _height = height;
            _items = CopyItems(items);
            RebuildRuntime();
        }

        /// <summary>
        /// Resizes the runtime array and updates the serialized backing data.
        /// </summary>
        public void Resize(int width, int height)
        {
            ValidateDimensions(width, height);

            Runtime.Resize(width, height);
            SyncSerializedItemsFromRuntime();
        }

        /// <summary>
        /// Clears all serialized values without changing dimensions.
        /// </summary>
        public void Clear()
        {
            Runtime.Clear();
            SyncSerializedItemsFromRuntime();
        }

        /// <summary>
        /// Returns a copy of the flattened serialized data.
        /// </summary>
        public T[] ToFlatArray()
        {
            return Runtime.State.Data;
        }

        /// <summary>
        /// Returns a copy as a standalone <see cref="SwiftArray2D{T}"/>.
        /// </summary>
        public SwiftArray2D<T> ToSwiftArray2D()
        {
            Array2DState<T> state = Runtime.State;
            return new SwiftArray2D<T>(new Array2DState<T>(state.Width, state.Height, state.Data));
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
            ValidateShape(_width, _height, _items.Length, nameof(_items));
            _runtime = new SwiftArray2D<T>(new Array2DState<T>(_width, _height, CopyItems(_items)));
        }

        private void SyncSerializedItemsFromRuntime()
        {
            if (_runtime == null)
            {
                _width = 0;
                _height = 0;
                _items = Array.Empty<T>();
                return;
            }

            Array2DState<T> state = _runtime.State;
            _width = state.Width;
            _height = state.Height;
            _items = state.Data ?? Array.Empty<T>();
        }

        private static void ValidateShape(int width, int height, int itemCount, string paramName)
        {
            ValidateDimensions(width, height);

            int expectedLength = GetExpectedLength(width, height);
            if (itemCount != expectedLength)
            {
                throw new ArgumentException(
                    $"Serialized 2D array data length must equal width * height. Expected {expectedLength}, got {itemCount}.",
                    paramName);
            }
        }

        private static void ValidateDimensions(int width, int height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Width cannot be negative.");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Height cannot be negative.");
        }

        private static int GetExpectedLength(int width, int height)
        {
            long expectedLength = (long)width * height;
            if (expectedLength > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(width), "Serialized 2D array dimensions are too large.");

            return (int)expectedLength;
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
