using UnityEngine;
using UnityEngine.AI;

namespace Member.JJK._02._Scripts.Skill
{
    public class EnemyTimeScaleReceiver : MonoBehaviour
    {
        private Animator[] _animators;
        private float[] _animatorSpeeds;

        private NavMeshAgent[] _agents;
        private float[] _agentSpeeds;
        private float[] _agentAccelerations;
        private float[] _agentAngularSpeeds;

        private ParticleSystem[] _particleSystems;
        private float[] _particleSpeeds;

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

            _agents = GetComponentsInChildren<NavMeshAgent>(true);
            _agentSpeeds = new float[_agents.Length];
            _agentAccelerations = new float[_agents.Length];
            _agentAngularSpeeds = new float[_agents.Length];
            for (int i = 0; i < _agents.Length; i++)
            {
                _agentSpeeds[i] = _agents[i].speed;
                _agentAccelerations[i] = _agents[i].acceleration;
                _agentAngularSpeeds[i] = _agents[i].angularSpeed;
            }

            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            _particleSpeeds = new float[_particleSystems.Length];
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSpeeds[i] = _particleSystems[i].main.simulationSpeed;

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

            // 시간이 s배로 느려지면 중력 가속도는 s^2배가 되어야 같은 궤적을 느리게 그린다.
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

            for (int i = 0; i < _agents.Length; i++)
            {
                _agents[i].speed = _agentSpeeds[i] * scale;
                _agents[i].acceleration = _agentAccelerations[i] * scale;
                _agents[i].angularSpeed = _agentAngularSpeeds[i] * scale;
            }

            for (int i = 0; i < _particleSystems.Length; i++)
            {
                ParticleSystem.MainModule main = _particleSystems[i].main;
                main.simulationSpeed = _particleSpeeds[i] * scale;
            }

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
