using System;

namespace CHG._02.Script.CombatSystem
{
    public interface IDamageable
    {
        public event Action OnDeaded;
        public event Action<DamageData> OnDamaged;
        
        public float CurrentHealth { get; }
        public float MaxHealth { get; }
        public bool IsDead { get; }
        
        public void TakeDamage(DamageData data);
        public void Heal(float heal);
        public void Dead();
    }
}