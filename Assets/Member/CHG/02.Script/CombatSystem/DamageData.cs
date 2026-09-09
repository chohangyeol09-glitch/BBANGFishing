using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem
{
    public struct DamageData
    {
        public ModuleOwner Attacker;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public Vector3 HitDirection;

        public float Damage;
        public float KnockbackPower;

        public DamageData(ModuleOwner attacker, Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection, 
            float damage, float knockbackPower)
        {
            Attacker = attacker;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
            HitDirection = hitDirection;
            Damage = damage;
            KnockbackPower = knockbackPower;
        }
    }
}