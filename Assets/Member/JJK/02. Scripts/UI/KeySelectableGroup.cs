using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.JJK._02._Scripts.UI
{
    // 1번/2번 같은 키를 누르면 그 슬롯의 selectImage만 켜고 나머지는 끈다.
    public class KeySelectableGroup : MonoBehaviour
    {
        [Serializable]
        private class Entry
        {
            public Key key = Key.Digit1;
            public SelectableSlotUI slot;
        }

        [SerializeField] private List<Entry> entries = new();
        [SerializeField] private int defaultIndex;

        public event Action<int> Selected;

        public int SelectedIndex { get; private set; } = -1;

        private void Start()
        {
            if (defaultIndex >= 0 && defaultIndex < entries.Count)
                Select(defaultIndex);
        }

        private void Update()
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (Keyboard.current[entries[i].key].wasPressedThisFrame)
                {
                    Select(i);
                    break;
                }
            }
        }

        public void Select(int index)
        {
            if (index < 0 || index >= entries.Count) return;
            if (index == SelectedIndex) return;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].slot != null)
                    entries[i].slot.SetSelected(i == index);
            }

            SelectedIndex = index;
            Selected?.Invoke(index);
        }
    }
}
