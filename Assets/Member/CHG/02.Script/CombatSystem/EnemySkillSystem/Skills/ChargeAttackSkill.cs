using System.Collections;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.EnemySkillSystem.Skills
{
    public class ChargeAttackSkill : AbstractEnemySkill
    {
        [Header("Strike")]
        [Tooltip("발동하고 데미지가 들어가기까지의 시간 (이펙트가 대상에 닿는 시간에 맞춘다)")]
        [SerializeField] private float impactDelay = 0.6f;
        [SerializeField] private float knockbackPower;

        [Header("Target Effect")]
        [Tooltip("발동 순간 대상 위치에서 재생 (없으면 생략)")]
        [SerializeField] private ParticleSystem impactEffect;
        [Tooltip("데미지 순간 대상 위치에서 재생 (없으면 생략)")]
        [SerializeField] private ParticleSystem hitEffect;
        [Tooltip("대상 위치 기준 오프셋 (월드). 대상의 기준점이 발밑이면 y를 올린다")]
        [SerializeField] private Vector3 targetOffset;
        [Tooltip("켜면 대상 이펙트가 보스 → 대상 방향(이펙트의 +Z)을 향한다")]
        [SerializeField] private bool faceFromOwner = true;

        protected override IEnumerator ExecuteSkill(GameObject target)
        {
            float fireTime = Time.time;
            Debug.Log($"[ChargeAttack] 차지 끝 (WarningTime {Data.WarningTime}초) / Time {fireTime:F2}");

            if (target == null) yield break;

            PlayAtTarget(impactEffect, target);
            yield return new WaitForSeconds(impactDelay);
            StopImpactEffect(); //반복 재생 이펙트여도 명중 순간 끝낸다
            if (target == null) yield break;

            PlayAtTarget(hitEffect, target);

            var damageable = target.GetComponentInParent<IDamageable>();
            if (damageable == null) yield break;

            Vector3 dir = (target.transform.position - Owner.transform.position).normalized;
            damageable.TakeDamage(new DamageData(Owner, target.transform.position, -dir, dir, SkillDamage, knockbackPower));
            Debug.Log($"[ChargeAttack] 데미지 {SkillDamage} → {target.name} / 발동 후 {Time.time - fireTime:F2}초 (impactDelay {impactDelay})");
        }

        //발동 뒤 그로기·사망으로 끊기면 발동 이펙트를 멈춘다 (코루틴이 멈춰서 데미지는 들어가지 않는다)
        protected override void OnStopped() => StopImpactEffect();

        private void StopImpactEffect()
        {
            if (impactEffect != null) impactEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void PlayAtTarget(ParticleSystem effect, GameObject target)
        {
            if (effect == null) return;

            Vector3 position = target.transform.position + targetOffset;
            effect.transform.position = position;

            Vector3 dir = position - Owner.transform.position;
            if (faceFromOwner && dir.sqrMagnitude > 0.0001f)
                effect.transform.rotation = Quaternion.LookRotation(dir.normalized);

            effect.Play(true);
        }
    }
}
