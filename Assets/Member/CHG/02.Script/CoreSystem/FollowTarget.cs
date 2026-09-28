using UnityEngine;

namespace CHG._02.Script.CoreSystem
{
    /// <summary>
    /// target에 붙어 있는 것처럼 매 프레임 위치/회전을 따라간다. 부모-자식 관계를 바꾸지 않는다.
    /// keepInitialOffset을 켜면 시작 시점의 상대 위치/회전을 target 기준으로 유지한다.
    /// </summary>
    [DefaultExecutionOrder(1000)] // 다른 스크립트가 target을 움직인 뒤에 따라가도록
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [Header("따라갈 항목")]
        [SerializeField] private bool followPosition = true;
        [SerializeField] private bool followRotation = true;

        [Header("오프셋")]
        [Tooltip("켜면 시작할 때의 target 기준 상대 위치/회전을 그대로 유지한다 (아래 오프셋 값은 무시)")]
        [SerializeField] private bool keepInitialOffset = true;
        [Tooltip("target 로컬 기준 위치 오프셋")]
        [SerializeField] private Vector3 positionOffset;
        [Tooltip("target 기준 회전 오프셋")]
        [SerializeField] private Vector3 rotationOffset;

        private Vector3 _localPosOffset;
        private Quaternion _localRotOffset = Quaternion.identity;

        public Transform Target => target;

        private void Start()
        {
            SetTarget(target);
        }

        /// <summary>런타임에 따라갈 대상을 바꾼다. null이면 따라가지 않는다.</summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target == null) return;

            if (keepInitialOffset)
            {
                _localPosOffset = target.InverseTransformPoint(transform.position);
                _localRotOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
            }
            else
            {
                _localPosOffset = positionOffset;
                _localRotOffset = Quaternion.Euler(rotationOffset);
            }

            Snap();
        }

        // 애니메이션/물리 이동이 끝난 뒤 따라가야 떨림이 없다
        private void LateUpdate()
        {
            if (target == null) return;
            Snap();
        }

        private void Snap()
        {
            if (followPosition)
                transform.position = target.TransformPoint(_localPosOffset);
            if (followRotation)
                transform.rotation = target.rotation * _localRotOffset;
        }

#if UNITY_EDITOR
        // 플레이 중 인스펙터에서 오프셋을 고치면 바로 반영
        private void OnValidate()
        {
            if (Application.isPlaying && target != null && !keepInitialOffset)
            {
                _localPosOffset = positionOffset;
                _localRotOffset = Quaternion.Euler(rotationOffset);
            }
        }
#endif
    }
}
