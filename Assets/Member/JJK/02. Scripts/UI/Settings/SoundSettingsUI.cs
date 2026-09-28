using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Member.JJK._02._Scripts.Settings
{
    public class SoundSettingsUI : MonoBehaviour
    {
        private const float MutedDecibel = -80f;

        private const string MasterVolumeKey = "Settings.MasterVolume";
        private const string BgmVolumeKey = "Settings.BgmVolume";
        private const string SfxVolumeKey = "Settings.SfxVolume";
        private const string MasterMutedKey = "Settings.MasterMuted";
        private const string BgmMutedKey = "Settings.BgmMuted";
        private const string SfxMutedKey = "Settings.SfxMuted";

        [Header("Master")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Toggle masterMuteToggle;

        [Header("Mixer (BGM / SFX)")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string bgmParameter = "BGMVolume";
        [SerializeField] private string sfxParameter = "SFXVolume";
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Toggle bgmMuteToggle;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle sfxMuteToggle;

        private bool _bgmMuted;
        private bool _sfxMuted;

        public void Configure(Slider masterSlider, Toggle masterMute, AudioMixer mixerRef, string bgmParam, string sfxParam,
            Slider bgmSlider, Toggle bgmMute, Slider sfxSlider, Toggle sfxMute)
        {
            masterVolumeSlider = masterSlider;
            masterMuteToggle = masterMute;
            mixer = mixerRef;
            bgmParameter = bgmParam;
            sfxParameter = sfxParam;
            bgmVolumeSlider = bgmSlider;
            bgmMuteToggle = bgmMute;
            sfxVolumeSlider = sfxSlider;
            sfxMuteToggle = sfxMute;
        }

        public void ResetToDefaults()
        {
            if (masterMuteToggle != null) masterMuteToggle.isOn = false;
            if (masterVolumeSlider != null) masterVolumeSlider.value = 1f;
            else SetMasterVolume(1f);

            if (mixer != null)
            {
                if (bgmMuteToggle != null) bgmMuteToggle.isOn = false;
                if (bgmVolumeSlider != null) bgmVolumeSlider.value = 1f;
                else SetBgmVolume(1f);

                if (sfxMuteToggle != null) sfxMuteToggle.isOn = false;
                if (sfxVolumeSlider != null) sfxVolumeSlider.value = 1f;
                else SetSfxVolume(1f);
            }
        }

        private bool _initialized;

        private void Awake() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            bool masterMuted = PlayerPrefs.GetInt(MasterMutedKey, 0) == 1;
            float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, AudioListener.volume);
            AudioListener.volume = masterMuted ? 0f : masterVolume;

            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.SetValueWithoutNotify(masterVolume);
                masterVolumeSlider.interactable = !masterMuted;
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }

            if (masterMuteToggle != null)
            {
                masterMuteToggle.SetIsOnWithoutNotify(masterMuted);
                masterMuteToggle.onValueChanged.AddListener(SetMasterMuted);
            }

            BindMixerChannel(bgmVolumeSlider, bgmMuteToggle, bgmParameter, BgmVolumeKey, BgmMutedKey,
                SetBgmVolume, SetBgmMuted, out _bgmMuted);
            BindMixerChannel(sfxVolumeSlider, sfxMuteToggle, sfxParameter, SfxVolumeKey, SfxMutedKey,
                SetSfxVolume, SetSfxMuted, out _sfxMuted);
        }

        private void BindMixerChannel(Slider slider, Toggle muteToggle, string parameterName, string volumeKey, string mutedKey,
            UnityAction<float> onVolumeChanged, UnityAction<bool> onMuteChanged, out bool muted)
        {
            muted = false;
            if (mixer == null) return;

            muted = PlayerPrefs.GetInt(mutedKey, 0) == 1;
            float volume = PlayerPrefs.GetFloat(volumeKey, 1f);
            mixer.SetFloat(parameterName, muted ? MutedDecibel : LinearToDecibel(volume));

            if (slider != null)
            {
                slider.SetValueWithoutNotify(volume);
                slider.interactable = !muted;
                slider.onValueChanged.AddListener(onVolumeChanged);
            }

            if (muteToggle != null)
            {
                muteToggle.SetIsOnWithoutNotify(muted);
                muteToggle.onValueChanged.AddListener(onMuteChanged);
            }
        }

        private void SetMasterVolume(float value)
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, value);
            if (masterMuteToggle == null || !masterMuteToggle.isOn)
                AudioListener.volume = value;
        }

        private void SetMasterMuted(bool muted)
        {
            PlayerPrefs.SetInt(MasterMutedKey, muted ? 1 : 0);
            float volume = masterVolumeSlider != null ? masterVolumeSlider.value : PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            AudioListener.volume = muted ? 0f : volume;
            if (masterVolumeSlider != null) masterVolumeSlider.interactable = !muted;
        }

        private void SetBgmVolume(float value)
        {
            PlayerPrefs.SetFloat(BgmVolumeKey, value);
            if (!_bgmMuted) mixer.SetFloat(bgmParameter, LinearToDecibel(value));
        }

        private void SetBgmMuted(bool muted)
        {
            _bgmMuted = muted;
            PlayerPrefs.SetInt(BgmMutedKey, muted ? 1 : 0);
            float volume = bgmVolumeSlider != null ? bgmVolumeSlider.value : PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
            mixer.SetFloat(bgmParameter, muted ? MutedDecibel : LinearToDecibel(volume));
            if (bgmVolumeSlider != null) bgmVolumeSlider.interactable = !muted;
        }

        private void SetSfxVolume(float value)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, value);
            if (!_sfxMuted) mixer.SetFloat(sfxParameter, LinearToDecibel(value));
        }

        private void SetSfxMuted(bool muted)
        {
            _sfxMuted = muted;
            PlayerPrefs.SetInt(SfxMutedKey, muted ? 1 : 0);
            float volume = sfxVolumeSlider != null ? sfxVolumeSlider.value : PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            mixer.SetFloat(sfxParameter, muted ? MutedDecibel : LinearToDecibel(volume));
            if (sfxVolumeSlider != null) sfxVolumeSlider.interactable = !muted;
        }

        // AudioMixer의 Volume 파라미터는 데시벨 단위라 0~1 슬라이더 값을 로그 스케일로 변환한다.
        private static float LinearToDecibel(float linearValue) =>
            linearValue <= 0.0001f ? MutedDecibel : Mathf.Log10(linearValue) * 20f;
    }
}
