using System;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.Projectile
{
    public class StraightProjectile : AbstractProjectile
    {
        
        private Vector3 _direction;
        private float _speed;
        private float _lifeTime;
        private float _elapsed;

        public void Launch(Vector3 origin, Vector3 dir, float speed, float lifeTime, float health, DamageData damageData,
            PoolManagerSO poolManager)
        {
            InitializeProjectile(damageData,poolManager, health);
            transform.position = origin;
            transform.rotation = Quaternion.LookRotation(dir);
            _direction = dir;
            _speed = speed;
            _lifeTime = lifeTime;
            _elapsed = 0f;
        }

        private void Update()
        {
            transform.position += _direction * (_speed * Time.deltaTime);
            _elapsed += Time.deltaTime;
            if (_elapsed >= _lifeTime) ReleaseToPool();
        }

        protected override DamageData SetDamageData()
            => new DamageData(DamageTemplate.Attacker, transform.position, -_direction, _direction,
                DamageTemplate.Damage, DamageTemplate.KnockbackPower);

        public override void ResetItem()
        {
            base.ResetItem();
            _elapsed = 0f;
        }
    }
}