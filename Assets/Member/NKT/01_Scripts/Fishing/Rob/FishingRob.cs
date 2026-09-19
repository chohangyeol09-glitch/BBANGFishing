using UnityEngine;

namespace NKT.Fishing.Rob
{
    //낚시대
    public class FishingRob : MonoBehaviour
    {
        [SerializeField] private FishingRobSO data;
        [SerializeField] private Transform bobberTransform;

        public FishingRobSO Data => data;
        public Transform BobberTransform => bobberTransform;
    }
}
