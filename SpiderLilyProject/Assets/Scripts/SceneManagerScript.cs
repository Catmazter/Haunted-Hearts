using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;

    [Header("Cinematic")]
    [SerializeField] PlayableDirector introDirector;
    [SerializeField] bool playIntroOnStart = false;

    [Header("Fade Transition")]
    [SerializeField] Image fadeOverlay;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float transitionHoldTime = 3f;


    // --- Transition Scene Data ---
    [HideInInspector] public string nextLevelName;
    [HideInInspector] public string nextLevelDescription;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playIntroOnStart && introDirector != null)
            PlayIntroCinematic();
    }

    public void PlayIntroCinematic()
    {
        gameManager.instance.player.SetActive(false);
        introDirector.stopped += OnIntroComplete;
        introDirector.Play();
    }

    private void OnIntroComplete(PlayableDirector d)
    {
        introDirector.stopped -= OnIntroComplete;
        gameManager.instance.player.SetActive(true);
    }

    // Call this to begin the transition process
    public void OnLevelCompleted()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
        {
            // Use scene names for display
            nextLevelName = SceneUtility.GetScenePathByBuildIndex(next).Split('/')[^1].Replace(".unity", "");
            nextLevelDescription = GetLevelDescription(nextLevelName);
            LoadTransitionScene(next);
        }
        else
        {
            Debug.Log("All levels complete!");
            LoadLevel(0); // back to main menu
        }
    }

    private string GetLevelDescription(string levelName)
    {
        // Customize these however you like:
        return levelName switch
        {
            "Level 1" => "Title 1",
            "Level 2" => "Title 2",
            "Level 3" => "Title 3",
            _ => "Next Challenge Awaits..."
        };
    }

    public void LoadTransitionScene(int nextLevelIndex)
    {
        StartCoroutine(LoadTransitionRoutine(nextLevelIndex));
    }

    private IEnumerator LoadTransitionRoutine(int nextLevelIndex)
    {
        yield return Fade(1f);
        SceneManager.LoadScene("TransitionScene");
        yield return Fade(0f);

        // Wait a few seconds in transition before loading the level
        yield return new WaitForSeconds(transitionHoldTime);
        LoadLevel(nextLevelIndex);
    }

    public void LoadLevel(int index)
    {
        StartCoroutine(LoadLevelRoutine(index));
    }

    private IEnumerator LoadLevelRoutine(int index)
    {
        yield return Fade(1f);
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        while (!async.isDone) yield return null;
        yield return Fade(0f);
    }

    IEnumerator Fade(float targetAlpha)
    {
        if (fadeOverlay == null)
            yield break;

        fadeOverlay.raycastTarget = true;
        Color color = fadeOverlay.color;
        float startAlpha = color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeOverlay.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeOverlay.color = color;
        fadeOverlay.raycastTarget = (targetAlpha > 0);
    }
}
