using NKT.Player.Modules;
using UnityEngine;

namespace NKT.Fishing
{
    //차징 궤도 그리기
    public class CastPreview : MonoBehaviour
    {
        [SerializeField] private FishingModule fishingModule;
        [SerializeField] private LineRenderer line;
        [SerializeField] private int pointCount = 24;

        [SerializeField] private bool isDebug;

        private void Awake()
        {
            fishingModule.OnAimUpdated += Draw;
            fishingModule.OnStateChanged += OnStateChanged;

            line.enabled = false;
        }

        private void OnDestroy()
        {
            if (fishingModule == null) return;

            fishingModule.OnAimUpdated -= Draw;
            fishingModule.OnStateChanged -= OnStateChanged;
        }

        private void Draw(CastAim aim)
        {
            if (!isDebug) return;
            
            line.positionCount = pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);
                line.SetPosition(i, CastArc.Evaluate(aim, t));
            }
        }

        private void OnStateChanged(FishingState state)
        {
            line.enabled = state == FishingState.Charging;
        }
    }
}
