using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

  
 
    [Header("Menus")]
    [SerializeField] GameObject menuRoot;
   // [SerializeField] GameObject background;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuLoseMatch;
    [SerializeField] GameObject menuLoseBreathe;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuCredits;


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
    [Header("Lose Video")]
    [SerializeField] private VideoPlayer loseVideoPlayer;
    [SerializeField] private GameObject loseVideoScreen;

    [Header("Match Count")]
    public TMP_Text matchCount;

    [Header("Hold Breath")]
    //public TMP_Text holdBreath;
    public GameObject holdingBreath;
    //public float timer;

    [Header("Hit Effect")]
    public GameObject hit;

    // --- Public read-only accessors ---
    public GameObject CurrentMenu => currentMenu;
    public Dictionary<string, GameObject> Menus => menus;
    [Header("Player")]
    public GameObject player;
    public PlayerMovement playerScript;

    public bool isPaused;
    float timeScaleOrig;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private UnityEngine.UI.Slider xSlider;
    [SerializeField] private UnityEngine.UI.Slider ySlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool cameraSettingsApplied = false;

    private void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        if(player != null) 
        playerScript = player.GetComponent<PlayerMovement>();

        //add any other menus to dictionary here
        menus.Add("Pause", menuPause);
        menus.Add("Lose", menuLose);
        menus.Add("LoseMatch", menuLoseMatch);
        menus.Add("LoseBreathe", menuLoseBreathe);
        menus.Add("Settings", menuSettings);
        menus.Add("Settings-Gameplay", settingsGameplay);

        menus.Add("Settings-Audio", settingsAudio);
        menus.Add("Credits", menuCredits);

        xSlider.minValue = 100;
        xSlider.maxValue = 1000;
        ySlider.minValue = 100;
        ySlider.maxValue = 1000;

        xSlider.value = 653;
        ySlider.value = 653;

        xSlider.onValueChanged.AddListener(OnXSliderChanged);
        ySlider.onValueChanged.AddListener(OnYSliderChanged);

    }

    void OnXSliderChanged(float newValue)
    {
        PlayerPrefs.SetInt("SensitivityX", (int)newValue);
        PlayerPrefs.Save();

        // Chỉ apply nếu CameraController tồn tại
        if (cameraController != null)
        {
            cameraController.SetSensitivityX((int)newValue);
        }
        else
        {
           // Debug.Log("CameraController not found, X sensitivity saved but not applied yet.");
        }
    }

    void OnYSliderChanged(float newValue)
    {
        PlayerPrefs.SetInt("SensitivityY", (int)newValue);
        PlayerPrefs.Save();

        if (cameraController != null)
        {
            cameraController.SetSensitivityY((int)newValue);
        }
        else
        {
           // Debug.Log("CameraController not found, Y sensitivity saved but not applied yet.");
        }
    }
    public void ApplyCameraSettings()
    {
        if (cameraController == null) return; // an toàn

        int sensX = PlayerPrefs.GetInt("SensitivityX", 653);
        int sensY = PlayerPrefs.GetInt("SensitivityY", 653);

        cameraController.SetSensitivityX(sensX);
        cameraController.SetSensitivityY(sensY);

       // Debug.Log($"Camera settings applied: X={sensX}, Y={sensY}");
    }


    // Update is called once per frame
    void Update()
    {
        if (!cameraSettingsApplied && cameraController != null)
        {
            ApplyCameraSettings();
            cameraSettingsApplied = true;
        }

        if ((Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.P) ))
        {
            if (currentMenu == menuLose) return;
            else if (currentMenu == null)
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
          //  Debug.LogWarning("Menu " + menuName + " does not exist!");
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
        if (menuRoot != null && menuName != "Lose" && menuName != "LoseBreathe" && menuName != "LoseMatch") 
            menuRoot.SetActive(true);



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
            xSlider.value = PlayerPrefs.GetInt("SensitivityX", 653);
            ySlider.value = PlayerPrefs.GetInt("SensitivityY", 653);

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
        if (isPaused && SceneManager.GetActiveScene().name != "MainMenu")
            stateUnpause();
        else if (isPaused && SceneManager.GetActiveScene().name == "MainMenu")
            unpauseTime();
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
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        if (AudioManager.instance != null)
            AudioManager.instance.PauseAllAudio();
    }
    public void unpauseTime()
    {
        Time.timeScale = timeScaleOrig;
        isPaused = false;
    }
    public void stateUnpause()
    {
        unpauseTime();
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        if (wasRadarOpenBefore) { ToggleRadarUI(); }
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            currentMenu = null;
        }

        if (AudioManager.instance != null)
            AudioManager.instance.ResumeAllAudio();
    }
    public void stateLose(int _loseType)
    {
       
        if (_loseType == 0)
        {
            StartCoroutine(PlayLoseVideoThenShowMenu());
        }
        else if (_loseType == 1)
        {
            OpenMenu("LoseMatch");
        }
        else if (_loseType == 2)
        {
            
            holdingBreath.SetActive(false);    
            OpenMenu("LoseBreathe");
            
        }
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
    private IEnumerator PlayLoseVideoThenShowMenu()
    {
        playerScript.enabled = false;
        if (currentMenu != null) currentMenu.SetActive(false);
        if (radarUI != null) radarUI.SetActive(false);

        if (AudioManager.instance != null)
            AudioManager.instance.PauseAllAudio();

        if (loseVideoScreen != null)
            loseVideoScreen.SetActive(true);
       

        if (loseVideoPlayer != null)
        {
            loseVideoPlayer.Play();
           // Debug.Log("Lose video started...");
           
            yield return new WaitForSeconds(2.9f);
            
        }

       // Debug.Log("Lose video finished!");

        if (loseVideoScreen != null)
            loseVideoScreen.SetActive(false);

     
      
        statePause();
        if (isRadarOpen) radarUI.SetActive(false);
        playerScript.enabled = true;
        OpenMenu("Lose");
    }
}
