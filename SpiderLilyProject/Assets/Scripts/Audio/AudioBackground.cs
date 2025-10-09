using UnityEngine;

public class AudioBackground : MonoBehaviour
{
    [Header("Transition Sounds")]
    [SerializeField] private AudioSource transitionSource;       
    [SerializeField] private AudioClip[] transitionClips; 
    [SerializeField] private float minInterval = 10f;     
    [SerializeField] private float maxInterval = 60f;
    [Range(0f, 1f)] public float transitionVolume;

    private float timer;
    private float nextPlayTime;

    void Start()
    {
       
        SetNextPlayTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Khi hết thời gian chờ -> phát transition
        if (timer >= nextPlayTime && !transitionSource.isPlaying)
        {
            PlayRandomTransition();
            SetNextPlayTime();
        }
    }

    void SetNextPlayTime()
    {
        timer = 0f;
        nextPlayTime = Random.Range(minInterval, maxInterval);
    }

    void PlayRandomTransition()
    {
        if (transitionClips.Length == 0 || transitionSource == null)
            return;

        transitionSource.PlayOneShot(transitionClips[Random.Range(0, transitionClips.Length)], transitionVolume);


    }
}
