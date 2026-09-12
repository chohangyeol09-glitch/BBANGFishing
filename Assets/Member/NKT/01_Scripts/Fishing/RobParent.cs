using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Fishing
{
    public abstract class RobParent : MonoBehaviour
    {
        public FishingRobSO Data { get; private set; }
    
        public virtual void OnEquip(FishingRobSO data)
        {
            Data = data;
        }

        public abstract void OnPrimaryAction(float power);
    }
}