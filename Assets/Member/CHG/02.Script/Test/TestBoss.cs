using System;
using CHG._02.Script.BossSystem;
using CHG._02.Script.CombatSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CHG._02.Script.Test
{
    public class TestBoss : MonoBehaviour
    {
        [SerializeField] private Boss boss;
        [SerializeField] private GameObject target;
        [SerializeField] private float testDamage = 10f;
        [SerializeField] private float testGroggy = 30f;

        private GroggyModule _groggy;

        private void Start()
        {
            _groggy = boss.GetModule<GroggyModule>();
            boss.OnStateChanged += state => Debug.Log($"보스 상태: {state}");
            _groggy.OnGaugeChanged += value => Debug.Log($"Gauge: {value}");
            PatternBreakModule breakModule = boss.GetModule<PatternBreakModule>();
            breakModule.OnBreakProgress += value => Debug.Log($"파훼 진행: {value:P0}");
            breakModule.OnPatternBroken += () => Debug.Log("패턴 파훼!");
            //소환(OnSpawn)은 BossSummoner가 한다. 보스가 켜진 뒤에야 이 Start가 돌기 때문
            //보스를 켜 둔 채 테스트하려면 BossSummoner 없이 여기서 boss.OnSpawn(target)을 부른다
        }

        private void Update()
        {
            if (Keyboard.current.jKey.wasPressedThisFrame)
                boss.TakeDamage(new DamageData(boss, boss.transform.position, Vector3.up, Vector3.up, testDamage, 0f));
            
            if (Keyboard.current.gKey.wasPressedThisFrame)
                _groggy.AddGroggy(testGroggy);
        }
    }
}