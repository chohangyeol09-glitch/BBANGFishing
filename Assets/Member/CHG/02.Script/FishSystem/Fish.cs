using CHG._02.Script.Agents;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    public class Fish : Agent
    {
        public bool IsJumping = false;
        
        [field:SerializeField] public FishDataSO Data { get; private set; }
        
        private Rigidbody _rb;
        public override float MaxHealth => Data.Health;


        protected override void InitializeModules()
        {
            base.InitializeModules();
            _rb = GetComponent<Rigidbody>();

        }

        public void OnSpawn()
        {
            _rb.mass = Data.Weight;
            _rb.angularVelocity = Vector3.zero;
            CurrentHealth = MaxHealth;
            
            IsJumping = true;
            _rb.AddForce(Vector3.up * Data.JumpPower, ForceMode.Impulse);
        }

        public override void Dead()
        {
            
        }
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Sea"))
            {
                if (IsJumping)
                {
                    IsJumping = false;
                }
            }
        }
        
    }
}
