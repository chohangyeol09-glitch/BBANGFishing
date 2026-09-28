using DevLib.ObjectPool.Runtime;
using DevLib.SoundSystem;
using UnityEngine;

namespace BBANGFishing.Audio
{
    public enum GameplaySound
    {
        DropCollected, FishHit, FishSplash, Good, Miss, Perfect, BobberSplash, Parry, PlayerDeath
    }

    public sealed class GameplaySoundLibrary : ScriptableObject
    {
        public SoundManager managerPrefab;
        public PoolManagerSO soundPool;
        public SoundClipSo[] clips;
    }

    // Playback is delegated to the project's pooled SoundManager and mixer.
    public static class GameplayAudio
    {
        private static GameplaySoundLibrary _library;
        private static SoundManager _manager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _library = null;
            _manager = null;
        }

        public static void Play(GameplaySound sound, Vector3 position)
        {
            if (_library == null)
                _library = Resources.Load<GameplaySoundLibrary>("GameplayAudio/Library");
            if (_library == null)
            {
                Debug.LogError("Gameplay audio library is missing.");
                return;
            }

            if (_manager == null)
            {
                _manager = Object.FindFirstObjectByType<SoundManager>();
                if (_manager == null)
                {
                    var poolRoot = new GameObject("Gameplay Sound Pool");
                    Object.DontDestroyOnLoad(poolRoot);
                    _library.soundPool.InitializePool(poolRoot.transform);
                    _manager = Object.Instantiate(_library.managerPrefab);
                }
            }

            SoundClipSo clip = _library.clips[(int)sound];
            _manager.SoundChannel.RaiseEvent(new PlaySoundEvent().Init(position, clip));
        }
    }
}
