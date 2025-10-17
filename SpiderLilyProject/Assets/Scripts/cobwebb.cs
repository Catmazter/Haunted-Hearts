using System.Collections;
using UnityEngine;

public class cobwebb : MonoBehaviour
{
    [Header("Renderer & Material")]
    public Renderer objectRenderer;

    [Header("Emission Settings")]
    public Color damageColor = Color.red;
    public float emissionIntensity = 3f;
    public float fadeDuration = 0.5f;

    private Material materialInstance;
    private Color originalEmissionColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        if (objectRenderer == null)
            objectRenderer = GetComponent<Renderer>();

        // Make a material instance (avoid editing shared materials)
        materialInstance = objectRenderer.material;
        originalEmissionColor = materialInstance.GetColor("_EmissionColor");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Optional: filter specific tag
        // if (!collision.collider.CompareTag("Enemy")) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashEmission());
    }

    IEnumerator FlashEmission()
    {
        // Set to red instantly
        Color targetColor = damageColor * emissionIntensity;
        materialInstance.SetColor("_EmissionColor", targetColor);

        // Fade back to original color over fadeDuration seconds
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            Color current = Color.Lerp(targetColor, originalEmissionColor, elapsed / fadeDuration);
            materialInstance.SetColor("_EmissionColor", current);
            yield return null;
        }

        materialInstance.SetColor("_EmissionColor", originalEmissionColor);
    }
}
