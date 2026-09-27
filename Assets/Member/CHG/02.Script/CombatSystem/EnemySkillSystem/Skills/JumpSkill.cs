using System.Collections;
using CHG._02.Script.Agents;
using CHG._02.Script.CoreSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class JumpSkill : AbstractEnemySkill
    {
        [SerializeField] private float jumpHeight = 2f; 

        private Rigidbody _rb;

        public override void InitSkill(Agent owner)
        {
            base.InitSkill(owner);
            _rb = owner.GetComponent<Rigidbody>();
        }

        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            if (_rb == null || _rb.isKinematic) yield break;

            float upSpeed = PhysicsUtil.SpeedForHeight(jumpHeight);
            _rb.AddForce(Vector3.up * (upSpeed - _rb.linearVelocity.y), ForceMode.VelocityChange);
        }
    }
}
