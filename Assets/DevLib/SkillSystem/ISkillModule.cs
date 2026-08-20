using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace DevLib.SkillSystem
{
    public interface ISkillModule
    {
        event Action OnSkillEnd;
        ModuleOwner Owner { get; }
        
        bool CanUseSkill(int skillId, GameObject target = null);
        void UseSkill(int skillId, GameObject target = null);
        
        float GetBaseDamage(SkillDataSo skillData);
    }
}