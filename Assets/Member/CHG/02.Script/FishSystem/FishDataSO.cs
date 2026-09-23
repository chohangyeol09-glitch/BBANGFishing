using System;
using CHG._02.Script.CombatSystem.EnemySkillSystem;
using CHG._02.Script.CoreSystem;
using DevLib.AnimatorSystem;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    [Serializable]
    public class SkillEntry
    {
        public HashDataSO Skill;
        public int Priority;
        public AbstractSkillCondition[] Conditions;
    }
    
    [CreateAssetMenu(fileName = "Fish data", menuName = "CHG/Fish/Fish data", order = 0)]
    public class FishDataSO : ScriptableObject
    {
        public Grade Grade;
        public float Health;
        public float Weight;
        public float JumpPower;

        public SkillEntry[] Skills;
        
        public Sprite Sprite;
        public int Price;

        [Header("Lunge")]
        public bool CanLunge; //Sea에 닿으면 플레이어에게 돌진하는가
        public float LungeFlightTime = 1f; //돌진 비행 시간
        public float LungeFrontDistance = 1.5f; //도착 지점: 플레이어 앞쪽 거리
        public float LungeHeightOffset; //도착 지점: 플레이어 위치 기준 높이 보정
        public float ParryRange = 3f; //플레이어와 이 거리 안이면 패링 가능
        public float ReturnFlightTime = 1f; //되돌아가는 비행 시간

    }
}