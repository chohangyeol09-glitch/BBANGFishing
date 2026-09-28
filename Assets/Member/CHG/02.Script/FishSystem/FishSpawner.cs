using System.Collections.Generic;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using CHG._02.Script.CoreSystem;
using DevLib.ObjectPool.Runtime;
using NKT.Player.Modules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG._02.Script.FishSystem
{
    public class FishSpawner : MonoBehaviour
    {
        [SerializeField] private FishSpawnListSO fishSpawnList;
        [Tooltip("찌가 없을 때 쓰는 스폰 위치")]
        [SerializeField] private Transform spawnPoint;
        [Tooltip("연결하면 현재 장착된 낚싯대의 찌 위치에서 스폰한다")]
        [SerializeField] private RobEquipModule robEquip;
        [Tooltip("스폰 위치에 더하는 월드 기준 오프셋 (찌보다 위에서 나오게 하려면 Y를 올린다)")]
        [SerializeField] private Vector3 spawnOffset;
        [SerializeField] private bool allowDowngrade;
        [SerializeField] private GameObject attackTarget;
        [SerializeField] private PoolManagerSO poolManager;
        
        
        public Fish TrySpawnFish(Grade grade, Vector3 pullForce)
        {
            if (!TryPickFish(grade, out Fish prefab))
            {
                Debug.LogError($"Failed to pick Fish : {fishSpawnList.name}");
                return null;
            }

            return SpawnFish(prefab, pullForce);
        }

        private bool TryPickFish(Grade grade, out Fish prefab)
        {
            prefab = null;

            List<FishSpawnListSO.Entry> fishs;
            while (!fishSpawnList.TryGetFish(grade, out fishs))
            {
                if (!allowDowngrade || grade == Grade.Common) return false;
                grade--;
            }

            float total = 0f;
            foreach (FishSpawnListSO.Entry e in fishs) total += e.Weight;

            if (total <= 0f)
            {
                prefab = fishs[Random.Range(0, fishs.Count)].Prefab;
                return true;
            }

            float r = Random.value * total;

            foreach (FishSpawnListSO.Entry e in fishs)
            {
                if (r < e.Weight)
                {
                    prefab = e.Prefab;
                    return true;
                }
                
                r -= e.Weight;
            }

            prefab = fishs[^1].Prefab;
            return true;
        }

        private Fish SpawnFish(Fish prefab, Vector3 pullForce)
        {
            if (prefab.PoolItem == null)
            {
                Debug.LogError($"pool Item is null : {prefab.name}");
                return null;
            }
            
            Fish fish = poolManager.Pop<Fish>(prefab.PoolItem);
            if (fish == null)
            {
                Debug.LogError($"this fish is not pool manager registration : {prefab.name}");
                return null;
            }
            
            fish.transform.SetPositionAndRotation(GetSpawnPosition(), Quaternion.identity);
            fish.OnSpawn(pullForce, attackTarget, poolManager);
            return fish;
        }

        // 찌가 있으면 찌 위치, 없으면 spawnPoint
        private Vector3 GetSpawnPosition()
        {
            Transform point = robEquip != null && robEquip.CurrentBobber != null
                ? robEquip.CurrentBobber.transform
                : spawnPoint;

            return point.position + spawnOffset;
        }
        
    }
}