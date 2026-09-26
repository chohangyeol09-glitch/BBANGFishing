using System;
using System.Collections;
using CHG._02.Script.CombatSystem.Projectile;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{

    [Serializable]
    public struct CurvedShot
    {
        public float CurveAngle;
        public float AimAngle;
        public float Delay;
    }
    public class CurvedProjectileSkill : AbstractEnemySkill, IProjectileSkill
    {
        public PoolManagerSO PoolManager => poolManager;
        public PoolItemSO PoolItem => poolItem;

        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO poolItem;
        [SerializeField] private float travelDuration;
        [SerializeField] private float curveOffset;
        [SerializeField] private CurvedShot[] shots = new CurvedShot[1];
        [SerializeField] private float health;
        
        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            foreach (CurvedShot shot in shots)
            {
                if (shot.Delay > 0f) 
                    yield return new WaitForSeconds(shot.Delay);
                if (target == null) yield break;

                Vector3 start = transform.position;
                Vector3 toTarget = target.transform.position - start;
                Vector3 end = start + Quaternion.AngleAxis(shot.AimAngle, Vector3.up) * toTarget;

                Vector3 mid = (start + end) * 0.5f;
                Vector3 lineDir = (end - start).normalized;
                
                Vector3 side = Vector3.Cross(lineDir, Vector3.up);
                if (side.sqrMagnitude < 0.001f) side = Vector3.right;
                side = Quaternion.AngleAxis(shot.CurveAngle, lineDir) * side.normalized;

                Vector3 controlPoint = mid + side * curveOffset;
                
                CurvedProjectile projectile = poolManager.Pop<CurvedProjectile>(poolItem);
                DamageData data = new DamageData(Owner, Vector3.zero, Vector3.zero, lineDir, SkillDamage, 0f);
                projectile.Launch(start, controlPoint, end, travelDuration, health, data, poolManager);
            }
        }

    }
}