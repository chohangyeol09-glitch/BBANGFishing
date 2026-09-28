using System;
using DevLib.ModuleSystem;
using Member.JJK._02._Scripts.Weapon;
using NKT.Fishing.Rob;
using UnityEngine;

namespace NKT.Player.Modules
{
    //핫바. 선택된 슬롯에 든 것이 곧 손에 든 것이 된다
    public class HotbarModule : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private PlayerInputSO input;
        [SerializeField] private int slotCount = 3;

        //시작 장비. FishingRobSO 또는 WeaponSO를 끌어다 놓는다
        [SerializeField] private ScriptableObject[] startSlots;

        public event Action<int> OnSelectionChanged;        //핫바 UI가 구독한다
        public event Action<int, Sprite> OnSlotChanged;     //슬롯 그림이 바뀌었을 때

        public int Selected => _selected;
        public int SlotCount => slotCount;

        private ScriptableObject[] _slots;
        private int _selected;
        private bool _locked;

        private WeaponModeModule _mode;

        public void Initialize(ModuleOwner owner)
        {
            _mode = owner.GetModule<WeaponModeModule>();

            slotCount = Mathf.Max(1, slotCount);
            _slots = new ScriptableObject[slotCount];

            if (startSlots != null)
            {
                int count = Mathf.Min(startSlots.Length, slotCount);
                for (int i = 0; i < count; i++)
                    _slots[i] = startSlots[i];
            }
        }

        public void AfterInit()
        {
            input.OnHotbarScrolled += HandleScroll;
            input.OnHotbarSlotSelected += Select;
            input.OnInputLocked += HandleLocked;
            input.OnInputUnlocked += HandleUnlocked;

            RefreshView();
            ApplySlot();        //시작 슬롯을 손에 쥐어준다
        }

        //현재 상태를 통째로 다시 알린다. UI가 늦게 구독했을 때 쓴다
        public void RefreshView()
        {
            if (_slots == null) return;

            for (int i = 0; i < slotCount; i++)
                OnSlotChanged?.Invoke(i, SpriteOf(_slots[i]));

            OnSelectionChanged?.Invoke(_selected);
        }

        private void OnDestroy()
        {
            if (input == null) return;

            input.OnHotbarScrolled -= HandleScroll;
            input.OnHotbarSlotSelected -= Select;
            input.OnInputLocked -= HandleLocked;
            input.OnInputUnlocked -= HandleUnlocked;
        }

        //인벤토리가 슬롯 내용을 바꿀 때 부른다
        public void SetSlot(int index, ScriptableObject item)
        {
            if (!IsValid(index)) return;

            _slots[index] = item;
            OnSlotChanged?.Invoke(index, SpriteOf(item));

            if (index == _selected) ApplySlot();
        }

        public ScriptableObject GetSlot(int index)
            => IsValid(index) ? _slots[index] : null;

        public void Select(int index)
        {
            if (_locked || !IsValid(index) || index == _selected) return;

            _selected = index;
            ApplySlot();
            OnSelectionChanged?.Invoke(_selected);
        }

        private void HandleScroll(float delta)
        {
            if (_locked) return;

            //위로 굴리면 앞 슬롯, 아래로 굴리면 뒤 슬롯. 끝에서 반대편으로 넘어간다
            int step = delta > 0f ? -1 : 1;
            Select((_selected + step + slotCount) % slotCount);
        }

        //선택된 슬롯의 타입이 모드를 결정한다
        private void ApplySlot()
        {
            switch (_slots[_selected])
            {
                case FishingRobSO rod:
                    _mode.EquipRod(rod);
                    break;

                case WeaponSO gun:
                    //내구도는 인벤토리가 생기면 여기서 꺼내 넘긴다
                    _mode.EquipGun(gun, null);
                    break;

                default:
                    _mode.EquipNothing();
                    break;
            }
        }

        //슬롯에 띄울 그림. WeaponSO엔 아직 아이콘 필드가 없어서 총은 빈 칸으로 나온다
        private static Sprite SpriteOf(ScriptableObject item)
            => item switch
            {
                FishingRobSO rod => rod.rodSprite,
                _ => null,
            };

        private bool IsValid(int index) => index >= 0 && index < slotCount;

        private void HandleLocked() => _locked = true;
        private void HandleUnlocked() => _locked = false;
    }
}
