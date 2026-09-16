using System;
using UnityEngine;

namespace NKT.Fishing.Rob
{
    [CreateAssetMenu(fileName = "FishingRob SO", menuName = "KT/Fishing/Rob", order = 0)]
    public class FishingRobSO : ScriptableObject
    {
        public RobObject prefab;
        public float power = 1;
    }

    [Serializable]
    public struct RobObject
    {
        public GameObject RobGameobject;
        public GameObject BobberGameobject;
    }
}