using UnityEngine;

[CreateAssetMenu(menuName = "Lily")]
public class lilyScriptableObject : ScriptableObject
{
    [SerializeField] GameObject itemModel;
    public AudioClip pickupSound;
    public string itemName;
    public ParticleSystem pickupEffect;

}