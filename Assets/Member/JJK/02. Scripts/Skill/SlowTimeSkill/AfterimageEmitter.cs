using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public class AfterimageEmitter : MonoBehaviour
    {
        private static readonly List<AfterimageEmitter> ActiveEmitters = new();

        [SerializeField] private Transform modelRoot;
        [SerializeField] private Color ghostColor = new Color(1f, 0.3f, 0.2f, 0.5f);
        [SerializeField] private float spawnInterval = 0.08f;
        [SerializeField] private float ghostLifetime = 0.4f;

        private Renderer[] _renderers;
        private Shader _ghostShader;
        private Coroutine _spawnRoutine;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _ghostShader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        private void OnEnable() => ActiveEmitters.Add(this);

        private void OnDisable()
        {
            StopEffect();
            ActiveEmitters.Remove(this);
        }

        public static void StartAll()
        {
            foreach (AfterimageEmitter emitter in ActiveEmitters)
                emitter.StartEffect();
        }

        public static void StopAll()
        {
            foreach (AfterimageEmitter emitter in ActiveEmitters)
                emitter.StopEffect();
        }

        public void StartEffect()
        {
            if (_spawnRoutine != null || _ghostShader == null) return;
            _spawnRoutine = StartCoroutine(SpawnLoop());
        }

        public void StopEffect()
        {
            if (_spawnRoutine == null) return;
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                SpawnGhost();
                yield return new WaitForSecondsRealtime(spawnInterval);
            }
        }

        private void SpawnGhost()
        {
            foreach (Renderer sourceRenderer in _renderers)
            {
                if (sourceRenderer == null || !sourceRenderer.enabled) continue;
                if (!TryExtractMesh(sourceRenderer, out Mesh mesh, out Mesh ownedMesh)) continue;

                var ghostGo = new GameObject("Afterimage");
                ghostGo.transform.SetPositionAndRotation(sourceRenderer.transform.position, sourceRenderer.transform.rotation);
                ghostGo.transform.localScale = sourceRenderer.transform.lossyScale;
                ghostGo.AddComponent<MeshFilter>().sharedMesh = mesh;

                var meshRenderer = ghostGo.AddComponent<MeshRenderer>();
                var material = new Material(_ghostShader);
                SetupTransparentBlend(material);
                material.SetColor("_BaseColor", ghostColor);
                meshRenderer.material = material;

                ghostGo.AddComponent<AfterimageFade>().Init(meshRenderer, ghostColor, ghostLifetime, ownedMesh);
            }
        }

        private static bool TryExtractMesh(Renderer sourceRenderer, out Mesh mesh, out Mesh ownedMesh)
        {
            switch (sourceRenderer)
            {
                case SkinnedMeshRenderer skinned:
                    mesh = new Mesh();
                    skinned.BakeMesh(mesh);
                    ownedMesh = mesh;
                    return true;
                case MeshRenderer when sourceRenderer.TryGetComponent(out MeshFilter meshFilter) && meshFilter.sharedMesh != null:
                    mesh = meshFilter.sharedMesh;
                    ownedMesh = null;
                    return true;
                default:
                    mesh = null;
                    ownedMesh = null;
                    return false;
            }
        }

        private static void SetupTransparentBlend(Material material)
        {
            material.SetFloat("_Surface", 1f);
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }
}
