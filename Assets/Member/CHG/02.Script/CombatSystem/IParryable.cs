namespace CHG._02.Script.CombatSystem
{
    public interface IParryable
    {
        bool IsParryable { get; } //지금 패링을 받을 수 있는 상태인가 (범위 안 + 접근 중)
        bool TryParry(DamageData data); //성공하면 true. 피해량은 data.Damage로 전달
    }
}
