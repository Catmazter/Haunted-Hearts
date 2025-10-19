using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [Header("Audio")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterParam = "MasterVolume";
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam = "SFXVolume";
    [SerializeField] private string voiceParam = "VoiceVolume";

    [Header("Controls / Sensitivity")]
    [Range(100, 1000)] public int sensX = 653;
    [Range(100, 1000)] public int sensY = 653;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region ==== AUDIO ====
    public void SetMasterVolume(float value)
    {
        SetMixerVolume(masterParam, value);
    }

    public void SetMusicVolume(float value)
    {
        SetMixerVolume(musicParam, value);
    }

    public void SetSFXVolume(float value)
    {
        SetMixerVolume(sfxParam, value);
    }

    public void SetVoiceVolume(float value)
    {
        SetMixerVolume(voiceParam, value);
    }

    private void SetMixerVolume(string param, float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 30f;
        mixer.SetFloat(param, volume);
        PlayerPrefs.SetFloat(param, value);
        PlayerPrefs.Save();
    }
    #endregion

    #region ==== CAMERA SENSITIVITY ====
    public void SetSensitivityX(int value)
    {
        sensX = value;
        PlayerPrefs.SetInt("SensitivityX", value);
        PlayerPrefs.Save();
    }

    public void SetSensitivityY(int value)
    {
        sensY = value;
        PlayerPrefs.SetInt("SensitivityY", value);
        PlayerPrefs.Save();
    }

    public void ApplySensitivityToCamera(CameraController cam)
    {
        if (cam == null) return;
        cam.SetSensitivityX(sensX);
        cam.SetSensitivityY(sensY);
    }
    #endregion

    public void LoadSettings()
    {
        // --- Load audio ---
        float master = PlayerPrefs.GetFloat(masterParam, 1f);
        float music = PlayerPrefs.GetFloat(musicParam, 1f);
        float sfx = PlayerPrefs.GetFloat(sfxParam, 1f);
        float voice = PlayerPrefs.GetFloat(voiceParam, 1f);

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);
        SetVoiceVolume(voice);

        // --- Load sensitivity ---
        sensX = PlayerPrefs.GetInt("SensitivityX", sensX);
        sensY = PlayerPrefs.GetInt("SensitivityY", sensY);
    }
}
