using System;
using System.Collections.Generic;
using System.Linq;
using DevLib.AnimatorSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public enum SkillConditionType //스킬 사용 조건 
    {
        None,  
        HPPercent, //일정 체력 이하로 내려가면
    }

    [Serializable]
    public class SkillStrategyEntry
    {
        public HashDataSO SkillIdHash;
        public int Priority; //값이 높으면 우선
        public SkillConditionType ConditionType;
        public float ConditionValue; //체력이면 0~1, 
    }
    
    [CreateAssetMenu(fileName = "priority skill strategy", menuName = "Fish/Priority skill strategy", order = 0)]
    public class PrioritySkillStrategySO : ScriptableObject, ISkillStrategy
    {
        [SerializeField] public List<SkillStrategyEntry> skillEntries;
        
        //다음 사용할 스킬 선정
        //조건 있는 스킬 -> 우선도 높은거 -> 같은 우선도 내 랜덤
        public int? SelectNextSkillId(SkillStrategyContext context)
        {
            foreach (var group in skillEntries
                         .GroupBy(e => e.Priority).OrderByDescending(x => x.Key))
            {
                int? chosen = null;
                int count = 0;
                foreach (var entry in group)
                {
                    if (!IsCanUseCondition(entry, context)) continue;
                    
                    int skillId = entry.SkillIdHash.HashValue;
                    if (!context.SkillModule.CanUseSkill(skillId, context.Target)) continue;
                    
                    count++;
                    if (Random.Range(0, count) == 0) chosen = skillId;
                }
                
                if (chosen.HasValue) return chosen;
            }
            return null;
        }

        private bool IsCanUseCondition(SkillStrategyEntry entry, SkillStrategyContext context)
        {
            return entry.ConditionType switch
            {
                SkillConditionType.None => true,
                SkillConditionType.HPPercent => 
                    context.Owner.CurrentHealth / context.Owner.MaxHealth <= entry.ConditionValue,
                _ => false
            };
        }
    }
}