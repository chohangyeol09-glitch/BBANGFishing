using System;
using System.Collections;
using CHG._02.Script.CombatSystem.Projectile;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    [Serializable]
    public struct StraightShot
    {
        public float AimAngle; 
        public float Delay;    
    }

    public class StraightProjectileSkill : AbstractEnemySkill, IProjectileSkill
    {
        public PoolManagerSO PoolManager => poolManager;
        public PoolItemSO PoolItem => poolItem;

        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO poolItem;

        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float health;

        [SerializeField] private StraightShot[] shots = new StraightShot[1]; 

        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            foreach (StraightShot shot in shots)
            {
                if (shot.Delay > 0f) yield return new WaitForSeconds(shot.Delay);
                if (target == null) yield break;

                Vector3 origin = transform.position;
                Vector3 baseDir = (target.transform.position - origin).normalized;
                Vector3 dir = Quaternion.AngleAxis(shot.AimAngle, Vector3.up) * baseDir;

                StraightProjectile projectile = poolManager.Pop<StraightProjectile>(PoolItem);
                DamageData data = new DamageData(Owner, Vector3.zero, Vector3.zero, dir, SkillDamage, 0f);
                projectile.Launch(origin, dir, speed, lifeTime, health, data, poolManager);
            }
        }
    }
}
