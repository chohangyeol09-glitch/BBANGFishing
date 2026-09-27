using UnityEngine;

namespace CHG._02.Script.MapSystem
{
    /// <summary>
    /// 맵 경계용 벽. 부딪힌 Rigidbody를 벽 반대쪽(맵 안쪽)으로 튕겨낸다.
    /// 콜라이더는 Box/Sphere/Capsule 또는 Convex Mesh여야 한다 (ClosestPoint를 쓰기 때문).
    /// Is Trigger를 끄면 물리적으로도 막고, 켜면 통과하려는 대상을 밀어내기만 한다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BounceWall : MonoBehaviour
    {
        [Tooltip("튕겨낼 대상 레이어 (물고기 레이어만 고르는 것을 권장)")]
        [SerializeField] private LayerMask targetLayers = ~0;
        [Tooltip("벽으로 들어오던 속도를 얼마나 되돌려 줄지. 0이면 멈추고, 1이면 같은 속도로 튕긴다")]
        [SerializeField, Range(0f, 2f)] private float bounciness = 1f;
        [Tooltip("벽에서 최소 이 속도로는 밀려난다 (느리게 닿아도 벽에 붙어 있지 않게)")]
        [SerializeField, Min(0f)] private float minPushSpeed = 3f;

        private Collider _collider;

        private void Awake() => _collider = GetComponent<Collider>();

        private void OnCollisionEnter(Collision collision) => Push(collision.rigidbody);
        private void OnCollisionStay(Collision collision) => Push(collision.rigidbody);
        private void OnTriggerEnter(Collider other) => Push(other.attachedRigidbody);
        private void OnTriggerStay(Collider other) => Push(other.attachedRigidbody);

        private void Push(Rigidbody rb)
        {
            //돌진처럼 kinematic으로 직접 움직이는 중에는 건드리지 않는다
            if (rb == null || rb.isKinematic) return;
            if ((targetLayers.value & (1 << rb.gameObject.layer)) == 0) return;

            Vector3 center = rb.worldCenterOfMass;
            Vector3 normal = center - _collider.ClosestPoint(center); //벽 → 대상 (맵 안쪽)
            if (normal.sqrMagnitude < 0.000001f) normal = center - _collider.bounds.center; //중심이 벽 안까지 들어온 경우
            if (normal.sqrMagnitude < 0.000001f) return;
            normal.Normalize();

            //벽으로 들어오던 속도는 반사하고, 밖으로 나가는 속도는 최소 minPushSpeed를 보장한다
            float outward = Vector3.Dot(rb.linearVelocity, normal);
            float wanted = Mathf.Max(outward < 0f ? -outward * bounciness : outward, minPushSpeed);
            if (wanted > outward)
                rb.AddForce(normal * (wanted - outward), ForceMode.VelocityChange);
        }
    }
}
