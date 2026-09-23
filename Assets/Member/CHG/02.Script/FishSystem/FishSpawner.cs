using System.Collections.Generic;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using CHG._02.Script.CoreSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG._02.Script.FishSystem
{
    public class FishSpawner : MonoBehaviour
    {
        [SerializeField] private FishSpawnListSO fishSpawnList;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool allowDowngrade;
        [SerializeField] private GameObject attackTarget;
        
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
            Fish fish = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            fish.OnSpawn(pullForce, attackTarget);
            return fish;
        }
        
    }
}