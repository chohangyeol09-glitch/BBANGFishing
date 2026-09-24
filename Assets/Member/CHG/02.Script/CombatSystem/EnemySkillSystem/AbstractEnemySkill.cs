using System;
using System.Collections;
using CHG._02.Script.Agents;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem
{
    public abstract class AbstractEnemySkill : MonoBehaviour
    {
        public event Action<AbstractEnemySkill> OnSkillEnd;
        
        public Agent Owner { get; private set; }
        public bool IsUsing { get; private set; }
        [field: SerializeField] public EnemySkillDataSO Data { get; private set; }
        public float NormalizedCooldown
        {
            get
            {
                if (Data == null || Data.skillCoolTime <= 0) return 0f;
                return Mathf.Clamp01(1f - (Time.time - _lastUsedTime) / Data.skillCoolTime);
            }
        }
            
        private float _lastUsedTime = float.NegativeInfinity;
        private Coroutine _routine;

        public virtual void InitSkill(Agent owner)
        {
            Owner = owner;
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
            yield return new WaitForSeconds(Data.WarningTime); 
            yield return ExecuteSkill(target);                              
            CleanUpSkillData();
        }


        public void StopSkill()
        {
            if (_routine != null) StopCoroutine(_routine);
            CleanUpSkillData();
        }
        
        private void CleanUpSkillData()
        {
            _lastUsedTime = Time.time;
            IsUsing = false;
            OnSkillEnd?.Invoke(this);
        }
        
        protected abstract IEnumerator ExecuteSkill(GameObject target);
    }
}
