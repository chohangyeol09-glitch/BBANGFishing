using System;
using System.Collections;
using UnityEngine;

namespace NKT.Fishing.Rob
{
    public class Bobber : MonoBehaviour
    {
        public event Action OnLanded;
        public event Action OnBite; //후에 물고기 SO 있으면 그거 받기

        public void Launch(Vector3 from, Vector3 to, float height, float duration)
        {
            StartCoroutine(FlyRoutine(from, to, height, duration));
        }

        private IEnumerator FlyRoutine(Vector3 from, Vector3 to, float height, float duration)
        {
            float t = 0f;

            while (t < 1f)
            {
                t = Mathf.Min(t + Time.deltaTime / duration, 1f);
                
                Vector3 pos = Vector3.Lerp(from, to, t);
                pos.y += height * 4f * t * (1f - t);
                
                transform.position = pos;
                yield return null;
            }
            transform.position = to;
            OnLanded?.Invoke();
        }
    }
}