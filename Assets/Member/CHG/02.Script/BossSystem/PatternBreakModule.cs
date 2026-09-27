using System;
using CHG._02.Script.CombatSystem;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.BossSystem
{
    public class PatternBreakModule : Module, IAfterInitModule
    {
        public event Action<float> OnBreakProgress;
        public event Action OnPatternBroken;

        private Boss _boss;
        private EnemySkillModule _skillModule;
        private GroggyModule _groggyModule;

        private BreakableSkillEntry _current;
        private float _damage;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            _boss = _owner as Boss;
            _skillModule = owner.GetModule<EnemySkillModule>();
            _groggyModule = owner.GetModule<GroggyModule>();
        }

        public void AfterInit()
        {
            if (_boss == null || _skillModule == null) return;

            _boss.OnDamaged += HandleDamaged;
            _skillModule.OnSkillEnd += HandleSkillEnd;
        }

        private void OnDestroy()
        {
            if (_boss == null)
                _boss.OnDamaged += HandleDamaged;
            if (_skillModule != null)
                _skillModule.OnSkillEnd += HandleSkillEnd;
        }

        private void HandleDamaged(DamageData data)
        {
            AbstractEnemySkill skill = _skillModule.CurrentSkill;
            if (skill == null || _boss.IsDead || _boss.State != BossStateEnum.Combat) return;

            if (_current == null)
            {
                _current = FindEntry(skill.Data.SkillIdHash.HashValue);
                _damage = 0f;
                if (_current == null) return;
            }

            _damage += data.Damage;
            OnBreakProgress?.Invoke(Mathf.Clamp01(_damage / _current.BreakDamage));
            if (_damage < _current.BreakDamage) return;

            float groggyAmount = _current.GroggyAmount;
            skill.StopSkill();
            OnPatternBroken?.Invoke();
            if (_groggyModule != null)
                _groggyModule.AddGroggy(groggyAmount);
        }

        private void HandleSkillEnd()
        {
            _current = null;
            _damage = 0f;
            OnBreakProgress?.Invoke(0f);
        }

        private BreakableSkillEntry FindEntry(int skillId)
        {
            BreakableSkillEntry[] entries = _boss.Data.BreakableSkills;
            if (entries == null) return null;

            foreach (BreakableSkillEntry entry in entries)
            {
                if (entry.Skill != null && entry.Skill.HashValue == skillId)
                    return entry;
            }
            return null;
        }
    }
}