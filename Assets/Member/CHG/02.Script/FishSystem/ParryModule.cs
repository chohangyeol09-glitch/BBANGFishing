using System;
using CHG._02.Script.CombatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    /// <summary>
    /// 돌진(LungeModule)해 오는 물고기의 패링 판정과 결과 처리.
    /// 접근 중이고 대상과 FishDataSO.ParryRange 안일 때만 패링되며, 성공하면 데미지를 주고 바로 되돌려 보낸다.
    /// </summary>
    public class ParryModule : MonoBehaviour, IModule
    {
        public event Action OnParried;

        [Tooltip("패링 성공 시 데미지 = 최대 체력 × 이 값 (공격 데미지와 상관없이 고정)")]
        [SerializeField, Range(0f, 1f)] private float maxHealthRatio = 0.25f;

        //지금 패링을 받을 수 있는가 (접근 중 + 대상과 ParryRange 안)
        public bool IsParryable => _lunge != null && _lunge.IsApproaching && _lunge.Target != null &&
                                   Vector3.Distance(_fish.transform.position, _lunge.Target.transform.position) <=
                                   _fish.Data.ParryRange;

        private Fish _fish;
        private LungeModule _lunge;

        public void Initialize(ModuleOwner owner)
        {
            _fish = owner as Fish;
            _lunge = owner.GetModule<LungeModule>();
        }

        public bool TryParry(DamageData data)
        {
            if (!IsParryable || _fish.IsDead) return false;

            BBANGFishing.Audio.GameplayAudio.Play(BBANGFishing.Audio.GameplaySound.Parry, _fish.transform.position);
            data.Damage = _fish.MaxHealth * maxHealthRatio; //들어온 공격 데미지 대신 최대 체력 비율로 고정
            _fish.TakeParryDamage(data); //돌진 중 무적을 무시하는 패링 전용 경로
            OnParried?.Invoke();
            if (!_fish.IsDead) _lunge.ReturnNow(); //도착을 기다리지 않고 바로 되돌아간다
            return true;
        }
    }
}
