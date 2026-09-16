namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public interface ISkillStrategy
    {
        int? SelectNextSkillId(SkillStrategyContext context);
    }
}