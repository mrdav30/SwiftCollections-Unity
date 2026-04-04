using System;
using UnityEngine;

namespace SwiftCollections.Pool
{
    /// <summary>
    /// A ScriptableObject that manages multiple <see cref="SwiftGameObjectPool"/> instances.
    /// </summary>
    [CreateAssetMenu(menuName = "Utilities/ScriptableObjectPooler")]
    public class SwiftGameObjectPoolAsset : ScriptableObject, IDisposable
    {
        #region Unity Inspector Properties

        /// <summary>
        /// Array of object pools managed by this asset.
        /// </summary>
        [Tooltip("Array of object pools managed by this asset.")]
        [SerializeField]
        private SwiftGameObjectPool[] _pools;

        #endregion

        [NonSerialized]
        private Transform _parentTransform;
        public Transform ParentTransform => _parentTransform;

        [NonSerialized]
        private SwiftDictionary<string, SwiftGameObjectPool> _poolDict;

        public SwiftGameObjectPoolAsset(SwiftGameObjectPool[] pools)
        {
            _pools = pools;
        }

        /// <summary>
        /// Initializes the object pool asset, creating the parent transform and prewarming pools as needed.
        /// </summary>
        public void Init()
        {
            if (_poolDict != null && _parentTransform != null)
                return;

            ValidatePools();

            if (_poolDict != null || _parentTransform != null)
                Dispose();

            _poolDict = new SwiftDictionary<string, SwiftGameObjectPool>(_pools.Length);
            _parentTransform = CreatePoolRoot();

            foreach (var pool in _pools)
            {
                _poolDict.Add(pool.PoolName, pool);

                if (pool.Prewarm)
                    pool.PrewarmObject(_parentTransform);
            }
        }

        /// <summary>
        /// Gets an object from the specified pool by ID.
        /// </summary>
        /// <param name="id">The pool name.</param>
        /// <returns>A pooled GameObject instance.</returns>
        public GameObject GetObject(string id)
        {
            if (TryGetObject(id, out var gameObject))
                return gameObject;

            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Pool ID cannot be null or empty.", nameof(id));

            throw new InvalidOperationException($"Pool with ID '{id}' not found or is exhausted.");
        }

        /// <summary>
        /// Attempts to get an object from the specified pool by ID without throwing when the pool is missing or exhausted.
        /// </summary>
        /// <param name="id">The pool name.</param>
        /// <param name="gameObject">The pooled GameObject instance when successful.</param>
        /// <returns><c>true</c> when an object was acquired; otherwise <c>false</c>.</returns>
        public bool TryGetObject(string id, out GameObject gameObject)
        {
            Init();
            gameObject = null;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            if (!_poolDict.TryGetValue(id, out var pool))
                return false;

            return pool.TryGetObject(_parentTransform, out gameObject);
        }

        /// <summary>
        /// Releases an object back into the specified pool.
        /// </summary>
        /// <param name="id">The pool name.</param>
        /// <param name="gameObject">The pooled instance to release.</param>
        public void ReleaseObject(string id, GameObject gameObject)
        {
            Init();

            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Pool ID cannot be null or empty.", nameof(id));

            if (!_poolDict.TryGetValue(id, out var pool))
                throw new InvalidOperationException($"Pool with ID '{id}' not found.");

            if (ReferenceEquals(gameObject, null))
                throw new ArgumentNullException(nameof(gameObject));

            pool.ReleaseObject(gameObject, _parentTransform);
        }

        /// <summary>
        /// Disposes all pools and their resources.
        /// </summary>
        public void Dispose()
        {
            if (_pools != null)
            {
                foreach (var pool in _pools)
                    pool?.Dispose();
            }

            if (_parentTransform != null)
                UnityEngine.Object.Destroy(_parentTransform.gameObject);

            _poolDict = null;
            _parentTransform = null;
        }

        private void ValidatePools()
        {
            if (_pools == null || _pools.Length == 0)
                throw new InvalidOperationException("Pools array is not initialized or empty.");

            var poolNames = new SwiftHashSet<string>(StringComparer.Ordinal);
            foreach (var pool in _pools)
            {
                if (pool == null)
                    throw new InvalidOperationException("Pools array contains a null pool definition.");

                if (string.IsNullOrWhiteSpace(pool.PoolName))
                    throw new InvalidOperationException("Each pool must have a non-empty pool name.");

                if (!poolNames.Add(pool.PoolName))
                    throw new InvalidOperationException($"Duplicate pool name '{pool.PoolName}' found.");

                if (pool.Prefab == null)
                    throw new InvalidOperationException($"Pool '{pool.PoolName}' is missing a prefab.");

                if (pool.Budget <= 0)
                    throw new InvalidOperationException(
                        $"Pool '{pool.PoolName}' must have a budget greater than zero.");
            }
        }

        private Transform CreatePoolRoot()
        {
            var poolRoot = new GameObject($"{name} Pool Root");
            DontDestroyOnLoad(poolRoot);
            return poolRoot.transform;
        }
    }
}
