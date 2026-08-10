//=======================================================================
// BoundVolume.Extensions.cs
//=======================================================================
// MIT License, Copyright (c) 2024–present David Oravsky (mrdav30)
// See LICENSE file in the project root for full license information.
//=======================================================================

using SwiftCollections.Query;
using UnityEngine;

using NumericVector3 = System.Numerics.Vector3;

namespace SwiftCollections
{
    /// <summary>
    /// Provides conversion from Unity bounds to SwiftCollections query bounds.
    /// </summary>
    public static class BoundVolumeExtensions
    {
        /// <summary>
        /// Converts Unity axis-aligned bounds to a SwiftCollections bound volume.
        /// </summary>
        /// <param name="bounds">The Unity bounds to convert.</param>
        /// <returns>A bound volume with matching minimum and maximum coordinates.</returns>
        public static BoundVolume ToBoundVolume(this Bounds bounds)
        {
            BoundVolume volume = new(bounds.min.ToNumerics(), bounds.max.ToNumerics());

            return volume;
        }

        private static NumericVector3 ToNumerics(this Vector3 vector) =>
            new(vector.x, vector.y, vector.z);
    }
}
