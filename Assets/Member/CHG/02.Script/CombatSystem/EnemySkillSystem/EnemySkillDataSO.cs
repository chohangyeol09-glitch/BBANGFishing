using DevLib.AnimatorSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public enum EnemySkillType
    {
        Damage, 
        Buff,
        Move
    }
    
    [CreateAssetMenu(fileName = "Enemy skill data", menuName = "Fish/Skill/Skill data", order = 0)]
    public class EnemySkillDataSO : ScriptableObject
    {
        public HashDataSO SkillIdHash;
        public HashDataSO SkillAnimHash;
        public EnemySkillType SkillType;
        public float Damage;
        public float KbForce; //넉백
        public float WarningTime; //이펙트, 모션 등 경고시간
        
        [Header("CoolTime")]
        public float skillCoolTime; //스킬 자체의 쿨타임
        public float moduleCoolTimeOffset; //모듈 쿨타임 보정값
        public bool ignoreModuleCoolTime; //모듈 쿨타임 무시
    }
}