using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

  
 
    [Header("Menus")]
    [SerializeField] GameObject menuRoot;
   // [SerializeField] GameObject background;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuSettings;
   

    [Header("Settings")]
    [SerializeField] GameObject settingsGameplay;
    [SerializeField] GameObject settingsAudio;
    private Stack<GameObject> menuStack = new Stack<GameObject>();
    [Header("Menu Title")]
    [SerializeField] TextMeshProUGUI menuTitle;
    [SerializeField] float titleFadeDuration = 0.3f;
    private Coroutine titleFadeRoutine;
    private Dictionary<string, GameObject> menus = new Dictionary<string, GameObject>();
    private GameObject currentMenu;

    [Header("GameGoal")]
    int gameGoalCount ;
    [SerializeField] TextMeshProUGUI gameGoalCountText;
    [Header("Radar")]
    [SerializeField] GameObject radarUI;
    public bool isRadarOpen;
    bool wasRadarOpenBefore;


            // --- Public read-only accessors ---
    public GameObject CurrentMenu => currentMenu;
    public Dictionary<string, GameObject> Menus => menus;
    [Header("Player")]
    public GameObject player;
    public PlayerMovement playerScript;

    public bool isPaused;
    float timeScaleOrig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        if(player != null) 
        playerScript = player.GetComponent<PlayerMovement>();

        //add any other menus to dictionary here
        menus.Add("Pause", menuPause);
        menus.Add("Lose", menuLose);
        menus.Add("Settings", menuSettings);
        menus.Add("Settings-Gameplay", settingsGameplay);

        menus.Add("Settings-Audio", settingsAudio);
        
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.P))
        {
           if(currentMenu == null )
            {
                OpenMenu("Pause");
            }
           else 
            {
                CloseCurrentMenu();
            }
        }
        if(Input.GetButtonDown("Map"))
        {
            ToggleRadarUI();
        }
    }
    public void OpenMenu(string menuName)
    {
        if (!menus.ContainsKey(menuName))
        {
            Debug.LogWarning("Menu " + menuName + " does not exist!");
            return;
        }

        // Pause the game if needed
        if (!isPaused)
        {
            wasRadarOpenBefore = isRadarOpen;
            if(isRadarOpen && radarUI != null)
            {
                ToggleRadarUI();
            }
            statePause();
        }
        // Ensure background and root are visible
        if (menuRoot != null) menuRoot.SetActive(true);
      

        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            menuStack.Push(currentMenu); // Push current menu onto stack
        }

        currentMenu = menus[menuName];
        currentMenu.SetActive(true);
        UpdateMenuTitle(menuName);

        if (menuName == "Settings")
        {
           
            currentMenu.SetActive(true);
            SetActiveSettingsTab("Audio");
            
        }
    }
    public void SetActiveSettingsTab(string tabName)
    {
        // Disable all tabs first
        settingsGameplay.SetActive(false);
        settingsAudio.SetActive(false);

        // Enable chosen tab
        switch (tabName)
        {
            case "Gameplay":
                settingsGameplay.SetActive(true);
                UpdateMenuTitle("Gameplay");
                break;

            case "Audio":
                settingsAudio.SetActive(true);
                UpdateMenuTitle("Audio");
                break;
        }
    }

    public void CloseCurrentMenu()
    {
        if (currentMenu == null) return;

        currentMenu.SetActive(false);

        // If there's a menu to go back to, open it
        if (menuStack.Count > 0)
        {
            currentMenu = menuStack.Pop();
            currentMenu.SetActive(true);
            UpdateMenuTitle(currentMenu.name);
            return; // don't unpause
        }

        // Otherwise, no more menus → unpause
        currentMenu = null;
        if (menuRoot != null) menuRoot.SetActive(false);
        if (isPaused)
            stateUnpause();
        UpdateMenuTitle("");
    }
    public void ToggleRadarUI()
    {
        if (currentMenu!= null || isPaused) return; // don't open radar if in a menu or paused
        isRadarOpen = !isRadarOpen;
        radarUI.SetActive(isRadarOpen);
    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        if (gameGoalCount <= 0)
        {
            // you win!!

            SceneManagerScript.instance.OnLevelCompleted();

        }
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
                "Lose" => "",
                "Gameplay" => "Gameplay",
                "Audio" => "Audio",
                _ => menuName
            };

            titleFadeRoutine = StartCoroutine(FadeTitle(displayName, true));
        }
    }

    public void pauseTime()
    {         
        Time.timeScale = 0;
        isPaused = true;
    }
    public void statePause()
    {
        pauseTime();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void unpauseTime()
    {
        Time.timeScale = timeScaleOrig;
        isPaused = false;
    }
    public void stateUnpause()
    {
        unpauseTime();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (wasRadarOpenBefore) { ToggleRadarUI(); }
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            currentMenu = null;
        }
    }
    public void stateLose()
    {
        OpenMenu("Lose");
    }
    private IEnumerator FadeTitle(string newText, bool fadeIn)
    {
       
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
            while (timer < titleFadeDuration)
            {
                timer += Time.unscaledDeltaTime; // Unscaled so it works when paused
                color.a = Mathf.Lerp(0f, 1f, timer / titleFadeDuration);
                menuTitle.color = color;
                yield return null;
            }
        }
        else
        {
            // Fade from 1 → 0
            while (timer < titleFadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(1f, 0f, timer / titleFadeDuration);
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
}
