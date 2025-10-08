using UnityEngine;

[CreateAssetMenu(menuName = "Map Data")]
public class  mapData : ScriptableObject
{
    public string mapName;
    public Texture2D mapTexture;
    [HideInInspector] public RenderTexture revealMask;


}
public class mapPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
}
