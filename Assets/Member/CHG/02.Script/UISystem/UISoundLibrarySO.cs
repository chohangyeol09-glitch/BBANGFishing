using System;
using System.Collections.Generic;
using DevLib.SoundSystem;
using UnityEngine;

namespace CHG._02.Script.UISystem
{
    /// <summary>
    /// UI 소리 목록. UISoundType → SoundClipSo 매핑과 희귀도별 팡파레를 한곳에 모아 둔다.
    /// </summary>
    [CreateAssetMenu(fileName = "UISoundLibrary", menuName = "Lib/Sound/UI Sound Library", order = 1)]
    public class UISoundLibrarySO : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public UISoundType type;
            public SoundClipSo clip;
        }

        [SerializeField] private Entry[] entries;

        [Tooltip("FishRarity 순서(Common, Rare, Epic, Legendary)대로 넣는다")]
        [SerializeField] private SoundClipSo[] rarityFanfares = new SoundClipSo[4];

        private Dictionary<UISoundType, SoundClipSo> _clipDict;

        public bool TryGetClip(UISoundType type, out SoundClipSo clip)
        {
            clip = null;
            if (type == UISoundType.None) return false;

            if (_clipDict == null) BuildDict();
            return _clipDict.TryGetValue(type, out clip) && clip != null;
        }

        public bool TryGetRarityClip(FishRarity rarity, out SoundClipSo clip)
        {
            int index = (int)rarity;
            clip = rarityFanfares != null && index >= 0 && index < rarityFanfares.Length
                ? rarityFanfares[index]
                : null;
            return clip != null;
        }

        private void BuildDict()
        {
            _clipDict = new Dictionary<UISoundType, SoundClipSo>();
            if (entries == null) return;

            foreach (Entry entry in entries)
            {
                if (!_clipDict.TryAdd(entry.type, entry.clip))
                    Debug.LogWarning($"[UISoundLibrary] {entry.type} 중복 — 먼저 있는 항목을 사용 : {name}");
            }
        }

        // 인스펙터에서 목록을 고치면 다음 조회 때 다시 만든다
        private void OnValidate() => _clipDict = null;
    }
}
