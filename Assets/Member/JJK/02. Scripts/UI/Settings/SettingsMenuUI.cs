using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.Settings
{
    public class SettingsMenuUI : MonoBehaviour
    {
        [Serializable]
        public class Tab
        {
            public Button tabButton;
            public GameObject panel;
        }

        [SerializeField] private List<Tab> tabs = new();
        [SerializeField] private int defaultTabIndex;
        [SerializeField] private GameObject menuPannel;

        public void AddTab(Button tabButton, GameObject panel) => tabs.Add(new Tab { tabButton = tabButton, panel = panel });

        public void SetDefaultTab(int index) => defaultTabIndex = index;

        // 다른 스크립트(MouseLook 등)가 설정창이 열려있는 동안 자기 입력을 끄고 싶을 때 참조한다.
        public static bool IsOpen { get; private set; }

        // 타이틀 화면의 "설정하기" 버튼처럼, 외부에서 이 패널을 열고 닫을 때 쓰는 진입점.
        public void Open() => menuPannel.SetActive(true);
        public void Close() => menuPannel.SetActive(false);

        private bool _initialized;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            for (int i = 0; i < tabs.Count; i++)
            {
                int index = i;
                tabs[i].tabButton.onClick.AddListener(() => SelectTab(index));
            }

            SelectTab(defaultTabIndex);

            // 시작할 때는 커서를 건드리지 않는다. 타이틀 화면처럼 마우스가 원래 보여야 하는 씬도 있고,
            // 게임 플레이 씬이면 MouseLook이 이미 자기 몫으로 잠가둔다 — 여기서 강제로 잠가버리면
            // (닫힌 상태 = 잠금으로 가정) 타이틀 화면에서 커서가 사라지고 버튼도 못 누르게 된다.
            _wasPanelActive = menuPannel.activeSelf;
            IsOpen = _wasPanelActive;
        }

        private bool _wasPanelActive;
        private CursorLockMode _cursorLockBeforeOpen;
        private bool _cursorVisibleBeforeOpen;

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                menuPannel.SetActive(!menuPannel.activeSelf);

            // Escape뿐 아니라 닫기/취소/적용 버튼으로 닫아도 여기서 한 번에 잡아서
            // 커서/타임스케일/IsOpen을 전부 맞춘다. 버튼 쪽에 따로 안 걸어도 된다.
            bool isActive = menuPannel.activeSelf;
            if (isActive == _wasPanelActive) return;

            _wasPanelActive = isActive;
            IsOpen = isActive;
            Time.timeScale = isActive ? 0f : 1f;

            if (isActive)
            {
                // 열기 직전의 커서 상태를 기억해뒀다가, 닫을 때 그대로 되돌린다.
                // (타이틀 화면처럼 원래 안 잠겨있었으면 닫아도 안 잠긴 채로, 게임 플레이 중이었으면
                // MouseLook이 걸어둔 잠금 상태로 되돌아온다 — "닫힘 = 무조건 잠금"으로 단정하지 않는다.)
                _cursorLockBeforeOpen = Cursor.lockState;
                _cursorVisibleBeforeOpen = Cursor.visible;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = _cursorLockBeforeOpen;
                Cursor.visible = _cursorVisibleBeforeOpen;

                // PlayerPrefs.SetFloat/SetInt는 디스크에 바로 안 쓰이고 메모리에만 남는다.
                // 에디터에서 Stop을 누르는 건 정상 종료가 아니라서 Save()를 명시적으로 안 부르면
                // 슬라이더로 바꾼 값이 다음 실행 때 사라진 것처럼 보인다.
                PlayerPrefs.Save();
            }
        }

        private void OnApplicationQuit() => PlayerPrefs.Save();

        private void SelectTab(int selectedIndex)
        {
            for (int i = 0; i < tabs.Count; i++)
                tabs[i].panel.SetActive(i == selectedIndex);
        }
    }
}
