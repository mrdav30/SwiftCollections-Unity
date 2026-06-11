using FixedMathSharp;
using NUnit.Framework;
using SwiftCollections.Query;
using UnityEngine;

namespace SwiftCollections.Unity.Tests
{
    public class FixedBoundVolumeUnityExtensionsTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void ToFixedBoundVolume_UsesUnityBoundsMinAndMax()
        {
            Bounds source = new(new Vector3(4f, 5f, 6f), new Vector3(2f, 4f, 8f));

            FixedBoundVolume result = source.ToFixedBoundVolume();

            AssertVectorEqual(result.Min, source.min);
            AssertVectorEqual(result.Max, source.max);
            AssertVectorEqual(result.Center, source.center);
            AssertVectorEqual(result.Size, source.size);
        }

        private static void AssertVectorEqual(Vector3d actual, Vector3 expected)
        {
            Assert.That((float)actual.X, Is.EqualTo(expected.x).Within(Tolerance));
            Assert.That((float)actual.Y, Is.EqualTo(expected.y).Within(Tolerance));
            Assert.That((float)actual.Z, Is.EqualTo(expected.z).Within(Tolerance));
        }
    }
}
