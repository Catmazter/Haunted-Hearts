using System.Collections;
using UnityEngine;

public class RadarUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;     // Assign same object
    [SerializeField] private RectTransform radarPanel;    // Assign the radar panel itself
    [SerializeField] private AudioSource openSound;       // Optional
    [SerializeField] private AudioSource closeSound;      // Optional

    [Header("Animation Settings")]
    [SerializeField] private float slideDistance = 300f;  // How far offscreen it starts
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine animRoutine;
    private bool isVisible;
    private Vector2 startPos;

    private void Awake()
    {
        // Cache the anchored position (its on-screen resting spot)
        startPos = radarPanel.anchoredPosition;

        // Hide initially
        radarPanel.anchoredPosition = startPos + new Vector2(slideDistance, -slideDistance);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowRadar()
    {
        if (isVisible) return;
        isVisible = true;

        if (animRoutine != null) StopCoroutine(animRoutine);
        gameObject.SetActive(true);
        animRoutine = StartCoroutine(SlideRoutine(true));
        if (openSound) openSound.Play();
    }

    public void HideRadar()
    {
        if (!isVisible) return;
        isVisible = false;

        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(SlideRoutine(false));
        if (closeSound) closeSound.Play();
    }

    public void ForceHide()
    {
        if (animRoutine != null) StopCoroutine(animRoutine);

        radarPanel.anchoredPosition = startPos + new Vector2(slideDistance, -slideDistance);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
        isVisible = false;
    }

    private IEnumerator SlideRoutine(bool showing)
    {
        float t = 0f;
        Vector2 from = showing ? startPos + new Vector2(slideDistance, -slideDistance) : radarPanel.anchoredPosition;
        Vector2 to = showing ? startPos : startPos + new Vector2(slideDistance, -slideDistance);

        float startAlpha = showing ? 0f : 1f;
        float endAlpha = showing ? 1f : 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / slideDuration;
            float eased = easeCurve.Evaluate(Mathf.Clamp01(t));

            radarPanel.anchoredPosition = Vector2.Lerp(from, to, eased);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(t / (fadeDuration / slideDuration)));

            yield return null;
        }

        radarPanel.anchoredPosition = to;
        canvasGroup.alpha = endAlpha;

        bool active = showing;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;

        if (!active) gameObject.SetActive(false);
        animRoutine = null;
    }
}
