using CHG._02.Script.CombatSystem.EnemySkillSystem;
using UnityEngine;

namespace CHG._02.Script.BossSystem
{
    [CreateAssetMenu(fileName = "Boss data", menuName = "Boss/Boss data", order = 0)]
    public class BossDataSO : ScriptableObject
    {
        public float Health;
        public SkillEntry[] Skills;
        public float DamageMultiplier = 1f;

        public float AppearDuration = 1f; //등장 연출 시간

        public float GroggyMaxGauge = 100f; //게이지가 이만큼 쌓이면 그로기
        public float GroggyDuration; 
        
        public BreakableSkillEntry[] BreakableSkills;

    }
}