namespace CHG._02.Script.UISystem
{
    /// <summary>
    /// UI에서 재생하는 소리의 종류. UISoundLibrarySO에서 각 값에 SoundClipSo를 연결한다.
    /// 새 소리는 끝에 추가할 것 — 중간에 끼우면 이미 저장된 에셋/프리팹의 enum 값이 밀린다.
    /// </summary>
    public enum UISoundType
    {
        None,

        // 버튼
        Hover,
        Click,

        // 상점
        BuySuccess,
        NotEnoughMoney,
        Sell,
        UpgradeSuccess,

        // 인벤토리
        InventoryOpen,
        InventoryClose,
        SlotSelect,
        HotbarChange,

        // 메뉴
        MenuOpen,
        MenuClose,
    }
}
