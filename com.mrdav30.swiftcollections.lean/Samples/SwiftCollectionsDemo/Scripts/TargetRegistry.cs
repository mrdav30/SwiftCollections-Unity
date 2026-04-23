using SwiftCollections;
using UnityEngine;

namespace SwiftCollections.Samples
{
    [DisallowMultipleComponent]
    public sealed class TargetRegistry : MonoBehaviour
    {
        private SwiftDictionary<int, TargetAgent> _targetsById;
        private SwiftHashSet<int> _activeQueryTargetIds;
        private SwiftList<int> _previousQueryTargetIds;

        private int _nextTargetId = 1;

        public int RegisteredTargetCount => _targetsById != null ? _targetsById.Count : 0;
        public int ActiveProjectileCount { get; private set; }
        public int PoolMissCount { get; private set; }
        public int CurrentQueryHitCount { get; private set; }
        public string QueryModeLabel { get; private set; } = "BVH Query";

        private void Awake()
        {
            EnsureRuntimeState();
        }

        public void InitializeRuntimeState(int expectedTargetCount)
        {
            _targetsById = new SwiftDictionary<int, TargetAgent>(Mathf.Max(1, expectedTargetCount));
            _activeQueryTargetIds = new SwiftHashSet<int>();
            _previousQueryTargetIds = new SwiftList<int>(Mathf.Max(1, expectedTargetCount));

            _nextTargetId = 1;
            ActiveProjectileCount = 0;
            PoolMissCount = 0;
            CurrentQueryHitCount = 0;
            QueryModeLabel = "BVH Query";
        }

        public int RegisterTarget(TargetAgent target)
        {
            EnsureRuntimeState();

            int targetId = _nextTargetId++;
            if (!_targetsById.Add(targetId, target))
            {
                Debug.LogWarning($"Failed to register target id {targetId}.", this);
            }

            return targetId;
        }

        public bool TryGetTarget(int targetId, out TargetAgent target)
        {
            EnsureRuntimeState();
            return _targetsById.TryGetValue(targetId, out target);
        }

        public void RegisterProjectileCheckout()
        {
            ActiveProjectileCount++;
        }

        public void RegisterProjectileReturn()
        {
            if (ActiveProjectileCount > 0)
            {
                ActiveProjectileCount--;
            }
        }

        public void RegisterPoolMiss()
        {
            PoolMissCount++;
        }

        public void SetQueryModeLabel(string queryModeLabel)
        {
            QueryModeLabel = string.IsNullOrWhiteSpace(queryModeLabel) ? "Unknown" : queryModeLabel;
        }

        public void ApplyHighlights(SwiftList<int> highlightedTargetIds)
        {
            EnsureRuntimeState();

            _previousQueryTargetIds.Clear();
            foreach (int targetId in _activeQueryTargetIds)
            {
                _previousQueryTargetIds.Add(targetId);
            }

            for (int i = 0; i < _previousQueryTargetIds.Count; i++)
            {
                int targetId = _previousQueryTargetIds[i];
                if (TryGetTarget(targetId, out TargetAgent target) && target != null)
                {
                    target.SetHighlighted(false);
                }
            }

            _activeQueryTargetIds.Clear();

            for (int i = 0; i < highlightedTargetIds.Count; i++)
            {
                int targetId = highlightedTargetIds[i];
                _activeQueryTargetIds.Add(targetId);

                if (TryGetTarget(targetId, out TargetAgent target) && target != null)
                {
                    target.SetHighlighted(true);
                }
            }

            CurrentQueryHitCount = highlightedTargetIds.Count;
        }

        private void EnsureRuntimeState()
        {
            _targetsById ??= new SwiftDictionary<int, TargetAgent>();
            _activeQueryTargetIds ??= new SwiftHashSet<int>();
            _previousQueryTargetIds ??= new SwiftList<int>();
        }
    }
}
