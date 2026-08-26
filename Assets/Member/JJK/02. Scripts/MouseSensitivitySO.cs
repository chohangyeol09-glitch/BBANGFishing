using UnityEngine;

namespace Member.JJK._02._Scripts
{
    [CreateAssetMenu(fileName = "MouseSensivitySO", menuName = "JJK/MouseSensivity", order = 0)]
    public class MouseSensitivitySO : ScriptableObject
    {
        [SerializeField] private float value = 0.1f;
        public float Value => value;

        public void SetValue(float newValue) => value = newValue;
    }
}