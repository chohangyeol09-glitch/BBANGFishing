using System;
using DevLib.ModuleSystem;
using NKT.Fishing.UI;
using UnityEngine;
using UnityEngine.Rendering.UI;

namespace NKT.Fishing.Rob
{
    //낚시대
    public class FishingRob : MonoBehaviour
    {
        public FishingRobSO Data { get; private set; }
    
        public void OnEquip(FishingRobSO data)
        {
            Data = data;
        }

        public void Cast(float power)
        {
        }
    }
}