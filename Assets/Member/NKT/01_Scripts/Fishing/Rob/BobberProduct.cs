using System;
using System.Collections;
using UnityEngine;

namespace NKT.Fishing.Rob
{
    public class BobberProduct : MonoBehaviour
    {
        [SerializeField] private Bobber bobber;
        [SerializeField] private ParticleSystem biteParticle;
        [SerializeField] private int bobCount = 3;
        [SerializeField] private float bobDepth = 0.25f;
        [SerializeField] private float bobDuration = 0.3f;

        private void Awake()
        {
            bobber.OnBite += HandleBiteRoutine;
        }

        private void OnDestroy()
        {
            bobber.OnBite -= HandleBiteRoutine;
        }

        private void HandleBiteRoutine()
        {
            StartCoroutine(BiteCoroutine());
        }

        private IEnumerator BiteCoroutine()
        {
            Vector3 basePos = transform.position;
            biteParticle.transform.position = basePos;

            for (int i = 0; i < bobCount; i++)
            {
                if (biteParticle != null)
                    biteParticle.Play();

                float t = 0f;
                while (t < 1f)
                {
                    t = Mathf.Min(t + Time.deltaTime / bobDuration, 1f);

                    float offset = -Mathf.Sin(t * Mathf.PI) * bobDepth;

                    transform.position = basePos + Vector3.up * offset;
                    yield return null;
                }
            }

            transform.position = basePos;
        }
    }
}