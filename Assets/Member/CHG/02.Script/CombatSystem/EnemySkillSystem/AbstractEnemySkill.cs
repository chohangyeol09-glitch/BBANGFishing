using System;
using System.Collections;
using CHG._02.Script.Agents;
using DevLib.AnimatorSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public abstract class AbstractEnemySkill : MonoBehaviour
    {
        public event Action<AbstractEnemySkill> OnSkillEnd;
        
        public Agent Owner { get; private set; }
        public bool IsUsing { get; private set; }
        
        [field: SerializeField] public EnemySkillDataSO Data { get; private set; }

        private EnemyRenderer _renderer;
        
        public float NormalizedCooldown
        {
            get
            {
                if (Data == null || Data.skillCoolTime <= 0) return 0f;
                return Mathf.Clamp01(1f - (Time.time - _lastUsedTime) / Data.skillCoolTime);
            }
        }
        
        protected float SkillDamage => Data.Damage * (_damageMultiplier != null ? _damageMultiplier.DamageMultiplier : 1f);

        private IDamageMultiplier _damageMultiplier; 
        private float _lastUsedTime = float.NegativeInfinity;
        private Coroutine _routine;

        public virtual void InitSkill(Agent owner)
        {
            Owner = owner;
            _damageMultiplier = owner as IDamageMultiplier;
            _renderer = owner.GetModule<EnemyRenderer>();
        }
        
        public virtual bool CanUseSkill(GameObject target = null)
        {
            if (IsUsing) return false;
            return NormalizedCooldown <= 0f;
        }
        
        public void UseSkill(GameObject target)
        {
            IsUsing = true;
            _routine = StartCoroutine(SkillRoutine(target));
        }

        private IEnumerator SkillRoutine(GameObject target)
        {
            if (Data.WarningTime > 0f)
            {
                PlayAnim(Data.WarningAnimHash);
                yield return new WaitForSeconds(Data.WarningTime); 
            }

            PlayAnim(Data.SkillAnimHash);
            yield return ExecuteSkill(target);                              
            CleanUpSkillData();
        }

        private void PlayAnim(HashDataSO anim)
        {
            if (_renderer != null) _renderer.SendAnim(anim);
        }


        public void StopSkill()
        {
            if (_routine != null) StopCoroutine(_routine);
            OnStopped();
            CleanUpSkillData();
        }

        //StopSkill로 끊겼을 때 하위 스킬이 정리할 게 있으면 override
        protected virtual void OnStopped() { }
        
        private void CleanUpSkillData()
        {
            _lastUsedTime = Time.time;
            IsUsing = false;
            if (_renderer != null) _renderer.PlayIdle();
            OnSkillEnd?.Invoke(this);
        }

        protected abstract IEnumerator ExecuteSkill(GameObject target);
    }
}
