//=======================================================================
// FixedBoundVolume.Extensions.cs
//=======================================================================
// MIT License, Copyright (c) 2024–present David Oravsky (mrdav30)
// See LICENSE file in the project root for full license information.
//=======================================================================

using FixedMathSharp;
using SwiftCollections.Query;
using UnityEngine;

namespace SwiftCollections
{
    /// <summary>
    /// Provides conversion from Unity bounds to fixed-point SwiftCollections query bounds.
    /// </summary>
    public static class FixedBoundVolumeExtensions
    {
        /// <summary>
        /// Converts Unity axis-aligned bounds to a fixed-point bound volume.
        /// </summary>
        /// <param name="bounds">The Unity bounds to convert.</param>
        /// <returns>A fixed-point bound volume with converted minimum and maximum coordinates.</returns>
        public static FixedBoundVolume ToFixedBoundVolume(this Bounds bounds)
        {
            FixedBoundVolume volume = new(bounds.min.ToVector3d(), bounds.max.ToVector3d());

            return volume;
        }
    }
}
