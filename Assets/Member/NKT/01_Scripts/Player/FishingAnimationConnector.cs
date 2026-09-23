using UnityEngine;
using UnityEngine.Events;

namespace NKT.Player
{
    public class FishingAnimationConnector : MonoBehaviour
    {
        public UnityEvent OnRetrieveEnd;

        public void OnRetrieveEndEvent() => OnRetrieveEnd?.Invoke();
    }
}