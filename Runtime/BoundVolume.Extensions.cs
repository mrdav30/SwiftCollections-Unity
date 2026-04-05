using SwiftCollections.Query;
using UnityEngine;
using NumericVector3 = System.Numerics.Vector3;

namespace SwiftCollections
{
    public static class BoundVolumeExtensions
    {
        public static BoundVolume ToBoundVolume(this Bounds bounds)
        {
            BoundVolume volume = new(bounds.min.ToNumerics(), bounds.max.ToNumerics());

            return volume;
        }

        private static NumericVector3 ToNumerics(this Vector3 vector) =>
            new(vector.x, vector.y, vector.z);
    }
}
