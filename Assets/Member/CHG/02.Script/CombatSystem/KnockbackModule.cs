using CHG._02.Script.Agents;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem
{
    public class KnockbackModule : MonoBehaviour, IModule, IAfterInitModule
    {
        private Rigidbody _rb;
        private Agent _agent;

        public void Initialize(ModuleOwner owner)
        {
            _rb = owner.GetComponent<Rigidbody>();
            _agent = owner as Agent;
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
            
            _rb.AddForceAtPosition(data.HitDirection.normalized * data.KnockbackPower,
                data.HitPoint, ForceMode.Impulse);
        }
    }
}