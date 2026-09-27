using System;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.Projectile
{
    public abstract class AbstractProjectile : PoolableMono, IDamageable
    {
        public event Action OnDeath;
        public event Action<DamageData> OnDamaged;
        
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;
        
        protected DamageData DamageTemplate { get; private set; }
        protected PoolManagerSO PoolManager { get; private set; }

        private bool _released;

        protected void InitializeProjectile(DamageData data, PoolManagerSO poolManager, float health)
        {
            DamageTemplate = data;
            PoolManager = poolManager;
            MaxHealth = CurrentHealth = health;
            _released = false;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (_released) return;
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null || ReferenceEquals(damageable, DamageTemplate.Attacker)) return; // 자기 자신 무시

            damageable.TakeDamage(SetDamageData());
            ReleaseToPool();
        }

        
        
        public void TakeDamage(DamageData data)
        {
            if (IsDead) return;
            CurrentHealth -= data.Damage;
            OnDamaged?.Invoke(data);
            if (IsDead) Dead();
        }

        public void Heal(float heal) { }

        public void Dead()
        {
            OnDeath?.Invoke();
            ReleaseToPool();
        }
        
        protected abstract DamageData SetDamageData(); //hit 방향등 자식이 채우고 반환
        
        protected void ReleaseToPool()
        {
            if (_released) return; // 두 번 Push하면 풀에 같은 객체가 중복으로 들어가는 것 방지
            _released = true;
            PoolManager.Push(this);
        }

    }
}