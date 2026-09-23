using CHG._02.Script.CoreSystem;
using CHG._02.Script.FishSystem;
using CHG._02.Script.RodSystem;
using UnityEngine;

namespace CHG._02.Script.Test
{
    public class TestRod : MonoBehaviour
    {
        [SerializeField] private RodDataSO data;
        [SerializeField] private FishSpawner spawner;
        [SerializeField] private float spreadAngle = 20f;
        
        [ContextMenu("Catch")]
        private void Catch()
        {
            
            
            Grade grade = data.RollGrade();
            Fish fish = spawner.TrySpawnFish(grade, Vector3.up * data.PullPower);
        }

        private Vector3 BuildPullForce()
        {
            float tilt = Random.Range(0, spreadAngle);
            float spin = Random.Range(0f, 360f);

            Vector3 dir = Quaternion.Euler(0, spin, 0) * Quaternion.Euler(tilt, 0, 0) * Vector3.up;

            return dir * data.PullPower;
        }
    }
}