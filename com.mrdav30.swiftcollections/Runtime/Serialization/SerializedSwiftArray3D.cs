//=======================================================================
// SerializedSwiftArray3D.cs
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
    /// Unity-serializable adapter for persisted authoring data that is consumed as a <see cref="SwiftArray3D{T}"/>.
    /// </summary>
    /// <typeparam name="T">A Unity-serializable item type.</typeparam>
    [Serializable]
    public sealed class SerializedSwiftArray3D<T> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        [SerializeField]
        private int _depth;

        [SerializeField]
        private T[] _items = Array.Empty<T>();

        [NonSerialized]
        private SwiftArray3D<T> _runtime;

        /// <summary>
        /// Gets the live runtime 3D array rebuilt from Unity serialized data.
        /// </summary>
        /// <remarks>Direct runtime mutations are copied into serialized backing data before Unity serialization.</remarks>
        public SwiftArray3D<T> Runtime
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
        /// Gets the serialized depth.
        /// </summary>
        public int Depth => Runtime.Depth;

        /// <summary>
        /// Gets the total serialized item count.
        /// </summary>
        public int Length => Runtime.Length;

        /// <summary>
        /// Gets or sets the item at the specified coordinates.
        /// </summary>
        /// <param name="x">The zero-based horizontal coordinate.</param>
        /// <param name="y">The zero-based vertical coordinate.</param>
        /// <param name="z">The zero-based depth coordinate.</param>
        /// <returns>The item at the specified coordinates.</returns>
        public T this[int x, int y, int z]
        {
            get => Runtime[x, y, z];
            set
            {
                Runtime[x, y, z] = value;
                SyncSerializedItemsFromRuntime();
            }
        }

        /// <summary>
        /// Replaces the serialized contents with the supplied dimensions and flat item data.
        /// </summary>
        /// <param name="width">The array width.</param>
        /// <param name="height">The array height.</param>
        /// <param name="depth">The array depth.</param>
        /// <param name="items">Flat item data in SwiftArray3D index order.</param>
        /// <exception cref="ArgumentNullException"><paramref name="items" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A dimension is negative or the resulting array is too large.</exception>
        /// <exception cref="ArgumentException">The item count does not equal the product of the dimensions.</exception>
        public void SetItems(int width, int height, int depth, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            ValidateShape(width, height, depth, items.Length, nameof(items));

            _width = width;
            _height = height;
            _depth = depth;
            _items = CopyItems(items);
            RebuildRuntime();
        }

        /// <summary>
        /// Resizes the runtime array and updates the serialized backing data.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <param name="depth">The new depth.</param>
        /// <exception cref="ArgumentOutOfRangeException">A dimension is negative or the resulting array is too large.</exception>
        public void Resize(int width, int height, int depth)
        {
            ValidateDimensions(width, height, depth);

            Runtime.Resize(width, height, depth);
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
        /// <returns>A new flat array in SwiftArray3D index order.</returns>
        public T[] ToFlatArray()
        {
            return Runtime.State.Data;
        }

        /// <summary>
        /// Returns a copy as a standalone <see cref="SwiftArray3D{T}"/>.
        /// </summary>
        /// <returns>A new 3D array containing the current values.</returns>
        public SwiftArray3D<T> ToSwiftArray3D()
        {
            Array3DState<T> state = Runtime.State;
            return new SwiftArray3D<T>(new Array3DState<T>(state.Width, state.Height, state.Depth, state.Data));
        }

        /// <summary>
        /// Copies live runtime contents into Unity's serialized backing data.
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (_runtime != null)
                SyncSerializedItemsFromRuntime();
            else if (_items == null)
                _items = Array.Empty<T>();
        }

        /// <summary>
        /// Rebuilds runtime contents after Unity deserialization.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">A serialized dimension is invalid.</exception>
        /// <exception cref="ArgumentException">The serialized item count does not match the dimensions.</exception>
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
            ValidateShape(_width, _height, _depth, _items.Length, nameof(_items));
            _runtime = new SwiftArray3D<T>(new Array3DState<T>(_width, _height, _depth, CopyItems(_items)));
        }

        private void SyncSerializedItemsFromRuntime()
        {
            if (_runtime == null)
            {
                _width = 0;
                _height = 0;
                _depth = 0;
                _items = Array.Empty<T>();
                return;
            }

            Array3DState<T> state = _runtime.State;
            _width = state.Width;
            _height = state.Height;
            _depth = state.Depth;
            _items = state.Data ?? Array.Empty<T>();
        }

        private static void ValidateShape(int width, int height, int depth, int itemCount, string paramName)
        {
            ValidateDimensions(width, height, depth);

            int expectedLength = GetExpectedLength(width, height, depth);
            if (itemCount != expectedLength)
            {
                throw new ArgumentException(
                    $"Serialized 3D array data length must equal width * height * depth. Expected {expectedLength}, got {itemCount}.",
                    paramName);
            }
        }

        private static void ValidateDimensions(int width, int height, int depth)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), width, "Width cannot be negative.");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), height, "Height cannot be negative.");
            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth), depth, "Depth cannot be negative.");
        }

        private static int GetExpectedLength(int width, int height, int depth)
        {
            long expectedLength = (long)width * height * depth;
            if (expectedLength > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(width), "Serialized 3D array dimensions are too large.");

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
