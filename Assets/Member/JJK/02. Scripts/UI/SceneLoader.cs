using UnityEngine;
using UnityEngine.SceneManagement;

namespace Member.JJK._02._Scripts.UI
{
    // TitleUI의 On Start Animation Completed 같은 UnityEvent에 바로 걸어 쓰는 씬 전환용 컴포넌트.
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void LoadScene() => SceneManager.LoadScene(sceneName);

        public void LoadScene(string overrideSceneName) => SceneManager.LoadScene(overrideSceneName);
    }
}
