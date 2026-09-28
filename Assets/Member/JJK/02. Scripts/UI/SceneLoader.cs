using UnityEngine;
using UnityEngine.SceneManagement;

namespace Member.JJK._02._Scripts.UI
{
    // TitleUI의 On Start Animation Completed 같은 UnityEvent에 바로 걸어 쓰는 씬 전환용 컴포넌트.
    // SceneFader가 씬에 있으면 페이드로, 없으면 바로 전환한다.
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void LoadScene() => Load(sceneName);

        public void LoadScene(string overrideSceneName) => Load(overrideSceneName);

        private static void Load(string targetSceneName)
        {
            if (SceneFader.Instance != null)
                SceneFader.Instance.FadeToScene(targetSceneName);
            else
                SceneManager.LoadScene(targetSceneName);
        }
    }
}
