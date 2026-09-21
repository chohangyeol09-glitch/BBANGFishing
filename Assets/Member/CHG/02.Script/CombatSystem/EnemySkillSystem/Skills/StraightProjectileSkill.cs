using System.Collections;
using CHG._02.Script.CombatSystem.Projectile;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class StraightProjectileSkill : AbstractEnemySkill, IProjectileSkill
    {
        public PoolManagerSO PoolManager => poolManager;
        public PoolItemSO PoolItem => poolItem;

        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO poolItem;

        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float health;
        
        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            StraightProjectile projectile = poolManager.Pop<StraightProjectile>(PoolItem);
            var template = new DamageData(Owner, Vector3.zero, Vector3.zero, dir, Data.Damage, 0f);
            Debug.Log(projectile == null);
            projectile.Launch(transform.position, dir, speed, lifeTime, health, template, poolManager);
            yield break;
        }

    }
}