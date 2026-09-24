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
            boss.OnSpawn(target);
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