using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.Projectile
{
    public interface IProjectile
    {
        public Vector3 Direction { get; } 
        public float Speed { get; }
        public float LifeTime { get; }
        public float Elapsed { get; }
        public DamageData DamageData { get; }
        public PoolManagerSO PoolManager { get; }

        public void Launch(Vector3 origin, Vector3 dir, float speed, float lifeTime, float elapsed, DamageData data, PoolManagerSO poolManager);
    }
}