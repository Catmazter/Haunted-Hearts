using UnityEngine;

[CreateAssetMenu]
public class Match : ScriptableObject
{
    public GameObject matchModel;
    public float matchTimer;
    public float matchRadius;
    public AudioClip[] matchSound;
    [Range(0, 1)] public float matchSoundVol;
}
