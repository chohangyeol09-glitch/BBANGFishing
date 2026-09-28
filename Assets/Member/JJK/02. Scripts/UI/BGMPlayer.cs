using System.Collections;
using UnityEngine;

namespace Member.JJK._02._Scripts.UI
{
    // BGM용 AudioSource가 붙은 오브젝트에 같이 붙여서 씬이 넘어가도 안 끊기게 만든다.
    // SceneFader처럼 DontDestroyOnLoad 싱글톤이라, 다음 씬에 같은 컴포넌트가 또 있으면
    // (실수로 두 번 배치한 경우) 새로 들어온 쪽을 지워서 소리가 겹치지 않게 한다.
    // 씬마다 다른 곡을 틀고 싶으면 각 씬에 SceneBGM을 하나씩 놓고 PlayTrack을 통해 바꾼다.
    [RequireComponent(typeof(AudioSource))]
    public class BGMPlayer : MonoBehaviour
    {
        public static BGMPlayer Instance { get; private set; }

        private AudioSource _audioSource;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = true;

            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }

        // 지금 재생 중인 곡과 다르면, 볼륨을 줄였다가 곡을 바꾸고 다시 올리는 식으로 끊김 없이 전환한다.
        public void PlayTrack(AudioClip clip, float fadeDuration = 1f)
        {
            if (clip == null || _audioSource.clip == clip) return;

            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(CrossfadeRoutine(clip, fadeDuration));
        }

        private IEnumerator CrossfadeRoutine(AudioClip clip, float fadeDuration)
        {
            float targetVolume = _audioSource.volume;

            yield return Fade(_audioSource.volume, 0f, fadeDuration);

            _audioSource.clip = clip;
            _audioSource.Play();

            yield return Fade(0f, targetVolume, fadeDuration);

            _fadeRoutine = null;
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _audioSource.volume = Mathf.Lerp(from, to, duration > 0f ? elapsed / duration : 1f);
                yield return null;
            }

            _audioSource.volume = to;
        }
    }
}
