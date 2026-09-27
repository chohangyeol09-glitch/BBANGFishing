using UnityEngine;

namespace Member.JJK._02._Scripts
{
    [CreateAssetMenu(fileName = "MouseSensivitySO", menuName = "JJK/MouseSensivity", order = 0)]
    public class MouseSensitivitySO : ScriptableObject
    {
        [SerializeField] private float value = 0.1f;
        [SerializeField] private float zoomValue = 0.05f;
        public float Value => value;
        public float ZoomValue => zoomValue;

        public void SetValue(float newValue) => value = newValue;
        public void SetZoomValue(float newValue) => zoomValue = newValue;
    }
}