using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace Member.JJK._02._Scripts.Weapon
{
    public class BulletTracerModule : MonoBehaviour, IModule
    {
        [SerializeField] private float tracerDuration = 0.05f;
        
        private LineRenderer _lineRenderer;
        private float _timer;
        private bool _isActive;
        
        public void Initialize(ModuleOwner owner)
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
        }
        
        public void ShowTracer(Vector3 start, Vector3 end)
        {
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
            _lineRenderer.enabled = true;
            _timer = tracerDuration;
            _isActive = true;
        }
        
        private void FixedUpdate()
        {
            if (!_isActive) return;

            _timer -= Time.fixedDeltaTime;
            float t = Mathf.Clamp01(_timer / tracerDuration);

            var color = _lineRenderer.startColor;
            color.a = t;
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = new Color(color.r, color.g, color.b, 0f);
            
            if (_timer <= 0f)
            {
                _lineRenderer.enabled = false;
                _isActive = false;
            }
        }
    }
}