using System;
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
        private IDamageable _damageable;
        private Vignette _vignette;
        private float _baseIntensity;
        
        public void Initialize(ModuleOwner owner)
        {
            _damageable = owner as IDamageable;
            
            Debug.Assert(volume != null, "volume == null");
            if (!volume.profile.TryGet<Vignette>(out _vignette))
            {
                Debug.Log("Vignette not found");
            }
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
            StartCoroutine(HitReaction());
        }

        private IEnumerator HitReaction()
        {
            float t = 0f;

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