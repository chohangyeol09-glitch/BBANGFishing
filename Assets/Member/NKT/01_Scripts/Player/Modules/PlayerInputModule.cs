using DevLib.ModuleSystem;
using UnityEngine;

namespace NKT.Player.Modules
{
    public class PlayerInputModule : MonoBehaviour, IAfterInitModule, IModule
    {
        [SerializeField] private PlayerInputSO control;
        private LookModule _lookModule;

        public void Initialize(ModuleOwner owner)
        {
            _lookModule = owner.GetModule<LookModule>();
        }

        public void AfterInit()
        {
            control.OnLookChange += _lookModule.OnLookChange;
        }

        private void OnDestroy()
        {
            if (control == null) return;

            control.OnLookChange -= _lookModule.OnLookChange;
        }
    }
}