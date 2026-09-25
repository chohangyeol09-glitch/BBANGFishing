using System;
using System.Collections;
using CHG._02.Script.FishSystem;
using NKT.Fishing.Bait;
using UnityEngine;

namespace NKT.Fishing.Rob
{
    public class Bobber : MonoBehaviour
    {
        [SerializeField] private ParticleSystem bobberParticle;
        [SerializeField] private Transform restPoint;
        
        public event Action OnLanded;
        public event Action OnBite;
        
        private bool _isAttached = true;

        private void Awake()
        {
            bobberParticle.Pause();
            restPoint = transform.parent;
        }

        public void PositionInit()
        {
            StopAllCoroutines();
            _isAttached = true;
            transform.SetParent(restPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void PlayBite()
        {
            OnBite?.Invoke();
        }
        
        public void Launch(CastAim aim, float duration)
        {
            bobberParticle.Play();
            _isAttached = false;
            transform.SetParent(null, true);
            StartCoroutine(FlyRoutine(aim, duration));
        }
        public void Return(float duration, float arcHeight)
        {
            bobberParticle.Stop();
            StopAllCoroutines();
            StartCoroutine(ReturnRoutine(duration, arcHeight));
        }

        private void LateUpdate()
        {
            if(!_isAttached || restPoint == null) return;
            
            transform.position = restPoint.position;
        }

        private IEnumerator ReturnRoutine(float duration, float arcHeight)
        {
            Vector3 from = transform.position;
            float t = 0f;

            while (t < 1f)
            {
                t = Mathf.Min(t + Time.deltaTime / duration, 1f);

                CastAim aim = new CastAim
                {
                    origin = from,
                    landPoint = restPoint.position,
                    arcHeight = arcHeight,
                };

                transform.position = CastArc.Evaluate(aim, t);
                yield return null;
            }
            
            _isAttached = true;
        }

        private IEnumerator FlyRoutine(CastAim aim, float duration)
        {
            float t = 0f;

            while (t < 1f)
            {
                t = Mathf.Min(t + Time.deltaTime / duration, 1f);

                transform.position = CastArc.Evaluate(aim, t);
                yield return null;
            }

            transform.rotation = Quaternion.identity;
            OnLanded?.Invoke();
        }
    }
}
