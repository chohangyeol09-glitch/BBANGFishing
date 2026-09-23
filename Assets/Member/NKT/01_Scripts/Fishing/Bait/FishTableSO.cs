using System.Collections.Generic;
using CHG._02.Script;
using CHG._02.Script.FishSystem;
using UnityEngine;

namespace NKT.Fishing.Bait
{
    [CreateAssetMenu(fileName = "FishTable", menuName = "KT/Fishing/FishTableSO", order = 0)]
    public class FishTableSO : ScriptableObject
    {
        public FishDataSO[] fishes;

        public FishDataSO Pick(BaitSO bait)
        {
            if (bait == null) return fishes.Length > 0 ? fishes[0] : null;

            if (bait.bossFishData != null)
                return bait.bossFishData;

            Grade grade = PickGrade(bait);
            
            List<FishDataSO> candidates =  new List<FishDataSO>();
            foreach (FishDataSO f in fishes)
            {
                if(f.Grade == grade) candidates.Add(f);
            }
            
            if(candidates.Count == 0) return fishes.Length > 0 ? fishes[0] : null;
            
            return candidates[Random.Range(0, candidates.Count)];
        }

        private Grade PickGrade(BaitSO bait)
        {
            float total = 0f;
            foreach (var w in bait.weights)
            {
                total += w.weight;
            }
            
            float roll = Random.Range(0f, total);
            foreach (var w in bait.weights)
            {
                roll -= w.weight;
                if(roll <= 0f) return w.grade;
            }

            return Grade.Common;
        }
    }
}