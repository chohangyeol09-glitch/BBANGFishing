using DevLib.ModuleSystem;
using UnityEngine;

namespace Member.CHG._02.Script.Agents
{
    public abstract class Agent : ModuleOwner, IDamageable
    {
        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Max(value, 0f);
                if (_currentHealth <= 0f)
                    Dead();

            }
        }
        public float MaxHealth => _maxHealth;
        
        private float _currentHealth;
        private float _maxHealth;
        public bool IsDead => _currentHealth <= 0f;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _currentHealth = MaxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }

        public void Heal(float heal)
        {
            CurrentHealth += heal;
        }

        public void Dead()
        {
            
        }
    }
}