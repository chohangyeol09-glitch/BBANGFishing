using DevLib.ModuleSystem;
using UnityEngine;

namespace NKT.Player.Modules
{
    public class PlayerInputModule : MonoBehaviour, IAfterInitModule, IModule
    {
        [SerializeField] private PlayerInputSO control;
        private LookModule _lookModule;
        private FishingModule _fishingModule;

        public void Initialize(ModuleOwner owner)
        {
            _lookModule = owner.GetModule<LookModule>();
            _fishingModule = owner.GetModule<FishingModule>();
        }

        public void AfterInit()
        {
            control.OnLookChange += _lookModule.OnLookChange;
            control.OnAttackPressed += _fishingModule.OnAttackPressed;
            control.OnAttackReleased += _fishingModule.OnAttackReleased;
        }

        private void OnDestroy()
        {
            if (control == null) return;

            control.OnLookChange -= _lookModule.OnLookChange;
            control.OnAttackPressed -= _fishingModule.OnAttackPressed;
            control.OnAttackReleased -= _fishingModule.OnAttackReleased;
        }
    }
}