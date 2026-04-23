using SwiftCollections.Query;
using UnityEngine;

namespace SwiftCollections.Samples
{
    public enum TargetMovementMode
    {
        PingPongX,
        PingPongZ,
        Orbit,
        VerticalBob
    }

    [DisallowMultipleComponent]
    public sealed class TargetAgent : MonoBehaviour
    {
        [SerializeField]
        private Renderer _targetRenderer;

        [SerializeField]
        private Vector3 _fallbackBoundsSize = new(1f, 1f, 1f);

        [SerializeField]
        private Color _baseColor = new(0.2f, 0.7f, 1f, 1f);

        [SerializeField]
        private Color _highlightColor = new(0.25f, 1f, 0.45f, 1f);

        private MaterialPropertyBlock _propertyBlock;

        private SpatialQueryController _spatialQueryController;
        private Vector3 _anchorPosition;
        private float _movementAmplitude;
        private float _movementSpeed;
        private float _phaseOffset;
        private TargetMovementMode _movementMode;

        public int TargetId { get; private set; } = -1;

        private void Awake()
        {
            _propertyBlock = new();
            _targetRenderer = _targetRenderer != null 
                ? _targetRenderer 
                : GetComponentInChildren<Renderer>();
        }

        public void Initialize(
            int targetId,
            Vector3 anchorPosition,
            TargetMovementMode movementMode,
            float movementAmplitude,
            float movementSpeed,
            float phaseOffset,
            SpatialQueryController spatialQueryController)
        {
            TargetId = targetId;
            _anchorPosition = anchorPosition;
            _movementMode = movementMode;
            _movementAmplitude = Mathf.Max(0.1f, movementAmplitude);
            _movementSpeed = Mathf.Max(0.1f, movementSpeed);
            _phaseOffset = phaseOffset;
            _spatialQueryController = spatialQueryController;

            transform.position = _anchorPosition;
            SetHighlighted(false);
        }

        private void Update()
        {
            float t = (Time.time * _movementSpeed) + _phaseOffset;
            Vector3 position = _anchorPosition;

            switch (_movementMode)
            {
                case TargetMovementMode.PingPongX:
                    position.x += Mathf.Sin(t) * _movementAmplitude;
                    break;

                case TargetMovementMode.PingPongZ:
                    position.z += Mathf.Sin(t) * _movementAmplitude;
                    break;

                case TargetMovementMode.Orbit:
                    position.x += Mathf.Cos(t) * _movementAmplitude;
                    position.z += Mathf.Sin(t) * _movementAmplitude;
                    break;

                case TargetMovementMode.VerticalBob:
                    position.y += Mathf.Sin(t) * _movementAmplitude * 0.5f;
                    break;
            }

            transform.position = position;
            if(_spatialQueryController != null)
                _spatialQueryController.NotifyTargetMoved(this);
        }

        public Bounds GetWorldBounds()
        {
            if (_targetRenderer != null)
            {
                return _targetRenderer.bounds;
            }

            return new Bounds(transform.position, _fallbackBoundsSize);
        }

        public BoundVolume GetBoundVolume()
        {
            return GetWorldBounds().ToBoundVolume();
        }

        public void SetHighlighted(bool highlighted)
        {
            if (_targetRenderer == null)
            {
                return;
            }

            Color color = highlighted ? _highlightColor : _baseColor;
            _targetRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", color);
            _propertyBlock.SetColor("_BaseColor", color);
            _targetRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
