using System;
using DevLib.AnimatorSystem;
using DevLib.ModuleSystem;
using NKT.Agent;
using NKT.Player.Modules;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKT.Player
{
    public class Player : ModuleOwner
    {
        public LookModule Look { get; private set; }
        public IRenderer Renderer { get; private set; }
        
        [SerializeField] private HashDataSO idle;
        [SerializeField] private HashDataSO cast;
        
        protected override void Awake()
        {
            base.Awake();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            
            Look = GetModule<LookModule>();
            Renderer = GetModule<IRenderer>();
        }

        private void Update()
        {
            Look.LookUpdate();
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Renderer.PlayClip(idle.HashValue, 0, 0.3f, 1);
            }
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                Renderer.PlayClip(cast.HashValue, 0, 0.2f, 1);
            }
        }
    }
}