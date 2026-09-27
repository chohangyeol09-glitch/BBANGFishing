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
        public bool IsWarning { get; private set; } 

        [field: SerializeField] public EnemySkillDataSO Data { get; private set; }
        
        [SerializeField] private ParticleSystem warningEffect;
        [SerializeField] private ParticleSystem executeEffect;

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
                if (warningEffect != null) warningEffect.Play(true);
                IsWarning = true;
                yield return new WaitForSeconds(Data.WarningTime);
                IsWarning = false;
                StopEffect(warningEffect);
            }

            PlayAnim(Data.SkillAnimHash);
            if (executeEffect != null) executeEffect.Play(true); 
            yield return ExecuteSkill(target);
            CleanUpSkillData();
        }

        public void StopSkill()
        {
            if (_routine != null) StopCoroutine(_routine);
            IsWarning = false;
            OnStopped();
            StopEffect(warningEffect); 
            if (_renderer != null) _renderer.PlayIdle();
            CleanUpSkillData();
        }

        private static void StopEffect(ParticleSystem effect)
        {
            if (effect != null) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        private void PlayAnim(HashDataSO anim)
        {
            if (_renderer != null) _renderer.SendAnim(anim);
        }

        protected virtual void OnStopped() { }
        
        private void CleanUpSkillData()
        {
            _lastUsedTime = Time.time;
            IsUsing = false;
            OnSkillEnd?.Invoke(this);
        }

        protected abstract IEnumerator ExecuteSkill(GameObject target);
    }
}
