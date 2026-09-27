using CHG._02.Script.CombatSystem;
using NKT.Agent;
using NKT.Player.Modules;
using Unity.Cinemachine;
using UnityEngine;

namespace NKT.Player
{
    public class Player : CHG._02.Script.Agents.Agent
    {
        [SerializeField] private CinemachineBrain brain;
        public LookModule Look { get; private set; }
        public IRenderer Renderer { get; private set; }
        
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
            brain.ManualUpdate();
        }
    }
}