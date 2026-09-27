using System;
using UnityEngine;

namespace Member.JJK._02._Scripts.Skill
{
    public static class EnemyTime
    {
        public static event Action<float> ScaleChanged;

        public static float Scale { get; private set; } = 1f;

        public static float DeltaTime => Time.deltaTime * Scale;

        public static float Now => _accumulated + (Time.time - _anchorTime) * Scale;

        private static float _accumulated;
        private static float _anchorTime;

        public static void SetScale(float scale)
        {
            _accumulated = Now;
            _anchorTime = Time.time;
            Scale = scale;
            ScaleChanged?.Invoke(scale);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            Scale = 1f;
            _accumulated = 0f;
            _anchorTime = 0f;
            ScaleChanged = null;
        }
    }
}
