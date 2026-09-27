using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.Settings
{
    public class SoundSettingsUI : MonoBehaviour
    {
        private const float MutedDecibel = -80f;

        private const string MasterVolumeKey = "Settings.MasterVolume";
        private const string BgmVolumeKey = "Settings.BgmVolume";
        private const string SfxVolumeKey = "Settings.SfxVolume";

        [Header("Master")]
        [SerializeField] private Slider masterVolumeSlider;

        [Header("Mixer (BGM / SFX)")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string bgmParameter = "BGMVolume";
        [SerializeField] private string sfxParameter = "SFXVolume";
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        public void Configure(Slider masterSlider, AudioMixer mixerRef, string bgmParam, string sfxParam, Slider bgmSlider, Slider sfxSlider)
        {
            masterVolumeSlider = masterSlider;
            mixer = mixerRef;
            bgmParameter = bgmParam;
            sfxParameter = sfxParam;
            bgmVolumeSlider = bgmSlider;
            sfxVolumeSlider = sfxSlider;
        }

        public void ResetToDefaults()
        {
            if (masterVolumeSlider != null) masterVolumeSlider.value = 1f;
            else SetMasterVolume(1f);

            if (bgmVolumeSlider != null) bgmVolumeSlider.value = 1f;
            else SetBgmVolume(1f);

            if (sfxVolumeSlider != null) sfxVolumeSlider.value = 1f;
            else SetSfxVolume(1f);
        }

        private bool _initialized;

        private void Awake() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            AudioListener.volume = PlayerPrefs.GetFloat(MasterVolumeKey, AudioListener.volume);
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.SetValueWithoutNotify(AudioListener.volume);
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }

            BindMixerSlider(bgmVolumeSlider, bgmParameter, BgmVolumeKey, SetBgmVolume);
            BindMixerSlider(sfxVolumeSlider, sfxParameter, SfxVolumeKey, SetSfxVolume);
        }

        private void BindMixerSlider(Slider slider, string parameterName, string prefsKey, UnityEngine.Events.UnityAction<float> onChanged)
        {
            if (slider == null || mixer == null) return;

            float saved = PlayerPrefs.GetFloat(prefsKey, 1f);
            ApplyVolume(parameterName, saved);
            slider.SetValueWithoutNotify(saved);
            slider.onValueChanged.AddListener(onChanged);
        }

        private void SetMasterVolume(float value)
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat(MasterVolumeKey, value);
        }

        private void SetBgmVolume(float value)
        {
            ApplyVolume(bgmParameter, value);
            PlayerPrefs.SetFloat(BgmVolumeKey, value);
        }

        private void SetSfxVolume(float value)
        {
            ApplyVolume(sfxParameter, value);
            PlayerPrefs.SetFloat(SfxVolumeKey, value);
        }

        private void ApplyVolume(string parameterName, float linearValue)
        {
            // AudioMixer의 Volume 파라미터는 데시벨 단위라 0~1 슬라이더 값을 로그 스케일로 변환한다.
            float decibel = linearValue <= 0.0001f ? MutedDecibel : Mathf.Log10(linearValue) * 20f;
            mixer.SetFloat(parameterName, decibel);
        }
    }
}
