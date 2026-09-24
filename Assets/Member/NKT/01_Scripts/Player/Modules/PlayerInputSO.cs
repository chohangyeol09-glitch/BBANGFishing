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
        public Action OnAttackPressed;
        public Action OnAttackReleased;
        public Action OnInteractChange;
        public Action OnInputLocked;
      
        private Controls _control;
        private Camera _mainCam;
        private int _lockCount;

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
            _lockCount = 0;
            _control.UI.Disable();
            _control.Player.Enable();
        }
        
        private void OnDisable()
        {
            _control.Player.Disable();
            _control.UI.Disable();
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
            if (context.performed)
                OnAttackPressed?.Invoke();
            else if (context.canceled)
                OnAttackReleased?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnInteractChange?.Invoke();
        }

        public void PushLock()
        {
            _lockCount++;
            if (_lockCount > 1) return;
            
            OnInputLocked?.Invoke();
            _control.Player.Disable();
            _control.UI.Enable();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void PopLock()
        {
            _lockCount = Mathf.Max(0, _lockCount - 1);
            if (_lockCount > 0) return;
            
            _control.UI.Disable();
            _control.Player.Enable();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}