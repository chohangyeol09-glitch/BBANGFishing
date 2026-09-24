using System.Collections;
using CHG._02.Script.Agents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class DashSkill : AbstractEnemySkill
    {
        [SerializeField] private float dashDuration;
        [SerializeField] private float dashDistance;

        [SerializeField] private float minSideAngle = 60f;
        [SerializeField] private float maxSideAngle = 120f;

        private Rigidbody _rb;

        public override void InitSkill(Agent owner)
        {
            base.InitSkill(owner);
            _rb = owner.GetComponent<Rigidbody>();
        }

        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            Vector3 forward = Vector3.ProjectOnPlane(Owner.transform.forward, Vector3.up).normalized;

            float angle = Random.Range(minSideAngle, maxSideAngle);
            if (Random.value < 0.5f) angle = -angle;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;

            float speed = dashDistance / dashDuration;
            SetHorizontalVelocity(dir * speed);

            yield return new WaitForSeconds(dashDuration);

            SetHorizontalVelocity(Vector3.zero); 
        }

        private void SetHorizontalVelocity(Vector3 target)
        {
            Vector3 v = _rb.linearVelocity;
            Vector3 horizontal = new Vector3(v.x, 0f, v.z);
            _rb.AddForce(target - horizontal, ForceMode.VelocityChange);
        }
    }
}
