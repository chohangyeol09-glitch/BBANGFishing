using System;
using CHG._02.Script.CombatSystem.HitFeedback;
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

        [Header("Effect")]
        [Tooltip("무언가를 맞혔을 때 재생할 풀 파티클(PoolParticle). 없으면 생략")]
        [SerializeField] private PoolItemSO hitEffectItem;
        [Tooltip("공격받아 부서졌을 때 재생할 풀 파티클(PoolParticle). 없으면 생략")]
        [SerializeField] private PoolItemSO breakEffectItem;

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
            PlayEffect(hitEffectItem);
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
            PlayEffect(breakEffectItem);
            ReleaseToPool();
        }

        //투사체가 풀로 돌아가기 전에, 그 자리에서 풀 파티클을 재생한다 (파티클은 끝나면 스스로 반납된다)
        private void PlayEffect(PoolItemSO effectItem)
        {
            if (effectItem == null || PoolManager == null || _released) return;
            var effect = PoolManager.Pop<PoolParticle>(effectItem);
            if (effect != null) effect.transform.SetPositionAndRotation(transform.position, transform.rotation);
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