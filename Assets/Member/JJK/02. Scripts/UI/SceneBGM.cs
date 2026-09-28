using UnityEngine;

namespace Member.JJK._02._Scripts.UI
{
    // 씬마다 하나씩 놓고 원하는 BGM 클립을 지정하면, 이 씬이 로드될 때 BGMPlayer가 그 곡으로 바뀐다.
    public class SceneBGM : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private float fadeDuration = 1f;

        private void Start()
        {
            if (BGMPlayer.Instance != null)
                BGMPlayer.Instance.PlayTrack(clip, fadeDuration);
        }
    }
}
