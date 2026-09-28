using UnityEngine;

namespace NKT.Fishing.Rob
{
    public class WeaponGrip : MonoBehaviour
    {
        [SerializeField] private Transform rightGrip;
        [SerializeField] private Transform leftGrip;
        
        public Transform RightGrip => rightGrip;
        public Transform LeftGrip => leftGrip;
    }
}