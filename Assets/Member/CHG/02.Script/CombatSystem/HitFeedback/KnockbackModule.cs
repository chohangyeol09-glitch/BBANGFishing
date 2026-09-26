using CHG._02.Script.Agents;
using CHG._02.Script.CoreSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.HitFeedback
{
    public class KnockbackModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField, Range(0f, 1f)] private float weightInfluence = 0.5f;
        [SerializeField, Min(0f)] private float upMultiplier = 1f;           

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

            float impulse = PhysicsUtil.ResolveImpulse(data.KnockbackPower, _rb.mass, weightInfluence);

            Vector3 dir = data.HitDirection.normalized;
            if (dir.y > 0f) dir.y *= upMultiplier;

            _rb.AddForce(dir * impulse, ForceMode.Impulse);
        }
    }
}
