using FixedMathSharp;
using SwiftCollections.Query;
using UnityEngine;

namespace SwiftCollections
{
    public static class FixedBoundVolumeExtensions
    {
        public static FixedBoundVolume ToFixedBoundVolume(this Bounds bounds)
        {
            FixedBoundVolume volume = new(bounds.min.ToVector3d(), bounds.max.ToVector3d());

            return volume;
        }
    }
}