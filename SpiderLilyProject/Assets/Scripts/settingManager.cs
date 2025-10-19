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

    [Header("Camera / Controls")]
    [SerializeField] private CameraController camController;

    public int sensX = 653;
    public int sensY = 653;

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

    #region Audio
    public void SetMasterVolume(float value)
    {
        mixer.SetFloat(masterParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 30f);
        PlayerPrefs.SetFloat(masterParam, value);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat(musicParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 30f);
        PlayerPrefs.SetFloat(musicParam, value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat(sfxParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 30f);
        PlayerPrefs.SetFloat(sfxParam, value);
    }

    public void SetVoiceVolume(float value)
    {
        mixer.SetFloat(voiceParam, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 30f);
        PlayerPrefs.SetFloat(voiceParam, value);
    }
    #endregion

    #region Camera / Controls
    public void SetSensitivityX(int value)
    {
        sensX = value;
        PlayerPrefs.SetInt("SensitivityX", value);
        if (camController != null) camController.SetSensitivityX(value);
    }

    public void SetSensitivityY(int value)
    {
        sensY = value;
        PlayerPrefs.SetInt("SensitivityY", value);
        if (camController != null) camController.SetSensitivityY(value);
    }
    #endregion

    public void LoadSettings()
    {
        
        SetMasterVolume(PlayerPrefs.GetFloat(masterParam, 1f));
        SetMusicVolume(PlayerPrefs.GetFloat(musicParam, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(sfxParam, 1f));
        SetVoiceVolume(PlayerPrefs.GetFloat(voiceParam, 1f));

    
        sensX = PlayerPrefs.GetInt("SensitivityX", sensX);
        sensY = PlayerPrefs.GetInt("SensitivityY", sensY);

     
       

        if (camController != null)
        {
            camController.SetSensitivityX(sensX);
            camController.SetSensitivityY(sensY);
        }
    }
}
