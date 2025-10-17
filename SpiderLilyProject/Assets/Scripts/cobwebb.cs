using System.Collections;
using UnityEngine;

public class cobwebb : MonoBehaviour
{
    [Header("Renderer & Material")]
    private Renderer objectRenderer;

    [Header("Emission Settings")]
    public Color damageColor = Color.red;
    public float emissionIntensity = 3f;
    public float fadeDuration = 0.3f;
    public int flashCount = 2;

    private Material materialInstance;
    private Color originalEmissionColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        materialInstance = objectRenderer.material;
        originalEmissionColor = materialInstance.GetColor("_EmissionColor");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashEmission());
    }

    IEnumerator FlashEmission()
    {
        Color targetColor = damageColor * emissionIntensity;

        for (int i = 0; i < flashCount; i++)
        {
            
            materialInstance.SetColor("_EmissionColor", targetColor);
            yield return new WaitForSeconds(fadeDuration);

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                Color current = Color.Lerp(targetColor, originalEmissionColor, elapsed / fadeDuration);
                materialInstance.SetColor("_EmissionColor", current);
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
