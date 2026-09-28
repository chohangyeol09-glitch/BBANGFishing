using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.UI
{
    // 씬 전환 때 화면을 덮었다 걷어내는 페이드 오버레이. 씬이 바뀌어도 안 죽는 DontDestroyOnLoad
    // 싱글톤이라, 맨 처음 씬에 하나만 놔두면 이후 씬 전환에서 계속 살아서 쓰인다.
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance { get; private set; }

        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Color fadeColor = Color.black;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildOverlay();
        }

        private void BuildOverlay()
        {
            var canvasGo = new GameObject("FadeCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            var imageGo = new GameObject("FadeImage", typeof(RectTransform), typeof(Image));
            imageGo.transform.SetParent(canvasGo.transform, false);
            var rect = (RectTransform)imageGo.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            imageGo.GetComponent<Image>().color = fadeColor;

            _canvasGroup = canvasGo.GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void FadeToScene(string sceneName)
        {
            StartCoroutine(FadeToSceneRoutine(sceneName));
        }

        private IEnumerator FadeToSceneRoutine(string sceneName)
        {
            yield return Fade(0f, 1f);

            SceneManager.LoadScene(sceneName);

            yield return Fade(1f, 0f);
        }

        private IEnumerator Fade(float from, float to)
        {
            _canvasGroup.blocksRaycasts = true;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = to;
            _canvasGroup.blocksRaycasts = to > 0f;
        }
    }
}
