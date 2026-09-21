using System;
using System.Collections;
using UnityEngine;

namespace NKT.Fishing.Rob
{
    public class Bobber : MonoBehaviour
    {
        public event Action OnLanded;
        public event Action OnBite; //후에 물고기 SO 있으면 그거 받기
        
        [SerializeField] private ParticleSystem bobberParticle;

        private Vector3 _initPosition;

        private void Awake()
        {
            _initPosition = transform.position; 
        }

        public void PositionInit()
        {
            bobberParticle.Pause();
            transform.position = _initPosition;
            bobberParticle.Play();
        }
        
        public void Launch(CastAim aim, float duration)
        {
            StartCoroutine(FlyRoutine(aim, duration));
        }
        public void Return(float duration, float arcHeight)
        {
            StopAllCoroutines();
            StartCoroutine(ReturnRoutine(duration, arcHeight));
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
                    landPoint = _initPosition,
                    arcHeight = arcHeight,
                };

                transform.position = CastArc.Evaluate(aim, t);
                yield return null;
            }
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

            OnLanded?.Invoke();
        }
    }
}
