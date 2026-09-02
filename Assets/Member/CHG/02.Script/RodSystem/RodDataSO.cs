using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG._02.Script.RodSystem
{
    [CreateAssetMenu(fileName = "rod data", menuName = "CHG/Rod/Rod data", order = 0)]
    public class RodDataSO : ScriptableObject
    {
        [SerializeField] private float[] gradeWeight = new float[Enum.GetValues(typeof(Grade)).Length];

        private void OnValidate()
        {
            int count = Enum.GetValues(typeof(Grade)).Length;
            if (gradeWeight == null || gradeWeight.Length != count)
                Array.Resize(ref gradeWeight, count);
        }

        public Grade RollGrade()
        {
            float total = 0;
            for (int i = 0; i < gradeWeight.Length; i++) total += gradeWeight[i];

            if (total == 0f) return Grade.Common;
            
            float r = Random.value * total;
            for (int i = 0; i < gradeWeight.Length; i++)
            {
                if (r <gradeWeight[i]) return (Grade)i;
                r -= gradeWeight[i];    
            }
            
            return Grade.Common;
        }
    }
}