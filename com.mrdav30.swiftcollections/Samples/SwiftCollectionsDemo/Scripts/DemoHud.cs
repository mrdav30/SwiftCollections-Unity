using UnityEngine;

namespace SwiftCollections.Samples
{
    [DisallowMultipleComponent]
    public sealed class DemoHud : MonoBehaviour
    {
        [SerializeField]
        private TargetRegistry _targetRegistry;

        [SerializeField]
        private SpatialQueryController _spatialQueryController;

        [SerializeField]
        private ProjectileLauncher _projectileLauncher;

        [SerializeField]
        private Rect _panelRect = new Rect(16f, 16f, 360f, 220f);

        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _warningStyle;

        public void Initialize(
            TargetRegistry targetRegistry,
            SpatialQueryController spatialQueryController,
            ProjectileLauncher projectileLauncher)
        {
            _targetRegistry = targetRegistry;
            _spatialQueryController = spatialQueryController;
            _projectileLauncher = projectileLauncher;
        }

        private void OnGUI()
        {
            if (_targetRegistry == null)
            {
                return;
            }

            EnsureStyles();

            Rect areaRect = new Rect(
                _panelRect.x,
                _panelRect.y,
                _panelRect.width,
                Mathf.Max(1f, Screen.height - _panelRect.y));

            GUILayout.BeginArea(areaRect);
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("SwiftCollections Showcase", _titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label($"Pool: {_projectileLauncher?.PoolId ?? "Projectile"}", _labelStyle);
            GUILayout.Label($"Configured Budget: {_projectileLauncher?.ConfiguredPoolBudget ?? 0}", _labelStyle);
            GUILayout.Label($"Prewarm Enabled: {(_projectileLauncher != null && _projectileLauncher.PrewarmEnabled ? "Yes" : "No")}", _labelStyle);
            GUILayout.Label($"Active Projectiles: {_targetRegistry.ActiveProjectileCount}", _labelStyle);

            GUIStyle missStyle = _targetRegistry.PoolMissCount > 0 ? _warningStyle : _labelStyle;
            GUILayout.Label($"Pool Misses: {_targetRegistry.PoolMissCount}", missStyle);

            GUILayout.Space(6f);
            GUILayout.Label($"Registered Targets: {_targetRegistry.RegisteredTargetCount}", _labelStyle);
            GUILayout.Label($"Current Query Hits: {_targetRegistry.CurrentQueryHitCount}", _labelStyle);
            GUILayout.Label($"Query Mode: {_targetRegistry.QueryModeLabel}", _labelStyle);

            if (_spatialQueryController != null)
            {
                GUILayout.Label($"Query Volume Size: {_spatialQueryController.QueryVolumeSize}", _labelStyle);
            }

            GUILayout.Space(6f);
            GUILayout.Label("Press Tab to switch between BVH Query and Linear Scan.", _labelStyle);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                wordWrap = true
            };

            _warningStyle = new GUIStyle(_labelStyle);
            _warningStyle.normal.textColor = new Color(1f, 0.35f, 0.35f, 1f);
        }
    }
}
