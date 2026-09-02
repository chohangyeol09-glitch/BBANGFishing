namespace Member.CHG._02.Script
{
    public interface IDamageable
    {
        public float CurrentHealth { get; }
        public float MaxHealth { get; }
        public bool IsDead { get; }
        
        public void TakeDamage(float damage);
        public void Heal(float heal);
        public void Dead();
    }
}