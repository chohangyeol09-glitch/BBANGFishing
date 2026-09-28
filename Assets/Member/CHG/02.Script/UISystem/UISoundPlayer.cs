using DevLib.EventChannelSystem;
using DevLib.SoundSystem;
using UnityEngine;

namespace CHG._02.Script.UISystem
{
    /// <summary>
    /// UI 소리 재생 창구. 어디서든 UISoundPlayer.Play(UISoundType.Click)처럼 호출하면
    /// 사운드 채널로 PlaySoundEvent를 보내고, 실제 재생은 SoundManager가 한다.
    /// 씬 루트에 하나 두면 DontDestroyOnLoad로 유지된다.
    /// </summary>
    public class UISoundPlayer : MonoBehaviour
    {
        public static UISoundPlayer Instance { get; private set; }

        [Tooltip("SoundManager.SoundChannel과 같은 에셋")]
        [SerializeField] private EventChannelSO soundChannel;
        [SerializeField] private UISoundLibrarySO library;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public static void Play(UISoundType type)
        {
            if (type == UISoundType.None) return;
            if (!TryGetInstance(out UISoundPlayer player)) return;

            if (player.library.TryGetClip(type, out SoundClipSo clip))
                player.Raise(clip);
            else
                Debug.LogWarning($"[UISoundPlayer] {type}에 연결된 클립 없음");
        }

        public static void PlayRarity(FishRarity rarity)
        {
            if (!TryGetInstance(out UISoundPlayer player)) return;

            if (player.library.TryGetRarityClip(rarity, out SoundClipSo clip))
                player.Raise(clip);
            else
                Debug.LogWarning($"[UISoundPlayer] {rarity} 팡파레 클립 없음");
        }

        private static bool TryGetInstance(out UISoundPlayer player)
        {
            player = Instance;
            if (player != null && player.library != null && player.soundChannel != null) return true;

            Debug.LogWarning("[UISoundPlayer] 씬에 UISoundPlayer가 없거나 library/soundChannel이 비어 있음");
            return false;
        }

        // UI 소리는 위치가 의미 없고 일회성이므로 채널 0으로 보낸다
        private void Raise(SoundClipSo clip)
        {
            soundChannel.RaiseEvent(SoundEvents.PlaySoundEvent.Init(Vector3.zero, clip));
        }
    }
}
