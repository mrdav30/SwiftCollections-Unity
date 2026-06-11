using System;
using NUnit.Framework;
using SwiftCollections.Pool;
using UnityEngine;

namespace SwiftCollections.Unity.Tests
{
    public class SwiftGameObjectPoolTests
    {
        private GameObject _prefab;
        private GameObject _parent;

        [SetUp]
        public void SetUp()
        {
            _prefab = new GameObject("SwiftCollections Pool Prefab");
            _parent = new GameObject("SwiftCollections Pool Parent");
        }

        [TearDown]
        public void TearDown()
        {
            DestroyImmediate(_parent);
            DestroyImmediate(_prefab);
        }

        [Test]
        public void TryGetObject_CreatesInactiveChildAndReusesReleasedInstance()
        {
            SwiftGameObjectPool pool = new("projectiles", _prefab, 1);

            try
            {
                Assert.That(pool.TryGetObject(_parent.transform, out GameObject first), Is.True);
                Assert.That(first, Is.Not.SameAs(_prefab));
                Assert.That(first.activeSelf, Is.False);
                Assert.That(first.transform.parent, Is.EqualTo(_parent.transform));
                Assert.That(pool.CreatedObjects.Count, Is.EqualTo(1));

                pool.ReleaseObject(first, _parent.transform);

                Assert.That(pool.TryGetObject(_parent.transform, out GameObject reused), Is.True);
                Assert.That(reused, Is.SameAs(first));
                Assert.That(pool.CreatedObjects.Count, Is.EqualTo(1));
            }
            finally
            {
                DestroyCreatedObjects(pool);
            }
        }

        [Test]
        public void TryGetObject_WhenBudgetIsExhausted_ReturnsFalseWithoutAllocatingMore()
        {
            SwiftGameObjectPool pool = new("single", _prefab, 1);

            try
            {
                Assert.That(pool.TryGetObject(_parent.transform, out GameObject first), Is.True);
                Assert.That(pool.TryGetObject(_parent.transform, out GameObject second), Is.False);

                Assert.That(first, Is.Not.Null);
                Assert.That(second, Is.Null);
                Assert.That(pool.CreatedObjects.Count, Is.EqualTo(1));
            }
            finally
            {
                DestroyCreatedObjects(pool);
            }
        }

        [Test]
        public void ReleaseObject_WhenObjectWasNotCheckedOut_Throws()
        {
            SwiftGameObjectPool pool = new("projectiles", _prefab, 1);
            GameObject foreign = new("Foreign Object");

            try
            {
                Assert.Throws<InvalidOperationException>(() =>
                    pool.ReleaseObject(foreign, _parent.transform));
            }
            finally
            {
                DestroyCreatedObjects(pool);
                DestroyImmediate(foreign);
            }
        }

        private static void DestroyCreatedObjects(SwiftGameObjectPool pool)
        {
            foreach (GameObject createdObject in pool.CreatedObjects)
                DestroyImmediate(createdObject);

            pool.CreatedObjects.Clear();
        }

        private static void DestroyImmediate(GameObject gameObject)
        {
            if (gameObject != null)
                UnityEngine.Object.DestroyImmediate(gameObject);
        }
    }
}
