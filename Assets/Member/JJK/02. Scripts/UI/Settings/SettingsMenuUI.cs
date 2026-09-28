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

            _wasPanelActive = menuPannel.activeSelf;
            IsOpen = _wasPanelActive;
            ApplyCursorState(_wasPanelActive);
        }

        private bool _wasPanelActive;

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
            ApplyCursorState(isActive);
            Time.timeScale = isActive ? 0f : 1f;

            // PlayerPrefs.SetFloat/SetInt는 디스크에 바로 안 쓰이고 메모리에만 남는다.
            // 에디터에서 Stop을 누르는 건 정상 종료가 아니라서 Save()를 명시적으로 안 부르면
            // 슬라이더로 바꾼 값이 다음 실행 때 사라진 것처럼 보인다.
            if (!isActive) PlayerPrefs.Save();
        }

        private void OnApplicationQuit() => PlayerPrefs.Save();

        private static void ApplyCursorState(bool menuOpen)
        {
            Cursor.lockState = menuOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = menuOpen;
        }

        private void SelectTab(int selectedIndex)
        {
            for (int i = 0; i < tabs.Count; i++)
                tabs[i].panel.SetActive(i == selectedIndex);
        }
    }
}
