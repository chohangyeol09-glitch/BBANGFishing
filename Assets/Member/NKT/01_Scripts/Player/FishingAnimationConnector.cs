using UnityEngine;
using UnityEngine.Events;

namespace NKT.Player
{
    public class FishingAnimationConnector : MonoBehaviour
    {
        public UnityEvent OnCastChange;

        public void OnAnimationEvent()
        {
            OnCastChange?.Invoke();
        }
    }
}