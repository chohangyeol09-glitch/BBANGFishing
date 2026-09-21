using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public class WaitForEnemySeconds : CustomYieldInstruction
    {
        private readonly float _endTime;

        public WaitForEnemySeconds(float seconds)
        {
            _endTime = EnemyTime.Now + seconds;
        }

        public override bool keepWaiting => EnemyTime.Now < _endTime;
    }
}
