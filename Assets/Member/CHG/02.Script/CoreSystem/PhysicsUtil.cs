using UnityEngine;

namespace CHG._02.Script.CoreSystem
{
    public static class PhysicsUtil
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

        /// <summary>중력만 받으며 duration 후 end에 도착하기 위한 초기 속도</summary>
        public static Vector3 SolveBallisticVelocity(Vector3 start, Vector3 end, float duration)
            => (end - start - 0.5f * Physics.gravity * duration * duration) / duration;

        /// <summary>초기 속도와 중력만으로 t초 뒤의 위치</summary>
        public static Vector3 BallisticPosition(Vector3 start, Vector3 velocity, float t)
            => start + velocity * t + 0.5f * Physics.gravity * t * t;

        public static Vector3 Bezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1f - t;
            return u * u * p0 + 2f * u * t * p1 + t * t * p2;
        }
    }
}