using CHG._02.Script.FishSystem;
using CHG._02.Script.RodSystem;
using UnityEngine;

namespace CHG._02.Script.Test
{
    public class TestRod : MonoBehaviour
    {
        [SerializeField] private RodDataSO data;
        [SerializeField] private FishSpawner spawner;
        
        [ContextMenu("Catch")]
        private void Catch()
        {
            Grade grade = data.RollGrade();
            Fish fish = spawner.TrySpawnFish(grade);
        }
    }
}