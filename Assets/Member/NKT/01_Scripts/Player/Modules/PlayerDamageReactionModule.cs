using System.Collections;
using CHG._02.Script.CombatSystem;
using DevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NKT.Player.Modules
{
    public class PlayerDamageReactionModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private Volume volume;
        [SerializeField] private float maxIntensity = 0.5f;
        [SerializeField] private float fadeTime = 0.35f;
        [SerializeField] private float restIntensity = 0f;   //안 맞았을 때 비네트 세기
        private IDamageable _damageable;
        private Vignette _vignette;
        private float _baseIntensity;
        
        public void Initialize(ModuleOwner owner)
        {
            _damageable = owner as IDamageable;
            
            Debug.Assert(volume != null, "volume == null");
            if (!volume.profile.TryGet<Vignette>(out _vignette))
            {
                Debug.LogError("PlayerDamageReactionModule : 프로파일에 Vignette가 없습니다.", volume);
                return;
            }

            //프로파일에 뭐가 적혀 있든 평상시 세기는 여기서 정한다
            _baseIntensity = restIntensity;
            _vignette.intensity.overrideState = true;
            _vignette.intensity.value = _baseIntensity;
        }

        public void AfterInit()
        {
            _damageable.OnDamaged += OnDamaged;
        }

        private void OnDestroy()
        {
            _damageable.OnDamaged -= OnDamaged;
        }

        [ContextMenu("test")]
        private void Test()
        {
            DamageData damageData = new DamageData();
            OnDamaged(damageData);
        }
        private void OnDamaged(DamageData data)
        {
            if (_vignette == null) return;

            StopAllCoroutines();        //연속으로 맞으면 처음부터 다시
            StartCoroutine(HitReaction());
        }

        private IEnumerator HitReaction()
        {
            float t = 0f;
            Debug.Log("아야");

            while (t < 1f)
            {
                t = Mathf.Min(t + Time.deltaTime / fadeTime, 1f);
                _vignette.intensity.value = Mathf.Lerp(maxIntensity, _baseIntensity, t);

                yield return null;
            }

            _vignette.intensity.value = _baseIntensity;
        }
    }
}