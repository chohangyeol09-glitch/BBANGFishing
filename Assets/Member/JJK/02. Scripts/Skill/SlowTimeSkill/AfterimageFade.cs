using System.Collections;
using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public class AfterimageFade : MonoBehaviour
    {
        public void Init(Renderer targetRenderer, Color startColor, float lifetime, Mesh ownedMesh)
        {
            StartCoroutine(FadeRoutine(targetRenderer, startColor, lifetime, ownedMesh));
        }

        private IEnumerator FadeRoutine(Renderer targetRenderer, Color startColor, float lifetime, Mesh ownedMesh)
        {
            Material material = targetRenderer.material;
            float elapsed = 0f;

            while (elapsed < lifetime)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / lifetime);
                material.SetColor("_BaseColor", new Color(startColor.r, startColor.g, startColor.b, alpha));
                yield return null;
            }

            if (ownedMesh != null) Destroy(ownedMesh);
            Destroy(material);
            Destroy(gameObject);
        }
    }
}
