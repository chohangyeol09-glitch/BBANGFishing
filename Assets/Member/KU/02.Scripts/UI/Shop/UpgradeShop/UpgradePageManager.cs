using Member.JJK._02._Scripts.Weapon;
using UnityEngine;
using NKT.Fishing;

public class UpgradePageManager : MonoBehaviour
{
    [Header("카테고리 페이지")]
    [SerializeField]
    private GameObject gunPage;

    [SerializeField]
    private GameObject statPage;

    [SerializeField]
    private GameObject capacityPage;


    [Header("업그레이드 UI 부모")]
    [SerializeField]
    private Transform gunContent;

    [SerializeField]
    private Transform statContent;

    [SerializeField]
    private Transform capacityContent;


    [Header("업그레이드 UI 프리팹")]
    [SerializeField]
    private UpgradeStatItemUI upgradeItemPrefab;


    [Header("총기 업그레이드")]
    [SerializeField]
    private UpgradeStatData[] gunUpgrades;


    [Header("플레이어 스탯 업그레이드")]
    [SerializeField]
    private UpgradeStatData[] statUpgrades;


    [Header("용량 업그레이드")]
    [SerializeField]
    private UpgradeStatData[] capacityUpgrades;


    [Header("실제 적용 대상")]
    [SerializeField]
    private WeaponController weaponController;


    private bool isCreated = false;



    private void OnEnable()
    {
        if (!isCreated)
        {
            CreateUpgradeItems();

            isCreated = true;
        }


        OpenGunPage();
    }



    private void CreateUpgradeItems()
    {
        CreateItems(
            gunUpgrades,
            gunContent
        );


        CreateItems(
            statUpgrades,
            statContent
        );


        CreateItems(
            capacityUpgrades,
            capacityContent
        );
    }



    private void CreateItems(
        UpgradeStatData[] upgrades,
        Transform parent)
    {
        if (upgrades == null)
            return;


        if (parent == null)
            return;


        if (upgradeItemPrefab == null)
            return;


        foreach (UpgradeStatData upgrade in upgrades)
        {
            if (upgrade == null)
                continue;


            UpgradeStatItemUI item =
                Instantiate(
                    upgradeItemPrefab,
                    parent
                );


            item.Setup(
                upgrade,
                this
            );
        }
    }



    // =========================================
    // 페이지
    // =========================================

    public void OpenGunPage()
    {
        CloseAllPages();


        if (gunPage != null)
        {
            gunPage.SetActive(true);
        }
    }



    public void OpenStatPage()
    {
        CloseAllPages();


        if (statPage != null)
        {
            statPage.SetActive(true);
        }
    }



    public void OpenCapacityPage()
    {
        CloseAllPages();


        if (capacityPage != null)
        {
            capacityPage.SetActive(true);
        }
    }



    private void CloseAllPages()
    {
        if (gunPage != null)
        {
            gunPage.SetActive(false);
        }


        if (statPage != null)
        {
            statPage.SetActive(false);
        }


        if (capacityPage != null)
        {
            capacityPage.SetActive(false);
        }
    }



    // =========================================
    // 업그레이드
    // =========================================

    public void UpgradeStat(
        UpgradeStatData stat,
        UpgradeStatItemUI itemUI)
    {
        if (stat == null)
            return;


        if (stat.IsMaxLevel)
        {
            Debug.Log(
                $"{stat.upgradeName}은 최대 레벨입니다."
            );

            return;
        }


        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning(
                "MoneyManager가 없습니다."
            );

            return;
        }


        int cost =
            stat.CurrentCost;


        // =========================================
        // 돈 확인
        // =========================================

        if (!MoneyManager.Instance
            .CanAfford(cost))
        {
            Debug.Log(
                "업그레이드할 돈이 부족합니다."
            );

            return;
        }


        // =========================================
        // 업그레이드 전 값
        // =========================================

        float previousValue =
            stat.CurrentValue;


        // =========================================
        // 데이터 업그레이드
        // =========================================

        bool success =
            stat.Upgrade();


        if (!success)
            return;


        // =========================================
        // 이번 업그레이드 증가량
        // =========================================

        float upgradeAmount =
            stat.CurrentValue -
            previousValue;


        // =========================================
        // 돈 차감
        // =========================================

        MoneyManager.Instance
            .TrySpendMoney(cost);


        // =========================================
        // 실제 게임에 적용
        // =========================================

        ApplyUpgrade(
            stat,
            upgradeAmount
        );


        // =========================================
        // UI 갱신
        // =========================================

        if (itemUI != null)
        {
            itemUI.Refresh();
        }


        Debug.Log(
            $"{stat.upgradeName} 업그레이드 완료 / " +
            $"Lv.{stat.currentLevel} / " +
            $"증가량 {upgradeAmount} / " +
            $"비용 {cost}원"
        );
    }



    // =========================================
    // 실제 업그레이드 적용
    // =========================================

    private void ApplyUpgrade(
        UpgradeStatData stat,
        float upgradeAmount)
    {
        switch (stat.statType)
        {
            // =====================================
            // 공격력
            // =====================================

            case UpgradeStatType.AttackDamage:

                if (weaponController != null)
                {
                    weaponController.UpgradeDamage(
                        upgradeAmount
                    );
                }
                else
                {
                    Debug.LogWarning(
                        "WeaponController가 연결되어 있지 않습니다."
                    );
                }


                Debug.Log(
                    $"공격력 +{upgradeAmount}"
                );

                break;



            // =====================================
            // 공격 속도
            // =====================================

            case UpgradeStatType.AttackSpeed:

                if (weaponController != null)
                {
                    weaponController.UpgradeFireRate(
                        upgradeAmount
                    );
                }
                else
                {
                    Debug.LogWarning(
                        "WeaponController가 연결되어 있지 않습니다."
                    );
                }


                Debug.Log(
                    $"공격 속도 +{upgradeAmount}"
                );

                break;



            // =====================================
            // 체력
            // =====================================

            case UpgradeStatType.Health:

                Debug.Log(
                    $"체력 +{upgradeAmount}"
                );


                // 나중에 실제 체력 시스템 연결
                //
                // 예:
                //
                // health.UpgradeMaxHealth(
                //     upgradeAmount
                // );

                break;



            // =====================================
            // 물고기 인벤토리 용량
            // =====================================

            case UpgradeStatType.FishCapacity:

                if (FishInventoryManager.Instance == null)
                {
                    Debug.LogWarning(
                        "FishInventoryManager가 없습니다."
                    );

                    break;
                }


                // 용량 업그레이드 1회
                // =
                // 인벤토리 페이지 1개 추가
                FishInventoryManager.Instance
                    .UnlockNextPage();


                Debug.Log(
                    $"물고기 인벤토리 페이지 증가 / " +
                    $"현재 " +
                    $"{FishInventoryManager.Instance.UnlockedPageCount}페이지"
                );

                break;
        }
    }
}