using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.HitFeedback
{
    public class PoolParticle : PoolableMono
    {
        [SerializeField] private PoolManagerSO poolManager;
        private ParticleSystem _ps;
        private bool _released;

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        public override void ResetItem()
        {
            _released = false;
            _ps.Clear(true);
            _ps.Play(true);
        }

        private void OnParticleSystemStopped()
        {
            if (_released) return;
            _released = true;
            poolManager.Push(this);
        }
    }
}