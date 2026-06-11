using NUnit.Framework;
using SwiftCollections.Query;
using UnityEngine;

using NumericVector3 = System.Numerics.Vector3;

namespace SwiftCollections.Unity.Tests
{
    public class BoundVolumeUnityExtensionsTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void ToBoundVolume_UsesUnityBoundsMinAndMax()
        {
            Bounds source = new(new Vector3(4f, 5f, 6f), new Vector3(2f, 4f, 8f));

            BoundVolume result = source.ToBoundVolume();

            AssertVectorEqual(result.Min, source.min);
            AssertVectorEqual(result.Max, source.max);
            AssertVectorEqual(result.Center, source.center);
            AssertVectorEqual(result.Size, source.size);
        }

        [Test]
        public void ToBoundVolume_CanQueryBvhFromUnityBounds()
        {
            SwiftBVH<int> bvh = new(4);
            bvh.Insert(10, new Bounds(Vector3.zero, Vector3.one).ToBoundVolume());
            bvh.Insert(20, new Bounds(new Vector3(5f, 0f, 0f), Vector3.one).ToBoundVolume());

            SwiftList<int> results = new();
            Bounds query = new(new Vector3(0.25f, 0f, 0f), Vector3.one);
            bvh.Query(query.ToBoundVolume(), results);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0], Is.EqualTo(10));
        }

        private static void AssertVectorEqual(NumericVector3 actual, Vector3 expected)
        {
            Assert.That(actual.X, Is.EqualTo(expected.x).Within(Tolerance));
            Assert.That(actual.Y, Is.EqualTo(expected.y).Within(Tolerance));
            Assert.That(actual.Z, Is.EqualTo(expected.z).Within(Tolerance));
        }
    }
}
