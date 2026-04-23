using System;
using UnityEngine;

namespace SwiftCollections.Pool
{
    /// <summary>
    /// A singleton-based manager for accessing and managing pooled GameObjects through a <see cref="SwiftGameObjectPoolAsset"/>.
    /// </summary>
    public class SwiftGameObjectPoolManager
    {
        /// <summary>
        /// A lazily initialized singleton instance of the pooler.
        /// </summary>
        private static readonly Lazy<SwiftGameObjectPoolManager> _instance =
            new(() => new SwiftGameObjectPoolManager());

        /// <summary>
        /// Gets the shared singleton instance of the object pooler.
        /// </summary>
        public static SwiftGameObjectPoolManager Shared => _instance.Value;

        private readonly SwiftGameObjectPoolAsset _poolAsset;

        private SwiftGameObjectPoolManager()
        {
            _poolAsset = Resources.Load<SwiftGameObjectPoolAsset>("SwiftGameObjectPoolAsset");
            SwiftThrowHelper.ThrowIfNull(_poolAsset, "SwiftGameObjectPoolAsset asset not found in Resources.");
            _poolAsset.Init();
        }

        /// <summary>
        /// Gets a pooled GameObject by ID.
        /// </summary>
        /// <param name="id">The pool name.</param>
        /// <returns>A pooled GameObject instance.</returns>
        public GameObject GetObject(string id)
        {
            return _poolAsset.GetObject(id);
        }

        /// <summary>
        /// Attempts to get a pooled GameObject by ID without throwing when the pool is missing or exhausted.
        /// </summary>
        /// <param name="id">The pool name.</param>
        /// <param name="gameObject">The pooled GameObject instance when successful.</param>
        /// <returns><c>true</c> when an object was acquired; otherwise <c>false</c>.</returns>
        public bool TryGetObject(string id, out GameObject gameObject)
        {
            return _poolAsset.TryGetObject(id, out gameObject);
        }

        /// <summary>
        /// Releases a pooled GameObject back into the specified pool.
        /// </summary>
        /// <param name="id">The pool name.</param>
        /// <param name="gameObject">The pooled instance to release.</param>
        public void ReleaseObject(string id, GameObject gameObject)
        {
            _poolAsset.ReleaseObject(id, gameObject);
        }

        /// <summary>
        /// Gets the parent transform managing all pooled objects.
        /// </summary>
        /// <returns>The parent transform.</returns>
        public Transform GetParentTransform()
        {
            _poolAsset.Init();
            return _poolAsset.ParentTransform;
        }
    }
}
