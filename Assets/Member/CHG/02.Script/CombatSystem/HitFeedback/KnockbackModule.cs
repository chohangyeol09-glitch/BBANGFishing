using CHG._02.Script.Agents;
using CHG._02.Script.CoreSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.HitFeedback
{
    public class KnockbackModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField, Range(0f, 1f)] private float weightInfluence = 0.5f;
        [SerializeField, Min(0f)] private float upMultiplier = 2f;
        [Tooltip("켜면 공격자에게서 멀어지는(깊이) 방향으로는 밀리지 않는다")]
        [SerializeField] private bool removeDepth = true;
        [Tooltip("넉백 방향의 위쪽 성분 최솟값. -1이면 제한 없음(윗부분을 맞으면 아래로 밀림), 0이면 아래로는 밀리지 않음")]
        [SerializeField, Range(-1f, 1f)] private float minUp = -1f;

        private Rigidbody _rb;
        private Agent _agent;
        private IKnockbackGate _gate;

        public void Initialize(ModuleOwner owner)
        {
            _rb = owner.GetComponent<Rigidbody>();
            _agent = owner as Agent;
            _gate = owner as IKnockbackGate;
        }

        public void AfterInit()
        {
            if (_agent == null) return;
            _agent.OnDamaged += OnKnockback;
        }

        public void OnDestroy()
        {
            if (_agent == null) return;
            _agent.OnDamaged -= OnKnockback;
        }

        private void OnKnockback(DamageData data)
        {
            if (_rb == null) return;
            if (_gate != null && !_gate.CanBeKnockedBack) return;
            _rb.linearVelocity = Vector3.zero;
            float impulse = PhysicsUtil.ResolveImpulse(data.KnockbackPower, _rb.mass, weightInfluence);
            Vector3 dir = KnockbackDirection(data);
            if (dir.y > 0f) dir.y *= upMultiplier;

            _rb.AddForce(dir * impulse, ForceMode.Impulse);
        }

        private Vector3 KnockbackDirection(DamageData data)
        {
            bool fromOther = data.Attacker != null && data.Attacker != _agent; 

            Vector3 dir = fromOther && data.HitPoint != Vector3.zero
                ? _rb.worldCenterOfMass - data.HitPoint
                : data.HitDirection;

            if (removeDepth && fromOther)
            {
                Vector3 away = Vector3.ProjectOnPlane(_rb.position - data.Attacker.transform.position, Vector3.up);
                if (away.sqrMagnitude > 0.0001f) dir -= Vector3.Project(dir, away.normalized);
            }

            if (dir.sqrMagnitude < 0.000001f) return Vector3.up;
            dir.Normalize();

            if (dir.y < minUp)
            {
                dir.y = minUp;
                if (dir.sqrMagnitude < 0.000001f) return Vector3.up;
                dir.Normalize();
            }
            return dir;
        }
    }
}
