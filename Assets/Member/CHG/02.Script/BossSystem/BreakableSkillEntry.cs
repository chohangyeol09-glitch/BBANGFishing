using System;
using DevLib.AnimatorSystem;

namespace CHG._02.Script.BossSystem
{
    [Serializable]
    public class BreakableSkillEntry
    {
        public HashDataSO Skill; //파훼 가능한 스킬

        public float BreakDamage = 30f; //이만큼 맞으면 파훼
        public float GroggyAmount; //파훼하면 쌓이는 그로기
    }
}