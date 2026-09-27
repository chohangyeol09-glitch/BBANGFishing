using System;
using CHG._02.Script.CoreSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.Projectile
{
    public class CurvedProjectile : AbstractProjectile
    {
        private Vector3 _p0, _p1, _p2;
        private float _duration;
        private float _elapsed;
        
        public void Launch(Vector3 start, Vector3 control, Vector3 end, float duration, float health, DamageData damageData, PoolManagerSO poolManager)
        {
            InitializeProjectile(damageData, poolManager, health);
            _p0 = start;
            _p1 = control;
            _p2 = end;
            _duration = duration;
            _elapsed = 0f;
            transform.position = start;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);
            Vector3 prev = transform.position;
            transform.position = PhysicsUtil.Bezier(t,_p0, _p1, _p2);
            if (transform.position != prev) transform.rotation = Quaternion.LookRotation(transform.position - prev);
            if (t >= 1f) PoolManager.Push(this);
        }



        protected override DamageData SetDamageData()
            => new DamageData(DamageTemplate.Attacker, transform.position, Vector3.up, transform.forward,
                DamageTemplate.Damage, 0f);

        public override void ResetItem()
        {
            base.ResetItem();
            _elapsed = 0f;
        }
    }
}