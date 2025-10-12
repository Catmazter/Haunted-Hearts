using UnityEngine;
using System.Collections;
using System.Diagnostics.Tracing;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource source;
    public float[] samples = new float[1024];
    public static float currentAMP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        getSpectrumAudioSource();
    }
    void getSpectrumAudioSource()
    {
        source.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        currentAMP = Mathf.Sqrt(sum) / samples.Length;
    }
}
