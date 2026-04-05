using SwiftCollections.Pool;
using UnityEngine;

namespace SwiftCollections.Samples
{
    [DisallowMultipleComponent]
    public sealed class PooledProjectile : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _arenaExtents = new Vector3(22f, 6f, 18f);

        private static int s_nextSequenceId = 1;

        private string _poolId;
        private Vector3 _velocity;
        private float _remainingLifetime;
        private bool _isLive;
        private TargetRegistry _targetRegistry;

        public int SequenceId { get; private set; }

        public void Launch(string poolId, Vector3 direction, float speed, float lifetime, TargetRegistry targetRegistry)
        {
            if (SequenceId == 0)
            {
                SequenceId = s_nextSequenceId++;
                gameObject.name = $"Projectile [{SequenceId}]";
            }

            _poolId = poolId;
            _velocity = direction.normalized * speed;
            _remainingLifetime = lifetime;
            _targetRegistry = targetRegistry;
            _isLive = true;
        }

        private void Update()
        {
            if (!_isLive)
            {
                return;
            }

            transform.position += _velocity * Time.deltaTime;
            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f || IsOutsideArenaBounds())
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isLive)
            {
                return;
            }

            if (other.GetComponentInParent<TargetAgent>() != null)
            {
                ReturnToPool();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isLive)
            {
                return;
            }

            if (collision.collider.GetComponentInParent<TargetAgent>() != null)
            {
                ReturnToPool();
            }
        }

        public void ReturnToPool()
        {
            if (!_isLive)
            {
                return;
            }

            _isLive = false;
            _targetRegistry?.RegisterProjectileReturn();

            gameObject.SetActive(false);
            SwiftGameObjectPoolManager.Shared.ReleaseObject(_poolId, gameObject);
        }

        private bool IsOutsideArenaBounds()
        {
            Vector3 position = transform.position;

            return Mathf.Abs(position.x) > _arenaExtents.x
                || Mathf.Abs(position.y) > _arenaExtents.y
                || Mathf.Abs(position.z) > _arenaExtents.z;
        }
    }
}
