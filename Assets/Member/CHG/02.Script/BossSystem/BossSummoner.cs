using UnityEngine;

namespace CHG._02.Script.BossSystem
{
    /// <summary>
    /// 씬에 비활성으로 둔 보스를 소환한다. 켜져 있는 오브젝트에 붙이고, 게임 흐름에서 Summon()을 부른다.
    /// 보스는 씬에 놓인 자리(떠오른 뒤의 최종 위치)에 둔다.
    /// </summary>
    public class BossSummoner : MonoBehaviour
    {
        [SerializeField] private Boss boss;
        [Tooltip("보스가 공격할 대상 (플레이어)")]
        [SerializeField] private GameObject target;
        [Tooltip("켜면 씬 시작 시 바로 소환한다 (테스트용)")]
        [SerializeField] private bool summonOnStart;

        private void Start()
        {
            if (summonOnStart) Summon();
        }

        [ContextMenu("Summon")]
        public void Summon()
        {
            if (boss == null)
            {
                Debug.LogWarning("소환할 보스가 연결되지 않았습니다.", this);
                return;
            }
            boss.Summon(target);
        }
    }
}
