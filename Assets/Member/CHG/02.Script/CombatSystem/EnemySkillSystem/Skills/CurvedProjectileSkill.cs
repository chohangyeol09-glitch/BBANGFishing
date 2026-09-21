using System.Collections;
using CHG._02.Script.CombatSystem.Projectile;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    
    public class CurvedProjectileSkill : AbstractEnemySkill, IProjectileSkill
    {
        public PoolManagerSO PoolManager => poolManager;
        public PoolItemSO PoolItem => poolItem;

        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO poolItem;
        [SerializeField] private float travelDuration;
        [SerializeField] private float curveOffset;
        [SerializeField] private float health;
        
        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            Vector3 start = transform.position;
            Vector3 end = target.transform.position;
            Vector3 mid = (start + end) * 0.5f;
            Vector3 lineDir = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(lineDir, Vector3.up).normalized;
            Vector3 controlPoint = mid + perpendicular * curveOffset;

            CurvedProjectile projectile = poolManager.Pop<CurvedProjectile>(PoolItem);
            DamageData template = new DamageData(Owner, Vector3.zero, Vector3.zero, lineDir, Data.Damage, 0f);
                projectile.Launch(start, controlPoint, end, travelDuration, health, template, poolManager);
            yield break;
        }

    }
}