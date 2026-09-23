using DevLib.ObjectPool.Runtime;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public interface IProjectileSkill
    {
        PoolManagerSO PoolManager { get; }
        PoolItemSO PoolItem { get; }
    }
}