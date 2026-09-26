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
            //좌우로 꺾어서 날리려면 위 줄 대신: spawner.TrySpawnFish(grade, BuildSidePullForce());
        }

        //위쪽에서 좌우(화면 기준)로만 랜덤하게 조금 기울인 방향. Z축 회전이 화면(XY 평면)의 좌우 기울기다
        private Vector3 BuildSidePullForce()
        {
            float tilt = Random.Range(-spreadAngle, spreadAngle);
            Vector3 dir = Quaternion.Euler(0f, 0f, tilt) * Vector3.up;

            return dir * data.PullPower;
        }
    }
}