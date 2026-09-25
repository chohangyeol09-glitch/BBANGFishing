using DevLib.EventChannelSystem;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.HitFeedback
{
    public class HitStopEvent : GameEvent
    {
        public float Duration;
    }

    public class HitStopManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO feedbackChannel;
        private float _endTime;
        private bool _stopping;

        private void OnEnable() => feedbackChannel.AddListener<HitStopEvent>(HandleHitStop);
        private void OnDisable() => feedbackChannel.RemoveListener<HitStopEvent>(HandleHitStop);

        private void HandleHitStop(HitStopEvent evt)
        {
            _endTime = Mathf.Max(_endTime, Time.unscaledTime + evt.Duration);
            _stopping = true;
            Time.timeScale = 0f;
        }

        private void Update()
        {
            if (!_stopping || Time.unscaledTime < _endTime) return;
            _stopping = false;
            Time.timeScale = 1f;
        }
    }
}