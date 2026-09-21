using System.Collections;
using CHG._02.Script.CombatSystem.Projectile;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class BombSkill : AbstractEnemySkill
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO poolItem;
        [SerializeField] private float fuseTime = 3f;
        [SerializeField] private float health = 1f;

        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            Bomb bomb = poolManager.Pop<Bomb>(poolItem);
            var template = new DamageData(Owner, Vector3.zero, Vector3.zero, Vector3.zero, Data.Damage, 0f);
            bomb.Place(Owner.transform.position, fuseTime, health, target, template, poolManager);
            yield break;
        }
    }
}