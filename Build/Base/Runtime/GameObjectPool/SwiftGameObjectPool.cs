using System;
using UnityEngine;

namespace SwiftCollections.Pool
{
    /// <summary>
    /// Manages a pool of GameObjects for efficient reuse, reducing memory allocation overhead in Unity.
    /// </summary>
    [Serializable]
    public class SwiftGameObjectPool : IDisposable
    {
        #region Unity Inspector Properties

        /// <summary>
        /// Name of the pool used for identification.
        /// </summary>
        [Tooltip("Name of the pool used for identification.")]
        [SerializeField]
        private string _poolName;
        public string PoolName => _poolName;

        /// <summary>
        /// The prefab to be instantiated and pooled.
        /// </summary>
        [Tooltip("The prefab to be instantiated and pooled.")]
        [SerializeField]
        private GameObject _prefab;
        public GameObject Prefab => _prefab;

        /// <summary>
        /// The maximum number of objects the pool can manage.
        /// </summary>
        [Tooltip("The maximum number of objects the pool can manage.")]
        [SerializeField]
        private int _budget;
        public int Budget => _budget;

        /// <summary>
        /// Whether the pool should be prewarmed (instantiating objects up to the budget at initialization).
        /// </summary>
        [Tooltip("Whether the pool should be prewarmed (instantiating objects up to the budget at initialization).")]
        [SerializeField]
        private bool _prewarm;
        public bool Prewarm => _prewarm;

        #endregion

        [NonSerialized]
        private SwiftList<GameObject> _createdObjects;
        public SwiftList<GameObject> CreatedObjects
        {
            get
            {
                EnsureRuntimeState();
                return _createdObjects;
            }
        }

        [NonSerialized]
        private SwiftStack<GameObject> _availableObjects;

        [NonSerialized]
        private SwiftHashSet<GameObject> _inUseObjects;

        [NonSerialized]
        private int _liveObjectCount;

        public SwiftGameObjectPool()
        {
            EnsureRuntimeState();
        }

        public SwiftGameObjectPool(string poolName, GameObject prefab, int budget, bool prewarm = false): this()
        {
            _poolName = poolName;
            _prefab = prefab;
            _budget = budget;
            _prewarm = prewarm;
        }

        /// <summary>
        /// Rents a GameObject from the pool. If the pool is not full, a new object is instantiated.
        /// </summary>
        /// <param name="parent">The parent transform for the GameObject.</param>
        /// <returns>A pooled GameObject instance.</returns>
        public GameObject GetObject(Transform parent)
        {
            if (TryGetObject(parent, out var gameObject))
                return gameObject;

            throw new InvalidOperationException(
                $"Pool '{_poolName}' is exhausted. Release an object back to the pool before requesting another.");
        }

        /// <summary>
        /// Attempts to rent a GameObject from the pool without throwing when the pool is exhausted.
        /// </summary>
        /// <param name="parent">The parent transform for the GameObject.</param>
        /// <param name="gameObject">The pooled GameObject instance when successful.</param>
        /// <returns><c>true</c> when an object was acquired; otherwise <c>false</c>.</returns>
        public bool TryGetObject(Transform parent, out GameObject gameObject)
        {
            EnsureRuntimeState();
            SwiftThrowHelper.ThrowIfNull(parent, nameof(parent));
            SwiftThrowHelper.ThrowIfNull(_prefab, nameof(_prefab));
            ValidateBudget();

            gameObject = TryGetAvailableObject();
            if (gameObject == null && _liveObjectCount < _budget)
            {
                GameObject go = UnityEngine.Object.Instantiate(_prefab);
                go.transform.SetParent(parent, false);
                go.SetActive(false);

                _createdObjects.Add(go);
                _liveObjectCount++;
                gameObject = go;
            }

            if (gameObject == null)
                return false;

            gameObject.transform.SetParent(parent, false);
            gameObject.SetActive(false);
            _inUseObjects.Add(gameObject);

            return true;
        }

        /// <summary>
        /// Releases a GameObject back into the pool so it can be reused in O(1) time.
        /// </summary>
        /// <param name="gameObject">The pooled instance to release.</param>
        /// <param name="parent">The pool parent transform.</param>
        public void ReleaseObject(GameObject gameObject, Transform parent)
        {
            EnsureRuntimeState();
            SwiftThrowHelper.ThrowIfNull(parent, nameof(parent));
            if (ReferenceEquals(gameObject, null))
                throw new ArgumentNullException(nameof(gameObject));

            if (!_inUseObjects.Remove(gameObject))
            {
                var objectName = gameObject == null ? "<destroyed>" : gameObject.name;
                throw new InvalidOperationException(
                    $"GameObject '{objectName}' is not currently checked out from pool '{_poolName}'.");
            }

            if (gameObject == null)
            {
                if (_liveObjectCount > 0)
                    _liveObjectCount--;

                return;
            }

            gameObject.SetActive(false);
            gameObject.transform.SetParent(parent, false);
            _availableObjects.Push(gameObject);
        }

        /// <summary>
        /// Prewarms the pool by instantiating objects up to the budget.
        /// </summary>
        /// <param name="parent">The parent transform for the prewarmed objects.</param>
        public void PrewarmObject(Transform parent)
        {
            EnsureRuntimeState();
            SwiftThrowHelper.ThrowIfNull(parent, nameof(parent));
            SwiftThrowHelper.ThrowIfNull(_prefab, nameof(_prefab));
            ValidateBudget();

            int objectsToCreate = _budget - _liveObjectCount;
            for (int i = 0; i < objectsToCreate; i++)
            {
                GameObject go = UnityEngine.Object.Instantiate(_prefab);
                go.SetActive(false);
                go.transform.SetParent(parent, false);
                _createdObjects.Add(go);
                _availableObjects.Push(go);
                _liveObjectCount++;
            }
        }

        /// <summary>
        /// Disposes the pool, destroying all managed GameObjects.
        /// </summary>
        public void Dispose()
        {
            EnsureRuntimeState();

            foreach (var obj in _createdObjects)
            {
                if (obj != null)
                    UnityEngine.Object.Destroy(obj);
            }

            _createdObjects.Clear();
            _availableObjects.Clear();
            _inUseObjects.Clear();
            _liveObjectCount = 0;
        }

        private void EnsureRuntimeState()
        {
            _createdObjects ??= new SwiftList<GameObject>();
            _availableObjects ??= new SwiftStack<GameObject>();
            _inUseObjects ??= new SwiftHashSet<GameObject>();
        }

        private void ValidateBudget()
        {
            if (_budget <= 0)
            {
                throw new InvalidOperationException(
                    $"Pool '{_poolName}' must have a budget greater than zero.");
            }
        }

        private GameObject TryGetAvailableObject()
        {
            while (_availableObjects.Count > 0)
            {
                var candidate = _availableObjects.Pop();
                if (candidate != null)
                    return candidate;

                if (_liveObjectCount > 0)
                    _liveObjectCount--;
            }

            return null;
        }
    }
}
