using System;
using System.Collections.Generic;
using System.Linq;
using CHG._02.Script.Agents;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public class EnemySkillModule : MonoBehaviour, IModule
    {
        public event Action OnSkillEnd;
        
        public AbstractEnemySkill CurrentSkill { get; private set; } 

        [SerializeField] private float baseSkillCoolTime;

        private Agent _owner;
        private Dictionary<int, AbstractEnemySkill> _skillDict; // Key: 스킬 id, Value: 스킬 자체
        private bool _usingSkill; 
        private float _lastActionCoolTime = float.NegativeInfinity;
        private float _currentModuleCoolTime;
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
            _skillDict = GetComponentsInChildren<AbstractEnemySkill>()
                .ToDictionary(k => k.Data.SkillIdHash.HashValue, v => v);
            foreach (var skill in _skillDict.Values) skill.InitSkill(_owner);
        }

        public bool CanUseSkill(int skillId, GameObject target = null) 
        {
            if (_usingSkill || //스킬을 사용중이거나
                !_skillDict.TryGetValue(skillId, out var skill)) return false; //스킬이 없다면 실패

            bool moduleCoolTimeReady = skill.Data.ignoreModuleCoolTime //쿨타임을 무시하는가
            || Time.time - _lastActionCoolTime >= _currentModuleCoolTime; //쿨타임이 돌았는가
            if (!moduleCoolTimeReady) return false;
            return skill.CanUseSkill(target);
        }

        public bool UseSkill(int skillId, GameObject target = null)
        {
            if (!CanUseSkill(skillId, target)) return false;

            CurrentSkill = _skillDict[skillId];
            CurrentSkill.OnSkillEnd += HandleSkillEnd;
            CurrentSkill.UseSkill(target);
            _usingSkill = true;
            return true;
        }

        public float GetNormalizedCooldown(int skillId)
        {
            if (_skillDict != null && _skillDict.TryGetValue(skillId, out var skill))
                return skill.NormalizedCooldown;
            return 0f;
        }

        private void HandleSkillEnd(AbstractEnemySkill skill)
        {
            skill.OnSkillEnd -= HandleSkillEnd;

            _currentModuleCoolTime = baseSkillCoolTime + skill.Data.moduleCoolTimeOffset;
            _lastActionCoolTime = Time.time;
            _usingSkill = false;
            CurrentSkill = null;
            OnSkillEnd?.Invoke();
        }
            
        
    }
}