using CHG._02.Script.FishSystem;
using UnityEngine;

namespace CHG._02.Script.MapSystem
{
    [RequireComponent(typeof(Collider))]
    public class BounceWall : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField, Range(0f, 2f)] private float bounciness = 1f;
        [SerializeField, Min(0f)] private float minPushSpeed = 3f;

        private Collider _collider;

        private void Awake() => _collider = GetComponent<Collider>();

        private void OnCollisionEnter(Collision collision) => Push(collision.rigidbody);
        private void OnCollisionStay(Collision collision) => Push(collision.rigidbody);
        private void OnTriggerEnter(Collider other) => Push(other.attachedRigidbody);
        private void OnTriggerStay(Collider other) => Push(other.attachedRigidbody);

        private void Push(Rigidbody rb)
        {
            if (rb == null || rb.isKinematic) return;
            if ((targetLayers.value & (1 << rb.gameObject.layer)) == 0) return;
            if (!rb.gameObject.TryGetComponent<Fish>(out var fish)) return;
            if (fish.State == FishStateEnum.Lunge || fish.State == FishStateEnum.Return || fish.State == FishStateEnum.Dead) return;
            

            Vector3 center = rb.worldCenterOfMass;
            Vector3 normal = center - _collider.ClosestPoint(center);
            if (normal.sqrMagnitude < 0.000001f) normal = center - _collider.bounds.center;
            if (normal.sqrMagnitude < 0.000001f) return;
            normal.Normalize();

            float outward = Vector3.Dot(rb.linearVelocity, normal);
            float wanted = Mathf.Max(outward < 0f ? -outward * bounciness : outward, minPushSpeed);
            if (wanted > outward)
                rb.AddForce(normal * (wanted - outward), ForceMode.VelocityChange);
        }
    }
}
