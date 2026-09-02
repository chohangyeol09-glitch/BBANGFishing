using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKT.Player.Modules
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/Player", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public Action<Vector2> OnMovement;
        public Action<Vector2> OnLookChange;
        public Action OnAttackChange;
        public Action OnInteractChange;
      
        private Controls _control;
        private Camera _mainCam;

        public Camera MainCam
        {
            get
            {
                if(_mainCam == null)
                    _mainCam = Camera.main;
                return _mainCam;
            }
        }

        private void OnEnable()
        {
            if (_control == null)
            {
                _control = new Controls();
                _control.Player.SetCallbacks(this);
            }
            _control.Player.Enable();
        }
        
        private void OnDisable()
        {
            _control.Player.Disable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            OnMovement?.Invoke(movement);
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 look = context.ReadValue<Vector2>();
            OnLookChange?.Invoke(look);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnAttackChange?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnInteractChange?.Invoke();
        }
    }
}