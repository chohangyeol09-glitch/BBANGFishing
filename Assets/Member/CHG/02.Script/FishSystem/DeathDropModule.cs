using CHG._02.Script.Agents;
using DevLib.ModuleSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    /// <summary>
    /// 오너(물고기·보스)가 죽으면 그 자리에서 FishDrop을 꺼내 DropDestination으로 날려 보낸다.
    /// </summary>
    public class DeathDropModule : Module, IAfterInitModule
    {
        [SerializeField] private PoolManagerSO poolManager;
        [Tooltip("FishDrop 프리팹의 PoolItem (poolManager에 등록돼 있어야 한다)")]
        [SerializeField] private PoolItemSO dropItem;
        [Tooltip("목적지까지 걸리는 시간(초)")]
        [SerializeField, Min(0.01f)] private float flyDuration = 1.2f;
        [Tooltip("날아가는 동안 위로 솟는 높이. 0이면 직선")]
        [SerializeField] private float arcHeight = 1.5f;
        [Tooltip("나오는 위치를 오너 위치에서 얼마나 옮길지 (월드)")]
        [SerializeField] private Vector3 spawnOffset;

        private Agent _agent;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            _agent = owner as Agent;
        }

        public void AfterInit()
        {
            if (_agent != null) _agent.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            if (_agent != null) _agent.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            if (poolManager == null || dropItem == null) return;

            Transform destination = DropDestination.Current;
            if (destination == null)
            {
                Debug.LogWarning("DropDestination이 씬에 없어 드롭을 만들지 않습니다.", this);
                return;
            }

            FishDrop drop = poolManager.Pop<FishDrop>(dropItem);
            if (drop == null) return;
            FishDataSO fishData = _agent is Fish fish ? fish.Data : null;
            drop.Fly(_agent.transform.position + spawnOffset, destination, flyDuration, arcHeight, poolManager, fishData);
        }
    }
}
