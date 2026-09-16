using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.Projectile
{
    public class StraightProjectile : PoolableMono, IProjectile
    {
        private Vector3 _direction;
        private float _speed;
        private float _lifeTime;
        private float _elapsed;


        public Vector3 Direction => _direction;
        public float Speed => _speed;
        public float LifeTime => _lifeTime;
        public float Elapsed => _elapsed;
        public DamageData DamageData { get; private set; }
        public PoolManagerSO PoolManager { get; }

        public void Launch(Vector3 origin, Vector3 dir, float speed, float lifeTime, float elapsed, DamageData data,
            PoolManagerSO poolManager)
        {
            
        }
    }
}