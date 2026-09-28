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
        [SerializeField] private Vector2 bobIntervalRange = new Vector2(0.15f, 0.3f);

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

            for (int i = 0; i < bobCount; i++)
            {
                PlaySplash(basePos);
                
                float t = 0f;
                while (t < 1f)
                {
                    t = Mathf.Min(t + Time.deltaTime / bobDuration, 1f);

                    float offset = -Mathf.Sin(t * Mathf.PI) * bobDepth;

                    transform.position = basePos + Vector3.up * offset;
                    
                    if (biteParticle != null)
                        biteParticle.transform.position = basePos;
                    
                    yield return null;
                }
                if (i < bobCount - 1)
                    yield return new WaitForSeconds(
                        Random.Range(bobIntervalRange.x, bobIntervalRange.y));
            }

            transform.position = basePos;
        }

        private void PlaySplash(Vector3 surfacePos)
        {
            if (biteParticle == null) return;
            
            biteParticle.transform.position = surfacePos;
            biteParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            biteParticle.Play(true);
            Debug.Log("Splash Played");
        }
    }
}