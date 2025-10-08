using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
   // [SerializeField] GameObject menuWin;

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

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("cancel"))
        {
           if(menuActive == null )
            {
              statePause();
                menuActive = menuPause;
                menuPause.SetActive(true);  

            }
           else if(menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }
    public void stateLose()
    {
        statePause();
        menuActive = menuLose;
        menuLose.SetActive(true);
    }
}
