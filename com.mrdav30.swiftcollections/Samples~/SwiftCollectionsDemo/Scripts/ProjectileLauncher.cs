using System;
using SwiftCollections.Pool;
using UnityEngine;

namespace SwiftCollections.Samples
{
    [DisallowMultipleComponent]
    public sealed class ProjectileLauncher : MonoBehaviour
    {
        [SerializeField]
        private string _poolId = "Projectile";

        [SerializeField]
        private Transform _muzzle;

        [SerializeField]
        [Min(0.1f)]
        private float _shotsPerSecond = 8f;

        [SerializeField]
        [Min(0.1f)]
        private float _projectileSpeed = 18f;

        [SerializeField]
        [Min(0.1f)]
        private float _projectileLifetime = 2f;

        [SerializeField]
        [Min(1)]
        private int _configuredPoolBudget = 48;

        [SerializeField]
        private bool _prewarmEnabled = true;

        private TargetRegistry _targetRegistry;
        private float _shotTimer;

        public string PoolId => _poolId;
        public int ConfiguredPoolBudget => _configuredPoolBudget;
        public bool PrewarmEnabled => _prewarmEnabled;

        private void Awake()
        {
            if (_muzzle == null)
            {
                _muzzle = transform;
            }
        }

        private void Update()
        {
            if (_targetRegistry == null)
            {
                return;
            }

            _shotTimer -= Time.deltaTime;
            while (_shotTimer <= 0f)
            {
                _shotTimer += 1f / Mathf.Max(0.01f, _shotsPerSecond);
                Fire();
            }
        }

        public void Initialize(TargetRegistry targetRegistry)
        {
            _targetRegistry = targetRegistry;
        }

        public void Fire()
        {
            try
            {
                if (!SwiftGameObjectPoolManager.Shared.TryGetObject(_poolId, out GameObject projectileObject))
                {
                    _targetRegistry?.RegisterPoolMiss();
                    return;
                }

                projectileObject.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);

                PooledProjectile projectile = projectileObject.GetComponent<PooledProjectile>();
                if (projectile == null)
                {
                    Debug.LogError(
                        $"Pooled object from pool '{_poolId}' is missing a {nameof(PooledProjectile)} component.",
                        this);

                    SwiftGameObjectPoolManager.Shared.ReleaseObject(_poolId, projectileObject);
                    return;
                }

                projectile.Launch(_poolId, _muzzle.forward, _projectileSpeed, _projectileLifetime, _targetRegistry);
                _targetRegistry?.RegisterProjectileCheckout();
                projectileObject.SetActive(true);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                enabled = false;
            }
        }
    }
}
