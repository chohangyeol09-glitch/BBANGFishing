using DevLib.ModuleSystem;
using UnityEngine;

namespace NKT.Player.Modules
{
    public class PlayerInputModule : MonoBehaviour, IAfterInitModule, IModule
    {
        [SerializeField] private PlayerInputSO control;
        private LookModule _lookModule;
        private RobEquipModule _equipModule;

        public void Initialize(ModuleOwner owner)
        {
            _lookModule = owner.GetModule<LookModule>();
            _equipModule = owner.GetModule<RobEquipModule>();
        }

        public void AfterInit()
        {
            control.OnLookChange += _lookModule.OnLookChange;
            control.OnAttackPressed += _equipModule.OnChargeStart;
            control.OnAttackReleased += _equipModule.OnChargeEnd;
        }

        private void OnDestroy()
        {
            if (control == null) return;

            control.OnLookChange -= _lookModule.OnLookChange;
            control.OnAttackPressed -= _equipModule.OnChargeStart;
            control.OnAttackReleased -= _equipModule.OnChargeEnd;
        }
    }
}