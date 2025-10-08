using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [Header("Menus")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuSettings;
    // [SerializeField] GameObject menuWin;
    private Coroutine titleFadeRoutine;
    [SerializeField] TextMeshProUGUI menuTitle;
    private Dictionary<string, GameObject> menus = new Dictionary<string, GameObject>();
    private GameObject currentMenu;
    public GameObject player;
    public PlayerMovement playerScript;

    public bool isPaused;
    float timeScaleOrig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        playerScript = player.GetComponent<PlayerMovement>();

        //add any other menus to dictionary here
        menus.Add("Pause", menuPause);
        menus.Add("Lose", menuLose);
        menus.Add("Settings", menuSettings);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
           if(currentMenu == null )
            {
                OpenMenu("Pause");
            }
           else if(currentMenu == menuPause)
            {
                CloseCurrentMenu();
            }
        }
    }
    public void OpenMenu(string menuName)
    {
        if(!menus.ContainsKey(menuName))
        {
            Debug.LogWarning("Menu " + menuName + " does not exist!");
            return;
        }

        // Pause the game if needed
        if (!isPaused) statePause();

        if (currentMenu != null)
            currentMenu.SetActive(false);

        currentMenu = menus[menuName];
        currentMenu.SetActive(true);
    }
    public void CloseCurrentMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            currentMenu = null;
        }

        // Only unpause when closing top-level menus like Pause or Lose
        if (isPaused && !AnyMenuActive())
            stateUnpause();
    }
    private bool AnyMenuActive()
    {
        foreach (var entry in menus)
            if (entry.Value != null && entry.Value.activeSelf)
                return true;
        return false;
    }
    private void UpdateMenuTitle(string menuName)
    {
        if (menuTitle == null) return;

        // Stop any fade already happening
        if (titleFadeRoutine != null)
            StopCoroutine(titleFadeRoutine);

        if (string.IsNullOrEmpty(menuName))
        {
            // Fade out and clear text
            titleFadeRoutine = StartCoroutine(FadeTitle("", false));
        }
        else
        {
            // Fade in with new text
            string displayName = menuName switch
            {
                "Pause" => "Pause Menu",
                "Settings" => "Settings",
                "Lose" => "Game Over",
                _ => menuName
            };

            titleFadeRoutine = StartCoroutine(FadeTitle(displayName, true));
        }
    }
    private IEnumerator FadeTitle(string newText, bool fadeIn)
    {
        float duration = 0.3f;
        float timer = 0f;

        // Get current color and alpha
        Color color = menuTitle.color;

        if (fadeIn)
        {
            // Set new text, start transparent
            menuTitle.text = newText;
            color.a = 0f;
            menuTitle.color = color;
            menuTitle.gameObject.SetActive(true);

            // Fade from 0 → 1
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime; // Unscaled so it works when paused
                color.a = Mathf.Lerp(0f, 1f, timer / duration);
                menuTitle.color = color;
                yield return null;
            }
        }
        else
        {
            // Fade from 1 → 0
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(1f, 0f, timer / duration);
                menuTitle.color = color;
                yield return null;
            }

            // Hide it after fade out
            menuTitle.gameObject.SetActive(false);
            menuTitle.text = "";
        }

        // Ensure alpha is correct at the end
        color.a = fadeIn ? 1f : 0f;
        menuTitle.color = color;
        titleFadeRoutine = null;
    }
    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void stateLose()
    {
        OpenMenu("Lose");
    }
}
