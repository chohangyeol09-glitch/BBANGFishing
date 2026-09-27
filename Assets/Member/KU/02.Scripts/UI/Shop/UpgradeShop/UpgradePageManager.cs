using UnityEngine;

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


        // 돈 확인
        if (!MoneyManager.Instance
            .CanAfford(cost))
        {
            Debug.Log(
                "업그레이드할 돈이 부족합니다."
            );

            return;
        }


        bool success =
            stat.Upgrade();


        if (!success)
            return;


        // 업그레이드 성공 후 돈 차감
        MoneyManager.Instance
            .TrySpendMoney(cost);


        ApplyUpgrade(
            stat
        );


        if (itemUI != null)
        {
            itemUI.Refresh();
        }


        Debug.Log(
            $"{stat.upgradeName} 업그레이드 완료 / " +
            $"Lv.{stat.currentLevel} / " +
            $"비용 {cost}원"
        );
    }


    private void ApplyUpgrade(
        UpgradeStatData stat)
    {
        switch (stat.statType)
        {
            case UpgradeStatType.AttackDamage:

                Debug.Log(
                    $"공격력 = {stat.CurrentValue}"
                );

                break;


            case UpgradeStatType.AttackSpeed:

                Debug.Log(
                    $"공격 속도 = {stat.CurrentValue}"
                );

                break;


            case UpgradeStatType.Health:

                Debug.Log(
                    $"체력 = {stat.CurrentValue}"
                );

                break;


            case UpgradeStatType.FishCapacity:

                Debug.Log(
                    $"물고기 용량 = " +
                    $"{stat.CurrentValue}"
                );

                break;
        }
    }
}