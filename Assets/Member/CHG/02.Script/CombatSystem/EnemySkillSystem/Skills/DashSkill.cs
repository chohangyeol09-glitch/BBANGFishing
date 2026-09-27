using System.Collections;
using CHG._02.Script.Agents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class DashSkill : AbstractEnemySkill
    {
        [Header("Dash")]
        [SerializeField] private float dashImpulse = 6f; //한 번에 가하는 충격량 
        [SerializeField] private float dashDuration = 0.5f; //이 시간 동안 수평 속도가 줄어들고, 끝나면 멈춤
        [SerializeField] private float deceleration = 6f; //수평 속도 감쇠 정도 

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
            Debug.Log("대쉬 사용");
            if (_rb == null || _rb.isKinematic) yield break;

            Vector3 forward = Vector3.ProjectOnPlane(Owner.transform.forward, Vector3.up).normalized;

            float angle = Random.Range(minSideAngle, maxSideAngle);
            if (Random.value < 0.5f) angle = -angle;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;

            _rb.AddForce(dir * dashImpulse, ForceMode.Impulse); //한 번에 튀어나감

            WaitForFixedUpdate wait = new WaitForFixedUpdate();
            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                yield return wait;
                elapsed += Time.fixedDeltaTime;
                if (_rb.isKinematic) yield break; //도중에 물리가 꺼지면 중단
                DampHorizontalVelocity(Mathf.Exp(-deceleration * Time.fixedDeltaTime));
            }

            DampHorizontalVelocity(0f);
        }

        protected override void OnStopped()
        {
            if (_rb == null || _rb.isKinematic) return;
            DampHorizontalVelocity(0f); //도중에 끊겨도 수평 속도가 남지 않게
        }

        //수평 속도만 변화
        private void DampHorizontalVelocity(float factor)
        {
            Vector3 v = _rb.linearVelocity;
            Vector3 horizontal = new Vector3(v.x, 0f, v.z);
            _rb.AddForce(-horizontal * (1f - factor), ForceMode.VelocityChange);
        }
    }
}
