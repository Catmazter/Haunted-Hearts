using UnityEngine;

public class QuitButtonWeb : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject quitButton;
    private void Start()
    {
        #if UNITY_WEBGL
         quitButton.SetActive(false);
        #endif
    }

}
