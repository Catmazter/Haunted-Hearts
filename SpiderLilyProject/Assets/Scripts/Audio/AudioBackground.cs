using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioBackground : MonoBehaviour
{
    [Header("Transition Sounds")]
    [SerializeField] private AudioSource transitionSource;
    [SerializeField] private AudioClip[] transitionClips;
    [SerializeField] private float minInterval = 10f;
    [SerializeField] private float maxInterval = 60f;
    [Range(0f, 1f)] public float transitionVolume = 0.5f;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string _volumeParameter = "MasterVolume";
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private float _multiplier = 30f;

    private float timer;
    private float nextPlayTime;

    private void Start()
    {
      
        float savedValue = PlayerPrefs.GetFloat(_volumeParameter, sliderMaster.value);
        savedValue = Mathf.Clamp(savedValue, 0.0001f, 1f);
        sliderMaster.SetValueWithoutNotify(savedValue);
        ApplyVolume(savedValue);
        sliderMaster.onValueChanged.AddListener(HandleSliderValueChanged);
        SetNextPlayTime();
    }

    private void HandleSliderValueChanged(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        ApplyVolume(value);
        PlayerPrefs.SetFloat(_volumeParameter, value);
    }

    private void ApplyVolume(float value)
    {
        mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * _multiplier);
    }

    void SetNextPlayTime()
    {
        timer = 0f;
        nextPlayTime = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextPlayTime && !transitionSource.isPlaying)
        {
            PlayRandomTransition();
            SetNextPlayTime();
        }
    }

    void PlayRandomTransition()
    {
        if (transitionClips.Length == 0 || transitionSource == null)
            return;

        transitionSource.PlayOneShot(
            transitionClips[Random.Range(0, transitionClips.Length)],
            transitionVolume
        );
    }
}
