using CHG._02.Script.CombatSystem;
using NKT.Agent;
using NKT.Player.Modules;
using Unity.Cinemachine;
using UnityEngine;

namespace NKT.Player
{
    public class Player : CHG._02.Script.Agents.Agent
    {
        [SerializeField]
        private CinemachineBrain brain;


        [Header("Input")]
        [SerializeField]
        private PlayerInputSO playerInput;


        public LookModule Look { get; private set; }

        public IRenderer Renderer { get; private set; }


        // 화면 회전 가능 여부
        private bool canLook = true;


        // 여러 곳에서 동시에 Lock을 걸어도
        // 하나가 풀렸다고 바로 카메라가 풀리지 않도록
        private int lookLockCount = 0;


        public bool CanLook =>
            canLook;



        private bool _deathSoundPlayed;

        public override void Dead()
        {
            if (!_deathSoundPlayed)
            {
                _deathSoundPlayed = true;
                BBANGFishing.Audio.GameplayAudio.Play(BBANGFishing.Audio.GameplaySound.PlayerDeath, transform.position);
            }
            base.Dead();
        }

        protected override void Awake()
        {
            base.Awake();


            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible =
                false;
        }



        protected override void InitializeModules()
        {
            base.InitializeModules();


            Look =
                GetModule<LookModule>();


            Renderer =
                GetModule<IRenderer>();
        }



        private void Update()
        {
            if (canLook)
            {
                Look.LookUpdate();
            }


            brain.ManualUpdate();
        }



        // =========================================
        // 화면 회전만 설정
        // =========================================

        public void SetLookEnabled(
            bool value)
        {
            canLook = value;
        }



        // =========================================
        // UI 열 때 호출
        //
        // 화면 회전 정지
        // 플레이어 입력 정지
        // 마우스 커서 활성화
        // =========================================

        public void LockLook()
        {
            lookLockCount++;


            canLook = false;


            if (playerInput != null)
            {
                playerInput.PushLock();
            }
        }



        // =========================================
        // UI 닫을 때 호출
        //
        // 화면 회전 활성화
        // 플레이어 입력 활성화
        // 마우스 다시 잠금
        // =========================================

        public void UnlockLook()
        {
            lookLockCount =
                Mathf.Max(
                    0,
                    lookLockCount - 1
                );


            if (playerInput != null)
            {
                playerInput.PopLock();
            }


            if (lookLockCount == 0)
            {
                canLook = true;
            }
        }
    }
}