using System.Collections;
using UnityEngine;

public class lightFade : MonoBehaviour
{
    [SerializeField] Light matchLight;
    float lightDuration;
    float lightIntensity;
    float lightIntensityOrig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightIntensity = 0;
        lightIntensityOrig = matchLight.intensity;
        lightDuration = gameManager.instance.playerScript.matchTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (matchLight.intensity > 0)
        {
            StartCoroutine(fadeLight());
        }
        else if (matchLight.intensity <= 0)
        {
            matchLight.intensity = lightIntensityOrig;
        }
    }
    IEnumerator fadeLight()
    {
        float timer = 0f;
        if (gameManager.instance.playerScript.isMatchLit)
        {
            while (timer < lightDuration)
            {
                float progress = timer / lightDuration;
                matchLight.intensity = Mathf.Lerp(lightIntensityOrig, lightIntensity, progress);
                yield return null;
            }
            matchLight.intensity = lightIntensity;
        }
        matchLight.intensity = lightIntensityOrig;
    }
}
