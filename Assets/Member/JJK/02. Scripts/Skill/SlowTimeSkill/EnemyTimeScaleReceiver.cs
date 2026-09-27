using UnityEngine;
using UnityEngine.AI;

namespace Member.JJK._02._Scripts.Skill
{
    public class EnemyTimeScaleReceiver : MonoBehaviour
    {
        private Animator[] _animators;
        private float[] _animatorSpeeds;

        private Rigidbody[] _bodies;
        private bool[] _bodyUseGravity;
        private float[] _bodyLinearDamping;
        private float[] _bodyAngularDamping;
        private float _appliedScale = 1f;

        private void Awake()
        {
            _animators = GetComponentsInChildren<Animator>(true);
            _animatorSpeeds = new float[_animators.Length];
            for (int i = 0; i < _animators.Length; i++)
                _animatorSpeeds[i] = _animators[i].speed;

            _bodies = GetComponentsInChildren<Rigidbody>(true);
            _bodyUseGravity = new bool[_bodies.Length];
            _bodyLinearDamping = new float[_bodies.Length];
            _bodyAngularDamping = new float[_bodies.Length];
            for (int i = 0; i < _bodies.Length; i++)
            {
                _bodyUseGravity[i] = _bodies[i].useGravity;
                _bodyLinearDamping[i] = _bodies[i].linearDamping;
                _bodyAngularDamping[i] = _bodies[i].angularDamping;
            }
        }

        private void OnEnable()
        {
            EnemyTime.ScaleChanged += Apply;
            Apply(EnemyTime.Scale);
        }

        private void OnDisable()
        {
            EnemyTime.ScaleChanged -= Apply;
        }

        private void FixedUpdate()
        {
            if (Mathf.Approximately(_appliedScale, 1f)) return;
            
            Vector3 scaledGravity = Physics.gravity * (_appliedScale * _appliedScale);
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodyUseGravity[i] && !_bodies[i].isKinematic)
                    _bodies[i].AddForce(scaledGravity, ForceMode.Acceleration);
            }
        }

        private void Apply(float scale)
        {
            for (int i = 0; i < _animators.Length; i++)
                _animators[i].speed = _animatorSpeeds[i] * scale;

            ApplyToBodies(scale);
        }

        private void ApplyToBodies(float scale)
        {
            float velocityRatio = _appliedScale > 0.0001f ? scale / _appliedScale : 0f;
            bool isNormalSpeed = Mathf.Approximately(scale, 1f);

            for (int i = 0; i < _bodies.Length; i++)
            {
                Rigidbody body = _bodies[i];
                body.linearDamping = _bodyLinearDamping[i] * scale;
                body.angularDamping = _bodyAngularDamping[i] * scale;
                body.useGravity = _bodyUseGravity[i] && isNormalSpeed;

                if (body.isKinematic) continue;

                body.linearVelocity *= velocityRatio;
                body.angularVelocity *= velocityRatio;
            }

            _appliedScale = scale;
        }
    }
}
