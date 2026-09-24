using System;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.Projectile
{
    public class Bomb : PoolableMono, IDamageable
    {
        public event Action OnDeath;
        public event Action<DamageData> OnDamaged;
        public event Action<float> OnFuseProgress;
        public event Action OnExploded;
        
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;
        
        private DamageData _damageTemplate;
        private GameObject _target;
        private PoolManagerSO _poolManager;
        private float _fuseTime;
        private float _elapsed;
        private bool _released;
        
        public void Place(Vector3 position, float fuseTime, float health, GameObject target,
            DamageData damageTemplate, PoolManagerSO poolManager)
        {
            transform.position = position;
            _fuseTime = fuseTime;
            _elapsed = 0f;
            MaxHealth = CurrentHealth = health;
            _target = target;
            _damageTemplate = damageTemplate;
            _poolManager = poolManager;
            _released = false;
        }
        
        private void Update()
        {
            if (_released) return;

            _elapsed += Time.deltaTime;
            OnFuseProgress?.Invoke(Mathf.Clamp01(_elapsed / _fuseTime));
            if (_elapsed >= _fuseTime) Explode();
        }

        private void Explode()
        {
            if (_target != null)
            {
                var damageable = _target.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    Vector3 dir = (_target.transform.position - transform.position).normalized;
                    damageable.TakeDamage(new DamageData(_damageTemplate.Attacker, _target.transform.position,
                        -dir, dir, _damageTemplate.Damage, 0f));
                }
            }
            OnExploded?.Invoke();
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
        
        private void ReleaseToPool()
        {
            if (_released) return; // 파괴와 폭발이 같은 프레임에 겹쳐도 풀에 두 번 들어가지 않게
            _released = true;
            _poolManager.Push(this);
        }

        public override void ResetItem()
        {
            base.ResetItem();
            _elapsed = 0f;
        }
    }
}