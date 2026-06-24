using UnityEngine;

namespace SwiftCollections.Samples
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TargetRegistry))]
    [RequireComponent(typeof(SpatialQueryController))]
    public sealed class DemoBootstrap : MonoBehaviour
    {
        [SerializeField]
        private TargetAgent _targetPrefab;

        [SerializeField]
        private Transform _targetsContainer;

        [SerializeField]
        private ProjectileLauncher _projectileLauncher;

        [SerializeField]
        private DemoHud _demoHud;

        [SerializeField]
        [Min(1)]
        private int _targetCount = 24;

        [SerializeField]
        [Min(1)]
        private int _targetRows = 4;

        [SerializeField]
        private Vector2 _arenaDimensions = new Vector2(24f, 14f);

        [SerializeField]
        private float _targetHeight = 0.75f;

        [SerializeField]
        private float _pingPongAmplitude = 1.5f;

        [SerializeField]
        private float _orbitRadius = 1.25f;

        [SerializeField]
        private float _movementSpeedMin = 0.8f;

        [SerializeField]
        private float _movementSpeedMax = 1.4f;

        private TargetRegistry _targetRegistry;
        private SpatialQueryController _spatialQueryController;
        private SwiftList<Transform> _spawnAnchors;

        private void Start()
        {
            if (_targetPrefab == null)
            {
                Debug.LogError($"{nameof(DemoBootstrap)} needs a {nameof(TargetAgent)} prefab reference.", this);
                enabled = false;
                return;
            }

            ResolveReferences();
            _targetRegistry.InitializeRuntimeState(_targetCount);
            _spatialQueryController.Initialize(_targetRegistry, _targetCount);

            BuildSpawnAnchors();
            SpawnTargets();

            _projectileLauncher?.Initialize(_targetRegistry);
            _demoHud?.Initialize(_targetRegistry, _spatialQueryController, _projectileLauncher);
        }

        private void ResolveReferences()
        {
            _targetRegistry = GetComponent<TargetRegistry>();
            _spatialQueryController = GetComponent<SpatialQueryController>();

            if (_targetsContainer == null)
            {
                GameObject targetContainer = new GameObject("Targets");
                targetContainer.transform.SetParent(transform, false);
                _targetsContainer = targetContainer.transform;
            }

            if (_projectileLauncher == null)
            {
                _projectileLauncher = GetComponentInChildren<ProjectileLauncher>();
            }

            if (_demoHud == null)
            {
                _demoHud = GetComponentInChildren<DemoHud>();
            }
        }

        private void BuildSpawnAnchors()
        {
            _spawnAnchors = new SwiftList<Transform>(_targetCount);

            GameObject anchorRoot = new GameObject("Generated Spawn Anchors");
            anchorRoot.transform.SetParent(transform, false);

            int rows = Mathf.Max(1, _targetRows);
            int columns = Mathf.CeilToInt((float)_targetCount / rows);

            float xStep = columns > 1 ? _arenaDimensions.x / (columns - 1) : 0f;
            float zStep = rows > 1 ? _arenaDimensions.y / (rows - 1) : 0f;

            for (int i = 0; i < _targetCount; i++)
            {
                int row = i % rows;
                int column = i / rows;

                float x = (-_arenaDimensions.x * 0.5f) + (column * xStep);
                float z = (-_arenaDimensions.y * 0.5f) + (row * zStep);

                GameObject anchor = new GameObject($"Target Anchor {i:00}");
                anchor.transform.SetParent(anchorRoot.transform, false);
                anchor.transform.localPosition = new Vector3(x, _targetHeight, z);
                _spawnAnchors.Add(anchor.transform);
            }
        }

        private void SpawnTargets()
        {
            float speedRange = Mathf.Max(0.1f, _movementSpeedMax - _movementSpeedMin);

            for (int i = 0; i < _spawnAnchors.Count; i++)
            {
                Transform anchor = _spawnAnchors[i];
                TargetAgent target = Instantiate(_targetPrefab, anchor.position, Quaternion.identity, _targetsContainer);

                int targetId = _targetRegistry.RegisterTarget(target);
                TargetMovementMode movementMode = (TargetMovementMode)(i % 4);

                float amplitude = movementMode == TargetMovementMode.Orbit
                    ? _orbitRadius
                    : _pingPongAmplitude + ((i % 3) * 0.35f);

                float speed = _movementSpeedMin + ((i % 5) * (speedRange / 4f));
                float phaseOffset = i * 0.33f;

                target.Initialize(
                    targetId,
                    anchor.position,
                    movementMode,
                    amplitude,
                    speed,
                    phaseOffset,
                    _spatialQueryController);

                _spatialQueryController.RegisterTarget(target);
            }
        }
    }
}
