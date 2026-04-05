using SwiftCollections;
using SwiftCollections.Query;
using UnityEngine;

namespace SwiftCollections.Samples
{
    [DisallowMultipleComponent]
    public sealed class SpatialQueryController : MonoBehaviour
    {
        [SerializeField]
        private TargetRegistry _targetRegistry;

        [SerializeField]
        private Transform _queryVolumeVisual;

        [SerializeField]
        private Vector3 _queryVolumeSize = new Vector3(6f, 3f, 6f);

        [SerializeField]
        private Vector3 _sweepExtents = new Vector3(10f, 0f, 8f);

        [SerializeField]
        [Min(0.1f)]
        private float _sweepSpeed = 0.8f;

        [SerializeField]
        private bool _animateQueryVolume = true;

        [SerializeField]
        private bool _useBvh = true;

        [SerializeField]
        private KeyCode _toggleModeKey = KeyCode.Tab;

        private SwiftBVH<int> _bvh;
        private SwiftList<int> _queryResults;
        private SwiftList<TargetAgent> _registeredTargets;
        private Vector3 _queryOrigin;

        public string QueryModeLabel => _useBvh ? "BVH Query" : "Linear Scan";
        public int RegisteredTargetCount => _registeredTargets != null ? _registeredTargets.Count : 0;
        public Vector3 QueryVolumeSize => _queryVolumeSize;

        private void Awake()
        {
            if (_queryVolumeVisual == null)
            {
                _queryVolumeVisual = transform;
            }

            _queryOrigin = _queryVolumeVisual.position;
            EnsureRuntimeState(1);
            SyncQueryVisualScale();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleModeKey))
            {
                _useBvh = !_useBvh;
                _targetRegistry?.SetQueryModeLabel(QueryModeLabel);
            }
        }

        private void LateUpdate()
        {
            if (_queryVolumeVisual == null || _targetRegistry == null)
            {
                return;
            }

            if (_animateQueryVolume)
            {
                AnimateQueryVolume();
            }

            ExecuteQuery();
        }

        private void OnValidate()
        {
            _queryVolumeSize.x = Mathf.Max(0.1f, _queryVolumeSize.x);
            _queryVolumeSize.y = Mathf.Max(0.1f, _queryVolumeSize.y);
            _queryVolumeSize.z = Mathf.Max(0.1f, _queryVolumeSize.z);

            if (_queryVolumeVisual != null)
            {
                SyncQueryVisualScale();
            }
        }

        public void Initialize(TargetRegistry targetRegistry, int expectedTargetCount)
        {
            _targetRegistry = targetRegistry;
            int capacity = Mathf.Max(1, expectedTargetCount);
            _queryResults = new SwiftList<int>(capacity);
            _registeredTargets = new SwiftList<TargetAgent>(capacity);
            _bvh = new SwiftBVH<int>(capacity);
            _targetRegistry?.SetQueryModeLabel(QueryModeLabel);

            if (_queryVolumeVisual == null)
            {
                _queryVolumeVisual = transform;
            }

            _queryOrigin = _queryVolumeVisual.position;
            SyncQueryVisualScale();
        }

        public void RegisterTarget(TargetAgent target)
        {
            if (target == null)
            {
                return;
            }

            EnsureRuntimeState(RegisteredTargetCount + 1);

            if (_registeredTargets.Contains(target))
            {
                return;
            }

            _registeredTargets.Add(target);
            _bvh.Insert(target.TargetId, target.GetBoundVolume());
        }

        public void NotifyTargetMoved(TargetAgent target)
        {
            if (target == null || _bvh == null)
            {
                return;
            }

            _bvh.UpdateEntryBounds(target.TargetId, target.GetBoundVolume());
        }

        private void ExecuteQuery()
        {
            if (_bvh == null)
            {
                return;
            }

            BoundVolume queryVolume = new Bounds(_queryVolumeVisual.position, _queryVolumeSize).ToBoundVolume();
            _queryResults.Clear();

            if (_useBvh)
            {
                _bvh.Query(queryVolume, _queryResults);
            }
            else
            {
                for (int i = 0; i < _registeredTargets.Count; i++)
                {
                    TargetAgent target = _registeredTargets[i];
                    if (target == null)
                    {
                        continue;
                    }

                    if (queryVolume.Intersects(target.GetBoundVolume()))
                    {
                        _queryResults.Add(target.TargetId);
                    }
                }
            }

            _targetRegistry.SetQueryModeLabel(QueryModeLabel);
            _targetRegistry.ApplyHighlights(_queryResults);
        }

        private void AnimateQueryVolume()
        {
            float t = Time.time * _sweepSpeed;
            Vector3 offset = new Vector3(
                Mathf.Sin(t) * _sweepExtents.x,
                Mathf.Sin(t * 0.5f) * _sweepExtents.y,
                Mathf.Cos(t * 0.85f) * _sweepExtents.z);

            _queryVolumeVisual.position = _queryOrigin + offset;
        }

        private void EnsureRuntimeState(int expectedTargetCount)
        {
            int capacity = Mathf.Max(1, expectedTargetCount);

            _queryResults ??= new SwiftList<int>(capacity);
            _registeredTargets ??= new SwiftList<TargetAgent>(capacity);
            _bvh ??= new SwiftBVH<int>(capacity);
        }

        private void SyncQueryVisualScale()
        {
            _queryVolumeVisual.localScale = _queryVolumeSize;
        }
    }
}
