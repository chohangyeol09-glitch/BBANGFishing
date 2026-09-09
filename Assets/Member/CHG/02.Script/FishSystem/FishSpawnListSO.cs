using System;
using System.Collections.Generic;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    [CreateAssetMenu(fileName = "Fish spawn list", menuName = "CHG/Fish/Fish spawn list", order = 0)]
    public class FishSpawnListSO : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public Fish Prefab;
            [Min(0f)] public float Weight; //같은 확률 안에서 가중치
        }
        
        [SerializeField] private Entry[] entries;
        private Dictionary<Grade, List<Entry>> _fishDict;


        private void OnEnable() => UpdateDict();
        private void OnValidate() => UpdateDict();

        private void UpdateDict()
        {
            _fishDict = new Dictionary<Grade, List<Entry>>();
            if (entries == null) return;
            foreach (var e in entries)
            {
                if (e.Prefab == null || e.Prefab.Data == null) continue;
                Grade grade = e.Prefab.Data.Grade;
                if (!_fishDict.TryGetValue(grade, out List<Entry> list))
                    _fishDict[grade] = list = new List<Entry>();
                list.Add(e);
            }
            
        }
        
        public bool TryGetFish(Grade grade, out List<Entry> fishs)
            => _fishDict.TryGetValue(grade, out fishs) && fishs.Count > 0;

    }
}