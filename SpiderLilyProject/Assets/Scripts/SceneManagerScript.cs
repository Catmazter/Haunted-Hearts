using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;

    [Header("Cinematic")]
    [SerializeField] PlayableDirector introDirector;
    [SerializeField] bool playIntroOnStart = false;

    [Header("Fade Transition")]
    [SerializeField] Image fadeOverlay;
    [SerializeField] float fadeDuration = 1f;

    // Runtime data
    [HideInInspector] public int nextLevelIndex = -1;
    [HideInInspector] public string nextLevelName = "";
    [HideInInspector] public bool isFinalLevel = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
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

    // Called when player wins a level
    public void OnLevelCompleted()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = next;
            nextLevelName = SceneUtility.GetScenePathByBuildIndex(next)
                .Split('/')[^1].Replace(".unity", "");
            isFinalLevel = (next == SceneManager.sceneCountInBuildSettings - 1);

            StartCoroutine(GoToTransitionScene());
        }
        else
        {
            // No next level — show Win UI
            nextLevelIndex = -1;
            nextLevelName = "";
            isFinalLevel = true;
            StartCoroutine(GoToTransitionScene());
        }
    }

    private IEnumerator GoToTransitionScene()
    {
        yield return Fade(1f);
        SceneManager.LoadScene("TransitionScene");
        yield return Fade(0f);
    }

    public void LoadNextLevel()
    {
        if (nextLevelIndex >= 0)
            StartCoroutine(LoadLevelRoutine(nextLevelIndex));
        else
            SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator LoadLevelRoutine(int index)
    {
        yield return Fade(1f);
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        while (!async.isDone) yield return null;
        yield return Fade(0f);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeOverlay == null) yield break;

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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Debug.Log("[DEV] Forcing level complete via F10");
            OnLevelCompleted();
        }
    }
#endif
}
