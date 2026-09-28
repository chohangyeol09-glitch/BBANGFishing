using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    /// <summary>
    /// 물고기가 죽고 나온 드롭(FishDrop)이 날아갈 목적지. 씬에 하나만 둔다.
    /// 고정 지점(양동이 등)이면 그 자리에, 플레이어에게 날아가야 하면 플레이어(또는 그 자식)에 붙인다.
    /// </summary>
    public class DropDestination : MonoBehaviour
    {
        public static Transform Current { get; private set; }

        private void OnEnable()
        {
            if (Current != null && Current != transform)
                Debug.LogWarning($"DropDestination이 여러 개입니다. '{name}'을(를) 목적지로 씁니다.", this);
            Current = transform;
        }

        private void OnDisable()
        {
            if (Current == transform) Current = null;
        }
    }
}
