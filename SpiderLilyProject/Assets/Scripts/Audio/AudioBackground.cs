using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";
    [SerializeField] private string voiceVolumeParam = "VoiceVolume";

    [Header("Sliders")]
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] transitionClips;
    [SerializeField] private AudioClip womenCry;
    [Range(0f, 1f)] public float transitionVolume = 0.5f;
    [Range(0f, 1f)] public float womenCryVolume = 0.5f;
    [SerializeField] private float minInterval = 15f;
    [SerializeField] private float maxInterval = 40f;

    [Header("Enemy / Timer SFX")]
    [SerializeField] private AudioSource sfxSource;               
    [SerializeField] private AudioClip enemyDestroyedClip;
    [Range(0f, 1f)] public float enemyDestroyedVol = 1f;

    [SerializeField] private AudioClip timerStartedClip;
    [Range(0f, 1f)] public float timerStartedVol = 0.9f;

    [SerializeField] private AudioClip timerAlmostDoneClip; 
    [Range(0f, 1f)] public float timerAlmostDoneVol = 0.9f;

    [Header("Toggles")]
    [SerializeField] private Toggle toggleMuteAll;
    [SerializeField] private Toggle toggleMuteVoice;

    [Header("UI Sounds")]
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioClip menuClickClip;
    [Range(0f, 1f)] public float uiVolume = 0.7f;

    private float timer = 0f;
    private float nextPlayTime;

    
    private float masterSaved = 1f;
    private float musicSaved = 1f;
    private float sfxSaved = 1f;
    private float voiceSaved = 1f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Debug.LogWarning("Multiple AudioManager instances!");

        // Load saved slider values
        sliderMaster.value = PlayerPrefs.GetFloat(masterVolumeParam, sliderMaster.value);
        sliderMusic.value = PlayerPrefs.GetFloat(musicVolumeParam, sliderMusic.value);
        sliderSFX.value = PlayerPrefs.GetFloat(sfxVolumeParam, sliderSFX.value);

        masterSaved = sliderMaster.value;
        musicSaved = sliderMusic.value;
        sfxSaved = sliderSFX.value;

        ApplyVolume(masterVolumeParam, sliderMaster.value);
        ApplyVolume(musicVolumeParam, sliderMusic.value);
        ApplyVolume(sfxVolumeParam, sliderSFX.value);

        // Add slider listeners
        sliderMaster.onValueChanged.AddListener((value) =>
        {
            masterSaved = value;
            if (!toggleMuteAll.isOn)
            {
                ApplyVolume(masterVolumeParam, value);
            }
            PlayerPrefs.SetFloat(masterVolumeParam, value);
        });

        sliderMusic.onValueChanged.AddListener((value) =>
        {
            musicSaved = value;
            ApplyVolume(musicVolumeParam, value);
            PlayerPrefs.SetFloat(musicVolumeParam, value);
        });

        sliderSFX.onValueChanged.AddListener((value) =>
        {
            sfxSaved = value;
            ApplyVolume(sfxVolumeParam, value);
            PlayerPrefs.SetFloat(sfxVolumeParam, value);
        });

        // Add toggle listeners
        toggleMuteAll.onValueChanged.AddListener(MuteAllChanged);
        toggleMuteVoice.onValueChanged.AddListener(MuteVoiceChanged);

       
        mixer.GetFloat(voiceVolumeParam, out float voiceDb);
        voiceSaved = Mathf.Pow(10, voiceDb / 30f);
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Level 1")
        {
        musicSource.PlayOneShot(
            womenCry,
            womenCryVolume);
        }
        SetNextPlayTime();

        if (SceneManager.GetActiveScene().name == "Level 3")
        {
            musicSource.PlayOneShot(
                timerStartedClip, timerStartedVol
                );
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextPlayTime && !musicSource.isPlaying)
        {
            PlayRandomTransition();
            SetNextPlayTime();
        }
    }

    public void PlayEnemyDestroy()
    {
        if(sfxSource != null && enemyDestroyedClip  != null)
        {
            sfxSource.PlayOneShot(enemyDestroyedClip, enemyDestroyedVol);
        }
    }
    public void PlayTimerStarted()
    {
        if(sfxSource != null && timerStartedClip != null)
        {
            sfxSource.PlayOneShot(timerStartedClip, timerStartedVol);
        }
    }
    public void PlayTimerAlmostDone()
    {
        if(sfxSource != null && timerAlmostDoneClip != null)
        {
            sfxSource.PlayOneShot(timerAlmostDoneClip, timerAlmostDoneVol);
        }
    }

    private void ApplyVolume(string parameter, float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        mixer.SetFloat(parameter, Mathf.Log10(value) * 30f);
    }

    private void SetNextPlayTime()
    {
        timer = 0f;
        nextPlayTime = Random.Range(minInterval, maxInterval);
    }

    private void PlayRandomTransition()
    {
        if (transitionClips.Length == 0 || musicSource == null) return;
        musicSource.PlayOneShot(
            transitionClips[Random.Range(0, transitionClips.Length)],
            transitionVolume
        );
    }

    private void MuteAllChanged(bool isMuted)
    {
        PlayMenuClick();
        if (isMuted) mixer.SetFloat(masterVolumeParam, -80f);
        else ApplyVolume(masterVolumeParam, masterSaved);
    }

    private void MuteVoiceChanged(bool isMuted)
    {
        PlayMenuClick();
        if (isMuted) mixer.SetFloat(voiceVolumeParam, -80f);
        else ApplyVolume(voiceVolumeParam, voiceSaved);
    }

    public void PlayMenuClick()
    {
        if (uiSource != null && menuClickClip != null)
        {
            uiSource.PlayOneShot(menuClickClip, uiVolume);
        }
    }
    public void PauseAllAudio()
    {
        AudioSource[] all = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var src in all)
        {
            if (src.isPlaying)
                src.Pause();
        }
    }

    public void ResumeAllAudio()
    {
        AudioSource[] all = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var src in all)
        {
            src.UnPause();
        }
    }

   

}
