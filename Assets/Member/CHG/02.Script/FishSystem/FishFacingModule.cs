using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    public class FishFacingModule : MonoBehaviour, IModule
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Vector3 modelHeadDir = Vector3.left;
        [SerializeField] private Vector3 modelEulerOffset = new Vector3(-90f, 0f, 0f);
        [SerializeField] private float turnSpeed = 360f;
        [SerializeField] private float minSpeed = 0.5f;

        private Fish _fish;
        private Rigidbody _rb;
        private LungeModule _lunge;
        
        public void Initialize(ModuleOwner owner)
        {
            _fish = owner as Fish;
            _rb = owner.GetComponent<Rigidbody>();
            _lunge = owner.GetModule<LungeModule>();
        }

        private void LateUpdate()
        {
            if (_fish.IsDead) return;

            Vector3 velocity = _rb.isKinematic && _lunge != null ? _lunge.FlightVelocity : _rb.linearVelocity;
            if (velocity.sqrMagnitude < minSpeed * minSpeed) return;

            Quaternion target = FaceRotation(velocity.normalized);
            pivot.rotation = Quaternion.RotateTowards(pivot.rotation, target, turnSpeed * Time.deltaTime);
        }
        
        public void SnapTo(Vector3 direction) => pivot.rotation = FaceRotation(direction.normalized);

        private Quaternion FaceRotation(Vector3 dir)
        {
            Vector3 head = modelHeadDir.normalized;
            Quaternion look = Vector3.Dot(head, dir) < -0.999f ? Quaternion.AngleAxis(180f, Vector3.up) 
                    : Quaternion.FromToRotation(head, dir);
            return look * Quaternion.Euler(modelEulerOffset);
        }
        
    }
}