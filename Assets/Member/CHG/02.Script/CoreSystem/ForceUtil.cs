using UnityEngine;

namespace CHG._02.Script.CoreSystem
{
    public static class ForceUtil
    {
        /// <summary>무게 저항과 최소 속도를 반영한 실제 속도 변화량</summary>
        public static float ResolveDeltaV(float power, float mass, float weightInfluence, float minSpeed = 0f)
            => Mathf.Max(power /Mathf.Pow(mass, weightInfluence), minSpeed);

        /// <summary>무게 저항을 반영한 충격량</summary>
        public static float ResolveImpulse(float power, float mass, float weightInfluence)
            => power * Mathf.Pow(mass, 1f - weightInfluence);
        
        /// <summary>높이까지 올라가기 위한 최소 필요 속도</summary>
        public static float SpeedForHeight(float height)
            => Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * height);
    }
}