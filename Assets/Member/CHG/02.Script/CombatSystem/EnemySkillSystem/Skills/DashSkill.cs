using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class DashSkill : AbstractEnemySkill
    {
        [SerializeField] private float dashDuration;
        [SerializeField] private float dashDirection;

        [SerializeField] private float minSideAngle = 60f;
        [SerializeField] private float maxSideAngle = 120f;


        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            Vector3 toTarget = (target.transform.position - transform.position).normalized;
            
            
        }
    }
}