using System;
using DevLib.ModuleSystem;
using NKT.Fishing.UI;
using UnityEngine;

namespace NKT.Fishing.Rob
{
    //낚시대
    public class FishingRob : MonoBehaviour
    {
        public FishingRobSO Data { get; private set; }
    
        public virtual void OnEquip(FishingRobSO data)
        {
            Data = data;
        }

        public void Cast(float power)
        {
            
        }
    }
}