using System;
using Member.JJK._02._Scripts.Weapon;
using UnityEngine;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.Settings
{
    public class ControlSettingsUI : MonoBehaviour
    {
        private const float DefaultSensitivity = 0.1f;
        private const float DefaultZoomSensitivity = 0.05f;
        private const bool DefaultDotEnabled = true;
        private const float DefaultDotSize = 4f;
        private const float DefaultLineLength = 10f;
        private const float DefaultLineThickness = 2f;
        private const float DefaultLineGap = 6f;
        private const float DefaultOpacity = 1f;
        private static readonly Color DefaultCrosshairColor = Color.white;

        private const string SensitivityKey = "Settings.Sensitivity";
        private const string ZoomSensitivityKey = "Settings.ZoomSensitivity";
        private const string DotEnabledKey = "Settings.Crosshair.DotEnabled";
        private const string DotSizeKey = "Settings.Crosshair.DotSize";
        private const string LineLengthKey = "Settings.Crosshair.LineLength";
        private const string LineThicknessKey = "Settings.Crosshair.LineThickness";
        private const string LineGapKey = "Settings.Crosshair.LineGap";
        private const string OpacityKey = "Settings.Crosshair.Opacity";
        private const string ColorKey = "Settings.Crosshair.Color";

        [Header("Sensitivity")]
        [SerializeField] private MouseSensitivitySO sensitivity;
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Slider zoomSensitivitySlider;

        public void Configure(MouseSensitivitySO sensitivitySO, Slider generalSlider, Slider zoomSlider)
        {
            sensitivity = sensitivitySO;
            sensitivitySlider = generalSlider;
            zoomSensitivitySlider = zoomSlider;
        }

        private bool _initialized;

        private void Awake() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            LoadAndApply();
            BindUI();
        }

        private void LoadAndApply()
        {
            if (sensitivity != null)
            {
                sensitivity.SetValue(PlayerPrefs.GetFloat(SensitivityKey, sensitivity.Value));
                sensitivity.SetZoomValue(PlayerPrefs.GetFloat(ZoomSensitivityKey, sensitivity.ZoomValue));
            }
        }

        private void BindUI()
        {
            if (sensitivitySlider != null && sensitivity != null)
            {
                sensitivitySlider.SetValueWithoutNotify(sensitivity.Value);
                sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
            }

            if (zoomSensitivitySlider != null && sensitivity != null)
            {
                zoomSensitivitySlider.SetValueWithoutNotify(sensitivity.ZoomValue);
                zoomSensitivitySlider.onValueChanged.AddListener(SetZoomSensitivity);
            }
        }

        public void ResetToDefaults()
        {
            if (sensitivitySlider != null) sensitivitySlider.value = DefaultSensitivity;
            else SetSensitivity(DefaultSensitivity);

            if (zoomSensitivitySlider != null) zoomSensitivitySlider.value = DefaultZoomSensitivity;
            else SetZoomSensitivity(DefaultZoomSensitivity);
        }

        private void SetSensitivity(float value)
        {
            sensitivity.SetValue(value);
            PlayerPrefs.SetFloat(SensitivityKey, value);
        }

        private void SetZoomSensitivity(float value)
        {
            sensitivity.SetZoomValue(value);
            PlayerPrefs.SetFloat(ZoomSensitivityKey, value);
        }

        private static void SaveColor(string key, Color value)
        {
            PlayerPrefs.SetFloat(key + ".r", value.r);
            PlayerPrefs.SetFloat(key + ".g", value.g);
            PlayerPrefs.SetFloat(key + ".b", value.b);
        }

        private static Color LoadColor(string key, Color fallback)
        {
            float r = PlayerPrefs.GetFloat(key + ".r", fallback.r);
            float g = PlayerPrefs.GetFloat(key + ".g", fallback.g);
            float b = PlayerPrefs.GetFloat(key + ".b", fallback.b);
            return new Color(r, g, b);
        }
    }
}
