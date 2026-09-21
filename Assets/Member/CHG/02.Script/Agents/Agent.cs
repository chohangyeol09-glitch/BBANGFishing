using System;
using CHG._02.Script.CombatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.Agents
{
    public abstract class Agent : ModuleOwner, IDamageable
    {
        public event Action OnDeaded;
        public event Action<DamageData> OnDamaged;
        
        public float CurrentHealth
        {
            get => _currentHealth;
            protected set
            {
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
                if (_currentHealth <= 0f)
                    Dead();

            }
        }

        public virtual float MaxHealth => _maxHealth;
        
        private float _currentHealth;
        [SerializeField] protected float _maxHealth = 100f;
        public bool IsDead => _currentHealth <= 0f;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _currentHealth = MaxHealth;
        }

        public virtual void TakeDamage(DamageData data)
        {
            if (IsDead) return;
            CurrentHealth -= data.Damage;
            OnDamaged?.Invoke(data);
        }
        
        public void Heal(float heal)
        {
            CurrentHealth += heal;
        }

        public virtual void Dead()
        {
            OnDeaded?.Invoke();
        }
    }
}