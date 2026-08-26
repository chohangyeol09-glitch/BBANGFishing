using Member.CHG._02.Script.Agents;
using Member.CHG._02.Script.FIsh.FishSystem;
using UnityEngine;

namespace Member.CHG._02.Script.FIshs.FishSystem
{
    public class Fish : Agent
    {

        public bool IsJumping = false;
        
        [SerializeField] private FishDataSO _fishData;
        
        private Rigidbody _rb;
        

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _rb = GetComponent<Rigidbody>();
            
        }
        

        public void OnCatch(float power)
        {
            IsJumping = true;
            
            Jump(power, Vector3.up);
        }

        private void Jump(float power, Vector3 dir)
        {
            _rb.AddForce(dir * power / _fishData.Weight, ForceMode.Impulse);
        }

        public void OnHit(float power, Vector3 hitDir)
        {
            Jump(power, hitDir);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Sea"))
            {
                IsJumping = false;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("TestHit")]
        private void TestHit()
        {
            
        }
#endif
    }
}
