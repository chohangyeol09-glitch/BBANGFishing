using System;
using DevLib.ModuleSystem;
using NKT.Player.Modules;
using UnityEngine;

namespace NKT.Player
{
    public class Player : ModuleOwner
    {
        public LookModule Look { get; private set; }
        
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
        }

        private void Update()
        {
            Look.LookUpdate();
        }
    }
}