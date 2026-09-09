using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace NKT.Player.Modules
{
    public class LookModule : MonoBehaviour, IModule
    {
        [SerializeField] private GameObject playerBody;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private float pitchXLimit = 90f;
        [SerializeField] private float pitchUpLimit = 90f;
        [SerializeField] private float pitchDownLimit = 45f;

        private Transform _cameraTransform;
        public Transform CameraTransform => _cameraTransform;
        public float Pitch => _pitch;
        
        private Vector2 _lookDelta;
        private float _pitch;
        private float _yaw;

        public void Initialize(ModuleOwner owner)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("카메라 어디갔어");
                return;
            }

            _cameraTransform = mainCamera.transform;
        }
        
        public void OnLookChange(Vector2 obj)
        {
            _lookDelta = obj;
        }

        public void LookUpdate()
        {
            HandleScreenRotate();
            HandleBodyRotate();
        }
        private void HandleBodyRotate()
        {
            
        }

        private void HandleScreenRotate()
        {
            _yaw += _lookDelta.x * sensitivity;
            _yaw = Mathf.Clamp(_yaw, -pitchXLimit, pitchXLimit);
            _pitch = Mathf.Clamp(_pitch - _lookDelta.y * sensitivity, -pitchUpLimit, pitchDownLimit);
            
            _cameraTransform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);

            _lookDelta = Vector2.zero;
        }
    }
}