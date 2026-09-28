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
        [Tooltip("지형과 겹치면 이 거리 안에서 위쪽의 빈 공간을 찾는다")]
        [SerializeField, Min(0.1f)] private float maxSpawnLift = 8f;
        [SerializeField, Min(0.01f)] private float spawnClearance = 0.05f;
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
            if (!TryClearSpawnPosition(fish))
            {
                Debug.LogWarning($"[FishSpawner] 물고기가 나올 빈 공간이 없습니다: {fish.name}", this);
                poolManager.Push(fish);
                return null;
            }
            fish.OnSpawn(pullForce, attackTarget, poolManager);
            return fish;
        }

        private bool TryClearSpawnPosition(Fish fish)
        {
            // 풀에서 이동한 직후이므로 물리 쿼리에 최신 위치를 반영한다.
            Physics.SyncTransforms();
            Collider[] colliders = fish.GetComponentsInChildren<Collider>();
            Vector3 origin = fish.transform.position;
            float limit = Mathf.Max(0.1f, maxSpawnLift);
            int steps = Mathf.CeilToInt(limit / 0.1f);
            for (int step = 0; step <= steps; step++)
            {
                Vector3 offset = Vector3.up * Mathf.Min(step * 0.1f, limit);
                bool blocked = false;
                foreach (Collider own in colliders)
                {
                    if (!own.enabled || own.isTrigger || !own.gameObject.activeInHierarchy) continue;
                    Bounds bounds = own.bounds;
                    // 어종마다 다른 크기를 반영하고 표면에 닿지 않도록 여유를 둔다.
                    Collider[] overlaps = Physics.OverlapBox(bounds.center + offset,
                        bounds.extents + Vector3.one * Mathf.Max(0.01f, spawnClearance),
                        Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
                    foreach (Collider other in overlaps)
                    {
                        if (other.transform.IsChildOf(fish.transform)) continue;
                        if (Physics.GetIgnoreLayerCollision(own.gameObject.layer, other.gameObject.layer)
                            || Physics.GetIgnoreCollision(own, other)) continue;
                        blocked = true;
                        break;
                    }
                    if (blocked) break;
                }

                if (blocked) continue;
                fish.transform.position = origin + offset;
                fish.GetComponent<Rigidbody>().position = fish.transform.position;
                Physics.SyncTransforms();
                return true;
            }
            return false;
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
